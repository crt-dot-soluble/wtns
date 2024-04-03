namespace WTNS;

public static class HashingService
{
    /// <summary>
    /// <para>
    /// Hashes string using BCrypt with a custom cost factor.
    /// </para>
    /// <para>
    /// Pepper will be set to the value of the environment variable `WTNS_PEPPER`
    /// </para>
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="costFactor">The cost factor for BCrypt. Higher values increase computation time.</param>
    /// <returns>The hashed password.</returns>
    public static (string hashedPassword, string salt) CreateHash(
        string password,
        int costFactor = 12
    )
    {
        // Generate a salt
        var salt = BCrypt.Net.BCrypt.GenerateSalt(costFactor);

        // Combine password with pepper before hashing
        var combinedPassword =
            (
                ConfigurationRegistry.Instance.CurrentConfiguration.Pepper != null
                && ConfigurationRegistry.Instance.CurrentConfiguration.Pepper.Length > 0
            )
                ? password + ConfigurationRegistry.Instance.CurrentConfiguration.Pepper
                : password + ConfigurationRegistry.DefaultConfiguration.Pepper;

        // Hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(combinedPassword, salt);

        return (hashedPassword, salt);
    }

    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>True if the password matches the hashed password, false otherwise.</returns>
    public static bool VerifyHash(string password, string hashedPassword)
    {
        // Combine password with pepper before hashing
        var combinedPassword =
            password + ConfigurationRegistry.Instance.CurrentConfiguration.Pepper;

        // Use BCrypt to verify the password
        var isValid = BCrypt.Net.BCrypt.Verify(combinedPassword, hashedPassword);

        return isValid;
    }
}
