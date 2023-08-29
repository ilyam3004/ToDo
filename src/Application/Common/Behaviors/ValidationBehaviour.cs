﻿using LanguageExt.Common;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : 
    IPipelineBehavior<TRequest, Result<TResponse>> 
    where TRequest : notnull
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehavior(IValidator<TRequest>? validator)
    {
        _validator = validator;
    }
    //
    // public async Task<Result<TResponse, Exception>> Handle(
    //     TRequest request, 
    //     RequestHandlerDelegate<Result<TResponse, Exception>> next, 
    //     CancellationToken cancellationToken)
    // {
    //     if (_validator is null)
    //     {
    //         return await next();
    //     }
    //
    //     var validationResult = await _validator.ValidateAsync(request, cancellationToken);
    //     if (validationResult.IsValid)
    //     {
    //         return await next();
    //     }
    //     
    //     return new ValidationException(validationResult.Errors);
    // }

    public async Task<Result<TResponse>> Handle(
        TRequest request, 
        RequestHandlerDelegate<Result<TResponse>> next, 
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await next();
        }

        var exception = new ValidationException(validationResult.Errors);
        
        return new Result<TResponse>(exception);
    }
} 