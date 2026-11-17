using Application;
using Application.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IValidator<ApplicationProductModel> _productValidator;
        private readonly IValidator<ProductsRequest> _requestValidator;

        public ProductsController(IProductService productService,
                                  IValidator<ApplicationProductModel> productValidator,
                                  IValidator<ProductsRequest> requestValidator)
        {
            _productService = productService;
            _productValidator = productValidator;
            _requestValidator = requestValidator;
        }

        [HttpPost]
        public IActionResult Create([FromBody] ApplicationProductModel product)
        {
            _productValidator.ValidateAndThrow(product);
            var id = _productService.CreateProduct(product);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] ApplicationProductModel product)
        {
            _productValidator.ValidateAndThrow(product);

            _productService.UpdateProduct(id, product);

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            var product = _productService.GetById(id);
            return Ok(product);
        }

        [HttpGet]
        public IActionResult GetList([FromQuery] ProductsRequest request)
        {
            _requestValidator.ValidateAndThrow(request);

            var products = _productService.GetFilteredProducts(
                request.id,
                request.productName,
                request.priceFrom,
                request.priceTo,
                request.weightFrom,
                request.weightTo,
                request.type,
                request.warehouseNumber,
                request.createdFrom,
                request.createdTo,
                request.pageNumber,
                request.pageSize);

            return Ok(products);
        }
    }
}