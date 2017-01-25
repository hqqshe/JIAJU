using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    /// <summary>
    /// 管理员管理商家，面料商，衣服加工厂商
    /// </summary>
    public class AdminController : ParentController
    {
        /// <summary>
        /// 工厂店铺关系
        /// </summary>
        /// <returns></returns>
        public ActionResult FactoryShopRelation()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return View();
            }
            return View();
        }

        public ActionResult GetFactoryShopRelation()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return View();
            }
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
            API.Controllers.FactoryShopRelationController factoryShopRelationController = new API.Controllers.FactoryShopRelationController();
            List<YNFactoryShopRelation> relationList = factoryShopRelationController.getRelationListByAdmin(searchKey, searchType, pageIndex, pageSize);
            int count = factoryShopRelationController.getRelationListByAdminCount(searchKey, searchType);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.page = page;
            ViewBag.relationList = relationList;
            return View();
        }
        //删除关联
        public JsonResult deleteRelation()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return getLoginJsonMessage(0, "权限不够");
            }
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.FactoryShopRelationController factoryShopRelationController = new API.Controllers.FactoryShopRelationController();
            factoryShopRelationController.Detete(id);
            return getLoginJsonMessage(1, "删除成功");
        }
        //添加关联
        public JsonResult addRelation()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return getLoginJsonMessage(0, "权限不够");
            }
            string shopAccount = Request.Form["shopAccount"];
            string factoryAccount = Request.Form["factoryAccount"];
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser shopUser = shopUserController.GetUserByAccount(shopAccount);
            if (shopUser == null)
            {
                return getLoginJsonMessage(0, "商家账号不正确");
            }
            if (shopUser.parent_shop_id != 0)
            {
                return getLoginJsonMessage(0, "商家账号不是主帐号");
            }
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(shopUser.shop_id);
            if (yNShopInfo == null)
            {
                return getLoginJsonMessage(0, "商家信息不存在");
            }
            YNShopUser factoryUser = shopUserController.GetUserByAccount(factoryAccount);
            if (factoryUser == null)
            {
                return getLoginJsonMessage(0, "工厂账号不正确");
            }
            if (factoryUser.role_type != API.Controllers.ShopUserController.role_type0)
            {
                return getLoginJsonMessage(0, "工厂账号不是主帐号");
            }
            API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
            YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(factoryUser.cloth_factory_id);
            if (yNClothFactoryInfo == null)
            {
                return getLoginJsonMessage(0, "工厂信息不存在");
            }
            API.Controllers.FactoryShopRelationController factoryShopRelationController = new API.Controllers.FactoryShopRelationController();
            if (factoryShopRelationController.checkExist(yNShopInfo.id, yNClothFactoryInfo.id))
            {
                return getLoginJsonMessage(0, "关联信息已经存在，不能重复添加");
            }
            YNFactoryShopRelation yNFactoryShopRelation = new YNFactoryShopRelation();
            yNFactoryShopRelation.factory_id = yNClothFactoryInfo.id;
            yNFactoryShopRelation.factory_user_id = factoryUser.id;
            yNFactoryShopRelation.factory_user_account = factoryUser.account;
            yNFactoryShopRelation.factory_name = yNClothFactoryInfo.cloth_factory_name;
            yNFactoryShopRelation.factory_link_man = yNClothFactoryInfo.link_man;
            yNFactoryShopRelation.factory_link_phone = yNClothFactoryInfo.phone;
            yNFactoryShopRelation.factory_link_address = yNClothFactoryInfo.address_detail;
            yNFactoryShopRelation.shop_id = yNShopInfo.id;
            yNFactoryShopRelation.shop_user_id = shopUser.id;
            yNFactoryShopRelation.shop_user_account = shopUser.account;
            yNFactoryShopRelation.shop_name = yNShopInfo.shop_name;
            yNFactoryShopRelation.shop_link_man = yNShopInfo.link_man;
            yNFactoryShopRelation.shop_link_phone = yNShopInfo.phone;
            yNFactoryShopRelation.shop_link_address = yNShopInfo.address_detail;
            yNFactoryShopRelation.create_time = DateTime.Now;
            yNFactoryShopRelation.modify_time = DateTime.Now;
            factoryShopRelationController.Create(yNFactoryShopRelation);
            return getLoginJsonMessage(1, "添加成功");
        }

        /// <summary>
        /// 商家列表
        /// </summary>
        /// <returns></returns>
        public ActionResult shopInfoList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return View();
            }
            return View();
        }
        public ActionResult shopInfoListItems()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return View();
            }
            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            string searchKey = Request.QueryString["searchKey"];
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            List<YNShopUser> shopUserList = shopUserController.getShopUserListAdmin(searchKey, API.Controllers.ShopUserController.type_1, pageIndex, pageSize);
            int count = shopUserController.getShopUserListAdminCount(searchKey, API.Controllers.ShopUserController.type_1);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.shopUserList = shopUserList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 衣服加工厂列表
        /// </summary>
        /// <returns></returns>
        public ActionResult clothFactoryList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return View();
            }
            return View();
        }
        public ActionResult clothFactoryListItems()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
                API.Controllers.PagesController page = new API.Controllers.PagesController();
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                page.GetPage(1, 0, 20);
                ViewBag.shopUserList = new  List<YNShopUser> ();
                ViewBag.page = page;
                return View();
            }
            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            string searchKey = Request.QueryString["searchKey"];
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            List<YNShopUser> shopUserList = shopUserController.getShopUserListAdmin(searchKey, API.Controllers.ShopUserController.type_3, pageIndex, pageSize);
            int count = shopUserController.getShopUserListAdminCount(searchKey, API.Controllers.ShopUserController.type_3);
            //API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.shopUserList = shopUserList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 面料厂商列表
        /// </summary>
        /// <returns></returns>
        public ActionResult fabricFactoryList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return View();
            }
            return View();
        }
        public ActionResult fabricFactoryListItems()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return View();
            }
            int pageIndex = 1;
            int pageSize = 20;
            if (!string.IsNullOrEmpty(Request.QueryString["pageIndex"]))
            {
                pageIndex = Convert.ToInt32(Request.QueryString["pageIndex"]);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["pageSize"]))
            {
                pageSize = Convert.ToInt32(Request.QueryString["pageSize"]);
            }
            string searchKey = Request.QueryString["searchKey"];
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            List<YNShopUser> shopUserList = shopUserController.getShopUserListAdmin(searchKey, API.Controllers.ShopUserController.type_2, pageIndex, pageSize);
            int count = shopUserController.getShopUserListAdminCount(searchKey, API.Controllers.ShopUserController.type_2);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.shopUserList = shopUserList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 根据id获取商家或者工厂信息
        /// </summary>
        /// <returns></returns>
        public JsonResult getShopUserById()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0,"请登陆");
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return getLoginJsonMessage(0, "不是管理员账号，不能操作");
            }
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetUserByID(userId);
            if (yNShopUser == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            if (!string.IsNullOrEmpty(yNShopUser.r_p_d))
            {
                yNShopUser.password = API.Controllers.EncryptionController.DESPWDecrypt(yNShopUser.r_p_d);
            }
            else
            {
                yNShopUser.password = "";
            }
            return getDataJsonMessage(1, "成功", yNShopUser);
        }
        /// <summary>
        /// 根据id删除工厂或者面料商的信息
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteShopUser()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return getLoginJsonMessage(0, "不是管理员账号，不能操作");
            }
            int userId = Convert.ToInt32(Request.QueryString["userId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetUserByID(userId);
            if (yNShopUser == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            //商家
            if (yNShopUser.type == API.Controllers.ShopUserController.type_1)
            {
                API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
                shopInfoController.Delete(yNShopUser.shop_id);
                shopUserController.Delete(yNShopUser);
            }
            //面料商
            else if (yNShopUser.type == API.Controllers.ShopUserController.type_2)
            {
                //API.Controllers.FabricFactoryInfoController fabricFactoryInfoController = new API.Controllers.FabricFactoryInfoController();
                //fabricFactoryInfoController.Delete(yNShopUser.fabric_factory_id);
                //shopUserController.Delete(yNShopUser);
            }
            //衣服加工厂
            else if (yNShopUser.type == API.Controllers.ShopUserController.type_3)
            {
                API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
                clothFactoryInfoController.Delete(yNShopUser.cloth_factory_id);
                shopUserController.Delete(yNShopUser);
            }
            return getLoginJsonMessage(1, "删除成功");
        }
        /// <summary>
        /// 保存或者新增商家和厂家信息
        /// </summary>
        /// <returns></returns>
        public JsonResult saveShopUser()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            //判断是否是管理员账号
            int type = Convert.ToInt32(Session["Type"]);
            if (type != API.Controllers.ShopUserController.type_0)
            {
                return getLoginJsonMessage(0, "不是管理员账号，不能操作");
            }
            string userIdStr = Request.Form["userId"];
            int userId = 0;
            if(!string.IsNullOrEmpty(userIdStr)){
                userId = Convert.ToInt32(userIdStr);
            }
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
            //API.Controllers.FabricFactoryInfoController fabricFactoryInfoController = new API.Controllers.FabricFactoryInfoController();
            API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
            YNShopUser yNShopUser = new YNShopUser();
            if (userId != 0)
            {
                yNShopUser = shopUserController.GetUserByID(userId);
                if (yNShopUser == null)
                {
                    return getLoginJsonMessage(0, "参数错误");
                }
            }
            yNShopUser.account = Request.Form["account"];
            yNShopUser.phone = Request.Form["phone"];
            yNShopUser.password = Request.Form["password"];
            if (string.IsNullOrEmpty(yNShopUser.account) || string.IsNullOrEmpty(yNShopUser.password))
            {
                return getLoginJsonMessage(0, "账号和密码不能为空");
            }
            if (userId != 0)
            {
                if (shopUserController.GetUserByAccountPhoneExists(yNShopUser.account, yNShopUser.phone,userId))
                {
                    return getLoginJsonMessage(0, "账号和电话不能重复，请重新设置账号");
                }
            }
            else
            {
                if (shopUserController.GetUserByAccountPhoneExists(yNShopUser.account, yNShopUser.phone))
                {
                    return getLoginJsonMessage(0, "账号和电话不能重复，请重新设置账号");
                }
            }
            
            yNShopUser.r_p_d = API.Controllers.EncryptionController.DESPWEncrypt(yNShopUser.password);
            yNShopUser.password = API.Controllers.EncryptionController.MD5(yNShopUser.password + API.Controllers.TokenController.DIYToken);
            yNShopUser.email = Request.Form["email"];
            yNShopUser.link_man = Request.Form["link_man"];
            //yNShopUser.pisition = Request.Form["pisition"];
            //yNShopUser.nick_name = Request.Form["nick_name"];
            //yNShopUser.job_number = Request.Form["job_number"];
            yNShopUser.type = Convert.ToInt32(Request.Form["type"]);
            yNShopUser.role_type = API.Controllers.ShopUserController.role_type0;
            yNShopUser.status = API.Controllers.ShopUserController.status_0;
            //商家
            if (yNShopUser.type == API.Controllers.ShopUserController.type_1)
            {
                yNShopUser.shop_name = Request.Form["shop_name"];
            }
            //面料供应商
            else if (yNShopUser.type == API.Controllers.ShopUserController.type_2)
            {
                yNShopUser.fabric_factory_name = Request.Form["fabric_factory_name"];
            }
            //衣服加工厂
            else if (yNShopUser.type == API.Controllers.ShopUserController.type_3)
            {
                yNShopUser.cloth_factory_name = Request.Form["cloth_factory_name"];
            }
            if (userId != 0)
            {
                yNShopUser.modify_time = DateTime.Now;
                shopUserController.SaveChanges();
                return getLoginJsonMessage(1, "保存成功");
            }
            else
            {
                yNShopUser.create_time = DateTime.Now;
                yNShopUser.modify_time = DateTime.Now;
                //商家
                if (yNShopUser.type == API.Controllers.ShopUserController.type_1)
                {
                    YNShopInfo yNShopInfo = new YNShopInfo();
                    yNShopInfo.shop_name = yNShopUser.shop_name;
                    yNShopInfo.link_man = yNShopUser.link_man;
                    yNShopInfo.phone = yNShopUser.phone;
                    yNShopInfo.email = yNShopUser.email;
                    yNShopInfo.parent_shop_id = 0;
                    yNShopInfo.status = API.Controllers.ShopInfoController.status_0;
                    yNShopInfo.create_time = DateTime.Now;
                    yNShopInfo.modify_time = DateTime.Now;
                    //添加商家信息
                    shopInfoController.Create(yNShopInfo);
                    yNShopUser.shop_id = yNShopInfo.id;
                    shopUserController.Create(yNShopUser);
                    return getLoginJsonMessage(1, "添加商家信息成功");

                }
                //面料供应商
                else if (yNShopUser.type == API.Controllers.ShopUserController.type_2)
                {
                    //YNFabricFactoryInfo yNFabricFactoryInfo = new YNFabricFactoryInfo();
                    //yNFabricFactoryInfo.shop_id = 0;
                    //yNFabricFactoryInfo.fabric_factory_name = yNShopUser.fabric_factory_name;
                    //yNFabricFactoryInfo.link_man = yNShopUser.link_man;
                    //yNFabricFactoryInfo.phone = yNShopUser.phone;
                    //yNFabricFactoryInfo.email = yNShopUser.email;
                    //yNFabricFactoryInfo.status = API.Controllers.FabricFactoryInfoController.status_0;
                    //yNFabricFactoryInfo.type = API.Controllers.FabricFactoryInfoController.type_1;
                    //yNFabricFactoryInfo.show_state = API.Controllers.FabricFactoryInfoController.show_state0;
                    //yNFabricFactoryInfo.create_time = DateTime.Now;
                    //yNFabricFactoryInfo.modify_time = DateTime.Now;
                    ////添加面料商信息
                    //fabricFactoryInfoController.Create(yNFabricFactoryInfo);
                    //yNShopUser.fabric_factory_id = yNFabricFactoryInfo.id;
                    //shopUserController.Create(yNShopUser);
                    return getLoginJsonMessage(1, "添加面料商信息成功");
                }
                //衣服加工厂
                else if (yNShopUser.type == API.Controllers.ShopUserController.type_3)
                {
                    YNClothFactoryInfo yNClothFactoryInfo = new YNClothFactoryInfo();
                    yNClothFactoryInfo.shop_id = 0;
                    yNClothFactoryInfo.cloth_factory_name = yNShopUser.cloth_factory_name;
                    yNClothFactoryInfo.link_man = yNShopUser.link_man;
                    yNClothFactoryInfo.phone = yNShopUser.phone;
                    yNClothFactoryInfo.email = yNShopUser.email;
                    yNClothFactoryInfo.status = API.Controllers.ClothFactoryInfoController.status_0;
                    yNClothFactoryInfo.type = API.Controllers.ClothFactoryInfoController.type_1;
                    yNClothFactoryInfo.show_state = API.Controllers.ClothFactoryInfoController.show_state0;
                    yNClothFactoryInfo.create_time = DateTime.Now;
                    yNClothFactoryInfo.modify_time = DateTime.Now;
                    //添加工厂信息
                    clothFactoryInfoController.Create(yNClothFactoryInfo);
                    yNShopUser.cloth_factory_id = yNClothFactoryInfo.id;
                    shopUserController.Create(yNShopUser);

                    //新建自己为系统客户
                    YNShopInfo yNShopInfo = new YNShopInfo();
                    yNShopInfo.cloth_factory_id = yNClothFactoryInfo.id;
                    yNShopInfo.shop_name = yNShopUser.cloth_factory_name;
                    yNShopInfo.link_man = yNShopUser.link_man;
                    yNShopInfo.phone = yNShopUser.phone;
                    yNShopInfo.email = yNShopUser.email;
                    yNShopInfo.parent_shop_id = 0;
                    yNShopInfo.status = API.Controllers.ShopInfoController.status_0;
                    yNShopInfo.type = API.Controllers.ShopInfoController.type_1;
                    yNShopInfo.create_time = DateTime.Now;
                    yNShopInfo.modify_time = DateTime.Now;
                    //添加商家信息
                    shopInfoController.Create(yNShopInfo);
                    return getLoginJsonMessage(1, "添加衣服加工厂信息成功");
                }
                else
                {
                    return getLoginJsonMessage(0, "参数错误");
                }
            }
        }
    }
}
