using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CodeFirstApproachASPCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstApproachASPCore.Controllers;

public class HomeController : Controller
{
    private readonly StudentDBContext studentDb;

    // private readonly ILogger<HomeController> _logger;

    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }


    public HomeController(StudentDBContext studentDb)
    {
        this.studentDb = studentDb;

    }

    // Search for the Students Or
    // else By default it will return the Student db table on the Index View Page.
    public async Task<IActionResult> Index(string searchString)
    {
        var students = from item in studentDb.Students
                       select item;

        if (!string.IsNullOrEmpty(searchString))
        {
            students = students.Where(s => s.Name.Contains(searchString));
        }
        var studentList = await students.AsNoTracking().ToListAsync();

        return View(studentList);
    }


    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || studentDb.Students == null)
        {
            return NotFound();
        }
        var stdData = await studentDb.Students.FirstOrDefaultAsync(x => x.Id == id);

        if (stdData == null)
        {
            return NotFound();
        }

        return View(stdData);

    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    //Validate antiforgoeryToken for external csrf attack
    //Use below token where you submit your data/form.
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student std)
    {
        if (ModelState.IsValid)
        {
            await studentDb.Students.AddAsync(std);
            await studentDb.SaveChangesAsync();
            TempData["insertSuccess"] = "Data has been inserted";
            return RedirectToAction("Index", "Home");
        }
        else
        {
            return View(std);
        }

    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || studentDb.Students == null)
        {
            return NotFound();
        }
        var stdData = await studentDb.Students.FindAsync(id);

        if (stdData == null)
        {
            return NotFound();
        }
        return View(stdData);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, Student std)
    {
        if (id != std.Id)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            studentDb.Students.Update(std);
            await studentDb.SaveChangesAsync();
            TempData["UpdateSuccess"] = "Record has been updated";

            return RedirectToAction("Index", "Home");
        }
        return View(std);

    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || studentDb.Students == null)
        {
            return NotFound();
        }
        var stdData = await studentDb.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (stdData == null)
        {
            return NotFound();
        }
        return View(stdData);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> ConfirmDelete(int? id)
    {
        var stdData = await studentDb.Students.FindAsync(id);
        if (stdData != null)
        {
            studentDb.Students.Remove(stdData);
        }
        await studentDb.SaveChangesAsync();
        TempData["DeleteSuccess"] = "Your Prefered Record has been deleted successfully";
        return RedirectToAction("Index", "Home");

    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
