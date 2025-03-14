﻿using IT15_TripoleMedelTijol.Models;
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
        public DbSet<EmployeeAttendance> EmployeeAttendances { get; set; }
        public DbSet<Attendance> Attendance { get; set; } // No longer used

        // Payroll Table
        public DbSet<Payroll> Payrolls { get; set; }


        // Performance Table
        public DbSet<Performance> Performance { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ✅ Seed default Leave Types
            builder.Entity<LeaveType>().HasData(
                new LeaveType { LeaveTypeId = 1, Name = "Sick Leave", DefaultDays = 10, IsPaid = true },
                new LeaveType { LeaveTypeId = 2, Name = "Vacation Leave", DefaultDays = 15, IsPaid = true },
                new LeaveType { LeaveTypeId = 3, Name = "Maternity Leave", DefaultDays = 60, IsPaid = true },
                new LeaveType { LeaveTypeId = 4, Name = "Paternity Leave", DefaultDays = 7, IsPaid = true },
                new LeaveType { LeaveTypeId = 5, Name = "Unpaid Leave", DefaultDays = 0, IsPaid = false }
            );



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
                .Property(s => s.MonthlySalary)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Employee>()
                .HasOne(e => e.JobTitle)
                .WithMany(j => j.Employees) // specify the navigation property
                .HasForeignKey(e => e.JobTitleId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Employee - Department Relationship
            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull); // This is fine, departments can cascade delete

            // Other configurations...
            // Configure JobPosting relationship with Department (Many-to-One)
            builder.Entity<JobPosting>()
                .HasOne(jp => jp.Department) // A JobPosting belongs to one Department
                .WithMany() // A Department can have multiple JobPostings
                .HasForeignKey(jp => jp.DepartmentId) // Foreign key in JobPosting table
                .OnDelete(DeleteBehavior.Restrict);  // 🔹 Prevent cascading delete

            // Configure JobPosting relationship with JobTitle (Many-to-One)
            builder.Entity<JobPosting>()
                .HasOne(jp => jp.JobTitle) // A JobPosting belongs to one JobTitle
                .WithMany() // A JobTitle can have multiple JobPostings
                .HasForeignKey(jp => jp.JobTitleId)// Foreign key in JobPosting table
                 .OnDelete(DeleteBehavior.Restrict);  // 🔹 Prevent cascading delete

            // Configure JobTitle relationship with Employee (One-to-One)
            //builder.Entity<JobTitle>()
            //    .HasOne<Employee>(jt => jt.Employee) // A JobTitle is optionally held by one Employee
            //    .WithOne(e => e.JobTitle) // An Employee holds one JobTitle
            //    .OnDelete(DeleteBehavior.SetNull); // If the Employee is deleted, set EmployeeId to NULL in JobTitle (position becomes vacant)

        }
    }
}