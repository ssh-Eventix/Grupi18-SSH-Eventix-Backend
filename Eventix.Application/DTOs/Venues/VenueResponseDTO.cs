using System;

namespace Eventix.Application.DTOs.Venues
{
    public class VenueResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;

        public string AddressLine1 { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;

        public int TotalCapacity { get; set; }

        public bool IsIndoor { get; set; }
        public bool IsAccessible { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
//