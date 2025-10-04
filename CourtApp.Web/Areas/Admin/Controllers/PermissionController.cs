using CourtApp.Application.Constants;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Areas.Admin.Models;
using CourtApp.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PermissionController : BaseController<PermissionController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Index(string roleId)
        {
            //var model = new PermissionViewModel();
            //var allPermissions = new List<RoleClaimsViewModel>();
            //allPermissions.GetPermissions(typeof(Permissions.Admin), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Dashboard), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Books), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.BookTypes), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Cases), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Publishers), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Subjects), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Clients), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.ProceedingHeads), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.ProceedingSubHeads), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.CaseHearing), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Titles), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Complex), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Cadre), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.LawyerDirectory), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.FormBuilder), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Role), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.TempDesign), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.CaseDrafting), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.WorkType), roleId);
            //allPermissions.GetPermissions(typeof(Permissions.Work), roleId);

            var model = new PermissionViewModel { RoleId = roleId };
            var allPermissions = new List<RoleClaimsViewModel>();
            // Fetch all permission categories dynamically
            var allPermissionDictionary = Permissions.GetAllPermissions();
            foreach (var permissionGroup in allPermissionDictionary)
            {
                allPermissions.Add(new RoleClaimsViewModel { Value = permissionGroup, Type = "Permission" });
                //foreach (var permission in permissionGroup.)
                //{
                //    allPermissions.Add(new RoleClaimsViewModel { Value = permission, Type = "Permission" });
                //}
            }

            var role = await _roleManager.FindByIdAsync(roleId);
            model.RoleId = roleId;
            var claims = await _roleManager.GetClaimsAsync(role);
            var claimsModel = _mapper.Map<List<RoleClaimsViewModel>>(claims);
            var allClaimValues = allPermissions.Select(a => a.Value).ToList();
            var roleClaimValues = claimsModel.Select(a => a.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            foreach (var permission in allPermissions)
            {
                if (authorizedClaims.Any(a => a == permission.Value))
                {
                    permission.Selected = true;
                }
            }
            model.RoleClaims = _mapper.Map<List<RoleClaimsViewModel>>(allPermissions);
            ViewData["Title"] = $"Permissions for {role.Name} Role";
            ViewData["Caption"] = $"Manage {role.Name} Role Permissions.";
            _notify.Success($"Updated Claims / Permissions for Role '{role.Name}'");
            return View(model);
        }

        public async Task<IActionResult> Update(PermissionViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            //Remove all Claims First
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddPermissionClaim(role, claim.Value);
            }
            _notify.Success($"Updated Claims / Permissions for Role '{role.Name}'");
            //var user = await _userManager.GetUserAsync(User);
            //await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Index", new { roleId = model.RoleId });
        }
        public async Task<IActionResult> ManagePermissions(string operatorId)
        {
            // Get current logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            List<string> Permissions = new List<string>();
            if (CurrentUser.Role.ToUpper() == "SUPERADMIN")
                Permissions = await GetAllPermissionsAsync();
            else
            {
                // Fetch claims of the current user
                var currentUserClaims = await _userManager.GetClaimsAsync(currentUser);
                Permissions = currentUserClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToList();

                var userRoles = await _userManager.GetRolesAsync(currentUser);
                var rolePermissions = new List<string>();
                foreach (var role in userRoles)
                {
                    var roleObj = await _roleManager.FindByNameAsync(role);
                    if (roleObj != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(roleObj);
                        var permissions = roleClaims
                            .Where(c => c.Type == "Permission")
                            .Select(c => c.Value)
                            .ToList();

                        rolePermissions.AddRange(permissions);
                    }
                }
                Permissions = Permissions.Union(rolePermissions).ToList();
            }
            var selectUser = await _userManager.FindByIdAsync(operatorId);
            if (selectUser == null) return NotFound();
            var selectUserClaims = await _userManager.GetClaimsAsync(selectUser);
            var usersAssignedPermission = selectUserClaims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var model = new OperatorPermissionsViewModel
            {
                OperatorId = operatorId,
                AvailablePermissions = Permissions, // Directly use current user's permissions
                AssignedPermissions = usersAssignedPermission
            };

            ViewData["Title"] = $"Permissions for {selectUser.FirstName} {selectUser.LastName} Operator";
            ViewData["Caption"] = $"Manage {selectUser.FirstName} {selectUser.LastName} Permissions.";
            _notify.Success($"Updated Claims / Permissions for {selectUser.FirstName} {selectUser.LastName}");

            return View(model);
        }


        //public async Task<IActionResult> ManagePermissions(string operatorId)
        //{
        //    var operatorUser = await _userManager.FindByIdAsync(operatorId);
        //    if (operatorUser == null) return NotFound();

        //    var claims = await _userManager.GetClaimsAsync(operatorUser);
        //    var assignedPermissions = claims.Where(c => c.Type == "Permission")
        //        .Select(c => c.Value).ToList();
        //    var model = new OperatorPermissionsViewModel
        //    {
        //        OperatorId = operatorId,
        //        AvailablePermissions = await GetAllPermissionsAsync(),
        //        AssignedPermissions = assignedPermissions
        //    };
        //    ViewData["Title"] = $"Permissions for {operatorUser.FirstName + " " + operatorUser.LastName} Operator";
        //    ViewData["Caption"] = $"Manage {operatorUser.FirstName + " " + operatorUser.LastName}  Permissions.";
        //    _notify.Success($"Updated Claims / Permissions for Role '{operatorUser.FirstName + " " + operatorUser.LastName}'");
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> ManagePermissions(OperatorPermissionsViewModel model)
        {
            var operatorUser = await _userManager.FindByIdAsync(model.OperatorId);
            if (operatorUser == null) return NotFound();

            var existingClaims = await _userManager.GetClaimsAsync(operatorUser);
            var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();

            // Remove all existing permission claims
            foreach (var claim in permissionClaims)
            {
                await _userManager.RemoveClaimAsync(operatorUser, claim);
            }

            // Add selected permissions
            foreach (var permission in model.AssignedPermissions)
            {
                await _userManager.AddClaimAsync(operatorUser, new System.Security.Claims.Claim("Permission", permission));
            }

            return RedirectToAction("ManagePermissions", new { operatorId = model.OperatorId });
        }
        public async Task<List<string>> GetAllPermissionsAsync()
        {
            var allPermissions = new HashSet<string>(); // Use HashSet to avoid duplicates

            var roles = _roleManager.Roles.ToList(); // Get all roles
            foreach (var role in roles)
            {
                var claims = await _roleManager.GetClaimsAsync(role); // Get role claims
                var permissions = claims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value);

                allPermissions.UnionWith(permissions); // Add unique permissions
            }

            return allPermissions.ToList();
        }

        //private List<string> GetAllPermissions()
        //{
        //    return Permissions.Admin.Menues()
        //        .Concat(Permissions.Cases.GetAllPermissions())
        //        .Concat(Permissions.CaseHearing.GetAllPermissions())
        //        .Concat(Permissions.LawyerDirectory.GetAllPermissions())
        //        .Concat(Permissions.Cadre.GetAllPermissions())
        //        .Concat(Permissions.CaseDrafting.GetAllPermissions())
        //        .ToList();
        //}
    }
}