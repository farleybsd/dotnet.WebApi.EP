using DevIo.Api.Extensions;
using DevIo.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
                return CustomerResponse(GerarJwt());
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
                return CustomerResponse(GerarJwt());
            }
            if (result.IsLockedOut)
            {
                NotificarErro("Usuario bloqueado por tentativas invalidas");
                return CustomerResponse(loginUser);
            }
            NotificarErro("Usuario ou senha incoretos");
            return CustomerResponse(loginUser);
        }

        private string GerarJwt()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)

            });
            var encodedToken = tokenHandler.WriteToken(token);

            return encodedToken;
        }
    }
}