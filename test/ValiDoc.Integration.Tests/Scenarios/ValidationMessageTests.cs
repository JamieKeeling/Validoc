﻿using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using ValiDoc.CommonTest.TestData.Validators;
using ValiDoc.Core;
using ValiDoc.Output;
using ValiDoc.Utility;
using Xunit;

namespace ValiDoc.Integration.Tests.Scenarios
{
    public class ValidationMessageTests
    {
        [Fact]
        public void ValiDoc_WithMultipleRuleValidator_OutputMultipleRulesWithValidationMessage()
        {
            var validator = new MultipleRuleValidator();

	        var ruleGenerator = new DocBuilder(new RuleDescriptionBuilder(new ValidatorErrorMessageBuilder(new FluentValidationHelpers())));

            var validationRules = ruleGenerator.Document(validator).ToList();

            var expectedOutput = new List<RuleDescriptor>
            {
                new RuleDescriptor
                {
                    MemberName = "First Name",
                    Rules = new List<RuleDescription>
                    {
                        new RuleDescription
                        {
                            FailureSeverity = "Error",
                            OnFailure = "Continue",
                            ValidatorName = "NotNullValidator",
                            ValidationMessage = "'First Name' must not be empty."
                        }
                    }
                },
                new RuleDescriptor
                {
                    MemberName = "Last Name",
                    Rules = new List<RuleDescription>
                    {
                        new RuleDescription
                        {
                            FailureSeverity = "Error",
                            OnFailure = "Continue",
                            ValidatorName = "NotEmptyValidator",
                            ValidationMessage = "'Last Name' should not be empty."
                        },
                        new RuleDescription
                        {
                            FailureSeverity = "Error",
                            OnFailure = "Continue",
                            ValidatorName = "MinimumLengthValidator",
                            ValidationMessage = "'Last Name' must be more than {MinLength} characters. You entered {TotalLength} characters."
                        },
                        new RuleDescription
                        {
                            FailureSeverity = "Error",
                            OnFailure = "Continue",
                            ValidatorName = "MaximumLengthValidator",
                            ValidationMessage = "'Last Name' must be less than {MaxLength} characters. You entered {TotalLength} characters."
                        }
                    }
                }
            };

            validationRules.Should().HaveCount(2);
            validationRules.ShouldBeEquivalentTo(expectedOutput, options => options.WithStrictOrdering());
        }

		[Fact(Skip = "To be reviewed when this functionality is reinstated.")]
		public void ValiDoc_WithMultipleChildValidatorsAndDeepDocument_ReturnsRulesForAllChildValidatorsAndValidationMessages()
        {
            var validator = new MultipleRuleMultipleChildValidator();

	        var ruleGenerator = new DocBuilder(new RuleDescriptionBuilder(new ValidatorErrorMessageBuilder(new FluentValidationHelpers())));

			var validationRules = ruleGenerator.Document(validator).ToList();

            validationRules.Should().HaveCount(11);

            var expectedOutput = new List<RuleDescription>
            {
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "NotEmptyValidator",
					ValidationMessage = "'First Name' should not be empty."
				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "NotEmptyValidator",
					ValidationMessage = "'Last Name' should not be empty."
				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "MaximumLengthValidator",
					ValidationMessage = "'Last Name' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters."
				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "NotEmptyValidator",
	                ValidationMessage = "'House Number' should not be empty."
				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "NotEmptyValidator",
					ValidationMessage = "'Street Name' should not be empty."
				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "NotEmptyValidator",
	                ValidationMessage = "'Post Code' should not be empty."
				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "AddressValidator",
					ValidationMessage = null
                },
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "NotEqualValidator",
	                ValidationMessage = "'Employment Status' should not be equal to '{ComparisonValue}'."
                },
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "EnumValidator",
					ValidationMessage = "'Employment Status' has a range of values which does not include 'NotSet'."

				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "NotEmptyValidator",
	                ValidationMessage = "'Job Title' should not be empty."
				},
                new RuleDescription
                {
                    FailureSeverity = "Error",
                    OnFailure = "Continue",
                    ValidatorName = "OccupationDetailsValidator",
					ValidationMessage = null
                }
            };

            validationRules.ShouldBeEquivalentTo(expectedOutput, options => options.WithStrictOrdering());
        }
    }
}
