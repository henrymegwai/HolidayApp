using MediatR;

namespace HolidayApp.Application.Common.Interfaces
{
    /// <summary>
    /// A handler for a query that returns a response of type TResponse.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IQueryHandler<in TQuery, TResponse>
        : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> where TResponse : notnull;
}
