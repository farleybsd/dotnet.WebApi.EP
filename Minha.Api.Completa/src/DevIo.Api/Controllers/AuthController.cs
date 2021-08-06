using DevIo.Api.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevIo.Api.Controllers
{
    [Route("api")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AuthController(INotificador notificador,
                              SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager) : base(notificador)
        {

            _signInManager = signInManager;
            _userManager = userManager;
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
                return CustomerResponse(registerUser);
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
                return CustomerResponse(loginUser);
            }
            if (result.IsLockedOut)
            {
                NotificarErro("Usuario bloqueado por tentativas invalidas");
                return CustomerResponse(loginUser);
            }
            NotificarErro("Usuario ou senha incoretos");
            return CustomerResponse(loginUser);
        }
    }
}