using System;
using System.Linq;
using System.Collections.Generic;
using System.Transactions;
using System.Web.Mvc;
using YNDIY.API.Models;
using YNDIY.API.Controllers;
using System.Web.Script.Serialization;

namespace YNDIY.Admin.Controllers
{
    public class CreateSaleMessage
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int backId { get; set; }
    }
    public class ERPOrderController : ParentController
    {
        /// 获取客户可以添加的品牌
        public JsonResult GetCustomerBrandProducttList()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int shopId = Convert.ToInt32(Request.QueryString["shopId"]);
            YNDIY.API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            YNDIY.API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(shopId, clothFactoryId);
            if (String.IsNullOrEmpty(shop.brands_id))
            {
                return getLoginJsonMessage(0, "该客户还未添加产品品牌");
            }
            List<int> brands_id = new JavaScriptSerializer().Deserialize<List<int>>(shop.brands_id);
            List<YNBanShiBrand> brand_list = brand_ctrl.GetBrandListInBrandIDList(clothFactoryId, brands_id);
            API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            API.Controllers.ShopPriceController price_ctrl = new API.Controllers.ShopPriceController();
            List<YNShopPrice> price_list = price_ctrl.GetCustomerPriceListIdInBrandList(clothFactoryId, brands_id, shop.id);
            List<ShopBrandProduct> list = new List<ShopBrandProduct>();
            for (var i = 0; i < brand_list.Count; i++)
            {
                ShopBrandProduct brand = new ShopBrandProduct();
                brand.brand = brand_list[i];
                List<YNBanShiProduct> product_list = product_ctrl.GetProductListByBrand(clothFactoryId, brand_list[i].id);
                if (product_list.Count > 0)
                {
                    brand.product = product_list;
                    //List<int> product_id = new List<int>();
                    List<decimal> price = new List<decimal>();
                    for (var j = 0; j < product_list.Count; j++)
                    {
                        //product_id.Add(product_list[j].id);
                        YNShopPrice temp_product = price_list.FirstOrDefault(w => w.brand_id == brand_list[i].id && w.product_id == product_list[j].id);
                        if (temp_product != null)
                        {
                            price.Add(temp_product.price);
                        }
                        else
                        {
                            price.Add(product_list[j].standard_price);
                        }
                    }
                    brand.customerPrice = price;
                }
                else
                {
                    brand.product = new List<YNBanShiProduct>();
                    brand.customerPrice = new List<decimal>();
                }
                list.Add(brand);
            }


            return Json(new { code = 1, message = "成功", data = list, isLock = shop.is_lockStore, isExamine = shop.is_examine }, JsonRequestBehavior.AllowGet);
        }
        /// 新建订单
        public ActionResult NewOrder()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int shopId = Convert.ToInt32(Request.QueryString["shopId"]);
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByFactoryId(shopId, clothFactoryId);
            //API.Controllers.ShopPriceController shopPriceController = new API.Controllers.ShopPriceController();
            //List<YNShopPrice> shopPriceList = shopPriceController.GetPriceListByCustomerId(clothFactoryId, shopId);
            //List<int> brandIdList = shopPriceList.GroupBy(s => s.brand_id).Select(s => s.Key).ToList();
            //API.Controllers.FactoryBrandController factoryBrandController = new API.Controllers.FactoryBrandController();
            //List<YNBanShiBrand> brandList = new List<YNBanShiBrand>();
            //for (int i = 0; i < brandIdList.Count; i++)
            //{
            //    YNBanShiBrand yNBanShiBrand = factoryBrandController.GetBrandById(brandIdList[i], clothFactoryId);
            //    brandList.Add(yNBanShiBrand);
            //}
            //API.Controllers.ProductController productController = new API.Controllers.ProductController();
            //List<YNBanShiProduct> productList = new List<YNBanShiProduct>();
            //for (int i = 0; i < shopPriceList.Count; i++)
            //{
            //    YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, shopPriceList[i].product_id);
            //    productList.Add(yNBanShiProduct);
            //}
            ViewBag.yNShopInfo = yNShopInfo;
            //ViewBag.brandList = brandList;
            //ViewBag.productList = productList;
            //ViewBag.shopPriceList = shopPriceList;
            return View();
        }
        // 创建销售单函数
        private CreateSaleMessage CreateSubmitOrder(int shopId, string shopName, string orderId, FactoryOrderController sale_order_ctrl, int factory_id, FactoryOrder order_args, string factory_consumer_address, string factory_consumer_linkman, string factory_consumer_phone, string factory_consumer_postcode, ProductController product_ctrl, YNShopInfo shop, ShopInfoController shop_ctrl, bool is_computing_value = true, int relation_order_id = 0)
        {
            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, order_args.product_id);
            if (product == null)
            {
                var error_msg = new CreateSaleMessage();
                error_msg.IsSuccess = false;
                error_msg.Message = "不存在id为：[" + order_args.product_id + "] 的产品";
                return error_msg;
            }

            if (CheckProductInventory(product.id))
            {
                var error_msg = new CreateSaleMessage();
                error_msg.IsSuccess = false;
                error_msg.Message =  product.model_name+ "正在等待盘点确认，不能执行下单操作";
                return error_msg;
            }


            YNFactoryOrder sale_order = new YNFactoryOrder();
            sale_order.shop_id = shopId;
            sale_order.shop_name = shopName;
            sale_order.order_id = orderId;

            sale_order.jiaju_factory_id = factory_id;
            sale_order.factory_delivery_day = order_args.factory_delivery_day;
            sale_order.factory_consumer_address = factory_consumer_address;
            sale_order.factory_consumer_linkman = factory_consumer_linkman;
            sale_order.factory_consumer_phone = factory_consumer_phone;
            sale_order.factory_consumer_postcode = factory_consumer_postcode;
            sale_order.remarks = order_args.remarks;
            sale_order.operator_id = Convert.ToInt32(Session["UserId"]);
            sale_order.operator_name = Convert.ToString(Session["NickName"]);
            sale_order.order_time = DateTime.Now;
            sale_order.product_id = order_args.product_id;
            sale_order.brand_id = order_args.brand_id;
            sale_order.brand_name = order_args.brand_name;
            sale_order.product_name = order_args.product_name;
            sale_order.product_model = order_args.product_model;
            sale_order.product_format = order_args.product_format;
            sale_order.product_color = order_args.product_color;
            sale_order.product_number = order_args.product_number;//这个数量要对应添加到产品
            product.total_order_num += sale_order.product_number;
            sale_order.checkFiance = order_args.checkFiance;
            sale_order.lockStock = order_args.lockStock;
            //设置库存锁定数据
            if (sale_order.lockStock == 0)
            {
                int lock_number = product.total_avaible_num > sale_order.product_number ? sale_order.product_number : product.total_avaible_num;
                product.toal_lock_num += lock_number;
                product.total_avaible_num -= lock_number;
                sale_order.product_lock_number = lock_number;

                //锁定相应产品包数据
                JavaScriptSerializer js = new JavaScriptSerializer();
                if (String.IsNullOrEmpty(product.package_info))
                {
                    var error_msg = new CreateSaleMessage();
                    error_msg.IsSuccess = false;
                    error_msg.Message = product.model_name+"没有编辑包信息，不能下单";
                    return error_msg;
                }
                List<ProductPackage> set_package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
                for (var i = 0; i < set_package_list.Count; i++)
                {
                    if (set_package_list[i].relation_id == 0)
                    {
                        set_package_list[i].lockNumber += lock_number;
                        set_package_list[i].number -= lock_number;
                    }
                }
                product.package_info = js.Serialize(set_package_list);
            }
            sale_order.unit = order_args.unit;
            sale_order.type = order_args.type == 0 ? 1 : 0;//判断客户订单还是自备库存订单

            //主订单
            if (is_computing_value)
            {
                sale_order.sale_id = "X" + DateTime.Now.ToString("yyyyMMdd") + "-" + (sale_order_ctrl.getFactoryOrderCountByData() + 1);
                //金额计算
                sale_order.is_relation_order = FactoryOrderController.relation_order_0;//主订单类型
                decimal price = product.standard_price;
                API.Controllers.ShopPriceController price_ctrl = new ShopPriceController();
                YNShopPrice shop_price = price_ctrl.GetPriceByProductId(factory_id, product.id, shopId);
                if (shop_price != null)
                {
                    price = shop_price.price;
                }
                sale_order.product_price = price;
                sale_order.money = price * order_args.product_number;//主订单价格
                if ((shop.balance_money + (shop.credit_limit - shop.credit_lock)) < sale_order.money)
                {
                    var error_msg = new CreateSaleMessage();
                    error_msg.IsSuccess = false;
                    error_msg.Message = "余额不足，不能够下销售单";
                    return error_msg;
                }
                if (shop.balance_money >= sale_order.money)
                {
                    shop.balance_money -= sale_order.money;
                    shop.lock_money += sale_order.money;
                }
                else
                {
                    shop.lock_money += shop.balance_money;
                    sale_order.money -= shop.balance_money;
                    shop.balance_money = 0;
                    shop.credit_lock += sale_order.money;
                }
                shop_ctrl.SaveChanges();
            }
            //子订单 不需要计算产品价格
            else
            {
                sale_order.sale_id = "[关联]X" + DateTime.Now.ToString("yyyyMMdd") + "-" + (sale_order_ctrl.getFactoryOrderCountByData() + 1);
                sale_order.is_relation_order = FactoryOrderController.relation_order_1;//子订单类型
                sale_order.product_price = 0;
                sale_order.money = 0;//子订单价格为0
                sale_order.relation_order_id = relation_order_id;//主订单ID
            }

            sale_order.create_time = DateTime.Now;
            sale_order.modify_time = DateTime.Now;
            product_ctrl.SaveChanges();
            //创建主销售单
            if (is_computing_value)
            {
                
                var message = new CreateSaleMessage();
                message.backId = sale_order_ctrl.CreateInt(sale_order);
                if (message.backId != -1)
                {
                    message.IsSuccess = true;
                    message.Message = "创建成功";
                }
                else {
                    message.IsSuccess = false;
                    message.Message = "创建失败";
                }
                return message;
            }
            //创建子销售单
            else
            {
                bool is_success = sale_order_ctrl.Create(sale_order);
                var message = new CreateSaleMessage();
                if (is_success)
                {
                    message.IsSuccess = true;
                    message.Message = "创建成功";
                }
                else
                {
                    message.IsSuccess = false;
                    message.Message = "创建失败";
                }
                return message;
            }
        }
        /// 新建订单数据提交
        public JsonResult submitNewOrder()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int shopId = Convert.ToInt32(Request.Form["shopId"]);
            string shopName = Request.Form["shopName"];
            string orderId = Request.Form["orderId"];
            string factory_consumer_address = Request.Form["factory_consumer_address"];
            string factory_consumer_postcode = Request.Form["factory_consumer_postcode"];
            string factory_consumer_linkman = Request.Form["factory_consumer_linkman"];
            string factory_consumer_phone = Request.Form["factory_consumer_phone"];
            if (string.IsNullOrEmpty(orderId))
            {
                return getLoginJsonMessage(0, "订单号不能为空");
            }
            string orderInfo = Request.Form["orderInfo"];
            if (string.IsNullOrEmpty(orderInfo))
            {
                return getLoginJsonMessage(0, "产品数据不能为空");
            }
            //List<YNFactoryOrder> orderList = new List<YNFactoryOrder>();
            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
                YNShopInfo shop = shop_ctrl.GetShopInfoByID(shopId);
                API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
                API.Controllers.FactoryOrderController sale_order_ctrl = new API.Controllers.FactoryOrderController();
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<FactoryOrder> orderListTemp = js.Deserialize<List<FactoryOrder>>(orderInfo);

                for (int i = 0; i < orderListTemp.Count; i++)
                {
                    #region private void CreateSubmitOrder
                    //YNFactoryOrder yNFactoryOrder = new YNFactoryOrder();
                    //yNFactoryOrder.shop_id = shopId;
                    //yNFactoryOrder.shop_name = shopName;
                    //yNFactoryOrder.order_id = orderId;
                    //string saleOrderId = "X" + DateTime.Now.ToString("yyyyMMdd") + "-" + (factoryOrderController.getFactoryOrderCountByData() + 1);
                    //yNFactoryOrder.sale_id = saleOrderId;
                    //yNFactoryOrder.jiaju_factory_id = clothFactoryId;
                    //yNFactoryOrder.factory_delivery_day = orderListTemp[i].factory_delivery_day;
                    //yNFactoryOrder.factory_consumer_address = factory_consumer_address;
                    //yNFactoryOrder.factory_consumer_linkman = factory_consumer_linkman;
                    //yNFactoryOrder.factory_consumer_phone = factory_consumer_phone;
                    //yNFactoryOrder.factory_consumer_postcode = factory_consumer_postcode;
                    //yNFactoryOrder.remarks = orderListTemp[i].remarks;
                    //yNFactoryOrder.operator_id = Convert.ToInt32(Session["UserId"]);
                    //yNFactoryOrder.operator_name = Convert.ToString(Session["NickName"]);
                    //yNFactoryOrder.order_time = DateTime.Now;
                    //yNFactoryOrder.brand_id = orderListTemp[i].brand_id;
                    //yNFactoryOrder.brand_name = orderListTemp[i].brand_name;
                    //yNFactoryOrder.product_id = orderListTemp[i].product_id;
                    //yNFactoryOrder.product_name = orderListTemp[i].product_name;
                    //yNFactoryOrder.product_model = orderListTemp[i].product_model;
                    //yNFactoryOrder.product_format = orderListTemp[i].product_format;
                    //yNFactoryOrder.product_color = orderListTemp[i].product_color;
                    //yNFactoryOrder.product_number = orderListTemp[i].product_number;//这个数量要对应添加到产品
                    //YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, yNFactoryOrder.product_id);
                    //yNBanShiProduct.total_order_num += yNFactoryOrder.product_number;
                    //yNFactoryOrder.checkFiance = orderListTemp[i].checkFiance;
                    //yNFactoryOrder.lockStock = orderListTemp[i].lockStock;
                    ////设置库存锁定数据
                    //if (yNFactoryOrder.lockStock == 0)
                    //{
                    //    int lock_number = yNBanShiProduct.total_avaible_num > yNFactoryOrder.product_number ? yNFactoryOrder.product_number : yNBanShiProduct.total_avaible_num;
                    //    yNBanShiProduct.toal_lock_num += lock_number;
                    //    yNBanShiProduct.total_avaible_num -= lock_number;
                    //    yNFactoryOrder.product_lock_number = lock_number;
                    //}
                    //yNFactoryOrder.product_price = orderListTemp[i].product_price;
                    //yNFactoryOrder.unit = orderListTemp[i].unit;
                    //yNFactoryOrder.type = yNShopInfo.type==0 ? 1 : 0;//判断客户订单还是自备库存订单
                    //yNFactoryOrder.money = orderListTemp[i].product_price * orderListTemp[i].product_number;
                    ////yNFactoryOrder 相应金额后面处理
                    //yNFactoryOrder.create_time = DateTime.Now;
                    //yNFactoryOrder.modify_time = DateTime.Now;
                    //productController.SaveChanges();
                    //factoryOrderController.CreateByFactory(yNFactoryOrder, clothFactoryId);
                    #endregion
                    CreateSaleMessage msg = CreateSubmitOrder(shopId, shopName, orderId, sale_order_ctrl, factory_id, orderListTemp[i], factory_consumer_address, factory_consumer_linkman, factory_consumer_phone, factory_consumer_postcode, product_ctrl, shop, shop_ctrl);
                    if (msg.IsSuccess == false)
                    {
                        return getLoginJsonMessage(0, msg.Message);
                    }
                    YNBanShiProduct sale_product = product_ctrl.GetProductById(factory_id, orderListTemp[i].product_id);
                    #region 创建子销售单
                    if (!String.IsNullOrEmpty(sale_product.package_info))
                    {
                        List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(sale_product.package_info);
                        for (var j = 0; j < package_list.Count; j++)
                        {
                            if (package_list[j].relation_id != 0)
                            {
                                YNBanShiProduct temp_product = product_ctrl.GetProductById(factory_id, package_list[j].relation_id);
                                if (temp_product == null)
                                {
                                    return getLoginJsonMessage(0, "不存在id为：[" + package_list[j].relation_id + "] 的产品");
                                }
                                FactoryOrder temp_order = new FactoryOrder();
                                temp_order.factory_delivery_day = orderListTemp[i].factory_delivery_day;
                                temp_order.remarks = orderListTemp[i].remarks;
                                temp_order.product_id = temp_product.id;
                                temp_order.brand_id = temp_product.brand_id;
                                FactoryBrandController brand_ctrl = new FactoryBrandController();
                                temp_order.brand_name = brand_ctrl.GetBrandById(temp_order.brand_id, factory_id).name;
                                temp_order.product_name = temp_product.produce_name;
                                temp_order.product_model = temp_product.model_name;
                                temp_order.product_format = temp_product.format;
                                temp_order.product_color = temp_product.color;
                                temp_order.product_number = orderListTemp[i].product_number;//这个数量要对应添加到产品
                                temp_order.checkFiance = orderListTemp[i].checkFiance;
                                temp_order.lockStock = orderListTemp[i].lockStock;
                                CreateSaleMessage sub_create = CreateSubmitOrder(shopId, shopName, orderId, sale_order_ctrl, factory_id, temp_order, factory_consumer_address, factory_consumer_linkman, factory_consumer_phone, factory_consumer_postcode, product_ctrl, shop, shop_ctrl, false, msg.backId);
                                if (sub_create.IsSuccess == false)
                                {
                                    return getLoginJsonMessage(0, sub_create.Message);
                                }

                            }
                        }
                    }
                    #endregion
                }
                sale_order_ctrl.SaveChanges();
                transaction.Complete();
            }
            return getLoginJsonMessage(1, "添加订单成功");
        }
        /// 编辑订单
        public ActionResult EditOrder()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int shopId = Convert.ToInt32(Request.QueryString["shopId"]);
            int orderId = Convert.ToInt32(Request.QueryString["orderId"]);

            API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(shopId, clothFactoryId);
            API.Controllers.FactoryOrderController order_ctrl = new API.Controllers.FactoryOrderController();
            YNFactoryOrder order = order_ctrl.FactoryGetFactoryOrderById(orderId, clothFactoryId);
            API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(clothFactoryId, order.product_id);
            API.Controllers.ShopPriceController price_ctrl = new API.Controllers.ShopPriceController();
            YNShopPrice price = price_ctrl.GetPriceByProductId(clothFactoryId, order.product_id, shop.id);


            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            //API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
            //YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(clothFactoryId);
            //ViewBag.yNClothFactoryInfo = yNClothFactoryInfo;
            //if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            //{
            //    id = Convert.ToInt32(Request.QueryString["id"]);
            //    YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
            //    YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(yNFactoryOrder.shop_id);
            //    ViewBag.yNFactoryOrder = yNFactoryOrder;
            //    ViewBag.yNShopInfo = yNShopInfo;
            //}
            //else
            //{
            //    shopId = Convert.ToInt32(Request.QueryString["shopId"]);
            //    YNFactoryOrder yNFactoryOrder = new YNFactoryOrder();
            //    YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByFactoryId(shopId, clothFactoryId);
            //    ViewBag.yNFactoryOrder = yNFactoryOrder;
            //    ViewBag.yNShopInfo = yNShopInfo;
            //}
            ViewBag.order = order;
            ViewBag.shop = shop;
            ViewBag.product = product;
            if (price != null)
            {
                ViewBag.customer_price = price.price;
            }
            else
            {
                ViewBag.customer_price = product.standard_price;
            }
            return View();
        }
        /// 订单列表
        public ActionResult OrderList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// 订单列表数据
        public ActionResult GetOrderList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string key = Request.QueryString["searchKey"];
            int order_status = Convert.ToInt32(Request.QueryString["order_status"]);
            int yanqi_status = Convert.ToInt32(Request.QueryString["yanqi_status"]);
            int kucun_status = Convert.ToInt32(Request.QueryString["kucun_status"]);
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
            string start_data = Request.QueryString["start_data"];
            string end_data = Request.QueryString["end_data"];
            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            List<YNFactoryOrder> orderList = factoryOrderController.getFactoryOrderListByFactory(key, clothFactoryId, order_status, yanqi_status, kucun_status, -1, pageIndex, pageSize, null, null, start_data, end_data);
            List<int> product_id_list = new List<int>();
            for (var i = 0; i < orderList.Count; i++)
            {
                product_id_list.Add(orderList[i].product_id);
            }
            API.Controllers.ProductController product_ctrl = new ProductController();
            List<YNBanShiProduct> productList = product_ctrl.GetProductListInProductIdList(clothFactoryId, product_id_list);

            int count = factoryOrderController.getFactoryOrderListByFactoryCount(key, clothFactoryId, order_status, yanqi_status, kucun_status, -1, null, null, start_data, end_data);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.orderList = orderList;
            ViewBag.productList = productList;
            ViewBag.page = page;
            ViewBag.product_ctrl = product_ctrl;
            ViewBag.factory_id = clothFactoryId;
            ViewBag.sale_order_ctrl = factoryOrderController;
            return View();
        }
        /// 订单详情
        public ActionResult OrderDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.FactoryOrderController order_ctrl = new FactoryOrderController();
            YNFactoryOrder order = order_ctrl.FactoryGetFactoryOrderById(id, clothFactoryId);
            if (order == null)
            {
                return getLoginJsonMessage(0, "不存在该订单号");
            }
            API.Controllers.ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(order.shop_id, clothFactoryId);
            API.Controllers.ShopPriceController price_ctrl = new ShopPriceController();
            YNShopPrice price = price_ctrl.GetPriceByProductId(clothFactoryId, order.product_id, shop.id);
            decimal customer_price = order.product_price;
            if (price != null)
            {
                customer_price = price.price;
            }
            API.Controllers.ProductController product_ctrl = new ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(clothFactoryId, order.product_id);

            ViewBag.product = product;
            ViewBag.customer_price = customer_price;
            ViewBag.sale_order = order;
            ViewBag.shop = shop;
            return View();
        }
        /// 出库计划
        public ActionResult DeliverPlan()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            return View();
        }
        /// 出库计划数据
        public ActionResult GetDeliverPlan()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string out_date = Request.QueryString["outDate"];
            int is_examine = Convert.ToInt32(Request.QueryString["isExamine"]);
            int examine_state = Convert.ToInt32(Request.QueryString["examineState"]);
            string sale_num = Request.QueryString["saleNum"];
            API.Controllers.FactoryStorageOutController out_ctrl = new FactoryStorageOutController();
            List<YNStorageOut> out_list = out_ctrl.GetListByOutDateAndStateAndSaleNum(clothFactoryId, out_date, is_examine, examine_state, sale_num);

            ViewBag.out_list = out_list;
            return View();
        }
        /// 拣货单
        public ActionResult PickingList()
        {
            return View();
        }
        /// 销货单
        public ActionResult SalesList()
        {
            return View();
        }

        public CreateSaleMessage CreateOutPlan(int factory_id, FactoryOrderController sale_order_ctrl, ProductController product_ctrl, JavaScriptSerializer js, int orderId, int out_count, string out_remark, string out_date, int relation_plan_id = 0)
        {
            CreateSaleMessage msg = new CreateSaleMessage();
            msg.IsSuccess = false;
            YNFactoryOrder sale_order = sale_order_ctrl.FactoryGetFactoryOrderById(orderId, factory_id);
            if (sale_order == null)
            {
                msg.Message = "订单数据查询错误";
                return msg;
            }
            if ((sale_order.product_number - sale_order.product_out_number - sale_order.out_lock_number) < out_count)
            {
                msg.Message = "出库数量不能大于待出库数量";
                return msg;
            }
            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, sale_order.product_id);
            if (product == null)
            {
                msg.Message = "该产品不存在，不能设置出库";
                return msg;
            }
            if (CheckProductInventory(product.id))
            {
                msg.Message = "该产品正在等待盘点确认，不能设置出库";
                return msg;
            }

            API.Controllers.ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(sale_order.shop_id, factory_id);
            if (shop == null)
            {
                msg.Message = "该出库计划的客户不存在，不能设置出库";
                return msg;
            }

            YNStorageOut plan = new YNStorageOut();
            plan.factory_id = factory_id;
            plan.order_id = sale_order.id;
            plan.order_num = sale_order.order_id;
            plan.sale_num = sale_order.sale_id;
            plan.customer_id = sale_order.shop_id;
            plan.customer_name = sale_order.shop_name;
            plan.brand_id = sale_order.brand_id;
            plan.brand_name = sale_order.brand_name;
            plan.product_id = sale_order.product_id;
            plan.product_name = sale_order.product_name;
            plan.product_model = sale_order.product_model;
            plan.product_format = sale_order.product_format;
            plan.product_color = sale_order.product_color;
            plan.product_number = sale_order.product_number;
            plan.out_plan_money = out_count * sale_order.product_price;
            plan.plan_state = FactoryStorageOutController.plan_state_0;//主销售单
            if (relation_plan_id != 0)
            {
                plan.plan_state = FactoryStorageOutController.plan_state_1;//子销售单
                plan.relation_plan_id = relation_plan_id;
                plan.out_plan_money = 0;
            }
            //计划出库产品价值
            decimal out_money = plan.out_plan_money;
            #region 拨款到 计划出库单
            //从 销售单余额 拨款到 计划出库单
            if (out_money > 0)
            {
                if (sale_order.balance_money > 0)
                {
                    if (sale_order.balance_money >= out_money)
                    {
                        plan.sale_payed += out_money;
                        sale_order.balance_money -= out_money;
                        out_money = 0;
                    }
                    else
                    {
                        out_money -= sale_order.balance_money;
                        plan.sale_payed += sale_order.balance_money;
                        sale_order.balance_money = 0;
                    }
                }
            }
            ////从 客户锁定余额 拨款到 计划出库单
            //if (out_money > 0)
            //{
            //    if (shop.lock_money > 0)
            //    {
            //        if (shop.lock_money >= out_money)
            //        {
            //            shop.lock_money -= out_money;
            //            plan.lock_payed += out_money;
            //            out_money = 0;
            //        }
            //        else
            //        {
            //            out_money -= shop.lock_money;
            //            plan.lock_payed += shop.lock_money;
            //            shop.lock_money = 0;
            //        }
            //    }
            //}
            ////从 客户可用余额 拨款到 计划出库单
            //if (out_money > 0)
            //{
            //    if (shop.balance_money > 0)
            //    {
            //        if (shop.balance_money >= out_money)
            //        {
            //            shop.balance_money -= out_money;
            //            plan.balance_payed += out_money;
            //            out_money = 0;
            //        }
            //        else
            //        {
            //            out_money -= shop.balance_money;
            //            plan.balance_payed += shop.balance_money;
            //            shop.balance_money = 0;
            //        }
            //    }
            //}
            shop_ctrl.SaveChanges();
            #endregion
            int min_out_count = 0;//最小出库数量
            if (sale_order.lockStock == 0)
            {
                min_out_count = sale_order.product_lock_number;
            }
            else
            {
                min_out_count = product.total_avaible_num + sale_order.product_lock_number;
            }

            List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
            if (min_out_count < out_count)
            {
                msg.Message = "该产品库存数量不足够出库";
                return msg;
            }
            //*****************************////
            //  把库存拨出的数据放进出库单 ||||
            //*****************************////// 非锁定库存
            if (sale_order.product_lock_number >= out_count)//先从锁定数据扣
            {
                sale_order.product_lock_number -= out_count;//销售单锁定库存数量 减去 出库数量
                sale_order.out_lock_number += out_count;//销售单预备出库锁定 加上 出库数量
                plan.out_lock_num += out_count;
                product.toal_lock_num -= out_count;
                //从库存包拨出数据
                for (var i = 0; i < package_list.Count; i++)
                {
                    if (package_list[i].relation_id == 0)
                    {
                        package_list[i].lockNumber -= out_count;
                    }
                }
                out_count = 0;
            }
            else
            {//先从锁定数据扣
                out_count -= sale_order.product_lock_number;
                sale_order.out_lock_number += sale_order.product_lock_number;
                plan.out_lock_num += sale_order.product_lock_number;
                product.toal_lock_num -= sale_order.product_lock_number;
                //从库存包拨出数据
                for (var i = 0; i < package_list.Count; i++)
                {
                    if (package_list[i].relation_id == 0)
                    {
                        package_list[i].lockNumber -= sale_order.product_lock_number;
                    }
                }
                sale_order.product_lock_number = 0;
            }
            //再从未锁定数据扣
            //从库存包拨出数据
            for (var i = 0; i < package_list.Count; i++)
            {
                if (package_list[i].relation_id == 0)
                {
                    package_list[i].number -= out_count;
                }
            }
            plan.out_num = out_count;
            product.total_avaible_num -= out_count;

            product.package_info = js.Serialize(package_list);
            plan.product_price = sale_order.product_price;
            plan.jiaohuo_date = sale_order.factory_delivery_day;
            plan.out_remark = out_remark;
            //plan.examin_remark
            plan.plan_out_date = Convert.ToDateTime(out_date);
            if (sale_order.checkFiance == 0)
            {
                plan.is_examine = 0;//需要审核
                plan.examine_state = 0;//待审核
            }
            else
            {
                plan.is_examine = 1;//不需要审核
                plan.examine_state = 1;//审核通过                    
            }
            plan.delete_state = 0;
            plan.modify_date = DateTime.Now;
            plan.create_date = DateTime.Now;

            API.Controllers.FactoryStorageOutController out_ctrl = new FactoryStorageOutController();
            int plan_id = out_ctrl.CreateInt(plan);
            if (plan_id == -1)
            {
                msg.Message = "发起出库计划失败";
                return msg;
            }
            sale_order.out_lock_number += out_count;//销售单 待出库锁定 加上 出库数量
            msg.IsSuccess = true;
            msg.Message = "成功";
            msg.backId = plan_id;
            return msg;
        }
        /// 添加出库计划 
        public JsonResult AddStorageOutPlan()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            int orderId = Convert.ToInt32(Request.QueryString["orderId"]);
            int out_count = Convert.ToInt32(Request.QueryString["outCount"]);
            string out_date = Request.QueryString["outDate"];
            string out_remark = Request.QueryString["remarks"];
            JavaScriptSerializer js = new JavaScriptSerializer();
            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                API.Controllers.FactoryOrderController sale_order_ctrl = new FactoryOrderController();
                API.Controllers.ProductController product_ctrl = new ProductController();

                CreateSaleMessage msg = CreateOutPlan(factory_id, sale_order_ctrl, product_ctrl, js, orderId, out_count, out_remark, out_date);
                if (msg.IsSuccess == false)
                {
                    return getLoginJsonMessage(0, msg.Message);
                }
                List<YNFactoryOrder> sub_sale_order_list = sale_order_ctrl.GetRelationSaleOrderListBySaleOrderId(factory_id, orderId);
                for (var i = 0; i < sub_sale_order_list.Count; i++)
                {
                    CreateSaleMessage sub_msg = CreateOutPlan(factory_id, sale_order_ctrl, product_ctrl, js, sub_sale_order_list[i].id, out_count, out_remark, out_date, msg.backId);
                    if (sub_msg.IsSuccess == false)
                    {
                        return getLoginJsonMessage(0, sub_msg.Message);
                    }
                }


                sale_order_ctrl.SaveChanges();
                product_ctrl.SaveChanges();
                transaction.Complete();
            }

            return getLoginJsonMessage(1, "成功");
        }
        /// 获取某个销售单出库历史数据
        public JsonResult GetSaleOrderOutHistory()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int orderId = Convert.ToInt32(Request.QueryString["orderId"]);//销售单ID
            API.Controllers.FactoryStorageOutController out_ctrl = new FactoryStorageOutController();
            List<YNStorageOut> out_list = out_ctrl.GetHistroyListByOrderId(clothFactoryId, orderId);
            List<object> obj_list = new List<object>();
            for (var i = 0; i < out_list.Count; i++)
            {
                Object obj = new
                {
                    plan_out_date = out_list[i].plan_out_date.ToString("yyyy-MM-dd"),
                    out_count = (out_list[i].out_lock_num + out_list[i].out_num),
                    state = (out_list[i].examine_state == 0 ? "未处理"
                           : out_list[i].examine_state == 1 ? "审核通过"
                           : out_list[i].examine_state == 2 ? "审核未通过" : "已经出库")
                };
                obj_list.Add(obj);
            }

            return Json(new { code = 1, message = "成功", data = obj_list }, JsonRequestBehavior.AllowGet);
        }
        /// 获取可调拨库存数据
        public ActionResult GetAllocation()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            //int product_id = Convert.ToInt32(Request.QueryString["productId"]);
            int self_sale_order_id = Convert.ToInt32(Request.QueryString["id"]);

            API.Controllers.FactoryOrderController sale_order_ctrl = new FactoryOrderController();
            YNFactoryOrder sale_order = sale_order_ctrl.FactoryGetFactoryOrderById(self_sale_order_id, factory_id);
            //获取可以调拨的销售单
            List<YNFactoryOrder> avaible_sale_order_list = sale_order_ctrl.getAvaibleSaleOrderList(factory_id, sale_order.product_id, self_sale_order_id);
            ProductController product_ctrl = new ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(factory_id, sale_order.product_id);
            API.Controllers.FactoryBrandController brand_ctrl = new FactoryBrandController();
            YNBanShiBrand brand = brand_ctrl.GetBrandById(product.brand_id, factory_id);
            ViewBag.brand = brand;
            ViewBag.product = product;
            ViewBag.sale_order_list = avaible_sale_order_list;
            return View();
        }

        /// 发起库存调拨
        public JsonResult SetAllocation()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            //销售单ID
            int self_sale_order_id = Convert.ToInt32(Request.QueryString["id"]);
            //库存调拨数量
            string stock_count_str = Request.QueryString["stockCount"];
            int stock_count = 0;
            if (!String.IsNullOrEmpty(stock_count_str))
            {
                stock_count = Convert.ToInt32(stock_count_str);
            }
            //需要调拨出的销售单号
            string sale_order_ids_str = Request.QueryString["saleOrderIds"];
            //需要调拨出的数量
            string sale_order_count_str = Request.QueryString["saleOrderCount"];
            string[] sale_ids_arr_str = sale_order_ids_str.Split(",");
            string[] sale_count_arr_str = sale_order_count_str.Split(",");
            if (sale_ids_arr_str.Length != sale_count_arr_str.Length)
            {
                return getLoginJsonMessage(0, "调拨的销售单与调拨数量参数个数不匹配");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                ProductController product_ctrl = new ProductController();
                API.Controllers.FactoryOrderController sale_order_ctrl = new FactoryOrderController();
                YNFactoryOrder sale_order = sale_order_ctrl.FactoryGetFactoryOrderById(self_sale_order_id, factory_id);
                if (sale_order == null)
                {
                    return getLoginJsonMessage(0, "未查询到该销售单");
                }
                int allocation_count = stock_count;
                List<int> sale_count_list = new List<int>();
                for (var i = 0; i < sale_count_arr_str.Length; i++)
                {
                    int _temp_count = Convert.ToInt32(sale_count_arr_str[i]);
                    allocation_count += _temp_count;
                    sale_count_list.Add(_temp_count);
                }
                int wait_allocation_count = sale_order.product_number - sale_order.product_out_number - sale_order.product_lock_number - sale_order.out_lock_number;
                if (allocation_count > wait_allocation_count)
                {
                    return getLoginJsonMessage(0, "调拨数量不能大于原销售单欠缺数量");
                }
                List<int> allcation_sale_id_list = new List<int>();
                for (var i = 0; i < sale_ids_arr_str.Length; i++)
                {
                    allcation_sale_id_list.Add(Convert.ToInt32(sale_ids_arr_str[i]));
                }
                YNBanShiProduct product = product_ctrl.GetProductById(factory_id, sale_order.product_id);
                if (product == null)
                {
                    return getLoginJsonMessage(0, "该销售单所对应的产品不存在");
                }
                if (stock_count > product.total_avaible_num)
                {
                    return getLoginJsonMessage(0, "该产品没有足够的库存可供调拨");
                }
                if (String.IsNullOrEmpty(product.package_info))
                {
                    return getLoginJsonMessage(0, "该产品没有编辑包信息");
                }
                if (((stock_count < allocation_count) && product.total_avaible_num >= allocation_count) || (stock_count == 0 && product.total_avaible_num > 0))
                {
                    return getLoginJsonMessage(0, "请先从可用库存调拨");
                }
                List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product.package_info);
                
                //从库存调拨
                if (stock_count > 0)
                {
                    sale_order.product_lock_number += stock_count;
                    product.total_avaible_num -= stock_count;
                    product.toal_lock_num += stock_count;
                    for (var i = 0; i < package_list.Count; i++)
                    {
                        package_list[i].number -= stock_count;
                        package_list[i].lockNumber += stock_count;
                    }
                    product.package_info = js.Serialize(package_list);
                }
                List<YNFactoryOrder> temp_sale_order = sale_order_ctrl.GetListInIdList(allcation_sale_id_list, factory_id);
                if (temp_sale_order.Count != allcation_sale_id_list.Count)
                {
                    return getLoginJsonMessage(0, "调拨出的销售单ID数据错误");
                }
                //从销售单调拨
                for (var i = 0; i < temp_sale_order.Count; i++)
                {
                    if (temp_sale_order[i].product_id != product.id)
                    {
                        return getLoginJsonMessage(0, "非法操作，调拨出的销售单数据异常");
                    }
                    temp_sale_order[i].product_lock_number -= sale_count_list[i];
                    sale_order.product_lock_number += sale_count_list[i];
                }

                sale_order_ctrl.SaveChanges();
                product_ctrl.SaveChanges();
                transaction.Complete();
            }
            return getLoginJsonMessage(1, "调拨成功");
        }

    }
}