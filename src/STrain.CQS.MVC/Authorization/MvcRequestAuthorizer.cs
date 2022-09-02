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
            var type = typeof(TCommand);
            var authorizeData = type.GetAuthorizeData();

            if (authorizeData.Any())
            {
                var policy = await AuthorizationPolicy.CombineAsync(_httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>(), authorizeData);

                var policyEvaulator = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
                var authenticateResult = await policyEvaulator.AuthenticateAsync(policy, _httpContextAccessor.HttpContext);

                if (type.IsAllowedAnonymus()) return await _requestReceiver.ReceiveAsync(command, cancellationToken).ConfigureAwait(false);

                var authorizeResult = await policyEvaulator.AuthorizeAsync(policy, authenticateResult, _httpContextAccessor.HttpContext, command);

                if (authorizeResult.Challenged) return new ChallengeResult(policy.AuthenticationSchemes.ToArray());
                else if (authorizeResult.Forbidden) return new ForbidResult(policy.AuthenticationSchemes.ToArray());
            }

            return await _requestReceiver.ReceiveAsync(command, cancellationToken).ConfigureAwait(false);
        }

        public Task<IActionResult> ReceiveAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken) where TQuery : Query<T>
        {
            throw new NotImplementedException();
        }
    }

    internal static class MvcRequestAuthorizerExtensions
    {
        public static IEnumerable<IAuthorizeData> GetAuthorizeData(this Type type) => type.GetCustomAttributes().OfType<IAuthorizeData>();
        public static bool IsAllowedAnonymus(this Type type) => type.GetCustomAttributes().Any(ca => ca is IAllowAnonymous);
    }
}
