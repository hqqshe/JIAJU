using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using YNDIY.API.Controllers;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class ERPProcedureController : WebController
    {
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public ActionResult SalayDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserByIdIngoreStatus(userId, clothFactoryId);
            ViewBag.yNShopUser = yNShopUser;
            ViewBag.scanStartDate = Request.QueryString["scanStartDate"];
            ViewBag.scanEndDate = Request.QueryString["scanEndDate"];
            ViewBag.userId = Request.QueryString["userId"];
            ViewBag.type = Request.QueryString["type"];
            return View();
        }
         /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        public ActionResult Procedure()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 工序管理
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkPaySet()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);

            //var factoryDepartmentController = new FactoryDepartmentController();
            //var departMents = factoryDepartmentController.getDepartmenListByFactoryId(clothFactoryId);
            //ViewBag.DepartMents = departMents;

            return View();
        }

        /// <summary>
        /// 员工工资详情
        /// </summary> 
        public ActionResult WorkPaySetList(string startDate, string endDate,int? pageIndex,int?pageSize,  string searchKey,int? yixing)
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            if (yixing == null)
            {
                yixing = API.Controllers.FactoryOrderController.status_all;
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            if (pageSize == null)
            {
                pageSize = 10;
            }
            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            API.Controllers.FactoryOrderController factoryOrderController = new FactoryOrderController();
            //List<YNFactoryOrder> factoryOrderList = factoryOrderController.getFactoryOrderListByFactoryBySary(searchKey, clothFactoryId,yixing.Value,pageIndex.Value,pageSize.Value, API.Controllers.FactoryOrderController.status_all,null,startDate,endDate);
            //int count = factoryOrderController.getFactoryOrderListByFactoryBySaryCount(searchKey, clothFactoryId, yixing.Value, API.Controllers.FactoryOrderController.status_all, null, startDate, endDate);
            //API.Controllers.PagesController page = new PagesController();
            //page.GetPage(pageIndex.Value, count, pageSize.Value);
            //ViewBag.factoryOrderList = factoryOrderList;
            //ViewBag.page = page;
            return View();
        }

        /// <summary>
        /// 获取工价信息
        /// </summary>
        /// <returns></returns>
        public JsonResult WorkPaySetDetail()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.FactoryOrderController factoryOrderController = new FactoryOrderController();
            YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
            if (yNFactoryOrder == null)
            {
                return getLoginJsonMessage(0, "数据不存在");
            }
            //if(string.IsNullOrEmpty(yNFactoryOrder.model_detail))
            //{
            //    return getLoginJsonMessage(0, "当前订单工序价格未配置");
            //}
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //List<FactoryProcedureStepNumberOrder> list = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrder.model_detail);
            //return getDataJsonMessage(1, "请登陆", list);
            return null;
        }

        /// <summary>
        /// 保存工序价格调整
        /// </summary>
        /// <returns></returns>
        //public JsonResult saveWorkPaySet()
        //{
        //    if (!checkSession())
        //    {
        //        return getLoginJsonMessage(0, "请登陆");
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    int id = Convert.ToInt32(Request.Form["id"]);
        //    API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //    YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
        //    if (yNFactoryOrder == null)
        //    {
        //        return getLoginJsonMessage(0, "订单不存在");
        //    }
        //    string model_detail = Request.Form["model_detail"];
        //    if (string.IsNullOrEmpty(model_detail))
        //    {
        //        return getLoginJsonMessage(0, "步骤详情不能为空");
        //    }
        //    if(string.IsNullOrEmpty(yNFactoryOrder.model_detail))
        //    {
        //         return getLoginJsonMessage(0, "订单工序价格为设置");
        //    }
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<FactoryProcedureStepNumberOrder> oldList = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrder.model_detail);
        //    List<FactoryProcedureStepNumberOrder> newList = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(model_detail);
        //    for (int i = 0; i < oldList.Count; i++)
        //    {
        //        oldList[i].price = newList[i].price;
        //    }
        //    yNFactoryOrder.model_detail = js.Serialize(oldList);
        //    yNFactoryOrder.modify_time = DateTime.Now;
        //    factoryOrderController.SaveChanges();
        //    //修改工序价格列表
        //    API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new FactoryProcessDetailController();
        //    for (int i = 0; i < oldList.Count; i++)
        //    {
        //        List<YNFactoryProcessDetail> processList = factoryProcessDetailController.getFactoryProcessDetailList(id, clothFactoryId, oldList[i].stepName);
        //        for (int k = 0; k < processList.Count; k++)
        //        {
        //            processList[k].price = oldList[i].price;
        //        }
        //        factoryProcessDetailController.SaveChanges();
        //    }
        //    return getLoginJsonMessage(1, "成功", yNFactoryOrder.id.ToString());
        //}

        /// <summary>
        /// 离线导入条码
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportBarCode()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);

            var factoryDepartmentController = new FactoryDepartmentController();
            var departMents = factoryDepartmentController.getDepartmenListByFactoryId(clothFactoryId);
            ViewBag.DepartMents = departMents;

            return View();
        }

        /// <summary>
        /// 批量条形码录入接口
        /// </summary>
        /// <returns></returns>
        public JsonResult submitBarCode()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string barCode = Request.QueryString["barCode"];
            if (string.IsNullOrEmpty(barCode))
            {
                return getLoginJsonMessage(0, "条码不能为空");
            }

            string gonghao = Request.QueryString["gonghao"];
            if (string.IsNullOrEmpty(gonghao))
            {
                return getLoginJsonMessage(0, "工号不能为空");
            }
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserByEmployeeNo(gonghao, clothFactoryId);
            if (yNShopUser == null)
            {
                return getLoginJsonMessage(0, "工号不存在对应员工");
            }
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new FactoryDepartmentController();
            YNFactoryDepartMent yNFactoryDepartMent = factoryDepartmentController.GetDepartmentIgnoreStatusById(yNShopUser.factory_department_id, clothFactoryId);
            if (yNFactoryDepartMent == null)
            {
                return getLoginJsonMessage(0, "员工部门不存在");
            }

            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new FactoryProcessDetailController();
            API.Controllers.FactoryOrderController factoryOrderController = new FactoryOrderController();
            //家具条形码 状态_条码号 比如:0_123  状态：0表示通过，1表示异常
            var isMatsh = Regex.IsMatch(barCode, @"^[01]_\d+$");
            if(!isMatsh)
            {
               return getLoginJsonMessage(0, "条码编号格式错误");
            }

            var splitArrays = barCode.Split('_');
            var state = Convert.ToInt32(splitArrays[0]);
            var id = Convert.ToInt32(splitArrays[1]);
            YNFactoryProcessDetail yNFactoryProcessDetail = factoryProcessDetailController.getFactoryProcessDetailById(id, clothFactoryId);
            if (yNFactoryProcessDetail == null)
            {
                return getLoginJsonMessage(0, "条码编号错误");
            }
            yNFactoryProcessDetail.user_id = yNShopUser.id;
            yNFactoryProcessDetail.gonghao = yNShopUser.employee_no;
            yNFactoryProcessDetail.user_name = yNShopUser.nick_name;
            yNFactoryProcessDetail.department_id = yNFactoryDepartMent.id;
            yNFactoryProcessDetail.department_name = yNFactoryDepartMent.department_name;
            yNFactoryProcessDetail.scan_date = DateTime.Now;
            yNFactoryProcessDetail.modify_time = DateTime.Now;
            int oldState = yNFactoryProcessDetail.state;

            YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(yNFactoryProcessDetail.factory_order_id, clothFactoryId);
            //if (yNFactoryOrder == null || string.IsNullOrEmpty(yNFactoryOrder.model_detail))
            //{
            //    return getLoginJsonMessage(0, "生产订单信息错误");
            //}
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //List<FactoryProcedureStepNumberOrder> list = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrder.model_detail);
            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (list[i].id == yNFactoryProcessDetail.step_id)
            //    {
            //        //未开始
            //        if (oldState == 0)
            //        {
            //            //完成
            //            if (state == 0)
            //            {
            //                yNFactoryProcessDetail.state = 1;
            //                list[i].noStart -= yNFactoryProcessDetail.number;
            //                list[i].complete += yNFactoryProcessDetail.number;
            //            }
            //            //异常
            //            else if (state == 1)
            //            {
            //                yNFactoryProcessDetail.state = 2;
            //                list[i].noStart -= yNFactoryProcessDetail.number;
            //                list[i].error += yNFactoryProcessDetail.number;
            //            }
            //        }
            //        //已完成
            //        else if (oldState == 1)
            //        {
            //            //完成
            //            if (state == 0)
            //            {
            //            }
            //            //异常
            //            else if (state == 1)
            //            {
            //                yNFactoryProcessDetail.state = 2;
            //                list[i].complete -= yNFactoryProcessDetail.number;
            //                list[i].error += yNFactoryProcessDetail.number;
            //            }
            //        }
            //        //异常
            //        else if (oldState == 2)
            //        {
            //            //完成
            //            if (state == 0)
            //            {
            //                yNFactoryProcessDetail.state = 1;
            //                list[i].error -= yNFactoryProcessDetail.number;
            //                list[i].complete += yNFactoryProcessDetail.number;
            //            }
            //            //异常
            //            else if (state == 1)
            //            {
            //            }
            //        }
            //        list[i].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //        break;
            //    }
            //}
            //bool flag = true;
            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (list[i].error != 0 || list[i].noStart != 0)
            //    {
            //        flag = false;
            //        break;
            //    }
            //}
            //factoryProcessDetailController.SaveChanges();
            ////表示订单已经完成
            //if (flag)
            //{
            //    yNFactoryOrder.factory_order_status = API.Controllers.FactoryOrderController.factory_order_status2;
            //    //List<YNFactoryProcessDetail> processDetailList = factoryProcessDetailController.getFactoryProcessDetailListById(yNFactoryOrder.id, clothFactoryId);
            //    //for (int i = 0; i < processDetailList.Count; i++)
            //    //{
            //    //    processDetailList[i].order_complete_date = DateTime.Now;
            //    //    processDetailList[i].order_complete_state = API.Controllers.FactoryProcessDetailController.order_complete_state1;
            //    //}
            //    //factoryProcessDetailController.SaveChanges();
            //}
            
            //yNFactoryOrder.model_detail = js.Serialize(list);
            //factoryOrderController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }
      
        /// <summary>
        /// 工资统计列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SalayStatistics()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }

        public ActionResult SalayStatisticstList(string scanStartDate, string scanEndDate, int? departmentId, int? userId,int? type)
        {
            if (string.IsNullOrEmpty(scanStartDate))
            {
                return JsonFailed("统计开始时间不能为空");
            }
            if (string.IsNullOrEmpty(scanEndDate))
            {
                return JsonFailed("统计结束时间不能为空");
            }
            ViewBag.ScanDateFrom = scanStartDate;
            ViewBag.ScanDateTo = scanEndDate;
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            if (departmentId == null)
            {
                departmentId = -1;
            }
            if (userId == null)
            {
                userId = -1;
            }
            if (type == null)
            {
                type = 0;
            }
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new FactoryProcessDetailController();
            List<FactoryProcessDetail> processDetailList = factoryProcessDetailController.getFactoryProcessDetailListSalary(clothFactoryId, departmentId.Value, userId.Value,type.Value, 1, 100, scanStartDate, scanEndDate);
            decimal totalPrice = 0;
            if (processDetailList.Count > 0)
            {
                totalPrice = factoryProcessDetailController.getFactoryProcessDetailListSalaryTotal(clothFactoryId, departmentId.Value, userId.Value,type.Value, scanStartDate, scanEndDate);
            }
            ViewBag.processDetailList = processDetailList;
            ViewBag.totalPrice = totalPrice;
            return View();
        }

        /// <summary>
        /// 员工工资详情
        /// </summary> 
        public JsonResult EmployeeSalayStatisticsDetail(string scanStartDate, string scanEndDate, int userId,int type)
        {
            if (string.IsNullOrEmpty(scanStartDate))
            {
                return JsonFailed("统计开始时间不能为空");
            }
            if (string.IsNullOrEmpty(scanEndDate))
            {
                return JsonFailed("统计结束时间不能为空");
            }

            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            var shopUserController = new API.Controllers.ShopUserController();
            var shopUser = shopUserController.GetFactoryUserById(userId, clothFactoryId);
            if (shopUser == null)
            {
                return JsonFailed("员工不存在");
            }
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new FactoryProcessDetailController();
            List<FactoryProcessDetail> processDetailList = factoryProcessDetailController.getFactoryProcessDetailListSalaryDetail(clothFactoryId, -1, userId,type, scanStartDate, scanEndDate);
            API.Controllers.FactoryOrderController factoryOrderController = new FactoryOrderController();
            List<SalayDetail> salayDetailList = new List<SalayDetail>();
            for (int i = 0; i < processDetailList.Count; i++)
            {
                if (salayDetailList.Count() == 0 || (salayDetailList[salayDetailList.Count() - 1].factory_order_id != processDetailList[i].factory_order_id))
                {
                    SalayDetail salayDetail = new SalayDetail();
                    salayDetail.factory_order_id = processDetailList[i].factory_order_id;
                    salayDetail.order_id = processDetailList[i].order_id;
                    salayDetail.produce_id = processDetailList[i].produce_id;
                    salayDetail.model_name = processDetailList[i].model_name;
                    salayDetail.step_id = processDetailList[i].step_id;
                    YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdPrivete(processDetailList[i].factory_order_id);
                    //if (yNFactoryOrder != null)
                    //{
                    //    salayDetail.format = yNFactoryOrder.format;
                    //    salayDetail.remarks = yNFactoryOrder.remarks;
                    //    salayDetail.special_remarks = yNFactoryOrder.special_remarks;
                    //}
                    
                    salayDetail.processList = new List<FactoryProcessDetail>();
                    salayDetail.processList.Add(processDetailList[i]);
                    salayDetailList.Add(salayDetail);
                }
                else
                {
                    salayDetailList[salayDetailList.Count() - 1].processList.Add(processDetailList[i]);
                }
            }
            decimal total = processDetailList.Sum(s => s.price*s.number);
            return Json(new { code = 1, message = "成功", data = salayDetailList, total = total, name = shopUser.nick_name}, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SalayStatisticstList(DateTime? scanStartDate, DateTime? scanEndDate, int? departmentId, int? userId)
        //{
        //    if (scanStartDate == null)
        //    {
        //        return JsonFailed("统计开始时间不能为空");
        //    }
        //    if (scanEndDate == null)
        //    {
        //        return JsonFailed("统计结束时间不能为空");
        //    }
        //    ViewBag.ScanDateFrom = scanStartDate.Value;
        //    ViewBag.ScanDateTo = scanEndDate.Value;

        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    var factoryDepartmentController = new FactoryDepartmentController();
        //    List<YNFactoryDepartMent> departments;
        //    if (departmentId.HasValue)
        //    {
        //        var tempDepartmentIds = new List<int> { departmentId.Value };
        //        departments = factoryDepartmentController.getDepartmenByIds(tempDepartmentIds, clothFactoryId);
        //    }
        //    else
        //    {
        //        departments = factoryDepartmentController.getDepartmenListByFactoryId(clothFactoryId);
        //    }
        //    var departmentIds = departments.Select(m => m.id).ToList();
        //    var shopUserController = new API.Controllers.ShopUserController();
        //    var users = shopUserController.getUsersByDepartmentIds(departmentIds, clothFactoryId);
        //    if (userId.HasValue)
        //    {
        //        users = users.Where(m => m.id == userId.Value).ToList();
        //    }
        //    var userIds = users.Select(m => m.id).ToList();
        //    var bigOrderWorkStepBarCodeScanRecordController = new BigOrderWorkStepBarCodeScanRecordController();
        //    List<SalayStatisticsListModel> datas;


        //    var clothFactoryInfoController = new ClothFactoryInfoController();
        //    var factory = clothFactoryInfoController.GetClothFactoryInfoByID(clothFactoryId);
        //    var calcByInbound = factory.calc_salary_style == ClothFactoryInfoController.calc_salary_style1;
        //    if (calcByInbound)
        //    {
        //        var factoryBigPackageController = new FactoryBigPackageController();
        //        var packageIds = factoryBigPackageController.GetPackagesForSalary(clothFactoryId, scanStartDate.Value, scanEndDate.Value);
        //        datas = bigOrderWorkStepBarCodeScanRecordController.GetWorkStepCodeScanLogsForSalary(clothFactoryId, packageIds, scanStartDate.Value, scanEndDate.Value, userIds);
        //    }
        //    else
        //    {
        //        datas = bigOrderWorkStepBarCodeScanRecordController.GetWorkStepCodeScanLogsForSalary(clothFactoryId, scanStartDate.Value, scanEndDate.Value, userIds);
        //    }

        //    foreach (var data in datas)
        //    {
        //        var user = users.FirstOrDefault(m => m.id == data.UserId);

        //        if (user != null)
        //        {
        //            data.UserName = user.nick_name;
        //            data.EmployeeNo = user.employee_no;

        //            var department = departments.FirstOrDefault(m => m.id == user.factory_department_id);
        //            data.DepartmentName = department == null ? string.Empty : department.department_name;
        //        }
        //    }

        //    return View(datas);
        //}

        /// <summary>
        /// 员工工资详情
        /// </summary> 
        //public ActionResult EmployeeSalayStatisticsDetail(DateTime? scanStartDate, DateTime? scanEndDate, int userId)
        //{
        //    if (scanStartDate == null)
        //    {
        //        return JsonFailed("统计开始时间不能为空");
        //    }
        //    if (scanEndDate == null)
        //    {
        //        return JsonFailed("统计结束时间不能为空");
        //    }

        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    var shopUserController = new API.Controllers.ShopUserController();
        //    var shopUser = shopUserController.GetFactoryUserById(userId, clothFactoryId);
        //    if (shopUser == null)
        //    {
        //        return JsonFailed("员工不存在");
        //    }
        //    var bigOrderWorkStepBarCodeScanRecordController = new BigOrderWorkStepBarCodeScanRecordController();
        //    List<PackageSalayStatisticsModel> workstepLogs;


        //    var extraWorkstepLogs = bigOrderWorkStepBarCodeScanRecordController.GetUserExtraWorkStepCodeScanLogSalarys(clothFactoryId, scanStartDate.Value, scanEndDate.Value, shopUser.id);
        //    var factoryBigPackageController = new FactoryBigPackageController();
        //    var clothFactoryInfoController = new ClothFactoryInfoController();
        //    var factory = clothFactoryInfoController.GetClothFactoryInfoByID(clothFactoryId);
        //    var calcByInbound = factory.calc_salary_style == ClothFactoryInfoController.calc_salary_style1;
        //    if (calcByInbound)
        //    { 
        //         var filterPackageIds = factoryBigPackageController.GetPackagesForSalary(clothFactoryId, scanStartDate.Value, scanEndDate.Value);
        //         workstepLogs = bigOrderWorkStepBarCodeScanRecordController.GetPackageWorkStepCodeScanLogSalarys(clothFactoryId, filterPackageIds, scanStartDate.Value, scanEndDate.Value, shopUser.id);
        //    }
        //    else
        //    {
        //        workstepLogs = bigOrderWorkStepBarCodeScanRecordController.GetPackageWorkStepCodeScanLogSalarys(clothFactoryId, scanStartDate.Value, scanEndDate.Value, shopUser.id);
        //    }
            
        //    var extraWorkstepItemIds = extraWorkstepLogs.Select(m => m.PoExtraWorkStepItemId).ToList();
        //    var factoryPoExtraWorkStepController = new FactoryPoExtraWorkStepController();
        //    var extraWorkstepItems = factoryPoExtraWorkStepController.GetExtraWorkStepsIgnoreStatusByIds(extraWorkstepItemIds);

        //    var orderItemIds = extraWorkstepItems.Select(m => m.orderitem_id).Distinct().ToList();
        //    var factoryBigOrderItemController = new FactoryBigOrderItemController();
        //    var extraOrderItems = factoryBigOrderItemController.GetOrderItemsByIds(orderItemIds);
            
        //    var packageIds = workstepLogs.Select(m => m.PackageId).Distinct().ToList();
        //    var packages = factoryBigPackageController.GetPackagesByIds(packageIds, clothFactoryId);

        //    var factoryBigOrderStylePoController = new FactoryBigOrderStylePoController();
        //    var orderStylePoIds = packages.Select(m => m.orderstylepo_id).Concat(extraWorkstepItems.Select(m=>m.po_id)).Distinct().ToList();
        //    var orderStylePos = factoryBigOrderStylePoController.GetOrderStylePosByIds(orderStylePoIds);
        //    orderStylePos = orderStylePos.OrderBy(m => m.order_id).ThenBy(m => m.orderstyle_id).ThenBy(m => m.po_no).ToList();

        //    var factoryBigOrderStyleController = new FactoryBigOrderStyleController();
        //    var orderStyleIds = orderStylePos.Select(m => m.orderstyle_id).Distinct().ToList();
        //    var orderStyles = factoryBigOrderStyleController.GetOrderStyleByIds(orderStyleIds);

        //    var factoryBigOrderController = new FactoryBigOrderController();
        //    var orderIds = orderStyles.Select(m => m.order_id).Distinct().ToList();
        //    var orders = factoryBigOrderController.GetBigOrdersByOrderIds(orderIds, clothFactoryId);

        //    var factoryStyleWorkStepController = new FactoryStyleWorkStepController();

        //    var poStatisticDatas = new List<EmployeeSalayStatisticsDetailModel>();
        //    foreach (var orderStylePo in orderStylePos)
        //    {
        //        var data = new EmployeeSalayStatisticsDetailModel();

        //        var order = orders.FirstOrDefault(m => m.BigOrderId == orderStylePo.order_id);
        //        var orderStyle = orderStyles.FirstOrDefault(m => m.id == orderStylePo.orderstyle_id);

        //        var worksteps = new List<StyleWorkStepJsonModel>();
        //        if (orderStyle != null)
        //        {
        //            worksteps = orderStyle.work_steps.FromJson<List<StyleWorkStepJsonModel>>() ?? new List<StyleWorkStepJsonModel>();
        //            data.OrderStyleNo = orderStyle.style_no;
        //            data.OrderStyleName = orderStyle.style_name;
        //        }
        //        data.OrderNumber = order == null ? string.Empty : order.OrderNumber;
        //        data.PoNo = orderStylePo.po_no;

        //        var poPackages = packages.Where(m => m.orderstylepo_id == orderStylePo.id).ToList();
        //        var poPackageIds = poPackages.Select(m => m.id).ToList();

        //        var poPackageLogs = workstepLogs.Where(m => poPackageIds.Contains(m.PackageId)).ToList();
        //        foreach (var poPackageLog in poPackageLogs)
        //        {
        //            string workstepName = String.Empty;
        //            int workstepId=0;
        //            var workstep = worksteps.FirstOrDefault(m => m.Id == poPackageLog.WorkstepId);
        //            if (workstep != null)
        //            {
        //                workstepName = workstep.Name;
        //                workstepId = workstep.Id;
        //            }
        //            else
        //            {
        //                var workstepTemplate = factoryStyleWorkStepController.GetWorkStepById(poPackageLog.WorkstepId);
        //                if (workstepTemplate != null)
        //                {
        //                    workstepName = workstepTemplate.name;
        //                    workstepId = workstepTemplate.id;
        //                }
        //            }
        //            var package = poPackages.FirstOrDefault(m => m.id == poPackageLog.PackageId);

        //            var statisticPackageInfo = new EmployeeSalayStatisticsPackageInfoModel
        //            {
        //                WorkstepId = poPackageLog.WorkstepId,
        //                Quantity = poPackageLog.Quantity,
        //                SalaryAmount = poPackageLog.SalaryAmount,
        //                WorkstepName = workstepName,
        //                PackageId = poPackageLog.PackageId,
        //                WorkstepUnitPrice = poPackageLog.WorkstepUnitPrice,

        //                Color = string.Empty,
        //                Size = string.Empty,
        //                BedId = 0,
        //                PackageNumber = 0
        //            };
        //            if (package != null)
        //            {
        //                statisticPackageInfo.Color = package.color;
        //                statisticPackageInfo.Size = package.size;
        //                statisticPackageInfo.BedId = package.bed_id;
        //                statisticPackageInfo.PackageNumber = package.package_number;

        //                var packageProduceInfos = package.produce_info.FromJson<List<FactoryProcedureStepInfo>>();
        //                var produceStep = packageProduceInfos.FirstOrDefault(m => m.WorkSteps.Any(x => x.Id == workstepId));
        //                if (produceStep != null)
        //                {
        //                    var stepIndex = packageProduceInfos.IndexOf(produceStep);
        //                    if (produceStep.stepName == "成品质检")
        //                    {
        //                        statisticPackageInfo.WorkstepBarCode = string.Format("{0}", package.id);
        //                    }
        //                    else if (produceStep.stepName == "成品入库")
        //                    {
        //                        statisticPackageInfo.WorkstepBarCode = string.Format("4_{0}", package.id); //成品入库
        //                    }
        //                    else
        //                    {
        //                        statisticPackageInfo.WorkstepBarCode = string.Format("1_{0}_{1}_0_{2}", stepIndex, workstepId, package.id);
        //                    }
        //                }
        //            }
        //            data.Items.Add(statisticPackageInfo);
        //        }

        //        var poExtraWorkstepItems = extraWorkstepItems.Where(m => m.po_id == orderStylePo.id).ToList();
        //        foreach (var poExtraWorkstepItem in poExtraWorkstepItems)
        //        {
        //            string workstepName;
        //            var workstep = worksteps.FirstOrDefault(m => m.Id == poExtraWorkstepItem.factory_style_workstep_id);
        //            if (workstep != null)
        //            {
        //                workstepName = workstep.Name;
        //            }
        //            else
        //            {
        //                var workstepTemplate = factoryStyleWorkStepController.GetWorkStepById(poExtraWorkstepItem.factory_style_workstep_id);
        //                workstepName = workstepTemplate == null ? string.Empty : workstepTemplate.name;
        //            }
                   
        //            var extraWorkstepLog  = extraWorkstepLogs.FirstOrDefault(m=>m.PoExtraWorkStepItemId == poExtraWorkstepItem.id);
        //            if (extraWorkstepLog == null)
        //            {
        //                continue;
        //            }
        //            var statisticPackageInfo = new EmployeeSalayStatisticsPackageInfoModel
        //            {
        //                WorkstepId = poExtraWorkstepItem.factory_style_workstep_id,
        //                Quantity = extraWorkstepLog.Quantity,
        //                SalaryAmount = extraWorkstepLog.SalaryAmount,
        //                WorkstepName = workstepName,
        //                PackageId = 0,
        //                WorkstepUnitPrice = extraWorkstepLog.WorkstepUnitPrice,
                        
        //                Color = string.Empty,
        //                Size = string.Empty,
        //                BedId = 0,
        //                PackageNumber = 0,
        //                WorkstepBarCode = string.Format("2_{0}", poExtraWorkstepItem.id)
        //            };
        //            var orderItem = extraOrderItems.FirstOrDefault(m => m.id == poExtraWorkstepItem.orderitem_id);
        //            if (orderItem != null)
        //            {
        //                statisticPackageInfo.Color = orderItem.color;
        //                statisticPackageInfo.Size = orderItem.size;
        //            }
        //            data.Items.Add(statisticPackageInfo);
        //        }

        //        data.Items = data.Items.OrderBy(m => m.BedId).ThenBy(m => m.PackageNumber).ToList();

        //        poStatisticDatas.Add(data);
        //    }
        //    var result = poStatisticDatas.OrderBy(m=>m.OrderNumber).ThenBy(m=>m.OrderStyleNo).ThenBy(m=>m.PoNo).ToList();
        //    return JsonSuccess(result);
        //}
   
        /// <summary>
        /// 打印
        /// </summary>
        /// <returns></returns>
        public ActionResult SalayPrint()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserByIdIngoreStatus(userId, clothFactoryId);
            ViewBag.yNShopUser = yNShopUser;
            return View();
        }


    }
}