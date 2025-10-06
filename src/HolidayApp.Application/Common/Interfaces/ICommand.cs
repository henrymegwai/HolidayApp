using MediatR;

namespace HolidayApp.Application.Common.Interfaces
{
    /// <summary>
    ///   Marker interface for a command with no return value.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface ICommand<out TResponse>: IRequest<TResponse>;
}
