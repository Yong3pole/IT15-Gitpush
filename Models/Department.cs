using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    // Models/Department.cs
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; } // Nullable
    }
}
