using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WarehouseBLL.Const;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WarehouseBLL.BusinessServices.View_Models
{
    public class RegisterViewModel
    {
        [Display(Name = DisplayNames.UserName)]
        public string Username { get; set; } = null!;


        [DataType(DataType.Password)]
        [Display(Name = DisplayNames.Password)]
        public string Password { get; set; } = null!;
    }
}
