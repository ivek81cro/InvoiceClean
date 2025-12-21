using FluentValidation;
using InvoiceClean.Application.Common.Results;
using MediatR;

namespace InvoiceClean.Application.Common.Behaviors
{
    public sealed class ResultValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull 
        where TResponse : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ResultValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count == 0)
                return await next();

            var errors = failures
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).Distinct().ToArray()
                );

            var err = new Error(
                ErrorType.Validation,
                Code: "validation_failed",
                Message: "One or more validation errors occurred.",
                ValidationErrors: errors
            );

            // Kreiranje TResponse (Result ili Result<T>) preko reflectiona:
            // - za Result: Result.Fail(err)
            // - za Result<T>: Result<T>.Fail(err)
            var responseType = typeof(TResponse);

            if (responseType == typeof(Result))
                return (TResponse)(object)Result.Fail(err);

            // Result<T>
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var failMethod = responseType.GetMethod("Fail", new[] { typeof(Error) });
                if (failMethod is null)
                    throw new InvalidOperationException($"Missing Fail(Error) on {responseType.Name}");

                return (TResponse)failMethod.Invoke(null, new object[] { err })!;
            }

            throw new InvalidOperationException($"Unsupported result type: {responseType.FullName}");
        }
    }
}
