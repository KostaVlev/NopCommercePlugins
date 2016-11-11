using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.HomePageNewProducts.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.HomePageNewProducts.Controllers
{
    public class WidgetsHomePageNewProductsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;

        public WidgetsHomePageNewProductsController(
            IWorkContext workContext, 
            IStoreService storeService, 
            ISettingService settingService,
            ILocalizationService localizationService,
            IStoreContext storeContext)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(int? productThumbPictureSize)
        {
            var homePageNewProductsSettings = _settingService.LoadSetting<HomePageNewProductsSettings>(_storeContext.CurrentStore.Id);
            var productController = DependencyResolver.Current.GetService<ProductController>();
            productController.ControllerContext = new ControllerContext(this.Request.RequestContext, productController);

            this.RouteData.Values["controller"] = "Product";
            this.RouteData.Values["action"] = "HomepageProducts";

            var actionResult = productController.HomepageProducts(productThumbPictureSize);

            var model = ((PartialViewResult)actionResult).Model as IList<ProductOverviewModel>;
            var result = model.Where(m => m.MarkAsNew).Take(homePageNewProductsSettings.NumberOfProducts).ToList();

            return PartialView(result);
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();

            return View("~/Plugins/Widgets.HomePageNewProducts/Views/HomePageNewProducts/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var homePageNewProductsSettings = _settingService.LoadSetting<HomePageNewProductsSettings>(storeScope);
            homePageNewProductsSettings.NumberOfProducts = model.NumberOfProducts;

            this._settingService.SaveSetting(homePageNewProductsSettings, n => n.NumberOfProducts, storeScope, false);
            this._settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}
