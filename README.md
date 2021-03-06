# ValiDoc [![Build Status](https://travis-ci.org/JamieKeeling/ValiDoc.svg?branch=master)](https://travis-ci.org/JamieKeeling/ValiDoc)

An unobtrusive extension to FluentValidations for documenting validation rules.

##

#### Usage

1. Install ValiDoc via NuGet

```<language>
Install-Package ValiDoc -Pre
```

2. Configure Dependency Injection
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add ValiDoc dependencies
    services.AddValiDoc();
}
```

3. Reference ValiDoc namespace 

```csharp
using ValiDoc;
```

4. Constructor Inject the RuleDescriptor

```csharp
private readonly IRuleBuilder _ruleBuilder;

public ValidationController(IRuleBuilder ruleBuilder)
{
    _ruleBuilder = ruleBuilder;
}
```

4. Create an instance of DocBuilder that will be used to build the documentation

```csharp
var docBuilder = new DocBuilder(_ruleBuilder);
```

5. Invoke the .Document() method on the DocBuilder, passing in an implementation of AbstractValidator\<T> 

```csharp
var ruleDocumentation = docBuilder.Document(myValidatorInstance);
```


##

#### Example

##### Implementation of AbstractValidator&lt;T&gt;

```csharp
public class MultipleRuleSingleChildValidator : AbstractValidator<Person>
{
	public MultipleRuleSingleChildValidator()
	{
	    RuleFor(p => p.FirstName).NotEmpty();
	    RuleFor(p => p.LastName).NotEmpty().MaximumLength(20);
        RuleFor(p => p.Address).SetValidator(new AddressValidator());
	}
}
```
  
##### Dependency Injection and Invocation

```csharp
[Route("api/[controller]")]
public class ValidationController : Controller
{
    private readonly IRuleBuilder _ruleBuilder;

    public ValidationController(IRuleBuilder ruleBuilder)
    {
        _ruleBuilder = ruleBuilder;
    }

    [HttpGet]
    public IEnumerable<RuleDescriptor> ValidatorDocumentation()
    {
        var validator = new MultipleRuleSingleChildValidator();

        var docBuilder = new ValiDoc.DocBuilder(_ruleBuilder);

        return docBuilder.Document(validator);
    }
}
```


##### Output

```json
[
    {
        "memberName": "First Name",
        "rules": [
            {
                "validatorName": "NotEmptyValidator",
                "failureSeverity": "Error",
                "onFailure": "Continue",
                "validationMessage": "'First Name' should not be empty."
            }
        ]
    },
    {
        "memberName": "Last Name",
        "rules": [
            {
                "validatorName": "NotEmptyValidator",
                "failureSeverity": "Error",
                "onFailure": "Continue",
                "validationMessage": "'Last Name' should not be empty."
            },
            {
                "validatorName": "MaximumLengthValidator",
                "failureSeverity": "Error",
                "onFailure": "Continue",
                "validationMessage": "'Last Name' must be less than {MaxLength} characters. You entered {TotalLength} characters."
            }
        ]
    },
    {
        "memberName": "Address",
        "rules": [
            {
                "validatorName": "AddressValidator",
                "failureSeverity": "Error",
                "onFailure": "Continue",
                "validationMessage": "N/A - Refer to specific AddressValidator documentation"
            }
        ]
    }
]
```

## 
#### Supported Scenarios

1. Extraction of validation messages (Configured argument values not included)
2. [Built in validators](https://github.com/JeremySkinner/FluentValidation/wiki/c.-Built-In-Validators)
3. [Chained validators for the same property](https://github.com/JeremySkinner/FluentValidation/wiki/b.-Creating-a-Validator#chaining-validators-for-the-same-property)
4. [Complex properties](https://github.com/JeremySkinner/FluentValidation/wiki/b.-Creating-a-Validator#complex-properties)
5. [Cascade behaviour](https://github.com/JeremySkinner/FluentValidation/wiki/d.-Configuring-a-Validator#setting-the-cascade-mode)


#### Future Roadmap

1. Collections
2. RuleSets
3. .WithMessage()
4. .WithName()
5. Validation arguments (Greater than 10 characters for example)
6. Custom validators
7. .Must()
