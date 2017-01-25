using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class ERPPartmentController : ParentController
    {
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        public ActionResult PartmentList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPartmentList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string key = Request.QueryString["searchKey"];
            int pageIndex = 1;
            int pageSize = 10;
            if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            List<YNFactoryDepartMent> departmentList = factoryDepartmentController.getDepartmenList(clothFactoryId, key, pageIndex, pageSize);
            int count = factoryDepartmentController.getFactoryOrderListByShopCount(clothFactoryId, key);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.departmentList = departmentList;
            ViewBag.page = page;
            return View();
        }
        public ActionResult GetPartmentsIgnoreStatus()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            List<YNFactoryDepartMent> departMentList = factoryDepartmentController.getDepartmenListIgnoreStatus(clothFactoryId); 
    
            return Json(new { code = 1, message = "成功", data = departMentList }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取部门列表的json数据
        /// </summary>
        /// <returns></returns>
        public JsonResult getDepartmentList()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            List<YNFactoryDepartMent> departMentList = factoryDepartmentController.getDepartmenList(clothFactoryId, "", 1, 100);
            return Json(new { code = 1, message = "成功", data = departMentList }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 保存部门信息
        /// </summary>
        /// <returns></returns>
        public JsonResult saveDepartment()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = 0;
            if (!string.IsNullOrEmpty(Request.Form["id"]))
            {
                id = Convert.ToInt32(Request.Form["id"]);
            }
            YNFactoryDepartMent yNFactoryDepartMent = new YNFactoryDepartMent();
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            if (id != 0)
            {
                yNFactoryDepartMent = factoryDepartmentController.GetDepartmenById(id, clothFactoryId);
                if (yNFactoryDepartMent == null)
                {
                    return getLoginJsonMessage(0, "参数错误");
                }
            }
            yNFactoryDepartMent.cloth_factory_id = clothFactoryId;
            yNFactoryDepartMent.department_name = Request.Form["department_name"];
            if (string.IsNullOrEmpty(yNFactoryDepartMent.department_name))
            {
                return getLoginJsonMessage(0, "部门名称不能为空");
            }
            yNFactoryDepartMent.describle = Request.Form["describle"];
            yNFactoryDepartMent.operator_id = Convert.ToInt32(Session["UserId"]);
            yNFactoryDepartMent.operator_name = Convert.ToString(Session["NickName"]);
            if (id == 0)
            {
                yNFactoryDepartMent.create_time = DateTime.Now;
                yNFactoryDepartMent.modify_time = DateTime.Now;
                factoryDepartmentController.Create(yNFactoryDepartMent, clothFactoryId);
            }
            else
            {
                yNFactoryDepartMent.modify_time = DateTime.Now;
                factoryDepartmentController.SaveChanges();
            }
            return getLoginJsonMessage(1, "成功");
        }
        /// <summary>
        /// 删除部门信息
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteDepartment()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            factoryDepartmentController.Delete(id, clothFactoryId);
            return getLoginJsonMessage(1, "删除成功");
        }
    }
}
