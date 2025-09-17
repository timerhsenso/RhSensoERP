namespace RhSensoERP.Application.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string PublicKeyPem { get; set; } = string.Empty;  // RS256 Public key
    public string PrivateKeyPem { get; set; } = string.Empty; // RS256 Private key (dev only)
    public string SecretKey { get; set; } = string.Empty;     // Symmetric key for development
    public int AccessTokenMinutes { get; set; } = 15;
}
