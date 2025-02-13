using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementSystem.Models
{
    public class VacationRequest
    {
        [Key]
        public int RequestId { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Employee is required")]
        [StringLength(6)]
        [Display(Name = "Employee")]
        public string EmployeeNumber { get; set; }=null!;

        [Required(ErrorMessage = "Vacation type is required")]
        [StringLength(1)]
        [Display(Name = "Vacation Type")]
        public string VacationTypeCode { get; set; } = null!;

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Total Days")]
        public int TotalVacationDays { get; set; }

        [Display(Name = "Status")]
        public int RequestStateId { get; set; } = 1; // Default to Submitted

        [StringLength(6)]
        [Display(Name = "Approved By")]
        public string? ApprovedBy { get; set; }

        [StringLength(6)]
        [Display(Name = "Declined By")]
        public string? DeclinedBy { get; set; }

        // Navigation properties
        [ForeignKey("EmployeeNumber")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("VacationTypeCode")]
        public virtual VacationType VacationType { get; set; }

        [ForeignKey("RequestStateId")]
        public virtual RequestState RequestState { get; set; }
    }
}
