using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingType
    {
        [Key]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage = "Meeting type name is required.")]
        [StringLength(200, ErrorMessage = "Meeting type name cannot exceed 200 characters.")]
        public string MeetingTypeName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }   
    }
}
