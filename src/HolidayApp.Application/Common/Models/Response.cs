using FluentResults;

namespace HolidayApp.Application.Common.Models;

public record Response<T>(bool Status, T Data, string Message, Result Error = null!);