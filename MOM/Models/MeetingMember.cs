using System;
using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class MeetingMember
    {
        [Key]
        public int MeetingMemberID { get; set; }

        [Required(ErrorMessage = "Meeting is required.")]
        public int MeetingID { get; set; }

        [Required(ErrorMessage = "Staff member is required.")]
        public int StaffID { get; set; }

        public bool IsPresent { get; set; }

        [StringLength(500, ErrorMessage = "Remarks cannot exceed 500 characters.")]
        public string? Remarks { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}
