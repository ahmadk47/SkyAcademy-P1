namespace EmployeeManagementSystem.DTOs
{
    public class PendingRequestDTO
    {
        public int RequestId { get; set; }
        public string Description { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Salary { get; set; }
    }
}
