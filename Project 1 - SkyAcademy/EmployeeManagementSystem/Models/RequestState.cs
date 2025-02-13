using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class RequestState
    {
        [Key]
        public int StateId { get; set; }

        [Required]
        [StringLength(10)]
        public string StateName { get; set; }
    }
}
