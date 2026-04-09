using System;
using System.Reflection;
using MySecureBackend.WebApi.Controllers;
using Xunit;

namespace MySecureBackend.XUnitTests.AccountControllerTests
{
    public class AccountControllerValidationTests
    {
        private static MethodInfo GetPrivateStatic(string name) =>
            typeof(AccountController).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Method {name} not found");

        // Helper to call IsPasswordValid(string, out string)
        private static (bool valid, string error) CallIsPasswordValid(string password)
        {
            var method = GetPrivateStatic("IsPasswordValid");
            var args = new object?[] { password, null };
            var result = (bool)method.Invoke(null, args)!;
            var error = args[1] as string ?? string.Empty;
            return (result, error);
        }

        [Fact]
        public void ValidUsernameAndPassword_ReturnsTrue()
        {
            // Arrange
            var username = "User123";
            var password = "Aa1!aaaaaa"; // 10 chars, upper, lower, digit, non-alnum

            // Act
            var usernameMethod = GetPrivateStatic("IsUsernameValid");
            var isUsernameValid = (bool)usernameMethod.Invoke(null, new object[] { username })!;
            var (isPasswordValid, passwordError) = CallIsPasswordValid(password);

            // Assert
            Assert.True(isUsernameValid);
            Assert.True(isPasswordValid);
            Assert.True(string.IsNullOrEmpty(passwordError));
        }

        [Fact]
        public void Username_WithInvalidCharacters_ReturnsFalse()
        {
            // Arrange
            var username = "User 123!"; // contains space and punctuation

            // Act
            var usernameMethod = GetPrivateStatic("IsUsernameValid");
            var isUsernameValid = (bool)usernameMethod.Invoke(null, new object[] { username })!;

            // Assert
            Assert.False(isUsernameValid);
        }

        [Fact]
        public void Password_ExactlyMinimumLength_BoundaryCase()
        {
            // Arrange
            var password = "Aa1!aaaaaa"; // exactly 10 characters satisfying policy

            // Act
            var (isValid, error) = CallIsPasswordValid(password);

            // Assert
            Assert.True(isValid);
            Assert.True(string.IsNullOrEmpty(error));
        }

        [Fact]
        public void Username_Empty_ReturnsFalse()
        {
            // Arrange
            var username = string.Empty;

            // Act
            var usernameMethod = GetPrivateStatic("IsUsernameValid");
            var isUsernameValid = (bool)usernameMethod.Invoke(null, new object[] { username })!;

            // Assert
            Assert.False(isUsernameValid);
        }

        [Fact]
        public void Password_MissingNonAlphanumeric_ReturnsFalseAndContainsMessage()
        {
            // Arrange
            var password = "Aa1aaaaaaa"; // missing non-alphanumeric character

            // Act
            var (isValid, error) = CallIsPasswordValid(password);

            // Assert
            Assert.False(isValid);
            Assert.Contains("non-alphanumeric", error, StringComparison.OrdinalIgnoreCase);
        }
    }
}
