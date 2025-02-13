namespace EmployeeManagementSystem.Services
{
    public class VacationService
    {
        private readonly ApplicationDbContext _context;

        public VacationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool UpdateVacationDays(string employeeNumber, int days)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeNumber == employeeNumber);

            if (employee == null || employee.VacationDaysLeft < days)
                return false;

            employee.VacationDaysLeft -= days;
            _context.SaveChanges();
            return true;
        }
    }
}
