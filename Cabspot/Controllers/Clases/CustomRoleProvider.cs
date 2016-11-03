using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Cabspot.Models;

namespace Cabspot.Controllers.Clases
{
    public class CustomRoleProvider: RoleProvider
    {
        public CustomRoleProvider() { }

        public override bool IsUserInRole(string username, string roleName)
        {
            return true;
        }

        public override string[] GetRolesForUser(string username)
        {
            Cabspot.Models.CabspotDB db = new Cabspot.Models.CabspotDB();

            var roles = db.empleados.Include(e => e.roles).Where(e => e.usuario.Equals(username)).Select(e => e.roles.rol).ToArray();

            return roles;
        }

        // -- Snip --

        public override string[] GetAllRoles()
        {
            using (var db = new Cabspot.Models.CabspotDB())
            {
                return db.roles.Select(r => r.rol).ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (var db = new Cabspot.Models.CabspotDB())
            {
                return db.empleados.Where(r => r.roles.rol.Equals(roleName)).Select(r => r.usuario).ToArray();
            }
        }


        //public override string[] GetRolesForUser(string username)
        //{
        //    using (Cabspot.Models.CabspotDB db = new Cabspot.Models.CabspotDB())
        //    {
        //        var objUser = db.empleados.FirstOrDefault(x => x.usuario == username);
        //        if (objUser == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            string[] ret = { objUser.roles.rol };
        //            return ret;
        //        }
        //    }
        //}

        //public override bool IsUserInRole(string user, string roleName)
        //{
        //    return true;
        //}

        public override string ApplicationName { get; set; }
        public override void CreateRole(string roleName) { }
        //public override void AddUsersToRoles(string[] usernames, string[] roleNames);
        //public override void CreateRole(string roleName);
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) 
        {
            return true;
        }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch) 
        {
            return new string[] { "" };
        
        }
        //public override string[] GetAllRoles();
        //public override string[] GetRolesForUser(string username);
        //public override string[] GetUsersInRole(string roleName);
        //public override bool IsUserInRole(string username, string roleName);
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) { }
        public override bool RoleExists(string roleName) {
            return false;
        }

        public override void AddUsersToRoles(string[] users, string[] roles)   {    }
    }
}