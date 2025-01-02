namespace GtKasse.Core.Models;

public sealed record UserTwoFactor(bool IsEnabled, string SecretKey, string AuthUri);
