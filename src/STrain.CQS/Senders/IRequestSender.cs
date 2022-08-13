﻿namespace STrain.CQS.Senders
{
    public interface IRequestSender
    {
        Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command;
        Task<T?> GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T>;
    }
}
