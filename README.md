# Employee Payroll & HR Management System (EPHRS)

A console-based C# application for managing employee records and payroll figures.

## Features
- **Create**: add employees (Id auto-increments from 1001)
- **Read**: view all employees in a table
- **Search / Filter**: by name, department, status, salary range, or Id (LINQ)
- **Update**: edit any field of an existing employee
- **Delete**: remove an employee with confirmation
- **Payroll summary**: gross, tax and net totals per department (active employees)

## Employee fields (10 columns)
Id, FirstName, LastName, Department, JobTitle, MonthlySalary, TaxRate, HireDate, IsActive, Email

## Run
Requires the .NET 8 SDK.

```bash
dotnet run
```

The program starts with an empty employee list.

## Project structure
```
EPHRS/
├── EPHRS.csproj
├── Program.cs      # menu, CRUD, LINQ search, payroll report
├── Employee.cs     # data model
├── README.md
├── GIT_STEPS.md    # Git & GitHub workflow guide
└── .gitignore
```
