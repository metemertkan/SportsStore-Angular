using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductValuesController : Controller
    {
        private DataContext context;
        public ProductValuesController(DataContext ctx)
        {
            context = ctx;
        }

        [HttpGet("{id}")]
        public Product GetProduct(long id)
        {
            //System.Threading.Thread.Sleep(5000);
            var result = context.Products
            .Include(p => p.Supplier).ThenInclude(s => s.Products)
            .Include(p => p.Ratings)
            .FirstOrDefault(p => p.ProductId == id);

            if (result != null)
            {
                if (result.Supplier != null)
                {
                    result.Supplier.Products = result.Supplier.Products.Select(p => new Product
                    {
                        ProductId = p.ProductId,
                        Category = p.Category,
                        Description = p.Description,
                        Name = p.Name,
                        Price = p.Price
                    });
                }
                if (result.Ratings != null)
                {
                    foreach (var r in result.Ratings)
                    {
                        r.Product = null;
                    }
                }
            }
            return result;
        }

        [HttpGet]
        public IEnumerable<Product> GetProducts(bool related = false)
        {
            IQueryable<Product> query = context.Products;
            if (related)
            {
                query = query.Include(p => p.Supplier).Include(p => p.Ratings);
                var data = query.ToList();
                data.ForEach(p =>
                {
                    if (p.Supplier != null)
                    {
                        p.Supplier.Products = null;
                    }
                    if (p.Ratings != null)
                    {
                        p.Ratings.ForEach(r => r.Product = null);
                    }
                });
                return data;
            }
            else
            {
                return query;
            }
        }
    }
}