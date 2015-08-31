using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace TaskTracker.Objects
{
    public class BaseController: Controller
    {
        protected AdUser CurUser { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DisplayCurUser();
            base.OnActionExecuting(filterContext);
        }

        //private static NetworkCredential nc = GetAdUserCredentials();
        //public static NetworkCredential GetAdUserCredentials()
        //{
        //    string accUserName = @"UN1T\adUnit_prog";
        //    string accUserPass = "1qazXSW@";

        //    string domain = "UN1T";//accUserName.Substring(0, accUserName.IndexOf("\\"));
        //    string name = "adUnit_prog";//accUserName.Substring(accUserName.IndexOf("\\") + 1);

        //    NetworkCredential nc = new NetworkCredential(name, accUserPass, domain);

        //    return nc;
        //}

        [NonAction]
        public AdUser GetCurUser()
        {
            AdUser user = new AdUser();
            try
            {
                using (WindowsImpersonationContextFacade impersonationContext
                    = new WindowsImpersonationContextFacade(
                        AdHelper.GetAdUserCredentials()))
                {
                    var wi = (WindowsIdentity)base.User.Identity;
                    if (wi.User != null)
                    {
                        var domain = new PrincipalContext(ContextType.Domain);
                        string sid = wi.User.Value;
                        user.Sid = sid;
                        var login = wi.Name.Remove(0, wi.Name.IndexOf("\\", StringComparison.CurrentCulture) + 1);
                        user.Login = login;
                        var userPrincipal = UserPrincipal.FindByIdentity(domain, login);
                        if (userPrincipal != null)
                        {
                            var mail = userPrincipal.EmailAddress;
                            var name = userPrincipal.DisplayName;
                            user.Email = mail;
                            user.FullName = name;
                            //user.AdGroups = new List<AdGroup>();
                            //var wp = new WindowsPrincipal(wi);
                            //foreach (var role in AdUserGroup.GetList())
                            //{
                            //    var grpSid = new SecurityIdentifier(role.Sid);
                            //    if (wp.IsInRole(grpSid))
                            //    {
                            //        user.AdGroups.Add(role.Group);
                            //    }
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return user;
        }

        protected AdUser DisplayCurUser()
        {
            CurUser = GetCurUser();
            if (CurUser == new AdUser()) RedirectToAction("AccessDenied", "Error");
            ViewBag.CurUser = CurUser;
            return CurUser;
        }
    }
}