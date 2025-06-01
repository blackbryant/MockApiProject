using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Repositories;

namespace mockAPI.Models
{
    public class ProductDTO : EntityId<int>
    {
         
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    
    }
}