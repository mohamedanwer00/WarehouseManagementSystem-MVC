using WarehouseBLL.Extensions;

namespace WarehousePL.Web.Controllers.Users
{
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<UsersController> _localization;
        public UsersController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IUnitOfWork unitOfWork,
            IStringLocalizer<UsersController> localization)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _localization = localization;
        }
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var viewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var vm = user.Adapt<UserViewModel>();
                vm.Roles = roles;

                viewModels.Add(vm);
            }

            return View(viewModels);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();

            var model = new UserFormViewModel
            {
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                })
            };

            return PartialView("_Form", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Roles = await GetRolesAsync();
                return PartialView("_Form", viewModel);
            }

            // التحقق من الحروف العربية
            if (System.Text.RegularExpressions.Regex.IsMatch(viewModel.UserName, @"\p{IsArabic}"))
            {
                ModelState.AddModelError(nameof(viewModel.UserName), "لا يُسمح باستخدام الأحرف العربية في اسم المستخدم.");
                viewModel.Roles = await GetRolesAsync();
                return PartialView("_Form", viewModel);
            }

            // التحقق من وجود اسم المستخدم
            var userExists = await _userManager.Users.AnyAsync(x => x.UserName == viewModel.UserName);
            if (userExists)
            {
                ModelState.AddModelError(nameof(viewModel.UserName), "اسم المستخدم مستخدم بالفعل.");
                viewModel.Roles = await GetRolesAsync();
                return PartialView("_Form", viewModel);
            }

            var user = viewModel.Adapt<User>();
            user.CreateDate = DateTime.Now;
            user.LastAction = LastActionName.Insert;

            IdentityResult result = await _userManager.CreateAsync(user, viewModel.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                viewModel.Roles = await GetRolesAsync();
                return PartialView("_Form", viewModel);
            }

            // إضافة الدور (Role)
            Role? role = await _roleManager.FindByIdAsync(viewModel.SelectedRole.ToString());
            if (role != null)
            {
                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, role.Name!);
                if (!roleResult.Succeeded)
                {
                    foreach (IdentityError error in roleResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    viewModel.Roles = await GetRolesAsync();
                    return PartialView("_Form", viewModel);
                }
            }

            // 🎯 هنا التحسين: إرجاع الـ PartialView الخاص بالصف فقط ليتم إضافته في الجدول
            var userViewModel = user.Adapt<UserViewModel>();

            // إسناد اسم الـ Role للـ ViewModel ليعرض في الصف
            if (role != null)
            {
                userViewModel.Roles = new List<string> { role.Name! };
            }

            return PartialView("_Row", userViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            var model = user.Adapt<UserFormViewModel>();

            model.UserId = user.Id;

            model.Roles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList();

            var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                    model.SelectedRole = role.Id;
            }
            return PartialView("_Form", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel viewModel)
        {
            viewModel.Roles = await GetRolesAsync();

            if (!ModelState.IsValid)
                return PartialView("_Form", viewModel);

            if (viewModel.UserId <= 0)
                return NotFound();
            var user = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
            if (user == null)
                return NotFound();
            if (System.Text.RegularExpressions.Regex.IsMatch(viewModel.UserName, @"\p{IsArabic}"))
            {
                ModelState.AddModelError(nameof(viewModel.UserName), "لا يُسمح باستخدام الأحرف العربية في اسم المستخدم.");
                return PartialView("_Form", viewModel);
            }

            bool userExists = await _userManager.Users.AnyAsync(x =>
                x.UserName == viewModel.UserName &&
                x.Id != viewModel.UserId);

            if (userExists)
            {
                ModelState.AddModelError(nameof(viewModel.UserName), "اسم المستخدم مستخدم بالفعل.");
                return PartialView("_Form", viewModel);
            }

            if (!string.IsNullOrWhiteSpace(viewModel.PhoneNumber))
            {
                bool phoneExists = await _userManager.Users.AnyAsync(x =>
                    x.PhoneNumber == viewModel.PhoneNumber &&
                    x.Id != viewModel.UserId);

                if (phoneExists)
                {
                    ModelState.AddModelError(nameof(viewModel.PhoneNumber), "رقم الهاتف مستخدم بالفعل.");
                    return PartialView("_Form", viewModel);
                }
            }
            user.Name = viewModel.Name;
            user.UserName = viewModel.UserName;
            user.PhoneNumber = viewModel.PhoneNumber;
            user.Email = viewModel.Email;

            if (!string.IsNullOrWhiteSpace(viewModel.Password))
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                IdentityResult passwordResult =
                    await _userManager.ResetPasswordAsync(user, token, viewModel.Password);

                if (!passwordResult.Succeeded)
                {
                    foreach (IdentityError error in passwordResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                    return PartialView("_Form", viewModel);
                }
            }
            user.LastAction = LastActionName.Update;
            IdentityResult result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return PartialView("_Form", viewModel);
            }
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            Role? role = await _roleManager.FindByIdAsync(viewModel.SelectedRole.ToString());
            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name!);
            }
            var userViewModel = user.Adapt<UserViewModel>();

            if (role != null)
            {
                userViewModel.Roles = new List<string> { role.Name! };
            }

            return PartialView("_Row", userViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            user.LastAction = LastActionName.Delete;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest();

            var view = await BuildUserViewModelAsync(id);

            if (view == null)
                return NotFound();

            return PartialView("_Row", view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return NotFound();

            user.LastAction = LastActionName.Update;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest();

            var view = await BuildUserViewModelAsync(id);

            if (view == null)
                return NotFound();

            return PartialView("_Row", view);
        }
        private async Task<UserViewModel?> BuildUserViewModelAsync(int userId)
        {
            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return null;

            var userForRoles = new User { Id = user.Id };
            var userRoles = await _userManager.GetRolesAsync(userForRoles);

            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName!,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                LastAction = user.LastAction,
                Roles = userRoles.Select(role => _localization[role].Value).ToList()
            };
        }
        [HttpGet]
        public async Task<IActionResult> AllowUserName(string userName, int userId = 0)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return Json(true);

            return Json(user.Id == userId);
        }
        [HttpGet]
        public IActionResult AllowPhoneNumber(string phoneNumber, int userId = 0)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

            if (user == null)
                return Json(true);

            return Json(user.Id == userId);
        }
        private async Task<IEnumerable<SelectListItem>> GetRolesAsync()
        {
            return  _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                })
                .ToList();
        }  
    }
}
