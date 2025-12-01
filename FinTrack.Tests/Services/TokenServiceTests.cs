using FinTrack.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinTrack.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly TokenService _tokenService;
    private readonly string _secretKey = "MyVerySecretKeyForTestingPurposesWithAtLeast32Characters!";
    private readonly string _validAudience = "https://localhost";
    private readonly string _validIssuer = "https://localhost";
    private readonly string _tokenValidityInMinutes = "60";

    public TokenServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _tokenService = new TokenService();

        // Setup default configuration values
        SetupConfiguration();
    }

    private void SetupConfiguration()
    {
        _mockConfiguration.Setup(c => c["Jwt:SecretKey"]).Returns(_secretKey);
        _mockConfiguration.Setup(c => c["Jwt:TokenValidityInMinutes"]).Returns(_tokenValidityInMinutes);
        _mockConfiguration.Setup(c => c["Jwt:ValidAudience"]).Returns(_validAudience);
        _mockConfiguration.Setup(c => c["Jwt:ValidIssuer"]).Returns(_validIssuer);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidJwtToken_WhenCalledWithValidClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "123")
        };

        // Act
        var token = _tokenService.GenerateAccessToken(claims, _mockConfiguration.Object);

        // Assert
        Assert.NotNull(token);
        Assert.IsType<JwtSecurityToken>(token);
        Assert.NotNull(token.RawData);
        Assert.Equal(_validAudience, token.Audiences.First());
        Assert.Equal(_validIssuer, token.Issuer);
        Assert.True(token.ValidTo > DateTime.UtcNow);
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Name && c.Value == "testuser");
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
        Assert.Contains(token.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "123");
    }

    [Fact]
    public void GenerateAccessToken_ShouldSetCorrectExpirationTime_WhenConfiguredWithValidityMinutes()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser")
        };
        var expectedValidityMinutes = 60;

        // Act
        var beforeGeneration = DateTime.UtcNow;
        var token = _tokenService.GenerateAccessToken(claims, _mockConfiguration.Object);
        var afterGeneration = DateTime.UtcNow;

        // Assert
        Assert.NotNull(token);
        Assert.True(token.ValidTo >= beforeGeneration.AddMinutes(expectedValidityMinutes));
        Assert.True(token.ValidTo <= afterGeneration.AddMinutes(expectedValidityMinutes).AddSeconds(1));
    }

    [Fact]
    public void GenerateAccessToken_ShouldThrowException_WhenSecretKeyIsNull()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns((string?)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GenerateAccessToken(claims, mockConfig.Object));

        Assert.Equal("Invalid Secret Key", exception.Message);
    }

    [Fact]
    public void GenerateAccessToken_ShouldSetIssuedAtAndNotBefore_ToCurrentUtcTime()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };

        // Act
        var beforeGeneration = DateTime.UtcNow;
        var token = _tokenService.GenerateAccessToken(claims, _mockConfiguration.Object);
        var afterGeneration = DateTime.UtcNow;

        // Assert
        Assert.NotNull(token);
        Assert.True(token.ValidFrom >= beforeGeneration && token.ValidFrom <= afterGeneration);
    }

    [Fact]
    public void GenerateAccessToken_ShouldUseHmacSha256Algorithm()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };

        // Act
        var token = _tokenService.GenerateAccessToken(claims, _mockConfiguration.Object);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(SecurityAlgorithms.HmacSha256, token.SignatureAlgorithm);
    }

    [Fact]
    public void GenerateAccessToken_ShouldHandleEmptyClaims()
    {
        // Arrange
        var emptyClaims = new List<Claim>();

        // Act
        var token = _tokenService.GenerateAccessToken(emptyClaims, _mockConfiguration.Object);

        // Assert
        Assert.NotNull(token);
        Assert.IsType<JwtSecurityToken>(token);
    }

    [Fact]
    public void GenerateAccessToken_ShouldHandleInvalidTokenValidityMinutes_DefaultToZero()
    {
        // Arrange
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns(_secretKey);
        mockConfig.Setup(c => c["Jwt:TokenValidityInMinutes"]).Returns("invalid");
        mockConfig.Setup(c => c["Jwt:ValidAudience"]).Returns(_validAudience);
        mockConfig.Setup(c => c["Jwt:ValidIssuer"]).Returns(_validIssuer);

        // Act
        var beforeGeneration = DateTime.UtcNow;
        var token = _tokenService.GenerateAccessToken(claims, mockConfig.Object);

        // Assert
        Assert.NotNull(token);
        // When parsing fails, int.TryParse returns 0, so expiration should be close to now
        Assert.True(token.ValidTo <= beforeGeneration.AddMinutes(1));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnNonEmptyString()
    {
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
        Assert.IsType<string>(refreshToken);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidBase64String()
    {
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(refreshToken);

        // Should be able to convert from Base64 without exception
        var bytes = Convert.FromBase64String(refreshToken);
        Assert.NotNull(bytes);
        Assert.Equal(128, bytes.Length); // 128 bytes as defined in the implementation
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnDifferentTokensOnMultipleCalls()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();
        var token3 = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(token1, token2);
        Assert.NotEqual(token2, token3);
        Assert.NotEqual(token1, token3);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldReturnClaimsPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        var token = _tokenService.GenerateAccessToken(claims, _mockConfiguration.Object);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(tokenString, _mockConfiguration.Object);

        // Assert
        Assert.NotNull(principal);
        Assert.IsType<ClaimsPrincipal>(principal);
        Assert.NotNull(principal.Identity);
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Name && c.Value == "testuser");
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldWorkWithExpiredToken()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns(_secretKey);
        mockConfig.Setup(c => c["Jwt:TokenValidityInMinutes"]).Returns("-10"); // Negative for expired token
        mockConfig.Setup(c => c["Jwt:ValidAudience"]).Returns(_validAudience);
        mockConfig.Setup(c => c["Jwt:ValidIssuer"]).Returns(_validIssuer);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser")
        };

        var token = _tokenService.GenerateAccessToken(claims, mockConfig.Object);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Act
        var principal = _tokenService.GetPrincipalFromExpiredToken(tokenString, mockConfig.Object);

        // Assert
        Assert.NotNull(principal);
        Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Name && c.Value == "testuser");
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldThrowException_WhenSecretKeyIsNull()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["Jwt:SecretKey"]).Returns((string?)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _tokenService.GetPrincipalFromExpiredToken("sometoken", mockConfig.Object));

        Assert.Equal("Invalid Secret Key", exception.Message);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldThrowException_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "invalid.token.string";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _tokenService.GetPrincipalFromExpiredToken(invalidToken, _mockConfiguration.Object));
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldThrowException_WhenTokenUsesWrongAlgorithm()
    {
        // Arrange
        // Create a token with a different algorithm (not HmacSha256)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); // Different algorithm

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Act & Assert
        var exception = Assert.Throws<SecurityTokenException>(() =>
            _tokenService.GetPrincipalFromExpiredToken(tokenString, _mockConfiguration.Object));

        Assert.Equal("Invalid Token", exception.Message);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ShouldThrowException_WhenTokenSignedWithDifferentKey()
    {
        // Arrange
        var differentKey = "DifferentSecretKeyForTestingPurposesWithAtLeast32Chars!";
        var mockConfigForGeneration = new Mock<IConfiguration>();
        mockConfigForGeneration.Setup(c => c["Jwt:SecretKey"]).Returns(differentKey);
        mockConfigForGeneration.Setup(c => c["Jwt:TokenValidityInMinutes"]).Returns(_tokenValidityInMinutes);
        mockConfigForGeneration.Setup(c => c["Jwt:ValidAudience"]).Returns(_validAudience);
        mockConfigForGeneration.Setup(c => c["Jwt:ValidIssuer"]).Returns(_validIssuer);

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "testuser") };
        var token = _tokenService.GenerateAccessToken(claims, mockConfigForGeneration.Object);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Act & Assert
        // Trying to validate with the original _mockConfiguration (different secret key)
        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
            _tokenService.GetPrincipalFromExpiredToken(tokenString, _mockConfiguration.Object));
    }

    [Fact]
    public void GenerateAccessToken_ShouldIncludeAllProvidedClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "John Doe"),
            new Claim(ClaimTypes.Email, "john@example.com"),
            new Claim(ClaimTypes.NameIdentifier, "12345"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("CustomClaim", "CustomValue")
        };

        // Act
        var token = _tokenService.GenerateAccessToken(claims, _mockConfiguration.Object);

        // Assert
        Assert.NotNull(token);
        Assert.Equal(5, token.Claims.Count(c =>
            c.Type == ClaimTypes.Name ||
            c.Type == ClaimTypes.Email ||
            c.Type == ClaimTypes.NameIdentifier ||
            c.Type == ClaimTypes.Role ||
            c.Type == "CustomClaim"));
    }
}