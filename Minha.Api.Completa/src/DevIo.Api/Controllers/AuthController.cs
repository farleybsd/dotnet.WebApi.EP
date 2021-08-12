using DevIo.Api.Extensions;
using DevIo.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DevIo.Api.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        public AuthController(INotificador notificador,
                              SignInManager<IdentityUser> signInManager,
                              IOptions<AppSettings> appSettings,
                              UserManager<IdentityUser> userManager) : base(notificador)
        {

            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Register(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid) return CustomerResponse(ModelState);

            var user = new IdentityUser
            {

                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return CustomerResponse(await GerarJwt(user.Email));
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }
            return CustomerResponse(registerUser);
        }

        [HttpPost("entrar")]
        public async Task<ActionResult> Login(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid) return CustomerResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(loginUser.Email,
                                                                   loginUser.Password,
                                                                   false,
                                                                   true);
            if (result.Succeeded)
            {
                return CustomerResponse(await GerarJwt(loginUser.Email));
            }
            if (result.IsLockedOut)
            {
                NotificarErro("Usuario bloqueado por tentativas invalidas");
                return CustomerResponse(loginUser);
            }
            NotificarErro("Usuario ou senha incoretos");
            return CustomerResponse(loginUser);
        }

        //private string GerarJwt()
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var Key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //    var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        //    {
        //        Issuer = _appSettings.Emissor,
        //        Audience = _appSettings.ValidoEm,
        //        Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)

        //    });
        //    var encodedToken = tokenHandler.WriteToken(token);

        //    return encodedToken;
        //}

        private async Task<string>GerarJwt(string Email)
        {
            // adicinando clains no jwt
            var user = await _userManager.FindByEmailAsync(Email);
            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);


            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
           
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            var tokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)

            });
            var encodedToken = tokenHandler.WriteToken(token);

            return encodedToken;
        }
        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}