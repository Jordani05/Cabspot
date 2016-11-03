using Cabspot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cabspot.Controllers.Clases
{
    public class CustomAuthorizeRoleAttribute: AuthorizeAttribute
    {

        CabspotDB context = new CabspotDB(); // my entity  
       private readonly string[] allowedroles;
       public CustomAuthorizeRoleAttribute(params string[] roles)  
       {  
          this.allowedroles = roles;  
       }

       protected override bool AuthorizeCore(HttpContextBase httpContext)
       {           
           foreach (var role in allowedroles)
           {
               var user = context.empleados.Where(m => m.usuario == httpContext.User.Identity.Name && m.roles.rol == role); // checking active users with allowed roles.  
               if (user.Count() > 0)
               {
                   return true; 
               }
           }
           return false;  
       }

       protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
       {
           if (filterContext.HttpContext.Request.IsAuthenticated)
           {

               filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary 
                                   {
                                       { "action", "Forbidden" },
                                       { "controller", "Error" }
                                   });
                   
                   //new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
           }
           else
           {
               base.HandleUnauthorizedRequest(filterContext);
           }
       }


    }
}