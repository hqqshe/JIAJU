using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using System.IO;
using System.Text;

namespace YNDIY.Admin.Controllers
{
    public class ERPMaterialController: ParentController
    {
        public static string[] fileExt = { "xls"};
        /// <summary>
        /// 原材料列表(展示)
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// <summary>
        /// 获取原材料列表(ajax)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMaterialList()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string name = Request.QueryString["name"];
            string xinghao = Request.QueryString["xinghao"];
            string guige = Request.QueryString["guige"];
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
            int type = Convert.ToInt32(Request.QueryString["type"]);
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            List<YNMaterial> materialList = materialController.getMaterialList(clothFactoryId, name,xinghao,guige, type, pageIndex, pageSize);
            int count = materialController.getMaterialListCount(clothFactoryId, name,xinghao,guige,type);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.materialList = materialList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 添加原材料
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMaterial()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            List<YNSupplier> supplierList = supplierController.getSupplierListByFactoryId(clothFactoryId);
            ViewBag.supplierList = supplierList;
            return View();
        }
        /// <summary>
        /// 编辑原材料
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMaterial()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = materialController.GetMaterialById(id, clothFactoryId);
            if (yNMaterial == null)
            {
                return getLoginJsonMessage(0, "数据不存在");
            }
            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            List<YNSupplier> supplierList = supplierController.getSupplierListByFactoryId(clothFactoryId);
            ViewBag.supplierList = supplierList;
            ViewBag.yNMaterial = yNMaterial;
            return View();
        }

        /// <summary>
        /// 库存原材料详情
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialDetail()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = materialController.GetMaterialById(id, clothFactoryId);
            if (yNMaterial == null)
            {
                return getLoginJsonMessage(0, "数据不存在");
            }
            API.Controllers.StorageMaterialController storageMaterialController = new API.Controllers.StorageMaterialController();
            List<YNStorageMaterial> storageMaterialList = storageMaterialController.getStorageMaterialList(yNMaterial.id, clothFactoryId);

            ViewBag.yNMaterial = yNMaterial;
            ViewBag.storageMaterialList = storageMaterialList;
            return View();
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <returns></returns>
        public JsonResult doAddMaterial()
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
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = new YNMaterial();
            if (id != 0)
            {
                yNMaterial = materialController.GetMaterialById(id, clothFactoryId);
                if (yNMaterial == null)
                {
                    return getLoginJsonMessage(0, "参数错误");
                }
            }
            yNMaterial.supplier_id = Convert.ToInt32(Request.Form["supplier_id"]);
            yNMaterial.supplier_name = Request.Form["supplier_name"];
            yNMaterial.image = Request.Form["image"];
            yNMaterial.material_type = Convert.ToInt32(Request.Form["material_type"]);
            yNMaterial.material_name = Request.Form["material_name"];
            if (materialController.existMaterialByName(id, yNMaterial.material_name, clothFactoryId))
            {
                return getLoginJsonMessage(0, "材料名称已经存在，不能重复添加");
            }
            if (string.IsNullOrEmpty(yNMaterial.material_name))
            {
                return getLoginJsonMessage(0, "材料名称不能为空");
            }
            yNMaterial.supplier_model = Request.Form["supplier_model"];
            yNMaterial.factory_model = Request.Form["factory_model"];
            yNMaterial.gui_ge = Request.Form["gui_ge"];
            yNMaterial.unit = Request.Form["unit"];
            yNMaterial.price = Request.Form["price"];
            yNMaterial.color = Request.Form["color"];
            yNMaterial.remarks = Request.Form["remarks"];
            API.Controllers.SupplierController supplierController = new API.Controllers.SupplierController();
            YNSupplier yNSupplier = supplierController.GetSupplierById(yNMaterial.supplier_id, clothFactoryId);
            yNMaterial.phone = yNSupplier.link_phone;
            if (id != 0)
            {
                yNMaterial.modify_time = DateTime.Now;
                materialController.SaveChanges();
                return getLoginJsonMessage(1, "保存成功");
            }
            else
            {
                yNMaterial.create_time = DateTime.Now;
                yNMaterial.modify_time = DateTime.Now;
                yNMaterial.kucun = 0;
                materialController.Create(yNMaterial, clothFactoryId);
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
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            materialController.Delete(id, clothFactoryId);
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
            HSSFWorkbook hSSFWorkbook = InitializeWorkbook("YNDIY_MATERIAL.xls");

            string filename = "原材料模版.xls";
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

            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
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
                YNMaterial yNMaterial = new YNMaterial();
                yNMaterial.material_name = Convert.ToString(row.GetCell(0));
                if (string.IsNullOrEmpty(yNMaterial.material_name))
                {
                    string temp = "第" + index + "行【原材料名称为空】上传失败";
                    messageList.Add(temp);
                    break;
                }
                if (materialController.existMaterialByName(0, yNMaterial.material_name, clothFactoryId))
                {
                    string temp = "第" + index + "行【原材料名称已经存在】上传失败";
                    messageList.Add(temp);
                    continue;
                }
                string supplier_name = Convert.ToString(row.GetCell(8));
                yNMaterial.supplier_name = supplier_name;
                yNMaterial.supplier_id = 0;
                if (!string.IsNullOrEmpty(supplier_name))
                {
                    YNSupplier yNSupplier = supplierController.GetSupplierByName(supplier_name, clothFactoryId);
                    if (yNSupplier != null)
                    {
                        yNMaterial.supplier_name = supplier_name;
                        yNMaterial.supplier_id = yNSupplier.id;
                        yNMaterial.phone = yNSupplier.link_phone;
                    }
                    else
                    {
                        string temp = "第" + index + "行【供应商不存在】上传失败";
                        messageList.Add(temp);
                        continue;
                    }
                }
                else
                {
                    string temp = "第" + index + "行【供应商不存在】上传失败";
                    messageList.Add(temp);
                    continue;
                    //yNMaterial.phone = Convert.ToString(row.GetCell(9));
                }
               
                yNMaterial.image = "";
                
                int material_type = 0;
                string materialTypeStr = Convert.ToString(row.GetCell(1));
                if (materialTypeStr != null && materialTypeStr.Contains("辅"))
                {
                    material_type = 1;
                }
                yNMaterial.material_type = material_type;
                yNMaterial.supplier_model = Convert.ToString(row.GetCell(2));
                yNMaterial.factory_model = Convert.ToString(row.GetCell(3));
                yNMaterial.gui_ge = Convert.ToString(row.GetCell(4));
                yNMaterial.unit = Convert.ToString(row.GetCell(6));
                yNMaterial.price = Convert.ToString(row.GetCell(7));
                yNMaterial.color = Convert.ToString(row.GetCell(5));
                yNMaterial.remarks = Convert.ToString(row.GetCell(10));
                yNMaterial.create_time = DateTime.Now;
                yNMaterial.modify_time = DateTime.Now;
                yNMaterial.kucun = 0;
                materialController.Create(yNMaterial, clothFactoryId);
            }
            hSSFWorkbook.Close();
            return Json(new { code = 1, message = "上传结束",data=messageList}, JsonRequestBehavior.AllowGet);
           
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

        /// <summary>
        /// 原材料库存
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialInventories()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// <summary>
        /// 获取原材料库存列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMaterialInventories()
        {
            if (!checkSession())
            {
                return loginHtmlMessage();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string name = Request.QueryString["name"];
            string xinghao = Request.QueryString["xinghao"];
            string guige = Request.QueryString["guige"];
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
            int type = Convert.ToInt32(Request.QueryString["type"]);
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            List<YNMaterial> materialList = materialController.getMaterialList(clothFactoryId, name, xinghao, guige, type, pageIndex, pageSize);
            int count = materialController.getMaterialListCount(clothFactoryId, name, xinghao, guige, type);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.materialList = materialList;
            ViewBag.page = page;
            return View();
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public JsonResult getMaterialDetail()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = materialController.GetMaterialById(id, clothFactoryId);
            if (yNMaterial == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            API.Controllers.StorageMaterialController storageMaterialController = new API.Controllers.StorageMaterialController();
            List<YNStorageMaterial> storageMaterialList = storageMaterialController.getStorageMaterialList(yNMaterial.id, clothFactoryId);
            return Json(new { code = 1, message = "成功", yNMaterial = yNMaterial, storageMaterialList = storageMaterialList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改最后提交库存记录信息
        /// </summary>
        /// <returns></returns>
        public JsonResult setStorageInfo()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int number = Convert.ToInt32(Request.QueryString["number"]);
            if (number < 0)
            {
                return getLoginJsonMessage(0, "数量不能小于0");
            }
            API.Controllers.StorageMaterialController storageMaterialController = new API.Controllers.StorageMaterialController();
            YNStorageMaterial yNStorageMaterial = storageMaterialController.getStorageMaterialById(id, clothFactoryId);
            if (yNStorageMaterial == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            int oldNumber = yNStorageMaterial.number;
            if (oldNumber == number)
            {
                return getLoginJsonMessage(1, "成功");
            }
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = materialController.GetMaterialById(yNStorageMaterial.material_id, clothFactoryId);
            int sub = number - oldNumber;
            //入库
            if (yNStorageMaterial.type == API.Controllers.StorageMaterialController.type_0)
            {
                yNMaterial.kucun += sub;
                yNMaterial.modify_time = DateTime.Now;
            }
            //出库
            else if (yNStorageMaterial.type == API.Controllers.StorageMaterialController.type_1)
            {
                yNMaterial.kucun -= sub;
                yNMaterial.modify_time = DateTime.Now;
                if (yNMaterial.kucun < 0)
                {
                    return getLoginJsonMessage(0, "出库数量大于库存总量，请重新设置数据");
                }
            }
            //盘点
            else
            {
                yNMaterial.kucun = number;
                yNMaterial.modify_time = DateTime.Now;
            }
            materialController.SaveChanges();
            yNStorageMaterial.number = number;
            yNStorageMaterial.modify_time = DateTime.Now;
            yNStorageMaterial.remarks = Request.QueryString["remarks"];
            storageMaterialController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 材料入库
        /// </summary>
        /// <returns></returns>
        public JsonResult storageIn()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int number = Convert.ToInt32(Request.QueryString["number"]);
            if (number < 0)
            {
                return getLoginJsonMessage(0, "数量不能小于0");
            }
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = materialController.GetMaterialById(id, clothFactoryId);
            if (yNMaterial == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            //已完成入库数量
            int oldKucun = yNMaterial.kucun;
            yNMaterial.kucun += number;
            yNMaterial.modify_time = DateTime.Now;
            materialController.SaveChanges();
            //添加入库记录
            API.Controllers.StorageMaterialController storageMaterialController = new API.Controllers.StorageMaterialController();
            YNStorageMaterial yNStorageMaterial = new YNStorageMaterial();
            yNStorageMaterial.jiaju_factory_id = clothFactoryId;
            yNStorageMaterial.material_id = yNMaterial.id;
            yNStorageMaterial.operator_id = Convert.ToInt32(Session["UserId"]);
            yNStorageMaterial.operator_name = Convert.ToString(Session["NickName"]);
            yNStorageMaterial.type = API.Controllers.StorageMaterialController.type_0;
            yNStorageMaterial.number = number;
            yNStorageMaterial.ext_number = oldKucun;
            yNStorageMaterial.unit = yNMaterial.unit;
            yNStorageMaterial.create_time = DateTime.Now;
            yNStorageMaterial.modify_time = DateTime.Now;
            yNStorageMaterial.remarks = Request.QueryString["remarks"];
            storageMaterialController.Create(yNStorageMaterial, clothFactoryId);
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 材料出库
        /// </summary>
        /// <returns></returns>
        public JsonResult storageOut()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int number = Convert.ToInt32(Request.QueryString["number"]);
            if (number < 0)
            {
                return getLoginJsonMessage(0, "数量不能小于0");
            }
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = materialController.GetMaterialById(id, clothFactoryId);
            if (yNMaterial == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            int oldKuCun = yNMaterial.kucun;
            yNMaterial.kucun -= number;
            if (yNMaterial.kucun < 0)
            {
                return getLoginJsonMessage(0, "出库数量大于库存总量，请重新设置数据");
            }
            yNMaterial.modify_time = DateTime.Now;
            materialController.SaveChanges();
            //添加库出库记录
            API.Controllers.StorageMaterialController storageMaterialController = new API.Controllers.StorageMaterialController();
            YNStorageMaterial yNStorageMaterial = new YNStorageMaterial();
            yNStorageMaterial.jiaju_factory_id = clothFactoryId;
            yNStorageMaterial.material_id = yNMaterial.id;
            yNStorageMaterial.operator_id = Convert.ToInt32(Session["UserId"]);
            yNStorageMaterial.operator_name = Convert.ToString(Session["NickName"]);
            yNStorageMaterial.type = API.Controllers.StorageMaterialController.type_1;
            yNStorageMaterial.number = number;
            yNStorageMaterial.ext_number = oldKuCun;
            yNStorageMaterial.unit = yNMaterial.unit;
            yNStorageMaterial.create_time = DateTime.Now;
            yNStorageMaterial.modify_time = DateTime.Now;
            yNStorageMaterial.remarks = Request.QueryString["remarks"];
            storageMaterialController.Create(yNStorageMaterial, clothFactoryId);

            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 盘点库存
        /// </summary>
        /// <returns></returns>
        public JsonResult storageSet()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int number = Convert.ToInt32(Request.QueryString["number"]);
            if (number < 0)
            {
                return getLoginJsonMessage(0, "数量不能小于0");
            }
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            YNMaterial yNMaterial = materialController.GetMaterialById(id, clothFactoryId);
            if (yNMaterial == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            int oldKuCun = yNMaterial.kucun;
            yNMaterial.kucun = number;
            yNMaterial.modify_time = DateTime.Now;
            materialController.SaveChanges();
            //添加库盘点记录
            API.Controllers.StorageMaterialController storageMaterialController = new API.Controllers.StorageMaterialController();
            YNStorageMaterial yNStorageMaterial = new YNStorageMaterial();
            yNStorageMaterial.jiaju_factory_id = clothFactoryId;
            yNStorageMaterial.material_id = yNMaterial.id;
            yNStorageMaterial.operator_id = Convert.ToInt32(Session["UserId"]);
            yNStorageMaterial.operator_name = Convert.ToString(Session["NickName"]);
            yNStorageMaterial.type = API.Controllers.StorageMaterialController.type_2;
            yNStorageMaterial.number = number;
            yNStorageMaterial.ext_number = oldKuCun;
            yNStorageMaterial.unit = yNMaterial.unit;
            yNStorageMaterial.create_time = DateTime.Now;
            yNStorageMaterial.modify_time = DateTime.Now;
            yNStorageMaterial.remarks = Request.QueryString["remarks"];
            storageMaterialController.Create(yNStorageMaterial, clothFactoryId);

            return getLoginJsonMessage(1, "成功");
        }
    }
}

