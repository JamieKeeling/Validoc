﻿using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using ValiDoc.Output;
using ValiDoc.Tests.TestData.Validators;
using Xunit;

namespace ValiDoc.Tests.Scenarios
{
    public class CasecadeModeTests
    {
        [Fact]
        public void ValiDoc_WithMultipleRuleWithGlobalCascadeValidator_OutputsMultipleRulesWithIdenticalCascade()
        {
            var validator = new MultipleRuleValidatorWithGlobalCascade();

            var validationRules = validator.GetRules().ToList();

            var expectedOutput = new List<RuleDescription>
            {
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    MemberName = "First Name",
                    OnFailure = "StopOnFirstFailure",
                    ValidatorName = "NotEmptyValidator"
                },
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    MemberName = "Last Name",
                    OnFailure = "StopOnFirstFailure",
                    ValidatorName = "NotEmptyValidator"
                },
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    MemberName = "Last Name",
                    OnFailure = "StopOnFirstFailure",
                    ValidatorName = "MaximumLengthValidator"
                }
            };

            validationRules.ShouldBeEquivalentTo(expectedOutput);
        }

        [Fact]
        public void ValiDoc_WithGlobalCascadeValidatorAndRuleOverride_OutputsMultipleRulesWithPOverriddenCascade()
        {
            var validator = new MultipleRuleValidatorWithMixedCascade();

            var validationRules = validator.GetRules().ToList();

            var expectedOutput = new List<RuleDescription>
            {
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    MemberName = "First Name",
                    OnFailure = "StopOnFirstFailure",
                    ValidatorName = "NotEmptyValidator"
                },
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    MemberName = "Last Name",
                    OnFailure = "Continue",
                    ValidatorName = "NotEmptyValidator"
                },
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    MemberName = "Last Name",
                    OnFailure = "Continue",
                    ValidatorName = "MaximumLengthValidator"
                }
            };

            validationRules.ShouldBeEquivalentTo(expectedOutput);
        }
    }
}
