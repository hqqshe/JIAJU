using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Controllers;
using YNDIY.API.Models;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Transactions;

namespace YNDIY.Admin.Controllers
{
    public class ERPProductController : ParentController
    {
        
        
        /// <summary>
        /// 生产预测信息列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductForecast()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
         /// <summary>
        /// 生产预测信息列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProductForecast()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int brand_id = -1;
            string product_name = Request.QueryString["productName"];
            int safeState = Convert.ToInt32(Request.QueryString["safeState"]);
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
            API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            List<YNBanShiProduct> productList = product_ctrl.GetProductList(clothFactoryId, brand_id, product_name, pageIndex, pageSize, safeState);
            API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> brandList = brand_ctrl.GetAllBrandList(clothFactoryId);
            List<BrandProduct> brandProduct = new List<BrandProduct>();
            for (var i = 0; i < productList.Count; i++)
            {
                BrandProduct product = new BrandProduct();
                for (var j = 0; j < brandList.Count; j++)
                {
                    if (productList[i].brand_id == brandList[j].id)
                    {
                        product.brand = brandList[j].name;
                        product.product = productList[i];
                        brandProduct.Add(product);
                        break;
                    }
                }
            }
            int count = product_ctrl.GetProductCount(clothFactoryId, brand_id, product_name, safeState);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.list = brandProduct;
            ViewBag.page = page;
            return View();
        }
        /// 生产计预测列表页面
        public ActionResult ForecastDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int product_id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController product_ctrl = new ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(clothFactoryId, product_id);
            API.Controllers.FactoryBrandController brand_ctrl = new FactoryBrandController();
            YNBanShiBrand brand = brand_ctrl.GetBrandById(product.brand_id, clothFactoryId);
            API.Controllers.ShopInfoController shop_ctrl = new ShopInfoController();
            List<YNShopInfo> system_shop = shop_ctrl.GetShopListByFactoryId(clothFactoryId, API.Controllers.ShopInfoController.type_1);
            List<YNShopInfo> temp_list = shop_ctrl.GetShopListByFactoryId(clothFactoryId);
            List<YNShopInfo> shop_list = new List<YNShopInfo>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            for (var i = 0; i < temp_list.Count; i++)
            {
                if (!String.IsNullOrEmpty(temp_list[i].brands_id))
                {
                    List<int> brand_list = js.Deserialize<List<int>>(temp_list[i].brands_id);
                    if (brand_list.Contains(brand.id))
                    {
                        shop_list.Add(temp_list[i]);
                    }
                }
            }
            ViewBag.sys_shop = system_shop[0];
            ViewBag.shop_list = shop_list;
            ViewBag.product = product;
            ViewBag.brand = brand;
            return View();
        }
        /// <summary>
        /// 生产计划关联客户订单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetForecastDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int product_id = Convert.ToInt32(Request.QueryString["productId"]);
            string start_time = Request.QueryString["startTime"];
            string end_time = Request.QueryString["endTime"];
            string shop_name = Request.QueryString["shopName"];
            int is_relation_batch = Convert.ToInt32(Request.QueryString["isRelation"]);
            API.Controllers.FactoryOrderController sale_order_ctrl = new FactoryOrderController();
            List<YNFactoryOrder> sale_order_list = sale_order_ctrl.getWaitOutSaleListByProductId(clothFactoryId, product_id, shop_name, start_time, end_time, is_relation_batch);

            ViewBag.sale_order_list = sale_order_list;
            return View();
        }
        /// <summary>
        /// 生产订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        /// <summary>
        /// 生产订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProductList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string key = Request.QueryString["searchKey"];
            int type = API.Controllers.ProductionOrderController.type_all;
            int lateState = Convert.ToInt32(Request.QueryString["lateState"]);
            int orderState = Convert.ToInt32(Request.QueryString["orderState"]);
            string orderStartDate = Request.QueryString["orderStartDate"];
            string orderEndDate = Request.QueryString["orderEndDate"];
            string jiaohuoStartDate = Request.QueryString["jiaohuoStartDate"];
            string jiaohuoEndDate = Request.QueryString["jiaohuoEndDate"];
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
            API.Controllers.ProductionOrderController productionOrderController = new ProductionOrderController();
            List<YNBanShiProductionOrder> orderList = productionOrderController.GetProductList(clothFactoryId, key, orderState, lateState, type, pageIndex, pageSize, orderStartDate, orderEndDate, jiaohuoStartDate, jiaohuoEndDate);
            int count = productionOrderController.GetProductCount(clothFactoryId, key, orderState, lateState, type, orderStartDate, orderEndDate, jiaohuoStartDate, jiaohuoEndDate);
            API.Controllers.PagesController page = new PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.orderList = orderList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 生产订单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int orderId = Convert.ToInt32(Request.QueryString["id"]);
            ProductionOrderController order_ctrl = new ProductionOrderController();
            YNBanShiProductionOrder order = order_ctrl.GetProductionOrderById(clothFactoryId, orderId);
            if (order == null) {
                return getLoginJsonMessage(0, "生产订单不存在");
            }
            ViewBag.order = order;
            return View();
        }
        //获取生产单工序数据
        public JsonResult GetProductProcessJson()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "未登录或者登录过期");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int orderId = Convert.ToInt32(Request.QueryString["id"]);
            ProductionOrderController order_ctrl = new ProductionOrderController();
            YNBanShiProductionOrder order = order_ctrl.GetProductionOrderById(clothFactoryId, orderId);
            if (order == null) {
                return getLoginJsonMessage(0, "没有查询到相关数据");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<OrderProductProcess> process_list = new List<OrderProductProcess>();
            if (!String.IsNullOrEmpty(order.process_info)) {
                process_list = js.Deserialize<List<OrderProductProcess>>(order.process_info);
            }
            List<OrderProductProcess> try_list = new List<OrderProductProcess>();
            if (!String.IsNullOrEmpty(order.try_process_info)) {
                try_list = js.Deserialize<List<OrderProductProcess>>(order.try_process_info);
            }
            for (var i = 0; i < try_list.Count; i++) {
                process_list.Add(try_list[i]);
            }
            return Json(new { code = 1, message = "成功", data = process_list }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 待安排生产订单
        /// </summary>
        /// <returns></returns>
        public ActionResult PendingList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// <summary>
        /// 待安排生产订单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPendingList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string key = Request.QueryString["searchKey"];
            int type = API.Controllers.ProductionOrderController.type_0;
            int lateState = API.Controllers.ProductionOrderController.late_state_all;
            int orderState = API.Controllers.ProductionOrderController.state_all;
            string orderStartDate = Request.QueryString["orderStartDate"];
            string orderEndDate = Request.QueryString["orderEndDate"];
            string jiaohuoStartDate = Request.QueryString["jiaohuoStartDate"];
            string jiaohuoEndDate = Request.QueryString["jiaohuoEndDate"];
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
            API.Controllers.ProductionOrderController productionOrderController = new ProductionOrderController();
            List<YNBanShiProductionOrder> orderList = productionOrderController.GetProductList(clothFactoryId, key, orderState, lateState, type, pageIndex, pageSize, orderStartDate, orderEndDate, jiaohuoStartDate, jiaohuoEndDate);
            int count = productionOrderController.GetProductCount(clothFactoryId, key, orderState, lateState, type, orderStartDate, orderEndDate, jiaohuoStartDate, jiaohuoEndDate);
            API.Controllers.PagesController page = new PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.orderList = orderList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 已安排生产订单
        /// </summary>
        /// <returns></returns>
        public ActionResult ArrangedList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// <summary>
        /// 待安排生产订单
        /// </summary>
        /// <returns></returns>
        public ActionResult GetArrangedList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string key = Request.QueryString["searchKey"];
            int type = API.Controllers.ProductionOrderController.type_1;
            int lateState = API.Controllers.ProductionOrderController.late_state_all;
            int orderState = API.Controllers.ProductionOrderController.state_all;
            string orderStartDate = null;
            string orderEndDate = null;
            string jiaohuoStartDate = null;
            string jiaohuoEndDate = null;
            string date = Request.QueryString["date"];
            int pageIndex = 1;
            int pageSize = 1000;
            if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            API.Controllers.ProductionOrderController productionOrderController = new ProductionOrderController();
            List<YNBanShiProductionOrder> orderList = productionOrderController.GetProductList(clothFactoryId, key, orderState, lateState, type, pageIndex, pageSize, orderStartDate, orderEndDate, jiaohuoStartDate, jiaohuoEndDate, date);
            //int count = productionOrderController.GetProductCount(clothFactoryId, key, orderState, lateState, type, orderStartDate, orderEndDate, jiaohuoStartDate, jiaohuoEndDate);
            //API.Controllers.PagesController page = new PagesController();
            //page.GetPage(pageIndex, count, pageSize);
            ViewBag.orderList = orderList;
            //ViewBag.page = page;
            return View();
        }
         /// <summary>
        ///打印交接单
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintTransferList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int orderId = Convert.ToInt32(Request.QueryString["id"]);
            ProductionOrderController order_ctrl = new ProductionOrderController();
            YNBanShiProductionOrder order = order_ctrl.GetProductionOrderById(clothFactoryId, orderId);
            ProductController product_ctrl = new ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(clothFactoryId, order.product_id);
            if (order == null) {
                return getLoginJsonMessage(0, "不存在该生产单");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<OrderProductProcess> process_list = new List<OrderProductProcess>();
            if (!String.IsNullOrEmpty(order.process_info)) {
                process_list = js.Deserialize<List<OrderProductProcess>>(order.process_info);
            }
            List<OrderProductProcess> try_list = new List<OrderProductProcess>();
            if (!String.IsNullOrEmpty(order.try_process_info))
            {
                try_list = js.Deserialize<List<OrderProductProcess>>(order.try_process_info);
            }
            for (var i = 0; i < try_list.Count; i++) {
                process_list.Add(try_list[i]);
            }
            ViewBag.process_list = process_list;
            ViewBag.order = order;
            ViewBag.product = product;
                return View();
        }

        //获取拥有某个品牌的客户列表
        public JsonResult GetCustomerListByBrandId()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(1, "成功");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int brandId = Convert.ToInt32(Request.QueryString["brandId"]);
            ShopInfoController shop_ctrl = new ShopInfoController();
            List<YNShopInfo> shop_temp_list = shop_ctrl.GetShopListByFactoryId(clothFactoryId);
            List<YNShopInfo> shop_list = new List<YNShopInfo>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            for (var i = 0; i < shop_temp_list.Count; i++) {
                if (!String.IsNullOrEmpty(shop_temp_list[i].brands_id))
                {
                    List<int> brand_list = js.Deserialize<List<int>>(shop_temp_list[i].brands_id);
                    if (brand_list.Contains(brandId)) {
                        shop_list.Add(shop_temp_list[i]);
                    }
                }
            }
            if (shop_list.Count > 0)
            {
                return Json(new { code = 1, message = "成功", data = shop_list }, JsonRequestBehavior.AllowGet);
            }
            else {
                return getLoginJsonMessage(0, "请先为客户添加该产品的品牌");
            }
        }

        /// 创建生产单
        private CreateSaleMessage CreateProductionOrder(int factory_id, ProductionOrderController production_ctrl, ProductController product_ctrl, List<YNFactoryOrder> sale_order_list, JavaScriptSerializer js, YNFactoryOrder sale_order_args, string remarks)
        {
            CreateSaleMessage msg = new CreateSaleMessage();
            msg.IsSuccess = false;

            YNBanShiProductionOrder production_order = new YNBanShiProductionOrder();
            production_order.jiaju_factory_id = factory_id;
            production_order.batch_num = "S" + DateTime.Now.ToString("yyyyMMdd") + "-" + (production_ctrl.getProductOrderCountByDate() + 1);
            for (int i = 0; i < sale_order_list.Count; i++)
            {
                List<string> batch_list = new List<string>();
                if (!String.IsNullOrEmpty(sale_order_list[i].ralation_produce_orderid))
                {
                    batch_list = js.Deserialize<List<string>>(sale_order_list[i].ralation_produce_orderid);
                }
                batch_list.Add(production_order.batch_num);
                sale_order_list[i].ralation_produce_orderid = js.Serialize(batch_list);
            }
            List<RelationSaleOrder> relation_sale_list = new List<RelationSaleOrder>();
            for (var i = 0; i < sale_order_list.Count; i++)
            {
                RelationSaleOrder relation_order = new RelationSaleOrder();
                relation_order.id = sale_order_list[i].id;
                relation_order.sale_num = sale_order_list[i].sale_id;
                relation_sale_list.Add(relation_order);
            }
            production_order.relation_sale_order = js.Serialize(relation_sale_list);
            production_order.delivery_date = sale_order_args.factory_delivery_day;
            production_order.customer_id = sale_order_args.shop_id;
            production_order.customer_name = sale_order_args.shop_name;
            production_order.brand_id = sale_order_args.brand_id;
            production_order.brand_name = sale_order_args.brand_name;
            production_order.product_id = sale_order_args.product_id;
            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, sale_order_args.product_id);
            if (product == null)
            {
                msg.Message = "该产品不存在";
                return msg;
            }
            production_order.product_name = sale_order_args.product_name;
            production_order.product_model = sale_order_args.product_model;
            production_order.product_format = sale_order_args.product_format;
            production_order.product_color = sale_order_args.product_color;
            production_order.product_number = sale_order_args.product_number;
            production_order.plan_remarks = remarks;
            List<OrderProductProcess> process_list = new List<OrderProductProcess>();
            if (String.IsNullOrEmpty(product.process_info))
            {
                msg.Message = "该产品未编辑工序信息";
                return msg;
            }
            process_list = js.Deserialize<List<OrderProductProcess>>(product.process_info);
            if (String.IsNullOrEmpty(product.packing_info))
            {
                msg.Message = "该产品未编辑打包工序信息";
                return msg;
            }
            List<OrderProductProcess> packing_list = js.Deserialize<List<OrderProductProcess>>(product.packing_info);
            List<ProductPackage> package_list = new List<ProductPackage>();

            List<ProductPackage> production_package_list = new List<ProductPackage>();
            if (String.IsNullOrEmpty(product.package_info))
            {
                msg.Message = "该产品未编辑包信息";
                return msg;
            }
            package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
            if (packing_list.Count > 0)
            {
                for (var i = 0; i < packing_list.Count; i++)
                {
                    if (package_list[i].relation_id == 0)
                    {
                        process_list.Add(packing_list[i]);
                        production_package_list.Add(package_list[i]);
                    }
                }
                for (var i = 0; i < production_package_list.Count; i++)
                {
                    production_package_list[i].number = 0;
                    production_package_list[i].lockNumber = 0;
                }
                production_order.package_info = js.Serialize(production_package_list);
            }
            if (process_list.Count > 0)
            {
                for (var i = 0; i < process_list.Count; i++)
                {
                    process_list[i].complate = 0;
                    process_list[i].error = 0;
                    process_list[i].scan_date = "";
                }
                production_order.process_info = js.Serialize(process_list);
            }
            if (!String.IsNullOrEmpty(product.try_process_info))
            {
                List<OrderProductProcess> try_list = js.Deserialize<List<OrderProductProcess>>(product.try_process_info);
                for (var i = 0; i < try_list.Count; i++)
                {
                    try_list[i].complate = 0;
                    try_list[i].error = 0;
                }
                production_order.try_process_info = js.Serialize(try_list);
            }
            //production_order.is_lock = 1;//设置为不锁定状态  这个属性目前没有使用意义
            production_order.state = 0;
            production_order.delete_status = 0;
            production_order.create_date = sale_order_args.create_time;
            production_order.modify_date = DateTime.Now;
            product.total_producing_num += sale_order_args.product_number;
            production_ctrl.Create(production_order, factory_id);


            msg.IsSuccess = true;
            msg.Message = "创建成功";
            return msg;
        }
        //制定生产计划
        public JsonResult submitAddProductionOrder()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int brandId = Convert.ToInt32(Request.QueryString["brandId"]);
            string brandName = Request.QueryString["brandName"];
            string createTime = Request.QueryString["createTime"];
            int productId = Convert.ToInt32(Request.QueryString["productId"]);
            string productColor = Request.QueryString["productColor"];
            string productFormat = Request.QueryString["productFormat"];
            string productModel = Request.QueryString["productModel"];
            string productName = Request.QueryString["productName"];
            string customer = Request.QueryString["customer"];
            int customerId = Convert.ToInt32(Request.QueryString["customerId"]);
            string deliveryTime = Request.QueryString["deliveryTime"];
            int number = Convert.ToInt32(Request.QueryString["number"]);
            string remarks = Request.QueryString["remarks"];
            string sale_order_id_str = Request.QueryString["saleOrderIds"];
            string[] sale_order_id_arr = null;
            if (!String.IsNullOrEmpty(Request.QueryString["saleOrderIds"]))
            {
                sale_order_id_arr = sale_order_id_str.Split(",");
            }
            List<int> sale_order_id_list = new List<int>();
            if (sale_order_id_arr != null)
            {
                for (int i = 0; i < sale_order_id_arr.Length; i++)
                {
                    sale_order_id_list.Add(Convert.ToInt32(sale_order_id_arr[i]));
                }
            }
            API.Controllers.ProductController product_ctrl = new ProductController();
            API.Controllers.ProductionOrderController production_ctrl = new ProductionOrderController();
            API.Controllers.FactoryOrderController sale_oder_ctrl = new FactoryOrderController();

            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, productId);
            if (product == null)
            {
                return getLoginJsonMessage(0, "示查询到该产品，不能下生产单");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (String.IsNullOrEmpty(product.package_info))
            {
                return getLoginJsonMessage(0, "该产品还未编辑包信息，不能下销售单");
            }
            List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
            //bool is_self = false;//自己否有有生产
            //bool is_relation = false;//是否有关联生产
            //for (var i = 0; i < package_list.Count; i++)
            //{
            //    if (package_list[i].relation_id == 0)
            //    {
            //        is_self = true;
            //    }
            //    else {
            //        is_relation = true;
            //    }
            //}

            //需要关联生产单的销售单
            List<YNFactoryOrder> sale_order_list = sale_oder_ctrl.GetSaleOrderListByIdInList(factory_id, productId, sale_order_id_list);

            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                //if (is_self)
                //{
                    //创建生产单
                    YNFactoryOrder sale_order_args = new YNFactoryOrder();
                    sale_order_args.factory_delivery_day = Convert.ToDateTime(deliveryTime);
                    sale_order_args.create_time = Convert.ToDateTime(createTime);
                    sale_order_args.shop_id = customerId;
                    sale_order_args.shop_name = customer;
                    sale_order_args.brand_id = brandId;
                    sale_order_args.brand_name = brandName;
                    sale_order_args.product_id = productId;
                    sale_order_args.product_name = productName;
                    sale_order_args.product_model = productModel;
                    sale_order_args.product_format = productFormat;
                    sale_order_args.product_color = productColor;
                    sale_order_args.product_number = number;
                    CreateSaleMessage msg = CreateProductionOrder(factory_id, production_ctrl, product_ctrl, sale_order_list, js, sale_order_args, remarks);
                    if (msg.IsSuccess == false)
                    {
                        return getLoginJsonMessage(0, msg.Message);
                    }
                //}
                ///主销售单下生产单时，跟随创建子生产单
                //API.Controllers.FactoryBrandController brand_ctrl = new FactoryBrandController();
                //if (is_relation)
                //{
                //    for (var i = 0; i < package_list.Count; i++)
                //    {
                //        if (package_list[i].relation_id != 0)
                //        {
                //            YNBanShiProduct temp_product = product_ctrl.GetProductById(factory_id, package_list[i].relation_id);
                //            if (temp_product == null)
                //            {
                //                return getLoginJsonMessage(0, "关联包的产品不存在");
                //            }
                //            //创建生产单
                //            YNFactoryOrder sale_order_args = new YNFactoryOrder();
                //            sale_order_args.factory_delivery_day = Convert.ToDateTime(deliveryTime);
                //            sale_order_args.create_time = Convert.ToDateTime(createTime);
                //            sale_order_args.shop_id = customerId;
                //            sale_order_args.shop_name = customer;
                //            YNBanShiBrand brand = brand_ctrl.GetBrandById(temp_product.brand_id, factory_id);
                //            if (brand == null)
                //            {
                //                return getLoginJsonMessage(0, "关联包产品的品牌不存在");
                //            }
                //            sale_order_args.brand_id = brand.id;
                //            sale_order_args.brand_name = brand.name;
                //            sale_order_args.product_id = temp_product.id;
                //            sale_order_args.product_name = temp_product.produce_name;
                //            sale_order_args.product_model = temp_product.model_name;
                //            sale_order_args.product_format = temp_product.format;
                //            sale_order_args.product_color = temp_product.color;
                //            sale_order_args.product_number = number;
                //            CreateSaleMessage msg = CreateProductionOrder(factory_id, production_ctrl, product_ctrl, sale_order_list, js, sale_order_args, remarks);
                //            if (msg.IsSuccess == false)
                //            {
                //                return getLoginJsonMessage(0, msg.Message);
                //            }
                //        }
                //    }

                //}
                sale_oder_ctrl.SaveChanges();
                product_ctrl.SaveChanges();
                transaction.Complete();
            }
            return getLoginJsonMessage(1, "保存成功");
        }
        
    }
}
