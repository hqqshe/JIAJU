using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class ShopUserController : ParentController
    {
        public ActionResult UserCenter()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            return View();
        }
        /// <summary>
        /// 用户中心
        /// </summary>
        /// <returns></returns>
        public JsonResult getUserInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0,"请登陆");
            }
            int userId = Convert.ToInt32(Session["UserId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetUserByID(userId);
            Dictionary<object, object> dataTemp = new Dictionary<object, object>();
            dataTemp.Add("account", yNShopUser.account);
            dataTemp.Add("phone", yNShopUser.phone);
            dataTemp.Add("email", yNShopUser.email);
            dataTemp.Add("shop_name", yNShopUser.shop_name);
            dataTemp.Add("fabric_factory_name", yNShopUser.fabric_factory_name);
            dataTemp.Add("cloth_factory_name", yNShopUser.cloth_factory_name);
            if (string.IsNullOrEmpty(yNShopUser.nick_name))
            {
                dataTemp.Add("nick_name", yNShopUser.link_man);
            }
            else
            {
                dataTemp.Add("nick_name", yNShopUser.nick_name);
            }
            
            dataTemp.Add("job_number", yNShopUser.job_number);
            dataTemp.Add("position", yNShopUser.position);
            return getDataJsonMessage(1, "成功", dataTemp);
        }
        /// <summary>
        /// 保存密码信息
        /// </summary>
        /// <returns></returns>
        public JsonResult saveUserInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int userId = Convert.ToInt32(Session["UserId"]);
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetUserByID(userId);
            string password = Request.QueryString["password"];
            if (string.IsNullOrEmpty(password))
            {
                return getLoginJsonMessage(0, "密码不能为空");
            }
            yNShopUser.password = password;
            yNShopUser.r_p_d = API.Controllers.EncryptionController.DESPWEncrypt(yNShopUser.password);
            yNShopUser.password = API.Controllers.EncryptionController.MD5(yNShopUser.password + API.Controllers.TokenController.DIYToken);
            yNShopUser.modify_time = DateTime.Now;
            shopUserController.SaveChanges();
            //清除cookie
            clearCookie();
            return getLoginJsonMessage(1, "密码修改成功", null, "/login/Login");
        }
    }
}
