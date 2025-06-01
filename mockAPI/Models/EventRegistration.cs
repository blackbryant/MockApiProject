using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace mockAPI.Models
{
    public class EventRegistration : EntityId<int>
    {

        public Guid GUID { get; set; }
        public required string FullName { get; set; }

        public required string Email { get; set; }

        public required string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public int DaysAttending { get; set; }

        public string? Notes { get; set; }
    }
}