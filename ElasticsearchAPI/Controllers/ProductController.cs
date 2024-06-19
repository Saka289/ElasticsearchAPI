using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticsearchAPI.Data;
using ElasticsearchAPI.Models;
using ElasticsearchAPI.SeedWork;
using ElasticsearchAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElasticsearchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IElasticSearchService<Product> _elasticSearchService;

        public ProductController(ApplicationDbContext context, IElasticSearchService<Product> elasticSearchService)
        {
            _context = context;
            _elasticSearchService = elasticSearchService;
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            await _elasticSearchService.IndexDocumentAsync("product", product, p => p.Id);
            return Ok(product);
        }
        
        [HttpPost("CreateProducts")]
        public async Task<IActionResult> CreateProducts(IEnumerable<Product> product)
        {
            await _context.Products.AddRangeAsync(product);
            await _context.SaveChangesAsync();
            await _elasticSearchService.IndexDocumentRangeAsync("product", product, p => p.Id);
            return Ok(product);
        }

        [HttpGet("SearchProduct")]
        public async Task<ActionResult<PagedList<Product>>> SearchProduct([FromQuery] string key,
            [FromQuery] string value, [FromQuery] PagingRequestParameters pagingRequestParameters)
        {
            var result = await _elasticSearchService.SearchAsync("product", pagingRequestParameters, key, value);
            return Ok(result);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _elasticSearchService.DeleteDocumentAsync("product", id);
            _context.Products.Remove(await _context.Products.FirstOrDefaultAsync(p => p.Id == id));
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var result = await _elasticSearchService.UpdateDocumentAsync("product", product, product.Id);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return Ok(result);
        }
    }
}