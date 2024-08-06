using EmployeeManagement.Models;

namespace EmployeeManagement.DataAccess.Repository
{
  public interface IEmployeeRepository
  {
    Employee GetEmployee(int Id);
    IEnumerable<Employee> GetAllEmployees();
    Employee Add(Employee employee);
    Employee Update(Employee employeeChanges);
    Employee Delete(int Id);
  }
}