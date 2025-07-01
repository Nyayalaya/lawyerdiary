using CourtApp.Application.Constants;
using CourtApp.Application.Enums;
using CourtApp.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourtApp.Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdminUser
    {
        public static async Task AddAllPermissionsToRole(this RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GetAllPermissions();

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                {
                    try
                    {
                        await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        //public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        //{
        //    var allClaims = await roleManager.GetClaimsAsync(role);
        //    List<string> panelPermission = new List<string>();

        //    switch (module)
        //    {
        //        case "Admin":
        //            panelPermission = Permissions.Admin.Menues();
        //            foreach (var permission in Permissions.Admin.UserActions())
        //            {
        //                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
        //                {
        //                    await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
        //                }
        //            }
        //            foreach (var permission in Permissions.Admin.LawyerDirectoryActions())
        //            {
        //                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
        //                {
        //                    await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
        //                }
        //            }
        //            foreach (var permission in Permissions.Admin.AssociateActions())
        //            {
        //                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
        //                {
        //                    await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
        //                }
        //            }
        //            break;
        //            // You can add more cases here if needed
        //    }
        //    foreach (var permission in panelPermission)
        //    {
        //        if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
        //        {
        //            await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
        //        }
        //    }

        //}

        //public static async Task AddGenPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        //{
        //    var allClaims = await roleManager.GetClaimsAsync(role);

        //    // Get permissions dynamically using reflection
        //    Type moduleType = typeof(Permissions).GetProperty(module)?.GetValue(null) as Type;
        //    if (moduleType == null) return;

        //    // Fetch menu permissions and action permissions dynamically
        //    var menuPermissions = moduleType.GetMethod("Menus")?.Invoke(null, null) as List<string>;
        //    var actionMethods = moduleType.GetMethods()
        //        .Where(m => m.ReturnType == typeof(List<string>) && m.Name.EndsWith("Actions"));

        //    List<string> allPermissions = menuPermissions ?? new List<string>();

        //    foreach (var method in actionMethods)
        //    {
        //        var actionPermissions = method.Invoke(null, null) as List<string>;
        //        if (actionPermissions != null)
        //            allPermissions.AddRange(actionPermissions);
        //    }

        //    // Add missing permissions
        //    foreach (var permission in allPermissions.Except(allClaims.Select(c => c.Value)))
        //    {
        //        await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
        //    }
        //}

        private async static Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("SuperAdmin");
            await roleManager.AddAllPermissionsToRole(adminRole);
            //await roleManager.AddGenPermissionClaim(adminRole, "Admin");
            //await roleManager.AddPermissionClaim(adminRole, "Admin");
            //await roleManager.AddPermissionClaim(adminRole, "Actions");

            //await roleManager.AddPermissionClaim(adminRole, "Users");
            //await roleManager.AddPermissionClaim(adminRole, "Cases");
            //await roleManager.AddPermissionClaim(adminRole, "Publishers");
            //await roleManager.AddPermissionClaim(adminRole, "Subjects");
            //await roleManager.AddPermissionClaim(adminRole, "BookTypes");
            //await roleManager.AddPermissionClaim(adminRole, "Books");
            //await roleManager.AddPermissionClaim(adminRole, "Clients");
            //await roleManager.AddPermissionClaim(adminRole, "ProceedingHeads");
            //await roleManager.AddPermissionClaim(adminRole, "ProceedingSubHeads");
            //await roleManager.AddPermissionClaim(adminRole, "Titles");
            //await roleManager.AddPermissionClaim(adminRole, "Cadre");
            //await roleManager.AddPermissionClaim(adminRole, "Complex");
            //await roleManager.AddPermissionClaim(adminRole, "WorkType");
            //await roleManager.AddPermissionClaim(adminRole, "Work");
            //await roleManager.AddPermissionClaim(adminRole, "CaseKind");
            //await roleManager.AddPermissionClaim(adminRole, "DeathClaimPetition");
            //await roleManager.AddPermissionClaim(adminRole, "CaseSWorks");
            //await roleManager.AddPermissionClaim(adminRole, "CaseWorks");
            //await roleManager.AddPermissionClaim(adminRole, "TypeOfCase");
            //await roleManager.AddPermissionClaim(adminRole, "LawyerDirectory");
            //await roleManager.AddPermissionClaim(adminRole, "DocType");
            //await roleManager.AddPermissionClaim(adminRole, "CaseCateogy");
            //await roleManager.AddPermissionClaim(adminRole, "CaseStage");
            //await roleManager.AddPermissionClaim(adminRole, "CourtDistrict");
            //await roleManager.AddPermissionClaim(adminRole, "CourtFee");
            //await roleManager.AddPermissionClaim(adminRole, "Court");
            //await roleManager.AddPermissionClaim(adminRole, "CourtType");
            //await roleManager.AddPermissionClaim(adminRole, "ExpenceHead");
        }

        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin",
                Email = "superadmin@gmail.com",
                FirstName = "Dushyant",
                LastName = "Singh",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123Pa$$word!");

                    await userManager.AddToRoleAsync(defaultUser, Roles.Clerk.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Lawyer.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Associate.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                }

                await roleManager.SeedClaimsForSuperAdmin();
            }
        }
    }
}