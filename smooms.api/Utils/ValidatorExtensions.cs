using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace smooms.api.Utils;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidEmailAddress<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$")
            .MaximumLength(320)
            .WithMessage("'{PropertyName}' is not a valid email address.");
    }
}