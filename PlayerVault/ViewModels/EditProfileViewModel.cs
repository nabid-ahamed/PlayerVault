using System.ComponentModel.DataAnnotations;

namespace PlayerVault.ViewModels
{
    public class EditProfileViewModel : IValidatableObject
    {
        public string CurrentEmail { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool anyPasswordEntered =
                !string.IsNullOrWhiteSpace(CurrentPassword) ||
                !string.IsNullOrWhiteSpace(NewPassword) ||
                !string.IsNullOrWhiteSpace(ConfirmPassword);

            if (anyPasswordEntered)
            {
                if (string.IsNullOrWhiteSpace(CurrentPassword))
                {
                    yield return new ValidationResult("Current password is required to change password.", new[] { nameof(CurrentPassword) });
                }

                if (string.IsNullOrWhiteSpace(NewPassword))
                {
                    yield return new ValidationResult("New password is required.", new[] { nameof(NewPassword) });
                }

                if (string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    yield return new ValidationResult("Please confirm your new password.", new[] { nameof(ConfirmPassword) });
                }
            }
        }

    }
}
