using System.Collections.Generic;
using System.IO;
using System.Web.Optimization;

namespace Contacts.App_Start
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			// Scripts
			bundles.Add(new ScriptBundle("~/bundles/layoutscripts") {Orderer = new AsDefinedBundleOrderer()}.Include(
				"~/Scripts/Common/json.js",
				"~/Scripts/Common/jquery-{version}.js",
				"~/Scripts/Common/jquery-ui-{version}.js",
				"~/Scripts/Common/jquery.unobtrusive*",
				"~/Scripts/Common/jquery.validate*"));

			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
				"~/Scripts/Common/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/contacts").Include(
				"~/Scripts/Views/contacts.js"));

			// Styles
			bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

			bundles.Add(new StyleBundle("~/Content/themes/base/css") {Orderer = new AsDefinedBundleOrderer()}.Include(
				"~/Content/themes/base/jquery.ui.core.css",
				"~/Content/themes/base/jquery.ui.resizable.css",
				"~/Content/themes/base/jquery.ui.selectable.css",
				"~/Content/themes/base/jquery.ui.accordion.css",
				"~/Content/themes/base/jquery.ui.autocomplete.css",
				"~/Content/themes/base/jquery.ui.button.css",
				"~/Content/themes/base/jquery.ui.dialog.css",
				"~/Content/themes/base/jquery.ui.slider.css",
				"~/Content/themes/base/jquery.ui.tabs.css",
				"~/Content/themes/base/jquery.ui.datepicker.css",
				"~/Content/themes/base/jquery.ui.progressbar.css",
				"~/Content/themes/base/jquery.ui.theme.css"));
		}
	}

	public class AsDefinedBundleOrderer : IBundleOrderer
	{
		public IEnumerable<FileInfo> OrderFiles(BundleContext context, IEnumerable<FileInfo> files)
		{
			return files;
		}
	}
}