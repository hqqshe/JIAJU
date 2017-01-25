using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YNDIY.API.Models;
using YNDIY.API.Controllers;

namespace YNDIY.Admin.Controllers
{
    public class ERPFinanceController : ParentController
    {
        /// <summary>
        /// 出库计划审核
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStoresCheck()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 出库计划审核数据
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverStoresCheckList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //DateTime outDate = Convert.ToDateTime(Request.QueryString["outDate"]);
            //int outStatus = Convert.ToInt32(Request.QueryString["outStatus"]);
            //string searchKey = Request.QueryString["searchKey"];

            //YNDIY.API.Controllers.FactoryStorageOutController _out_ctrl = new API.Controllers.FactoryStorageOutController();
            //YNStorageOut condition = new YNStorageOut();
            //condition.jiaju_factory_id = clothFactoryId;
            //condition.out_date = outDate;
            //condition.status = outStatus;
            //List<YNStorageOut> list;
            //if (searchKey != "")
            //{
            //    list = _out_ctrl.GetListByDate(condition, searchKey);
            //}
            //else
            //{
            //    list = _out_ctrl.GetListByDate(condition);
            //}
            //int[] id_arr = new int[list.Count];
            //for (var i = 0; i < list.Count; i++) {
            //    id_arr[i] = list[i].order_id;
            //}
            //YNDIY.API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> order = order_ctrl.GetListInIdArray(id_arr, clothFactoryId);
            //ViewBag.order = order;
            //ViewBag.list = list;
            return View();
        }
    /// <summary>
        /// 订单金额统计
        /// </summary>
        /// <returns></returns>
        public ActionResult AmountStatistics()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
         /// <summary>
        /// 订单金额统计列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AmountStatisticsList()
        {
            //if (!checkSession())
            //{
            //    return loginRerirect();
            //}
            //int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            //string startDate = Request.QueryString["startTime"];
            //string endDate = Request.QueryString["endTime"];
            //int yixing_type = Convert.ToInt32(Request.QueryString["type"]);
            //int modify_type = Convert.ToInt32(Request.QueryString["priceType"]);
            //string searchKey = Request.QueryString["searchKey"]; 
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
            //YNDIY.API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = order_ctrl.getFactoryOrderPriceList(factory_id, startDate, endDate, yixing_type, modify_type, searchKey, pageIndex, pageSize);
            //int count = order_ctrl.getFactoryOrderPriceCount(factory_id, startDate, endDate, yixing_type, modify_type, searchKey);
            //API.Controllers.PagesController page = new API.Controllers.PagesController();
            //page.GetPage(pageIndex, count, pageSize);
            //ViewBag.orderList = orderList;
            //ViewBag.page = page;
            return View();
        }
    /// <summary>
        /// 客户账户详情
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerAcountDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["id"]);

            YNDIY.API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(customer_id, factory_id);
            YNDIY.API.Controllers.FactoryOrderController sale_order_ctrl = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> sale_order_list = sale_order_ctrl.getFactoryAllWaitPayOrderByShopId(shop.id, factory_id);
            decimal AllWaitPay = 0;
            for (var i = 0; i < sale_order_list.Count; i++)
            {
                AllWaitPay += sale_order_list[i].wating_pay;
            }
            ViewBag.AllWaitPay = AllWaitPay;
            ViewBag.shop = shop;

            return View();
        }
        /// <summary>
        /// 客户账户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerAcountList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int type = Convert.ToInt32(Request.QueryString["type"]);
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
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            List<YNShopInfo> shopInfoList = shopInfoController.getShopInfoListByFactoryId(clothFactoryId, key, type, pageIndex, pageSize);
            int count = shopInfoController.getShopInfoListByFactoryIdCount(clothFactoryId, key, type);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.shopInfoList = shopInfoList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        ///产值统计
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductiveStatistics()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        ///产值统计列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductiveStatisticsList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            string startDate = Request.QueryString["startTime"];
            string endDate = Request.QueryString["endTime"];
            string matchShopName = Request.QueryString["searchKey"];
            YNDIY.API.Controllers.FinanceController finance_ctrl = new API.Controllers.FinanceController();
            List<CustomerStatisticsModel> list = finance_ctrl.GetCustomerStatisticsList(factory_id, startDate, endDate, matchShopName);
            ViewBag.list = list;
            ViewBag.start = startDate;
            ViewBag.end = endDate;
            return View();
        }
        /// <summary>
        /// 客户优惠列表
        /// </summary>
        /// <returns></returns>
        public ActionResult InfoDiscount()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// 客户订单列表
        public ActionResult InfoOrderList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["id"]);
            int payStatus = Convert.ToInt32(Request.QueryString["payStatus"]);
            string startDate = Request.QueryString["startTime"];
            string endDate = Request.QueryString["endTime"];
            string searchKey = Request.QueryString["searchKey"];
            int searchType = Convert.ToInt32(Request.QueryString["searchType"]);
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
            //下单日期
            if (searchType == 0)
            {
                YNDIY.API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
                List<YNFactoryOrder> orderList = order_ctrl.getFactoryOrderByShopId(customer_id, factory_id, payStatus, startDate, endDate, searchKey, pageIndex, pageSize);
                //查询欠款数据
                List<YNFactoryOrder> waitpay_order_list = order_ctrl.getFactoryWaitPayOrderList(customer_id, factory_id, payStatus, startDate, endDate, searchKey);
                decimal wait_pay = 0;
                for (var i = 0; i < waitpay_order_list.Count; i++)
                {
                    wait_pay += waitpay_order_list[i].wating_pay;
                }
                int count = order_ctrl.getFactoryOrderCountByShopId(customer_id, factory_id, payStatus, startDate, endDate, searchKey);
                API.Controllers.PagesController page = new API.Controllers.PagesController();
                page.GetPage(pageIndex, count, pageSize);
                ViewBag.orderList = orderList;
                ViewBag.startTime = startDate;
                ViewBag.endTime = endDate;
                ViewBag.wait_pay = wait_pay;
                ViewBag.type = 0;
                ViewBag.page = page;
            }
            else 
            {//出库日期
   
                YNDIY.API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
                List<YNStorageOut> out_list = out_ctrl.GetOutListInSomeTime(factory_id, startDate, endDate, -1, customer_id);
                List<int> sale_id_list = new List<int>();
                for (var i = 0; i < out_list.Count; i++) {
                    sale_id_list.Add(out_list[i].order_id);
                }
                YNDIY.API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
                List<YNFactoryOrder> orderList = order_ctrl.GetSaleOrderByIdInList(factory_id, customer_id, sale_id_list, payStatus, searchKey, pageIndex, pageSize);
                //查询欠款数据
                decimal wait_pay = 0;
                for (var i = 0; i < out_list.Count; i++)
                {
                    wait_pay += (out_list[i].out_plan_money - out_list[i].payed_money);
                }
                int count = order_ctrl.GetSaleOrderCountByIdInList(factory_id, customer_id, sale_id_list, payStatus, searchKey);
                API.Controllers.PagesController page = new API.Controllers.PagesController();
                page.GetPage(pageIndex, count, pageSize);
                sale_id_list = new List<int>();
                for (var i = 0; i < orderList.Count; i++)
                {
                    sale_id_list.Add(orderList[i].id);
                }
                out_list = out_list.Where(w => sale_id_list.Contains(w.order_id)).ToList();

                ViewBag.orderList = orderList;
                ViewBag.startTime = startDate;
                ViewBag.endTime = endDate;
                ViewBag.wait_pay = wait_pay;
                ViewBag.type = 1;
                ViewBag.page = page;
                ViewBag.out_list = out_list;
            }
            return View();
        }
        /// 待审核出库列表
        public ActionResult InfoPendingList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["id"]);
            string startDate = Request.QueryString["startTime"];
            string endDate = Request.QueryString["endTime"];
            string searchKey = Request.QueryString["searchKey"];
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
            API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
            List<YNStorageOut> out_list = out_ctrl.GetWaitPayExamineList(factory_id, customer_id, startDate, endDate, searchKey, pageIndex, pageSize);
            int count = out_ctrl.GetWaitPayExamineCount(factory_id, customer_id, startDate, endDate, searchKey);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);

            ViewBag.page = page;
            ViewBag.out_list = out_list;
            return View();
        }
        /// 客户收款列表
        public ActionResult InfoReceipt()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["id"]);

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
            YNDIY.API.Controllers.FinanceController finance_ctrl = new API.Controllers.FinanceController();
            List<YNRecharge> rechargeList = finance_ctrl.GetRecordList(factory_id, customer_id, pageIndex, pageSize);
            int count = finance_ctrl.GetRecordCount(factory_id, customer_id);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.list = rechargeList;
            ViewBag.page = page;
            return View();
        }
        /// 账户充值
        public JsonResult Recharge()
        {

            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int operator_id = Convert.ToInt32(Session["UserId"]);
            string operator_name = Session["Account"].ToString();
            int customer_id = Convert.ToInt32(Request.QueryString["shopId"]);
            decimal money = Convert.ToDecimal(Request.QueryString["money"]);
            string remarks = Request.QueryString["remarks"];
            int type = 1;//1-普通充值 2-优惠充值
            int charge_type = Convert.ToInt32(Request.QueryString["chargeType"]);// 0-帐户余额  1-锁定余额

            YNDIY.API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetShopInfoByID(customer_id);

            if (shop == null || money <=0) {
                return getLoginJsonMessage(0, "参数错误");            
            }

            YNRecharge entity = new YNRecharge();
            entity.factory_id = factory_id;
            entity.customer_id = customer_id;
            entity.money = money;
            entity.type = type;
            entity.remark = remarks;
            entity.operator_id = operator_id;
            entity.operator_name = operator_name;
            entity.create_date = DateTime.Now;

            YNDIY.API.Controllers.FinanceController finance = new API.Controllers.FinanceController();

            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                finance.Create(entity);
                if (charge_type == 0)
                {
                    shop.balance_money += money;
                }
                else 
                {
                    shop.lock_money += money;
                }
                if (type == 1) { 
                    //恢复信用额度
                    shop.credit_lock -= money;
                    if (shop.credit_lock < 0)
                    {
                        shop.credit_lock = 0;
                    }
                }
                shop_ctrl.SaveChanges();
                transaction.Complete();
            }

            return getLoginJsonMessage(1, "充值成功");
        }
        /// 冲抵
        public JsonResult PayTheOrderMoney()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }

            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["shopId"]);
            int sale_id = Convert.ToInt32(Request.QueryString["saleId"]);
            int type = Convert.ToInt32(Request.QueryString["type"]);//查询日期类型 0：下单日期 1：出库日期
            decimal money = Convert.ToDecimal(Request.QueryString["money"]);
            string startDate = Request.QueryString["startTime"];
            string endDate = Request.QueryString["endTime"];
            if (money <= 0)
            {
                return getLoginJsonMessage(0, "输入金额错误");
            }
            API.Controllers.FactoryOrderController sale_order_ctrl = new API.Controllers.FactoryOrderController();
            YNFactoryOrder sale_order = sale_order_ctrl.GetSaleOrderByIdAndCustomerId(factory_id, sale_id, customer_id);
            if (sale_order == null) {
                return getLoginJsonMessage(0, "未查询到该销售单");
            }
            API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(sale_order.shop_id, factory_id);
            if (shop == null)
            {
                return getLoginJsonMessage(0, "未查询到该销售单的客户信息");
            }
            API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();

            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                //交定金
                if ((sale_order.out_lock_number + sale_order.product_out_number) == 0 && sale_order.pre_payed == 0)
                {
                    if (money > sale_order.money)
                    {
                        return getLoginJsonMessage(0, "定金金额不能大于该销售单价值");
                    }
                    if ((shop.balance_money + shop.lock_money) < money)
                    {
                        return getLoginJsonMessage(0, "该客户帐户里面没有足够的金额交定金");
                    }
                    else
                    {
                        sale_order.balance_money = money;//设置销售单余额
                        sale_order.pre_payed = money;//设置销售单定金
                        if (shop.lock_money >= money)
                        {
                            shop.lock_money -= money;
                        }
                        else
                        {
                            money -= shop.lock_money;
                            shop.lock_money = 0;
                            shop.balance_money -= money;
                        }
                    }
                }
                //冲抵
                else
                {
                    decimal temp_money = money;
                    sale_order.payed_money += money;
                    sale_order.wating_pay -= money;
                    List<YNStorageOut> plan_out_list = null;
                    //下单日期冲抵
                    if (type == 0)
                    {
                        if (money > sale_order.wating_pay)
                        {
                            return getLoginJsonMessage(0, "冲抵金额不能大于该销售单欠款金额");
                        }
                        if ((shop.balance_money + shop.lock_money + sale_order.balance_money) < money)
                        {
                            return getLoginJsonMessage(0, "该客户帐户里面没有足够的金额做冲抵，可用于冲抵的金额为：" + (shop.balance_money + shop.lock_money + sale_order.balance_money));
                        }
                        plan_out_list = out_ctrl.GetOutListInSomeTime(factory_id, sale_order_id: sale_order.id, is_wait_pay: true);

                    }
                    //出库日期冲抵
                    else
                    {
                        //查询该销售单 某段时间内 已经出库的 计划出库单
                        plan_out_list = out_ctrl.GetOutListInSomeTime(factory_id, startDate, endDate, sale_order.id, customer_id, is_wait_pay: true);

                        //声明欠款变量
                        decimal sale_order_wait_pay_money = 0;
                        //计算出该订单在 某段时间内的 欠款之和
                        for (var i = 0; i < plan_out_list.Count; i++)
                        {
                            sale_order_wait_pay_money += (plan_out_list[i].out_plan_money + plan_out_list[i].payed_money);
                        }
                        if (money > sale_order_wait_pay_money)
                        {
                            return getLoginJsonMessage(0, "冲抵金额不能大于该销售单欠款金额");
                        }
                        if ((shop.balance_money + shop.lock_money + sale_order.balance_money) < money)
                        {
                            return getLoginJsonMessage(0, "该客户帐户里面没有足够的金额做冲抵，可用于冲抵的金额为：" + (shop.balance_money + shop.lock_money + sale_order.balance_money));
                        }

                    }

                    //销售单余额冲抵
                    if (money > 0)
                    {
                        if (sale_order.balance_money > 0)
                        {
                            if (sale_order.payed_money >= money)
                            {
                                sale_order.balance_money -= money;
                                money = 0;
                            }
                            else
                            {
                                money -= sale_order.balance_money;
                                sale_order.balance_money = 0;
                            }
                        }
                    }
                    //帐户锁定金额冲抵
                    if (money > 0)
                    {
                        if (shop.lock_money > 0)
                        {
                            if (shop.lock_money >= money)
                            {
                                shop.lock_money -= money;
                                money = 0;
                            }
                            else
                            {
                                money -= shop.lock_money;
                                shop.lock_money = 0;
                            }
                        }
                    }
                    //帐户余额冲抵
                    if (money > 0)
                    {
                        if (shop.balance_money > 0)
                        {
                            if (shop.balance_money >= money)
                            {
                                shop.balance_money -= money;
                                money = 0;
                            }
                            else
                            {
                                money -= shop.balance_money;
                                shop.balance_money = 0;
                            }
                        }
                    }
                    //该销售单的所有欠款出库单 逐个冲抵
                    for (var i = 0; i < plan_out_list.Count; i++)
                    {
                        if (temp_money == 0)
                        {
                            break;
                        }
                        decimal out_wait_pay = (plan_out_list[i].out_plan_money - plan_out_list[i].payed_money);
                        if (temp_money >= out_wait_pay)
                        {
                            temp_money -= out_wait_pay;
                            plan_out_list[i].payed_money += out_wait_pay;
                        }
                        else
                        {
                            plan_out_list[i].payed_money += temp_money;
                            temp_money = 0;
                        }
                    }
                }
                sale_order_ctrl.SaveChanges();
                out_ctrl.SaveChanges();
                shop_ctrl.SaveChanges();
                transaction.Complete();
            }

            return getLoginJsonMessage(1, "成功");
        }
        private void calcMoney(ref decimal all_money, ref decimal wait_pay_money)
        {
            if (all_money <= wait_pay_money)
            {
                wait_pay_money = all_money;
            }
        }
        /// 批量冲抵
        public JsonResult PayOrderListMoney()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }

            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["shopId"]);
            decimal money = Convert.ToDecimal(Request.QueryString["money"]);
            int type = Convert.ToInt32(Request.QueryString["type"]);//查询日期类型 0：下单日期 1：出库日期
            string startDate = Request.QueryString["startTime"];
            string endDate = Request.QueryString["endTime"];
            string order_id_list = Request.QueryString["orderList"];
            if (String.IsNullOrEmpty(order_id_list)) {
                return getLoginJsonMessage(0, "订单参数错误");
            }
            string[] order_id_list_str = order_id_list.Split(',');
            List<int> order_id_list_int = new List<int>();
            for (var i = 0; i < order_id_list_str.Length; i++) {
                order_id_list_int.Add(Convert.ToInt32(order_id_list_str[i]));
            }

            YNDIY.API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> orderList = order_ctrl.GetListInIdList(order_id_list_int, factory_id, customer_id);
            if (orderList.Count <= 0)
            {
                return getLoginJsonMessage(0, "未找到任何订单");
            }
            YNDIY.API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(customer_id, factory_id);
            if (shop == null)
            {
                return getLoginJsonMessage(0, "客户ID参数错误");
            }

            YNDIY.API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
            List<YNStorageOut> out_list = new List<YNStorageOut>();
            //下单日期
            if (type == 0)
            {
                out_list = out_ctrl.GetWaitPayOutListInSaleIdList(factory_id, customer_id, order_id_list_int);
            }
            //出库日期
            else 
            {
                out_list = out_ctrl.GetWaitPayOutListInSaleIdList(factory_id, customer_id, order_id_list_int, startDate, endDate);
            }
            decimal temp_money = shop.lock_money + shop.balance_money;
            for (var i = 0; i < orderList.Count; i++)
            {
                temp_money += orderList[i].balance_money;
            }
            for (var i = 0; i < out_list.Count; i++)
            {
                temp_money += (out_list[i].sale_payed + out_list[i].lock_payed + out_list[i].balance_payed);
                //temp_money -= (out_list[i].out_plan_money - out_list[i].payed_money);
            }
            if (temp_money < money)
            {
                return getLoginJsonMessage(0, "所选中订单可冲抵金额为:" + temp_money);
            }

            var _is_all_payed = true;
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                for (var i = 0; i < orderList.Count; i++)
                {
                    for (var j = 0; j < out_list.Count; j++)
                    {
                        if (out_list[j].order_id == orderList[i].id)
                        {
                            decimal out_plan_wait_pay = out_list[j].out_plan_money - out_list[j].payed_money;
                            //销售单余额冲抵
                            if (out_plan_wait_pay > 0 && money > 0)
                            {
                                if (orderList[i].balance_money > 0)
                                {
                                    calcMoney(ref money, ref out_plan_wait_pay);
                                    if (orderList[i].balance_money >= out_plan_wait_pay)
                                    {
                                        money -= out_plan_wait_pay;
                                        orderList[i].balance_money -= out_plan_wait_pay;
                                        orderList[i].payed_money += out_plan_wait_pay;
                                        orderList[i].wating_pay -= out_plan_wait_pay;
                                        out_list[j].payed_money += out_plan_wait_pay;
                                        out_plan_wait_pay = 0;
                                    }
                                    else
                                    {
                                        money -= orderList[i].balance_money;
                                        out_plan_wait_pay -= orderList[i].balance_money;
                                        out_list[j].payed_money += orderList[i].balance_money;
                                        orderList[i].payed_money += orderList[i].balance_money;
                                        orderList[i].wating_pay -= orderList[i].balance_money;
                                        orderList[i].balance_money = 0;
                                    }
                                }
                            }
                            //锁定余额冲抵
                            if (out_plan_wait_pay > 0 && money > 0)
                            {
                                if (shop.lock_money > 0)
                                {
                                    calcMoney(ref money, ref out_plan_wait_pay);
                                    if (shop.lock_money >= out_plan_wait_pay)
                                    {
                                        money -= out_plan_wait_pay;
                                        shop.lock_money -= out_plan_wait_pay;
                                        orderList[i].payed_money += out_plan_wait_pay;
                                        orderList[i].wating_pay -= out_plan_wait_pay;
                                        out_list[j].payed_money += out_plan_wait_pay;
                                        out_plan_wait_pay = 0;
                                    }
                                    else
                                    {
                                        money -= shop.lock_money;
                                        out_plan_wait_pay -= shop.lock_money;
                                        out_list[j].payed_money += shop.lock_money;
                                        orderList[i].payed_money += shop.lock_money;
                                        orderList[i].wating_pay -= shop.lock_money;
                                        shop.lock_money = 0;
                                    }
                                }
                            }
                            //可用余额冲抵
                            if (out_plan_wait_pay > 0)
                            {
                                if (shop.balance_money > 0)
                                {
                                    calcMoney(ref money, ref out_plan_wait_pay);
                                    if (shop.balance_money >= out_plan_wait_pay)
                                    {
                                        money -= out_plan_wait_pay;
                                        shop.balance_money -= out_plan_wait_pay;
                                        orderList[i].payed_money += out_plan_wait_pay;
                                        orderList[i].wating_pay -= out_plan_wait_pay;
                                        out_list[j].payed_money += out_plan_wait_pay;
                                        out_plan_wait_pay = 0;
                                    }
                                    else
                                    {
                                        money -= shop.balance_money;
                                        out_plan_wait_pay -= shop.balance_money;
                                        out_list[j].payed_money += shop.balance_money;
                                        orderList[i].payed_money += shop.balance_money;
                                        orderList[i].wating_pay -= shop.balance_money;
                                        shop.balance_money = 0;
                                    }
                                }
                            }
                            if (out_plan_wait_pay > 0)
                            {
                                _is_all_payed = false;
                            }
                        }
                    }
                }
                shop_ctrl.SaveChanges();
                order_ctrl.SaveChanges();
                out_ctrl.SaveChanges();
                transaction.Complete();
            }
            //if (_is_all_payed)
            //{
            return getLoginJsonMessage(1, "成功");
            //}
            //else {
            //    return getLoginJsonMessage(1, "该客户帐户余额不足，有订单未冲抵完成");
            //}
        }
        /// <summary>
        /// 客户账户
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerAcount()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 客户账户
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomerAcount()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int type = Convert.ToInt32(Request.QueryString["type"]);
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
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            List<YNShopInfo> shopInfoList = shopInfoController.getShopInfoListByFactoryId(clothFactoryId, key, type, pageIndex, pageSize);
            int count = shopInfoController.getShopInfoListByFactoryIdCount(clothFactoryId, key, type);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.shopInfoList = shopInfoList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 工资统计
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
        /// <summary>
        /// 工资统计
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSalayStatistics()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 出库审核
        /// </summary>
        /// <returns></returns>
        public ActionResult StoreOutCheck()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// 出库审核列表
        public ActionResult GetStoreOutCheck()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            string out_date = Request.QueryString["outDate"];
            string search_key = Request.QueryString["searchKey"];
            API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
            List<YNStorageOut> out_list = out_ctrl.GetWaitExamineList(factory_id, out_date, search_key);

            ViewBag.out_list = out_list;
            return View();
        }

        public CreateSaleMessage ExamineOutPlan(int factory_id, int plan_id, int examine_state, FactoryStorageOutController out_ctrl, string examine_remark, ProductController product_ctrl, FactoryOrderController order_ctrl, ShopInfoController shop_ctrl)
        {
            CreateSaleMessage msg = new CreateSaleMessage();
            msg.IsSuccess = false;
            YNStorageOut out_plan = out_ctrl.GetOutPlanById(factory_id, plan_id);
            if (out_plan == null)
            {
                msg.Message = "该出库单数据错误";
                return msg;
            }
            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, out_plan.product_id);
            if (product == null)
            {
                msg.Message = "该出库单产品数据不存在";
                return msg;
            }
            if (CheckProductInventory(product.id))
            {
                msg.Message = "该产品正在等待盘点确认，不能进行审核";
                return msg;
            }
            //审核通过
            if (examine_state == 1)
            {
                out_plan.examine_state = API.Controllers.FactoryStorageOutController.examine_state_1;
            }
            else
            {//审核不通过
                out_plan.examine_state = API.Controllers.FactoryStorageOutController.examine_state_2;

                ///库存数据回滚
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
                for (var i = 0; i < package_list.Count; i++)
                {
                    if (package_list[i].relation_id == 0)
                    {
                        package_list[i].number += out_plan.out_num;
                        package_list[i].lockNumber += out_plan.out_lock_num;
                    }
                }
                product.package_info = js.Serialize(package_list);
                product.total_avaible_num += out_plan.out_num;
                product.toal_lock_num += out_plan.out_lock_num;
                product_ctrl.SaveChanges();

                ///销售单数据回滚
                YNFactoryOrder order = order_ctrl.FactoryGetFactoryOrderById(out_plan.order_id, factory_id);
                order.out_lock_number -= (out_plan.out_lock_num + out_plan.out_num);
                order.product_lock_number += out_plan.out_lock_num;
                order.balance_money += out_plan.sale_payed;
                out_plan.sale_payed = 0;
                order_ctrl.SaveChanges();

                ///客户帐户数据回滚
                YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(order.shop_id, factory_id);
                if (shop == null)
                {
                    msg.Message = "未查询到该出库单的客户";
                    return msg;
                }
                shop.lock_money += out_plan.lock_payed;
                shop.balance_money += out_plan.balance_payed;
                shop_ctrl.SaveChanges();

                ///出库单数据回滚
                out_plan.sale_payed = 0;
                out_plan.lock_payed = 0;
                out_plan.balance_payed = 0;
                out_plan.payed_money = 0;
            }
            out_plan.examin_remark = examine_remark;
            out_plan.modify_date = DateTime.Now;
            out_ctrl.SaveChanges();
            msg.Message = "审核成功";
            msg.IsSuccess = true;
            return msg;
        }
        //出库审核获取客户信息
        public JsonResult GetOutExamineCustomerInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["id"]);
            ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(customer_id, factory_id);
            Object result = new { 
                shop_name = shop.shop_name,
                jie_suan = shop.jiesuan,
                balance_money = shop.balance_money,
                credit = shop.credit_limit - shop.credit_lock
            };
            return Json(new { code = 1, message = "获取成功", data = result }, JsonRequestBehavior.AllowGet);
        }
        //出库审核操作
        public JsonResult ExaminePlanOut()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int plan_id = Convert.ToInt32(Request.QueryString["planId"]);
            int examine_state = Convert.ToInt32(Request.QueryString["examineState"]);
            string examine_remark = Request.QueryString["remark"];
            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
                API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
                API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
                API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();

                CreateSaleMessage msg = ExamineOutPlan(factory_id, plan_id, examine_state, out_ctrl, examine_remark, product_ctrl, order_ctrl, shop_ctrl);
                if (msg.IsSuccess == false)
                {
                    return getLoginJsonMessage(0, msg.Message);
                }
                List<YNStorageOut> sub_plan_out_list = out_ctrl.GetRelationOutPlan(factory_id, plan_id);
                for (var i = 0; i < sub_plan_out_list.Count; i++)
                {
                    CreateSaleMessage sub_msg = ExamineOutPlan(factory_id, sub_plan_out_list[i].id, examine_state, out_ctrl, examine_remark, product_ctrl, order_ctrl, shop_ctrl);
                    if (sub_msg.IsSuccess == false)
                    {
                        return getLoginJsonMessage(0, sub_msg.Message);
                    }
                }
                transaction.Complete();
            }
            return getLoginJsonMessage(1, "审核完成");
        }
        /// <summary>
        /// 工资详情
        /// </summary>
        /// <returns></returns>
        public ActionResult SalayDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 工资详情打印
        /// </summary>
        /// <returns></returns>
        public ActionResult SalayPrint()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
      
    }
}