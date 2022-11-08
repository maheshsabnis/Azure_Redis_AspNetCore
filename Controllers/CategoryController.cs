using System.Text.Json;
using ASPNet_Redis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace ASPNet_Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        IDistributedCache redisCache;

        eShoppingCodiContext ctx;

        public CategoryController(eShoppingCodiContext ctx, IDistributedCache redisCache)
        {
            this.ctx = ctx;
            this.redisCache = redisCache;
        }
        [HttpGet]
        public IActionResult Get()
        {
            // chec if the cache with name 'categories' is present
            string categoriesData = redisCache.GetString("categories");
            if (categoriesData == null)
            {
                // if the cache is null then add data in cache 
              
                List<Category> categories = ctx.Categories.ToList();
                // serialize data in JSON Form
                categoriesData = JsonSerializer.Serialize<List<Category>>(categories);
                // save data in cache
                // DistributedCacheEntryOptions: class used to define caching metadata e.g. life span for cache
                 
                var cacheOptions = new DistributedCacheEntryOptions();
                // expiration time from the Cache Time
                cacheOptions.SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(1));
                redisCache.SetString("categories", categoriesData, cacheOptions);
                return Ok(new
                {
                    message = "Data Received from Database",
                    data = categories
                });
            }
            else
            {
                // read data from cache and return it
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };
                // deserilized the data from cache
                List<Category>? cats = JsonSerializer.Deserialize<List<Category>>(categoriesData, options);
                // return dara from cache
                return Ok(new
                {
                    message = "Data Received from Cache",
                    data = cats
                });
            }

        }
    }
}
