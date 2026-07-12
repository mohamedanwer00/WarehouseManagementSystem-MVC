using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WarehouseBLL.Const;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
namespace WarehouseBLL.FormViewModels.User
{
    public class UserFormViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [Display(Name = DisplayNames.Name)]
        public string Name { get; set; } = null!;

        [Display(Name = DisplayNames.Email)]
        public string? Email { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [Display(Name = DisplayNames.UserName)]
        [Remote("AllowUserName", "Users", AdditionalFields = "UserId", ErrorMessage = Errors.Duplicated)]
        public string UserName { get; set; } = null!;

        [Display(Name = DisplayNames.PhoneNumber)]
        [Required(ErrorMessage = Errors.RequiredField)]
        [Remote("AllowPhoneNumber", "Users", AdditionalFields = "UserId", ErrorMessage = Errors.Duplicated)]
        public string? PhoneNumber { get; set; }

        [Display(Name = DisplayNames.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [Display(Name = DisplayNames.SelectRoles)]
        public int SelectedRole { get; set; }

        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
