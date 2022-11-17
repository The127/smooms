using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace smooms.api.Utils;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> EmailAddressOrEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(s => string.IsNullOrEmpty(s) || new EmailAddressAttribute().IsValid(s))
            .WithMessage("'{PropertyName}' is not a valid email address.");
    }
}