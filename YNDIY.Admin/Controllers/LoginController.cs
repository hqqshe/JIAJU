using System;
using System.Linq;
using System.Web.Mvc;
using YNDIY.API.Models;
using MvcCaptcha;

namespace YNDIY.Admin.Controllers
{
    public class LoginController : ParentController
    { 
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCode()
        {
            return View();
        }
        /// <summary>
        /// 网页登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login() {
            return View();
        }

        /// <summary>
        /// ajax异步加载信息的超时页面
        /// </summary>
        /// <returns></returns>
        public ActionResult TimeoutLogin()
        {
            return View();
        }

        /// <summary>
        /// 网页登录认证
        /// </summary>
        /// <param name="collection">表单元素</param>
        /// <returns>返回页面</returns>
        [AcceptVerbs(HttpVerbs.Post), ValidateMvcCaptcha()]
        [HttpPost]
        public JsonResult WebLogin(FormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return getLoginJsonMessage(0, "验证码输入错误,请重新输入", "_mvcCaptchaText");
                }
                if (string.IsNullOrEmpty(collection["account"]) || string.IsNullOrEmpty(collection["password"]))
                {
                    //参数错误
                    return getLoginJsonMessage(0, "账号或者密码不能为空,请重新输入", "account");
                }
                string account = collection["account"].Trim();
                string password = collection["password"].Trim();
                password = API.Controllers.EncryptionController.MD5(password + API.Controllers.TokenController.DIYToken);
                API.Controllers.ShopUserController userController = new API.Controllers.ShopUserController();
                //帐号登录
                var user = userController.GetUserByAccountPhonePasswd(account, password);
                if (user != null)
                {
                    Session["UserId"] = user.id;
                    Session["Account"] = user.account;
                    Session["NickName"] = user.nick_name;
                    Session["Type"] = user.type;
                    Session["RoleType"] = user.role_type;
                    Session["Role3D"] = 0;
                    //管理员账号
                    if (user.type == YNDIY.API.Controllers.ShopUserController.type_0)
                    {
                        Session["logo"] = "http://files.younidz.com/content/n3/20160501121952050.png";
                        Session["Name"] = "管理员";
                        Session["_reffer_uri"] = "/admin/clothFactoryList";
                    }
                    //商家账号
                    else if (user.type == YNDIY.API.Controllers.ShopUserController.type_1)
                    {
                        Session["ShopId"] = user.shop_id;
                        Session["ParentShopId"] = user.parent_shop_id;
                        API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
                        if (user.role_type == YNDIY.API.Controllers.ShopUserController.role_type0)
                        {
                            YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(user.shop_id);
                            Session["logo"] = yNShopInfo.logo;
                            Session["Name"] = yNShopInfo.shop_name;
                            if (_3DShopId.Contains(user.shop_id))
                            {
                                Session["Role3D"] = 1;
                            }
                        }
                        else
                        {
                            YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(user.parent_shop_id);
                            Session["logo"] = yNShopInfo.logo;
                            Session["Name"] = yNShopInfo.shop_name;
                            if (_3DShopId.Contains(user.parent_shop_id))
                            {
                                Session["Role3D"] = 1;
                            }
                        }
                    }
                    //面料厂商账号
                    else if (user.type == YNDIY.API.Controllers.ShopUserController.type_2)
                    {
                        //Session["FabricFactoryId"] = user.fabric_factory_id;
                        //API.Controllers.FabricFactoryInfoController fabricFactoryInfoController = new API.Controllers.FabricFactoryInfoController();
                        //YNFabricFactoryInfo yNFabricFactoryInfo = fabricFactoryInfoController.GetFabricFactoryInfoByID(user.fabric_factory_id);
                        //Session["logo"] = "http://files.younidz.com/content/n3/20160501121952050.png";
                        //Session["Name"] = yNFabricFactoryInfo.fabric_factory_name;
                    }
                    //工厂账号
                    else if (user.type == YNDIY.API.Controllers.ShopUserController.type_3)
                    {
                        Session["ClothFactoryId"] = user.cloth_factory_id;
                        API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
                        YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(user.cloth_factory_id);
                        Session["logo"] = "http://files.younidz.com/content/n3/20160501121952050.png";
                        Session["Name"] = yNClothFactoryInfo.cloth_factory_name;
                        Session["ClothFactoryBarType"] = yNClothFactoryInfo.bar_code_type;
                        Session["ClothFactoryEnableWorksteps"] = yNClothFactoryInfo.enable_worksteps;

                        string _referrer = Convert.ToString(Session["_reffer_uri"]);
                        if (string.IsNullOrEmpty(_referrer) || "/".Equals(_referrer) || "/Home/Index".Equals(_referrer))
                        {
                            Session["_reffer_uri"] = "/ERPOrder/OrderList";
                        }
                        if ("/ERPprocess/ScanCode".Equals(_referrer) || "/ERPprocess/GroupScanCode".Equals(_referrer))
                        {
                            if (user.role_type != API.Controllers.ShopUserController.role_type1)
                            {
                                Session["UserId"] = null;
                                return getLoginJsonMessage(0, "您不是生产线员工，不能进入扫码页面", null); 
                            }
                        }
                        else
                        {
                            if (user.role_type != API.Controllers.ShopUserController.role_type0)
                            {
                                Session["UserId"] = null;
                               return getLoginJsonMessage(0, "您是生产线员工，没有权限进入", null);
                            }
                        }
                    }
                  
                   
                    writeCookie(user);
                    string referrer = Convert.ToString(Session["_reffer_uri"]);
                    if (string.IsNullOrEmpty(referrer))
                    {
                        referrer = "/";
                    }
                    return getLoginJsonMessage(1, "登录成功", null, referrer);
                }
                //登录失败
                return getLoginJsonMessage(0, "账号或者密码输入错误,请重新输入", "account");
            }
            catch
            {
                //异常错误
                return getLoginJsonMessage(0, "系统异常,请重新登录", "account");
            }
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgetPwd()
        {
            return View();
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <returns></returns>
        public JsonResult changePassword()
        {
            string phone = Request.Form["phone"];
            string phone_code = Request.Form["phone_code"];
            string password = Request.Form["password"];
            if (string.IsNullOrEmpty(phone))
            {
                return getLoginJsonMessage(0, "手机不能为空");
            }
            if (string.IsNullOrEmpty(phone_code))
            {
                return getLoginJsonMessage(0, "手机验证码不能为空");
            }
            if (string.IsNullOrEmpty(password))
            {
                return getLoginJsonMessage(0, "密码不能为空");
            }
            if (!phone.Equals(Session["Phone"]))
            {
                return getLoginJsonMessage(0, "手机号码存在变动");
            }
            if (!phone_code.Equals(Session["PhoneCode"]))
            {
                return getLoginJsonMessage(0, "手机验证码输入错误，请重新输入");
            }
            API.Controllers.ShopUserController userController = new API.Controllers.ShopUserController();
            var yNUser = userController.GetUserByAccount(phone);
            if (yNUser == null)
            {
                return getLoginJsonMessage(0, "当前手机号用户不存在，请重新输入");
            }
            yNUser.password = API.Controllers.EncryptionController.MD5(password + API.Controllers.TokenController.DIYToken);
            yNUser.modify_time = DateTime.Now;
            userController.SaveChanges();
            return getLoginJsonMessage(1, "密码修改成功");
        }

        /// <summary>
        /// 退出登陆操作
        /// </summary>
        /// <returns></returns>
        public ActionResult logout()
        {
            clearCookie();
            if (!string.IsNullOrEmpty(Request.QueryString["url"]))
            {
                Session["_reffer_uri"] = Request.QueryString["url"];
            }
            return Redirect("/Login/Login");
        }
    }
}
