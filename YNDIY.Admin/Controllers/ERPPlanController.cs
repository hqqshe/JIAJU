using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;

namespace YNDIY.Admin.Controllers
{
    public class ERPPlanController : ParentController
    {
        /// <summary>
        /// 待生产订单
        /// </summary>
        /// <returns></returns>
        public ActionResult WaitingForProduction()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        /// <summary>
        /// 获取待生产订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWaitingList()
        {
            //if (!checkSession())
            //{
            //    return loginHtmlMessage();
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //string key = Request.QueryString["searchKey"];
            //int pageIndex = 1;
            //int pageSize = 10;
            //if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            //{
            //    pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            //}
            //if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            //{
            //    pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            //}
            //string startTime = Request.QueryString["startTime"];
            //string endTime = Request.QueryString["endTime"];
            //string createStartTime = Request.QueryString["createStartTime"];
            //string createEndTime = Request.QueryString["createEndTime"];
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = factoryOrderController.getFactoryOrderProduceList(key, clothFactoryId, API.Controllers.FactoryOrderController.start_state0,pageIndex, pageSize,startTime, endTime,createStartTime,createEndTime);
            //int count = factoryOrderController.getFactoryOrderProduceListCount(key, clothFactoryId, API.Controllers.FactoryOrderController.start_state0, startTime, endTime, createStartTime, createEndTime);
            //API.Controllers.PagesController page = new API.Controllers.PagesController();
            //page.GetPage(pageIndex, count, pageSize);
            //ViewBag.orderList = orderList;
            //ViewBag.page = page;
            return View();
        }

        /// <summary>
        /// 设置开始日期
        /// </summary>
        /// <returns></returns>
        public JsonResult setStartTime()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string starTime = Request.QueryString["starTime"];
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductionOrderController product_order_ctrl = new API.Controllers.ProductionOrderController();
            YNBanShiProductionOrder order = product_order_ctrl.GetProductionOrderById(clothFactoryId, id);
            if (order == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            order.plan_start_date = Convert.ToDateTime(starTime);
            order.state = API.Controllers.ProductionOrderController.state_1;//设置为已经安排，未生产
            product_order_ctrl.SaveChanges();

            //API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            //List<YNFactoryProcessDetail> list = factoryProcessDetailController.getFactoryProcessDetailListById(id, clothFactoryId);
            //for (int i = 0; i < list.Count; i++)
            //{
            //    list[i].srart_produce_time = starTime;
            //}
            //factoryProcessDetailController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }
        /// <summary>
        /// 生产计划
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductionPlan()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// <summary>
        /// 获取生产计划订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProductionPlanList()
        {
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string dateTime = Request.QueryString["dateTime"];
            //int pageIndex = 1;
            //int pageSize = 10;
            //if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            //{
            //    pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            //}
            //if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            //{
            //    pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            //}
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = factoryOrderController.getFactoryOrderProduceListNew(0,dateTime, clothFactoryId);
            //ViewBag.orderList = orderList;
            return View();
        }

        /// <summary>
        /// 取消生产计划
        /// </summary>
        /// <returns></returns>
        public JsonResult cancelPlan()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登陆");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int id = Convert.ToInt32(Request.QueryString["id"]);
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
            //if (yNFactoryOrder == null)
            //{
            //    return getLoginJsonMessage(0, "参数错误");
            //}
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //List<FactoryProcedureStepNumberOrder> stepList = new List<FactoryProcedureStepNumberOrder>();
            //if (!string.IsNullOrEmpty(yNFactoryOrder.model_detail))
            //{
            //    stepList = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrder.model_detail);
            //}
            //bool flag = false;
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    //已经有做完的工序，不能取消
            //    if (stepList[i].complete != 0 || stepList[i].error !=0)
            //    {
            //        flag = true;
            //        break;
            //    }
            //}
            //if (flag)
            //{
            //    return getLoginJsonMessage(0, "已经有做完的工序，不能取消");
            //}
            //yNFactoryOrder.start_state = API.Controllers.FactoryOrderController.start_state0;
            //yNFactoryOrder.srart_produce_time = "";
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    stepList[i].userList = new List<FactoryProcedureStepUser>();
            //}
            //yNFactoryOrder.model_detail = js.Serialize(stepList);
            //yNFactoryOrder.plan_state = API.Controllers.FactoryOrderController.plan_state0;
            //factoryOrderController.SaveChanges();

            //API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            //List<YNFactoryProcessDetail> list = factoryProcessDetailController.getFactoryProcessDetailListById(id, clothFactoryId);
            //for (int i = 0; i < list.Count; i++)
            //{
            //    list[i].user_id = 0;
            //    list[i].gonghao = "";
            //    list[i].user_name = "";
            //    list[i].department_id = 0;
            //    list[i].department_name = "";
            //    list[i].srart_produce_time = "";
            //}
            //factoryProcessDetailController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 删除订单工序
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteStep()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登陆");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            //string gongxuId = Request.QueryString["gongxuId"];
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(orderId, clothFactoryId);
            //if (yNFactoryOrder == null)
            //{
            //    return getLoginJsonMessage(0, "订单不存在");
            //}
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //List<FactoryProcedureStepNumberOrder> stepList = new List<FactoryProcedureStepNumberOrder>();
            //if (!string.IsNullOrEmpty(yNFactoryOrder.model_detail))
            //{
            //    stepList = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrder.model_detail);
            //}
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    if (stepList[i].id.Equals(gongxuId))
            //    {
            //        //已经有做完的工序，不能取消
            //        if (stepList[i].complete != 0 || stepList[i].error != 0)
            //        {
            //            return getLoginJsonMessage(0, "当前工序已被扫码，不能删除");
            //        }
            //        stepList.Remove(stepList[i]);
            //        break;
            //    }
            //}
            ////删除工序
            //API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            //factoryProcessDetailController.deleteList(clothFactoryId, yNFactoryOrder.id, gongxuId);

            //bool flag = true;
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    if (stepList[i].error != 0 || stepList[i].noStart != 0)
            //    {
            //        flag = false;
            //        break;
            //    }
            //}
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
            //yNFactoryOrder.model_detail = js.Serialize(stepList);
            //factoryOrderController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 设置订单工序步骤的执行人
        /// </summary>
        /// <returns></returns>
        public JsonResult setFactoryOrderStep()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登陆");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int id = Convert.ToInt32(Request.QueryString["id"]);
            //int userId = Convert.ToInt32(Request.QueryString["userId"]);
            //string gongxuId = Request.QueryString["gongxuId"];
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
            //if (yNFactoryOrder == null)
            //{
            //    return getLoginJsonMessage(0, "订单不存在");
            //}
            //API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            //YNShopUser yNShopUser = new YNShopUser();
            //if (userId != 0)
            //{
            //    yNShopUser = shopUserController.GetFactoryUserByIdIngoreStatus(userId, clothFactoryId);
            //}
            //if (yNShopUser == null)
            //{
            //    return getLoginJsonMessage(0, "用户不存在");
            //}
            //API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            //YNFactoryDepartMent yNFactoryDepartMent = factoryDepartmentController.GetDepartmentIgnoreStatusById(yNShopUser.factory_department_id,clothFactoryId);
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //List<FactoryProcedureStepNumberOrder> stepList = new List<FactoryProcedureStepNumberOrder>();
            //if (!string.IsNullOrEmpty(yNFactoryOrder.model_detail))
            //{
            //    stepList = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrder.model_detail);
            //}
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    if (stepList[i].id == gongxuId)
            //    {
            //        if (stepList[i].complete != 0 || stepList[i].error != 0)
            //        {
            //            return getLoginJsonMessage(0, "当前工序已经被执行，不能更换执行人");
            //        }
            //        stepList[i].userList = new List<FactoryProcedureStepUser>();
            //        if (yNShopUser.id != 0)
            //        {
            //            FactoryProcedureStepUser temp = new FactoryProcedureStepUser();
            //            temp.userId = yNShopUser.id;
            //            temp.userName = yNShopUser.nick_name;
            //            temp.gonghao = yNShopUser.employee_no;
            //            stepList[i].userList.Add(temp);
            //        }
                   
            //        break;
            //    }
            //}
            //int setCount = 0;
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    if (stepList[i].userList != null && stepList[i].userList.Count > 0)
            //    {
            //        setCount++;
            //    }
            //}
            //if (setCount == 0)
            //{
            //    yNFactoryOrder.plan_state = API.Controllers.FactoryOrderController.plan_state0;
            //}
            //else if (setCount == stepList.Count)
            //{
            //    yNFactoryOrder.plan_state = API.Controllers.FactoryOrderController.plan_state2;
            //}
            //else
            //{
            //    yNFactoryOrder.plan_state = API.Controllers.FactoryOrderController.plan_state1;
            //}
            //yNFactoryOrder.model_detail = js.Serialize(stepList);
            //yNFactoryOrder.modify_time = DateTime.Now;
            //factoryOrderController.SaveChanges();
            ////修改工序相关
            //API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            //List<YNFactoryProcessDetail> list = factoryProcessDetailController.getFactoryProcessDetailListById(id, clothFactoryId);
            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (list[i].step_id == gongxuId)
            //    {
            //        if (yNShopUser.id != 0)
            //        {
            //            list[i].user_id = yNShopUser.id;
            //            list[i].gonghao = yNShopUser.employee_no;
            //            list[i].user_name = yNShopUser.nick_name;
            //            list[i].department_id = yNFactoryDepartMent.id;
            //            list[i].department_name = yNFactoryDepartMent.department_name;
            //        }
            //        else
            //        {
            //            list[i].user_id = 0;
            //            list[i].gonghao = "";
            //            list[i].user_name = "";
            //            list[i].department_id = 0;
            //            list[i].department_name = "";
            //        }
                   
            //    }
            //}
            //factoryProcessDetailController.SaveChanges();
            //return getLoginJsonMessage(1, "成功", yNFactoryOrder.id.ToString());
            return null;
        }

        /// <summary>
        /// 员工生产计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult EmployerPlan()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        public ActionResult EmployerPlanList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string startTime = Request.QueryString["startTime"];
            if (string.IsNullOrEmpty(startTime))
            {
                return getLoginJsonMessage(0, "请选择计划生产日期");
            }
            int departMentId = Convert.ToInt32(Request.QueryString["departMentId"]);
            string searchKey = Request.QueryString["searchKey"];
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            List<FactoryProcessDetail> list = factoryProcessDetailController.getFactoryPlanList(clothFactoryId, startTime, departMentId, searchKey);
            ViewBag.list = list;
            ViewBag.startTime = startTime;
            return View();
        }

        /// <summary>
        /// 更新打印状态
        /// </summary>
        /// <returns></returns>
        public JsonResult updatePrintState()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            if (userId == 0)
            {
                return getLoginJsonMessage(0, "员工参数错误");
            }
            string dateTime = Request.QueryString["dateTime"];
            if (string.IsNullOrEmpty(dateTime))
            {
                return getLoginJsonMessage(0, "日期不能为空");
            }
            int state = Convert.ToInt32(Request.QueryString["state"]);
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            List<YNFactoryProcessDetail> list = factoryProcessDetailController.getFactoryPlanDetailByUserIdAndTime(clothFactoryId, userId, dateTime);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].print_state = state;
            }
            factoryProcessDetailController.SaveChanges();
            return getLoginJsonMessage(1, "修改成功");
        }
        /// <summary>
        /// 员工计划详情
        /// </summary>
        /// <returns></returns>
        public ActionResult EmployerDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            string startTime = Request.QueryString["startTime"];
            if (string.IsNullOrEmpty(startTime))
            {
                return getLoginJsonMessage(0, "请选择计划生产日期");
            }
            //API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            //List<YNFactoryProcessDetail> processList = factoryProcessDetailController.getFactoryPlanDetail(clothFactoryId, startTime, userId);
            //List<int> idList = processList.GroupBy(s => s.factory_order_id).Select(s => s.Key).ToList();
            //decimal total = processList.Sum(s => s.time);
            //decimal sub = processList.Where(s => s.state == 0).Sum(s => s.time);
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            //for (int i = 0; i < idList.Count; i++)
            //{
            //    YNFactoryOrder temp = factoryOrderController.getFactoryOrderByIdPrivete(idList[i]);
            //    orderList.Add(temp);
            //}
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserByIdIngoreStatus(userId, clothFactoryId);
            ViewBag.yNShopUser = yNShopUser;
            ViewBag.userId = userId;
            ViewBag.startTime = startTime;
            return View();
        }
        /// <summary>
        /// 员工计划详情
        /// </summary>
        /// <returns></returns>
        public ActionResult EmployerDetailList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            string startTime = Request.QueryString["startTime"];
            if (string.IsNullOrEmpty(startTime))
            {
                return getLoginJsonMessage(0, "请选择计划生产日期");
            }
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            List<YNFactoryProcessDetail> processList = factoryProcessDetailController.getFactoryPlanDetail(clothFactoryId, startTime, userId);
            List<int> idList = processList.GroupBy(s => s.factory_order_id).Select(s => s.Key).ToList();
            decimal total = processList.Sum(s => s.time);
            decimal sub = processList.Where(s => s.state == 0).Sum(s => s.time);
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            for (int i = 0; i < idList.Count; i++)
            {
                YNFactoryOrder temp = factoryOrderController.getFactoryOrderByIdPrivete(idList[i]);
                orderList.Add(temp);
            }

            ViewBag.total = total;
            ViewBag.sub = sub;
            ViewBag.orderList = orderList;
            ViewBag.processList = processList;
            return View();
        }

        /// <summary>
        /// 更新生产说明
        /// </summary>
        /// <returns></returns>
        public JsonResult updateProduceRemarks()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登陆");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int id = Convert.ToInt32(Request.QueryString["id"]);
            //string specialRemarks = Request.QueryString["specialRemarks"];
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
            //if (yNFactoryOrder == null)
            //{
            //    return getLoginJsonMessage(0, "订单不存在");
            //}
            //yNFactoryOrder.special_remarks = specialRemarks;
            //factoryOrderController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }
        /// <summary>
        /// 打印员工生产计划安排
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintEmployerPlan()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            string startTime = Request.QueryString["startTime"];
            if (string.IsNullOrEmpty(startTime))
            {
                return getLoginJsonMessage(0, "请选择计划生产日期");
            }
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            List<YNFactoryProcessDetail> processList = factoryProcessDetailController.getFactoryPlanDetail(clothFactoryId, startTime, userId);
            List<int> idList = processList.GroupBy(s => s.factory_order_id).Select(s => s.Key).ToList();
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            for (int i = 0; i < idList.Count; i++)
            {
                YNFactoryOrder temp = factoryOrderController.getFactoryOrderByIdPrivete(idList[i]);
                orderList.Add(temp);
            }
            ViewBag.orderList = orderList;
            ViewBag.processList = processList;
            return View();
        }
        /// <summary>
        /// 打印员工生产计划安排
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintEmployerPlan2()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            string startTime = Request.QueryString["startTime"];
            if (string.IsNullOrEmpty(startTime))
            {
                return getLoginJsonMessage(0, "请选择计划生产日期");
            }
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            List<YNFactoryProcessDetail> processList = factoryProcessDetailController.getFactoryPlanDetail(clothFactoryId, startTime, userId);
            List<int> idList = processList.GroupBy(s => s.factory_order_id).Select(s => s.Key).ToList();
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            for (int i = 0; i < idList.Count; i++)
            {
                YNFactoryOrder temp = factoryOrderController.getFactoryOrderByIdPrivete(idList[i]);
                orderList.Add(temp);
            }
            ViewBag.orderList = orderList;
            ViewBag.processList = processList;
            return View();
        }
        /// <summary>
        /// 打印订单生产计划安排
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintOrderPlan()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            List<YNFactoryProcessDetail> processList = factoryProcessDetailController.getFactoryPlanDetailByOrderId(clothFactoryId, orderId);
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            YNFactoryOrder temp = factoryOrderController.getFactoryOrderByIdPrivete(orderId);
            if (temp == null)
            {
                return getLoginJsonMessage(0, "订单参数错误");
            }
            orderList.Add(temp);
            ViewBag.orderList = orderList;
            ViewBag.processList = processList;
            return View();
        }
        /// <summary>
        /// 打印订单生产计划流程卡
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintOrderPro()
        {
            //if (!checkSession())
            //{
            //    return loginHtmlMessage();
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            //string startTime = Request.QueryString["startTime"];
            //if (string.IsNullOrEmpty(startTime) && orderId == 0)
            //{
            //    return getLoginJsonMessage(0, "参数错误");
            //}
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = factoryOrderController.getFactoryOrderProduceListNew(orderId, startTime, clothFactoryId);

            ////API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            ////List<YNFactoryProcessDetail> processList = factoryProcessDetailController.getFactoryPlanDetailLiuCheng(clothFactoryId, startTime, orderId);
            ////List<int> idList = processList.GroupBy(s => s.factory_order_id).Select(s => s.Key).ToList();
            ////API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            ////List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            ////for (int i = 0; i < idList.Count; i++)
            ////{
            ////    YNFactoryOrder temp = factoryOrderController.getFactoryOrderByIdPrivete(idList[i]);
            ////    orderList.Add(temp);
            ////}
            //ViewBag.orderList = orderList;
            //ViewBag.processList = processList;
            return View();
        }

        /// <summary>
        /// 复制工人工序订单
        /// </summary>
        /// <returns></returns>
        public JsonResult copyOrderPlan()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登陆");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int id = Convert.ToInt32(Request.QueryString["id"]);
            //int copyId = Convert.ToInt32(Request.QueryString["copyId"]);
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
            //if (yNFactoryOrder == null)
            //{
            //    return getLoginJsonMessage(0, "订单不存在");
            //}
            //JavaScriptSerializer js = new JavaScriptSerializer();
            //List<FactoryProcedureStepNumberOrder> stepList = new List<FactoryProcedureStepNumberOrder>();
            //if (!string.IsNullOrEmpty(yNFactoryOrder.model_detail))
            //{
            //    stepList = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrder.model_detail);
            //}
            ////判断当前订单是否已经被执行
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    if (stepList[i].complete != 0 || stepList[i].error != 0)
            //    {
            //        return getLoginJsonMessage(0, "当前订单已有工序被执行，不能更换执行人");
            //    }
            //}
            //YNFactoryOrder yNFactoryOrderCopy = factoryOrderController.getFactoryOrderByIdFactory(copyId, clothFactoryId);
            //if (yNFactoryOrderCopy == null)
            //{
            //    return getLoginJsonMessage(0, "被复制的订单不存在，请重新复制");
            //}
            //List<FactoryProcedureStepNumberOrder> stepListCopy = new List<FactoryProcedureStepNumberOrder>();
            //if (!string.IsNullOrEmpty(yNFactoryOrderCopy.model_detail))
            //{
            //    stepListCopy = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(yNFactoryOrderCopy.model_detail);
            //}
            //int count = stepListCopy.Count >= stepList.Count ? stepList.Count : stepListCopy.Count;
            //API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            //API.Controllers.FactoryDepartmentController factoryDepartmentController = new API.Controllers.FactoryDepartmentController();
            //API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new API.Controllers.FactoryProcessDetailController();
            //List<YNFactoryProcessDetail> list = factoryProcessDetailController.getFactoryProcessDetailListById(id, clothFactoryId);
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    FactoryProcedureStepNumberOrder orderTemp = null;
            //    for (int k = 0; k < stepListCopy.Count; k++)
            //    {
            //        if (stepList[i].stepName.Equals(stepListCopy[k].stepName))
            //        {
            //            orderTemp = stepListCopy[k];
            //            break;
            //        }
            //    }
            //    //如果没找到就相当于当前步骤不做处理
            //    if (orderTemp == null)
            //    {
            //        continue;
            //    }
            //    YNShopUser yNShopUser = new YNShopUser();
            //    //复制工序没有执行人
            //    if (orderTemp.userList == null || orderTemp.userList.Count == 0)
            //    {
            //    }
            //    else
            //    {
            //        FactoryProcedureStepUser stepUser = orderTemp.userList[0];
            //        yNShopUser = shopUserController.GetFactoryUserByIdIngoreStatus(stepUser.userId, clothFactoryId);
            //        if (yNShopUser == null)
            //        {
            //            yNShopUser = new YNShopUser();
            //        }
            //    }
            //    YNFactoryDepartMent yNFactoryDepartMent = factoryDepartmentController.GetDepartmentIgnoreStatusById(yNShopUser.factory_department_id, clothFactoryId);
            //    stepList[i].userList = new List<FactoryProcedureStepUser>();
            //    if (yNShopUser.id != 0)
            //    {
            //        FactoryProcedureStepUser temp = new FactoryProcedureStepUser();
            //        temp.userId = yNShopUser.id;
            //        temp.userName = yNShopUser.nick_name;
            //        temp.gonghao = yNShopUser.employee_no;
            //        stepList[i].userList.Add(temp);
            //    }
            //    //修改工序流程配置详情
            //    for (int k = 0; k < list.Count; k++)
            //    {
            //        if (list[k].step_id == stepList[i].id)
            //        {
            //            if (yNShopUser.id != 0)
            //            {
            //                list[i].user_id = yNShopUser.id;
            //                list[i].gonghao = yNShopUser.employee_no;
            //                list[i].user_name = yNShopUser.nick_name;
            //                list[i].department_id = yNFactoryDepartMent.id;
            //                list[i].department_name = yNFactoryDepartMent.department_name;
            //            }
            //            else
            //            {
            //                list[i].user_id = 0;
            //                list[i].gonghao = "";
            //                list[i].user_name = "";
            //                list[i].department_id = 0;
            //                list[i].department_name = "";
            //            }
            //        }
            //    }
            //}

            //int setCount = 0;
            //for (int i = 0; i < stepList.Count; i++)
            //{
            //    if (stepList[i].userList != null && stepList[i].userList.Count > 0)
            //    {
            //        setCount++;
            //    }
            //}
            //if (setCount == 0)
            //{
            //    yNFactoryOrder.plan_state = API.Controllers.FactoryOrderController.plan_state0;
            //}
            //else if (setCount == stepList.Count)
            //{
            //    yNFactoryOrder.plan_state = API.Controllers.FactoryOrderController.plan_state2;
            //}
            //else
            //{
            //    yNFactoryOrder.plan_state = API.Controllers.FactoryOrderController.plan_state1;
            //}
            //yNFactoryOrder.model_detail = js.Serialize(stepList);
            //yNFactoryOrder.modify_time = DateTime.Now;
            //factoryOrderController.SaveChanges();
            ////修改工序相关
            //factoryProcessDetailController.SaveChanges();
            //return Json(new { code = 1, message = "成功", data = stepList }, JsonRequestBehavior.AllowGet);
            return null;
        }
    }
}
