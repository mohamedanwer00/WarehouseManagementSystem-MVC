using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WarehouseBLL.Const;

namespace WarehouseBLL.FormViewModels.User
{
    public class UserFormEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [Display(Name = DisplayNames.Name)]
        public string Name { get; set; }
        [Display(Name = DisplayNames.Email)]
        public string? Email { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        [Display(Name = DisplayNames.UserName)]
        public string UserName { get; set; }
        [Display(Name = DisplayNames.PhoneNumber)]
        public string? PhoneNumber { get; set; }
        [Display(Name = DisplayNames.Password)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
