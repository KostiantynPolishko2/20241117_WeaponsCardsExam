using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using AdminPageServer.PL.DTO;
using AdminPageServer.PL.Infrastructures;
using AdminPageServer.PL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AdminPageServer.PL.Controllers
{
    [EnableCors("AllowAll")]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeaponsItemsController : ControllerBase
    {
        private readonly ILogger<WeaponsItemsController> logger;
        private readonly IWeaponsItemRepository weapons;

        public WeaponsItemsController(ILogger<WeaponsItemsController> logger, IWeaponsItemRepository weapons)
        {
            this.logger = logger;
            this.weapons = weapons;
        }

        [HttpGet("models", Name = "GetWeaponsItemsDto")]
        public ActionResult<IEnumerable<WeaponsItemDto>> GetWeaponsItemsDto() 
        {
            try
            {
                return Ok(weapons.getItemsDto());
            }
            catch (Exception ex) 
            {
                return BadRequest(getException(ex));
            }
        }

        [AllowAnonymous]
        [HttpGet("client-models", Name = "GetWeaponsCards")]
        public ActionResult<IEnumerable<WeaponsCardDto>> GetWeaponsCards()
        {
            try
            {
                return Ok(weapons.getCardsDto());
            }
            catch (Exception ex)
            {
                return BadRequest(getException(ex));
            }
        }

        [AllowAnonymous]
        [HttpGet("model/{model}", Name = "GetWeaponsCardDtoById")]
        public ActionResult<WeaponsCardDto> GetWeaponsCardDtoById([FromRoute] string? model)
        {         
            try
            {
                if (string.IsNullOrWhiteSpace(model))
                {
                    throw new ArgumentNullException($"no name of weapons");
                }

                return Ok(weapons.getCardDtoById(model));
            }
            catch (Exception ex)
            {
                return BadRequest(getException(ex));
            }
        }

        [HttpPost("new-model/{model}", Name = "PostNewWeaponsModel")]
        public IActionResult PostNewWeaponsModel([FromRoute] string? model, [FromBody] WeaponsDataDto weaponsDataDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model))
                {
                    throw new ArgumentNullException($"no name of weapons");
                }

                weapons.addNewWeaponsData(model, weaponsDataDto);

                return Ok(201);
            }
            catch (Exception ex)
            {
                return BadRequest(getException(ex));
            }
        }

        [HttpDelete("model/{model}", Name = "DeleteWeaponsModel")]
        public IActionResult DeleteWeaponsModel([FromRoute] string? model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model))
                {
                    throw new ArgumentNullException($"no name of weapons");
                }

                weapons.deleteWeaponsData(model);

                return Ok(201);
            }
            catch (Exception ex)
            {
                return BadRequest(getException(ex));
            }
        }

        private string getException(Exception ex)
        {
            if (ex is WeaponsException weaponsEx)
            {
                var _ex = (ex as WeaponsException);
                return $"Error! msg: {weaponsEx.Message} {weaponsEx.property}, source: {weaponsEx.Source}";
            }
            return $"Error! msg: {ex.Message}, details: {ex.InnerException}";
        }
    }
}
