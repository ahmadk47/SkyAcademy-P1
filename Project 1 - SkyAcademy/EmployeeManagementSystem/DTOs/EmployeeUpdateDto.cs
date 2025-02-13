namespace EmployeeManagementSystem.DTOs
{
    public class EmployeeUpdateDto
    {
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public decimal Salary { get; set; }
    }
}
