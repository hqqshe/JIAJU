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
    public class ERPCustomerController : ParentController
    {
        public static string[] fileExt = { "xls" };
        /// <summary>
        /// 客户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomerList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
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
        /// 客户详情
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            int id = Convert.ToInt32(Request.QueryString["id"]);
            ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(id, clothFactoryId);
            if (shop == null) {
                return getLoginJsonMessage(0, "该客户不存在");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            FactoryBrandController brand_ctrl = new FactoryBrandController();
            List<int> brands_id = new List<int>();
            if (!String.IsNullOrEmpty(shop.brands_id)) {
                brands_id = js.Deserialize<List<int>>(shop.brands_id);
            }
            List<YNBanShiBrand> brand_list = brand_ctrl.GetBrandListInBrandIDList(clothFactoryId, brands_id);

            ViewBag.brands_list = brand_list;
            ViewBag.yNShopInfo = shop;
            return View();
        }

        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <returns></returns>
        public JsonResult CustomerDetailJson()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                id = Convert.ToInt32(Request.QueryString["id"]);
            }
            //0表示自建，1表示系统
            int type = Convert.ToInt32(Request.QueryString["type"]);
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            YNShopInfo yNShopInfo = null;
            if (type == 0)
            {
                yNShopInfo = shopInfoController.GetShopInfoByFactoryId(id, clothFactoryId);
            }
            else
            {
                API.Controllers.FactoryShopRelationController factoryShopRelationController = new API.Controllers.FactoryShopRelationController();
                if (!factoryShopRelationController.checkExist(id, clothFactoryId))
                {
                    return getLoginJsonMessage(0, "数据不存在");
                }
                yNShopInfo = shopInfoController.GetShopInfoByID(id);
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("customer_name", yNShopInfo.shop_name);
            data.Add("link_name", yNShopInfo.link_man);
            data.Add("link_phone", yNShopInfo.phone);
            data.Add("link_address", yNShopInfo.address_detail);
            data.Add("fandian", yNShopInfo.fandian);
            data.Add("special_price", yNShopInfo.special_price);
            data.Add("tags", yNShopInfo.tags);
            data.Add("jiesuan", yNShopInfo.jiesuan);
            data.Add("piaoju", yNShopInfo.piaoju);
            data.Add("order_requires", yNShopInfo.order_requires);
            data.Add("huoyun", yNShopInfo.huoyun);
            data.Add("huoyun_phone", yNShopInfo.huoyun_phone);
            data.Add("fahuo", yNShopInfo.fahuo);
            data.Add("remarks", yNShopInfo.remarks);
            data.Add("post_code", yNShopInfo.post_code);
            return Json(new { code = 1, message = "成功", data = data }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomerInfoJson()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = 0;
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                return getLoginJsonMessage(0, "请求地址错误");
            }
            id = Convert.ToInt32(Request.QueryString["id"]);
            ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(id, clothFactoryId);
            if (shop == null)
            {
                return getLoginJsonMessage(0, "请求参数错误");
            }
            return Json(new { code = 1, message = "成功", data = shop }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerListJson()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            ShopInfoController shop_ctrl = new ShopInfoController();
            List<YNShopInfo> shop_list = shop_ctrl.GetShopListByFactoryId(clothFactoryId);
            return Json(new { code = 1, message = "成功", data = shop_list }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存客户详情
        /// </summary>
        /// <returns></returns>
        public JsonResult saveCustomer()
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
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            YNShopInfo yNShopInfo = new YNShopInfo();
            if (id != 0)
            {
                yNShopInfo = shopInfoController.GetShopInfoByFactoryId(id,clothFactoryId);
                if (yNShopInfo == null)
                {
                    return getLoginJsonMessage(0, "参数错误");
                }
            }
            yNShopInfo.cloth_factory_id = clothFactoryId;
            yNShopInfo.shop_name = Request.Form["customer_name"];
            if (string.IsNullOrEmpty(yNShopInfo.shop_name))
            {
                return getLoginJsonMessage(0, "客户名称不能为空");
            }
            if (shopInfoController.existShopByName(id, yNShopInfo.shop_name, clothFactoryId))
            {
                return getLoginJsonMessage(0, "客户名称已经存在，不能重复添加");
            }
            yNShopInfo.link_man = Request.Form["link_name"];
            yNShopInfo.phone = Request.Form["link_phone"];
            yNShopInfo.address_detail = Request.Form["link_address"];
            yNShopInfo.fandian = Request.Form["fandian"];
            yNShopInfo.special_price = Request.Form["special_price"];
            yNShopInfo.tags = Request.Form["tags"];
            yNShopInfo.jiesuan = Request.Form["jiesuan"];
            yNShopInfo.piaoju = Request.Form["piaoju"];
            yNShopInfo.order_requires = Request.Form["order_requires"];
            yNShopInfo.huoyun = Request.Form["huoyun"];
            yNShopInfo.huoyun_phone = Request.Form["huoyun_phone"];
            yNShopInfo.fahuo = Request.Form["fahuo"];
            yNShopInfo.remarks = Request.Form["remarks"];
            yNShopInfo.post_code = Request.Form["post_code"];
            yNShopInfo.is_examine = Convert.ToInt32(Request.Form["is_examine"]);
            yNShopInfo.is_lockStore = Convert.ToInt32(Request.Form["is_lockStore"]);
            yNShopInfo.credit_limit = Convert.ToDecimal(Request.Form["credit_limit"]);

            string[] id_arr = Request.Form["brand_id"].Split(",");
            List<int> id_list = new List<int>();
            for (var i = 0; i < id_arr.Length; i++)
            {
                id_list.Add(Convert.ToInt32(id_arr[i]));
            }
            id_list = id_list.Where((x, i) => id_list.FindIndex(f => f == x) == i).ToList();//去重

            if (id == 0)
            {
                yNShopInfo.brands_id = new JavaScriptSerializer().Serialize(id_list);
                yNShopInfo.create_time = DateTime.Now;
                yNShopInfo.modify_time = DateTime.Now;
                shopInfoController.Create(yNShopInfo);
            }
            else
            {
                if (String.IsNullOrEmpty(yNShopInfo.brands_id))
                {
                    yNShopInfo.brands_id = new JavaScriptSerializer().Serialize(id_list);
                    shopInfoController.SaveChanges();
                }
                else
                {
                    List<int> originalList = new JavaScriptSerializer().Deserialize<List<int>>(yNShopInfo.brands_id);
                    List<int> _to_delete_id = new List<int>();
                    for (var i = 0; i < originalList.Count; i++)
                    {
                        var _is_same = false;
                        for (var j = 0; j < id_list.Count; j++)
                        {
                            if (id_list[j] == originalList[i])
                            {
                                _is_same = true;
                                break;
                            }
                        }
                        if (!_is_same)
                        {
                            _to_delete_id.Add(originalList[i]);
                        }
                    }
                    yNShopInfo.brands_id = new JavaScriptSerializer().Serialize(id_list);
                    //删除 客户价格表中的数据（条件是 价格表中的 brand_id  在 _to_delete_id这个list里面）
                    API.Controllers.ShopPriceController price_ctrl = new ShopPriceController();
                    List<YNShopPrice> price_list = price_ctrl.GetCustomerPriceListIdInBrandList(clothFactoryId, _to_delete_id, yNShopInfo.id);
                    //事务管理,设置事务隔离级别
                    TransactionOptions transactionOption = new TransactionOptions();
                    transactionOption.IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
                    using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                    {
                        for (var i = 0; i < price_list.Count; i++)
                        {
                            price_list[i].delete_status = ShopPriceController.delete_status_1;
                            price_list[i].modify_time = DateTime.Now;
                        }
                        price_ctrl.SaveChanges();
                        yNShopInfo.modify_time = DateTime.Now;
                        shopInfoController.SaveChanges();
                        transaction.Complete();
                    }
                }
            }
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteCustomer()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            shopInfoController.DeleteByFactoryId(id, clothFactoryId);
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 获取客户订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCustomerOrderList()
        {
            //if (!checkSession())
            //{
            //    return loginHtmlMessage();
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            ////订单类型，0表示定制订单，1表示大货订单
            ////int type = Convert.ToInt32(Request.QueryString["type"]);
            //int shopId = Convert.ToInt32(Request.QueryString["shopId"]);
            //string key = Request.QueryString["searchKey"];
            //int status = Convert.ToInt32(Request.QueryString["status"]);
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
            //API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            //List<YNShopInfo> shopList = shopInfoController.getShopInfoList(shopId, "", 1, 100);
            //List<int> shopIdList = new List<int>();
            //shopIdList.Add(shopId);
            //for (int i = 0; i < shopList.Count; i++)
            //{
            //    shopIdList.Add(shopList[i].id);
            //}
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = factoryOrderController.getFactoryOrderListByFactory(key, clothFactoryId, shopId, status,API.Controllers.FactoryOrderController.type_all, pageIndex, pageSize, shopIdList, 0, 0,0, startTime, endTime);
            //int count = factoryOrderController.getFactoryOrderListByFactoryCount(key, clothFactoryId, shopId, status,API.Controllers.FactoryOrderController.type_all, shopIdList, 0, 0,0, startTime, endTime);
            //API.Controllers.PagesController page = new API.Controllers.PagesController();
            //page.GetPage(pageIndex, count, pageSize);
            //ViewBag.orderList = orderList;
            //ViewBag.page = page;
            return View();
        }

        /// <summary>
        /// 新增客户
        /// </summary>
        /// <returns></returns>
        public ActionResult NewCustomer()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }

        /// <summary>
        /// 编辑客户
        /// </summary>
        /// <returns></returns>
        public ActionResult EditCustomer()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }

        /// <summary>
        /// 下载模版
        /// </summary>
        /// <returns></returns>
        public FileResult downloadTemplate()
        {
            if (!checkSession())
            {
                return null;
            }
            HSSFWorkbook hSSFWorkbook = InitializeWorkbook("YNDIY_CUSTOMER.xls");

            string filename = "客户上传模版.xls";
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            Response.Clear();

            MemoryStream file = new MemoryStream();
            hSSFWorkbook.Write(file);
            Response.BinaryWrite(file.GetBuffer());
            Response.End();
            hSSFWorkbook.Close();
            file.Close();
            return null;
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        private HSSFWorkbook InitializeWorkbook(string fileName)
        {
            string filePath = Server.MapPath("~/Content/" + fileName);
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            HSSFWorkbook hSSFWorkbook = new HSSFWorkbook(file);
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hSSFWorkbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hSSFWorkbook.SummaryInformation = si;
            return hSSFWorkbook;
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        private HSSFWorkbook InitializeWorkbookNew(string filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            HSSFWorkbook hSSFWorkbook = new HSSFWorkbook(file);
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hSSFWorkbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hSSFWorkbook.SummaryInformation = si;
            return hSSFWorkbook;
        }

        /// <summary>
        /// 上传原材料
        /// </summary>
        /// <returns></returns>
        public JsonResult uploadTemplate()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            HttpPostedFileBase uploadFile = Request.Files[0];
            //上传文件扩展名
            string uploadExt = uploadFile.FileName.Split('.').Last<string>();
            if (!fileExt.Contains(uploadExt))
            {
                return getLoginJsonMessage(0, "文件格式不正确，请重新上传");
            }
            string directoryPath = "D:\\jiajuFactoryData\\" + clothFactoryId + "\\";
            //不存在文件夹则创建文件夹
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string generateFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + uploadExt;
            using (FileStream fs = System.IO.File.Create(directoryPath + generateFileName))
            {
                //保存数据
                SaveFile(uploadFile.InputStream, fs);
            }

            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            HSSFWorkbook hSSFWorkbook = InitializeWorkbookNew(directoryPath + generateFileName);
            var sheet1 = hSSFWorkbook.GetSheetAt(0);
            int index = 5;
            while (true)
            {
                var row = sheet1.GetRow(index++);
                if (row == null)
                {
                    break;
                }
                YNShopInfo yNShopInfo = new YNShopInfo();
                yNShopInfo.cloth_factory_id = clothFactoryId;
                yNShopInfo.shop_name = Convert.ToString(row.GetCell(0));
                if (string.IsNullOrEmpty(yNShopInfo.shop_name))
                {
                    hSSFWorkbook.Close();
                    return getLoginJsonMessage(1, "第" + (index - 1) + "行数据为空，上传结束");
                }
                if (shopInfoController.existShopByName(0, yNShopInfo.shop_name, clothFactoryId))
                {
                    //return getLoginJsonMessage(0, "客户名称已经存在，不能重复添加");
                    continue;
                }
                yNShopInfo.link_man = Convert.ToString(row.GetCell(1));
                yNShopInfo.phone = Convert.ToString(row.GetCell(2));
                yNShopInfo.address_detail = Convert.ToString(row.GetCell(3));
                yNShopInfo.post_code = Convert.ToString(row.GetCell(4));
                yNShopInfo.fandian = Convert.ToString(row.GetCell(5));
                yNShopInfo.special_price = Convert.ToString(row.GetCell(6));
                yNShopInfo.tags = Convert.ToString(row.GetCell(7));
                yNShopInfo.jiesuan = Convert.ToString(row.GetCell(8));
                yNShopInfo.piaoju = Convert.ToString(row.GetCell(9));
                yNShopInfo.order_requires = Convert.ToString(row.GetCell(10));
                yNShopInfo.huoyun = Convert.ToString(row.GetCell(11));
                yNShopInfo.huoyun_phone = Convert.ToString(row.GetCell(12));
                yNShopInfo.fahuo = Convert.ToString(row.GetCell(13));
                yNShopInfo.remarks = Convert.ToString(row.GetCell(14));
                yNShopInfo.create_time = DateTime.Now;
                yNShopInfo.modify_time = DateTime.Now;
                shopInfoController.Create(yNShopInfo);
            }
            hSSFWorkbook.Close();
            return getLoginJsonMessage(1, "上传结束");

        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="fs">文件流</param>
        private void SaveFile(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
            }
        }
        /// 客户品牌list
        public ActionResult CustBrandList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int shopId = Convert.ToInt32(Request.QueryString["id"]);
            int brandId = Convert.ToInt32(Request.QueryString["brandId"]);
            ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(shopId, clothFactoryId);
            if (shop == null)
            {
                return getLoginJsonMessage(0, "不存在该客户");
            }
            List<YNBanShiProduct> product_list = new List<YNBanShiProduct>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<int> brands_id = new List<int>();
            if (!string.IsNullOrEmpty(shop.brands_id))
            {
                if (brandId == 100)
                {
                    brands_id = js.Deserialize<List<int>>(shop.brands_id);
                }
                else
                {
                    brands_id.Add(brandId);
                }
            }
            if (!string.IsNullOrEmpty(shop.brands_id))
            {
                ProductController product_ctrl = new ProductController();
                product_list = product_ctrl.GetProductListInBrandList(clothFactoryId, brands_id);
            }
            FactoryBrandController brand_ctrl = new FactoryBrandController();
            List<YNBanShiBrand> brand_list = brand_ctrl.GetBrandListInBrandIDList(clothFactoryId, brands_id);

            List<CustomerProductPriceModel> price_model_list = new List<CustomerProductPriceModel>();
            ShopPriceController price_ctrl = new ShopPriceController();
            List<YNShopPrice> price_list = price_ctrl.GetCustomerPriceListIdInBrandList(clothFactoryId, brands_id, shopId);
            for (var i = 0; i < product_list.Count; i++)
            {
                CustomerProductPriceModel model = new CustomerProductPriceModel();
                model.product = product_list[i];
                for (var j = 0; j < brand_list.Count; j++)
                {
                    if (brand_list[j].id == product_list[i].brand_id)
                    {
                        model.brand = brand_list[j];
                        break;
                    }
                }
                model.price = product_list[i].standard_price;
                for (var j = 0; j < price_list.Count; j++)
                {
                    if (price_list[j].product_id == product_list[i].id)
                    {
                        model.price = price_list[j].price;
                        break;
                    }
                }
                price_model_list.Add(model);
            }
            ViewBag.list = price_model_list;
            return View();
        }
        /// 客户订单list
        public ActionResult CustOrderList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int shopId = Convert.ToInt32(Request.QueryString["id"]);
            int pageIndex = 0;
            int pageSize = 10;
            if (!String.IsNullOrEmpty(Request.QueryString["pageIndex"])) {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            if (!String.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            string startTime = Request.QueryString["startTime"];
            string endTime = Request.QueryString["endTime"];
            string searchKey = Request.QueryString["searchKey"];
            int orderStatus = Convert.ToInt32(Request.QueryString["orderStatus"]);
            ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(shopId, clothFactoryId);
            if (shop == null) {
                return getLoginJsonMessage(0, "客户参数错误");
            }
            FactoryOrderController order_ctrl = new FactoryOrderController();
            List<YNFactoryOrder> list = order_ctrl.GetCustomerOrderList(clothFactoryId, shop.id, orderStatus, startTime, endTime, searchKey, pageIndex, pageSize);
            int count = order_ctrl.GetCustomerOrderListCount(clothFactoryId, shop.id, orderStatus, startTime, endTime, searchKey);
            PagesController page = new PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.page = page;
            ViewBag.list = list;
            return View();
        }
        /// 编辑产品价格
        public ActionResult EditProPrice()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int customer_id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ShopInfoController shop_ctrl = new ShopInfoController();
            YNShopInfo shop = shop_ctrl.GetMyShopInfoByID(customer_id, clothFactoryId);

            List<YNBanShiBrand> brand_list = new List<YNBanShiBrand>();
            List<int> count_list = new List<int>();
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (!String.IsNullOrEmpty(shop.brands_id)) {
                List<int> brand_id_list = js.Deserialize<List<int>>(shop.brands_id);
                API.Controllers.FactoryBrandController brand_ctrl = new FactoryBrandController();
                brand_list = brand_ctrl.GetBrandListInBrandIDList(clothFactoryId, brand_id_list);
                API.Controllers.ProductController product_ctrl = new ProductController();
                for (var i = 0; i < brand_list.Count; i++)
                {
                    count_list.Add(product_ctrl.GetProductCount(clothFactoryId, brand_list[i].id, ""));
                }
            }

            ViewBag.brand_list = brand_list;
            ViewBag.count_list = count_list;
            ViewBag.shop = shop;
            return View();
        }
    }
}
