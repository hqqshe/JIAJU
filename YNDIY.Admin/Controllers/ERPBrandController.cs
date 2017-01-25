using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class ERPBrandController : ParentController
    {
        /// <summary>
        /// 品牌列表
        /// </summary>
        /// <returns></returns>
        public ActionResult BrandList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// <summary>
        /// 品牌列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBrandList()
        {
            if (!checkSession())
            {
                return loginRerirect();
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
            YNDIY.API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> brand_list = brand_ctrl.GetBrandList(clothFactoryId, pageIndex, pageSize, key);

            List<int> brand_id = new List<int>();
            for (var i = 0; i < brand_list.Count; i++)
            {
                brand_id.Add(brand_list[i].id);
            }
            YNDIY.API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            List<YNBanShiProduct> product_list = product_ctrl.GetProductListInBrandList(clothFactoryId, brand_id);
            int count = brand_ctrl.GetBrandCount(clothFactoryId, key);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.list = brand_list;
            ViewBag.product_list = product_list;
            ViewBag.page = page;
            return View();
        }
       /// <summary>
       /// 获取所有品牌列表
       /// </summary>
       /// <returns> JSON格式数据 </returns>
        public JsonResult GetAllBranList()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            YNDIY.API.Controllers.FactoryBrandController brand_ctrl = new YNDIY.API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> list = brand_ctrl.GetAllBrandList(clothFactoryId);
            return Json(new { code = 1, message = "成功", data = list }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取客户某个品牌所有产品价格
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllPriceByBrandId()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int customerId = Convert.ToInt32(Request.QueryString["customer_id"]);
            int brandId = Convert.ToInt32(Request.QueryString["brand_id"]);
            YNDIY.API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            List<YNBanShiProduct> productList = product_ctrl.GetProductListByBrand(clothFactoryId, brandId);
            List<ShopMorePrice> morePrice = new List<ShopMorePrice>();
            for (var i = 0; i < productList.Count; i++) {
                ShopMorePrice more = new ShopMorePrice();
                more.product = productList[i];
                morePrice.Add(more);
            }
            YNDIY.API.Controllers.ShopPriceController price_ctrl = new API.Controllers.ShopPriceController();
            List<YNShopPrice> priceList = price_ctrl.GetPriceListByBrandId(clothFactoryId, brandId, customerId);
            if (priceList.Count > 0) {
                for (var i = 0; i < productList.Count; i++)
                {
                    for (var j = 0; j < priceList.Count; j++) {
                        if (priceList[j].product_id == productList[i].id) {
                            morePrice[i].price = priceList[j];
                            break;
                        }
                    }
                }
            }
            return Json(new { code = 1, message = "成功", data = morePrice }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 设置某个客户在某个产品下面的产品价格
        /// </summary>
        /// <returns></returns>
        public JsonResult SetPriceInBrand()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int customerId = Convert.ToInt32(Request.Form["customer_id"]);
            int brandId = Convert.ToInt32(Request.Form["brand_id"]);
            string data = Request.Form["data"];
            List<ShopMorePrice> more_list = new JavaScriptSerializer().Deserialize<List<ShopMorePrice>>(data);
            List<int> product_list = new List<int>();
            for (var i = 0; i < more_list.Count; i++)
            {
                product_list.Add(more_list[i].product.id);
            }
            API.Controllers.ShopPriceController price_ctrl = new API.Controllers.ShopPriceController();
            List<YNShopPrice> price_list = price_ctrl.GetPriceListByBrandId(clothFactoryId, brandId, customerId);
            List<YNShopPrice> price_list_modify = price_list.Where(w => product_list.Contains(w.product_id)).ToList();
            List<int> modifiy_list = new List<int>();
            for (var i = 0; i < price_list_modify.Count; i++)
            {
                modifiy_list.Add(price_list_modify[i].product_id);
            }
            List<ShopMorePrice> price_list_create = more_list.Where(w => !modifiy_list.Contains(w.product.id)).ToList();
            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                //修改
                for (var i = 0; i < price_list_modify.Count; i++)
                {
                    for (var j = 0; j < more_list.Count; j++)
                    {
                        if (more_list[j].price.id == price_list_modify[i].id)
                        {
                            price_list_modify[i].price = more_list[j].price.price;
                            price_list_modify[i].modify_time = DateTime.Now;
                            break;
                        }
                    }
                }
                //新增
                for (var i = 0; i < price_list_create.Count; i++)
                {
                    YNShopPrice price = new YNShopPrice();
                    price.factory_id = clothFactoryId;
                    price.customer_id = customerId;
                    price.brand_id = brandId;
                    price.product_id = price_list_create[i].product.id;
                    price.price = price_list_create[i].price.price;
                    price.create_time = DateTime.Now;
                    price.modify_time = price.create_time;
                    price_ctrl.Create(price);
                }
                price_ctrl.SaveChanges();
                transaction.Complete();
            }
            return Json(new { code = 1, message = "成功" });
        }
        public JsonResult DeleteSomeoneBrandInCustomer()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int customerId = Convert.ToInt32(Request.QueryString["customer_id"]);
            int brandId = Convert.ToInt32(Request.QueryString["brand_id"]);
            API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(customerId, clothFactoryId);
            if (shop == null) {
                return getLoginJsonMessage(0, "该客户不存在");
            }
            if (String.IsNullOrEmpty(shop.brands_id)) {
                return getLoginJsonMessage(0, "该客户下面没有该品牌");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<int> brand_id_list = js.Deserialize<List<int>>(shop.brands_id);
            if (!brand_id_list.Contains(brandId))
            {
                return getLoginJsonMessage(0, "该客户下面没有该品牌");
            }
            for (var i = 0; i < brand_id_list.Count; i++)
            {
                if (brand_id_list[i] == brandId) {
                    brand_id_list.RemoveAt(i);
                    break;
                }
            }
            API.Controllers.ShopPriceController price_ctrl = new API.Controllers.ShopPriceController();
            List<YNShopPrice> price_list = price_ctrl.GetPriceListByBrandId(clothFactoryId, brandId, customerId);

            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                shop.brands_id = js.Serialize(brand_id_list);
                shop_ctrl.SaveChanges();
                if (price_list.Count > 0) {
                    for (var i = 0; i < price_list.Count; i++) {
                        price_list[i].delete_status = API.Controllers.ShopPriceController.delete_status_1;
                    }
                    price_ctrl.SaveChanges();
                }
                transaction.Complete();
            }

            return Json(new { code = 1, message = "成功" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddBrand()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string brandName = Request.QueryString["brandName"];
            YNBanShiBrand brand = new YNBanShiBrand();
            brand.name = brandName;
            if (!string.IsNullOrEmpty(Request.QueryString["remarks"]))
            {
                brand.remarks = Request.QueryString["remarks"];
            }
            brand.create_time = DateTime.Now;
            brand.modify_type = DateTime.Now;
            brand.factory_id = clothFactoryId;
            YNDIY.API.Controllers.FactoryBrandController brand_ctrl = new YNDIY.API.Controllers.FactoryBrandController();
            bool add = brand_ctrl.Create(brand);
            if (add)
            {
                return Json(new { code = 1, message = "成功" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return getLoginJsonMessage(0, "添加品牌失败");
            }
        }
        public JsonResult DeleteBrand()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            YNDIY.API.Controllers.FactoryBrandController brand_ctrl = new YNDIY.API.Controllers.FactoryBrandController();
            YNBanShiBrand brand = brand_ctrl.GetBrandById(id, clothFactoryId);
            if (brand == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            YNDIY.API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            List<YNBanShiProduct> product_list = product_ctrl.GetProductListByBrand(clothFactoryId, id);
            YNDIY.API.Controllers.ShopInfoController shop_ctrl = new API.Controllers.ShopInfoController();
            List<YNShopInfo> shop_list = shop_ctrl.GetShopListByFactoryId(clothFactoryId);
            JavaScriptSerializer js = new JavaScriptSerializer();
            //事务管理,设置事务隔离级别
            TransactionOptions transactionOption = new TransactionOptions();
            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
            {
                for (var i = 0; i < product_list.Count; i++)
                {
                    product_list[i].delete_status = 1;//设置为删除状态
                }
                product_ctrl.SaveChanges();
                for (var i = 0; i < shop_list.Count; i++)
                {
                    if (!String.IsNullOrEmpty(shop_list[i].brands_id))
                    {
                        List<int> brand_list = js.Deserialize<List<int>>(shop_list[i].brands_id);
                        for (var j = 0; j < brand_list.Count; j++)
                        {
                            if (brand_list[j] == brand.id)
                            {
                                brand_list.Remove(j);
                                shop_list[i].brands_id = js.Serialize(brand_list);
                                break;
                            }
                        }
                    }
                }
                shop_ctrl.SaveChanges();
                brand.delete_status = API.Controllers.FactoryBrandController.delete_status_1;
                brand_ctrl.SaveChanges();
                transaction.Complete();
            }

            return Json(new { code = 1, message = "成功" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProduct()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            YNDIY.API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(clothFactoryId, id);
            if (product == null) {
                return getLoginJsonMessage(0, "不存在该产品");
            }
            product.delete_status = API.Controllers.ProductController.delete_status_1;
            product_ctrl.SaveChanges();
            return Json(new { code = 1, message = "成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> list = brand_ctrl.GetAllBrandList(clothFactoryId);
            ViewBag.list = list;
            return View();
        }
        /// <summary>
        /// 产品列表数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProductList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int brand_id = Convert.ToInt32(Request.QueryString["brandId"]);
            string product_name = Request.QueryString["productName"];
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
            List<YNBanShiProduct> productList = product_ctrl.GetProductList(clothFactoryId, brand_id, product_name, pageIndex, pageSize);
            API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> brandList = brand_ctrl.GetAllBrandList(clothFactoryId);
            List<BrandProduct> brandProduct = new List<BrandProduct>();
            for (var i = 0; i < productList.Count; i++) {
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
            int count = product_ctrl.GetProductCount(clothFactoryId, brand_id, product_name);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.list = brandProduct;
            ViewBag.page = page;
            return View();
        }
         /// <summary>
        /// 新建产品
        /// </summary>
        /// <returns></returns>
        public ActionResult NewProduct()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> list = brand_ctrl.GetAllBrandList(clothFactoryId);
            ViewBag.list = list;
            return View();
        }

        public JsonResult AddProduct()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string image = Request.QueryString["image"];
            int brand_id = Convert.ToInt32(Request.QueryString["brandId"]);
            string model_name = Request.QueryString["model"];
            string name = Request.QueryString["name"];
            string format = Request.QueryString["format"];
            string color = Request.QueryString["color"];
            int unit = Convert.ToInt32(Request.QueryString["unit"]);
            decimal price = Convert.ToDecimal(Request.QueryString["price"]);
            int lowerLine = Convert.ToInt32(Request.QueryString["lowerLine"]);//库存下限
            string remarks = Request.QueryString["remark"];
            YNBanShiProduct entity = new YNBanShiProduct();
            entity.factory_id = clothFactoryId;
            entity.image = image;
            API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            YNBanShiBrand brand = brand_ctrl.GetBrandById(brand_id, clothFactoryId);
            if (brand == null)
            {
                return getLoginJsonMessage(0, "该商户没有该类品牌");
            }
            entity.brand_id = brand_id;
            entity.model_name = model_name;
            entity.produce_name = name;
            entity.format = format;
            entity.color = color;
            entity.unit = unit;
            entity.standard_price = price;
            entity.safe_num = lowerLine;
            entity.remarks = remarks;
            entity.create_time = DateTime.Now;
            entity.modify_time = DateTime.Now;
            YNDIY.API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            product_ctrl.Create(entity);
            return getLoginJsonMessage(1, "产品添加成功");
        }
        public JsonResult SaveEditProduct()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            string image = Request.QueryString["image"];
            int brand_id = Convert.ToInt32(Request.QueryString["brandId"]);
            string model_name = Request.QueryString["model"];
            string name = Request.QueryString["name"];
            string format = Request.QueryString["format"];
            string color = Request.QueryString["color"];
            int unit = Convert.ToInt32(Request.QueryString["unit"]);
            decimal price = Convert.ToDecimal(Request.QueryString["price"]);
            int lowerLine = Convert.ToInt32(Request.QueryString["lowerLine"]);//库存下限
            string remarks = Request.QueryString["remark"];
            API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            YNBanShiProduct entity = product_ctrl.GetProductById(clothFactoryId, id);
            if (entity == null) {
                return getLoginJsonMessage(0, "不存在该产品");
            }
            entity.factory_id = clothFactoryId;
            entity.image = image;
            entity.brand_id = brand_id;
            entity.model_name = model_name;
            entity.produce_name = name;
            entity.format = format;
            entity.color = color;
            entity.unit = unit;
            entity.standard_price = price;
            entity.safe_num = lowerLine;
            entity.remarks = remarks;
            entity.modify_time = DateTime.Now;
            product_ctrl.SaveChanges();
            return getLoginJsonMessage(1, "修改产品信息成功");
        }

        /// <summary>
        /// 编辑产品
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProduct()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            YNBanShiProduct product = product_ctrl.GetProductById(clothFactoryId, id);
            API.Controllers.FactoryBrandController brand_ctrl = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> list = brand_ctrl.GetAllBrandList(clothFactoryId);
            ViewBag.list = list;
            ViewBag.product = product;
            return View();
        }

        /// <summary>
        /// 获取产品部件信息
        /// </summary>
        /// <returns></returns>
        public JsonResult getPartsInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            List<ProductParts> partsList = new List<ProductParts>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if(!string.IsNullOrEmpty(yNBanShiProduct.parts_info)){
                partsList = js.Deserialize<List<ProductParts>>(yNBanShiProduct.parts_info);
            }
            return Json(new { code = 1, message = "成功", partsList = partsList }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取产品部件信息
        /// </summary>
        /// <returns></returns>
        public JsonResult savePartsInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.Form["productId"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            string partsList = Request.Form["partsList"];
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<ProductParts> parts_info = js.Deserialize<List<ProductParts>>(partsList);
            yNBanShiProduct.parts_info = js.Serialize(parts_info);
            productController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }
        /// <summary>
        /// 编辑包号
        /// </summary>
        /// <returns></returns>
        public ActionResult EditBag()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            List<ProductParts> partsList = new List<ProductParts>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(yNBanShiProduct.parts_info))
            {
                partsList = js.Deserialize<List<ProductParts>>(yNBanShiProduct.parts_info);
            }
            List<ProductPackage> packageList = new List<ProductPackage>();
            if (!string.IsNullOrEmpty(yNBanShiProduct.package_info))
            {
                packageList = js.Deserialize<List<ProductPackage>>(yNBanShiProduct.package_info);
            }
            API.Controllers.FactoryBrandController factoryBrandController = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> brandList = factoryBrandController.GetAllBrandList(clothFactoryId);
            ViewBag.brandList = brandList;
            ViewBag.partsList = partsList;
            ViewBag.packageList = packageList;
            ViewBag.yNBanShiProduct = yNBanShiProduct;
            ViewBag.factory_id = clothFactoryId;
            return View();
        }
        /// <summary>
        /// 获取产品包信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPackageInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            List<ProductPackage> partsList = new List<ProductPackage>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(yNBanShiProduct.package_info))
            {
                partsList = js.Deserialize<List<ProductPackage>>(yNBanShiProduct.package_info);
            }
            return Json(new { code = 1, message = "成功", data = partsList }, JsonRequestBehavior.AllowGet);
        }
        /// 查询可供关联的产品
        public JsonResult GetRelationProducts()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string serarchKey = Request.QueryString["searchKey"];
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            List<YNBanShiProduct> product_list = productController.GetProductModelLikeName(clothFactoryId, serarchKey);
            List<YNBanShiProduct> temp_list = new List<YNBanShiProduct>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            for (var i = 0; i < product_list.Count; i++)
            {
                if (!String.IsNullOrEmpty(product_list[i].package_info))
                {
                    List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(product_list[i].package_info);
                    var _is_relation_bag = false;
                    for (var j = 0; j < package_list.Count; j++)
                    {
                        if (package_list[j].relation_id != 0)
                        {
                            _is_relation_bag = true;
                            break;
                        }
                    }
                    if (!_is_relation_bag)
                    {
                        temp_list.Add(product_list[i]);//可供关联的包必须没有关联包
                    }
                }
            }
            return Json(new { code = 1, message = "成功", data = temp_list }, JsonRequestBehavior.AllowGet);
        }
        /// 保存包号相关信息
        public JsonResult saveBag()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.Form["id"]);
            API.Controllers.ProductController product_ctrl = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = product_ctrl.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            string packageList = Request.Form["packageList"];
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<ProductPackage> package_info = js.Deserialize<List<ProductPackage>>(packageList);
            bool has_relation_bag = false;
            for (var i = 0; i < package_info.Count; i++)
            {
                if (package_info[i].relation_id != 0)
                {
                    has_relation_bag = true;
                    YNBanShiProduct relation_product = product_ctrl.GetProductById(clothFactoryId, package_info[i].relation_id);
                    if (String.IsNullOrEmpty(relation_product.package_info))
                    {
                        return getLoginJsonMessage(0, relation_product.produce_name + "产品还未编辑包信息，不能被关联");
                    }
                    List<ProductPackage> temp_package_info = js.Deserialize<List<ProductPackage>>(relation_product.package_info);
                    for (var j = 0; j < temp_package_info.Count; j++)
                    {
                        if (temp_package_info[j].relation_id != 0)
                        {
                            return getLoginJsonMessage(0, relation_product.produce_name + "产品关联其他产品，不能被关联");
                        }
                    }
                }
            }
            if (has_relation_bag)
            {
                List<YNBanShiProduct> all_product_list = product_ctrl.GetAllProductByFactoryId(clothFactoryId);
                for (var i = 0; i < all_product_list.Count; i++)
                {
                    if (!String.IsNullOrEmpty(all_product_list[i].package_info))
                    {
                        List<ProductPackage> temp_package_list = js.Deserialize<List<ProductPackage>>(all_product_list[i].package_info);
                        for (var j = 0; j < temp_package_list.Count; j++)
                        {
                            if (temp_package_list[j].relation_id == productId)
                            {
                                return getLoginJsonMessage(0, "该产品已经被" + all_product_list[i].produce_name + "关联，不能再编辑关联其他产品");
                            }
                        }
                    }
                }
            }
            yNBanShiProduct.package_info = js.Serialize(package_info);
            yNBanShiProduct.packing_info = "";
            product_ctrl.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }
        //获取工序数据
        public JsonResult getProcdureList()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            List<ProductProcess> list = new List<ProductProcess>();
            List<ProductProcess> packing_list = new List<ProductProcess>();
            List<ProductProcess> try_list = new List<ProductProcess>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(yNBanShiProduct.process_info)) {
                list = js.Deserialize<List<ProductProcess>>(yNBanShiProduct.process_info);
            }
            //打包工序
            if (!String.IsNullOrEmpty(yNBanShiProduct.packing_info))
            {
                packing_list = js.Deserialize<List<ProductProcess>>(yNBanShiProduct.packing_info);
            }
            if (packing_list.Count == 0)
            {
                if (String.IsNullOrEmpty(yNBanShiProduct.package_info)) {
                    return getLoginJsonMessage(0, "请先编辑产品包号");
                }
                List<ProductPackage> package_list = js.Deserialize<List<ProductPackage>>(yNBanShiProduct.package_info);
                for (var i = 0; i < package_list.Count; i++) {
                    ProductProcess process = new ProductProcess();
                    process.id = i + 1000;
                    process.name = "打包" + package_list[i].packageNumber;
                    process.price = 0;
                    process.stage = "";
                    process.stage_id = -1;
                    process.type = 1;//打包类型
                    packing_list.Add(process);
                }
            }
            //试装工序
            if (!String.IsNullOrEmpty(yNBanShiProduct.try_process_info)) {
                try_list = js.Deserialize<List<ProductProcess>>(yNBanShiProduct.try_process_info);
            }
            if (try_list.Count == 0) {
                ProductProcess process = new ProductProcess();
                process.id = 2000;
                process.name = "试装";
                process.price = 0;
                process.stage = "";
                process.stage_id = -1;
                process.type = 2;//试装类型
                try_list.Add(process);
            }
            for (var i = 0; i < packing_list.Count; i++) {
                list.Add(packing_list[i]);
            }
            for (var i = 0; i < try_list.Count; i++) {
                list.Add(try_list[i]);
            }
            return Json(new { code = 1, message = "成功", data = list.OrderBy(o => o.id) }, JsonRequestBehavior.AllowGet);
        }
        /// 编辑工序
        public ActionResult EditProcedure()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            API.Controllers.FactoryBrandController factoryBrandController = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> brandList = factoryBrandController.GetAllBrandList(clothFactoryId);
            API.Controllers.GongDuanController gongduan_ctrl = new API.Controllers.GongDuanController();
            List<YNGongDuan> gongduan_list = gongduan_ctrl.GetList(clothFactoryId);
            ViewBag.gongduan_list = gongduan_list;
            ViewBag.brandList = brandList;
            ViewBag.yNBanShiProduct = yNBanShiProduct;
            return View();
        }
        /// 保存工序相关信息
        public JsonResult saveProcedure()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.Form["id"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            string processList = Request.Form["processList"];
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<ProductProcess> produre_info = js.Deserialize<List<ProductProcess>>(processList);
            if (produre_info.Count == 0) {
                return getLoginJsonMessage(0, "参数错误");
            }
            List<ProductProcess> packing_list = new List<ProductProcess>();
            List<ProductProcess> process_list = new List<ProductProcess>();
            List<ProductProcess> try_list = new List<ProductProcess>();
            for (var i = 0; i < produre_info.Count; i++) {
                if (produre_info[i].type == 0)
                {
                    process_list.Add(produre_info[i]);
                }
                else if (produre_info[i].type == 1)
                {
                    packing_list.Add(produre_info[i]);
                }
                else {
                    try_list.Add(produre_info[i]);
                }
            }
            yNBanShiProduct.packing_info = js.Serialize(packing_list);
            yNBanShiProduct.process_info = js.Serialize(process_list);
            yNBanShiProduct.try_process_info = js.Serialize(try_list);
            productController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }
        /// 产品详情
        public ActionResult ProductDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int productId = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProductController productController = new API.Controllers.ProductController();
            YNBanShiProduct yNBanShiProduct = productController.GetProductById(clothFactoryId, productId);
            if (yNBanShiProduct == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            List<ProductParts> partsList = new List<ProductParts>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (!string.IsNullOrEmpty(yNBanShiProduct.parts_info))
            {
                partsList = js.Deserialize<List<ProductParts>>(yNBanShiProduct.parts_info);
            }
            List<ProductProcess> processList = new List<ProductProcess>();//普通工序
            if (!string.IsNullOrEmpty(yNBanShiProduct.process_info))
            {
                processList = js.Deserialize<List<ProductProcess>>(yNBanShiProduct.process_info);
            }
            List<ProductProcess> packing_process_list = new List<ProductProcess>();//打包工序
            if (!string.IsNullOrEmpty(yNBanShiProduct.packing_info))
            {
                packing_process_list = js.Deserialize<List<ProductProcess>>(yNBanShiProduct.packing_info);
            }
            List<ProductProcess> try_process_list = new List<ProductProcess>();
            if (!String.IsNullOrEmpty(yNBanShiProduct.try_process_info)) {
                try_process_list = js.Deserialize<List<ProductProcess>>(yNBanShiProduct.try_process_info);
            }
            for (var i = 0; i < packing_process_list.Count; i++) {//合并工序
                processList.Add(packing_process_list[i]);
            }
            for (var i = 0; i < try_process_list.Count; i++) {
                processList.Add(try_process_list[i]);
            }
            List<ProductPackage> packageList = new List<ProductPackage>();
            if (!string.IsNullOrEmpty(yNBanShiProduct.package_info))
            {
                packageList = js.Deserialize<List<ProductPackage>>(yNBanShiProduct.package_info);
            }
            API.Controllers.FactoryBrandController factoryBrandController = new API.Controllers.FactoryBrandController();
            List<YNBanShiBrand> brandList = factoryBrandController.GetAllBrandList(clothFactoryId);
            ViewBag.brandList = brandList;
            ViewBag.partsList = partsList;
            ViewBag.processList = processList;
            ViewBag.yNBanShiProduct = yNBanShiProduct;
            ViewBag.packageList = packageList;
            ViewBag.factory_id = clothFactoryId;
            return View();
        }
        /// 工段列表
        public ActionResult ProcedureList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        ///工段列表数据
        public ActionResult GetProcedureList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.GongDuanController gongduan_ctrl = new API.Controllers.GongDuanController();
            List<YNGongDuan> list = gongduan_ctrl.GetList(clothFactoryId);
            ViewBag.list = list;
            return View();
        }
        //添加工段
        public JsonResult AddGongDuan()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            if (String.IsNullOrEmpty(Request.QueryString["name"]))
            {
                return getLoginJsonMessage(0, "工段名不能为空");
            }
            string name = Request.QueryString["name"];
            YNGongDuan gongduan = new YNGongDuan();
            gongduan.factory_id = clothFactoryId;
            gongduan.name = name;
            if (!String.IsNullOrEmpty(Request.QueryString["remarks"])) {
                gongduan.remarks = Request.QueryString["remarks"];
            }
            gongduan.create_time = DateTime.Now;
            gongduan.modify_time = gongduan.create_time;

            API.Controllers.GongDuanController gongduan_ctrl = new API.Controllers.GongDuanController();
            gongduan_ctrl.Create(gongduan);           
            return getLoginJsonMessage(1, "成功");
        }
    }
}
