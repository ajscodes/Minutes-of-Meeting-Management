using System.ComponentModel.DataAnnotations;

namespace MOM.Models
{
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(200, ErrorMessage = "Department name cannot exceed 200 characters.")]
        public string DepartmentName { get; set; } = string.Empty;

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}
