using IT15_TripoleMedelTijol.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace IT15_TripoleMedelTijol.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Employee Table
        public DbSet<Employee> Employees { get; set; }

        // Leaves Tables
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }


        // Salary Table
        public DbSet<Salary> Salaries { get; set; }

        // Departments Table
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Department> Departments { get; set; }


        // Recruitments
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<Applicant> Applicants { get; set; }

        // Attendance Table
        public DbSet<Attendance> Attendance { get; set; }

        // Performance Table
        public DbSet<Performance> Performance { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Set EmployeeID as unique to prevent duplicates
            builder.Entity<Employee>()
                .HasIndex(e => e.EmployeeID)
                .IsUnique();

            // Configure the relationship between Employee and Salary
            builder.Entity<Salary>()
                .HasOne(s => s.Employee) // Salary has one Employee
                .WithMany(e => e.Salaries) // Employee has many Salaries
                .HasForeignKey(s => s.EmployeeID) // Foreign key in Salary table
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            // Ensure the Salary Amount column has correct precision for decimal values
            builder.Entity<Salary>()
                .Property(s => s.Amount)
                .HasColumnType("decimal(18,2)");

            // Configure one-to-one relationship between Employee and ApplicationUser
            builder.Entity<Employee>()
                .HasOne<ApplicationUser>() // An Employee is linked to an ApplicationUser (identity user)
                .WithOne()  // One-to-one relationship
                .HasForeignKey<Employee>(e => e.UserId) // Employee table holds the UserId as a foreign key
                .OnDelete(DeleteBehavior.Cascade); // If the user is deleted, delete the corresponding Employee record

            // Configure JobPosting relationship with Department (Many-to-One)
            builder.Entity<JobPosting>()
                .HasOne(jp => jp.Department) // A JobPosting belongs to one Department
                .WithMany() // A Department can have multiple JobPostings
                .HasForeignKey(jp => jp.DepartmentId); // Foreign key in JobPosting table

            // Configure JobPosting relationship with JobTitle (Many-to-One)
            builder.Entity<JobPosting>()
                .HasOne(jp => jp.JobTitle) // A JobPosting belongs to one JobTitle
                .WithMany() // A JobTitle can have multiple JobPostings
                .HasForeignKey(jp => jp.JobTitleId); // Foreign key in JobPosting table

            // Configure JobTitle relationship with Employee (One-to-One)
            builder.Entity<JobTitle>()
                .HasOne<Employee>(jt => jt.Employee) // A JobTitle is optionally held by one Employee
                .WithOne(e => e.JobTitle) // An Employee holds one JobTitle
                .HasForeignKey<JobTitle>(jt => jt.EmployeeId) // JobTitle table has the EmployeeId
                .OnDelete(DeleteBehavior.SetNull); // If the Employee is deleted, set EmployeeId to NULL in JobTitle (position becomes vacant)

            // Seed Leave Types
            builder.Entity<LeaveType>().HasData(
                new LeaveType { LeaveTypeId = 1, Name = "Vacation Leave", DefaultDays = 15, IsPaid = true },
                new LeaveType { LeaveTypeId = 2, Name = "Sick Leave", DefaultDays = 10, IsPaid = true },
                new LeaveType { LeaveTypeId = 3, Name = "Unpaid Leave", DefaultDays = 0, IsPaid = false },
                new LeaveType { LeaveTypeId = 4, Name = "Bereavement Leave", DefaultDays = 5, IsPaid = true },
                new LeaveType { LeaveTypeId = 5, Name = "Maternity Leave", DefaultDays = 105, IsPaid = true }
   );
        }
    }
}