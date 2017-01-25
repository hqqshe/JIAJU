using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Text;
using System.Configuration;
using System.Net.Mail;

namespace YNDIY.Admin.Controllers
{
    public class ParentController : Controller
    {
        //3D配置信息的店铺
        public static int[] _3DShopId = new int[] { 1, 15 };
        /// <summary>
        /// 登录操作的异步请求数据格式
        /// </summary>
        /// <param name="code">0：失败，1:成功</param>
        /// <param name="message">提示信息</param>
        /// <param name="id">出错对应的元素id</param>
        /// <param name="redirectUrl">成功后的跳转地址</param>
        /// <returns></returns>
        protected JsonResult getLoginJsonMessage(int code, string message, string id = null, string redirectUrl = null)
        {
            return Json(new { code = code, message = message, id = id, redirectUrl = redirectUrl }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 有json数据返回函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult getDataJsonMessage(int code, string message, Dictionary<object, object> data = null, List<Dictionary<object, object>> list = null)
        {
            return Json(new { code = code, message = message, data = data, list = list }, JsonRequestBehavior.AllowGet);
        }

        //根据面料编号和面料单位获取
        protected ResultObject getFabricPrice(string fabric, int UnitType, int fabricType)
        {
            //面料编号为空
            if (string.IsNullOrEmpty(fabric))
            {
                return new ResultObject(0, "面料编号不能为空");
            }
            //面料编号个数不对
            if (fabric.Length != 8)
            {
                return new ResultObject(0, "面料编号错误");
            }
            fabric = fabric.ToUpper();
            int fabricTypeTemp = 0;
            //国产面料
            if (fabric.Contains("G"))
            {
                fabricTypeTemp = 1;
                if (fabricTypeTemp != fabricType)
                {
                    return new ResultObject(0, "当前面料编号为国产面料，面料厂商选择错误");
                }
            }
            //进口面料
            else if (fabric.Contains("M"))
            {
                fabricTypeTemp = 2;
                if (fabricTypeTemp != fabricType)
                {
                    return new ResultObject(0, "当前面料编号为进口面料，面料厂商选择错误");
                }
            }
            else
            {
                return new ResultObject(0, "面料编号错误");
            }
            //单位为米
            decimal price = Convert.ToDecimal(fabric.Substring(2, 3));
            //单位为码
            if (UnitType == 1)
            {
                price = price * Convert.ToDecimal(0.9144);
                price = Convert.ToDecimal(price.ToString("0.00"));
            }
            return new ResultObject(1, "成功", price, fabricTypeTemp);
        }
        /// <summary>
        /// 面料单位转化
        /// </summary>
        /// <param name="oldUnit"></param>
        /// <param name="newUnit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected decimal convertFabricUnit(int oldUnit, int newUnit, decimal value)
        {
            if (oldUnit == newUnit)
            {
                return value;
            }
            //米转化为码
            if (oldUnit == 0)
            {
                decimal newValue = Convert.ToDecimal(1.0936) * value;
                return Convert.ToDecimal(newValue.ToString("0.00"));
            }
            //码转化为米
            else if (oldUnit == 1)
            {
                decimal newValue = Convert.ToDecimal(0.9144) * value;
                return Convert.ToDecimal(newValue.ToString("0.00"));
            }
            return value;
        }

        /// <summary>
        /// 获取json数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JsonResult getDataJsonMessage(int code, string message,object data,object liangTiData = null)
        {
            return Json(new { code = code, message = message, data = data, liangTiData = liangTiData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 判断用户是否已经登录
        /// </summary>
        /// <returns></returns>
        protected bool checkSession()
        {
            
            if (Session["UserID"] != null)
            {
                int type = Convert.ToInt32(Session["Type"]);
                int roleType = Convert.ToInt32(Session["RoleType"]);
                //管理员账号
                if (type == YNDIY.API.Controllers.ShopUserController.type_0)
                {
                    //string _referrer = Request.Url.AbsolutePath;
                    //if ("/admin/shopInfoList".Equals(_referrer) || "/admin/fabricFactoryList".Equals(_referrer) || "/admin/clothFactoryList".Equals(_referrer) || "/admin/FactoryShopRelation".Equals(_referrer))
                    //{
                    //    return true;
                    //}
                    //return false;
                }
                //商家账号
                else if (type == YNDIY.API.Controllers.ShopUserController.type_1)
                {
                }
                //面料厂商账号
                else if (type == YNDIY.API.Controllers.ShopUserController.type_2)
                {
                }
                //工厂账号
                else if (type == YNDIY.API.Controllers.ShopUserController.type_3)
                {
                    string _referrer = Request.Url.AbsolutePath;
                    //if (string.IsNullOrEmpty(_referrer) || "/".Equals(_referrer) || "/Home/Index".Equals(_referrer))
                    //{
                    //    Session["_reffer_uri"] = "/ERPOrder/OrderList";
                    //}
                    if ("/ERPprocess/ScanCode".Equals(_referrer) || "/ERPprocess/getCodeInfo".Equals(_referrer) || "/ERPprocess/submitBarCode".Equals(_referrer) || "/ERPprocess/MobileStatistics".Equals(_referrer) || "/ERPprocess/GetMobileStatisticsItems".Equals(_referrer) || "/ERPprocess/getEmployeeInfo".Equals(_referrer) || "/ERPprocess/GroupScanCode".Equals(_referrer))
                    {
                        if (roleType != API.Controllers.ShopUserController.role_type1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (roleType != API.Controllers.ShopUserController.role_type0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

           
            //读取cookie值
            HttpCookie cookie = Request.Cookies["_admin_younidz"];
            if (cookie == null)
            {
                return false;
            }
            string value = cookie.Value;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            LoginUserInfo loginUserInfo = null;
            try
            {
                string desc = API.Controllers.EncryptionController.DESDecrypt(value);
                JavaScriptSerializer js = new JavaScriptSerializer();
                loginUserInfo = js.Deserialize<LoginUserInfo>(desc);
            }
            catch (Exception e)
            {
                return false;
            }
            long currentTime = DateTime.Now.ToFileTime();
            //值为空或者已经过期，需要重新登录
            if (loginUserInfo == null || currentTime > loginUserInfo.ExpireTime)
            {
                return false;
            }
            Session["UserId"] = loginUserInfo.UserId;
            Session["Account"] = loginUserInfo.Account;
            API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
            YNShopUser yNShopUser = shopUserController.GetUserByID(loginUserInfo.UserId);
            Session["NickName"] = yNShopUser.nick_name;
            Session["Type"] = loginUserInfo.Type;
            Session["RoleType"] = loginUserInfo.RoleType;
            Session["Role3D"] = 0;
            //管理员账号
            if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_0)
            {
                Session["logo"] = "http://files.younidz.com/content/n3/20160501121952050.png";
                Session["Name"] = "管理员";
                //string _referrer = Request.Url.AbsolutePath;
                //if ("/admin/shopInfoList".Equals(_referrer) || "/admin/fabricFactoryList".Equals(_referrer) || "/admin/clothFactoryList".Equals(_referrer) || "/admin/FactoryShopRelation".Equals(_referrer))
                //{
                //    return true;
                //}
                //return false;
            }
            //商家账号
            else if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_1)
            {
                Session["ShopId"] = loginUserInfo.ShopId;
                Session["ParentShopId"] = loginUserInfo.ParentShopId;
                API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
                if (loginUserInfo.RoleType == YNDIY.API.Controllers.ShopUserController.role_type0)
                {
                    YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(loginUserInfo.ShopId);
                    Session["logo"] = yNShopInfo.logo;
                    Session["Name"] = yNShopInfo.shop_name;
                    if (_3DShopId.Contains(loginUserInfo.ShopId))
                    {
                        Session["Role3D"] = 1;
                    }
                }
                else
                {
                    YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(loginUserInfo.ParentShopId);
                    Session["logo"] = yNShopInfo.logo;
                    Session["Name"] = yNShopInfo.shop_name;
                    if (_3DShopId.Contains(loginUserInfo.ParentShopId))
                    {
                        Session["Role3D"] = 1;
                    }
                }
            }
            //面料厂商账号
            else if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_2)
            {
                //Session["FabricFactoryId"] = loginUserInfo.FabricFactoryId;
                //API.Controllers.FabricFactoryInfoController fabricFactoryInfoController = new API.Controllers.FabricFactoryInfoController();
                //YNFabricFactoryInfo yNFabricFactoryInfo = fabricFactoryInfoController.GetFabricFactoryInfoByID(loginUserInfo.FabricFactoryId);
                //Session["logo"] = "http://files.younidz.com/content/n3/20160501121952050.png";
                //Session["Name"] = yNFabricFactoryInfo.fabric_factory_name;
            }
            //工厂账号
            else if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_3)
            {
                Session["ClothFactoryId"] = loginUserInfo.ClothFactoryId;
                API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
                YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(loginUserInfo.ClothFactoryId);
                Session["logo"] = "http://files.younidz.com/content/n3/20160501121952050.png";
                Session["Name"] = yNClothFactoryInfo.cloth_factory_name;
                Session["ClothFactoryBarType"] = yNClothFactoryInfo.bar_code_type;
                Session["ClothFactoryEnableWorksteps"] = yNClothFactoryInfo.enable_worksteps;
                string _referrer = Request.Url.AbsolutePath;
                //if (string.IsNullOrEmpty(_referrer) || "/".Equals(_referrer) || "/Home/Index".Equals(_referrer))
                //{
                //    Session["_reffer_uri"] = "/ERPOrder/OrderList";
                //}
                if ("/ERPprocess/ScanCode".Equals(_referrer) || "/ERPprocess/getCodeInfo".Equals(_referrer) || "/ERPprocess/submitBarCode".Equals(_referrer) || "/ERPprocess/MobileStatistics".Equals(_referrer) || "/ERPprocess/GetMobileStatisticsItems".Equals(_referrer) || "/ERPprocess/getEmployeeInfo".Equals(_referrer) || "/ERPprocess/GroupScanCode".Equals(_referrer))
                {
                    if (loginUserInfo.RoleType != API.Controllers.ShopUserController.role_type1)
                    {
                        return false;
                    }
                }
                else
                {
                    if (loginUserInfo.RoleType != API.Controllers.ShopUserController.role_type0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 判断3D权限
        /// </summary>
        /// <returns></returns>
        protected ResultObject check3DAuth()
        {
            if (!checkSession())
            {
                return new ResultObject(0, "请登陆");
            }
            int shopId = Convert.ToInt32(Session["ShopId"]);
            int parentShopId = Convert.ToInt32(Session["ParentShopId"]);
            //不是管理员账号则替换成管理员账号
            if (parentShopId != 0)
            {
                shopId = parentShopId;
            }
            if (!_3DShopId.Contains(shopId))
            {
                return new ResultObject(0, "当前店铺没有权限");
            }
            return new ResultObject(1, "成功",shopId);
        }

        /// <summary>
        /// 写入登录cookie值
        /// </summary>
        /// <param name="yNUser"></param>
        protected void writeCookie(YNShopUser yNUser)
        {
            HttpCookie cookie = new HttpCookie("_admin_younidz");
            JavaScriptSerializer js = new JavaScriptSerializer();
            LoginUserInfo loginUserInfo = new LoginUserInfo();
            loginUserInfo.UserId = yNUser.id;
            loginUserInfo.Account = yNUser.account;
            //中文字符会出现乱码，所以不写入cookie中
            //loginUserInfo.NickName = yNUser.nick_name;
            loginUserInfo.Type = yNUser.type;
            loginUserInfo.RoleType = yNUser.role_type;
            loginUserInfo.ShopId = yNUser.shop_id;
            loginUserInfo.ParentShopId = yNUser.parent_shop_id;
            loginUserInfo.FabricFactoryId = yNUser.fabric_factory_id;
            loginUserInfo.ClothFactoryId = yNUser.cloth_factory_id;
            loginUserInfo.ExpireTime = DateTime.Now.AddDays(30).ToFileTime();
            string value = js.Serialize(loginUserInfo);
            string enValue = API.Controllers.EncryptionController.DESEncrypt(value);
            cookie.Value = enValue;
            //管理员账号
            if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_0)
            {
                cookie.Expires = DateTime.Now.AddDays(15);
            }
            //商家账号
            else if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_1)
            {
                //商家管理员账号
                if (loginUserInfo.RoleType == API.Controllers.ShopUserController.role_type0)
                {
                    cookie.Expires = DateTime.Now.AddHours(1);
                }
                else
                {
                    cookie.Expires = DateTime.Now.AddDays(15);
                }
            }
            //面料厂商账号
            else if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_2)
            {
                cookie.Expires = DateTime.Now.AddDays(15);
            }
            //衣服加工厂账号
            else if (loginUserInfo.Type == YNDIY.API.Controllers.ShopUserController.type_3)
            {
                cookie.Expires = DateTime.Now.AddDays(15);
            }
            
            cookie.Domain = ".younidz.com";
            Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 清除cooki操作
        /// </summary>
        protected void clearCookie()
        {
            //清除cookie
            HttpCookie cookie = new HttpCookie("_admin_younidz");
            cookie.Value = "";
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Domain = ".younidz.com";
            Response.Cookies.Add(cookie);
            //清除session
            Session["UserId"] = null;
            Session["Account"] = "";
            Session["NickName"] = "";
            Session["Type"] = "";
            Session["RoleType"] = "";
            Session["ShopId"] = "";
            Session["ParentShopId"] = "";
            Session["FabricFactoryId"] = "";
            Session["ClothFactoryId"] = "";
            Session["logo"] = "";
            Session["Name"] = "";
            Session["Role3D"] = "";
            Session["ClothFactoryBarType"] = "";
            Session["_reffer_uri"] = "";
        }

        /// <summary>
        /// 跳转到登录页
        /// </summary>
        /// <returns></returns>
        protected RedirectResult loginRerirect()
        {
            Session["_reffer_uri"] = Request.Url.AbsolutePath;
            return Redirect("/Login/Login");
        }

        /// <summary>
        /// 异步获取html数据时消息
        /// </summary>
        /// <returns></returns>
        protected ActionResult loginHtmlMessage()
        {
            Session["_reffer_uri"] = Request.UrlReferrer.AbsolutePath;
            return Redirect("/Login/TimeoutLogin");
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="url">URL地址</param>
        /// <param name="data">请求参数</param>
        /// <param name="method">POST,GET请求方式</param>
        /// <param name="referer">来源地址</param>
        /// <returns>返回结果</returns>
        protected string SendData(string url, string data = "", string method = "POST", string referer = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            //设置上一个跳转页
            request.Referer = referer;
            request.ContentType = "application/x-www-form-urlencoded";
            if (method == "POST")
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
            }
            //开始请求
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            //获取数据
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 创建默认账号信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        protected YNShopUser CreateUser(string account, string password, string phone)
        {
            //获取文件服务器域名
            string filesDomain = ConfigurationManager.AppSettings["FilesDomain"];
            YNShopUser user = new YNShopUser();
            user.account = account;
            user.password = API.Controllers.EncryptionController.MD5(password + API.Controllers.TokenController.DIYToken);
            //默认昵称使用帐号名称
            user.nick_name = account;
            //默认为商家账号
            user.type = API.Controllers.ShopUserController.type_1;
            user.phone = phone;
            user.email = "";
            //正常用户
            user.status = API.Controllers.ShopUserController.status_0;
            user.create_time = DateTime.Now;
            user.modify_time = DateTime.Now;
            return user;
        }
       /// <summary>
       /// 发送邮箱
       /// </summary>
       /// <param name="recieveUserMail"></param>
       /// <param name="sendUserMail"></param>
       /// <param name="password"></param>
       /// <param name="subject"></param>
       /// <param name="userName"></param>
       /// <param name="content"></param>
       /// <returns></returns>
        private bool SendMail(string recieveUserMail,string sendUserMail,string password,string subject,string userName,string content)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            //收件人
            msg.To.Add(recieveUserMail);
            //3个参数分别是发件人地址（可以随便写），发件人姓名，编码
            msg.From = new MailAddress(sendUserMail, userName, System.Text.Encoding.UTF8);
            msg.Subject = subject;//邮件标题 
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码 
            msg.Body = content;//邮件内容 
            msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码 
            msg.IsBodyHtml = false;//是否是HTML邮件 
            msg.Priority = MailPriority.High;//邮件优先级 
            SmtpClient client = new SmtpClient();
            //邮箱和密码 
            client.Credentials = new System.Net.NetworkCredential(sendUserMail, password);
            //设置邮箱host
            string host = "smtp."+ sendUserMail.Split('@')[1];
            client.Host = host;
            //object userState = msg;
            try
            {
                //非阻塞方式发送邮件
                //client.SendAsync(msg, userState); 
                //阻塞方式发送邮件
                client.Send(msg);
                return true;
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 发送短信信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        protected bool SendMessage(string message,string phone)
        {
            //string phoneCode = API.Controllers.TokenController.GenerateTokenNumber();
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(message))
            {
                return false;
            }
            string[] phoneList = phone.Split(',');
            for (int i = 0; i < phoneList.Length; i++)
            {
                string mobile = phoneList[i];
                if (string.IsNullOrEmpty(mobile))
                {
                    continue;
                }
                string tplValue = HttpUtility.UrlEncode("#app#=由你定制&#code#=" + message);
                try
                {
                    string recvData = this.SendData("http://v.juhe.cn/sms/send?mobile=" + mobile + "&tpl_id=1157&tpl_value=" + tplValue + "&key=37627d666d854812e1f0b7e50a70d22b", "", "GET");
                    //JsonResult jsonData = Json(recvData, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {

                }
            }
            return true;
        }
        /// <summary>
        /// 查询某个产品是否在待盘点确认
        /// </summary>
        /// <param name="product_id">产品ID</param>
        /// <returns></returns>
        public bool CheckProductInventory(int product_id)
        {
            int factory_id = Convert.ToInt32(Session["ClothFactoryId"]);
            YNDIY.API.Controllers.InventoryController inventory_ctrl = new API.Controllers.InventoryController();
            YNInventory inventory = inventory_ctrl.GetInventoryByProductId(factory_id, product_id);
            if (inventory == null)
            {
                return false;
            }
            return true;
        }
    }
}
