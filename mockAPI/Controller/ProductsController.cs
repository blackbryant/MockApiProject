using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mockAPI.DataContext;
using mockAPI.Models;
using mockAPI.Services;

namespace mockAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly ILogger<ProductsController> _logger;
        private readonly IProductReadService _productReadService;

        public ProductsController(IProductReadService productReadService, ILogger<ProductsController> logger)
        {
            _productReadService = productReadService!;
            _logger = logger!;
        }



        [HttpGet("{id}")]
        [ProducesResponseType(statusCode: 200, Type = typeof(ProductDTO))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 401, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 404, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetProduct(int id)
        {
            _logger.LogInformation($"Retrieving product with id {id}");
            try
            {
                var product = await _productReadService.GetById(id);
                if (product == null)
                {
                    return Problem(
                        detail: $"Product with ID {id} was not found.",
                        title: "Product not found",
                        statusCode: StatusCodes.Status404NotFound,
                        instance: HttpContext.TraceIdentifier
                        );
                }
                return Ok(product);

            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the product.");
                return Problem(
                    detail: ex.Message,
                    title: "Unauthorized Access",
                    statusCode: StatusCodes.Status401Unauthorized,
                    instance: HttpContext.TraceIdentifier
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the product.");
                return Problem(
                    detail: ex.Message,
                    title: "Internal Server Error",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.TraceIdentifier
                );
            }

        }

        [HttpGet]
        [ProducesResponseType(statusCode: 200, Type = typeof(IEnumerable<ProductDTO>))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 401, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(int pageSize, int? lastProductId = null)
        {
            _logger.LogInformation($"GetProducts called with pageSize: {pageSize}, lastProductId: {lastProductId}");

            var pagedResult = await _productReadService.GetPagedProductsAsync(pageSize, lastProductId);

            var previousPageUrl = pagedResult.HasPreviousPage ? Url.Action("GetProducts", new { pageSize, lastProductId = pagedResult.Items.First().Id })
                                                             : null;

            var nextPageUrl = pagedResult.HasNextPage ? Url.Action("GetProducts", new { pageSize, lastProductId = pagedResult.Items.Last().Id })
                                                             : null;

            var paginationMetadata = new
            {
                PageSize = pagedResult.PageSize,
                HasPreviousPage = pagedResult.HasPreviousPage,
                HasNextPage = pagedResult.HasNextPage,
                PreviousPageUrl = previousPageUrl,
                NextPageUrl = nextPageUrl,
                AveragePricePerCategory = pagedResult.AveragePriceByCategory
            };

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata, options));

            return Ok(pagedResult.Items);
        }

        [HttpGet("categories")]
        [ProducesResponseType(statusCode: 200, Type = typeof(IEnumerable<CategoryDTO>))]
        [ProducesResponseType(statusCode: 400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 401, Type = typeof(ProblemDetails))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetProductCategories()
        {
            _logger.LogInformation("GetProductCategories called");

            try
            {
                var categories = await _productReadService.GetCategories();
                if (categories == null || !categories.Any())
                {
                    return NoContent();
                }
                return Ok(categories); 
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product categories.");
                return Problem(
                    detail: ex.Message,
                    title: "Unauthorized Access",
                    statusCode: StatusCodes.Status401Unauthorized,
                    instance: HttpContext.TraceIdentifier
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product categories.");
                return Problem(
                    detail: ex.Message,
                    title: "Internal Server Error",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: HttpContext.TraceIdentifier
                );
            }
        }



    }
}