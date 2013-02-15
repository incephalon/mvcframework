using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCFramework.Web.Infrastructure
{
    public class PartialRazorViewEngine : RazorViewEngine
    {
        private static readonly string[] NewPartialViewLocationFormats = new[] 
        { 
        "~/Views/{1}/Partials/{0}.cshtml",
        "~/Views/Shared/Partials/{0}.cshtml" 
        };


        public PartialRazorViewEngine()
        {
            PartialViewLocationFormats = PartialViewLocationFormats.Union(NewPartialViewLocationFormats).ToArray();
        }
    }
}