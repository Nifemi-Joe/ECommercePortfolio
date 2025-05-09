// ECommercePortfolio.Web/Areas/Admin/AdminAreaRegistration.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommercePortfolio.Web.Areas.Admin
{
    // In ASP.NET Core, we register areas in Startup instead of using AreaRegistration
    // This file is just for organizational purposes
    public class AdminAreaRegistration
    {
        public static void RegisterArea(IMvcBuilder mvcBuilder)
        {
            // This would be done in Startup.cs using services.AddControllersWithViews()
            // .AddRazorOptions(options => { options.AllowAreaViewLocations = true; })
        }
    }
}