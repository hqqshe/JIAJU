using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using System.IO;
using System.Text;
using System.Web;

namespace YNDIY.Admin.Controllers
{
    public class ERPSupplierController : ParentController
    {
        public static string[] fileExt = { "xls" };
        /// <summary>
        /// 供应商列表(展示)
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        /// <summary>
        /// 获取供应商列表(ajax)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSupplierList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
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
            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            List<YNSupplier> supplierList = supplierController.getSupplierList(clothFactoryId, key, pageIndex, pageSize);
            int count = supplierController.getSupplierListCount(clothFactoryId, key);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.supplierList = supplierList;
            ViewBag.page = page;
            return View();
        }

        /// <summary>
        /// 添加供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult AddSupplier()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }

        /// <summary>
        /// 编辑供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult EditSupplier()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            YNSupplier yNSupplier = supplierController.GetSupplierById(id, clothFactoryId);
            if (yNSupplier == null)
            {
                return loginRerirect();
            }
            ViewBag.yNSupplier = yNSupplier;
            return View();
        }

        /// <summary>
        /// 保存操作
        /// </summary>
        /// <returns></returns>
        public JsonResult doAddSupplier()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = 0;
            if (!string.IsNullOrEmpty(Request.Form["id"]))
            {
                id = Convert.ToInt32(Request.Form["id"]);
            }
            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            YNSupplier yNSupplier = new YNSupplier();
            if (id != 0)
            {
                yNSupplier = supplierController.GetSupplierById(id, clothFactoryId);
                if (yNSupplier == null)
                {
                    return getLoginJsonMessage(0, "参数错误");
                }
            }
            yNSupplier.supplier_name = Request.Form["supplier_name"];
            if (string.IsNullOrEmpty(yNSupplier.supplier_name))
            {
                return getLoginJsonMessage(0, "供应商名称不能为空");
            }
            yNSupplier.link_phone = Request.Form["link_phone"];
            yNSupplier.link_name = Request.Form["link_name"];
            yNSupplier.link_address = Request.Form["link_address"];
            yNSupplier.supplier_goods = Request.Form["supplier_goods"];
            yNSupplier.jiesuan = Request.Form["jiesuan"];
            yNSupplier.remarks = Request.Form["remarks"];
            if (id != 0)
            {
                yNSupplier.modify_time = DateTime.Now;
                supplierController.SaveChanges();
                return getLoginJsonMessage(1, "保存成功");
            }
            else
            {
                yNSupplier.create_time = DateTime.Now;
                yNSupplier.modify_time = DateTime.Now;
                supplierController.Create(yNSupplier,clothFactoryId);
                return getLoginJsonMessage(1, "添加成功");
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public JsonResult delete()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登录");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            supplierController.Delete(id, clothFactoryId);
            return getLoginJsonMessage(1, "删除成功");
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
            HSSFWorkbook hSSFWorkbook = InitializeWorkbook("YNDIY_MATERIAL_SUPPLIER.xls");

            string filename = "原材料供应商模版.xls";
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

            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            HSSFWorkbook hSSFWorkbook = InitializeWorkbookNew(directoryPath + generateFileName);
            var sheet1 = hSSFWorkbook.GetSheetAt(0);
            int index = 5;
            List<string> messageList = new List<string>();
            while (true)
            {
                var row = sheet1.GetRow(index++);
                if (row == null)
                {
                    break;
                }
                YNSupplier yNSupplier = new YNSupplier();
                yNSupplier.jiaju_factory_id = clothFactoryId;
                yNSupplier.supplier_name = Convert.ToString(row.GetCell(0));
                if (string.IsNullOrEmpty(yNSupplier.supplier_name))
                {
                    string temp = "第" + index + "行【原材料供应商名称为空】上传失败";
                    messageList.Add(temp);
                    break;
                }
                if (supplierController.existSupplierByName(0, yNSupplier.supplier_name, clothFactoryId))
                {
                    string temp = "第" + index + "行【原材料供应商名称已经存在】上传失败";
                    messageList.Add(temp);
                    continue;
                }
                yNSupplier.link_name = Convert.ToString(row.GetCell(1));
                yNSupplier.link_phone = Convert.ToString(row.GetCell(2));
                yNSupplier.link_address = Convert.ToString(row.GetCell(3));
                yNSupplier.supplier_goods = Convert.ToString(row.GetCell(4));
                yNSupplier.jiesuan = Convert.ToString(row.GetCell(5));
                yNSupplier.remarks = Convert.ToString(row.GetCell(6));
                yNSupplier.create_time = DateTime.Now;
                yNSupplier.modify_time = DateTime.Now;
                supplierController.Create(yNSupplier, clothFactoryId);
            }
            hSSFWorkbook.Close();
            return Json(new { code = 1, message = "上传结束", data = messageList }, JsonRequestBehavior.AllowGet);

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
    }
}
