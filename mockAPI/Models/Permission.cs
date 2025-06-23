using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!; 
        public string Code { get; set; } = default! ; 
    }
}