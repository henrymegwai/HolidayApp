using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Common.Abstractions;

/// <summary>
///  Base class for decorated command handlers with logging capabilities.
/// </summary>
/// <param name="logger"></param>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class BaseDecoratedCommandHandler<TRequest, TResponse>(ILogger logger)
    : ICommandHandler<TRequest, Response<TResponse>>
    where TRequest : ICommand<Response<TResponse>>
    where TResponse : notnull
{
    protected readonly ILogger Logger = logger;

    /// <summary>
    ///  Handles the command by delegating to the abstract HandleCommandAsync method.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Response<TResponse>> Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        return await HandleCommandAsync(request, cancellationToken);
    }

    /// <summary>
    ///  Abstract method to be implemented by derived classes to handle the command logic.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<Response<TResponse>> HandleCommandAsync(TRequest request, CancellationToken cancellationToken);
}
