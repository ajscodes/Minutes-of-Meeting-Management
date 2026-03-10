using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Staff name is required.")]
        [StringLength(200, ErrorMessage = "Staff name cannot exceed 200 characters.")]
        public string StaffName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Mobile number cannot exceed 20 characters.")]
        public string? Mobile { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }   

        public Department? Department { get; set; }

        public int TotalMeetings { get; set; } = 0;
        
        public int AttendedMeetings { get; set; } = 0;
        
        public string AttendancePercentage => TotalMeetings > 0 ? $"{(AttendedMeetings * 100) / TotalMeetings}%" : "0%";
    }
}
