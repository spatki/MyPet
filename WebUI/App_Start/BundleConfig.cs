using System.Web;
using System.Web.Optimization;

namespace ProcessAccelerator.WebUI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
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
                        "~/Content/pa/datetime-picker/bootstrap-datetimepicker.min.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
            // Bundles needed by Process Accelerator
            bundles.Add(new ScriptBundle("~/bundles/PADependencies").Include(
                        "~/Scripts/jquery-1.11.0.min.js",
                        "~/Scripts/pa/jquery-ui.min.js",
                        "~/Scripts/pa/jquery.form.js", // 2
                        "~/Scripts/pa/slimscroll/jquery.slimscroll.min.js",
                        "~/Scripts/pa/touchswp/jquery.touchSwipe.min.js",
                        "~/Scripts/pa/icheck/jquery.icheck.min.js",
                        "~/Scripts/pa/visualize/visualize.jQuery.min.js",
                        "~/Scripts/pa/chosen/chosen.jquery.js", // 4
                        "~/Scripts/pa/tagsinput/jquery.tagsinput.js", // 5
                        "~/Scripts/pa/datetime-picker/moment.js", // 6
                        "~/Scripts/pa/datetime-picker/bootstrap-datetimepicker.min.js",
                        "~/Scripts/pa/inputmask/jquery.inputmask.bundle.js", // 7
                        "~/Scripts/pa/msdropdown/jquery.dd.min.js",
                        "~/Scripts/pa/minicolors/jquery.minicolors.js", // 8
                        "~/Scripts/pa/bwizard/bwizard.min.js",
                        "~/Scripts/pa/validate/jquery.validate.js",  // 1
                        "~/Scripts/pa/data-tables/jquery.dataTables.min.js",
                        "~/Scripts/pa/data-tables/DT_bootstrap.js", // 9
                        "~/Scripts/pa/data-tables/table-show-details.js", // 10
                        "~/Scripts/pa/data-tables/jquery.jeditable.js", // 11
                        "~/Scripts/pa/bootstrap-tree/bootstrap-tree.js", // 12
                        "~/Scripts/pa/tiles/jquery.freetile.min.js",
                        "~/Scripts/pa/contact-list/slidernav.js", // 13
                        "~/Scripts/pa/pnotify/jquery.pnotify.min.js",
                        "~/Scripts/pa/imgareaselect/jquery.imgareaselect.min.js",
                        "~/Scripts/pa/JSON2.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/pa/bootstrap-gallery/bootstrap-image-gallery.min.js",
                        "~/Scripts/pa/bootstrap-gallery/jquery.blueimp-gallery.min.js",
                        "~/Scripts/pa/bootstrap.js"));


            bundles.Add(new ScriptBundle("~/bundles/PAScripts").Include(
                        "~/Scripts/pa/custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/calendar").Include(
                        "~/Scripts/pa/fullcalendar/fullcalendar.js", // 3
                        "~/Scripts/pa/pa_calendar.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/editor").Include(
                    "~/Scripts/pa/ckeditor/ckeditor.js"
                    ));

            bundles.Add(new StyleBundle("~/Content/cssPA").Include(
                        "~/Content/pa/chosen.css",
                        "~/Content/pa/visualize.css",
                        "~/Content/pa/fullcalendar.css",
                        "~/Content/pa/bootstrap-tree.css",
                        "~/Content/pa/data-tables/data-table.css",
                        "~/Content/pa/bootstrap-slider/slider.css",
                        "~/Content/pa/datetime-picker/bootstrap-datetimepicker.min.css",
                        "~/Content/pa/pnotify/jquery.pnotify.default.css",
                        "~/Content/pa/bootstrap/bootstrap.css",
                        "~/Content/pa/base.css"));
        }
    }
}