using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        [Key]
        [StringLength(6)]
        public string EmployeeNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeName { get; set; }
        [ForeignKey(nameof(Department.DepartmentId))]
        public int DepartmentId { get; set; }
        [ForeignKey(nameof(Position.PositionId))]
        public int PositionId { get; set; }

        [StringLength(1)]
        public string GenderCode { get; set; }

        [StringLength(6)]
        public string? ReportedToEmployeeNumber { get; set; }

        [Range(0, 24)]
        public int VacationDaysLeft { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        // Navigation properties
        public Department Department { get; set; }
        public Position Position { get; set; }
        public Employee ReportedTo { get; set; }
        public ICollection<Employee> Subordinates { get; set; }
        public ICollection<VacationRequest> VacationRequests { get; set; }

        public Employee()
        {
            VacationDaysLeft = 24;
        }
    }
}
