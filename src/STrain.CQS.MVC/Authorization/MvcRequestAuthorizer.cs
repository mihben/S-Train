using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace STrain.CQS.MVC.Authorization
{
    public class MvcRequestAuthorizer : IMvcRequestReceiver
    {
        private readonly IMvcRequestReceiver _requestReceiver;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MvcRequestAuthorizer(IMvcRequestReceiver requestReceiver, IHttpContextAccessor httpContextAccessor)
        {
            _requestReceiver = requestReceiver;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> ReceiveAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : Command
        {
            var authorizeResult = await command.AuthorizeAsync(_httpContextAccessor.HttpContext).ConfigureAwait(false);
            return authorizeResult ?? await _requestReceiver.ReceiveAsync(command, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IActionResult> ReceiveAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken) where TQuery : Query<T>
        {
            var authorizeResult = await query.AuthorizeAsync(_httpContextAccessor.HttpContext).ConfigureAwait(false);
            return authorizeResult ?? await _requestReceiver.ReceiveAsync<TQuery, T>(query, cancellationToken).ConfigureAwait(false);
        }
    }

    internal static class MvcRequestAuthorizerExtensions
    {
        public static IEnumerable<IAuthorizeData> GetAuthorizeData(this Type type) => type.GetCustomAttributes().OfType<IAuthorizeData>();
        public static bool IsAllowedAnonymus(this Type type) => type.GetCustomAttributes().Any(ca => ca is IAllowAnonymous);

        public static async Task<IActionResult?> AuthorizeAsync<TRequest>(this TRequest request, HttpContext context)
        {
            var type = typeof(TRequest);
            var authorizeData = type.GetAuthorizeData();

            if (authorizeData.Any())
            {
                var policy = await AuthorizationPolicy.CombineAsync(context.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>(), authorizeData);

                var policyEvaulator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();
                var authenticateResult = await policyEvaulator.AuthenticateAsync(policy, context);

                if (type.IsAllowedAnonymus()) return null;

                var authorizeResult = await policyEvaulator.AuthorizeAsync(policy, authenticateResult, context, request);

                if (authorizeResult.Challenged) return new ChallengeResult(policy.AuthenticationSchemes.ToArray());
                else if (authorizeResult.Forbidden) return new ForbidResult(policy.AuthenticationSchemes.ToArray());
            }

            return null;
        }
    }
}
