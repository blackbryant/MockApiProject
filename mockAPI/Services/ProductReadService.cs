using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mockAPI.DataContext;
using mockAPI.Models;

namespace mockAPI.Services
{
    public class ProductReadService : IProductReadService
    {

        private readonly AppDbContext _context;
        private readonly ILogger<ProductReadService> _logger;
        public ProductReadService(AppDbContext context, ILogger<ProductReadService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            return await _context.Products.AsNoTracking()
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                })
                .ToListAsync();
        }

        public Task<ProductDTO?> GetById(int id)
        {
            return _context.Products.AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                })
                .FirstOrDefaultAsync();
        }



        public async Task<PagedProductResponseDTO> GetPagedProductsAsync(int pageSize = 1, int? lastProudctId = null)
        {
            var query = _context.Products.AsNoTracking().AsQueryable();
            if (lastProudctId.HasValue)
            {
                query = query.Where(p => p.Id > lastProudctId.Value);
            }

            var pageProducts = await query.OrderBy(p => p.Id)
                                     .Take(pageSize)
                                     .Select(p => new ProductDTO
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Price = p.Price,
                                         CategoryId = p.CategoryId,
                                     })
                                     .ToListAsync();

            if (pageProducts == null || !pageProducts.Any())
            {
                return new PagedProductResponseDTO
                {
                    Items = new List<ProductDTO>(),
                    PageSize = pageSize,
                    HasNextPage = false,
                    HasPreviousPage = false,
                    TotalPages = 0,
                    LastPage = 0,
                };
            }


            var totalItems = await query.CountAsync();
            var lastId = pageProducts.LastOrDefault()?.Id;
            var HasNextPage = await _context.Products.AnyAsync(p => p.Id > lastId);


            var averagePricePerCategory = await GetAveragePricePerCategoryAsync(pageProducts);

            var pagedProductResponseDTO = new PagedProductResponseDTO
            {
                Items = pageProducts,
                PageSize = pageSize,
                HasNextPage = HasNextPage,
                HasPreviousPage = lastProudctId.HasValue && lastProudctId.Value > 0,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                LastPage = lastId ?? 0,
                AveragePriceByCategory = averagePricePerCategory

            };

            return pagedProductResponseDTO;
        }

        private async Task<Dictionary<int, decimal>> GetAveragePricePerCategoryAsync(List<ProductDTO> products)
        {
            if (products == null || !products.Any())
            {
                return new Dictionary<int, decimal>();
            }

            var aggregateByTask = Task.Run(() =>
            {
                var aggregateBy = products.AggregateBy(
                    p => p.CategoryId,
                    x => (Sum: 0m, Count: 0),
                    (acc, p) => (acc.Sum + p.Price, acc.Count + 1)
                    );
                var averagePriceByCategory = aggregateBy.ToDictionary(
                    kvp => kvp.Key,
                    kvp => Math.Round(kvp.Value.Sum / kvp.Value.Count, 2)
                );
                return averagePriceByCategory;
            });

            return await aggregateByTask;
        }


        public async Task<IReadOnlyCollection<CategoryDTO>> GetCategories()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();
            var categories = products.CountBy(p => p.CategoryId)
                                     .Select( category =>  new CategoryDTO
                                    { 
                                        CategoryId = category.Key,
                                        Count = category.Value
                                    });

            if (categories == null || !categories.Any())
            {
                return new List<CategoryDTO>();
            }

            return categories.ToList();
        }

    }
}