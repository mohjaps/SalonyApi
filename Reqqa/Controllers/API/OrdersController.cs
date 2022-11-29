using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Salony.PathUrl;
using Salony.ViewModels.Workers;
using System;
using System.Threading.Tasks;

namespace Salony.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // [ApiExplorerSettings(GroupName = "MobileApi")]
    public class OrdersController : Controller
    {
        [HttpGet(ApiRoutes.Client.CancelOrder)]
        [ProducesResponseType(200, Type = typeof(ApiResult<String>))]
        [ProducesResponseType(400, Type = typeof(ApiResult<string>))]
        public async Task<IActionResult> CancelOrder(NewEvaluation model)
        {
            return Ok();
        }
    }
}
