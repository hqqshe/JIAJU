using System;
using System.Web.Mvc;

namespace YNDIY.Admin.Controllers
{
	public class HomeController : ParentController
	{
        //商家系统首页
        public ActionResult Index()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //string _referrer = Convert.ToString(Session["_reffer_uri"]);
            //if (!string.IsNullOrEmpty(_referrer) && !_referrer.Equals("/") && !_referrer.Equals("/Home/Index"))
            //{
            //    return Redirect(_referrer);
            //}
            //return View();
            return Redirect("/ERPOrder/OrderList");
        }
	}
}