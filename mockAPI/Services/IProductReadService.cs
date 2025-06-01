using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Models;

namespace mockAPI.Services
{
    public interface IProductReadService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();

        Task<PagedProductResponseDTO> GetPagedProductsAsync(int pageNumber = 1, int? lastProudctId = null);

        Task<ProductDTO?> GetById(int id);

        Task<IReadOnlyCollection<CategoryDTO>> GetCategories();
    }
}