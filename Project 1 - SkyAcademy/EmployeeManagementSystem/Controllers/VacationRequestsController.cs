using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Services;
using EmployeeManagementSystem.DTOs;

namespace EmployeeManagementSystem.Controllers
{
    public class VacationRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VacationRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [HttpPost]
        public IActionResult SubmitRequest(VacationRequest request)
        {
            // Calculate days including both start and end dates
            request.TotalVacationDays = (int)(request.EndDate - request.StartDate).Days + 1;

            if (!HasOverlappingRequests(request.EmployeeNumber, request.StartDate, request.EndDate))
            {
                request.SubmissionDate = DateTime.Now;
                request.RequestStateId = 1;
                _context.VacationRequests.Add(request);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View("Error");
        }

        private bool HasOverlappingRequests(string employeeNumber, DateTime startDate, DateTime endDate)
        {
            return _context.VacationRequests
                .Any(vr => vr.EmployeeNumber == employeeNumber &&
                         vr.RequestStateId != 3 && // Not declined
                         vr.StartDate <= endDate &&
                         vr.EndDate >= startDate);
        }
        public IActionResult PendingRequests()
        {
            string approverNumber = "E00001"; // Example manager ID

            var pendingRequests = _context.VacationRequests
                .Include(vr => vr.Employee)
                .Include(vr => vr.VacationType)
                .Where(vr => vr.Employee.ReportedToEmployeeNumber == approverNumber &&
                            vr.RequestStateId == 1) // 1 = Submitted
                .Select(vr => new PendingRequestDTO
                {
                    RequestId = vr.RequestId,
                    Description = vr.Description,
                    EmployeeNumber = vr.EmployeeNumber,
                    EmployeeName = vr.Employee.EmployeeName,
                    SubmittedDate = vr.SubmissionDate,
                    Duration = $"{vr.TotalVacationDays} days",
                    StartDate = vr.StartDate,
                    EndDate = vr.EndDate,
                    Salary = vr.Employee.Salary
                })
                .ToList();

            return View(pendingRequests);
        }
        public bool UpdateVacationDays(string employeeNumber, int days)
        {
            var employee = _context.Employees
                .FirstOrDefault(e => e.EmployeeNumber == employeeNumber);

            if (employee == null || days <= 0 || employee.VacationDaysLeft < days)
                return false;

            employee.VacationDaysLeft -= days;
            return true; // SaveChanges should be called in controller
        }
        // Approve Request
        public IActionResult Approve(int id)
        {
            var request = _context.VacationRequests
                .Include(vr => vr.Employee)
                .FirstOrDefault(vr => vr.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            var vacationService = new VacationService(_context);
            if (!vacationService.UpdateVacationDays(request.EmployeeNumber, request.TotalVacationDays))
            {
                return View("Error");
            }

            request.RequestStateId = 2; // Approved
            request.ApprovedBy = "ADMIN"; // Should come from authentication

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return View("Error");
            }

            return RedirectToAction(nameof(PendingRequests));
        }
        // Decline Request
        public IActionResult Decline(int id)
        {
            var request = _context.VacationRequests.Find(id);
            if (request != null)
            {
                request.RequestStateId = 3; // Declined
                request.DeclinedBy = "ADMIN";
                _context.SaveChanges();
            }
            return RedirectToAction("PendingRequests");
        }
        // GET: VacationRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.VacationRequests.Include(v => v.Employee).Include(v => v.RequestState).Include(v => v.VacationType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: VacationRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationRequest = await _context.VacationRequests
                .Include(v => v.Employee)
                .Include(v => v.RequestState)
                .Include(v => v.VacationType)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (vacationRequest == null)
            {
                return NotFound();
            }

            return View(vacationRequest);
        }

        // GET: VacationRequests/Create
        public IActionResult Create()
        {
            // Populate dropdowns
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeName");
            ViewBag.VacationTypes = new SelectList(_context.VacationTypes, "VacationTypeCode", "VacationTypeName");

            return View();
        }

        // POST: VacationRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeNumber,VacationTypeCode,StartDate,EndDate,Description")] VacationRequest request)
        {
            try
            {
                // Calculate vacation days
                request.TotalVacationDays = (int)(request.EndDate - request.StartDate).TotalDays + 1;
                request.SubmissionDate = DateTime.Now;
                if (request.EmployeeNumber == "E00001") request.RequestStateId = 2;
                else
                request.RequestStateId = 1; // Submitted

                // Check for overlapping requests
                if (HasOverlappingRequests(request.EmployeeNumber, request.StartDate, request.EndDate))
                {
                    ModelState.AddModelError(string.Empty, "This request overlaps with an existing vacation period");
                }

                //if (ModelState.IsValid)
                //{
                    _context.VacationRequests.Add(request);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                //}
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating request: {ex.Message}");
            }

            // Repopulate dropdowns if error
            ViewBag.Employees = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeName", request.EmployeeNumber);
            ViewBag.VacationTypes = new SelectList(_context.VacationTypes, "VacationTypeCode", "VacationTypeName", request.VacationTypeCode);

            return View(request);
        }

        // GET: VacationRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationRequest = await _context.VacationRequests.FindAsync(id);
            if (vacationRequest == null)
            {
                return NotFound();
            }
            ViewData["ApprovedBy"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", vacationRequest.ApprovedBy);
            ViewData["DeclinedBy"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", vacationRequest.DeclinedBy);
            ViewData["EmployeeNumber"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", vacationRequest.EmployeeNumber);
            ViewData["RequestStateId"] = new SelectList(_context.RequestStates, "StateId", "StateName", vacationRequest.RequestStateId);
            ViewData["VacationTypeCode"] = new SelectList(_context.VacationTypes, "VacationTypeCode", "VacationTypeCode", vacationRequest.VacationTypeCode);
            return View(vacationRequest);
        }

        // POST: VacationRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,SubmissionDate,Description,EmployeeNumber,VacationTypeCode,StartDate,EndDate,TotalVacationDays,RequestStateId,ApprovedBy,DeclinedBy")] VacationRequest vacationRequest)
        {
            if (id != vacationRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacationRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacationRequestExists(vacationRequest.RequestId))
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
            ViewData["ApprovedBy"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", vacationRequest.ApprovedBy);
            ViewData["DeclinedBy"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", vacationRequest.DeclinedBy);
            ViewData["EmployeeNumber"] = new SelectList(_context.Employees, "EmployeeNumber", "EmployeeNumber", vacationRequest.EmployeeNumber);
            ViewData["RequestStateId"] = new SelectList(_context.RequestStates, "StateId", "StateName", vacationRequest.RequestStateId);
            ViewData["VacationTypeCode"] = new SelectList(_context.VacationTypes, "VacationTypeCode", "VacationTypeCode", vacationRequest.VacationTypeCode);
            return View(vacationRequest);
        }

        // GET: VacationRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vacationRequest = await _context.VacationRequests
                .Include(v => v.Employee)
                .Include(v => v.RequestState)
                .Include(v => v.VacationType)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (vacationRequest == null)
            {
                return NotFound();
            }

            return View(vacationRequest);
        }

        // POST: VacationRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vacationRequest = await _context.VacationRequests.FindAsync(id);
            if (vacationRequest != null)
            {
                _context.VacationRequests.Remove(vacationRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacationRequestExists(int id)
        {
            return _context.VacationRequests.Any(e => e.RequestId == id);
        }
    }
}
