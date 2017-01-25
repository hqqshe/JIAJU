using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    /// <summary>
    /// 销售统计
    /// </summary>
    public class SaleStatisticsController : ParentController
    {
        /// <summary>
        /// 门店销售统计列表
        /// </summary>
        /// <returns></returns>
        public ActionResult StoreStatistics()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int shopId = Convert.ToInt32(Session["ShopId"]);
            int parentShopId = Convert.ToInt32(Session["ParentShopId"]);
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            List<YNShopInfo> shopInfoList = new List<YNShopInfo>();
            //不是管理员账号折直接返回
            if (parentShopId != 0)
            {
                YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(shopId);
                shopInfoList.Add(yNShopInfo);
            }
            else
            {
                shopInfoList = shopInfoController.getShopInfoList(shopId, "", 1, 100);
            }
            ViewBag.shopInfoList = shopInfoList;
            return View();
        }

        /// <summary>
        /// 根据
        /// </summary>
        /// <returns></returns>
        public JsonResult getUserListByShopId()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int shopId = Convert.ToInt32(Session["ShopId"]);
            int parentShopId = Convert.ToInt32(Session["ParentShopId"]);
            //不是管理员账号折直接返回
            if (parentShopId != 0)
            {
                shopId = parentShopId;
            }
            int shopIdChild = Convert.ToInt32(Request.QueryString["shopId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            List<YNShopUser> shopUserList = shopUserController.getShopUserListByShopId(shopId, "", shopIdChild, 1, 100);
            List<Dictionary<string, object>> userList = new List<Dictionary<string, object>>();
            Dictionary<string, object> temp = new Dictionary<string,object>();
            temp.Add("id",-1);
            temp.Add("name","所有");
            userList.Add(temp);
            for (int i = 0; i < shopUserList.Count; i++)
            {
                Dictionary<string, object> _temp = new Dictionary<string, object>();
                _temp.Add("id", shopUserList[i].id);
                _temp.Add("name", shopUserList[i].nick_name);
                userList.Add(_temp);
            }
            return Json(new { code = 1, message = "成功", userList = userList }, JsonRequestBehavior.AllowGet);
        }
    }
}

