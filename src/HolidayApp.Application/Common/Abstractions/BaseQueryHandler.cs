using HolidayApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace HolidayApp.Application.Common.Abstractions;

/// <summary>
/// Base class for handling Queries with logging.
/// Validation is handled by the ValidationBehavior pipeline.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="logger"></param>
public abstract class BaseQueryHandler<TQuery, TResponse>(ILogger logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    /// <inheritdoc/>
    public async Task<TResponse> Handle(TQuery request, CancellationToken cancellationToken)
    {
        // Log the start of handling
        logger.LogInformation("Handling {Query}", typeof(TQuery).Name);

        // Execute the specific logic
        var response = await this.HandleQueryAsync(request, cancellationToken);

        // Log the end of handling
        logger.LogInformation("Handled {Query}", typeof(TQuery).Name);

        return response;
    }

    /// <summary>
    /// Implement this method to handle the specific logic for the Query.
    /// </summary>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    protected abstract Task<TResponse> HandleQueryAsync(TQuery request, CancellationToken cancellationToken);
}
