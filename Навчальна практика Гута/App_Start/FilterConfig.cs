using System.Web;
using System.Web.Mvc;

namespace Навчальна_практика_Гута
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
