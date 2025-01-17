﻿using BitsOrchestraTZ.Models;
using BitsOrchestraTZ.Services;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace BitsOrchestraTZ.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var contacts = _context.Contacts.ToList();
            return View(contacts);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile CsvFile)
        {
            if (CsvFile == null || CsvFile.Length == 0)
            {
                TempData["Error"] = "Please upload a valid CSV file.";
                return RedirectToAction("Index");
            }

            using (var stream = CsvFile.OpenReadStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Register the mapping to ignore ContactID
                csv.Context.RegisterClassMap<ContactMap>();

                try
                {
                    var records = csv.GetRecords<Contact>().ToList();

                    await _context.Contacts.AddRangeAsync(records);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Contacts uploaded successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error processing file: {ex.Message}";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Contact updatedContact)
        {
            if (updatedContact == null)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var contact = await _context.Contacts.FindAsync(updatedContact.ContactID);
            if (contact == null)
            {
                return Json(new { success = false, message = "Contact not found." });
            }

            contact.Name = updatedContact.Name;
            contact.DateOfBirth = updatedContact.DateOfBirth;
            contact.Married = updatedContact.Married;
            contact.Phone = updatedContact.Phone;
            contact.Salary = updatedContact.Salary;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

   
}




