using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MOM.Models
{
    public class Meeting
    {
        [Key]
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Meeting date and time is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Meeting type is required.")]
        public int MeetingTypeID { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Meeting venue is required.")]
        public int MeetingVenueID { get; set; }

        public string? MeetingDescription { get; set; }

        [MaxLength(500, ErrorMessage = "Document path cannot exceed 500 characters.")]
        public string? DocumentPath { get; set; }

        public IFormFile? DocumentFile { get; set; }

        public bool IsCancelled { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CancellationDateTime { get; set; }

        [MaxLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters.")]
        public string? CancellationReason { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public MeetingType? MeetingType { get; set; }

        public Department? Department { get; set; }

        public MeetingVenue? MeetingVenue { get; set; }
    }
}
