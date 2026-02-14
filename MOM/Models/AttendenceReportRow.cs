using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class AttendanceReportRow
    {
        [Required(ErrorMessage = "Meeting date is required.")]
        [DataType(DataType.Date)]
        public DateTime MeetingDate { get; set; }

        [Required(ErrorMessage = "Meeting type is required.")]
        [StringLength(200, ErrorMessage = "Meeting type cannot exceed 200 characters.")]
        public string MeetingType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Meeting venue is required.")]
        [StringLength(200, ErrorMessage = "Meeting venue cannot exceed 200 characters.")]
        public string MeetingVenue { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(200, ErrorMessage = "Department name cannot exceed 200 characters.")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Staff name is required.")]
        [StringLength(200, ErrorMessage = "Staff name cannot exceed 200 characters.")]
        public string StaffName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Please enter a valid staff email address.")]
        [StringLength(200, ErrorMessage = "Staff email cannot exceed 200 characters.")]
        public string StaffEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Attendance status is required.")]
        public bool IsPresent { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }
    }
}