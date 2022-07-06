﻿using System;
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
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string strSortOrder, string currentFilter,string currentFilterCredits, string strSearchTitle, string strSearchCredits, int? pageNumber)
        {
            ViewData["CurrentSort"] = strSortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(strSortOrder) ? "title_desc" : "";
            ViewData["CreditsSortParm"] = strSortOrder == "Credits" ? "credits_desc" : "Credits";

            if (strSearchTitle != null)
            {
               pageNumber = 1;
            }
            else
            {
                strSearchTitle = currentFilter;
            }

            ViewData["CurrentFilter"] = strSearchTitle;

            var course = from s in _context.Courses
                         select s;

            if (strSearchCredits != null)
            {
                pageNumber = 1;
            }
            else
            {
                strSearchCredits = currentFilterCredits;
            }

            ViewData["CurrentFilterCredits"] = strSearchCredits;


            if (!String.IsNullOrEmpty(strSearchTitle))
            {
                course = course.Where(s => s.Title.Contains(strSearchTitle));
            }
            if (!String.IsNullOrEmpty(strSearchCredits))
            {
                course = course.Where(s => s.Credits.ToString().Contains(strSearchCredits));
            }


            switch (strSortOrder)
            {
                case "title_desc":
                    course = course.OrderByDescending(s => s.Title);
                    break;
                case "Credits":
                    course = course.OrderBy(s => s.Credits);
                    break;
                case "credits_desc":
                    course = course.OrderByDescending(s => s.Credits);
                    break;
                default:
                    course = course.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<Course>.CreateAsync(course.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return _context.Courses != null ? 
            //              View(await _context.Courses.ToListAsync()) :
            //              Problem("Entity set 'SchoolContext.Courses'  is null.");
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Title,Credits")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseID,Title,Credits")] Course course)
        {
            if (id != course.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseID))
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
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'SchoolContext.Courses'  is null.");
            }
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
          return (_context.Courses?.Any(e => e.CourseID == id)).GetValueOrDefault();
        }
    }
}
