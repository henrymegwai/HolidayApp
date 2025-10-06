using MediatR;

namespace HolidayApp.Application.Common.Interfaces
{
    /// <summary>
    ///  Marker interface for a command handler
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface ICommandHandler<in TCommand, TResponse>
        : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse> where TResponse: notnull;
}
