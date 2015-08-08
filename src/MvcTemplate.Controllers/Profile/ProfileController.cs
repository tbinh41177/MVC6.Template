﻿using Microsoft.AspNet.Mvc;
using MvcTemplate.Components.Alerts;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Components.Security;
using MvcTemplate.Objects;
using MvcTemplate.Resources.Views.AccountView;
using MvcTemplate.Services;
using MvcTemplate.Validators;

namespace MvcTemplate.Controllers
{
    [AllowUnauthorized]
    public class ProfileController : ValidatedController<IAccountValidator, IAccountService>
    {
        public ProfileController(IAccountValidator validator, IAccountService service)
            : base(validator, service)
        {
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!Service.IsActive(CurrentAccountId))
                return LogOut();

            return View(Service.Get<ProfileEditView>(CurrentAccountId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([BindExcludeId] ProfileEditView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return LogOut();

            if (!Validator.CanEdit(profile))
                return View(profile);

            Service.Edit(profile);

            Alerts.Add(AlertType.Success, Messages.ProfileUpdated);

            return RedirectToAction("Edit");
        }

        [HttpGet]
        public ActionResult Delete()
        {
            if (!Service.IsActive(CurrentAccountId))
                return LogOut();

            Alerts.Add(AlertType.Danger, Messages.ProfileDeleteDisclaimer, 0);

            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed([BindExcludeId] ProfileDeleteView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return LogOut();

            if (!Validator.CanDelete(profile))
            {
                Alerts.Add(AlertType.Danger, Messages.ProfileDeleteDisclaimer, 0);

                return View();
            }

            Service.Delete(CurrentAccountId);

            return LogOut();
        }

        private RedirectToActionResult LogOut()
        {
            return RedirectToAction("Logout", "Auth");
        }
    }
}
