using HolidayApp.Application.Common.Models;

namespace HolidayApp.Application.Common.Decorators;

public interface ICacheInvalidationDecorator<TRequest, TResponse>
{
    Task<Response<TResponse>> HandleAsync(TRequest request,
        Func<TRequest, CancellationToken, Task<Response<TResponse>>> next,
        CancellationToken cancellationToken);
}
