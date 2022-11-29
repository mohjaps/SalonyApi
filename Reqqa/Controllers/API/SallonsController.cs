using Core.TableDb;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Salony.PathUrl;
using Salony.ViewModels.Workers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Salony.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiExplorerSettings(GroupName = "MobileApi")]
    public class SallonsController : Controller
    {
        private readonly UserManager<ApplicationDbUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SallonsController(
                UserManager<ApplicationDbUser> userManager,
                ApplicationDbContext context
            )
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet(ApiRoutes.Client.NewSallonEvaluation)]
        [ProducesResponseType(200, Type = typeof(ApiResult<String>))]
        [ProducesResponseType(400, Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> NewEvaluation(NewEvaluation model)
        {
            ApiResult<string> Result = new ApiResult<String>
            {
                Errors = new List<String>()
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                ApplicationDbUser Sallon = await _userManager.FindByIdAsync(model.UserID);
                if (Sallon is null || Sallon.typeUser != (int)Enums.AllEnums.TypeUser.provider)
                {
                    Result.Errors.Add("Invalid Sallon Id");
                    return BadRequest(Result);
                }

                ApplicationDbUser user = await _userManager.FindByIdAsync(model.SenderID);
                if (user is null)
                {
                    Result.Errors.Add("Invalid User Id");
                    return BadRequest(Result);
                }
                SallonEvaluation data = new SallonEvaluation()
                {
                    UserID = model.SenderID,
                    SallonID = model.UserID,
                    Points = model.Points,
                    Comment = model.Comment,
                };
                _context.SallonEvaluations.Add(data);
                int result = await _context.SaveChangesAsync();
                if (result >= 0)
                {
                    Result.Result = true;
                    return Ok(Result);
                }
                throw new Exception();
            }
            catch (Exception)
            {
                return BadRequest(new ApiResult<String>
                {
                    Result = false,
                    Errors = new List<String>() { "Un Expected Error" }
                });
            }
        }
    }
}
