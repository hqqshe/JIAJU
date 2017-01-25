using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;

namespace YNDIY.Admin.Controllers
{
    public class OrderFactoryController : ParentController
    {
        /// <summary>
        /// 衣服加工订单【工厂】
        /// </summary>
        /// <returns></returns>
        public ActionResult clothFactoryOrderList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        public ActionResult clothFactoryOrderListItems()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string key = Request.QueryString["searchKey"];
            int factory_status = API.Controllers.FactoryOrderController.status_all;
            int pageIndex = 1;
            int pageSize = 10;
            if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["page_index"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["page_index"]);
            }
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> factoryOrderList = factoryOrderController.getFactoryOrderListByFactory(key, clothFactoryId, API.Controllers.FactoryOrderController.status_all, factory_status, API.Controllers.FactoryOrderController.status_all, API.Controllers.FactoryOrderController.status_all, pageIndex, pageSize);
            int count = factoryOrderController.getFactoryOrderListByFactoryCount(key, clothFactoryId, API.Controllers.FactoryOrderController.status_all, factory_status, API.Controllers.FactoryOrderController.status_all, API.Controllers.FactoryOrderController.status_all);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.factoryOrderList = factoryOrderList;
            ViewBag.page = page;
            return View();
        }

        /// <summary>
        /// 设置工厂信息
        /// </summary>
        /// <returns></returns>
        public ActionResult clothFactoryInfo()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);

            API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
            YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(clothFactoryId);
            ViewBag.yNClothFactoryInfo = yNClothFactoryInfo;
            return View();
        }
      
        /// <summary>
        /// 保存工厂信息
        /// </summary>
        /// <returns></returns>
        public JsonResult saveClothFactoryInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);

            API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
            YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(clothFactoryId);
            yNClothFactoryInfo.cloth_factory_name = Request.Form["cloth_factory_name"];
            yNClothFactoryInfo.describe = Request.Form["describe"];
            yNClothFactoryInfo.link_man = Request.Form["link_man"];
            yNClothFactoryInfo.phone = Request.Form["phone"];
            yNClothFactoryInfo.email = Request.Form["email"];
            yNClothFactoryInfo.address_detail = Request.Form["address_detail"];
            yNClothFactoryInfo.first_response_phone = Request.Form["first_response_phone"];
            yNClothFactoryInfo.second_response_phone = Request.Form["second_response_phone"];
            yNClothFactoryInfo.unit_type = Convert.ToInt32(Request.Form["unit_type"]);
            yNClothFactoryInfo.bar_code_type = Convert.ToInt32(Request.Form["bar_code_type"]);
            Session["ClothFactoryBarType"] = yNClothFactoryInfo.bar_code_type;

            if (yNClothFactoryInfo.enable_worksteps)
            {
                int calcSalaryType = 0;
                int.TryParse(Request.Form["calc_salary_style"], out calcSalaryType);

                yNClothFactoryInfo.calc_salary_style = calcSalaryType;
            }

            yNClothFactoryInfo.modify_time = DateTime.Now;
            clothFactoryInfoController.SaveChanges();
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser  yNShopUser = shopUserController.getFactoryMainUserByFactoryId(clothFactoryId);
            yNShopUser.cloth_factory_name = yNClothFactoryInfo.cloth_factory_name;
            yNShopUser.link_man = yNClothFactoryInfo.link_man;
            yNShopUser.email = yNClothFactoryInfo.email;
            shopUserController.SaveChanges();
            return getLoginJsonMessage(1, "保存成功");
        }

        /// <summary>
        /// 工厂用户中心
        /// </summary>
        public ActionResult factoryCenter()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
    }
}
