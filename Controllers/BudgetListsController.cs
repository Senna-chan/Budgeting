using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Budgeting.Data;
using Budgeting.Models;

namespace Budgeting.Controllers
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

            var budgetList = await _context.BudgetList.Include(bl => bl.BudgetEntries)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetList == null)
            {
                return NotFound();
            }
            if (_context.BudgetEntry == null)
            {
                return Problem("Entity set 'BudgetingContext.BudgetEntry' is null.");
            }
            BudgetListModel blm = new();
            blm.BudgetList = budgetList;
            if (budgetList.BudgetEntries == null) return View(blm);
			blm.BudgetEntries = budgetList.BudgetEntries.ToList();
            blm.BudgetEntries = blm.BudgetEntries.OrderBy(be => be.Category).ToList();
            foreach (var entry in blm.BudgetEntries)
            {
                var moneyForAllTimes = entry.CalculateCostsForAllTimes();

                
                if (!entry.VariableCosts)
                {
                    blm.AddToCombinedPrices("all", moneyForAllTimes);
                    blm.AddToCombinedPrices(entry.IsIncome ? "income" : "expenses", moneyForAllTimes);
                }
                blm.AddToCombinedPrices("all_with_variable", moneyForAllTimes);
                blm.AddToCombinedPrices(entry.IsIncome ? "income_with_variable" : "expenses_with_variable", moneyForAllTimes);


                if (entry.ToSharedAccount)
                {
                    blm.AddToCombinedPrices("To shared account", moneyForAllTimes);
                }

                if (entry.FromCreditcard)
                {
                    blm.AddToCombinedPrices("From creditcard", moneyForAllTimes);
                }

                if (entry.Category != null)
                {
                    blm.AddToCombinedPrices(entry.Category, moneyForAllTimes);
                }
            }
            return View(blm);
        }

        // GET: BudgetLists/Create
        public IActionResult Create()
        {
            var budgetEntries = _context.BudgetEntry.ToList();
            List<CheckboxViewModel> list = new List<CheckboxViewModel>();
            foreach (var budgetEntry in budgetEntries)
            {
                list.Add(new CheckboxViewModel()
                {
                    Id = budgetEntry.Id,
                    LabelName = budgetEntry.Name,
                    IsChecked = false
                });
            }
            BudgetEntryListCheckboxModel model = new BudgetEntryListCheckboxModel();
            model.checkboxes = list;
            ViewBag.budgetEntries = budgetEntries;
            return View(model);
        }

        // POST: BudgetLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BudgetEntryListCheckboxModel model)
        {
            if (ModelState.IsValid)
            {
                var budgetEntries = _context.BudgetEntry.ToList();
				model.budgetList.BudgetEntries = new List<BudgetEntry>();

				foreach (var checkbox in model.checkboxes)
                {
                    if (checkbox.IsChecked)
                    {
                        model.budgetList.BudgetEntries.Add(budgetEntries.First(be => be.Id == checkbox.Id));
                    }
				}
				_context.Add(model.budgetList);
				await _context.SaveChangesAsync(); // Save budgetlist

				return RedirectToAction(nameof(Index));
            }
            return View(model.budgetList);
        }

        // GET: BudgetLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BudgetList == null)
            {
                return NotFound();
            }

            var budgetList = await _context.BudgetList.Include(bl => bl.BudgetEntries).Where(bl => bl.Id == id).FirstOrDefaultAsync();
            if (budgetList == null)
            {
                return NotFound();
            }

            var budgetEntries = _context.BudgetEntry.ToList();
            List<CheckboxViewModel> list = new List<CheckboxViewModel>();
            if(budgetList.BudgetEntries == null) budgetList.BudgetEntries = new List<BudgetEntry>();
            foreach (var budgetEntry in budgetEntries)
            {
                list.Add(new CheckboxViewModel()
                {
                    Id = budgetEntry.Id,
                    LabelName = budgetEntry.Name,
                    IsChecked = budgetList.BudgetEntries.Contains(budgetEntry)
                });
            }
            BudgetEntryListCheckboxModel model = new BudgetEntryListCheckboxModel();
            model.checkboxes = list;
            model.budgetList = budgetList;
            ViewBag.budgetEntries = budgetEntries;
            return View(model);
        }

        // POST: BudgetLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] BudgetEntryListCheckboxModel model)
        {
            if (id != model.budgetList.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(model.budgetList.BudgetEntries == null)
                    {
                        var dbBudgetList = await _context.BudgetList.Include(bl => bl.BudgetEntries).Where(bl => bl.Id == id).FirstAsync();
                        dbBudgetList.Name = model.budgetList.Name;
                        dbBudgetList.Description = model.budgetList.Description;
                        model.budgetList = dbBudgetList;
                    }
                    foreach(var checkbox in model.checkboxes)
                    {
                        var budgetEntry = _context.BudgetEntry.AsNoTracking().Where(b => b.Id == checkbox.Id).First();
                        if(budgetEntry == null)
                        {
                            await Console.Out.WriteLineAsync("Budget entry is null. Not possible");
                            continue;
                        }
                        bool listContainsItem = model.budgetList.BudgetEntries.Any(be => be.Id == checkbox.Id);
                        if (
                            (checkbox.IsChecked && listContainsItem) || 
                            (!checkbox.IsChecked && !listContainsItem)
                            ) {
                            continue;
                        }
                        if (checkbox.IsChecked)
                        {
                            model.budgetList.BudgetEntries.Add(budgetEntry);
                        }
                        else if(!checkbox.IsChecked)
                        {
                            model.budgetList.BudgetEntries.Remove(model.budgetList.BudgetEntries.Where(be => be.Id == checkbox.Id).First());
                        }
                    }
                    _context.Update(model.budgetList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetListExists(model.budgetList.Id))
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
            return View(model);
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
                //var bels = _context.BudgetEntryLists.Where(bel => bel.BudgetList == budgetList).ToList();
                //foreach ( var bel in bels )
                //{
                //    _context.BudgetEntryLists.Remove(bel);
                //}
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
