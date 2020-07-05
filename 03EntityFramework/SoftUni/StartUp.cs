using Microsoft.EntityFrameworkCore.Internal;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new SoftUniContext();

            Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
        }

        //03. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var result = new StringBuilder();

            var employees = context.Employees
                .OrderBy(x => x.EmployeeId)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.MiddleName,
                    x.JobTitle,
                    x.Salary
                })
                .ToList();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return result.ToString().Trim();
        }

        //04. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var result = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.Salary > 50000)
                .Select(x => new
                {
                    x.FirstName,
                    x.Salary
                })
                .ToList()
                .OrderBy(x => x.FirstName);

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return result.ToString().Trim();
        }

        //05. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var result = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Department.Name,
                    x.Salary
                })
                .ToList()
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName);

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}");
            }

            return result.ToString().Trim();
        }

        //06. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var result = new StringBuilder();

            var street = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Employees
                   .Where(x => x.LastName == "Nakov")
                   .First().Address = street;

            context.SaveChanges();

            var employees = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => new
                {
                    x.Address.AddressText
                })
                .ToList();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.AddressText}");
            }

            return result.ToString().Trim();
        }

        //07. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var result = new StringBuilder();

            var employees = context.Employees
                                   .Where(e => e.EmployeesProjects.Any(ep => 2001 <= ep.Project.StartDate.Year
                                                                                  && ep.Project.StartDate.Year <= 2003))
                                   .Take(10)
                                   .Select(x => new
                                   {
                                       EmployeeName = $"{x.FirstName}" + " " + $"{x.LastName}",
                                       ManagerName = $"{x.Manager.FirstName}" + " " + $"{x.Manager.LastName}",
                                       Projects = x.EmployeesProjects
                                                   .Select(ep => new
                                                   {
                                                       ep.Project.Name,
                                                       ep.Project.StartDate,
                                                       ep.Project.EndDate
                                                   })
                                                   .ToList()
                                   })
                                   .ToList();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.EmployeeName} - Manager: {e.ManagerName}");

                foreach (var p in e.Projects)
                {
                    result.Append($"--{p.Name} - {p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - ");

                    if (p.EndDate != null)
                    {
                        result.AppendLine($"{p.EndDate?.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
                    }
                    else
                    {
                        result.AppendLine("not finished");
                    }
                }
            }
            return result.ToString().Trim();
        }

        //08. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var result = new StringBuilder();

            var addresses = context.Addresses
                .OrderByDescending(x => x.Employees.Count)
                .ThenBy(x => x.Town.Name)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .Select(x => new
                {
                    x.AddressText,
                    x.Town.Name,
                    x.Employees.Count
                })
                .ToList();

            foreach (var e in addresses)
            {
                result.AppendLine($"{e.AddressText}, {e.Name} - {e.Count} employees");
            }

            return result.ToString().Trim();
        }

        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var result = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects
                                .Select(ep => new
                                {
                                    ep.Project.Name
                                })
                                .OrderBy(x => x.Name)
                                .ToList()
                })
                .ToList();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                foreach (var p in e.Projects)
                {
                    result.AppendLine($"{p.Name}");
                }
            }

            return result.ToString().Trim();
        }

        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {

            var result = new StringBuilder();

            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .Select(x => new
                {
                    Employee = x.Employees
                                .OrderBy(x => x.FirstName)
                                .ThenBy(x => x.LastName)
                                .Select(e => new
                                {
                                    EmployeeName = $"{e.FirstName} {e.LastName} - {e.JobTitle}",
                                })
                                .ToList(),
                    Department = $"{x.Name} - {x.Manager.FirstName} {x.Manager.LastName}"
                })
                .ToList()
                .OrderBy(x => x.Employee.Count)
                .ThenBy(x => x.Department);

            foreach (var d in departments)
            {
                result.AppendLine($"{d.Department}");

                foreach (var e in d.Employee)
                {
                    result.AppendLine($"{e.EmployeeName}");
                }
            }

            return result.ToString().Trim();
        }

        //11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {

            var result = new StringBuilder();

            var projects = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    x.StartDate,
                    x.Description
                })
                .ToList();

            foreach (var p in projects)
            {
                result.AppendLine($"{p.Name}");
                result.AppendLine($"{p.Description}");
                result.AppendLine($"{p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return result.ToString().Trim();
        }

        //12. Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            var result = new StringBuilder();

            var forNewSalary = context.Employees
                                      .Where(x => x.Department.Name == "Engineering"
                                               || x.Department.Name == "Tool Design"
                                               || x.Department.Name == "Marketing"
                                               || x.Department.Name == "Information Services");

            foreach (var e in forNewSalary)
            {
                e.Salary *= 1.12m;
            }

            context.SaveChanges();

            var employees = context.Employees
                                   .Where(x => x.Department.Name == "Engineering"
                                            || x.Department.Name == "Tool Design"
                                            || x.Department.Name == "Marketing"
                                            || x.Department.Name == "Information Services")
                                   .Select(x => new
                                   {
                                       x.FirstName,
                                       x.LastName,
                                       x.Salary,
                                   })
                                   .ToList()
                                   .OrderBy(x => x.FirstName)
                                   .ThenBy(x => x.LastName);

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return result.ToString().Trim();
        }

        //13. Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var result = "";

            var employees = context.Employees
                                   .Where(x => x.FirstName.StartsWith("Sa"))
                                   .Select(x => new
                                   {
                                       x.FirstName,
                                       x.LastName,
                                       x.JobTitle,
                                       x.Salary
                                   })
                                   .ToList()
                                   .OrderBy(x => x.FirstName)
                                   .ThenBy(x => x.LastName);

            foreach (var e in employees)
            {
                result += $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"+ Environment.NewLine;
            }

            return result;
        }

        //14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var result = new StringBuilder();

            var forDeleteEmployeeProject = context.EmployeesProjects
                                                  .Where(x => x.ProjectId == 2)
                                                  .Single();

            context.EmployeesProjects.Remove(forDeleteEmployeeProject);

            var forDeleteProject = context.Projects
                                          .Where(x => x.ProjectId == 2)
                                          .Single();

            context.Projects.Remove(forDeleteProject);

            context.SaveChanges();

            var projects = context.Projects
                                  .Take(10)
                                  .Select(x => new
                                  {
                                      x.Name
                                  })
                                  .ToList();

            foreach (var p in projects)
            {
                result.AppendLine($"{p.Name}");
            }

            return result.ToString().Trim();
        }

        //15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            var result = new StringBuilder();

            var addressesId = context.Addresses
                                     .Where(x => x.Town.Name == "Seattle")
                                     .Select(x => x.AddressId);

            foreach (var a in addressesId)
            {
               var employees = context.Employees.Where(x => x.AddressId == a);

                foreach (var e in employees)
                {
                    e.AddressId = null;
                }
            }

            var forDeleteAddresses = context.Addresses
                                            .Where(x => x.Town.Name == "Seattle");

            int numberDeletedAddresses = forDeleteAddresses.Count();

            foreach (var a in forDeleteAddresses)
            {
                context.Addresses.Remove(a);
            }

            var forDeleteTown = context.Towns
                                          .Where(x => x.Name == "Seattle")
                                          .Single();

            context.Towns.Remove(forDeleteTown);

            context.SaveChanges();

            result.AppendLine($"{numberDeletedAddresses} addresses in Seattle were deleted");

            return result.ToString().Trim();
        }
    }
}
