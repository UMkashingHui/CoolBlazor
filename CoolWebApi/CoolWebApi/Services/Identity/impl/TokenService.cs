using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using CoolWebApi.Config;
using CoolWebApi.Models.Identity;
using System.Security.Claims;
using CoolWebApi.Models.Requests.Identity;
using CoolWebApi.Models.Responses.Identity;
using CoolWebApi.Utils.Wrapper;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using MongoDB.Driver.Linq;

namespace CoolWebApi.Services.Identity.impl
{
    public class TokenService : ITokenService
    {
        private const string InvalidErrorMessage = "Invalid email or password.";

        private readonly UserManager<CoolBlazorUser> _userManager;
        private readonly RoleManager<CoolBlazorRole> _roleManager;
        private readonly AppConfiguration _appConfiguration;
        private readonly SignInManager<CoolBlazorUser> _signInManager;
        private readonly IStringLocalizer<TokenService> _localizer;
        // Current User Test
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(
            UserManager<CoolBlazorUser> userManager,
            RoleManager<CoolBlazorRole> roleManager,
            IOptions<AppConfiguration> appConfiguration,
            SignInManager<CoolBlazorUser> signInManager,
            ICurrentUserService currentUserService,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<TokenService> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfiguration = appConfiguration.Value;
            _signInManager = signInManager;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            }
            if (!user.IsActive)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Active. Please contact the administrator."]);
            }
            if (!user.EmailConfirmed)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["E-Mail not confirmed."]);
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Credentials."]);
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);
            var claims = await GetClaimsAsync(user);

            var token = await GenerateJwtAsync(user, claims);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl };
            // Current User Test
            // await SetCurrentUser(user, _httpContextAccessor);
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
        {
            if (model is null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
            }
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found."]);
            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token."]);
            var claims = await GetClaimsAsync(user);
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            // Current User Test
            await SetCurrentUser(user, _httpContextAccessor);
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        private async Task<string> GenerateJwtAsync(CoolBlazorUser user, IEnumerable<Claim> claims)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), claims);
            return token;
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(CoolBlazorUser user)
        {
            var userClaims = _userManager.GetClaimsAsync(user).Result;
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };
            var unionClaims = claims
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);
            return unionClaims;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            DateTime now = DateTime.Now;
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.AddDays(2),
            //    notBefore: now, // Current User Test
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false
            };
            // String2XmlReader
            StringReader strRdr = new StringReader(token);
            XmlReader rdr = XmlReader.Create(strRdr);
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(rdr, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(_localizer["Invalid token"]);
            }

            return principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            // System.Diagnostics.Debug.WriteLine("_appConfiguration.Secret = " + _appConfiguration.Secret);
            if (string.IsNullOrEmpty(_appConfiguration.Secret))
                throw new Exception("JWT secret not configured");
            var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret!);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// 设置当前登录用户
        /// </summary>
        private async Task SetCurrentUser(CoolBlazorUser user, IHttpContextAccessor httpContextAccessor)
        {
            CurrentUser.Configure(httpContextAccessor);

            // var user = await PSURepository.GetUserByOIDAsync(oid, context);
            // var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // var roles = await _userManager.GetRolesAsync(user);
                CurrentUser.UserId = user.Id.ToString();
                CurrentUser.UserName = user.UserName;
                // _currentUserService.UserId = user.Id.ToString();
                // CurrentUser.UserRole = roles.ToString();
            }
        }
    }

}

