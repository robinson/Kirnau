﻿namespace Kirnau.Survey.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Samples.Web.ClaimsUtillities;
    using Kirnau.Survey.Admin.Models;
    using Kirnau.Survey.Admin.Security;
    using Kirnau.Survey.Web.Shared.Models;
    using Kirnau.Survey.Web.Shared.Stores;
    
    [RequireHttps]
    [AuthenticateAndAuthorizeKirnauAttribute(Roles = Kirnau.Roles.TenantAdministrator)]
    public class ManagementController : Controller
    {
        private readonly ITenantStore tenantStore;

        public ManagementController(ITenantStore tenantStore)
        {
            this.tenantStore = tenantStore;
        }

        public ActionResult Index()
        {
            var model = new TenantPageViewData<IEnumerable<string>>(this.tenantStore.GetTenantNames())
            {
                Title = "Subscribers"
            };
            return this.View(model);
        }

        public ActionResult Detail(string tenant)
        {
            var contentModel = this.tenantStore.GetTenant(tenant);
            var model = new TenantPageViewData<Tenant>(contentModel)
            {
                Title = string.Format("{0} details", contentModel.Name)
            };
            return this.View(model);
        }

        public ActionResult New()
        {
            var model = new TenantPageViewData<Tenant>(new Tenant())
            {
                Title = "New Tenant"
            };
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(Tenant tenant)
        {
            if (string.IsNullOrWhiteSpace(tenant.Name) || tenant.Name.Equals("new", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.ModelState.AddModelError("ContentModel.Name", "* Organization name cannot be empty or 'new'.");
            }
            else if (this.tenantStore.GetTenant(tenant.Name) != null)
            {
                this.ModelState.AddModelError("ContentModel.Name", "* Organization name already used.");
            }

            if (!this.ModelState.IsValid)
            {
                var model = new TenantPageViewData<Tenant>(tenant)
                {
                    Title = "New Tenant : Error!"
                };
                return this.View(model);
            }

            this.tenantStore.SaveTenant(tenant);

            return this.RedirectToAction("Index");
        }
    }
}
