using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eventix.Domain.Entities
{
    public class EventSession
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public Event Event { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public Guid? SpeakerId { get; set; }

        [ForeignKey(nameof(SpeakerId))]
        public Speaker Speaker { get; set; }
    }
}
