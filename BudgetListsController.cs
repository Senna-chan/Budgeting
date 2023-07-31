using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Budgeting.Data;
using Budgeting.Models;

namespace Budgeting
{
    public class BudgetListsController : Controller
    {
        private readonly BudgetingContext _context;

        public BudgetListsController(BudgetingContext context)
        {
            _context = context;
        }

        // GET: BudgetLists
        public async Task<IActionResult> Index()
        {
              return _context.BudgetList != null ? 
                          View(await _context.BudgetList.ToListAsync()) :
                          Problem("Entity set 'BudgetingContext.BudgetList'  is null.");
        }

        // GET: BudgetLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BudgetList == null)
            {
                return NotFound();
            }

            var budgetList = await _context.BudgetList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetList == null)
            {
                return NotFound();
            }

            return View(budgetList);
        }

        // GET: BudgetLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BudgetLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] BudgetList budgetList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(budgetList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(budgetList);
        }

        // GET: BudgetLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BudgetList == null)
            {
                return NotFound();
            }

            var budgetList = await _context.BudgetList.FindAsync(id);
            if (budgetList == null)
            {
                return NotFound();
            }
            return View(budgetList);
        }

        // POST: BudgetLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] BudgetList budgetList)
        {
            if (id != budgetList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budgetList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetListExists(budgetList.Id))
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
            return View(budgetList);
        }

        // GET: BudgetLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BudgetList == null)
            {
                return NotFound();
            }

            var budgetList = await _context.BudgetList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetList == null)
            {
                return NotFound();
            }

            return View(budgetList);
        }

        // POST: BudgetLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BudgetList == null)
            {
                return Problem("Entity set 'BudgetingContext.BudgetList'  is null.");
            }
            var budgetList = await _context.BudgetList.FindAsync(id);
            if (budgetList != null)
            {
                _context.BudgetList.Remove(budgetList);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetListExists(int id)
        {
          return (_context.BudgetList?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
