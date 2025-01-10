using FluentValidation.TestHelper;
using PromoCodes.Models;
using PromoCodes.Validators;

namespace PromoCodes.UnitTests.Validators;

    [TestFixture]
    public class GeneralRequestValidatorTests
    {
        private GeneralRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new GeneralRequestValidator();
        }

        [Test]
        public void Should_HaveError_When_CountIsBelowMinimal()
        {
            // Arrange
            var request = new GenerateRequest { Count = ConstValidationValues.MinimalCount - 1, Length = ConstValidationValues.MinimalCodeLength };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Count)
                .WithErrorMessage($"Count must be between {ConstValidationValues.MinimalCount} and {ConstValidationValues.MaximalCount}");
        }

        [Test]
        public void Should_HaveError_When_CountIsAboveMaximal()
        {
            // Arrange
            var request = new GenerateRequest { Count = ConstValidationValues.MaximalCount + 1, Length = ConstValidationValues.MinimalCodeLength };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Count)
                .WithErrorMessage($"Count must be between {ConstValidationValues.MinimalCount} and {ConstValidationValues.MaximalCount}");
        }

        [Test]
        public void Should_HaveError_When_LengthIsBelowMinimal()
        {
            // Arrange
            var request = new GenerateRequest { Count = ConstValidationValues.MinimalCount, Length = ConstValidationValues.MinimalCodeLength - 1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Length)
                .WithErrorMessage($"Length must be between {ConstValidationValues.MinimalCodeLength} and {ConstValidationValues.MaximalCodeLength}");
        }

        [Test]
        public void Should_HaveError_When_LengthIsAboveMaximal()
        {
            // Arrange
            var request = new GenerateRequest { Count = ConstValidationValues.MinimalCount, Length = ConstValidationValues.MaximalCodeLength + 1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Length)
                .WithErrorMessage($"Length must be between {ConstValidationValues.MinimalCodeLength} and {ConstValidationValues.MaximalCodeLength}");
        }

        [Test]
        public void Should_NotHaveError_When_ValuesAreWithinValidRange()
        {
            // Arrange
            var request = new GenerateRequest
            {
                Count = ConstValidationValues.MinimalCount,
                Length = ConstValidationValues.MinimalCodeLength
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Count);
            result.ShouldNotHaveValidationErrorFor(x => x.Length);
        }
    }