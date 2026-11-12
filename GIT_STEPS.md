# Git & GitHub Workflow (replace LastName / First with your own)

Example name: Juan Santos → repo `SantosJuanEPHRS`, branch `SantosJuan`.

## Step 1 – Create the repository
On github.com: New repository → name exactly `LastNameFirstNameEPHRS` → **Public**.
Do NOT add a README/.gitignore there (this project already has them).

## Step 2 – Initialize, first commit, push (main)
```bash
git init
git branch -M main
git add .
git commit -m "Initial commit: Employee Payroll & HR Management System"
git remote add origin https://github.com/<your-username>/LastNameFirstNameEPHRS.git
git push -u origin main
```

## Step 3 – Feature branch
```bash
git checkout -b LastNameFirst
```

## Step 4 – Create the deliberate conflict (same line in Program.cs)
The target line is:
```csharp
private const string AppTitle = "EMPLOYEE PAYROLL & HR MANAGEMENT SYSTEM";
```

On the **feature branch** (`LastNameFirst`), change that line to:
```csharp
private const string AppTitle = "EPHRS - Feature Branch Edition";
```
```bash
git add Program.cs
git commit -m "Change app title on feature branch"
git push -u origin LastNameFirst
```

Switch to **main** and change the SAME line differently:
```bash
git checkout main
```
```csharp
private const string AppTitle = "EPHRS - Main Branch Edition";
```
```bash
git add Program.cs
git commit -m "Change app title on main"
```

## Step 5 – Merge and resolve
```bash
git merge LastNameFirst
```
Git reports `CONFLICT (content): Merge conflict in Program.cs`. Open Program.cs:
```
<<<<<<< HEAD
private const string AppTitle = "EPHRS - Main Branch Edition";
=======
private const string AppTitle = "EPHRS - Feature Branch Edition";
>>>>>>> LastNameFirst
```
Keep one version (or combine), e.g.
```csharp
private const string AppTitle = "EPHRS - Final Merged Edition";
```
and delete all three marker lines. Then:
```bash
git add Program.cs
git commit -m "Merge LastNameFirst into main, resolve AppTitle conflict"
git push origin main
```

Check with `git log --oneline --graph --all`.
