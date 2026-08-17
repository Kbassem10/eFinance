# Applying the ERD to your database — step by step

## 1. Copy files into your project
Copy this zip's contents into your `eFinance` repo, overwriting existing files:
- `Entities/*.cs` -> replaces your current Entities folder (delete the old CourseSession.cs, it's replaced by CourseSchedule.cs + Lecture.cs)
- `Data/ApplicationDbContext.cs` -> replaces your current one
- `Program.cs` -> replaces your current one (now calls `Migrate()` instead of `EnsureCreated()`)

## 2. Make sure your Docker SQL Server is running
```bash
docker ps
```
You should see your SQL Server container listed. If not, start it (adjust to however you originally ran it, e.g.):
```bash
docker start <your-sql-container-name>
```

## 3. Set your connection string
In `appsettings.Development.json`, fill in `DefaultConnection`, e.g.:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=StudentRegistrationPortal;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
  }
}
```
Match host/port/user/password to whatever you used in your `docker run`/`docker-compose` command.

## 4. Install the EF Core CLI tool (once, if you haven't)
```bash
dotnet tool install --global dotnet-ef
```

## 5. Create the migration
From the project folder (where the .csproj is):
```bash
dotnet ef migrations add InitialErdSchema
```
This reads `ApplicationDbContext` and generates a `Migrations/` folder with the C# that builds all 24 tables and their foreign keys.

## 6. Apply it to the database
```bash
dotnet ef database update
```
This runs against your Docker SQL Server and creates the actual tables. You can also just run `dotnet run` — `Program.cs` now calls `dbContext.Database.Migrate()` automatically at startup, so it will apply any pending migrations for you.

## 7. Verify
Connect to the container with a client (Azure Data Studio, SSMS, or `sqlcmd`) and confirm all 24 tables exist:
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
```

## Notes on design decisions I made
- Every table/PK/FK/column from your ERD is mapped 1:1 (24 tables total).
- Join tables (`UserRoles`, `CourseDepartments`, `CourseOfferingInstructors`, `CoursePrerequisites`) use composite primary keys instead of a surrogate Id, matching the ERD.
- Delete behavior: I set most foreign keys to `Restrict` (SQL Server blocks "multiple cascade paths," and this schema has many converging relationships — e.g. Student is reachable through both Users and Departments). I only used `Cascade` for tables that are exclusively owned by one parent with no further children (e.g. `UserRoles`, `Enrollments` from `Student`, `StudentHolds`, `Attendance` from `Lecture`). You can loosen individual ones later, but Restrict is also just safer for academic records — you won't want deleting a Department to silently wipe out its Students.
- `CoursePrerequisites` is a self-referencing many-to-many on `Courses` (a course's prerequisites are other courses), both FKs set to `Restrict` since SQL Server disallows cascade on self-references.
