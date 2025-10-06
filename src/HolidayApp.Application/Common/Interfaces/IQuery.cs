using MediatR;

namespace HolidayApp.Application.Common.Interfaces
{
    /// <summary>
    ///  Marker interface for a query with a response
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull;
}
