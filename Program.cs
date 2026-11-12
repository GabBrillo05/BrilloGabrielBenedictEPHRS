using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPHRS
{
    internal class Program
    {
        private static readonly List<Employee> Employees = new List<Employee>(); // starts empty
        private static int _nextId = 1001;
   private const string AppTitle = "EPHRS - Final Merged Edition";

        private static void Main()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("==== " + AppTitle + " ====");
                Console.WriteLine("1. Add employee");
                Console.WriteLine("2. View all employees");
                Console.WriteLine("3. Search / filter");
                Console.WriteLine("4. Update employee");
                Console.WriteLine("5. Delete employee");
                Console.WriteLine("6. Payroll summary by department");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                switch (Console.ReadLine()?.Trim())
                {
                    case "1": AddEmployee(); break;
                    case "2": ViewAll(); break;
                    case "3": SearchFilter(); break;
                    case "4": UpdateEmployee(); break;
                    case "5": DeleteEmployee(); break;
                    case "6": PayrollSummary(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
        }

        // ---------- CREATE ----------
        private static void AddEmployee()
        {
            var emp = new Employee
            {
                Id = _nextId,
                FirstName = ReadString("First name: "),
                LastName = ReadString("Last name: "),
                Department = ReadString("Department (IT, HR, Sales, Finance...): "),
                JobTitle = ReadString("Job title: "),
                MonthlySalary = ReadDecimal("Monthly salary: "),
                TaxRate = ReadTaxRate("Tax rate (e.g. 0.12 for 12%): "),
                HireDate = ReadDate("Hire date (yyyy-MM-dd): "),
                IsActive = ReadBool("Active? (y/n): "),
                Email = ReadEmail("Email: ")
            };
            Employees.Add(emp);
            _nextId++;
            Console.WriteLine($"Added employee with Id {emp.Id}.");
        }

        // ---------- READ ----------
        private static void ViewAll()
        {
            PrintTable(Employees.OrderBy(e => e.Id));
        }

        // ---------- SEARCH / FILTER ----------
        private static void SearchFilter()
        {
            Console.WriteLine("1. By name  2. By department  3. By status  4. By salary range  5. By Id");
            Console.Write("Choose: ");
            IEnumerable<Employee> result;
            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    string name = ReadString("Name contains: ");
                    result = Employees.Where(e =>
                        e.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                        e.LastName.Contains(name, StringComparison.OrdinalIgnoreCase));
                    break;
                case "2":
                    string dept = ReadString("Department: ");
                    result = Employees.Where(e => e.Department.Equals(dept, StringComparison.OrdinalIgnoreCase));
                    break;
                case "3":
                    bool active = ReadBool("Active employees? (y = active, n = inactive): ");
                    result = Employees.Where(e => e.IsActive == active);
                    break;
                case "4":
                    decimal min = ReadDecimal("Min salary: ");
                    decimal max = ReadDecimal("Max salary: ");
                    result = Employees.Where(e => e.MonthlySalary >= min && e.MonthlySalary <= max);
                    break;
                case "5":
                    int id = ReadInt("Id: ");
                    result = Employees.Where(e => e.Id == id);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }
            PrintTable(result.OrderBy(e => e.Id));
        }

        // ---------- UPDATE ----------
        private static void UpdateEmployee()
        {
            int id = ReadInt("Id of employee to update: ");
            Employee? emp = Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            Console.WriteLine("Press Enter to keep the current value.");
            emp.FirstName = ReadOptional($"First name [{emp.FirstName}]: ", emp.FirstName);
            emp.LastName = ReadOptional($"Last name [{emp.LastName}]: ", emp.LastName);
            emp.Department = ReadOptional($"Department [{emp.Department}]: ", emp.Department);
            emp.JobTitle = ReadOptional($"Job title [{emp.JobTitle}]: ", emp.JobTitle);

            string s = ReadOptional($"Monthly salary [{emp.MonthlySalary}]: ", "");
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sal) && sal >= 0)
                emp.MonthlySalary = sal;

            s = ReadOptional($"Tax rate [{emp.TaxRate}]: ", "");
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) && tax >= 0 && tax <= 1)
                emp.TaxRate = tax;

            s = ReadOptional($"Hire date [{emp.HireDate:yyyy-MM-dd}]: ", "");
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime hd))
                emp.HireDate = hd;

            s = ReadOptional($"Active (y/n) [{(emp.IsActive ? "y" : "n")}]: ", "");
            if (s.Equals("y", StringComparison.OrdinalIgnoreCase)) emp.IsActive = true;
            else if (s.Equals("n", StringComparison.OrdinalIgnoreCase)) emp.IsActive = false;

            s = ReadOptional($"Email [{emp.Email}]: ", "");
            if (s.Contains('@') && s.Contains('.')) emp.Email = s;

            Console.WriteLine("Employee updated.");
        }

        // ---------- DELETE ----------
        private static void DeleteEmployee()
        {
            int id = ReadInt("Id of employee to delete: ");
            Employee? emp = Employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) { Console.WriteLine("Employee not found."); return; }

            if (ReadBool($"Delete {emp.FullName}? (y/n): "))
            {
                Employees.RemoveAll(e => e.Id == id);
                Console.WriteLine("Employee deleted.");
            }
        }

        // ---------- PAYROLL REPORT ----------
        private static void PayrollSummary()
        {
            if (!Employees.Any()) { Console.WriteLine("No records."); return; }

            var groups = Employees
                .Where(e => e.IsActive)
                .GroupBy(e => e.Department)
                .Select(g => new
                {
                    Dept = g.Key,
                    Count = g.Count(),
                    Gross = g.Sum(e => e.MonthlySalary),
                    Tax = g.Sum(e => e.TaxAmount),
                    Net = g.Sum(e => e.NetSalary)
                })
                .OrderBy(g => g.Dept);

            Console.WriteLine($"{"Department",-15}{"Count",6}{"Gross",15}{"Tax",13}{"Net",15}");
            foreach (var g in groups)
                Console.WriteLine($"{g.Dept,-15}{g.Count,6}{g.Gross,15:N2}{g.Tax,13:N2}{g.Net,15:N2}");
            Console.WriteLine("(Active employees only)");
        }

        // ---------- DISPLAY ----------
        private static void PrintTable(IEnumerable<Employee> list)
        {
            var rows = list.ToList();
            if (rows.Count == 0) { Console.WriteLine("No records found."); return; }

            Console.WriteLine($"{"Id",-6}{"First",-12}{"Last",-12}{"Dept",-10}{"Title",-18}{"Salary",12}{"Tax",7}{"Hired",-12}{"Active",-8}Email");
            Console.WriteLine(new string('-', 120));
            foreach (var e in rows)
                Console.WriteLine($"{e.Id,-6}{e.FirstName,-12}{e.LastName,-12}{e.Department,-10}{e.JobTitle,-18}{e.MonthlySalary,12:N2}{e.TaxRate,7:P0}{e.HireDate,-12:yyyy-MM-dd}{(e.IsActive ? "Yes" : "No"),-8}{e.Email}");
            Console.WriteLine($"{rows.Count} record(s).");
        }

        // ---------- INPUT HELPERS ----------
        private static string ReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(s)) return s;
                Console.WriteLine("Value required.");
            }
        }

        private static string ReadOptional(string prompt, string fallback)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine()?.Trim();
            return string.IsNullOrEmpty(s) ? fallback : s;
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int v)) return v;
                Console.WriteLine("Enter a whole number.");
            }
        }

        private static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v) && v >= 0)
                    return v;
                Console.WriteLine("Enter a non-negative number.");
            }
        }

        private static double ReadTaxRate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double v) && v >= 0 && v <= 1)
                    return v;
                Console.WriteLine("Enter a value between 0 and 1.");
            }
        }

        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime v))
                    return v;
                Console.WriteLine("Use format yyyy-MM-dd.");
            }
        }

        private static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (s == "y" || s == "yes") return true;
                if (s == "n" || s == "no") return false;
                Console.WriteLine("Enter y or n.");
            }
        }

        private static string ReadEmail(string prompt)
        {
            while (true)
            {
                string s = ReadString(prompt);
                if (s.Contains('@') && s.Contains('.')) return s;
                Console.WriteLine("Enter a valid email address.");
            }
        }
    }
}
