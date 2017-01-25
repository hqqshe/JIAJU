using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class UploadController : ParentController
    {
        //上传图片资源
        public string _UploadImage()
        {
            if (!checkSession())
            {
                return "0";
            }
            try
            {
                string fileExt = "png";
                API.Controllers.UploadController uploadController = new API.Controllers.UploadController();
                string path = uploadController.UploadNormalPicture(this.Request, this.Server, 2, fileExt);
                return path;
            }
            catch (Exception e)
            {
                return "";
            }
        }
        //上传模型资源
        public string _UploadModel()
        {
            ResultObject result = check3DAuth();
            if (result.code != 1)
            {
                return "0";
            }
            try
            {
                API.Controllers.UploadController uploadController = new API.Controllers.UploadController();
                string path = uploadController.UploadModel(this.Request, this.Server, 2);
                return path;
            }
            catch (Exception e)
            {
                return "";
            }
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadImage()
        {
            return View();
        }

        /// <summary>
        /// 上传模型
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadModel()
        {
            return View();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            return View();
        }

        /// <summary>
        /// 上传家具原材料EXCEL模板
        /// </summary>
        /// <returns></returns>
        public ActionResult ExcelTemplate()
        {
            return View();
        }
        /// <summary>
        /// 上传家具工厂客户EXCEL模板
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerExcelTemplate()
        {
            return View();
        }
        /// <summary>
        /// 上传供应商EXCEL模板
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierExcelTemplate()
        {
            return View();
        }
        /// <summary>
        /// 上传工序EXCEL模板
        /// </summary>
        /// <returns></returns>
        public ActionResult ProcedureExcelTemplate()
        {
            return View();
        }
    }
}