using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DWServices.Common
{
    public class User
    {
        private String id;

        private String usercode;

        public String Usercode
        {
            get { return usercode; }
            set { usercode = value; }
        }

        private String orgname;

        public String Orgname
        {
            get { return orgname; }
            set { orgname = value; }
        }
        public String Id
        {
            get { return id; }
            set { id = value; }
        }
        private String username;

        public String Username
        {
            get { return username; }
            set { username = value; }
        }
        private String password;

        public String Password
        {
            get { return password; }
            set { password = value; }
        }
        private String permissions;

        public String Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        } 
    }
}