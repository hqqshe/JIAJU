using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using YNDIY.API.Controllers;

namespace YNDIY.Admin.Controllers
{
    public class ERPprocessController : ParentController
    {
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        //public JsonResult updateProcedure()
        //{
        //    API.Controllers.FactoryProcedureController factoryProcedureController = new FactoryProcedureController();
        //    List<YNFactoryProcedure> procedureList = factoryProcedureController.getProcedureList(58,null, 0,1,200);
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    for (int i = 0; i < procedureList.Count; i++)
        //    {
        //        if (!string.IsNullOrEmpty(procedureList[i].step))
        //        {
        //            List<FactoryProcedureStepNumber> tempList = js.Deserialize<List<FactoryProcedureStepNumber>>(procedureList[i].step);
        //            FactoryProcedureStepNumber temp = new FactoryProcedureStepNumber();
        //            temp.id = Convert.ToString(tempList.Count + 1);
        //            temp.stepName = "成品质检";
        //            temp.price = 0;
        //            temp.time = 0;
        //            tempList.Add(temp);
        //            procedureList[i].step = js.Serialize(tempList);
        //            factoryProcedureController.SaveChanges();
        //        }
        //    }
        //    return getLoginJsonMessage(0, "成功");
        //}
        /// <summary>
        /// 手机端面查看订单工序完成量统计
        /// </summary>
        /// <returns></returns>
        public ActionResult MobileStatistics()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            ViewBag.type = Request.QueryString["type"];
            return View();
        }
        /// <summary>
        /// 获取订单工序完成量统计
        /// </summary>
        /// <returns></returns>
        //public ActionResult GetMobileStatisticsItems()
        //{
        //    if (!checkSession())
        //    {
        //        return loginRerirect();
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    int userId = Convert.ToInt32(Session["UserId"]);
        //    string startTime = Request.QueryString["startTime"];
        //    string endTime = Request.QueryString["endTime"];
        //    if (string.IsNullOrEmpty(startTime))
        //    {
        //        return loginRerirect();
        //    }
        //    if (string.IsNullOrEmpty(endTime))
        //    {
        //        return loginRerirect();
        //    }
        //    int type = Convert.ToInt32(Request.QueryString["type"]);
        //    API.Controllers.FactoryProcessDetailController factoryProcessDetailController = new FactoryProcessDetailController();
        //    List<FactoryProcessDetail> processDetailList = factoryProcessDetailController.getFactoryProcessDetailListSalaryDetail(clothFactoryId, -1, userId, type, startTime, endTime);
        //    API.Controllers.FactoryOrderController factoryOrderController = new FactoryOrderController();
        //    List<SalayDetail> salayDetailList = new List<SalayDetail>();
        //    for (int i = 0; i < processDetailList.Count; i++)
        //    {
        //        if (salayDetailList.Count() == 0 || (salayDetailList[salayDetailList.Count() - 1].factory_order_id != processDetailList[i].factory_order_id))
        //        {
        //            SalayDetail salayDetail = new SalayDetail();
        //            salayDetail.factory_order_id = processDetailList[i].factory_order_id;
        //            salayDetail.order_id = processDetailList[i].order_id;
        //            salayDetail.produce_id = processDetailList[i].produce_id;
        //            salayDetail.model_name = processDetailList[i].model_name;
        //            salayDetail.step_id = processDetailList[i].step_id;
                    
        //            YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdPrivete(processDetailList[i].factory_order_id);
        //            if (yNFactoryOrder != null)
        //            {
        //                salayDetail.format = yNFactoryOrder.format;
        //                salayDetail.remarks = yNFactoryOrder.remarks;
        //                salayDetail.special_remarks = yNFactoryOrder.special_remarks;
        //                salayDetail.yi_xing = yNFactoryOrder.yi_xing;
        //            }

        //            salayDetail.processList = new List<FactoryProcessDetail>();
        //            salayDetail.processList.Add(processDetailList[i]);
        //            salayDetailList.Add(salayDetail);
        //        }
        //        else
        //        {
        //            salayDetailList[salayDetailList.Count() - 1].processList.Add(processDetailList[i]);
        //        }
        //    }
        //    int orderCount = processDetailList.GroupBy(s => s.factory_order_id).Select(s => s.Key).Count();
        //    int processCount = processDetailList.Sum(s => s.number);
        //    ViewBag.orderCount = orderCount;
        //    ViewBag.processCount = processCount;
        //    ViewBag.salayDetailList = salayDetailList;
        //    return View();
        //}

        /// <summary>
        /// 扫码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ScanCode()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserById(userId, clothFactoryId);
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new FactoryDepartmentController();
            YNFactoryDepartMent yNFactoryDepartMent = factoryDepartmentController.GetDepartmentIgnoreStatusById(yNShopUser.factory_department_id, clothFactoryId);
            if (yNFactoryDepartMent == null)
            {
                return getLoginJsonMessage(0, "员工部门不存在");
            }
            ViewBag.yNShopUser = yNShopUser;
            ViewBag.yNFactoryDepartMent = yNFactoryDepartMent;
            WeiXinJsSdkController weiXinJsSdkController = new WeiXinJsSdkController();
            Dictionary<string, string> dictionary = weiXinJsSdkController.sign(Request.Url.AbsoluteUri);
            ViewBag.dictionary = dictionary;
            return View();
        }

        /// <summary>
        /// 组长扫码
        /// </summary>
        /// <returns></returns>
        public ActionResult GroupScanCode()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserById(userId, clothFactoryId);
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new FactoryDepartmentController();
            YNFactoryDepartMent yNFactoryDepartMent = factoryDepartmentController.GetDepartmentIgnoreStatusById(yNShopUser.factory_department_id, clothFactoryId);
            if (yNFactoryDepartMent == null)
            {
                return getLoginJsonMessage(0, "员工部门不存在");
            }
            ViewBag.yNShopUser = yNShopUser;
            ViewBag.yNFactoryDepartMent = yNFactoryDepartMent;
            WeiXinJsSdkController weiXinJsSdkController = new WeiXinJsSdkController();
            Dictionary<string, string> dictionary = weiXinJsSdkController.sign(Request.Url.AbsoluteUri);
            ViewBag.dictionary = dictionary;
            return View();
        }

        /// 根据条码获取信息
        public JsonResult getCodeInfo()
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
            //板式家具条形码 比如:0_123  状态：0表示【订单号】，123表示【工序号】
            var isMatsh = Regex.IsMatch(barCode, @"^\d+_\d+$");
            if (!isMatsh)
            {
                return getLoginJsonMessage(0, "条码编号格式错误");
            }

            var splitArrays = barCode.Split('_');
            var orderId = Convert.ToInt32(splitArrays[0]);
            var processId = Convert.ToInt32(splitArrays[1]);
            ProductionOrderController order_ctrl = new ProductionOrderController();
            YNBanShiProductionOrder order = order_ctrl.GetProductionOrderById(clothFactoryId, orderId);
            if (order == null)
            {
                return getLoginJsonMessage(0, "条码编号错误");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<OrderProductProcess> process_list = new List<OrderProductProcess>();
            if (!String.IsNullOrEmpty(order.process_info)) {
                process_list = js.Deserialize<List<OrderProductProcess>>(order.process_info);
            }
            List<OrderProductProcess> try_list = new List<OrderProductProcess>();
            if (!String.IsNullOrEmpty(order.process_info))
            {
                try_list = js.Deserialize<List<OrderProductProcess>>(order.try_process_info);
            }
            for (var i = 0; i < try_list.Count; i++) {
                process_list.Add(try_list[i]);
            }
            OrderProductProcess process = new OrderProductProcess();
            for (var i = 0; i < process_list.Count; i++) {
                if (process_list[i].id == processId) {
                    process = process_list[i];
                    break;
                }
            }

            return getDataJsonMessage(1, "成功", new { batchNum = order.batch_num, customer = order.customer_name, brandName = order.brand_name, productName = order.product_name, modeName = order.product_model, format = order.product_format, color = order.product_color, number = order.product_number, process = process });
        }

        /// <summary>
        /// 根据工号获取员工信息
        /// </summary>
        /// <returns></returns>
        public JsonResult getEmployeeInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string gonghao = Request.QueryString["gonghao"];
            if (string.IsNullOrEmpty(gonghao))
            {
                return getLoginJsonMessage(0, "请输入正确的工号");
            }
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserByEmployeeNo(gonghao, clothFactoryId);
            if (yNShopUser == null)
            {
                return getLoginJsonMessage(0, "当前工号不存在，请输入正确的工号");
            }
            API.Controllers.FactoryDepartmentController factoryDepartmentController = new FactoryDepartmentController();
            YNFactoryDepartMent yNFactoryDepartMent = factoryDepartmentController.GetDepartmentIgnoreStatusById(yNShopUser.factory_department_id, clothFactoryId);
            if (yNFactoryDepartMent == null)
            {
                return getLoginJsonMessage(0, "员工部门不存在");
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("id", yNShopUser.id);
            data.Add("gonghao", yNShopUser.employee_no);
            data.Add("name", yNShopUser.nick_name);
            data.Add("departMentName", yNFactoryDepartMent.department_name);
            return Json(new { code = 1, message = "成功", data = data }, JsonRequestBehavior.AllowGet);
        }

        //创建统计数据
        public YNProcessStastics GetProcessStasticsModel(YNBanShiProductionOrder order,OrderProductProcess process,int complate_num, YNShopUser user,int factory_id) {
            YNProcessStastics process_stastics = new YNProcessStastics();
            process_stastics.batch_num = order.batch_num;
            process_stastics.order_id = order.id;
            process_stastics.product_model = order.product_model;
            process_stastics.product_format = order.product_format;
            process_stastics.product_name = order.product_name;
            process_stastics.product_color = order.product_color;
            process_stastics.product_remark = order.plan_remarks;
            process_stastics.process_id = process.id;
            process_stastics.process_name = process.name;
            process_stastics.complate_num = complate_num;
            process_stastics.process_price = process.price;
            process_stastics.department_id = user.factory_department_id;
            API.Controllers.FactoryDepartmentController partment_ctrl = new FactoryDepartmentController();
            YNFactoryDepartMent department = partment_ctrl.GetDepartmenById(user.factory_department_id, factory_id);
            if (department == null) {
                return null;
            }
            process_stastics.department_name = department.department_name;
            process_stastics.user_num = user.employee_no;
            process_stastics.user_name = user.nick_name;
            process_stastics.start_date = Convert.ToDateTime(order.plan_start_date);
            process_stastics.scan_date = DateTime.Now;
            process_stastics.delete_status = ProcessStasticsController.delete_0;
            process_stastics.create_date = DateTime.Now;
            process_stastics.modify_date = DateTime.Now;
            return process_stastics;
        }

        /// 在线条形码录入接口
        public JsonResult submitBarCode()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string gonghao = Request.QueryString["gonghao"];
            int count = Convert.ToInt32(Request.QueryString["count"]);
            if (count <= 0)
            {
                return getLoginJsonMessage(0, "完成数量不正确");
            }
            string barCode = Request.QueryString["barCode"];
            if (string.IsNullOrEmpty(barCode))
            {
                return getLoginJsonMessage(0, "条码不能为空");
            }
            //板式家具条形码 比如:0_123  状态：0表示【订单号】，123表示【工序号】
            var isMatsh = Regex.IsMatch(barCode, @"^\d+_\d+$");
            if (!isMatsh)
            {
                return getLoginJsonMessage(0, "条码编号格式错误");
            }

            var splitArrays = barCode.Split('_');
            var orderId = Convert.ToInt32(splitArrays[0]);
            var processId = Convert.ToInt32(splitArrays[1]);
            ProductionOrderController order_ctrl = new ProductionOrderController();
            YNBanShiProductionOrder order = order_ctrl.GetProductionOrderById(clothFactoryId, orderId);//后续修改 查询条件 状态为：未完成，未停止
            if (order == null)
            {
                return getLoginJsonMessage(0, "条码编号错误");
            }
            API.Controllers.ShopUserController user_ctrl = new API.Controllers.ShopUserController();
            YNShopUser user = user_ctrl.GetFactoryUserByEmployeeNo(gonghao, clothFactoryId);
            if (user == null)
            {
                return getLoginJsonMessage(0, "员工信息错误");
            }
            API.Controllers.ProcessStasticsController process_ctrl = new ProcessStasticsController();
            JavaScriptSerializer js = new JavaScriptSerializer();


            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {

                List<OrderProductProcess> process_list = new List<OrderProductProcess>();
                if (!String.IsNullOrEmpty(order.process_info))
                {
                    process_list = js.Deserialize<List<OrderProductProcess>>(order.process_info);
                }
                //修改工序完成数量
                for (var i = 0; i < process_list.Count; i++)
                {
                    if (process_list[i].id == processId)
                    {
                        if (count > (order.product_number - process_list[i].complate))
                        {
                            return getLoginJsonMessage(0, "完成数量不正确");
                        }
                        if (order.state == 3)
                        {
                            return getLoginJsonMessage(0, "该订单已经生产完成，不能执行此操作");
                        }
                        process_list[i].complate += count;
                        order.process_info = js.Serialize(process_list);
                        order.state = 2;//状态设置为开始生产中
                        order.modify_date = DateTime.Now;
                        //工资计算
                        YNProcessStastics process_stastics = GetProcessStasticsModel(order, process_list[i], count, user, clothFactoryId);
                        process_ctrl.Create(process_stastics);
                        break;
                    }
                }
                bool _is_complate = true;
                //修改订单状态
                for (var i = 0; i < process_list.Count; i++)
                {
                    if (process_list[i].complate != order.product_number)
                    {
                        _is_complate = false;
                        break;
                    }
                }
                if (_is_complate)
                {
                    order.state = 3;//状态设置为完成
                    order.modify_date = DateTime.Now;
                }
                List<OrderProductProcess> try_list = new List<OrderProductProcess>();
                if (!String.IsNullOrEmpty(order.process_info))
                {
                    try_list = js.Deserialize<List<OrderProductProcess>>(order.try_process_info);
                }
                for (var i = 0; i < try_list.Count; i++)
                {
                    if (try_list[i].id == processId)
                    {
                        if (count > (order.product_number - try_list[i].complate))
                        {
                            return getLoginJsonMessage(0, "完成数量不正确");
                        }
                        try_list[i].complate += count;
                        order.try_process_info = js.Serialize(try_list);
                        //工资计算
                        YNProcessStastics process_stastics = GetProcessStasticsModel(order, process_list[i], count, user, clothFactoryId);
                        process_ctrl.Create(process_stastics);
                        break;
                    }
                }
                order_ctrl.SaveChanges();
                process_ctrl.SaveChanges();

                transaction.Complete();
            }
            return getLoginJsonMessage(1, "成功");
        }
        

        /// <summary>
        /// 登录页
        /// </summary>
        /// <returns></returns>
        //public ActionResult Login()
        //{
        //    return View();
        //}
        /// <summary>
        /// 环节选择
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectLink()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int userId = Convert.ToInt32(Session["UserId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            YNShopUser yNShopUser = shopUserController.GetFactoryUserById(userId, clothFactoryId);
            List<FactoryProcess> processList = new List<FactoryProcess>();
            List<FactoryUserProcedureStep> stepList = new List<FactoryUserProcedureStep>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(yNShopUser.factory_role))
            {
                stepList = js.Deserialize<List<FactoryUserProcedureStep>>(yNShopUser.factory_role);
            }
            for (int i = 0; i < stepList.Count; i++)
            {
                YNFactoryProcedure temp = factoryProcedureController.GetProcedureById(stepList[i].factoryProcedureId, clothFactoryId);
                if (temp == null)
                {
                    continue;
                }
                FactoryProcess process = new FactoryProcess();
                process.factoryProcedureId = temp.id;
                process.factoryProcedureName = temp.name;
                process.stepName = stepList[i].stepName;
                processList.Add(process);
            }
            ViewBag.processList = processList;
            return View();
        }
        /// <summary>
        /// 扫码
        /// </summary>
        /// <returns></returns>
        public ActionResult Sweep()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int factoryProcedureId = Convert.ToInt32(Request.QueryString["factoryProcedureId"]);
            string factoryProcedureName = Request.QueryString["factoryProcedureName"];
            string stepName = Request.QueryString["stepName"];
            ViewBag.factoryProcedureId = factoryProcedureId;
            ViewBag.stepName = stepName;
            ViewBag.factoryProcedureName = factoryProcedureName;
            return View();
        }

        
        /// <summary>
        /// 设置进度数据
        /// </summary>
        /// <param name="clothList"></param>
        /// <param name="packageId"></param>
        /// <param name="clothId"></param>
        /// <param name="type">0为包号，1为流水号</param>
        /// <param name="flag">0为完成，1为异常</param>
        /// <param name="stepName"></param>
        //private void setClothProcessData(List<FactoryClothInfo> clothList, int packageId, int clothId, int type, int flag, string stepName, YNFactoryOrder yNFactoryOrder)
        //{
        //    for (int i = 0; i < clothList.Count; i++)
        //    {
        //        if (clothList[i].packageId == packageId)
        //        {
        //            //包号
        //            if (type == 0)
        //            {
        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (clothList[i].stepList[k].stepName.Equals(stepName))
        //                    {
        //                        //完成
        //                        if (flag == 0)
        //                        {
        //                            clothList[i].stepList[k].noStart = 0;
        //                            clothList[i].stepList[k].error = 0;
        //                            clothList[i].stepList[k].complete = clothList[i].number;
        //                            clothList[i].stepList[k].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                {
        //                                    if (clothList[i].clothDetail[m].stepList[n].stepName.Equals(stepName))
        //                                    {
        //                                        clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].error = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].complete = 1;
        //                                        clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        //异常
        //                        else
        //                        {
        //                            SendMessage("生产异常：订单号[" + yNFactoryOrder.order_id + "]包号[" + packageId + "]", yNFactoryOrder.factory_response_phone);
        //                            clothList[i].stepList[k].noStart = 0;
        //                            clothList[i].stepList[k].error = clothList[i].number;
        //                            clothList[i].stepList[k].complete = 0;
        //                            clothList[i].stepList[k].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                {
        //                                    if (clothList[i].clothDetail[m].stepList[n].stepName.Equals(stepName))
        //                                    {
        //                                        clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].error = 1;
        //                                        clothList[i].clothDetail[m].stepList[n].complete = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //            //流水号
        //            else
        //            {
        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (clothList[i].stepList[k].stepName.Equals(stepName))
        //                    {
        //                        //完成
        //                        if (flag == 0)
        //                        {
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                if (clothId == clothList[i].clothDetail[m].clothId)
        //                                {
        //                                    for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                    {
        //                                        if (clothList[i].clothDetail[m].stepList[n].stepName.Equals(stepName))
        //                                        {
        //                                            //不处理
        //                                            if (clothList[i].clothDetail[m].stepList[n].complete == 1)
        //                                            {

        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].noStart == 1)
        //                                            {
        //                                                clothList[i].stepList[k].noStart--;
        //                                                clothList[i].stepList[k].complete++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].error == 1)
        //                                            {
        //                                                clothList[i].stepList[k].error--;
        //                                                clothList[i].stepList[k].complete++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            break;
        //                                        }
        //                                    }
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        //异常
        //                        else
        //                        {
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                if (clothId == clothList[i].clothDetail[m].clothId)
        //                                {
        //                                    for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                    {
        //                                        if (clothList[i].clothDetail[m].stepList[n].stepName.Equals(stepName))
        //                                        {
        //                                            //不处理
        //                                            if (clothList[i].clothDetail[m].stepList[n].error == 1)
        //                                            {

        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].noStart == 1)
        //                                            {
        //                                                SendMessage("生产异常：订单号[" + yNFactoryOrder.order_id + "]包号[" + packageId + "]流水号[" + clothId + "]", yNFactoryOrder.factory_response_phone);
        //                                                clothList[i].stepList[k].noStart--;
        //                                                clothList[i].stepList[k].error++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].complete == 1)
        //                                            {
        //                                                SendMessage("生产异常：订单号[" + yNFactoryOrder.order_id + "]包号[" + packageId + "]流水号[" + clothId + "]", yNFactoryOrder.factory_response_phone);
        //                                                clothList[i].stepList[k].complete--;
        //                                                clothList[i].stepList[k].error++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            break;
        //                                        }
        //                                    }
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //            break;
        //        }
        //    }
        //}

        /// <summary>
        /// 设置进度数据
        /// </summary>
        /// <param name="clothList"></param>
        /// <param name="packageId"></param>
        /// <param name="clothId"></param>
        /// <param name="type">0为包号，1为流水号</param>
        /// <param name="flag">0为完成，1为异常</param>
        /// <param name="stepId"></param>
        //private void setClothProcessDataByStepId(List<FactoryClothInfo> clothList, int packageId, int clothId, int type, int flag, int stepId, YNFactoryOrder yNFactoryOrder)
        //{
        //    for (int i = 0; i < clothList.Count; i++)
        //    {
        //        if (clothList[i].packageId == packageId)
        //        {
        //            //包号
        //            if (type == 0)
        //            {
        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (k == stepId)
        //                    {
        //                        //完成
        //                        if (flag == 0)
        //                        {
        //                            clothList[i].stepList[k].noStart = 0;
        //                            clothList[i].stepList[k].error = 0;
        //                            clothList[i].stepList[k].complete = clothList[i].number;
        //                            clothList[i].stepList[k].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                {
        //                                    if (n == stepId)
        //                                    {
        //                                        clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].error = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].complete = 1;
        //                                        clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        //异常
        //                        else
        //                        {
        //                            SendMessage("生产异常：订单号[" + yNFactoryOrder.order_id + "]包号[" + packageId + "]", yNFactoryOrder.factory_response_phone);
        //                            clothList[i].stepList[k].noStart = 0;
        //                            clothList[i].stepList[k].error = clothList[i].number;
        //                            clothList[i].stepList[k].complete = 0;
        //                            clothList[i].stepList[k].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                {
        //                                    if (n == stepId)
        //                                    {
        //                                        clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].error = 1;
        //                                        clothList[i].clothDetail[m].stepList[n].complete = 0;
        //                                        clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //            //流水号
        //            else
        //            {
        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (k == stepId)
        //                    {
        //                        //完成
        //                        if (flag == 0)
        //                        {
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                if (clothId == clothList[i].clothDetail[m].clothId)
        //                                {
        //                                    for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                    {
        //                                        if (n == stepId)
        //                                        {
        //                                            //不处理
        //                                            if (clothList[i].clothDetail[m].stepList[n].complete == 1)
        //                                            {

        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].noStart == 1)
        //                                            {
        //                                                clothList[i].stepList[k].noStart--;
        //                                                clothList[i].stepList[k].complete++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].error == 1)
        //                                            {
        //                                                clothList[i].stepList[k].error--;
        //                                                clothList[i].stepList[k].complete++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            break;
        //                                        }
        //                                    }
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        //异常
        //                        else
        //                        {
        //                            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //                            {
        //                                if (clothId == clothList[i].clothDetail[m].clothId)
        //                                {
        //                                    for (int n = 0; n < clothList[i].clothDetail[m].stepList.Count; n++)
        //                                    {
        //                                        if (n == stepId)
        //                                        {
        //                                            //不处理
        //                                            if (clothList[i].clothDetail[m].stepList[n].error == 1)
        //                                            {

        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].noStart == 1)
        //                                            {
        //                                                SendMessage("生产异常：订单号[" + yNFactoryOrder.order_id + "]包号[" + packageId + "]流水号[" + clothId + "]", yNFactoryOrder.factory_response_phone);
        //                                                clothList[i].stepList[k].noStart--;
        //                                                clothList[i].stepList[k].error++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            else if (clothList[i].clothDetail[m].stepList[n].complete == 1)
        //                                            {
        //                                                SendMessage("生产异常：订单号[" + yNFactoryOrder.order_id + "]包号[" + packageId + "]流水号[" + clothId + "]", yNFactoryOrder.factory_response_phone);
        //                                                clothList[i].stepList[k].complete--;
        //                                                clothList[i].stepList[k].error++;
        //                                                clothList[i].clothDetail[m].stepList[n].noStart = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].error = 1;
        //                                                clothList[i].clothDetail[m].stepList[n].complete = 0;
        //                                                clothList[i].clothDetail[m].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                                clothList[i].stepList[n].scanDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //                                            }
        //                                            break;
        //                                        }
        //                                    }
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //            break;
        //        }
        //    }
        //}

        /// <summary>
        /// 获取订单的状态
        /// </summary>
        /// <param name="clothList"></param>
        /// <param name="yNFactoryOrder"></param>
        /// <returns></returns>
        //private void setOrderState(List<FactoryClothInfo> clothList, YNFactoryOrder yNFactoryOrder)
        //{
        //    int factory_error_status = 0;
        //    for (int i = 0; i < clothList.Count; i++)
        //    {
        //        for (int k = 0; k < clothList[i].stepList.Count; k++)
        //        {
        //            if (clothList[i].stepList[k].error > 0)
        //            {
        //                factory_error_status = 1;
        //                break;
        //            }
        //        }
        //        if (factory_error_status == 1)
        //        {
        //            break;
        //        }
        //    }
        //    yNFactoryOrder.factory_error_status = factory_error_status;
        //    bool complete = true;
        //    for (int i = 0; i < clothList.Count; i++)
        //    {
        //        for (int k = 0; k < clothList[i].stepList.Count; k++)
        //        {
        //            if (clothList[i].stepList[k].complete != clothList[i].number)
        //            {
        //                complete = false;
        //                break;
        //            }
        //        }
        //        if (complete == false)
        //        {
        //            break;
        //        }
        //    }
        //    if (complete == true)
        //    {
        //        yNFactoryOrder.factory_order_status = API.Controllers.FactoryOrderController.factory_order_status2;
        //        yNFactoryOrder.complete_time = DateTime.Now;
        //        yNFactoryOrder.factory_delay_state = API.Controllers.FactoryOrderController.factory_delay_state0;
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(yNFactoryOrder.factory_delivery_day))
        //        {
        //            string currentData = DateTime.Now.ToString("yyyy-MM-dd");
        //            //总体时间过期
        //            if (yNFactoryOrder.factory_delivery_day.CompareTo(currentData) < 0)
        //            {
        //                yNFactoryOrder.factory_delay_state = API.Controllers.FactoryOrderController.factory_delay_state1;
        //            }
        //            //判断步骤日期是否过期
        //            else
        //            {
        //                bool guoqi = false;
        //                for (int i = 0; i < clothList.Count; i++)
        //                {
        //                    for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                    {
        //                        if (clothList[i].stepList[k].complete != clothList[i].number)
        //                        {
        //                            if (clothList[i].stepList[k].factoryDeliveryDay.CompareTo(currentData) < 0)
        //                            {
        //                                guoqi = true;
        //                                break;
        //                            }

        //                        }
        //                    }
        //                    if (guoqi == true)
        //                    {
        //                        break;
        //                    }
        //                }
        //                if (guoqi)
        //                {
        //                    yNFactoryOrder.factory_delay_state = API.Controllers.FactoryOrderController.factory_delay_state1;
        //                }
        //            }
        //        }
        //    }
        //}
        //设置工厂生产状态
        //public JsonResult setFactoryState()
        //{
        //    string token = Request.QueryString["token"];
        //    if (!"younidz".Equals(token))
        //    {
        //        return getLoginJsonMessage(0, "失败");
        //    }
        //    int factoryId = 0;
        //    if (!string.IsNullOrEmpty(Request.QueryString["factoryId"]))
        //    {
        //        factoryId = Convert.ToInt32(Request.QueryString["factoryId"]);
        //    }
        //    string result = "开始时间:[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]-------------";
        //    API.Controllers.FactoryOrderController factoryOrderController = new FactoryOrderController();
        //    List<YNFactoryOrder> factoryOrderList = factoryOrderController.getFactoryOrderList(factoryId);
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    for (int i = 0; i < factoryOrderList.Count; i++)
        //    {
        //        if (string.IsNullOrEmpty(factoryOrderList[i].factory_cloth_info))
        //        {
        //            continue;
        //        }
        //        try
        //        {
        //            List<FactoryClothInfo> clothList = js.Deserialize<List<FactoryClothInfo>>(factoryOrderList[i].factory_cloth_info);
        //            setOrderState(clothList, factoryOrderList[i]);
        //            factoryOrderController.SaveChanges();
        //            if (factoryOrderList[i].factory_delay_state == API.Controllers.FactoryOrderController.factory_delay_state1)
        //            {
        //                SendMessage("订单生产延期：订单号[" + factoryOrderList[i].order_id + "]", factoryOrderList[i].factory_response_phone);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            result += "订单数据异常:[" + factoryOrderList[i].order_id + "]-----------------------";
        //        }
        //    }
        //    result += "数据条数:[" + factoryOrderList.Count + "]------------结束时间:[" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
        //    return getLoginJsonMessage(1, result);
        //}
       
    }
}
