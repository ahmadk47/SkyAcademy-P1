using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.DTOs;
using EmployeeManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Services
{
    public class EmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Query 1: Get all employees
        public IQueryable<EmployeeDTO> GetAllEmployees()
        {
            return _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Select(e => new EmployeeDTO
                {
                    Number = e.EmployeeNumber,
                    Name = e.EmployeeName,
                    Department = e.Department.DepartmentName,
                    Salary = e.Salary
                });
        }

        // Query 2: Get employee details
        public EmployeeDetailsDTO GetEmployeeDetails(string number)
        {
            return _context.Employees
                .Where(e => e.EmployeeNumber == number)
                .Select(e => new EmployeeDetailsDTO
                {
                    Number = e.EmployeeNumber,
                    Name = e.EmployeeName,
                    Department = e.Department.DepartmentName,
                    Position = e.Position.PositionName,
                    ReportedTo = e.ReportedTo.EmployeeName,
                    VacationDaysLeft = e.VacationDaysLeft
                })
                .FirstOrDefault();
        }

        // Query 3: Employees with pending requests
        public IQueryable<Employee> GetEmployeesWithPendingRequests()
        {
            return _context.Employees
                .Where(e => e.VacationRequests
                    .Any(vr => vr.RequestStateId == 1))
                .Include(e => e.VacationRequests);
        }

        // Query 4: Approved requests history
        public IQueryable<VacationRequestDTO> GetApprovedRequestsHistory()
        {
            return _context.VacationRequests
                .Where(vr => vr.RequestStateId == 2)
                .Select(vr => new VacationRequestDTO
                {
                    Type = vr.VacationType.VacationTypeName,
                    Description = vr.Description,
                    Duration = $"{vr.TotalVacationDays} days",
                    ApprovedBy = vr.ApprovedBy
                });
        }

        // Query 5: Pending requests for approval
        public IQueryable<PendingRequestDTO> GetPendingRequests(string approverNumber)
        {
            return _context.VacationRequests
                .Where(vr => vr.Employee.ReportedToEmployeeNumber == approverNumber &&
                            vr.RequestStateId == 1)
                .Select(vr => new PendingRequestDTO
                {
                    RequestId = vr.RequestId, // Added missing property
                    Description = vr.Description,
                    EmployeeNumber = vr.EmployeeNumber,
                    EmployeeName = vr.Employee.EmployeeName,
                    SubmittedDate = vr.SubmissionDate,
                    Duration = $"{vr.TotalVacationDays} days",
                    StartDate = vr.StartDate,
                    EndDate = vr.EndDate,
                    Salary = vr.Employee.Salary
                });
        }
    }
}
