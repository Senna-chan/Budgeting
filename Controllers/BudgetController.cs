using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Budgeting.Data;
using Budgeting.Models;
using FastExcel;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Budgeting.Controllers
{
    public class BudgetController : Controller
    {
        private readonly BudgetingContext _context;

        public BudgetController(BudgetingContext context)
        {
            _context = context;
        }

        // GET: Budget
        public async Task<IActionResult> Index()
        {
            if(_context.BudgetEntry == null)
            {
                return Problem("Entity set 'BudgetingContext.BudgetEntry' is null.");
            }
            BudgetListModel blm = new BudgetListModel();
            blm.BudgetEntries = await _context.BudgetEntry.ToListAsync();
            blm.BudgetEntries = blm.BudgetEntries.OrderBy(be => be.Category).ToList();
            foreach (var entry in blm.BudgetEntries)
            {

                blm.BudgetEntriesPerTime.TryAdd(entry.TimeAmount, new List<BudgetEntry>());

                var moneyForAllTimes = entry.CalculateCostsForAllTimes();

                blm.AddToCombinedPrices("all", moneyForAllTimes);
                blm.AddToCombinedPrices(entry.IsIncome ? "income" : "expenses", moneyForAllTimes);

                if (entry.Category != null)
                {
                    blm.BudgetEntriesPerCategory.TryAdd(entry.Category, new List<BudgetEntry>());

                    blm.AddToCombinedPrices(entry.Category, moneyForAllTimes);
                    blm.BudgetEntriesPerCategory[entry.Category].Add(entry);
                }

                blm.BudgetEntriesPerTime[entry.TimeAmount].Add(entry);

                if (entry.ToSharedAccount)
                {
                    blm.AddToCombinedPrices("To shared account", moneyForAllTimes);
                }

                if (entry.FromCreditcard)
                {
                    blm.AddToCombinedPrices("From creditcard", moneyForAllTimes);
                }
            }
            return View(blm);
                        
        }

        private IEnumerable<BudgetEntry> ParseBudgetXML(string fileName)
        {
            var currentEntriesTask = _context.BudgetEntry.ToListAsync();
            currentEntriesTask.Wait();
            var currentEntries = currentEntriesTask.Result;
            List<BudgetEntry> entries = new List<BudgetEntry>();
            List<string> isExpenceNames = new List<string>() { "C", "D", "E", "F", "G" };
            using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(new FileInfo(fileName), true))
            {
                var sheet = fastExcel.Worksheets[0];
                sheet.Read();

                List<Row> rows = sheet.Rows.ToList();
                foreach (Row row in rows)
                {
                    if (row.RowNumber == 1) continue;
                    List<Cell> columns = row.Cells.ToList();
                    BudgetEntry budgetEntry = new BudgetEntry();
                    budgetEntry.FixedEntry = true;
                    foreach(Cell cell in columns)
                    {
                        Console.WriteLine($"CellColumn: {cell.ColumnName}, Value {cell.Value}");
                        switch (cell.ColumnName)
                        {
                            case "A":
                                budgetEntry.Name = (string)cell.Value;
                                break;
                            case "H":
                                budgetEntry.FromCreditcard = true;
                                break;
                            case "I":
                            case "P":
                                budgetEntry.ToSharedAccount = true;
                                break;
                            case "Q":
                                budgetEntry.Category = (string)cell.Value;
                                break;
                            case "C":
                            case "K":
                                budgetEntry.TimeAmount = TimeAmount.FOURWEEK;
                                break;
                            case "D":
                            case "L":
                                budgetEntry.TimeAmount = TimeAmount.MONTH;
                                break;
                            case "E":
                            case "M":
                                budgetEntry.TimeAmount = TimeAmount.QUARTERYEAR;
                                break;
                            case "F":
                            case "N":
                                budgetEntry.TimeAmount = TimeAmount.HALFYEAR;
                                break;
                            case "G":
                            case "O":
                                budgetEntry.TimeAmount = TimeAmount.YEAR;
                                break;
                        }
                        switch (cell.ColumnName)
                        {
                            case "C":
                            case "D":
                            case "E":
                            case "F":
                            case "G":
                            case "J":
                            case "K":
                            case "L":
                            case "M":
                            case "N":
                            case "O":
                                budgetEntry.IsIncome = !isExpenceNames.Contains(cell.ColumnName);
                                string moneyValue = (string)cell.Value;
                                moneyValue = moneyValue.Replace(".", ",");
                                budgetEntry.MoneyAmount = decimal.Parse(moneyValue); 
                                break;
                        }
                    }
                    var existingEntry = currentEntries.Find(be => be.Name.Equals(budgetEntry.Name));
                    if(existingEntry != null)
                    {
                        if(Math.Abs(existingEntry.MoneyAmount) != budgetEntry.MoneyAmount)
                        {
                            budgetEntry.Description = "orange";
                        }
                        else
                        {
                            budgetEntry.Description = "gray";
                        }
                    }
                    else
                    {
                        budgetEntry.Description = "green";
                    }
                    entries.Add(budgetEntry);
                }
            }
            return entries;
        }

        // POST: Budget/Import
        public async Task<IActionResult> Import(IFormFile ImportFile)
        {
            if (ImportFile.Length > 0)
            {
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImportFile.CopyToAsync(stream);
                }
                ViewBag.FilePath = filePath;
                return View(ParseBudgetXML(filePath));
            }
            return Problem("File is empty");
        }

        // POST: Budget/ImportAccept
        public async Task<IActionResult> ImportAccept(string FilePath)
        {
            var fromImportEntries = ParseBudgetXML(FilePath);
            var currentEntries = await _context.BudgetEntry.ToListAsync();
            foreach(var entry in fromImportEntries)
            {
                if (!entry.IsIncome) entry.MoneyAmount *= -1;
                if(currentEntries.Any(ce => ce.Name == entry.Name))
                {
                    await Console.Out.WriteLineAsync($"Budget entry with the name {entry.Name} already exists");
                } 
                else
                {
                    _context.Add(entry);
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        // GET: Budget/List
        public async Task<IActionResult> List()
        {
            return _context.BudgetEntry != null ?
                        View(await _context.BudgetEntry.ToListAsync()) :
                        Problem("Entity set 'BudgetingContext.BudgetEntry' is null.");
        }

        // GET: Budget/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BudgetEntry == null)
            {
                return NotFound();
            }

            var budgetEntry = await _context.BudgetEntry
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetEntry == null)
            {
                return NotFound();
            }

            return View(budgetEntry);
        }

        // GET: Budget/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Budget/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FixedEntry,Name,Category,IsIncome,MoneyAmount,TimeAmount,FromCreditcard,ToSharedAccount,TransferTime")] BudgetEntry budgetEntry)
        {
            if (ModelState.IsValid)
            {
                if (!budgetEntry.IsIncome) budgetEntry.MoneyAmount *= -1;
                _context.Add(budgetEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            return View(budgetEntry);
        }

        // GET: Budget/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BudgetEntry == null)
            {
                return NotFound();
            }

            var budgetEntry = await _context.BudgetEntry.FindAsync(id);
            if (budgetEntry == null)
            {
                return NotFound();
            }
            return View(budgetEntry);
        }

        // POST: Budget/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FixedEntry,Name,Category,IsIncome,MoneyAmount,TimeAmount,FromCreditcard,ToSharedAccount,TransferTime")] BudgetEntry budgetEntry)
        {
            if (id != budgetEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budgetEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetEntryExists(budgetEntry.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            return View(budgetEntry);
        }

        // GET: Budget/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BudgetEntry == null)
            {
                return NotFound();
            }

            var budgetEntry = await _context.BudgetEntry
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budgetEntry == null)
            {
                return NotFound();
            }

            return View(budgetEntry);
        }

        // POST: Budget/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BudgetEntry == null)
            {
                return Problem("Entity set 'BudgetingContext.BudgetEntry' is null.");
            }
            var budgetEntry = await _context.BudgetEntry.FindAsync(id);
            if (budgetEntry != null)
            {
                _context.BudgetEntry.Remove(budgetEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool BudgetEntryExists(int id)
        {
            return (_context.BudgetEntry?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
