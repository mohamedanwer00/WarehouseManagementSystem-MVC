using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WarehouseDAL.Entities.Identity
{
    public class User: IdentityUser<int>
    {
        public string Name { get; set; } = null!;
        public override string? Email { get => base.Email; set => base.Email = value; }
        public override string? UserName { get => base.UserName; set => base.UserName = value; }
        public override string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

        public string LastAction { get; set; } = null!;
        public int? CreatedUserId { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }

        public int? ModifiedUserId { get; set; }
        public User? ModifiedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }


        //public void SetDeleted(int? currentUser)
        //{
        //    if (currentUser != null && currentUser != 0)
        //        ModifiedUserId = currentUser;
        //    LastAction = LastActionName.Delete;
        //    ModifiedDate = DateTime.Now;
        //}
        //public void SetLastUpdatedOn(int? currentUser)
        //{
        //    if (currentUser != null && currentUser != 0)
        //        ModifiedUserId = currentUser;
        //    LastAction = LastActionName.Update;
        //    ModifiedDate = DateTime.Now;
        //}
        //public void SetCreatedOn(int? currentUser)
        //{
        //    if (currentUser != null && currentUser != 0)
        //        CreatedUserId = currentUser;
        //    CreateDate = DateTime.Now;
        //    LastAction = LastActionName.Insert;
        //}
    }
}
