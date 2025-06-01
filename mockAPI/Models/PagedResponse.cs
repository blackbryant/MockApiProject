using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.Models
{
    public abstract class PagedResponse<T>
    {
        public IReadOnlyCollection<T> Items { get; set; } = Array.Empty<T>();
        public int PageSize { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int TotalPages { get; set; }
        public int LastPage { get; set; }
        
        public Dictionary<int ,decimal> AveragePriceByCategory { get; set; } = new Dictionary<int, decimal>();
        
    }
}