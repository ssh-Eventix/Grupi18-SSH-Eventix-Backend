namespace Eventix.Application.DTOs.TicketType
{
    public class CreateTicketTypeDto
    {
        public Guid EventId { get; set; }
        public Guid EventSectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
    }
}