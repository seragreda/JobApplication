using FluentValidation;
using MediatR;
using JobApplication.Domain.Common;

namespace JobApplication.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var message = string.Join(" | ", failures.Select(f => f.ErrorMessage));

            // ·Ê «·‹ Response „‰ ‰Ê⁄ Result („‘ Generic)
            if (typeof(TResponse) == typeof(Result))
                return (TResponse)(object)Result.Fail(message, ErrorType.BadRequest);

            throw new ValidationException(failures);
        }

        return await next();
    }
}