using System.Linq;
using System.Web.Mvc;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class WebController : ParentController
    {
        public int UserId { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) &&
                !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                if (!checkSession())
                {
                    filterContext.Result = filterContext.HttpContext.Request.IsAjaxRequest()
                        ? getLoginJsonMessage(0, "请登录") : (ActionResult)loginRerirect();
                }
                else
                {
                    UserId = (int)Session["UserId"];
                }
            }


            if (!ModelState.IsValid)
            {
                var ferror = ModelState.Where(m=>m.Value.Errors.Count > 0).Select(m => m.Value).FirstOrDefault();
                var errorMessage = string.Empty;
                if (ferror != null)
                {
                    var error = ferror.Errors.FirstOrDefault();
                    if (error != null)
                    {
                        errorMessage = error.ErrorMessage;
                    }
                }
                filterContext.Result = JsonFailed(errorMessage);
            }


            base.OnActionExecuting(filterContext);
        }

        protected JsonResult JsonSuccess(object data = null)
        {
            return JsonData(1, "成功", data);
        }

        protected JsonResult JsonFailed(string message, object data = null)
        {
            return JsonData(0, message, data);
        }

        protected JsonResult JsonData(int code, string message, object data = null)
        {
            return Json(new { code = code, message = message, data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}