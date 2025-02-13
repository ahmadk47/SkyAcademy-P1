namespace EmployeeManagementSystem.DTOs
{
    public class EmployeeDetailsDTO
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string ReportedTo { get; set; }
        public int VacationDaysLeft { get; set; }
    }
}
