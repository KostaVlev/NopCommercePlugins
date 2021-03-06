﻿using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.HomePageNewProducts.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Widgets.HomePageNewProducts.NumberOfProducts")]
        [Display(Name = "Number of products to display")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int NumberOfProducts { get; set; }
    }
}