using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Models;
using PRN232.Lab2.CoffeeStore.Services.AddonService;
using PRN232.Lab2.CoffeeStore.Services.Models;

namespace PRN232.Lab2.CoffeeStore.API.Controllers
{
    [Route("api/addons")]
    public class AddonController : BaseController
    {
        private readonly IAddonService _addonService;

        public AddonController(IAddonService addonService)
        {
            _addonService = addonService;
        }

        // GET: api/addons
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CoffeeAddonResponse>>>> GetAllAddons([FromQuery] bool? isActive = null)
        {
            var addons = await _addonService.GetAllAddonsAsync(isActive);
            return Ok(addons);
        }
    }
}

