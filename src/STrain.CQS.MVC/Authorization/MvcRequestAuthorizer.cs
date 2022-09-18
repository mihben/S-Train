using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.CQS.Api;
using System.Reflection;

namespace STrain.CQS.MVC.Authorization
{
    public class MvcRequestAuthorizer : IMvcRequestReceiver
    {
        private readonly IMvcRequestReceiver _requestReceiver;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MvcRequestAuthorizer> _logger;

        public MvcRequestAuthorizer(IMvcRequestReceiver requestReceiver, IHttpContextAccessor httpContextAccessor, ILogger<MvcRequestAuthorizer> logger)
        {
            _requestReceiver = requestReceiver;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IActionResult> ReceiveCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            _logger.LogDebug("Authorizing command");
            var authorizeResult = await command.AuthorizeAsync(_httpContextAccessor.HttpContext, _logger).ConfigureAwait(false);
            return authorizeResult ?? await _requestReceiver.ReceiveCommandAsync(command, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IActionResult> ReceiveQueryAsync<TQuery>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery
        {
            _logger.LogDebug("Authorizing query");
            var authorizeResult = await query.AuthorizeAsync(_httpContextAccessor.HttpContext, _logger).ConfigureAwait(false);
            return authorizeResult ?? await _requestReceiver.ReceiveQueryAsync(query, cancellationToken).ConfigureAwait(false);
        }
    }

    internal static class MvcRequestAuthorizerExtensions
    {
        public static IEnumerable<IAuthorizeData> GetAuthorizeData(this Type type) => type.GetCustomAttributes().Where(ca => ca is IAuthorizeData).Cast<IAuthorizeData>();
        public static bool IsAllowedAnonymus(this Type type) => type.GetCustomAttributes().Any(ca => ca is IAllowAnonymous);

        public static async Task<IActionResult?> AuthorizeAsync<TRequest>(this TRequest request, HttpContext context, ILogger<MvcRequestAuthorizer> logger)
        {
            var type = typeof(TRequest);
            var authorizeData = type.GetAuthorizeData();

            if (authorizeData.Any())
            {
                var policy = await AuthorizationPolicy.CombineAsync(context.RequestServices.GetRequiredService<IAuthorizationPolicyProvider>(), authorizeData);

                var policyEvaulator = context.RequestServices.GetRequiredService<IPolicyEvaluator>();
                var authenticateResult = await policyEvaulator.AuthenticateAsync(policy, context);

                if (type.IsAllowedAnonymus())
                {
                    logger.LogDebug("Anonymus authorization is allowed");
                    return null;
                }

                logger.LogTrace("Policy: {@policy}", policy);
                var authorizeResult = await policyEvaulator.AuthorizeAsync(policy, authenticateResult, context, request);

                if (authorizeResult.Challenged) return new ChallengeResult(policy.AuthenticationSchemes.ToArray());
                else if (authorizeResult.Forbidden) return new ForbidResult(policy.AuthenticationSchemes.ToArray());
            }
            else
            {
                logger.LogDebug("Authorize data not found");
            }

            return null;
        }
    }
}
