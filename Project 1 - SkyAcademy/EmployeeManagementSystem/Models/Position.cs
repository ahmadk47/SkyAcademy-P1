using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }

        [Required]
        [StringLength(30)]
        public string PositionName { get; set; }
    }
}
