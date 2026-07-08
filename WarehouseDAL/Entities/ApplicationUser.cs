using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using WarehouseDAL.Entities.Entities;

namespace WarehouseDAL.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; } = true;
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();

    }
}
