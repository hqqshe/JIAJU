using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class ERPemployerController : ParentController
    {
        /// <summary>
        /// 员工列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EmployerList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            List<YNFactoryDepartMent> departMentList = factoryDepartmentController.getDepartmenList(clothFactoryId, "", 1, 50);
            ViewBag.departMentList = departMentList;
            return View();
        }
        /// <summary>
        /// 获取员工列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEmployerList()
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
            int departMentId = Convert.ToInt32(Request.QueryString["departMentId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            List<YNShopUser> shopUserList = shopUserController.getShopUserListByFactoryId(clothFactoryId, departMentId, key, pageIndex, pageSize);
            int count = shopUserController.getShopUserListByFactoryIdCount(clothFactoryId, departMentId, key);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.shopUserList = shopUserList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 获取部门员工列表json数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetEmployerListJson()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int departMentId = Convert.ToInt32(Request.QueryString["departMentId"]);
            string searchKey = Request.QueryString["searchKey"];
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            List<YNShopUser> shopUserList = shopUserController.getShopUserListByFactoryId(clothFactoryId, departMentId, searchKey, 1, 200);
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
            for (int i = 0; i < shopUserList.Count; i++)
            {
                Dictionary<string, object> _obejct = new Dictionary<string, object>();
                _obejct.Add("id", shopUserList[i].id);
                _obejct.Add("account", shopUserList[i].account);
                _obejct.Add("name", shopUserList[i].nick_name);
                _obejct.Add("no", shopUserList[i].employee_no);
                _obejct.Add("remarks", shopUserList[i].factory_remarks);
                dataList.Add(_obejct);
            }
            return Json(new { code = 1, message = "成功", data = dataList }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployerListIgnoreStatusJson()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int departMentId = Convert.ToInt32(Request.QueryString["departMentId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            List<YNShopUser> shopUserList = shopUserController.getShopUserListIgnoreStatusByFactoryId(clothFactoryId, departMentId);
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
            for (int i = 0; i < shopUserList.Count; i++)
            {
                Dictionary<string, object> _obejct = new Dictionary<string, object>();
                _obejct.Add("id", shopUserList[i].id);
                _obejct.Add("account", shopUserList[i].account);
                _obejct.Add("name", shopUserList[i].nick_name);
                _obejct.Add("no", shopUserList[i].employee_no);
                _obejct.Add("status", shopUserList[i].status);
                dataList.Add(_obejct);
            }
            return Json(new { code = 1, message = "成功", data = dataList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 员工信息详情
        /// </summary>
        /// <returns></returns>
        public ActionResult employerInfo()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                id = Convert.ToInt32(Request.QueryString["id"]);
            }
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserById(id, clothFactoryId);
            if (yNShopUser == null)
            {
                yNShopUser = new YNShopUser();
            }
            if (!string.IsNullOrEmpty(yNShopUser.r_p_d))
            {
                yNShopUser.password = API.Controllers.EncryptionController.DESPWDecrypt(yNShopUser.r_p_d);
            }
            else
            {
                yNShopUser.password = "";
            }
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            List<YNFactoryDepartMent> departMentList = factoryDepartmentController.getDepartmenList(clothFactoryId, "", 1, 50);
            ViewBag.departMentList = departMentList;
            ViewBag.yNShopUser = yNShopUser;
            return View();
        }
        /// <summary>
        /// 保存员工信息
        /// </summary>
        /// <returns></returns>
        public JsonResult saveEmployer()
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
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = new YNShopUser();
            if (id != 0)
            {
                yNShopUser = shopUserController.GetFactoryUserById(id, clothFactoryId);
                if (yNShopUser == null)
                {
                    return getLoginJsonMessage(0, "参数错误");
                }
            }
            yNShopUser.account = Request.Form["account"];
            yNShopUser.phone = Request.Form["phone"];
            yNShopUser.password = Request.Form["password"];
            if (string.IsNullOrEmpty(yNShopUser.account) || string.IsNullOrEmpty(yNShopUser.password))
            {
                return getLoginJsonMessage(0, "账号和密码不能为空");
            }
            if (id != 0)
            {
                if (shopUserController.GetUserByAccountPhoneExists(yNShopUser.account, yNShopUser.phone, id))
                {
                    return getLoginJsonMessage(0, "账号不能重复，请重新设置账号");
                }
            }
            else
            {
                if (shopUserController.GetUserByAccountPhoneExists(yNShopUser.account, yNShopUser.phone))
                {
                    return getLoginJsonMessage(0, "账号不能重复，请重新设置账号");
                }
            }
            yNShopUser.r_p_d = API.Controllers.EncryptionController.DESPWEncrypt(yNShopUser.password);
            yNShopUser.password = API.Controllers.EncryptionController.MD5(yNShopUser.password + API.Controllers.TokenController.DIYToken);
            yNShopUser.email = Request.Form["email"];
            yNShopUser.nick_name = Request.Form["nick_name"];
            if (string.IsNullOrEmpty(yNShopUser.nick_name))
            {
                return getLoginJsonMessage(0, "员工姓名不能为空");
            }
            yNShopUser.employee_no = Request.Form["employee_no"]; //员工工号
            if (string.IsNullOrWhiteSpace(yNShopUser.employee_no))
            {
                return getLoginJsonMessage(0, "员工工号不能为空");
            }
            yNShopUser.employee_no = yNShopUser.employee_no.Trim();
            if (shopUserController.EmployeeNoIsExists(clothFactoryId, yNShopUser.employee_no, yNShopUser.id > 0 ? (int?)yNShopUser.id : null))
            {
                return getLoginJsonMessage(0, "员工工号不能重复");
            }

            yNShopUser.type = API.Controllers.ShopUserController.type_3;
            yNShopUser.role_type = API.Controllers.ShopUserController.role_type1;
            yNShopUser.status = API.Controllers.ShopUserController.status_0;
            yNShopUser.cloth_factory_id = clothFactoryId;
            yNShopUser.cloth_factory_name = Convert.ToString(Session["Name"]);
            yNShopUser.factory_department_id = Convert.ToInt32(Request.Form["factory_department_id"]);
            yNShopUser.factory_remarks = Request.Form["factory_remarks"];
            yNShopUser.employ_type = Convert.ToInt32(Request.Form["employ_type"]);
            if (id != 0)
            {
                yNShopUser.modify_time = DateTime.Now;
                shopUserController.SaveChanges();
                return getLoginJsonMessage(1, "保存成功");
            }
            else
            {
                yNShopUser.create_time = DateTime.Now;
                yNShopUser.modify_time = DateTime.Now;
                shopUserController.Create(yNShopUser);
                return getLoginJsonMessage(1, "添加成功");
            }
        }
        /// <summary>
        /// 删除员工信息
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteEmployer()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            shopUserController.DeleteFactoryUserById(id, clothFactoryId);
            return getLoginJsonMessage(1, "删除成功");
        }



        #region 工号条码

        public ActionResult GetEmployerInfo(string employeeNo)
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            var shopUser = shopUserController.GetFactoryUserByEmployeeNo(employeeNo, clothFactoryId);
            if (shopUser == null)
            {
                return Json(new { code = 0, message = "不存在的工号" }, JsonRequestBehavior.AllowGet);
            }
            var data = new
            {
                Name = shopUser.nick_name,
                EmployeeNo = shopUser.employee_no,
                Id = shopUser.id,
                BarCode = "3_" + shopUser.id,
            };


            return Json(new { code = 1, message = "成功", data = data }, JsonRequestBehavior.AllowGet);
        }

        //下载员工工号条码
        public ActionResult DownloadEmplyerBarCodeData(int? departmentId)
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            if (departmentId == null)
            {
                return Json(new { code = 0, message = "请选择生产线"  }, JsonRequestBehavior.AllowGet);
            }
            string searchKey = Request.QueryString["searchKey"];
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);

            var factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            var department = factoryDepartmentController.GetDepartmenById(departmentId.Value, clothFactoryId);
            if (department == null)
            {
                return Json(new { code = 0, message = "生产线不存在" }, JsonRequestBehavior.AllowGet);
            }
            
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            var shopUserList = shopUserController.getUsersByDepartmentId(departmentId.Value, clothFactoryId, searchKey);

            var dataList = new List<Dictionary<string, object>>();
            var packageStepBarGroup = new Dictionary<string, object>();
            var employerBarCodeGroup = new List<Dictionary<string, object>>();

            packageStepBarGroup.Add("title", "员工条码 " + department.department_name);
            packageStepBarGroup.Add("data", employerBarCodeGroup);

            dataList.Add(packageStepBarGroup);

            foreach (var shopUser in shopUserList)
            {
                var temp = new Dictionary<string, object>();
                temp.Add("name", shopUser.employee_no + "-" + shopUser.nick_name);
                temp.Add("code", shopUser.employee_no);
                employerBarCodeGroup.Add(temp);
            }
            return Json(new { code = 1, message = "成功", data = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintBarCodes()
        {
            return View();
        }

        #endregion

    }
}
