using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class ERPDeliverStoreController : ParentController
    {
        /// <summary>
        /// 出库列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStore()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 获取出库列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStoretList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
             int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string key = Request.QueryString["searchKey"];
            string startTime = Request.QueryString["startTime"];
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
            API.Controllers.StorageTopController storageTopController = new API.Controllers.StorageTopController();
            List<YNStorageTop> storageTopList = storageTopController.getStorageTopList(clothFactoryId, key, startTime,pageIndex,pageSize);
            int count = storageTopController.getStorageTopListCount(clothFactoryId, key, startTime);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.storageTopList = storageTopList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 出库单明细
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStorePlan()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int outNumberId = Convert.ToInt32(Request.QueryString["outNumberId"]);
            API.Controllers.StorageTopController storageTopController = new API.Controllers.StorageTopController();
            YNStorageTop yNStorageTop = storageTopController.getStorageTopById(outNumberId, clothFactoryId);
            API.Controllers.StorageTypeController storageTypeController = new API.Controllers.StorageTypeController();
            List<YNStorageDetail> detailList = storageTypeController.getStorageDetailListByOutNumber(clothFactoryId, outNumberId);
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            for (int i = 0; i < detailList.Count; i++)
            {
                YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(detailList[i].factory_order_id, clothFactoryId);
                if (yNFactoryOrder != null)
                {
                    orderList.Add(yNFactoryOrder);
                }
            }
            ViewBag.yNStorageTop = yNStorageTop;
            ViewBag.orderList = orderList;
            ViewBag.detailList = detailList;
            return View();
        }
        /// <summary>
        /// 出库单明细
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStorePlanList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int outNumberId = Convert.ToInt32(Request.QueryString["outNumberId"]);
            API.Controllers.StorageTypeController storageTypeController = new API.Controllers.StorageTypeController();
            List<YNStorageDetail> detailList = storageTypeController.getStorageDetailListByOutNumber(clothFactoryId,outNumberId);
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            for(int i=0;i<detailList.Count;i++){
                YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(detailList[i].factory_order_id,clothFactoryId);
                if(yNFactoryOrder != null){
                    orderList.Add(yNFactoryOrder);
                }
            }
            ViewBag.orderList = orderList;
            ViewBag.detailList = detailList;
            return View();
        }
       
        /// <summary>
        /// 添加出库单查询列表
        /// </summary>
        /// <returns></returns>
        public ActionResult StoreOrderList()
        {
            //if (!checkSession())
            //{
            //    return loginHtmlMessage();
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //string key = Request.QueryString["searchKey"];
            //int orderStatus = -1;
            //int kucunStatus = -1;
            //int pageIndex = 1;
            //int pageSize = 100;
            //int orderKey = 0;
            //int order = 0;
            //if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            //{
            //    pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            //}
            //if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            //{
            //    pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            //}
            //int type = API.Controllers.FactoryOrderController.type_0;
            //string startTime = Request.QueryString["startTime"];
            //string endTime = Request.QueryString["endTime"];
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = factoryOrderController.getFactoryOrderListByFactory(key, clothFactoryId, API.Controllers.FactoryOrderController.status_all, orderStatus, type, pageIndex, pageSize, null, orderKey, order, 1, startTime, endTime, kucunStatus);
            //int count = factoryOrderController.getFactoryOrderListByFactoryCount(key, clothFactoryId, API.Controllers.FactoryOrderController.status_all, orderStatus, type, null, orderKey, order, 1, startTime, endTime, kucunStatus);
            //API.Controllers.PagesController page = new API.Controllers.PagesController();
            //page.GetPage(pageIndex, count, pageSize);
            //ViewBag.orderList = orderList;
            //ViewBag.page = page;
            return View();
        }

        /// <summary>
        /// 打印出库单
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintStorageOutList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int outNumberId = Convert.ToInt32(Request.QueryString["outNumberId"]);
            API.Controllers.StorageTopController storageTopController = new API.Controllers.StorageTopController();
            YNStorageTop yNStorageTop = storageTopController.getStorageTopById(outNumberId, clothFactoryId);
            API.Controllers.StorageTypeController storageTypeController = new API.Controllers.StorageTypeController();
            List<YNStorageDetail> detailList = storageTypeController.getStorageDetailListByOutNumber(clothFactoryId, outNumberId);
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            for (int i = 0; i < detailList.Count; i++)
            {
                YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(detailList[i].factory_order_id, clothFactoryId);
                if (yNFactoryOrder != null)
                {
                    orderList.Add(yNFactoryOrder);
                }
            }
            ViewBag.yNStorageTop = yNStorageTop;
            ViewBag.orderList = orderList;
            ViewBag.detailList = detailList;
            return View();
        }
        /// <summary>
        /// 打印销货单
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintDestoryList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int outNumberId = Convert.ToInt32(Request.QueryString["outNumberId"]);
            API.Controllers.StorageTopController storageTopController = new API.Controllers.StorageTopController();
            YNStorageTop yNStorageTop = storageTopController.getStorageTopById(outNumberId, clothFactoryId);
            API.Controllers.StorageTypeController storageTypeController = new API.Controllers.StorageTypeController();
            List<YNStorageDetail> detailList = storageTypeController.getStorageDetailListByOutNumber(clothFactoryId, outNumberId);
            List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            for (int i = 0; i < detailList.Count; i++)
            {
                YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(detailList[i].factory_order_id, clothFactoryId);
                if (yNFactoryOrder != null)
                {
                    orderList.Add(yNFactoryOrder);
                }
            }
            ViewBag.yNStorageTop = yNStorageTop;
            ViewBag.orderList = orderList;
            ViewBag.detailList = detailList;
            return View();
        }

        /// <summary>
        /// 打印出库单
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintStorageOutList2()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }

            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //string outOrderList = Request.QueryString["date"]; //outOrderList:"1,2,3"出库订单ID序列
            //string[] tempOut = outOrderList.Split(',');
            
            //int[] outOrders = new int[tempOut.Length];
            //for (var i = 0; i < tempOut.Length; i++)
            //{
            //    outOrders[i] = Convert.ToInt32(tempOut[i]);
            //}
            //API.Controllers.FactoryStorageOutController storageOutCtrl = new API.Controllers.FactoryStorageOutController();
            //List<YNStorageOut> outList = storageOutCtrl.GetListInIdArray(outOrders, clothFactoryId);

            //int[] factoryOrderIds = new int[outList.Count];
            //for (var i = 0; i < outList.Count; i++)
            //{
            //    factoryOrderIds[i] = outList[i].order_id;
            //}
            //API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> order = factoryOrder.GetListInIdArray(factoryOrderIds, clothFactoryId);

            //if (outList.Count > 0) {
            //    ViewBag.plan_name = outList[0].out_date.ToString("yyyy-MM-dd")+DateTime.Now.ToString(" HH:mm");
            //}

            //ViewBag.outList = outList;
            //ViewBag.orderList = order;
            return View();
        }
        /// <summary>
        /// 打印销货单
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintDestoryList2()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }

            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //string outOrderList = Request.QueryString["date"]; //outOrderList:"1,2,3"出库订单ID序列
            //string[] tempOut = outOrderList.Split(',');

            //int[] outOrders = new int[tempOut.Length];
            //for (var i = 0; i < tempOut.Length; i++)
            //{
            //    outOrders[i] = Convert.ToInt32(tempOut[i]);
            //}
            //API.Controllers.FactoryStorageOutController storageOutCtrl = new API.Controllers.FactoryStorageOutController();
            //List<YNStorageOut> outList = storageOutCtrl.GetListInIdArray(outOrders, clothFactoryId);

            
            //int[] factoryOrderIds = new int[outList.Count];
            //for (var i = 0; i < outList.Count; i++)
            //{
            //    factoryOrderIds[i] = outList[i].order_id;
            //}
            //API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> order = factoryOrder.GetListInIdArray(factoryOrderIds, clothFactoryId);

            
            //ViewBag.outList = outList;
            //ViewBag.orderList = order;
            return View();
        }
        /// <summary>
        /// 增加出库单号
        /// </summary>
        /// <returns></returns>
        //public JsonResult addOutNumber()
        //{
        //    if (!checkSession())
        //    {
        //        return getLoginJsonMessage(0, "请登录");
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    string orders = Request.QueryString["orders"];
        //    if(string.IsNullOrEmpty(orders)){
        //        return getLoginJsonMessage(0, "参数不能为空");
        //    }
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<OutNumberInfo> orderList = js.Deserialize<List<OutNumberInfo>>(orders);
        //    API.Controllers.StorageTopController storageTopController = new API.Controllers.StorageTopController();
        //    API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //    int count = storageTopController.getStorageTopListCount(clothFactoryId, "", DateTime.Now.ToString("yyyy-MM-dd"));
        //    YNStorageTop yNStorageTop = new YNStorageTop();
        //    yNStorageTop.jiajufactory_id = clothFactoryId;
        //    yNStorageTop.out_time = DateTime.Now;
        //    yNStorageTop.storage_out_number_show = DateTime.Now.ToString("yyyy-MM-dd") + "_" + (count + 1);
        //    yNStorageTop.order_number = orderList.Count();
        //    yNStorageTop.goods_number = orderList.Sum(s => s.num);
        //    yNStorageTop.state = API.Controllers.StorageTopController.state_0;
        //    yNStorageTop.create_time = DateTime.Now;
        //    yNStorageTop.modify_time = DateTime.Now;
        //    //事务管理,设置事务隔离级别
        //    TransactionOptions transactionOption = new TransactionOptions();
        //    transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
        //    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
        //    {
        //        storageTopController.Create(yNStorageTop, clothFactoryId);
        //        for (int i = 0; i < orderList.Count; i++)
        //        {
        //            YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(orderList[i].id, clothFactoryId);
        //            if (yNFactoryOrder == null)
        //            {
        //                return getLoginJsonMessage(0, "参数错误");
        //            }
        //            //if (yNFactoryOrder.factory_order_status != API.Controllers.FactoryOrderController.factory_order_status2)
        //            //{
        //            //    return getLoginJsonMessage(0, "订单状态还未完成，不能进行库存操作");
        //            //}
        //            if (yNFactoryOrder.storage_out + orderList[i].num > yNFactoryOrder.storage_in)
        //            {
        //                return getLoginJsonMessage(0, "出库数量超过剩余库存数量");
        //            }
        //            yNFactoryOrder.storage_out += orderList[i].num;
        //            yNFactoryOrder.modify_time = DateTime.Now;
        //            factoryOrderController.SaveChanges();
        //            //添加库出库记录
        //            API.Controllers.StorageTypeController storageTypeController = new API.Controllers.StorageTypeController();
        //            YNStorageDetail yNStorageDetail = new YNStorageDetail();
        //            yNStorageDetail.jiaju_factory_id = clothFactoryId;
        //            yNStorageDetail.storage_out_number = yNStorageTop.id;
        //            yNStorageDetail.factory_order_id = yNFactoryOrder.id;
        //            yNStorageDetail.order_id = yNFactoryOrder.order_id;
        //            yNStorageDetail.produce_id = yNFactoryOrder.produce_id;
        //            yNStorageDetail.operator_id = Convert.ToInt32(Session["UserId"]);
        //            yNStorageDetail.operator_name = Convert.ToString(Session["NickName"]);
        //            yNStorageDetail.type = API.Controllers.StorageTypeController.type_1;
        //            yNStorageDetail.number = orderList[i].num;
        //            yNStorageDetail.unit = yNFactoryOrder.unit;
        //            yNStorageDetail.relation_type = API.Controllers.StorageTypeController.relation_type0;
        //            yNStorageDetail.create_time = DateTime.Now;
        //            yNStorageDetail.modify_time = DateTime.Now;
        //            storageTypeController.Create(yNStorageDetail, clothFactoryId);
        //        }
        //        //提交事务
        //        transaction.Complete();
        //    }
           
        //    return getLoginJsonMessage(1, "成功");
        //}

        /// <summary>
        /// 确认出库
        /// </summary>
        /// <returns></returns>
        public JsonResult comfirm()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int plan_id = Convert.ToInt32(Request.QueryString["plan_id"]);
            API.Controllers.StorageTopController storageTopController = new API.Controllers.StorageTopController();
            YNStorageTop yNStorageTop = storageTopController.getStorageTopById(plan_id, clothFactoryId);
            if (yNStorageTop == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            yNStorageTop.state = 1;
            yNStorageTop.out_time = DateTime.Now;
            storageTopController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }
        /// <summary>
        /// 出库计划列表
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStoresD()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 出库计划列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStoresDList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //DateTime outDate = Convert.ToDateTime(Request.QueryString["outDate"]);
            //int outStatus = Convert.ToInt32(Request.QueryString["outStatus"]);

            //YNDIY.API.Controllers.FactoryStorageOutController _out_ctrl = new API.Controllers.FactoryStorageOutController();
            //YNStorageOut condition = new YNStorageOut();
            //condition.jiaju_factory_id = clothFactoryId;
            //condition.out_date = outDate;
            //condition.status = outStatus;
            //List<YNStorageOut> list = _out_ctrl.GetListByDate(condition);

            //ViewBag.list = list;
            return View();
        }

        /// <summary>
        /// 添加出库
        /// </summary>
        /// <returns></returns>
        public JsonResult AddStorageOut()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登录");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int outNumber = Convert.ToInt32(Request.QueryString["outNumber"]);
            //DateTime outDate = Convert.ToDateTime(Request.QueryString["outDate"]);
            //string remarks = Request.QueryString["remarks"];
            //int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            //API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            //API.Controllers.FactoryStorageOutController storageOutOrder = new API.Controllers.FactoryStorageOutController();
            //YNFactoryOrder order = factoryOrder.getFactoryOrderByIdFactory(orderId, clothFactoryId);
            //if (order.storage_out >= order.number) {
            //    return getLoginJsonMessage(0, "该订单已经出库完毕，不能再操作");
            //}
            //if (outNumber > (order.storage_in - order.storage_out - order.storage_lock))
            //{
            //    return getLoginJsonMessage(0, "出库数量不能大于库存剩余数量");
            //}
            //order.storage_lock = order.storage_lock + outNumber;
            ////事务管理,设置事务隔离级别
            //TransactionOptions transactionOption = new TransactionOptions();
            //transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            //{
            //    factoryOrder.SaveChanges();

            //    YNStorageOut outOrder = new YNStorageOut();
            //    outOrder.customer_id = order.shop_id;
            //    outOrder.customer_name = order.shop_name;
            //    outOrder.order_id = order.id;
            //    outOrder.order_name = order.order_id;
            //    outOrder.production_name = order.produce_id;
            //    outOrder.jiaohuo_date = order.factory_delivery_day;
            //    outOrder.model_id = order.model_id;
            //    outOrder.model_name = order.model_name;
            //    outOrder.format = order.format;
            //    outOrder.color = order.color;
            //    outOrder.storage_from = order.produce_type;
            //    outOrder.price = order.money;
            //    outOrder.all_count = order.number;
            //    outOrder.unit = order.unit;
            //    outOrder.out_count = outNumber;
            //    outOrder.jiaju_factory_id = clothFactoryId;
            //    outOrder.out_date = outDate;
            //    outOrder.create_date = DateTime.Now;
            //    outOrder.modify_date = DateTime.Now;
            //    outOrder.out_remarks = remarks;

            //    storageOutOrder.Create(outOrder);
            //    storageOutOrder.SaveChanges();
                
            //    transaction.Complete();
            //}
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 获取订单数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStorageMoreOptionNumber()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            //API.Controllers.FactoryStorageOutController storageOutOrder = new API.Controllers.FactoryStorageOutController();
            
            //var outOrder = storageOutOrder.GetEntityById(orderId,clothFactoryId);

            //API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            ////YNFactoryOrder order = factoryOrder.getFactoryOrderById(outOrder.order_id, outOrder.customer_id);
            //YNFactoryOrder order = factoryOrder.getFactoryOrderByIdFactory(outOrder.order_id, clothFactoryId);

            //if (order != null)
            //{
            //    return Json(new { code = 1, message = "成功", data = order }, JsonRequestBehavior.AllowGet);
            //}
            
            return getLoginJsonMessage(0, "查询失败");
        }

        /// <summary>
        /// 修改出库数量
        /// </summary>
        /// <returns></returns>
        public JsonResult ModifyStorageOutNum()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登录");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int outNumber = Convert.ToInt32(Request.QueryString["outNumber"]);
            //int outOrderId = Convert.ToInt32(Request.QueryString["orderId"]);
            //API.Controllers.FactoryStorageOutController storageOutCtrl = new API.Controllers.FactoryStorageOutController();
            //var outOrder = storageOutCtrl.GetEntityById(outOrderId, clothFactoryId);
            //if (outOrder == null) {
            //    return getLoginJsonMessage(0, "误操作");
            //}
            //API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            //YNFactoryOrder order = factoryOrder.getFactoryOrderByIdFactory(outOrder.order_id, clothFactoryId);
            //var _can_out_count = outOrder.out_count + (order.storage_in - order.storage_lock - order.storage_out);//可出库数量
                        
            //if (order.storage_out >= order.number)
            //{
            //    return getLoginJsonMessage(0, "该订单已经出库完毕，不能再操作");
            //}
            //if (outNumber > _can_out_count)
            //{
            //    return getLoginJsonMessage(0, "出库数量不能大于库存剩余数量");
            //}
            //order.storage_lock -= outOrder.out_count;
            //order.storage_lock += outNumber;
            ////事务管理,设置事务隔离级别
            //TransactionOptions transactionOption = new TransactionOptions();
            //transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            //{
            //    factoryOrder.SaveChanges();

            //    outOrder.out_count = outNumber;
            //    outOrder.modify_date = DateTime.Now;

            //    storageOutCtrl.SaveChanges();

            //    transaction.Complete();
            //}
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 删除出库订单
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteStorageOut()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登录");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int outOrderId = Convert.ToInt32(Request.QueryString["orderId"]);
            //API.Controllers.FactoryStorageOutController storageOutCtrl = new API.Controllers.FactoryStorageOutController();
            //var outOrder = storageOutCtrl.GetEntityById(outOrderId, clothFactoryId);
            //if (outOrder == null)
            //{
            //    return getLoginJsonMessage(0, "误操作");
            //}
            //API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            //YNFactoryOrder order = factoryOrder.getFactoryOrderByIdFactory(outOrder.order_id, clothFactoryId);
            //order.storage_lock -= outOrder.out_count;
            //if (outOrder.status == 0)
            //{
            //    //事务管理,设置事务隔离级别
            //    TransactionOptions transactionOption = new TransactionOptions();
            //    transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            //    {
            //        factoryOrder.SaveChanges();

            //        outOrder.delete_status = 1;
            //        outOrder.modify_date = DateTime.Now;

            //        storageOutCtrl.SaveChanges();

            //        transaction.Complete();
            //    }
            //    return getLoginJsonMessage(1, "成功");
            //}
            //else {
            //    return getLoginJsonMessage(0, "误操作，请刷新页面重试");
            //}

            return null;
        }

        /// <summary>
        /// 审核出库
        /// </summary>
        /// <returns></returns>
        public JsonResult ExamineStorageOutOrder()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登录");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //int outOrderId = Convert.ToInt32(Request.QueryString["orderId"]);
            //int examine = Convert.ToInt32(Request.QueryString["examine"]);
            //if (examine != 1 && examine != 2)
            //{
            //    return getLoginJsonMessage(0, "误操作");            
            //}
            //string remarks = "";
            //if (!String.IsNullOrEmpty(Request.QueryString["remarks"])) {
            //    remarks = Request.QueryString["remarks"];
            //}

            //API.Controllers.FactoryStorageOutController storageOutCtrl = new API.Controllers.FactoryStorageOutController();
            //var outOrder = storageOutCtrl.GetEntityById(outOrderId, clothFactoryId);
            //if (outOrder == null)
            //{
            //    return getLoginJsonMessage(0, "误操作");
            //}
            //if (outOrder.status == 0)
            //{
            //    if (examine == 1)//通过
            //    {
            //        outOrder.status = examine;
            //        outOrder.examine_remarks = remarks;
            //        outOrder.modify_date = DateTime.Now;

            //        YNDIY.API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
            //        YNFactoryOrder order = order_ctrl.FactoryGetFactoryOrderById(outOrder.order_id, clothFactoryId);
            //        if (order == null) {
            //            return getLoginJsonMessage(0, "订单数据错误");
            //        }
            //        //如果订单余额小于出库额
            //        if (order.balance_money < (order.money * outOrder.out_count))
            //        {
            //            YNDIY.API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            //            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(outOrder.customer_id, clothFactoryId);
            //            if (shop == null)
            //            {
            //                return getLoginJsonMessage(0, "客户数据错误");
            //            }
            //            if (shop.balance_money >= (order.money * outOrder.out_count - order.balance_money)) {
            //                shop.lock_money += (order.money * outOrder.out_count - order.balance_money);
            //                shop.balance_money -= (order.money * outOrder.out_count - order.balance_money);
            //            }

            //            TransactionOptions transactionOption = new TransactionOptions();
            //            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            //            {
            //                shop_ctrl.SaveChanges();
            //                storageOutCtrl.SaveChanges();
            //                transaction.Complete();
            //            }
            //            return getLoginJsonMessage(1, "成功");
            //        }
            //            //订单余额大于或等于出库额
            //        else
            //        {
            //            storageOutCtrl.SaveChanges();
            //            return getLoginJsonMessage(1, "成功");
            //        }
            //    }
            //    else//不通过
            //    {
            //        API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            //        YNFactoryOrder order = factoryOrder.getFactoryOrderByIdFactory(outOrder.order_id, clothFactoryId);
            //        order.storage_lock -= outOrder.out_count;
            //        TransactionOptions transactionOption = new TransactionOptions();
            //        transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //        using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            //        {
            //            factoryOrder.SaveChanges();
            //            outOrder.status = examine;
            //            outOrder.examine_remarks = remarks;
            //            outOrder.modify_date = DateTime.Now;
            //            storageOutCtrl.SaveChanges();
            //            transaction.Complete();
            //        }
            //        return getLoginJsonMessage(1, "成功");
            //    }
            //}
            //else
            //{
            //    return getLoginJsonMessage(0, "误操作，请刷新页面重试");
            //}

            return null;
        }

        /// <summary>
        /// 出库
        /// </summary>
        /// <returns></returns>
        public JsonResult SetStorageOut()
        {
            //if (!checkSession())
            //{
            //    return getLoginJsonMessage(0, "请登录");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //string outOrderList = Request.QueryString["outOrderList"]; //outOrderList:"1,2,3"出库订单ID序列
            //string[] tempOut = outOrderList.Split(',');
            //if (tempOut.Length == 1) {
            //    if (tempOut[0] == "") {
            //        return getLoginJsonMessage(0, "参数错误");
            //    }
            //}
            //int[] outOrders = new int[tempOut.Length];
            //for(var i = 0;i<tempOut.Length;i++){
            //    outOrders[i] = Convert.ToInt32(tempOut[i]);
            //}
            //API.Controllers.FactoryStorageOutController storageOutCtrl = new API.Controllers.FactoryStorageOutController();
            //List<YNStorageOut> outList = storageOutCtrl.GetListInIdArray(outOrders,clothFactoryId);
            //int[] factoryOrderIds = new int[outList.Count];
            //for (var i = 0; i < outList.Count; i++) {
            //    factoryOrderIds[i] = outList[i].order_id;
            //}
            //API.Controllers.FactoryOrderController factoryOrder = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> order = factoryOrder.GetListInIdArray(factoryOrderIds,clothFactoryId);
            //if (order.Count == 0) {
            //    return getLoginJsonMessage(0, "数据错误");
            //}
            //int[] shop_id_list = new int[outList.Count];
            //for (var i = 0; i < outList.Count; i++) {
            //    shop_id_list[i] = outList[i].customer_id;
            //}
            //API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            //List<YNShopInfo> shop_list = shop_ctrl.GetShopInfoInArray(shop_id_list, clothFactoryId);
            //if (shop_list.Count == 0) {
            //    return getLoginJsonMessage(0, "客户数据错误");
            //}
            //TransactionOptions transactionOption = new TransactionOptions();
            //transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            //using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            //{
            //    for (var i = 0; i < outList.Count; i++)
            //    {
            //        if (outList[i].status == 1)
            //        {
            //            outList[i].status = 3;
            //            for (var j = 0; j < order.Count; j++)
            //            {
            //                if (order[j].id == outList[i].order_id)
            //                {
            //                    order[j].storage_lock -= outList[i].out_count;
            //                    order[j].storage_out += outList[i].out_count;
            //                    decimal wait_to_pay = outList[i].out_count * order[j].money;
            //                    decimal payed_money = 0;
            //                    if (order[j].balance_money >= wait_to_pay)
            //                    {//订单余额冲抵
            //                        order[j].balance_money -= wait_to_pay;
            //                        wait_to_pay = 0;
            //                    }
            //                    else 
            //                    {
            //                        //订单余额以及帐户余额一起冲抵
            //                        wait_to_pay -= order[j].balance_money;
            //                        order[j].balance_money = 0;
            //                        for (var k = 0; k < shop_list.Count; k++) {
            //                            if (shop_list[k].id == order[j].shop_id) {
            //                                if (shop_list[k].lock_money > 0) { //如果锁定金额大于0
            //                                    if (shop_list[k].lock_money >= wait_to_pay)
            //                                    {
            //                                        shop_list[k].lock_money -= wait_to_pay;
            //                                        payed_money += wait_to_pay;
            //                                        wait_to_pay = 0;
            //                                        break;
            //                                    }
            //                                    else {
            //                                        wait_to_pay -= shop_list[k].lock_money;
            //                                        payed_money += shop_list[k].lock_money;
            //                                        shop_list[k].lock_money = 0;
            //                                    }
            //                                }
            //                                if (shop_list[k].balance_money > 0) {//如果帐户余额大于0
            //                                    if (shop_list[k].balance_money >= wait_to_pay)
            //                                    {
            //                                        shop_list[k].balance_money -= wait_to_pay;
            //                                        payed_money += wait_to_pay;
            //                                        wait_to_pay = 0;
            //                                    }
            //                                    else {
            //                                        wait_to_pay -= shop_list[k].balance_money;
            //                                        payed_money += shop_list[k].balance_money;
            //                                        shop_list[k].balance_money = 0;
            //                                    }
            //                                }
            //                                break;
            //                            }
            //                        }
            //                        order[j].payed_money += payed_money;
            //                    }
            //                    order[j].wating_pay += wait_to_pay;//出库时，就增加欠款
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    shop_ctrl.SaveChanges();
            //    factoryOrder.SaveChanges();
            //    storageOutCtrl.SaveChanges();
            //    transaction.Complete();
            //}
            return getLoginJsonMessage(1, "成功");
        }

        //public JsonResult GetList()
        //{
        //    API.Controllers.FactoryStorageOutController ctrl = new API.Controllers.FactoryStorageOutController();

        //    var list = ctrl.GetListInIdArray(new int[]{1,3,4});
        //    return Json(new{data=list},JsonRequestBehavior.AllowGet);
        //}
        
    }
}