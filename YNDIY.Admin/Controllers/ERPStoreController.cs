using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using System.Transactions;
using YNDIY.API.Controllers;

namespace YNDIY.Admin.Controllers
{
    public class ERPStoreController : ParentController
    {
        /// 生产订单入库
        public ActionResult ProductStoreIn()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// 生产订单入库列表
        public ActionResult GetProductStoreIn()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int pageIndex = 1;
            int pageSize = 10;
            if (!String.IsNullOrEmpty(Request.QueryString["pageIndex"])) {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            int storeState = Convert.ToInt32(Request.QueryString["store_state"]);
            int brandId = Convert.ToInt32(Request.QueryString["brandId"]);
            string searchKey = Request.QueryString["searchKey"];
            API.Controllers.ProductionOrderController order_ctrl = new API.Controllers.ProductionOrderController();
            List<YNBanShiProductionOrder> order_list = order_ctrl.GetToStorageInList(clothFactoryId, storeState, brandId, searchKey, pageIndex, pageSize);
            int count = order_ctrl.GetToStorageInCount(clothFactoryId, storeState, brandId, searchKey);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.order_list = order_list;
            ViewBag.page = page;
            return View();
        }
        //自动设置销售单锁定数量
        /// <summary>
        /// 自动设置销售单锁定数量
        /// </summary>
        /// <param name="product_package">产品包信息</param>
        /// <param name="factory_id">工厂ID</param>
        /// <param name="product_id">产品ID</param>
        /// <param name="sale_order_ctrl">销售单控制器</param>
        public void AutoSetSaleOrderLockNumber(ref List<ProductPackage> product_package, int factory_id, int product_id, FactoryOrderController sale_order_ctrl)
        {
            //可用最小数量
            var avaible_min = getListProductPackageMinData(product_package, false);

            //查询出所有锁定该产品的销售单(优先将锁定库存的数量补充完)
            List<YNFactoryOrder> waiting_in_list = sale_order_ctrl.GetNotLockFullProductList(factory_id, product_id);
            //从正常数量 划拨数据到 锁定数量
            for (var i = 0; i < waiting_in_list.Count; i++)
            {
                if (avaible_min == 0)
                {
                    break;
                }
                int wait_lock_num = waiting_in_list[i].product_number - waiting_in_list[i].product_out_number - waiting_in_list[i].product_lock_number - waiting_in_list[i].out_lock_number;
                if (wait_lock_num >= avaible_min)
                {
                    for (var j = 0; j < product_package.Count; j++)
                    {
                        product_package[j].number -= avaible_min;
                        product_package[j].lockNumber += avaible_min;
                    }
                    waiting_in_list[i].product_lock_number += avaible_min;
                    avaible_min = 0;
                }
                else
                {
                    for (var j = 0; j < product_package.Count; j++)
                    {
                        product_package[j].number -= wait_lock_num;
                        product_package[j].lockNumber += wait_lock_num;
                    }
                    waiting_in_list[i].product_lock_number += wait_lock_num;
                    avaible_min -= wait_lock_num;
                }
            }
            sale_order_ctrl.SaveChanges();
        }

        //生产订单入库 接口
        public JsonResult SetStorageIn()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int orderId = Convert.ToInt32(Request.Form["orderId"]);
            string data = Request.Form["data"];
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<ProductPackage> request_package_list = new List<ProductPackage>();
            request_package_list = js.Deserialize<List<ProductPackage>>(data);
            if (request_package_list.Count == 0)
            {
                return getLoginJsonMessage(0, "参数错误");
            }

            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                API.Controllers.ProductionOrderController order_ctrl = new API.Controllers.ProductionOrderController();
                YNBanShiProductionOrder order = order_ctrl.GetProductionOrderById(clothFactoryId, orderId);
                API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
                YNBanShiProduct product = product_ctrl.GetProductById(clothFactoryId, order.product_id);
                if (order == null)
                {
                    return getLoginJsonMessage(0, "该生产订单不存在");
                }
                if (CheckProductInventory(product.id))
                {
                    return getLoginJsonMessage(0, "该产品正在等待盘点确认，不能执行入库操作");
                }
                List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(order.package_info);
                for (var i = 0; i < request_package_list.Count; i++)
                {
                    for (var j = 0; j < package_list.Count; j++)
                    {
                        if (request_package_list[i].packageNumber == package_list[j].packageNumber)
                        {
                            package_list[j].number += request_package_list[i].number;
                            if (package_list[j].number > order.product_number)
                            {
                                return getLoginJsonMessage(0, "包" + package_list[j].packageNumber + "数据错误");
                            }
                            break;
                        }
                    }
                }
                order.package_info = js.Serialize(package_list);
                bool is_state_1 = false;//是否部分入库
                bool is_state_2 = true;//是否全部入库
                for (var i = 0; i < package_list.Count; i++)
                {
                    if (package_list[i].number != order.product_number)
                    {
                        is_state_2 = false;
                    }
                    if (package_list[i].number != 0)
                    {
                        is_state_1 = true;
                    }
                }
                if (is_state_1)
                {
                    order.storage_status = 1;
                    order.state = API.Controllers.ProductionOrderController.state_2;
                }
                if (is_state_2)
                {
                    order.storage_status = 2;
                    order.state = API.Controllers.ProductionOrderController.state_3;
                }
                if (!String.IsNullOrEmpty(product.package_info))
                {
                    List<ProductPackage> product_package = js.Deserialize<List<ProductPackage>>(product.package_info);
                    //入库前最小数量
                    var min_number = getListProductPackageMinData(product_package);
                    API.Controllers.FactoryOrderController sale_order_ctrl = new API.Controllers.FactoryOrderController();
                    //全部库存录入到正常数据
                    for (var i = 0; i < product_package.Count; i++)
                    {
                        for (var j = 0; j < request_package_list.Count; j++)
                        {
                            if (request_package_list[j].packageNumber == product_package[i].packageNumber)
                            {
                                product_package[i].number += request_package_list[j].number;
                                break;
                            }
                        }
                    }
                    //入库后最小数量
                    var min_number_2 = getListProductPackageMinData(product_package);
                    //入库整套数量
                    var _in_count = min_number_2 - min_number;
                    //减少生产中数量
                    product.total_producing_num = product.total_producing_num > _in_count ? product.total_producing_num - _in_count : 0;
                    //增加总入库数量
                    product.total_in += _in_count;

                    //销售单自动锁定库存
                    AutoSetSaleOrderLockNumber(ref product_package, clothFactoryId, product.id, sale_order_ctrl);


                    //设定总可用数量
                    product.total_avaible_num = getListProductPackageMinData(product_package, false);
                    for (var i = 0; i < product_package.Count; i++)
                    {
                        if (product_package[i].relation_id == 0)
                        {
                            //设定总锁定数量
                            product.toal_lock_num = product_package[i].lockNumber;
                            break;
                        }
                    }
                    product.package_info = js.Serialize(product_package);
                    product_ctrl.SaveChanges();
                    sale_order_ctrl.SaveChanges();
                }
                else {
                    return getLoginJsonMessage(0, "该产品还未添加包信息");
                }
                order_ctrl.SaveChanges();
                transaction.Complete();
            }
            return getLoginJsonMessage(1, "入库成功");
        }

        /// 客户订单出库页面
        public ActionResult ProductStoreOut()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        public CreateSaleMessage SetPlanOut(API.Controllers.FactoryStorageOutController out_ctrl, API.Controllers.FactoryOrderController sale_order_ctrl, int factory_id,int plan_id)
        {
            CreateSaleMessage msg = new CreateSaleMessage();
            msg.IsSuccess = false;
            YNStorageOut out_plan = out_ctrl.GetWaitOutPlanById(factory_id, plan_id);
            if (out_plan == null)
            {
                msg.Message = "抱歉没有查询到该出库单";
                return msg;
            }
            ProductController product_ctrl = new ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, out_plan.product_id);
            if (product == null)
            {
                msg.Message = "没有查询到该出库单的产品信息";
                return msg;
            }
            if (CheckProductInventory(product.id))
            {
                msg.Message = product.model_name + "正在等待产品盘点确认，不能进行出库操作";
                return msg;
            }
            YNFactoryOrder sale_order = sale_order_ctrl.FactoryGetFactoryOrderById(out_plan.order_id, factory_id);
            if (sale_order == null)
            {
                msg.Message = "抱歉没有查询到该出库单发起的销售单";
                return msg;
            }
            //出库单出库 修改数据
            out_plan.examine_state = API.Controllers.FactoryStorageOutController.examine_state_3;//设置出库状态为已经出库
            decimal already_pay = (out_plan.sale_payed + out_plan.lock_payed + out_plan.balance_payed);
            out_plan.payed_money += already_pay;//设置出库单已支付金额
            out_plan.sale_payed = 0;
            out_plan.lock_payed = 0;
            out_plan.balance_payed = 0;
            out_plan.plan_out_date = DateTime.Now;
            out_plan.modify_date = DateTime.Now;


            //销售单出库 修改数据
            sale_order.wating_pay += out_plan.out_plan_money - out_plan.payed_money;//销售单欠款累加
            sale_order.payed_money += already_pay;
            //sale_order.product_lock_number -= out_plan.out_lock_num;
            sale_order.product_out_number += (out_plan.out_lock_num + out_plan.out_num);
            sale_order.out_lock_number -= (out_plan.out_lock_num + out_plan.out_num);

            sale_order_ctrl.SaveChanges();
            out_ctrl.SaveChanges();

            msg.IsSuccess = true;
            msg.Message = "出库成功";
            return msg;
        }
        /// 计划出库单 出库操作（接口）
        public JsonResult SetProductStoreOut()
        {
            if (!checkSession())
            {
                return getDataJsonMessage(0, "请登录");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int plan_id = Convert.ToInt32(Request.QueryString["id"]);//出库单ID

            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {

                API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
                //YNStorageOut out_plan = out_ctrl.GetWaitOutPlanById(factory_id, plan_id);
                //if (out_plan == null)
                //{
                //    return getDataJsonMessage(0, "抱歉没有查询到该出库单");
                //}
                API.Controllers.FactoryOrderController sale_order_ctrl = new API.Controllers.FactoryOrderController();
                //YNFactoryOrder sale_order = sale_order_ctrl.FactoryGetFactoryOrderById(out_plan.order_id, factory_id);
                //if (sale_order == null)
                //{
                //    return getDataJsonMessage(0, "抱歉没有查询到该出库单发起的销售单");
                //}
                CreateSaleMessage msg = SetPlanOut(out_ctrl, sale_order_ctrl, factory_id, plan_id);
                if (!msg.IsSuccess)
                {
                    return getLoginJsonMessage(0, msg.Message);
                }

                List<YNStorageOut> sub_plan_out_list = out_ctrl.GetRelationOutPlan(factory_id, plan_id);
                for (var i = 0; i < sub_plan_out_list.Count; i++)
                {
                    CreateSaleMessage sub_msg = SetPlanOut(out_ctrl, sale_order_ctrl, factory_id, sub_plan_out_list[i].id);
                    if (!msg.IsSuccess)
                    {
                        return getLoginJsonMessage(0, sub_msg.Message);
                    }
                }               

                transaction.Complete();
            }
            return getDataJsonMessage(1, "成功");
        }
        /// 客户订单出库
        public ActionResult GetProductStoreOut()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string out_date = Request.QueryString["outDate"];
            string search_key = Request.QueryString["searchKey"];
            API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
            List<YNStorageOut> out_list = out_ctrl.GetList(clothFactoryId, out_date, search_key);
            out_list = out_list.Where(w => w.plan_state == API.Controllers.FactoryStorageOutController.plan_state_0).ToList();
            ViewBag.out_list = out_list;
            return View();
        }
        ///产品库存
        public ActionResult ProductStore()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// 产品库存列表
        public ActionResult GetProductStore()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int brandId = Convert.ToInt32(Request.QueryString["brandId"]);
            string searchKey = Request.QueryString["searchKey"];
            int pageIndex = 1;
            int pageSize = 10;
            if (!String.IsNullOrEmpty(Request.QueryString["pageIndex"])) {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            List<YNBanShiProduct> product_list = product_ctrl.GetProductStoreByBrandAndSearchKey(clothFactoryId, brandId, searchKey, pageIndex, pageSize);
            API.Controllers.FactoryStorageOutController out_ctrl = new API.Controllers.FactoryStorageOutController();
            List<int> plan_lock = new List<int>();
            List<int> brands_id = new List<int>();
            for (var i = 0; i < product_list.Count; i++)
            {
                //查询未出库的计划数量
                List<YNStorageOut> out_plan_list = out_ctrl.GetNotOutPlanListByProduct(clothFactoryId, product_list[i].id);
                int temp_count = 0;
                for (var j = 0; j < out_plan_list.Count; j++)
                {
                    temp_count += out_plan_list[j].out_num;
                    temp_count += out_plan_list[j].out_lock_num;
                }
                plan_lock.Add(temp_count);
                brands_id.Add(product_list[i].brand_id);
            }
            API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> brand_list = brand_ctrl.GetBrandListInBrandIDList(clothFactoryId, brands_id);
            int count = product_ctrl.GetProductStoreByBrandAndSearchKeyCount(clothFactoryId, brandId, searchKey);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.page = page;
            ViewBag.product_list = product_list;
            ViewBag.plan_lock = plan_lock;
            ViewBag.brand_list = brand_list;
            ViewBag.product_ctrl = product_ctrl;
            ViewBag.factory_id = clothFactoryId;
            return View();
        }
         ///打印出库单
        public ActionResult PrintStoreOut()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// 库存调拨
        public ActionResult AllotOrderList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// 库存调拨关联
        public ActionResult AllotOrderDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        //查询包数据最小整套数
        /// <summary>
        /// 查询包数据最小整套数
        /// </summary>
        /// <param name="package_list">库存包数据</param>
        /// <param name="contains_lock">是否包含锁定数据 默认包含</param>
        /// <returns></returns>
        private int getListProductPackageMinData(List<ProductPackage> package_list, bool contains_lock = true)
        {
            int min = 999999999;
            bool has_no_my_package = true;//没有自己的包
            for (var i = 0; i < package_list.Count; i++)
            {
                if (package_list[i].relation_id == 0)
                {
                    has_no_my_package = false;
                    if (contains_lock)
                    {
                        if (min > (package_list[i].number + package_list[i].lockNumber))
                        {
                            min = (package_list[i].number + package_list[i].lockNumber);
                        }
                    }
                    else
                    {
                        if (min > package_list[i].number)
                        {
                            min = package_list[i].number;
                        }
                    }
                }
            }
            if (has_no_my_package)
            {
                return 0;
            }
            return min;
        }
        //重置盘点处理数据
        private void resetInventoryHandle(YNInventory entity, string package_number, InventoryController inventory_ctrl)
        {
            if (entity != null)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductPackage> inventory_package = js.Deserialize<List<ProductPackage>>(entity.package_info);
                for (var i = 0; i < inventory_package.Count; i++)
                {
                    if (inventory_package[i].packageNumber == package_number)
                    {
                        inventory_package[i].number = 0;
                        break;
                    }
                }
                bool is_all_package_number_empty = true;
                for (var i = 0; i < inventory_package.Count; i++)
                {
                    if (inventory_package[i].relation_id == 0)
                    {
                        if (inventory_package[i].number != 0)
                        {
                            is_all_package_number_empty = false;
                        }
                    }
                }
                if (is_all_package_number_empty)
                {
                    entity.delete_state = InventoryController.delete_status_1;
                }
                else
                {
                    entity.package_info = js.Serialize(inventory_package);
                }
                inventory_ctrl.SaveChanges();
            }
        }
        /// 发起盘点
        public JsonResult SetInventory()
        {

            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int product_id = Convert.ToInt32(Request.QueryString["productId"]);
            string package_number = Request.QueryString["packageNumber"];
            int inventory_count = Convert.ToInt32(Request.QueryString["inventoryCount"]);


            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                ProductController product_ctrl = new ProductController();
                YNBanShiProduct product = product_ctrl.GetProductById(factory_id, product_id);
                if (product == null)
                {
                    return getLoginJsonMessage(0, "盘点产品不存在");
                }
                if (String.IsNullOrEmpty(product.package_info))
                {
                    return getLoginJsonMessage(0, "该产品未编辑包信息");
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
                int package_count = 0;
                bool is_contains_package_number = false;
                for (var i = 0; i < package_list.Count; i++)
                {
                    if (package_list[i].relation_id == 0)
                    {
                        if (package_list[i].packageNumber == package_number)
                        {
                            is_contains_package_number = true;
                            package_count = package_list[i].number;
                            break;
                        }
                    }
                }
                if (!is_contains_package_number)
                {
                    return getLoginJsonMessage(0, "该产品不包含该包信息");
                }
                //查询该产品的出库锁定数量
                int not_out_count = 0;
                FactoryStorageOutController out_ctrl = new FactoryStorageOutController();
                List<YNStorageOut> out_list = out_ctrl.GetNotOutPlanListByProduct(factory_id, product.id);
                if (out_list.Count > 0)
                {
                    for (var i = 0; i < out_list.Count; i++)
                    { 
                        not_out_count += (out_list[i].out_num + out_list[i].out_lock_num);
                    }
                }
                int product_all_count = package_count + product.toal_lock_num + not_out_count;
                if (product_all_count == inventory_count)
                {
                    return getLoginJsonMessage(0, "盘点后与原来数据相同");
                }
                //执行盘点后的数据  
                InventoryController inventory_ctrl = new InventoryController();
                YNInventory entity = inventory_ctrl.GetInventoryByProductId(factory_id, product_id);

                // 库存被盘大
                if (inventory_count > product_all_count)
                {
                    resetInventoryHandle(entity, package_number, inventory_ctrl);
                    int difference_count = inventory_count - product_all_count;
                    for (var i = 0; i < package_list.Count; i++)
                    {
                        if (package_list[i].packageNumber == package_number)
                        {
                            package_list[i].number += difference_count;
                        }
                    }

                    /*********** 自动补充锁定销售单 *********/
                    FactoryOrderController sale_order_ctrl = new FactoryOrderController();
                    //销售单自动锁定库存
                    AutoSetSaleOrderLockNumber(ref package_list, factory_id, product.id, sale_order_ctrl);
                    //设定总可用数量
                    product.total_avaible_num = getListProductPackageMinData(package_list, false);
                    for (var i = 0; i < package_list.Count; i++)
                    {
                        if (package_list[i].relation_id == 0)
                        {
                            //设定总锁定数量
                            product.toal_lock_num = package_list[i].lockNumber;
                            break;
                        }
                    }
                    //设定产品包数据信息
                    product.package_info = js.Serialize(package_list);
                }
                // 库存被盘小
                if (inventory_count < product_all_count)
                {
                    int difference_count = product_all_count - inventory_count;
                    //如果可用包可以直接盘小
                    if (package_count >= difference_count)
                    {
                        for (var i = 0; i < package_list.Count; i++)
                        {
                            if (package_list[i].packageNumber == package_number)
                            {
                                package_list[i].number -= difference_count;
                            }
                        }
                        product.total_avaible_num = getListProductPackageMinData(package_list, false);
                        product.package_info = js.Serialize(package_list);
                        resetInventoryHandle(entity, package_number, inventory_ctrl);
                    }
                    //如果需要从 销售单\出库单 与 可用包 同时盘小
                    else
                    {
                        if (entity == null)
                        {
                            entity = new YNInventory();
                            entity.factory_id = factory_id;
                            entity.brand_id = product.brand_id;
                            entity.product_id = product.id;
                            entity.product_name = product.produce_name;
                            entity.proudct_model = product.model_name;
                            entity.product_format = product.format;
                            entity.product_color = product.color;
                            List<ProductPackage> inventory_package = package_list;
                            for (var i = 0; i < inventory_package.Count; i++)
                            {
                                if (inventory_package[i].relation_id == 0)
                                {
                                    inventory_package[i].number = 0;
                                    inventory_package[i].lockNumber = 0;
                                    if (inventory_package[i].packageNumber == package_number)
                                    {
                                        inventory_package[i].number = difference_count;
                                    }
                                }
                            }
                            entity.package_info = js.Serialize(inventory_package);
                            entity.handle_state = InventoryController.handle_status_0;
                            entity.delete_state = InventoryController.delete_status_0;
                            entity.create_time = DateTime.Now;
                            entity.modify_time = entity.create_time;
                            inventory_ctrl.Create(entity);
                        }
                        else
                        {
                            List<ProductPackage> inventory_package = js.Deserialize<List<ProductPackage>>(entity.package_info);
                            for (var i = 0; i < inventory_package.Count; i++)
                            {
                                if (inventory_package[i].packageNumber == package_number)
                                {
                                    inventory_package[i].number = difference_count;
                                    break;
                                }
                            }
                            entity.package_info = js.Serialize(inventory_package);
                            inventory_ctrl.SaveChanges();
                        }

                    }
                }
                product_ctrl.SaveChanges();
                transaction.Complete();
            }
            return getLoginJsonMessage(1, "发起盘点成功");
        }
        /// 盘点调拨
        public ActionResult Inventory()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// 
        public ActionResult GetAllotOrderList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        //计算盘点后库存
        public int computeAfterInventoryStoreCount(List<ProductPackage> package_list, List<ProductPackage> inventory_package,int plan_not_out_count)
        {
            for (var i = 0; i < package_list.Count; i++)
            {
                for (var j = 0; j < inventory_package.Count; j++)
                {
                    if (inventory_package[j].relation_id == 0)
                    {
                        if (inventory_package[j].packageNumber == package_list[i].packageNumber)
                        {
                            if (inventory_package[j].number > 0 && package_list[i].number > 0)
                            {
                                if (package_list[i].number >= inventory_package[j].number)
                                {
                                    package_list[j].number -= inventory_package[j].number;
                                    inventory_package[j].number = 0;
                                }
                                else
                                {
                                    inventory_package[j].number -= package_list[i].number;
                                    package_list[i].number = 0;
                                }
                            }
                            if (inventory_package[j].number > 0 && package_list[i].lockNumber > 0)
                            {
                                if (package_list[i].lockNumber >= inventory_package[j].number)
                                {
                                    package_list[i].lockNumber -= inventory_package[j].number;
                                    inventory_package[j].number = 0;
                                }
                                else
                                {
                                    inventory_package[j].number -= package_list[i].lockNumber;
                                    package_list[i].lockNumber = 0;
                                }
                            }
                            if (inventory_package[j].number > 0 && plan_not_out_count > 0)
                            {
                                if (plan_not_out_count >= inventory_package[j].number)
                                {
                                    plan_not_out_count -= inventory_package[j].number;
                                }
                                else
                                {
                                    plan_not_out_count = 0;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return getListProductPackageMinData(package_list) + plan_not_out_count;
        }

        /// 盘点调拨列表
        public ActionResult GetInventoryList()
        {
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            InventoryController inventory_ctrl = new InventoryController();
            List<YNInventory> inventory_list = inventory_ctrl.GetInventoryList(factory_id);
            List<int> product_id_list = new List<int>();
            for (var i = 0; i < inventory_list.Count; i++)
            {
                product_id_list.Add(inventory_list[i].product_id);
            }
            ProductController product_ctrl = new ProductController();
            List<YNBanShiProduct> product_list = product_ctrl.GetProductListInProductIdList(factory_id, product_id_list);
            List<int> brand_id_list = new List<int>();
            for (var i = 0; i < product_list.Count; i++)
            {
                brand_id_list.Add(product_list[i].brand_id);
            }
            FactoryBrandController brand_ctrl = new FactoryBrandController();
            List<YNBanShiBrand> brand_list = brand_ctrl.GetBrandListInBrandIDList(factory_id, brand_id_list);


            ViewBag.inventory_list = inventory_list;
            ViewBag.product_list = product_list;
            ViewBag.brand_list = brand_list;
            ViewBag.myPage = this;
            return View();
        }
        /// 盘点调拨详情
        public ActionResult InventoryrDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int inventory_id = Convert.ToInt32(Request.QueryString["id"]);
            InventoryController inventory_ctrl = new InventoryController();
            YNInventory inventory = inventory_ctrl.GetInventoryById(factory_id, inventory_id);
            if (inventory == null)
            {
                return getLoginJsonMessage(0, "未查询到相关数据");
            }
            ProductController product_ctrl = new ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, inventory.product_id);
            if (product == null)
            {
                return getLoginJsonMessage(0, "未查询到产品数据");
            }
            FactoryBrandController brand_ctrl = new FactoryBrandController();
            YNBanShiBrand brand = brand_ctrl.GetBrandById(product.brand_id, factory_id);
            if (brand == null)
            {
                return getLoginJsonMessage(0, "未查询到品牌信息");
            }
            FactoryStorageOutController out_ctrl = new FactoryStorageOutController();
            List<YNStorageOut> not_out_list = out_ctrl.GetNotOutPlanListByProduct(factory_id, product.id);
            int not_out_cout = 0;
            for (var i = 0; i < not_out_list.Count; i++)
            {
                not_out_cout += not_out_list[i].out_num;
                not_out_cout += not_out_list[i].out_lock_num;
            }
            FactoryOrderController sale_order_ctrl = new FactoryOrderController();
            List<YNFactoryOrder> sale_order_list = sale_order_ctrl.GetHasLockNumberList(factory_id, product.id);
            ViewBag.brand = brand;
            ViewBag.product = product;
            ViewBag.inventory = inventory;
            ViewBag.not_out_list = not_out_list;
            ViewBag.not_out_count = not_out_cout;
            ViewBag.sale_order_list = sale_order_list;
            ViewBag.myPage = this;
            return View();
        }
        /// 商务确认盘点调拨
        public JsonResult HandleInventory()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int product_id = Convert.ToInt32(Request.QueryString["product_id"]);

            string sale_id_str = Request.QueryString["sale_id_str"];
            string sale_count_str = Request.QueryString["sale_count_str"];

            string out_id_str = Request.QueryString["outIds"];
            string out_count_str = Request.QueryString["out_count_str"];

            string[] sale_id_arr = sale_id_str.Split(",");
            string[] sale_count_arr = sale_count_str.Split(",");

            string[] out_id_arr = out_id_str.Split(",");
            string[] out_count_arr = out_count_str.Split(",");

            if (sale_id_arr.Length != sale_count_arr.Length || out_id_arr.Length != out_count_arr.Length)
            {
                return getLoginJsonMessage(0, "参数个数不匹配");
            }

            List<int> sale_id_list = new List<int>();
            for (var i = 0; i < sale_id_arr.Length; i++)
            {
                sale_id_list.Add(Convert.ToInt32(sale_id_arr[i]));
            }
            List<int> sale_count_list = new List<int>();
            for (var i = 0; i < sale_count_arr.Length; i++)
            {
                sale_count_list.Add(Convert.ToInt32(sale_count_arr[i]));
            }

            List<int> out_id_list = new List<int>();
            for (var i = 0; i < out_id_arr.Length; i++)
            {
                out_id_list.Add(Convert.ToInt32(out_id_arr[i]));
            }
            List<int> out_count_list = new List<int>();
            for (var i = 0; i < out_count_arr.Length; i++)
            {
                out_count_list.Add(Convert.ToInt32(out_count_arr[i]));
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
              //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                ProductController product_ctrl = new ProductController();
                YNBanShiProduct product = product_ctrl.GetProductById(factory_id, product_id);
                if (product == null)
                {
                    return getLoginJsonMessage(0, "产品信息错误");
                }
                InventoryController inventory_ctrl = new InventoryController();
                YNInventory inventory = inventory_ctrl.GetInventoryByProductId(factory_id, product.id);
                if (inventory == null)
                {
                    return getLoginJsonMessage(0, "该产品不需要盘点调拨数据");
                }
                FactoryOrderController sale_order_ctrl = new FactoryOrderController();
                List<YNFactoryOrder> sale_order_list = sale_order_ctrl.GetListInIdList(sale_id_list, factory_id).Where(w => w.product_lock_number > 0).ToList();
                if (sale_order_list.Count != sale_id_list.Count)
                {
                    return getLoginJsonMessage(0, "参数销售单个数与调拨数量个数不匹配");
                }
                FactoryStorageOutController out_ctrl = new FactoryStorageOutController();
                List<YNStorageOut> out_list = out_ctrl.GetNotOutListInIdList(out_id_list, factory_id);
                if (out_list.Count != out_count_list.Count)
                {
                    return getLoginJsonMessage(0, "参数出库单个数与调拨数量个数不匹配");
                }

                List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
                List<ProductPackage> inventory_package = js.Deserialize<List<ProductPackage>>(inventory.package_info);

                List<YNStorageOut> not_out_list = out_ctrl.GetNotOutPlanListByProduct(factory_id, product.id);
                //计划单未出库数量
                int not_out_count = 0;
                for (var i = 0; i < not_out_list.Count; i++)
                {
                    not_out_count += not_out_list[i].out_num;
                    not_out_count += not_out_list[i].out_lock_num;
                }
                //库存需要减少的数量
                int store_minus_count = 0;
                for (var i = 0; i < inventory_package.Count; i++)
                {
                    if (inventory_package[i].relation_id == 0)
                    {
                        if (store_minus_count < inventory_package[i].number)
                        {
                            store_minus_count = inventory_package[i].number;
                        }
                    }
                }
                //盘点前库存数量
                int store_count = getListProductPackageMinData(package_list) + not_out_count;
                //盘点后库存数量
                int after_inventory_count = computeAfterInventoryStoreCount(package_list, inventory_package, not_out_count);
                //需要总调拨的数量
                int all_inventory = store_count - after_inventory_count;
                int stock_avaible = product.total_avaible_num;
                //需要处理的调拨数量
                int handle_count = all_inventory - stock_avaible;
                //发起调拨的数量
                int set_inventory_count = 0;
                for (var i = 0; i < sale_count_list.Count; i++)
                {
                    set_inventory_count += sale_count_list[i];
                }
                for (var i = 0; i < out_count_list.Count; i++)
                {
                    set_inventory_count += out_count_list[i];
                }

                ////库存总可用销售单
                //List<YNFactoryOrder> all_has_lock_sale_list = sale_order_ctrl.getAvaibleSaleOrderList(factory_id, product.id);
                ////库存总可用出库单
                //List<YNStorageOut> all_has_count_out_list = out_ctrl.GetNotOutPlanListByProduct(factory_id, product.id);
                ////总共可盘点的库存
                //int all_inventory_count = 0;
                //for (var i = 0; i < all_has_lock_sale_list.Count; i++)
                //{
                //    all_inventory_count += all_has_lock_sale_list[i].product_lock_number;
                //}
                //for (var i = 0; i < all_has_count_out_list.Count; i++)
                //{
                //    all_inventory_count += all_has_count_out_list[i].out_lock_num;
                //    all_inventory_count += all_has_count_out_list[i].out_num;
                //}

                if (set_inventory_count > handle_count)
                {
                    return getLoginJsonMessage(0, "发起调拨的数量大于待处理的数量");
                }
                if (set_inventory_count < handle_count)
                {
                    return getLoginJsonMessage(0, "发起调拨的数量小于待处理的数量");
                }

                //如果可调拨的数据未使用完毕 并且 调拨的数据小于 未处理的数量
                //if (all_inventory_count > set_inventory_count && set_inventory_count < handle_count)
                //{
                //    return getLoginJsonMessage(0, "发起调拨的数量小于待处理的数量");
                //}
                //调拨销售单锁定数量
                for (var i = 0; i < sale_order_list.Count; i++)
                {
                    for (var j = 0; j < sale_id_list.Count; j++)
                    {
                        if (sale_order_list[i].id == sale_id_list[j])
                        {
                            if (sale_count_list[j] > sale_order_list[i].product_lock_number)
                            {
                                return getLoginJsonMessage(0, "销售单发起调拨数量大于销售单锁定数量");
                            }

                            sale_order_list[i].product_lock_number -= sale_count_list[j];//从销售单 锁定数量 减去 调拨出的数量

                            product = product_ctrl.GetProductById(factory_id, product_id);//重新获取产品信息
                            package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
                            package_list.ForEach(f => { f.number += sale_count_list[j]; f.lockNumber -= sale_count_list[j]; });
                            product.package_info = js.Serialize(package_list);
                            product.total_avaible_num = getListProductPackageMinData(package_list, false);
                            product.toal_lock_num = getListProductPackageMinData(package_list) - product.total_avaible_num;

                            sale_order_ctrl.SaveChanges();
                            product_ctrl.SaveChanges();
                            break;
                        }
                    }
                }
                //调拨出库单出库数量
                for (var i = 0; i < out_list.Count; i++)
                {
                    for (var j = 0; j < out_id_list.Count; i++)
                    {
                        if (out_list[i].id == out_id_list[j])
                        {
                            if (out_count_list[j] > (out_list[i].out_num + out_list[i].out_lock_num))
                            {
                                return getLoginJsonMessage(0, "出库单发起调拨数量大于出库单数量");
                            }
                            bool is_inventory = OutPlanMinusCountReturnMoney(factory_id, product_ctrl, sale_order_ctrl, out_ctrl, out_list[i], out_count_list[j]);
                            if (is_inventory == false)
                            {
                                return getLoginJsonMessage(0, "出库单库存划拨时，系统错误");
                            }
                            break;
                        }
                    }
                }
                product = product_ctrl.GetProductById(factory_id, product_id);
                package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
                inventory_package = js.Deserialize<List<ProductPackage>>(inventory.package_info);
                //从划拨后的库存减去盘点减少数量
                for (var i = 0; i < package_list.Count; i++)
                {
                    for (var j = 0; j < inventory_package.Count; j++)
                    {
                        if (inventory_package[j].relation_id == 0)
                        {
                            if (inventory_package[j].packageNumber == package_list[i].packageNumber)
                            {
                                package_list[i].number -= inventory_package[j].number;
                                break;
                            }
                        }
                    }
                }
                //重新计算package_list里面的最小值
                var min_package_count = 0;
                for (var i = 0; i < package_list.Count; i++)
                {
                    if (package_list[i].relation_id == 0)
                    {
                        if (min_package_count > package_list[i].number)
                        {
                            min_package_count = package_list[i].number;
                        }
                    }
                }
                //如果最小值小于0再重新分配一次数据
                if (min_package_count < 0)
                {
                    for (var i = 0; i < package_list.Count; i++)
                    {
                        if (package_list[i].relation_id == 0)
                        {
                            package_list[i].number -= min_package_count;
                            package_list[i].lockNumber += min_package_count;
                        }
                    }
                }


                product.package_info = js.Serialize(package_list);
                product.total_avaible_num = getListProductPackageMinData(package_list, false);
                product.toal_lock_num = getListProductPackageMinData(package_list) - product.total_avaible_num;
                
                inventory.delete_state = InventoryController.delete_status_1;
                inventory_ctrl.SaveChanges();
                product_ctrl.SaveChanges();

                transaction.Complete();
            }

            return getLoginJsonMessage(1, "调拨成功");
        }
        //从未出库的出库单里面划拨数量到库存
        public bool OutPlanMinusCountReturnMoney(int factory_id,ProductController product_ctrl,FactoryOrderController sale_order_ctrl,FactoryStorageOutController out_ctrl,YNStorageOut out_plan,int minus_count,int sub_out_plan_id = 0)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            //主出库单
            if (out_plan.plan_state == FactoryStorageOutController.plan_state_0)
            {
                //主出库单减少数量 需要 退还的金额
                YNFactoryOrder sale_order = sale_order_ctrl.FactoryGetFactoryOrderById(out_plan.order_id, factory_id);
                decimal truth_price = sale_order.product_price * (out_plan.out_lock_num + out_plan.out_num);

                if (minus_count == (out_plan.out_lock_num + out_plan.out_num))
                {
                    out_plan.delete_state = FactoryStorageOutController.delete_state_1;
                    if (out_plan.sale_payed > 0)
                    {
                        sale_order.balance_money += out_plan.sale_payed;
                    }
                }
                else
                {
                    decimal out_value = (out_plan.out_lock_num + out_plan.out_num - minus_count) * sale_order.product_price;
                    if (out_plan.sale_payed > out_value)
                    {
                        sale_order.balance_money += (out_plan.sale_payed - out_value);
                        out_plan.sale_payed -= (out_plan.sale_payed - out_value);
                    }
                }
                YNBanShiProduct product = product_ctrl.GetProductById(factory_id, sale_order.product_id);

                bool is_inventory = OutPlanClearOrReturnStore(sale_order, sale_order_ctrl, out_ctrl, product_ctrl, product, out_plan, factory_id, minus_count, out_plan.plan_state, out_plan.delete_state);
                if (!is_inventory)
                {
                    return false;
                }
                
                //子出库单 抹除数据 或者 返回数量到销售单及库存
                List<YNStorageOut> sub_out_list = out_ctrl.GetRelationOutPlan(factory_id, out_plan.id);
                if (sub_out_list.Count > 0)
                {
                    for (var i = 0; i < sub_out_list.Count; i++)
                    {
                        YNFactoryOrder sub_sale_order = sale_order_ctrl.FactoryGetFactoryOrderById(sub_out_list[i].order_id, factory_id);
                        if (sub_sale_order == null)
                        {
                            return false;
                        }
                        YNBanShiProduct sub_product = product_ctrl.GetProductById(factory_id, sub_sale_order.product_id);
                        if (sub_product == null)
                        {
                            return false;
                        }
                        is_inventory = OutPlanClearOrReturnStore(sub_sale_order, sale_order_ctrl, out_ctrl, product_ctrl, sub_product, sub_out_list[i], factory_id, minus_count, sub_out_list[i].plan_state, out_plan.delete_state, sub_out_plan_id);
                        if (!is_inventory)
                        {
                            return false;
                        }
                    }
                }


            }
            //子出库单
            else
            {
                YNStorageOut main_out_plan = out_ctrl.GetOutPlanById(factory_id, Convert.ToInt32(out_plan.relation_plan_id));
                if (main_out_plan == null)
                {
                    return false;
                }
                return OutPlanMinusCountReturnMoney(factory_id, product_ctrl, sale_order_ctrl, out_ctrl, main_out_plan, minus_count, out_plan.id);
            }

            sale_order_ctrl.SaveChanges();
            out_ctrl.SaveChanges();
            return true;
        }
        //未出库的出库单 退还库存
        public bool OutPlanClearOrReturnStore(YNFactoryOrder sale_order, FactoryOrderController sale_order_ctrl, FactoryStorageOutController out_ctrl, ProductController product_ctrl, YNBanShiProduct product, YNStorageOut exec_out_plan, int factory_id, int minus_count,int plan_state, int delete_state, int sub_out_plan_id = 0)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            int out_num_minus = 0;
            int out_lock_minus = 0;
            int temp_minus_count = minus_count;
            if (exec_out_plan.out_num > 0)
            {
                if (exec_out_plan.out_num >= temp_minus_count)
                {
                    out_num_minus = temp_minus_count;
                    exec_out_plan.out_num -= temp_minus_count;
                    temp_minus_count = 0;
                }
                else
                {
                    out_num_minus = exec_out_plan.out_num;
                    temp_minus_count -= exec_out_plan.out_num;
                    exec_out_plan.out_num = 0;
                }
            }
            if (temp_minus_count > 0)
            {
                out_lock_minus = temp_minus_count;
                exec_out_plan.out_lock_num -= temp_minus_count;
            }

            List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
            //出库单需要盘点减少数据
            if ((sub_out_plan_id == 0 && plan_state == FactoryStorageOutController.plan_state_0) || (sub_out_plan_id != 0 && sub_out_plan_id == exec_out_plan.id && plan_state == FactoryStorageOutController.plan_state_1))
            {
                package_list.ForEach(f => { if (f.relation_id == 0) { f.number += out_num_minus; f.lockNumber += out_lock_minus; } });
                product.package_info = js.Serialize(package_list);
                product.total_avaible_num = getListProductPackageMinData(package_list, false);
                product.toal_lock_num = getListProductPackageMinData(package_list) - product.total_avaible_num;
                
                sale_order.out_lock_number -= out_lock_minus;
                sale_order.product_lock_number += out_lock_minus;
            }
            //出库单不需要盘点减少数据
            else
            {
                package_list.ForEach(f => { if (f.relation_id == 0) { f.number += out_num_minus; f.lockNumber += out_lock_minus; } });
                //自动锁定库存到销售单
                AutoSetSaleOrderLockNumber(ref package_list, factory_id, product.id, sale_order_ctrl);

                product.package_info = js.Serialize(package_list);
                product.total_avaible_num = getListProductPackageMinData(package_list, false);
                product.toal_lock_num = getListProductPackageMinData(package_list) - product.total_avaible_num;

                sale_order.out_lock_number -= out_lock_minus;
                sale_order.product_lock_number += out_lock_minus;
            }

            if (minus_count == (exec_out_plan.out_lock_num + exec_out_plan.out_num))
            {
                exec_out_plan.delete_state = FactoryStorageOutController.delete_state_1;
            }
            product_ctrl.SaveChanges();
            sale_order_ctrl.SaveChanges();
            out_ctrl.SaveChanges();

            return true;
        }
    }
}
