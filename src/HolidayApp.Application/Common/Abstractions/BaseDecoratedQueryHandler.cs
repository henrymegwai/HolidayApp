using HolidayApp.Application.Common.Interfaces;
using HolidayApp.Application.Common.Models;

namespace HolidayApp.Application.Common.Abstractions;

public abstract class BaseDecoratedQueryHandler<TRequest, TResponse> : IQueryHandler<TRequest, Response<TResponse>>
    where TRequest : IQuery<Response<TResponse>>
    where TResponse : notnull
{
    public async Task<Response<TResponse>> Handle(TRequest request, CancellationToken cancellationToken = default)
    {
        return await HandleQueryAsync(request, cancellationToken);
    }

    protected abstract Task<Response<TResponse>> HandleQueryAsync(TRequest request, CancellationToken cancellationToken);
}
