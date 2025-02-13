using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class VacationType
    {
        [Key]
        [StringLength(1)]
        public string VacationTypeCode { get; set; }

        [StringLength(20)]
        public string VacationTypeName { get; set; }
    }
}
