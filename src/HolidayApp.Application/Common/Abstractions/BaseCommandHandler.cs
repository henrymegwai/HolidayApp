using HolidayApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Common.Abstractions;

/// <summary>
/// Base command handler that provides common functionality such as logging.
/// Validation is handled by the ValidationBehavior pipeline.
/// </summary>
/// <param name="logger"></param>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public abstract class BaseCommandHandler<TCommand, TResponse>(ILogger logger)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        // Log the start of handling
        logger.LogInformation("Handling {Command}", typeof(TCommand).Name);

        // Execute the specific logic
        var response = await HandleCommandAsync(request, cancellationToken);

        // Log the end of handling
        logger.LogInformation("Handled {Command}", typeof(TCommand).Name);

        return response;
    }

    /// <summary>
    /// Implement this method to handle the specific logic for the command.
    /// </summary>
    protected abstract Task<TResponse> HandleCommandAsync(TCommand request, CancellationToken cancellationToken);
}