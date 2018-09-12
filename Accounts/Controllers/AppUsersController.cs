using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Accounts.Models;

namespace Accounts.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly UHCContext _context;

        public AppUsersController(UHCContext context)
        {
            _context = context;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.AppUsers.ToListAsync());
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUsers = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUsers == null)
            {
                return NotFound();
            }

            return View(appUsers);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,MiddleName,LastName,SUBCOUNTY,WARD,VILLAGE,UserName,Password,TerminalId,PhoneNumber,IdNumber,DateRegistered,LastModified")] AppUsers appUsers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appUsers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appUsers);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUsers = await _context.AppUsers.FindAsync(id);
            if (appUsers == null)
            {
                return NotFound();
            }
            return View(appUsers);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,MiddleName,LastName,SUBCOUNTY,WARD,VILLAGE,UserName,Password,TerminalId,PhoneNumber,IdNumber,DateRegistered,LastModified")] AppUsers appUsers)
        {
            if (id != appUsers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUsers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUsersExists(appUsers.Id))
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
            return View(appUsers);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUsers = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUsers == null)
            {
                return NotFound();
            }

            return View(appUsers);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var appUsers = await _context.AppUsers.FindAsync(id);
            _context.AppUsers.Remove(appUsers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUsersExists(long id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }
    }
}
