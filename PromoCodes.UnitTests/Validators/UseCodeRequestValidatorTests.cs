using FluentValidation.TestHelper;
using PromoCodes.Models;
using PromoCodes.Validators;

namespace PromoCodes.UnitTests.Validators;

[TestFixture]
    public class UseCodeRequestValidatorTests
    {
        private UseCodeRequestValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new UseCodeRequestValidator();
        }

        [Test]
        public void Should_HaveError_When_CodeIsNull()
        {
            // Arrange
            var request = new UseCodeRequest { Code = null };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Code)
                .WithErrorMessage("Code is required.");
        }

        [Test]
        public void Should_HaveError_When_CodeIsEmpty()
        {
            // Arrange
            var request = new UseCodeRequest { Code = string.Empty };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Code)
                .WithErrorMessage("Code could not be empty.");
        }

        [Test]
        public void Should_HaveError_When_CodeIsTooShort()
        {
            // Arrange
            var request = new UseCodeRequest { Code = new string('A', ConstValidationValues.MinimalCodeLength - 1) };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Code)
                .WithErrorMessage($"Code length must be between {ConstValidationValues.MinimalCodeLength} and {ConstValidationValues.MaximalCodeLength}");
        }

        [Test]
        public void Should_HaveError_When_CodeIsTooLong()
        {
            // Arrange
            var request = new UseCodeRequest { Code = new string('A', ConstValidationValues.MaximalCodeLength + 1) };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Code)
                .WithErrorMessage($"Code length must be between {ConstValidationValues.MinimalCodeLength} and {ConstValidationValues.MaximalCodeLength}");
        }

        [Test]
        public void Should_NotHaveError_When_CodeIsValid()
        {
            // Arrange
            var request = new UseCodeRequest { Code = new string('A', ConstValidationValues.MinimalCodeLength) };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Code);
        }
    }