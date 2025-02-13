namespace EmployeeManagementSystem.DTOs
{
    public class VacationRequestDTO
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string ApprovedBy { get; set; }
        public string DeclinedBy { get; set; }
    }
}
