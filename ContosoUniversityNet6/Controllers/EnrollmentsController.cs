using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversityNet6.Data;
using ContosoUniversityNet6.Models;

namespace ContosoUniversityNet6.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly SchoolContext _context;

        public EnrollmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(string strSortOrder, string currentFilterTitle, string currentFilterLastName,
                                               string currentFilterGrade, string strSearchTitle, string strSearchLastName, string strSearchGrade,
                                               int? pageNumber)
        {

            var schoolContext = _context.Enrollments.Include(e => e.Course).Include(e => e.Student);

            ViewData["CurrentSort"] = strSortOrder;
            ViewData["CurrentFilterTitle"] = String.IsNullOrEmpty(strSortOrder) ? "title_desc" : "";
            ViewData["CurrentFilterLastName"] = strSortOrder == "LastName" ? "lastName_desc" : "LastName";
            ViewData["CurrentFilterGrade"] = strSortOrder == "Grade" ? "grade_desc" : "Grade";

            if (strSearchTitle == null)
            {
                pageNumber = 1;
            }
            else
            {
                strSearchTitle = currentFilterTitle;
            }

            ViewData["CurrentFilterTitle"] = strSearchTitle;

            if (strSearchLastName == null)
            {
                pageNumber = 1;
            }
            else
            {
                strSearchLastName = currentFilterLastName;
            }

            ViewData["CurrentFilterLastName"] = strSearchLastName;

            if (strSearchGrade == null)
            {
                pageNumber = 1;
            }
            else
            {
                strSearchGrade = currentFilterGrade;
            }

            ViewData["CurrentFilterGrade"] = strSearchGrade;

            var enrollment = from s in _context.Enrollments select s;

            if (!String.IsNullOrEmpty(strSearchTitle))
            {
                enrollment = enrollment.Where(e => e.Course.Title.Contains(strSearchTitle));
            }
            if (!String.IsNullOrEmpty(strSearchLastName))
            {
                enrollment = enrollment.Where(e => e.Student.LastName.Contains(strSearchLastName));
            }
            if (!String.IsNullOrEmpty(strSearchGrade))
            {
                enrollment = enrollment.Where(e => e.Grade.ToString().Contains(strSearchGrade));
            }

            switch (strSortOrder)
            {
                case "title_desc":
                    enrollment = enrollment.OrderByDescending(e => e.Course.Title);
                    break;
                case "LastName":
                    enrollment = enrollment.OrderBy(e => e.Student.LastName);
                    break;
                case "lastName_desc":
                    enrollment = enrollment.OrderByDescending(e => e.Student.LastName);
                    break;
                case "Grade":
                    enrollment = enrollment.OrderBy(e => e.Grade);
                    break;
                case "grade_desc":
                    enrollment = enrollment.OrderByDescending(e => e.Grade);
                    break;
                default:
                    enrollment = enrollment.OrderBy(e => e.Course.Title);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Enrollment>.CreateAsync(schoolContext.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return View(await schoolContext.AsNoTracking().ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID");
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID");
            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
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
            ViewData["CourseID"] = new SelectList(_context.Courses, "CourseID", "CourseID", enrollment.CourseID);
            ViewData["StudentID"] = new SelectList(_context.Students, "ID", "ID", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Enrollments == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Enrollments == null)
            {
                return Problem("Entity set 'SchoolContext.Enrollments'  is null.");
            }
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
          return (_context.Enrollments?.Any(e => e.EnrollmentID == id)).GetValueOrDefault();
        }
    }
}
