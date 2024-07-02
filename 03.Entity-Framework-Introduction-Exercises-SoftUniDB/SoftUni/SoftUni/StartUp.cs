using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System.Text;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            //Console.WriteLine(GetEmployeesFullInformation(context));
            //Console.WriteLine(GetEmployeesWithSalaryOver50000(context));
            //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));
            //Console.WriteLine(AddNewAddressToEmployee(context));
            //Console.WriteLine(GetEmployeesInPeriod(context));
            //Console.WriteLine(GetAddressesByTown(context));
            //Console.WriteLine(GetEmployee147(context));
            //Console.WriteLine(IncreaseSalaries(context));
            //Console.WriteLine(GetLatestProjects(context));
            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
            //Console.WriteLine(DeleteProjectById(context));
            Console.WriteLine(RemoveTown(context));

        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.MiddleName,
                        e.JobTitle,
                        e.Salary
                    }).ToList();

            string result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}"));


            return result;
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                }).Where(e => e.Salary > 50000)
                  .OrderBy(e => e.FirstName)
                  .ToList();

            return string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} - {e.Salary:f2}"));
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var rndEmployees = context.Employees
               .Where(e => e.Department.Name == "Research and Development")
               .Select(e => new
               {
                   e.FirstName,
                   e.LastName,
                   DepartmentName = e.Department.Name,
                   e.Salary
               })
               .OrderBy(e => e.Salary)
               .ThenByDescending(e => e.FirstName);


            return string.Join(Environment.NewLine,
                rndEmployees.Select(e => $"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}"));
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            employee.Address = address;
            context.SaveChanges();

            var employees = context.Employees
                .Select(e => new
                {
                    e.AddressId,
                    e.Address.AddressText
                })
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .ToList();

            return string.Join(Environment.NewLine, employees.Select(e => $"{e.AddressText}"));
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                    .Where(e => e.EmployeesProjects
                        .Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                    .Take(10)
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        ManagerFirstName = e.Manager.FirstName,
                        ManagerLastName = e.Manager.LastName,
                        Projects = e.EmployeesProjects
                            .Select(ep => ep.Project)
                    })
                    .ToList();

            var result = string.Empty;
            foreach (var e in employees)
            {
                result = $"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}{Environment.NewLine}" +
                   $"{String.Join(Environment.NewLine, e.Projects.Select(p => $"--{p.Name} - {p.StartDate.ToString()} - {(p.EndDate == null ? "not finished" : p.EndDate.ToString())}"))}";
            }

            return string.Join(Environment.NewLine, result);
        }


        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
            .Select(a => new
            {
                a.AddressText,
                TownName = a.Town.Name,
                EmployeeCount = a.Employees.Count
            })
            .OrderByDescending(a => a.EmployeeCount)
            .ThenBy(a => a.TownName)
            .ThenBy(a => a.AddressText)
            .Take(10)
            .ToList();

            var sb = new StringBuilder();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeeCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.
                    Where(e => e.EmployeeId == 147)
                    .Select(e => new {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle,
                        Projects = e.EmployeesProjects
                            .Select(ep => ep.Project.Name)
                            .OrderBy(p => p)
                            .ToList()
                    })
                    .First();

            return $"{employee.FirstName} {employee.LastName} - {employee.JobTitle}{Environment.NewLine}{String.Join(Environment.NewLine, employee.Projects)}";

        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            context.Employees
                   .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
                       .Contains(e.Department.Name))
                   .ToList()
                   .ForEach(e => e.Salary *= 1.12m);

            context.SaveChanges();

            var employees = context.Employees
                .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
                    .Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            return string.Join(Environment.NewLine, employees.Select(e => $"{e.FirstName} {e.LastName} (${e.Salary:f2})"));
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                    .Include(d => d.Employees)
                    .Include(d => d.Manager)
                    .Where(d => d.Employees.Count > 5)
                    .OrderBy(d => d.Employees.Count)
                    .ThenBy(d => d.Name)
                    .ToList();

            return string.Join(Environment.NewLine, departments.Select(d => $"{d.Name} - {d.Manager.FirstName} {d.Manager.LastName}{Environment.NewLine}{String.Join(Environment.NewLine, d.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle}").ToList())}{Environment.NewLine}{new string('-', 10)}"));
                    
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var projects = context.Projects.OrderByDescending(p => p.StartDate).Take(10).Select(s => new
            {
                ProjectName = s.Name,
                ProjectDescription = s.Description,
                ProjectStartDate = s.StartDate
            }).OrderBy(n => n.ProjectName).ToArray();

            foreach (var p in projects)
            {
                var startDate = p.ProjectStartDate.ToString("M/d/yyyy h:mm:ss tt");
                result.AppendLine($"{p.ProjectName}");
                result.AppendLine($"{p.ProjectDescription}");
                result.AppendLine($"{startDate}");
            }
            return result.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .Where(e => e.FirstName.Substring(0,2) == "Sa")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            return string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.First(p => p.ProjectId == 2);

            context.EmployeesProjects.ToList().ForEach(ep => context.EmployeesProjects.Remove(ep));
            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects.Take(10).Select(p => p.Name).ToList();

            return string.Join(Environment.NewLine, projects);
        }


        public static string RemoveTown(SoftUniContext context)
        {
            string townName = "Seattles";

            context.Employees
                .Where(e => e.Address.Town.Name == townName)
                .ToList()
                .ForEach(e => e.AddressId = null);

            int addressesCount = context.Addresses
                .Where(a => a.Town.Name == townName)
                .Count();

            context.Addresses
                .Where(a => a.Town.Name == townName)
                .ToList()
                .ForEach(a => context.Addresses.Remove(a));

            context.Towns
                .Remove(context.Towns
                    .FirstOrDefault(t => t.Name == townName));

            context.SaveChanges();

            return $"{addressesCount} {(addressesCount == 1 ? "address" : "addresses")} in {townName} {(addressesCount == 1 ? "was" : "were")} deleted";
        }
    }
}

