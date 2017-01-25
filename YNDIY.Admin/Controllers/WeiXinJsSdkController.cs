using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace YNDIY.Admin.Controllers
{
    public class WeiXinJsSdkController : ParentController
    {
        public static string access_token_all = "";
        public static string jsapi_ticket_all = "";
        //过期时间
        public static long time_expre = 7200;
        //获取token地址
        public static string access_token_url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=wx0eb2f308e7d7add2&secret=3f608b2ea48cc97779b176d28fde08c1";
        //默认值设置为过期时间
        public static DateTime set_time = DateTime.Now.AddHours(-3);

        /// <summary>
        /// 获取token值和ticket值
        /// </summary>
        /// <returns></returns>
        public  int  getTokenAndTicket()
        {
            string returnStr = SendData(access_token_url, "", "GET");
            if (string.IsNullOrEmpty(returnStr))
            {
                return -1;
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            Dictionary<string, object> data = js.Deserialize<Dictionary<string, object>>(returnStr);
            string access_token = getStringValue(data, "access_token");
            if (string.IsNullOrEmpty(access_token))
            {
                return -2;
            }
            string jsapi_ticket_url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + access_token + "&type=jsapi";
            returnStr = SendData(jsapi_ticket_url, "", "GET");
            if (string.IsNullOrEmpty(returnStr))
            {
                return -3;
            }
            data = js.Deserialize<Dictionary<string, object>>(returnStr);
            string jsapi_ticket = getStringValue(data, "ticket");
            access_token_all = access_token;
            jsapi_ticket_all = jsapi_ticket;
            //增加过期时间，缩短有效期时间
            set_time = DateTime.Now.AddMinutes(-20);
            return 1;
        }
        /// <summary>
        /// 根据地址获取签名
        /// </summary>
        /// <param name="jsapi_ticket"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Dictionary<string, string> sign(string url) 
         {
             DateTime now = DateTime.Now;
             DateTime oldTime = new DateTime(1970, 1, 1);
             double milliseconds = (set_time - oldTime).TotalMilliseconds;
             DateTime guoqi_time = oldTime.AddMilliseconds(milliseconds+(double)(7200 * 1000));
             //过期需要刷新token
             if(now.CompareTo(guoqi_time) > 0){
                 getTokenAndTicket();
             }

            Dictionary<string, string> ret = new Dictionary<string, string>();
            string nonce_str = API.Controllers.TokenController.GenerateUniqueCode(16);
            string timestamp = (DateTime.Now.Millisecond / 1000).ToString();
            string string1;
            string signature = "";
            //注意这里参数名必须全部小写，且必须有序
            string1 = "jsapi_ticket=" + jsapi_ticket_all +
                      "&noncestr=" + nonce_str +
                      "&timestamp=" + timestamp +
                      "&url=" + url;
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] dataToHash = enc.GetBytes(string1);
            byte[] dataHashed = sha1.ComputeHash(dataToHash);
            string hash = BitConverter.ToString(dataHashed).Replace("-", "");
            signature = hash.ToLower();

            ret.Add("url", url);
            ret.Add("jsapi_ticket", jsapi_ticket_all);
            ret.Add("nonceStr", nonce_str);
            ret.Add("timestamp", timestamp);
            ret.Add("signature", signature);

            return ret;
        }

         //public JsonResult test()
         //{
         //   string value = "jsapi_ticket=sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg&noncestr=Wm3WZYTPz0wzccnW&timestamp=1414587457&url=http://mp.weixin.qq.com?params=value";
         //   string url = Request.Url.AbsoluteUri;
         //   SHA1 sha1 = new SHA1CryptoServiceProvider();
         //   ASCIIEncoding enc = new ASCIIEncoding();
         //   byte[] dataToHash = enc.GetBytes(value);
         //   byte[] dataHashed = sha1.ComputeHash(dataToHash);
         //   string hash = BitConverter.ToString(dataHashed).Replace("-", "");
         //   hash = hash.ToLower();
         //   return getLoginJsonMessage(1, hash);
         //}

        //获取string值
        private string getStringValue(Dictionary<string, object> dic, string key)
        {
            if (dic.ContainsKey(key))
            {
                if (dic[key] != null)
                {
                    return dic[key].ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}