using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mockAPI.Models;
using mockAPI.Repositories;

namespace mockAPI.Services
{
    public class EventRegistractionsService : GenericService<EventRegistration>, IEventRegistractionsService
    {
        private readonly IEventRegistrationsRepository _repository;
        public EventRegistractionsService(IEventRegistrationsRepository repository)  
            : base(repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }


        public async Task CreateEventRegistrationAsync(EventRegistrationDTO eventRegistrationDTO)
        {
            if (eventRegistrationDTO == null) throw new ArgumentNullException(nameof(eventRegistrationDTO));

            try
            {
                Console.WriteLine(eventRegistrationDTO.ToString ());

                var eventRegistration = new EventRegistration
                {
                    Id = 0, // Assuming Id is auto-generated by the database
                    GUID = Guid.NewGuid(),
                    FullName = eventRegistrationDTO.FullName,
                    Email = eventRegistrationDTO.Email,
                    EventName = eventRegistrationDTO.EventName,
                    EventDate = eventRegistrationDTO.EventDate,
                    DaysAttending = eventRegistrationDTO.DaysAttending
                };

                await _repository.AddAsync(eventRegistration);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new InvalidOperationException("An error occurred while creating the event registration.", ex);
            }
            
        }
    }
}