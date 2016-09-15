using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using ProcessAccelerator.Core.Model;
using ProcessAccelerator.Data;

namespace ProcessAccelerator.WebUI.Controllers
{
    public static class PACacheManager
    {
        private static MemoryCache _cache = MemoryCache.Default;

        public static IQueryable<vw_org_role_access> MenuOptions
        {
            get
            {
                if (! _cache.Contains("MenuOptions"))
                    GetMenuOptions();
                return _cache.Get("MenuOptions") as IQueryable<vw_org_role_access>;
            }
        }

        public static void Refresh()
        {
            if (_cache.Contains("MenuOptions"))  _cache.Remove("MenuOptions");
        }

        public static void GetMenuOptions()
        {
            Db dbCon = new Db();
            var clientID = ((PAIdentity)HttpContext.Current.User.Identity).clientID;

            var MenuOpt = dbCon.vw_org_role_access.Where(o => o.ClientID == clientID);
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddDays(1);

            _cache.Add("MenuOptions", MenuOpt, cacheItemPolicy);
        }
    }
}