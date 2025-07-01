
using System.Collections.Generic;
using System.Linq;

namespace CourtApp.Application.Constants
{
    public static class Permissions
    {

        public static readonly Dictionary<string, Dictionary<string, List<string>>> Modules = new()
        {
            { "Admin", new Dictionary<string, List<string>>
                {
                    { "Menu",
                        new()
                        {
                            "AdminPanel.Menu.canAccessAdminPanel",
                            "AdminPanel.Menu.User",
                            "AdminPanel.Menu.Role",
                            "AdminPanel.Menu.FormBuilder",
                            "AdminPanel.Menu.LawyerDirectory",
                            "AdminPanel.Menu.Client",
                            "AdminPanel.Menu.Associate",
                            "AdminPanel.Menu.Template",
                            "AdminPanel.Menu.CaseForm"
                        }
                    },
                    { "User", new() { "AdminPanel.User.Create", "AdminPanel.User.View", "AdminPanel.User.Edit", "AdminPanel.User.Delete" } },
                    { "Role", new() { "AdminPanel.Role.Create", "AdminPanel.Role.View", "AdminPanel.Role.Edit", "AdminPanel.Role.Delete" } },
                    { "FormBuilder", new() { "AdminPanel.FormBuilder.Create", "AdminPanel.FormBuilder.View", "AdminPanel.FormBuilder.Edit", "AdminPanel.FormBuilder.Delete" } },
                    { "LawyerDirectory", new() { "AdminPanel.LawyerDirectory.Create", "AdminPanel.LawyerDirectory.View", "AdminPanel.LawyerDirectory.Edit", "AdminPanel.LawyerDirectory.Delete" } },
                    { "Client", new() { "AdminPanel.Client.Create", "AdminPanel.Client.View", "AdminPanel.Client.Edit", "AdminPanel.Client.Delete" } },
                    { "Associate", new() { "AdminPanel.Associate.Create", "AdminPanel.Associate.View", "AdminPanel.Associate.Edit", "AdminPanel.Associate.Delete" } },
                    { "Template", new() { "AdminPanel.Template.Create", "AdminPanel.Template.View", "AdminPanel.Template.Edit", "AdminPanel.Template.Delete" } },
                    { "CaseForm", new() { "AdminPanel.CaseForm.Create", "AdminPanel.CaseForm.View", "AdminPanel.CaseForm.Edit", "AdminPanel.CaseForm.Delete" } },
                }
            },
            { "Case", new Dictionary<string, List<string>>
                {
                    { "Menu",
                        new()
                        {
                            "CasePanel.Menu.canAccessCasePanel",
                            "CasePanel.Menu.ManageCase",
                            "CasePanel.Menu.CompleteTitle",
                            "CasePanel.Menu.TodayHearing",
                            "CasePanel.Menu.WithoutHearingDate",
                            "CasePanel.Menu.PendingWork",
                            "CasePanel.Menu.CaseSearch",
                            "CasePanel.Menu.CaseDrafting",
                            "CasePanel.Menu.FormPrint",
                        }
                    },
                    { "ManageCase", new() { "CasePanel.ManageCase.Create", "CasePanel.ManageCase.View", "CasePanel.ManageCase.Edit", "CasePanel.ManageCase.Delete", "CasePanel.ManageCase.DocUpload", "CasePanel.ManageCase.Detail"} },
                    { "CompleteTitle", new() { "CasePanel.CompleteTitle.Create", "CasePanel.CompleteTitle.View", "CasePanel.CompleteTitle.Edit", "CasePanel.CompleteTitle.Delete"} }
                }
            },
            { "Register", new Dictionary<string, List<string>>
                {
                    { "Menu",
                        new()
                        {
                            "Register.Menu.canAccessCasePanel",
                            "Register.Menu.Disposal",
                            "Register.Menu.Copying",
                            "Register.Menu.Institue",
                            "Register.Menu.Other"
                        }
                    }
                 }
            },

            { "Accounting", new Dictionary<string, List<string>>
                {
                    { "Menu", new() { "AccountingPanel.Menu.canAccessAccountingPanel", "AccountingPanel.Menu.Billing" } },
                    { "Billing", new() { "AccountingPanel.Billing.Create", "AccountingPanel.Billing.View", "AccountingPanel.Billing.Edit", "AccountingPanel.Billing.Delete" } }
                }
            },
            { "Tools", new Dictionary<string, List<string>>
                {
                    { "Menu",
                        new()
                        {
                            "Menu.Tools.canAccess",
                            "Menu.CourtFeeCalculator.canAccess",
                            "Menu.LimitationCounterCalculator.canAccess",
                            "Menu.InterestCalculator.canAccess",
                            "Menu.StampDutyCalculator.canAccess",
                        }
                    }
                }
            }
        };

        // Get all modules
        public static List<string> GetModules() => Modules.Keys.ToList();

        // Get all permissions
        public static List<string> GetAllPermissions() =>
            Modules.Values.SelectMany(module => module.Values).SelectMany(perms => perms).Distinct().ToList();

        // Get permissions for a specific module
        public static List<string> GetPermissionsForModule(string module) =>
            Modules.TryGetValue(module, out var permissions) ? permissions.Values.SelectMany(p => p).ToList() : new List<string>();







        //public static List<string> GeneratePermissionsForModule(string panel, string module)
        //{
        //    return new List<string>()
        //    {
        //        $"{panel}.{module}.Create",
        //        $"{panel}.{module}.View",
        //        $"{panel}.{module}.Edit",
        //        $"{panel}.{module}.Delete"
        //    };
        //}

        //public static class Admin
        //{
        //    public const string AdminPanel = "AdminPanel.Menu.canAccessAdminPanel";
        //    public const string Users = "AdminPanel.Menu.User";
        //    public const string FormBuilder = "AdminPanel.Menu.FormBuilder";
        //    public const string Roles = "AdminPanel.Menu.Role";
        //    public const string Template = "AdminPanel.Menu.Template";
        //    public const string LawyerDirectory = "AdminPanel.Menu.LawyerDirectory";
        //    public const string Client = "AdminPanel.Menu.Client";
        //    public const string Associate = "AdminPanel.Menu.Associate";
        //    public static List<string> Menues() =>
        //        new List<string> { AdminPanel, Users, FormBuilder, Roles, Template,
        //            LawyerDirectory, Associate };

        //    // User Actions
        //    public const string UserCreate = "AdminPanel.User.Create";
        //    public const string UserView = "AdminPanel.User.View";
        //    public const string UserDelete = "AdminPanel.User.Delete";
        //    public const string UserEdit = "AdminPanel.User.Edit";  // Fixed duplicate View key

        //    public static List<string> UserActions() =>
        //        new List<string> { UserCreate, UserView, UserDelete, UserEdit };

        //    // Lawyer Directory Actions
        //    public const string LawyerDirectoryCreate = "AdminPanel.LawyerDirectory.Create";
        //    public const string LawyerDirectoryView = "AdminPanel.LawyerDirectory.View";
        //    public const string LawyerDirectoryDelete = "AdminPanel.LawyerDirectory.Delete";
        //    public const string LawyerDirectoryEdit = "AdminPanel.LawyerDirectory.Edit";

        //    public static List<string> LawyerDirectoryActions() =>
        //        new List<string> { LawyerDirectoryCreate, LawyerDirectoryView, LawyerDirectoryDelete, LawyerDirectoryEdit };

        //    // Client Actions
        //    public const string ClientCreate = "AdminPanel.Client.Create";
        //    public const string ClientView = "AdminPanel.Client.View";
        //    public const string ClientDelete = "AdminPanel.Client.Delete";
        //    public const string ClientEdit = "AdminPanel.Client.Edit";

        //    public static List<string> ClientActions() =>
        //        new List<string> { ClientCreate, ClientView, ClientDelete, ClientEdit };

        //    // Associate Actions
        //    public const string AssociateCreate = "AdminPanel.Associate.Create";
        //    public const string AssociateView = "AdminPanel.Associate.View";
        //    public const string AssociateDelete = "AdminPanel.Associate.Delete";
        //    public const string AssociateEdit = "AdminPanel.Associate.Edit";

        //    public static List<string> AssociateActions() =>
        //        new List<string> { AssociateCreate, AssociateView, AssociateDelete, AssociateEdit };
        //}


        public static class LawyerDirectory
        {
            public const string View = "Permissions.LawyerDirectory.View";
            public const string Create = "Permissions.LawyerDirectory.Create";
            public const string Edit = "Permissions.LawyerDirectory.Edit";
            public const string Delete = "Permissions.LawyerDirectory.Delete";
            public const string MenuAccess = "Permissions.LawyerDirectory.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class Role
        {
            public const string View = "Permissions.Role.View";
            public const string Create = "Permissions.Role.Create";
            public const string Edit = "Permissions.Role.Edit";
            public const string Delete = "Permissions.Role.Delete";
            public const string MenuAccess = "Permissions.Role.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class FormBuilder
        {
            public const string View = "Permissions.FormBuilder.View";
            public const string Create = "Permissions.FormBuilder.Create";
            public const string Edit = "Permissions.FormBuilder.Edit";
            public const string Delete = "Permissions.FormBuilder.Delete";
            public const string MenuAccess = "Permissions.FormBuilder.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class TempDesign
        {
            public const string View = "Permissions.TempDesign.View";
            public const string Create = "Permissions.TempDesign.Create";
            public const string Edit = "Permissions.TempDesign.Edit";
            public const string Delete = "Permissions.TempDesign.Delete";
            public const string MenuAccess = "Permissions.TempDesign.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }



        public static class Dashboard
        {
            public const string View = "Permissions.Dashboard.View";
            public const string Create = "Permissions.Dashboard.Create";
            public const string Edit = "Permissions.Dashboard.Edit";
            public const string Delete = "Permissions.Dashboard.Delete";
            public const string MenuAccess = "Permissions.Dashboard.MenuAccess";
            public static List<string> GetAllPermissions() =>
                new List<string> { View, Create, Edit, Delete, MenuAccess };
        }

        public static class DocType
        {
            public const string View = "Permissions.DocType.View";
            public const string Create = "Permissions.DocType.Create";
            public const string Edit = "Permissions.DocType.Edit";
            public const string Delete = "Permissions.DocType.Delete";
            public const string MenuAccess = "Permissions.DocType.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }


        public static class CaseDrafting
        {
            public const string View = "Permissions.CaseDrafting.View";
            public const string Create = "Permissions.CaseDrafting.Create";
            public const string Edit = "Permissions.CaseDrafting.Edit";
            public const string Delete = "Permissions.CaseDrafting.Delete";
            public const string MenuAccess = "Permissions.CaseDrafting.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }


        public static class Cases
        {
            public const string View = "Permissions.Cases.View";
            public const string Create = "Permissions.Cases.Create";
            public const string Edit = "Permissions.Cases.Edit";
            public const string Delete = "Permissions.Cases.Delete";
            public const string MenuAccess = "Permissions.Cases.MenuAccess";
            public static List<string> GetAllPermissions() =>
                           new List<string> { View, Create, Edit, Delete, MenuAccess };
        }

        public static class Publishers
        {
            public const string View = "Permissions.Publishers.View";
            public const string Create = "Permissions.Publishers.Create";
            public const string Edit = "Permissions.Publishers.Edit";
            public const string Delete = "Permissions.Publishers.Delete";
            public const string MenuAccess = "Permissions.Publishers.MenuAccess";
            public static List<string> GetAllPermissions() =>
                           new List<string> { View, Create, Edit, Delete, MenuAccess };
        }

        public static class Subjects
        {
            public const string View = "Permissions.Subjects.View";
            public const string Create = "Permissions.Subjects.Create";
            public const string Edit = "Permissions.Subjects.Edit";
            public const string Delete = "Permissions.Subjects.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }

        public static class BookTypes
        {
            public const string View = "Permissions.BookTypes.View";
            public const string Create = "Permissions.BookTypes.Create";
            public const string Edit = "Permissions.BookTypes.Edit";
            public const string Delete = "Permissions.BookTypes.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }

        public static class Books
        {
            public const string View = "Permissions.Books.View";
            public const string Create = "Permissions.Books.Create";
            public const string Edit = "Permissions.Books.Edit";
            public const string Delete = "Permissions.Books.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }
        public static class Clients
        {
            public const string View = "Permissions.Clients.View";
            public const string Create = "Permissions.Clients.Create";
            public const string Edit = "Permissions.Clients.Edit";
            public const string Delete = "Permissions.Clients.Delete"; public const string MenuAccess = "Permissions.Publishers.Menu";
            public static List<string> GetAllPermissions() =>
                           new List<string> { View, Create, Edit, Delete, MenuAccess };
        }

        public static class ProceedingHeads
        {
            public const string View = "Permissions.PHead.View";
            public const string Create = "Permissions.PHead.Create";
            public const string Edit = "Permissions.PHead.Edit";
            public const string Delete = "Permissions.PHead.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }

        public static class ProceedingSubHeads
        {
            public const string View = "Permissions.PSHead.View";
            public const string Create = "Permissions.PSHead.Create";
            public const string Edit = "Permissions.PSHead.Edit";
            public const string Delete = "Permissions.PSHead.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }
        public static class TypeOfCase
        {
            public const string View = "Permissions.TypeOfCase.View";
            public const string Create = "Permissions.TypeOfCase.Create";
            public const string Edit = "Permissions.TypeOfCase.Edit";
            public const string Delete = "Permissions.TypeOfCase.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }

        public static class CaseWorks
        {
            public const string View = "Permissions.CaseWorks.View";
            public const string Create = "Permissions.CaseWorks.Create";
            public const string Edit = "Permissions.CaseWorks.Edit";
            public const string Delete = "Permissions.CaseWorks.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }

        public static class CaseSWorks
        {
            public const string View = "Permissions.CaseSWorks.View";
            public const string Create = "Permissions.CaseSWorks.Create";
            public const string Edit = "Permissions.CaseSWorks.Edit";
            public const string Delete = "Permissions.CaseSWorks.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }
        public static class CaseHearing
        {
            public const string View = "Permissions.Hearing.View";
            public const string Create = "Permissions.Hearing.Create";
            public const string Edit = "Permissions.Hearing.Edit";
            public const string Delete = "Permissions.Hearing.Delete";
            public const string Proceeding = "Permissions.Hearing.Proceeding";
            public const string Work = "Permissions.Hearing.Work";
            public const string BringToday = "Permissions.Hearing.BToday";
            public const string MenuAccess = "Permissions.Publishers.MenuAccess";
            public static List<string> GetAllPermissions() =>
                           new List<string> { View, Create, Edit, Delete, MenuAccess, Proceeding, Work, BringToday, MenuAccess };
        }

        public static class DeathClaimPetition
        {
            public const string View = "Permissions.Death.View";
            public const string Create = "Permissions.Death.Create";
            public const string Edit = "Permissions.Death.Edit";
            public const string Delete = "Permissions.Death.Delete";
            public const string MenuAccess = "Permissions.Death.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }

        public static class Titles
        {
            public const string View = "Permissions.Titles.View";
            public const string Create = "Permissions.Titles.Create";
            public const string Edit = "Permissions.Titles.Edit";
            public const string Delete = "Permissions.Titles.Delete";
            public const string MenuAccess = "Permissions.Titles.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class Complex
        {
            public const string View = "Permissions.Complex.View";
            public const string Create = "Permissions.Complex.Create";
            public const string Edit = "Permissions.Complex.Edit";
            public const string Delete = "Permissions.Complex.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }
        public static class Cadre
        {
            public const string View = "Permissions.Cadre.View";
            public const string Create = "Permissions.Cadre.Create";
            public const string Edit = "Permissions.Cadre.Edit";
            public const string Delete = "Permissions.Cadre.Delete";
            public const string MenuAccess = "Permissions.Cadre.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class WorkType
        {
            public const string View = "Permissions.Work.View";
            public const string Create = "Permissions.Work.Create";
            public const string Edit = "Permissions.Work.Edit";
            public const string Delete = "Permissions.Work.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }
        public static class Work
        {
            public const string View = "Permissions.SubWork.View";
            public const string Create = "Permissions.SubWork.Create";
            public const string Edit = "Permissions.SubWork.Edit";
            public const string Delete = "Permissions.SubWork.Delete";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete };
        }
        public static class CaseKind
        {
            public const string View = "Permissions.CaseKind.View";
            public const string Create = "Permissions.CaseKind.Create";
            public const string Edit = "Permissions.CaseKind.Edit";
            public const string Delete = "Permissions.CaseKind.Delete";
            public const string MenuAccess = "Permissions.CaseKind.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class CaseCateogy
        {
            public const string View = "Permissions.CaseCateogy.View";
            public const string Create = "Permissions.CaseCateogy.Create";
            public const string Edit = "Permissions.CaseCateogy.Edit";
            public const string Delete = "Permissions.CaseCateogy.Delete";
            public const string MenuAccess = "Permissions.CaseCateogy.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class CaseStage
        {
            public const string View = "Permissions.CaseStage.View";
            public const string Create = "Permissions.CaseStage.Create";
            public const string Edit = "Permissions.CaseStage.Edit";
            public const string Delete = "Permissions.CaseStage.Delete";
            public const string MenuAccess = "Permissions.CaseStage.Menu";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class CourtDistrict
        {
            public const string View = "Permissions.CourtDistrict.View";
            public const string Create = "Permissions.CourtDistrict.Create";
            public const string Edit = "Permissions.CourtDistrict.Edit";
            public const string Delete = "Permissions.CourtDistrict.Delete";
            public const string MenuAccess = "Permissions.CourtDistrict.Menu";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class CourtFee
        {
            public const string View = "Permissions.CourtFee.View";
            public const string Create = "Permissions.CourtFee.Create";
            public const string Edit = "Permissions.CourtFee.Edit";
            public const string Delete = "Permissions.CourtFee.Delete";
            public const string MenuAccess = "Permissions.CourtFee.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class Court
        {
            public const string View = "Permissions.Court.View";
            public const string Create = "Permissions.Court.Create";
            public const string Edit = "Permissions.Court.Edit";
            public const string Delete = "Permissions.Court.Delete";
            public const string MenuAccess = "Permissions.Court.Menu";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class CourtType
        {
            public const string View = "Permissions.CourtType.View";
            public const string Create = "Permissions.CourtType.Create";
            public const string Edit = "Permissions.CourtType.Edit";
            public const string Delete = "Permissions.CourtType.Delete";
            public const string MenuAccess = "Permissions.CourtType.Menu";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class ExpenceHead
        {
            public const string View = "Permissions.ExpenceHead.View";
            public const string Create = "Permissions.ExpenceHead.Create";
            public const string Edit = "Permissions.ExpenceHead.Edit";
            public const string Delete = "Permissions.ExpenceHead.Delete";
            public const string MenuAccess = "Permissions.ExpenceHead.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { View, Create, Edit, Delete, MenuAccess };
        }
        public static class Report
        {
            public const string MenuAccess = "Permissions.Report.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { MenuAccess };
        }

        public static class Tool
        {
            public const string MenuAccess = "Permissions.Tool.MenuAccess";
            public static List<string> GetAllPermissions() =>
               new List<string> { MenuAccess };
        }

    }
}