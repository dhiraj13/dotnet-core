using EmployeeManagement.DataAccess.Repository;
using EmployeeManagement.Models;
using EmployeeManagement.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
  public class HomeController : Controller
  {
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment hostingEnvironment)
    {
      _employeeRepository = employeeRepository;
      this._hostingEnvironment = hostingEnvironment;
    }

    public ViewResult Index()
    {
      // retrieve all the employees
      var model = _employeeRepository.GetAllEmployees();
      // Pass the list of employees to the view
      return View(model);
    }

    public ViewResult Details(int? id)
    {
      Employee employee = _employeeRepository.GetEmployee(id.Value);

      if (employee == null)
      {
        Response.StatusCode = 404;
        return View("EmployeeNotFound", id.Value);
      }

      HomeDetailsViewModel homeDetailsViewModel = new()
      {
        Employee = employee,
        PageTitle = "Employee Details"
      };

      return View(homeDetailsViewModel);
    }

    [HttpGet]
    public ViewResult Create()
    {
      return View();
    }

    [HttpPost]
    public IActionResult Create(EmployeeCreateViewModel model)
    {
      if (ModelState.IsValid)
      {
        string uniqueFileName = ProcessUploadedFile(model);

        Employee newEmployee = new()
        {
          Name = model.Name,
          Email = model.Email,
          Department = model.Department,
          PhotoPath = uniqueFileName
        };

        _employeeRepository.Add(newEmployee);
        return RedirectToAction("details", new { id = newEmployee.Id });
      }

      return View();
    }

    [HttpGet]
    public ViewResult Edit(int id)
    {
      Employee employee = _employeeRepository.GetEmployee(id);
      EmployeeEditViewModel employeeEditViewModel = new()
      {
        Id = employee.Id,
        Name = employee.Name,
        Email = employee.Email,
        Department = employee.Department,
        ExistingPhotoPath = employee?.PhotoPath ?? string.Empty
      };
      return View(employeeEditViewModel);
    }

    [HttpPost]
    public IActionResult Edit(EmployeeEditViewModel model)
    {
      if (ModelState.IsValid)
      {
        Employee employee = _employeeRepository.GetEmployee(model.Id);
        employee.Name = model.Name;
        employee.Email = model.Email;
        employee.Department = model.Department;

        if (model.Photo != null)
        {
          if (model.ExistingPhotoPath != null)
          {
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                    "images", model.ExistingPhotoPath);
            System.IO.File.Delete(filePath);
          }
          employee.PhotoPath = ProcessUploadedFile(model);
        }

        _employeeRepository.Update(employee);

        return RedirectToAction("index");
      }

      return View(model);
    }

    private string ProcessUploadedFile(EmployeeCreateViewModel model)
    {
      string uniqueFileName = null;

      if (model.Photo != null)
      {
        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
        uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          model.Photo.CopyTo(fileStream);
        }
      }

      return uniqueFileName;
    }
  }
}