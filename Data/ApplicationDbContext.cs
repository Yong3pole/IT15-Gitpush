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

            // Set EmployeeID as unique
            builder.Entity<Employee>()
                .HasIndex(e => e.EmployeeID)
                .IsUnique();

            // Configure the relationship between Employee and Salary
            builder.Entity<Salary>()
                .HasOne(s => s.Employee) // Salary has one Employee
                .WithMany(e => e.Salaries) // Employee has many Salaries
                .HasForeignKey(s => s.EmployeeID) // Foreign key in Salary table
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete

            // Ensure Salary Amount has correct precision
            builder.Entity<Salary>()
                .Property(s => s.Amount)
                .HasColumnType("decimal(18,2)");

            // If UserId exists, maintain FK
            builder.Entity<Employee>()
                .HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships
            builder.Entity<JobPosting>()
                .HasOne(jp => jp.Department)
                .WithMany()
                .HasForeignKey(jp => jp.DepartmentId);

            builder.Entity<JobPosting>()
                .HasOne(jp => jp.JobTitle)
                .WithMany()
                .HasForeignKey(jp => jp.JobTitleId);
        }
    }
}