using FluentValidation.Results;

namespace SuperMarket.Application.Common;

public static class ValidationResultExtensions
{
    public static Result ToResult(this ValidationResult validationResult)
    {
        return validationResult.IsValid
            ? Result.Success()
            : Result.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
    }

    public static Result<T> ToFailureResult<T>(this ValidationResult validationResult)
    {
        return Result<T>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
    }
}
