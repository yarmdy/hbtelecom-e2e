using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;

namespace DWServices.page
{
    public partial class index : System.Web.UI.Page
    { 
        protected void Page_Load(object sender, EventArgs e)
        {
            DWServices.Common.User user = (DWServices.Common.User)Session["user"];
            if (Session["user"] == null)
            {
                Response.Redirect("../login.html");
            }
            else {  

            }
        }
    }
}