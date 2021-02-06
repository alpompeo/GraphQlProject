using GraphQL;
using GraphQL.NewtonsoftJson;
using GraphQlProject.Interfaces;
using GraphQlProject.Models;
using GraphQlProject.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphQlProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private IProduct _productsSerrvice;

        public ProductsController(IProduct productServices)
        {
            _productsSerrvice = productServices;
        }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            //Random _random = new Random();

            //for (int i = 1; i < 6000000; i++)
            //{
            //    _productsSerrvice.AddProduct(new Product() { Id = i, Name = Guid.NewGuid().ToString(), Price = _random.Next(2, 565677) });
            //}


            return _productsSerrvice.GetAllProducts();
        }

        [HttpGet("{id}")]
        public Product Get(int id)
        {
            return _productsSerrvice.GetProductId(id);
        }

        //[HttpPost]
        //public Product Post([FromBody] Product product)
        //{
        //    _productsSerrvice.AddProduct(product);
        //    return product;
        //}

        public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
        {
            var inputs = query.Variables.ToInputs();

            var schema = new GraphQL.Types.Schema()
            {
                Query = new ProductQuery(_productsSerrvice)
            };

            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = query.Query;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest();
            }

            return Ok(result);
        }


        [HttpPost("{id}")]
        public Product Put(int id, [FromBody] Product product)
        {
            _productsSerrvice.UpdateProduct(id, product);
            return product;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _productsSerrvice.DeleteProduct(id);
        }
    }
}
