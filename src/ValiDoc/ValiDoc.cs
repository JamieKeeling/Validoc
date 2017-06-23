﻿using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using ValiDoc.Output;

namespace ValiDoc
{
    //TODO: This class is huge - should investigate splitting out dependencies
    // and relying on a DI container based on the consumer using the library.
    public static class ValiDoc
    {
        public static IEnumerable<RuleDescription> GetRules<T>(this AbstractValidator<T> validator, bool documentNested = false)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            var descriptor = validator.CreateDescriptor();

            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            var memberValidators = descriptor.GetMembersWithValidators();

            foreach (var member in memberValidators)
            {
                var rules = descriptor.GetRulesForMember(member.Key);

                foreach (var validationRule in rules)
                {
                    var rule = (PropertyRule)validationRule;
                    var propertyName = rule.GetDisplayName();

                    //TODO: Identify supplied parameters for bounds based on the validator (example Maximum of 20, etc..)
                    foreach (var validationRules in rule.Validators)
                    {
                        var documentedRules = BuildRuleDescription(validationRules, propertyName, rule.CascadeMode, documentNested, rule);

                        foreach (var documentedRule in documentedRules)
                        {
                            yield return documentedRule;
                        }
                    }
                }
            }
        }

        private static IEnumerable<RuleDescription> BuildRuleDescription(IPropertyValidator validationRules, string propertyName, CascadeMode cascadeMode, bool documentNested, PropertyRule rule)
        {
            string validatorName;
            Severity? validationFailureSeverity;

            if (validationRules is ChildValidatorAdaptor childValidator)
            {
                validatorName = childValidator.ValidatorType.Name;
                validationFailureSeverity = childValidator.Severity;

                if (documentNested)
                {
                    foreach (var ruleDescription in GetNestedRules(propertyName, rule, childValidator)) yield return ruleDescription;
                }
            }
            else
            {
                validatorName = validationRules.GetType().Name;
                validationFailureSeverity = validationRules.Severity;
            }

            yield return new RuleDescription
            {
                MemberName = propertyName,
                ValidatorName = validatorName,
                FailureSeverity = validationFailureSeverity.ToString(),
                OnFailure = cascadeMode.ToString()
            };
        }

        /// <summary>
        /// For a provided instance of AbstractValidator, recursively retrieves defined validation rules.
        /// </summary>
        private static IEnumerable<RuleDescription> GetNestedRules(string propertyName, PropertyRule rule, ChildValidatorAdaptor childValidator)
        {
            //TODO: This entire method is FAR too big. Needs to be leaner and testable in isolation.
            var runtimeMethods = typeof(ValiDoc).GetRuntimeMethods();

            MethodInfo generatedGetRules = null;

            // Nothing fancy for now, just pick the first option as we know it is GetRules
            using (var enumer = runtimeMethods.GetEnumerator())
            {
                if (enumer.MoveNext())
                    generatedGetRules = enumer.Current;
            }

            if (generatedGetRules == null)
                yield return null;

            // Create the generic method instance of GetRules()
            generatedGetRules = generatedGetRules.MakeGenericMethod(childValidator.ValidatorType.GetTypeInfo().BaseType.GenericTypeArguments[0]);

            //Parameter 1 = Validator instance derived from AbstractValidator<T>, Parameter 2 = boolean (documentNested)
            var parameterArray = new object[]
            {
                childValidator.GetValidator(new PropertyValidatorContext(new ValidationContext(rule.Member.DeclaringType), rule, propertyName)),
                true
            };

            //Invoke extension method with validator instance
            var nestedRules = generatedGetRules.Invoke(null, parameterArray) as IEnumerable<RuleDescription>;

            if (nestedRules == null)
                yield return null;

            foreach (var deepDocumentRule in nestedRules)
            {
                yield return deepDocumentRule;
            }
        }
    }
}