using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mockAPI.Models
{
    public class EntityId <T>  
    {
        public required T Id { get; set; } 
    }
}