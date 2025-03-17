using System;

namespace RestaurantManagementService.Controllers
{
    public class RestaurantDto
    {
        public int OwnerId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAvailable { get; set; }  // Add IsAvailable here
    }
}
