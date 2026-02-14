using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingVenue
    {
        [Key]
        public int MeetingVenueID { get; set; }

        [Required(ErrorMessage = "Meeting venue name is required.")]
        [StringLength(200, ErrorMessage = "Meeting venue name cannot exceed 200 characters.")]
        public string MeetingVenueName { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }  
    }
}
