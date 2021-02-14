using AutoMapper;
using FoodPal.Providers.Dtos;
using FoodPal.Providers.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FoodPal.Providers.API.Controllers
{
    [Route("api/providers/{providerId}/menu")]
    [ApiController]
    public class CatalogueItemsController : ControllerBase
    {
        private readonly ICatalogueItemService _catalogueItemService;
        private readonly IProviderService _providerService;
        private readonly IMapper _mapper;

        public CatalogueItemsController(ICatalogueItemService catalogueItemService, IProviderService providerService, IMapper mapper)
        {
            _catalogueItemService = catalogueItemService ?? throw new ArgumentNullException(nameof(catalogueItemService));
            _providerService = providerService ?? throw new ArgumentNullException(nameof(providerService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCatalogueItem(NewCatalogueItemDto catalogueItem)
        {
            try
            {
                if (catalogueItem.Name == string.Empty)
                {
                    ModelState.AddModelError(
                        "Name",
                        "The catalogue item name should not be empty!");
                }
                var providerId = await _catalogueItemService.GetProvidersIdForCatalogItemAsync(catalogueItem);
                if (await _catalogueItemService.CatalogueItemExists(catalogueItem.Name, providerId))
                {
                    ModelState.AddModelError(
                        "Name",
                        "A catalogue item with the same name already exists into the database!");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var insertedCatalogueItemId = await _catalogueItemService.Create(catalogueItem);

                if (insertedCatalogueItemId == 0)
                    return Problem();

                return CreatedAtRoute("GetCatalogueItem", new { providerId, itemId = insertedCatalogueItemId }, await _catalogueItemService.GetCatalogueItemById(insertedCatalogueItemId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCatalogueItems(int providerId)
        {
            try
            {
                var catalogItems = await _catalogueItemService.GetCatalogueItemsForProvider(providerId);

                return Ok(catalogItems);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpGet("{itemId}", Name = "GetCatalogueItem")]
        public async Task<IActionResult> GetCatalogueItem(int providerId, int itemId)
        {
            try
            {
                var catalogItem = (await _catalogueItemService.GetCatalogueItemsForProvider(providerId)).SingleOrDefault(x => x.Id == itemId);

                return Ok(catalogItem);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCatalogueItem(int id, [FromBody] CatalogueItemDto catalogueItem)
        {
            try
            {
                if (catalogueItem.Id != id)
                {
                    ModelState.AddModelError(
                        "Identifier",
                        "Request body not apropiate for ID");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _catalogueItemService.GetCatalogueItemById(id) == null)
                {
                    return NotFound();
                }

                await _catalogueItemService.Update(catalogueItem);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatalogueItem(int id)
        {
            try
            {
                if (await _catalogueItemService.GetCatalogueItemById(id) == null)
                {
                    return NotFound();
                }

                await _catalogueItemService.Delete(id);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to succeed the operation!");
            }
        }
    }
}