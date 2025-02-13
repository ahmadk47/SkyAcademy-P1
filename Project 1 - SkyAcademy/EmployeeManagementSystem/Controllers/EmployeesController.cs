using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.DTOs;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult SeedData()
        {
            try
            {

                // 2. Add Departments
                var departments = new List<Department>();
                for (int i = 1; i <= 20; i++)
                {
                    departments.Add(new Department { DepartmentName = $"Department {i:00}" });
                }
                _context.Departments.AddRange(departments);
                _context.SaveChanges();  // Departments now have IDs 1-20

                // 3. Add Positions
                var positions = new List<Position>();
                for (int i = 1; i <= 20; i++)
                {
                    positions.Add(new Position { PositionName = $"Position {i:00}" });
                }
                _context.Positions.AddRange(positions);
                _context.SaveChanges();  // Positions now have IDs 1-20

                // 4. Add Employees with valid references
                var employees = new List<Employee>();
                for (int i = 1; i <= 10; i++)
                {
                    employees.Add(new Employee
                    {
                        EmployeeNumber = $"E{i:00000}",
                        EmployeeName = $"Employee {i}",
                        DepartmentId = i,       // Valid (1-10 <= 20)
                        PositionId = i,         // Valid (1-10 <= 20)
                        GenderCode = i % 2 == 0 ? "M" : "F",
                        Salary = 2000 + (i * 100),
                        VacationDaysLeft = 24   // Explicit set from constructor
                    });
                }
                _context.Employees.AddRange(employees);
                _context.SaveChanges();

                // 5. Establish reporting hierarchy
                for (int i = 2; i <= 10; i++)  // Start from 2nd employee
                {
                    var employee = _context.Employees
                        .First(e => e.EmployeeNumber == $"E{i:00000}");
                    employee.ReportedToEmployeeNumber = $"E{i - 1:00000}";
                }
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log full error details
                return View("Error");
            }
        }


        // Update Employee Info
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateEmployee(string id, [Bind("EmployeeNumber,EmployeeName,DepartmentId,PositionId,Salary")] EmployeeUpdateDto model)
        {
            if (id != model.EmployeeNumber)
            {
                return NotFound();
            }

            var employee = _context.Employees.Find(model.EmployeeNumber);
            if (employee == null)
            {
                return NotFound();
            }

            employee.EmployeeName = model.EmployeeName;
            employee.DepartmentId = model.DepartmentId;
            employee.PositionId = model.PositionId;
            employee.Salary = model.Salary;

            try
            {
                _context.Update(employee);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(model.EmployeeNumber))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employees.Any(e => e.EmployeeNumber == id);
        }

        // Update Vacation Days
        public void UpdateVacationDays(string employeeNumber, int daysUsed)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeNumber == employeeNumber);

            if (employee != null && employee.VacationDaysLeft >= daysUsed)
            {
                employee.VacationDaysLeft -= daysUsed;
                _context.SaveChanges();
            }
        }

        // GET: Employees
        public IActionResult Index()
        {
            var employees = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Select(e => new EmployeeDTO
                {
                    Number = e.EmployeeNumber,
                    Name = e.EmployeeName,
                    Department = e.Department.DepartmentName,
                    Salary = e.Salary
                })
                .ToList();

            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.ReportedTo)
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName");
            ViewData["ReportedToEmployeeNumber"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeNumber,EmployeeName,DepartmentId,PositionId,GenderCode,ReportedToEmployeeNumber,VacationDaysLeft,Salary")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName", employee.PositionId);
            ViewData["ReportedToEmployeeNumber"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", employee.ReportedToEmployeeNumber);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName", employee.PositionId);
            ViewData["ReportedToEmployeeNumber"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", employee.ReportedToEmployeeNumber);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmployeeNumber,EmployeeName,DepartmentId,PositionId,GenderCode,ReportedToEmployeeNumber,VacationDaysLeft,Salary")] Employee employee)
        {
            if (id != employee.EmployeeNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["PositionId"] = new SelectList(_context.Positions, "PositionId", "PositionName", employee.PositionId);
            ViewData["ReportedToEmployeeNumber"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", employee.ReportedToEmployeeNumber);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.ReportedTo)
                .FirstOrDefaultAsync(m => m.EmployeeNumber == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
