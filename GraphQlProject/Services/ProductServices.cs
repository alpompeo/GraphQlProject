using GraphQlProject.Data;
using GraphQlProject.Interfaces;
using GraphQlProject.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphQlProject.Services
{
    public class ProductServices : IProduct
    {
        private GraphQLDbContext _dbContext;
        private IMemoryCache _memoryCache;

        public ProductServices(GraphQLDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public Product AddProduct(Product product)
        {
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            return product;
        }

        public void DeleteProduct(int id)
        {
            var productObj = _dbContext.Products.Find(id);
            _dbContext.Products.Remove(productObj);
            _dbContext.SaveChanges();
        }

        public List<Product> GetAllProducts()
        {
            if (!_memoryCache.TryGetValue("Products", out List<Product> products))
            {
                var options = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(30)
                };

                products = _dbContext.Products.ToList();

                _memoryCache.Set("Products", products, options);
            }

            return products;
        }

        public Product GetProductId(int id)
        {
            return _dbContext.Products.Find(id);
        }

        public Product UpdateProduct(int id, Product product)
        {
            var productObj = _dbContext.Products.Find(id);
            productObj.Name = product.Name;
            productObj.Price = product.Price;
            _dbContext.SaveChanges();

            return product;
        }
    }
}
