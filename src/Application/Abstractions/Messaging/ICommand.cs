namespace MarketLens.Application.Abstractions.Messaging;

using MarketLens.Domain.Common;

public interface ICommand : ICommand<Result>;

public interface ICommand<TResponse>;
