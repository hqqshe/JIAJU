using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using YNDIY.API.Controllers;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using System.IO;
using System.Text;
using System.Web;


namespace YNDIY.Admin.Controllers
{
    public class ERPProcessManageController : ParentController
    {
        public static string[] fileExt = { "xls" };
        /// <summary>
        /// 流程列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProcessList()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        /// <summary>
        /// 获取流程列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetProcessList()
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
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            List<YNFactoryProcedure> procedureList = factoryProcedureController.getProcedureList(clothFactoryId, key, API.Controllers.FactoryProcedureController.type_all, pageIndex, pageSize);
            int count = factoryProcedureController.getProcedureListCount(clothFactoryId, key, API.Controllers.FactoryProcedureController.type_all);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.procedureList = procedureList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 编辑流程
        /// </summary>
        /// <returns></returns>
        public ActionResult EditProcess() {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = 0;
            if(!string.IsNullOrEmpty(Request.QueryString["id"])){
                id = Convert.ToInt32(Request.QueryString["id"]);
            }
            ViewBag.id = id;
            return View();
        }
        /// <summary>
        /// 获取流程数据
        /// </summary>
        /// <returns></returns>
        public JsonResult getProcessData()
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
            JavaScriptSerializer js = new JavaScriptSerializer();
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            YNFactoryProcedure yNFactoryProcedure = factoryProcedureController.GetProcedureById(id, clothFactoryId);
            List<FactoryProcedureStepNumber> stepList = new List<FactoryProcedureStepNumber>();
            if (yNFactoryProcedure != null && !string.IsNullOrEmpty(yNFactoryProcedure.step))
            {
                try
                {
                    stepList = js.Deserialize<List<FactoryProcedureStepNumber>>(yNFactoryProcedure.step);
                }
                catch (Exception e)
                {

                }
            }
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("id", yNFactoryProcedure.id);
            data.Add("name", yNFactoryProcedure.name);
            //data.Add("type", yNFactoryProcedure.type);
            data.Add("dataList", stepList);
            return Json(new { code = 1, message = "成功", data = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取最新的流程数据
        /// </summary>
        /// <returns></returns>
        public JsonResult getLastProcessData()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            JavaScriptSerializer js = new JavaScriptSerializer();
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            YNFactoryProcedure yNFactoryProcedure = factoryProcedureController.GetLastProcedure(clothFactoryId);
            List<FactoryProcedureStepNumber> stepList = new List<FactoryProcedureStepNumber>();
            if (yNFactoryProcedure != null && !string.IsNullOrEmpty(yNFactoryProcedure.step))
            {
                try
                {
                    stepList = js.Deserialize<List<FactoryProcedureStepNumber>>(yNFactoryProcedure.step);
                }
                catch (Exception e)
                {

                }
            }
            return Json(new { code = 1, message = "成功", dataList = stepList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取总的流程列表
        /// </summary>
        /// <returns></returns>
        public JsonResult getProcessDataList()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //var produceType = Convert.ToInt32(Request["type"]);
            int produceType = API.Controllers.FactoryProcedureController.type_all;

            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            List<YNFactoryProcedure> procedureList = factoryProcedureController.getProcedureList(clothFactoryId, "", produceType, 1, 500);
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
            for (int i = 0; i < procedureList.Count; i++)
            {
                Dictionary<string, object> _object = new Dictionary<string, object>();
                _object.Add("factoryProcedureId", procedureList[i].id);
                _object.Add("factoryProcedureName", procedureList[i].name);
                dataList.Add(_object);
            }
            return Json(new { code = 1, message = "成功", data = dataList }, JsonRequestBehavior.AllowGet); 
        }

        /// <summary>
        /// 新增型号
        /// </summary>
        /// <returns></returns>
        public JsonResult addProcessData()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string name = Request.Form["name"];
            if (string.IsNullOrEmpty(name))
            {
                return getLoginJsonMessage(0, "型号名称不能为空");
            }
            //int type = Convert.ToInt32(Request.Form["type"]);
            //定制类型
            int type = API.Controllers.FactoryProcedureController.type_0;
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            YNFactoryProcedure temp = factoryProcedureController.GetProcedureByName(name, clothFactoryId);
            if (temp != null)
            {
                return getLoginJsonMessage(0, "型号名称重复，请修改");
            }
            YNFactoryProcedure yNFactoryProcedure = new YNFactoryProcedure();
            yNFactoryProcedure.cloth_factory_id = clothFactoryId;
            yNFactoryProcedure.name = name;
            yNFactoryProcedure.type = type;
            //yNFactoryProcedure.step = data;
            yNFactoryProcedure.count = 0;
            yNFactoryProcedure.create_time = DateTime.Now;
            yNFactoryProcedure.modify_time = DateTime.Now;
            factoryProcedureController.Create(yNFactoryProcedure, clothFactoryId);
            return getLoginJsonMessage(1, "成功", yNFactoryProcedure.id.ToString());
        }

        /// <summary>
        /// 保存流程数据
        /// </summary>
        /// <returns></returns>
        public JsonResult saveProcessData()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.Form["id"]);
            string name = Request.Form["name"];
            if (string.IsNullOrEmpty(name))
            {
                return getLoginJsonMessage(0, "型号名称不能为空");
            }
            //int type = Convert.ToInt32(Request.Form["type"]);
            //定制类型
            int type = API.Controllers.FactoryProcedureController.type_0;
            string data = Request.Form["data"];
            if (string.IsNullOrEmpty(data))
            {
                return getLoginJsonMessage(0, "参数不能为空");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<FactoryProcedureStepNumber> stepList = js.Deserialize<List<FactoryProcedureStepNumber>>(data);

            var duplicateSteps = stepList.GroupBy(m => m.stepName, StringComparer.OrdinalIgnoreCase)
                                         .Select(g => new { StepName =g.Key, Count = g.Count()}).Where(m => m.Count > 1).ToList();
            if (duplicateSteps.Any())
            {
                //var duplicateStepNames = duplicateSteps.Select(m => m.StepName).JoinWith(",");
                return getLoginJsonMessage(0, "重复的环节名:");
            }

            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            YNFactoryProcedure yNFactoryProcedure = new YNFactoryProcedure();
            if (id != 0)
            {
                yNFactoryProcedure = factoryProcedureController.GetProcedureById(id, clothFactoryId);
                if (yNFactoryProcedure == null)
                {
                    return getLoginJsonMessage(0, "数据不存在");
                }
            }
            yNFactoryProcedure.cloth_factory_id = clothFactoryId;
            yNFactoryProcedure.name = name;
            yNFactoryProcedure.type = type;
            yNFactoryProcedure.step = data;
            yNFactoryProcedure.count = stepList.Count();
            if (id == 0)
            {
                yNFactoryProcedure.create_time = DateTime.Now;
                yNFactoryProcedure.modify_time = DateTime.Now;
                factoryProcedureController.Create(yNFactoryProcedure, clothFactoryId);
                id = yNFactoryProcedure.id;
            }
            else
            {
                yNFactoryProcedure.modify_time = DateTime.Now;
                factoryProcedureController.SaveChanges();
            }
            return getLoginJsonMessage(1, "成功", yNFactoryProcedure.id.ToString());
        }
        /// <summary>
        /// 删除用户权限
        /// </summary>
        /// <returns></returns>
        //public JsonResult deleteUserRole()
        //{
        //    if (!checkSession())
        //    {
        //        return getLoginJsonMessage(0, "请登陆");
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    string data = Request.Form["data"];
        //    if (string.IsNullOrEmpty(data))
        //    {
        //        return getLoginJsonMessage(0, "参数错误");
        //    }
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    API.Controllers.ShopUserController shopUserController = new API.Controllers.ShopUserController();
        //    List<Dictionary<string, object>> dataList = js.Deserialize<List<Dictionary<string, object>>>(data);
        //    for(int k=0;k<dataList.Count;k++){
        //        int userId = Convert.ToInt32(dataList[k]["userId"]);
        //        int factoryProcedureId = Convert.ToInt32(dataList[k]["factoryProcedureId"]);
        //        string stepName = Convert.ToString(dataList[k]["stepName"]);
        //        if (factoryProcedureId == 0 || string.IsNullOrEmpty(stepName))
        //        {
        //            return getLoginJsonMessage(0, "参数异常");
        //        }
        //        YNShopUser yNShopUser = shopUserController.GetFactoryUserById(userId, clothFactoryId);
        //        if (yNShopUser == null)
        //        {
        //            return getLoginJsonMessage(0, "用户不存在");
        //        }
        //        if (!string.IsNullOrEmpty(yNShopUser.factory_role))
        //        {
        //            List<FactoryUserProcedureStep> stepList = js.Deserialize<List<FactoryUserProcedureStep>>(yNShopUser.factory_role);
        //            for (int i = 0; i < stepList.Count; i++)
        //            {
        //                if (stepList[i].factoryProcedureId == factoryProcedureId && stepList[i].stepName == stepName)
        //                {
        //                    stepList.RemoveAt(i);
        //                    break;
        //                }
        //            }
        //            yNShopUser.factory_role = js.Serialize(stepList);
        //            yNShopUser.modify_time = DateTime.Now;
        //            shopUserController.SaveChanges();
        //        }
        //    }
           
        //    return getLoginJsonMessage(1, "成功");
        //}
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteProcess()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            var process = factoryProcedureController.GetProcedureById(id, clothFactoryId);
            if (process == null)
            {
                return getLoginJsonMessage(1, "成功");
            }
            factoryProcedureController.Delete(id, clothFactoryId);
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 家具型号列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialType()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            return View();
        }
        public ActionResult MaterialTypeList()
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
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            List<YNFactoryProcedure> procedureList = factoryProcedureController.getProcedureList(clothFactoryId, key, API.Controllers.FactoryProcedureController.type_all, pageIndex, pageSize);
            int count = factoryProcedureController.getProcedureListCount(clothFactoryId, key, API.Controllers.FactoryProcedureController.type_all);
            API.Controllers.PagesController page = new API.Controllers.PagesController();
            page.GetPage(pageIndex, count, pageSize);
            ViewBag.procedureList = procedureList;
            ViewBag.page = page;
            return View();
        }
        /// <summary>
        /// 家具型号耗材详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Material()
        {
            if (!checkSession())
            {
                return loginRerirect();
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            YNFactoryProcedure yNFactoryProcedure = factoryProcedureController.GetProcedureById(id, clothFactoryId);
            if (yNFactoryProcedure == null)
            {
                return getLoginJsonMessage(0, "数据不存在");
            }
            API.Controllers.ProcedureMaterialController procedureMaterialController = new ProcedureMaterialController();
            List<YNProcedureMaterial> procedureMaterialList = procedureMaterialController.GetProcedureMaterialList(yNFactoryProcedure.id, clothFactoryId);
            ViewBag.yNFactoryProcedure = yNFactoryProcedure;
            ViewBag.procedureMaterialList = procedureMaterialList;
            return View();
        }

        /// <summary>
        /// 根据记录id删除原材料记录
        /// </summary>
        /// <returns></returns>
        public JsonResult deleteMaterial()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            API.Controllers.ProcedureMaterialController procedureMaterialController = new ProcedureMaterialController();
            procedureMaterialController.Delete(id, clothFactoryId);
            return getLoginJsonMessage(1, "删除成功");
        }

        /// <summary>
        /// 修改型号名称
        /// </summary>
        /// <returns></returns>
        public JsonResult updateProcedureName()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            } 
            API.Controllers.FactoryProcedureController factoryProcedureController = new FactoryProcedureController();
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.Form["id"]);
            string name = Request.Form["name"];
            YNFactoryProcedure temp = factoryProcedureController.GetProcedureByName(name, clothFactoryId);
            if (temp != null)
            {
                return getLoginJsonMessage(0, "型号名称重复，请修改");
            }
           
            YNFactoryProcedure yNFactoryProcedure = factoryProcedureController.GetProcedureById(id, clothFactoryId);
            if (yNFactoryProcedure == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            yNFactoryProcedure.name = name;
            factoryProcedureController.SaveChanges();
            return getLoginJsonMessage(1, "成功");
        }

        /// <summary>
        /// 根据记录id更新原材料记录
        /// </summary>
        /// <returns></returns>
        public JsonResult updateMaterial()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int number = Convert.ToInt32(Request.QueryString["number"]);
            string remarks = Request.QueryString["remarks"];
            API.Controllers.ProcedureMaterialController procedureMaterialController = new ProcedureMaterialController();
            YNProcedureMaterial yNProcedureMaterial = procedureMaterialController.GetProcedureMaterialById(id, clothFactoryId);
            if (yNProcedureMaterial == null)
            {
                return getLoginJsonMessage(0, "参数错误");
            }
            yNProcedureMaterial.number = number;
            yNProcedureMaterial.remarks = remarks;
            yNProcedureMaterial.modify_time = DateTime.Now;
            procedureMaterialController.SaveChanges();
            return getLoginJsonMessage(1, "保存成功");
        }

        /// <summary>
        /// 添加原材料记录
        /// </summary>
        /// <returns></returns>
        public JsonResult addMaterial()
        {
            if (!checkSession())
            {
                return getLoginJsonMessage(0, "请登陆");
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //家具型号id
            int id = Convert.ToInt32(Request.Form["id"]);
            string data = Request.Form["data"];
            if (string.IsNullOrEmpty(data))
            {
                return getLoginJsonMessage(0, "添加的记录数据不能为空");
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<Dictionary<string, object>> dataList = js.Deserialize<List<Dictionary<string, object>>>(data);
            API.Controllers.ProcedureMaterialController procedureMaterialController = new ProcedureMaterialController();
            API.Controllers.MaterialController materialController = new API.Controllers.MaterialController();
            for (int i = 0; i < dataList.Count; i++)
            {
                int materialId = Convert.ToInt32(dataList[i]["id"]);
                YNMaterial yNMaterial = materialController.GetMaterialById(materialId, clothFactoryId);
                if (yNMaterial == null)
                {
                    continue;
                }
                YNProcedureMaterial yNProcedureMaterial = new YNProcedureMaterial();
                yNProcedureMaterial.procedure_id = id;
                yNProcedureMaterial.material_id = yNMaterial.id;
                yNProcedureMaterial.supplier_id = yNMaterial.supplier_id;
                yNProcedureMaterial.supplier_name = yNMaterial.supplier_name;
                yNProcedureMaterial.image = yNMaterial.image;
                yNProcedureMaterial.material_type = yNMaterial.material_type;
                yNProcedureMaterial.material_name = yNMaterial.material_name;
                yNProcedureMaterial.supplier_model = yNMaterial.supplier_model;
                yNProcedureMaterial.factory_model = yNMaterial.factory_model;
                yNProcedureMaterial.gui_ge = yNMaterial.gui_ge;
                yNProcedureMaterial.unit = yNMaterial.unit;
                yNProcedureMaterial.price = yNMaterial.price;
                yNProcedureMaterial.color = yNMaterial.color;
                yNProcedureMaterial.remarks = Convert.ToString(dataList[i]["remarks"]);
                yNProcedureMaterial.number = Convert.ToInt32(dataList[i]["number"]);
                yNProcedureMaterial.create_time = DateTime.Now;
                yNProcedureMaterial.modify_time = DateTime.Now;
                procedureMaterialController.Create(yNProcedureMaterial, clothFactoryId);
            }
            return getLoginJsonMessage(1, "添加成功");
        }
        
        /// <summary>
        /// 获取原材料列表
        /// </summary>
        /// <returns></returns>
        public ActionResult MateriaAllList()
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
        /// 下载模版
        /// </summary>
        /// <returns></returns>
        public FileResult downloadTemplate()
        {
            if (!checkSession())
            {
                return null;
            }
            HSSFWorkbook hSSFWorkbook = InitializeWorkbook("YNDIY_PROCESS.xls");

            string filename = "型号工序模版.xls";
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
            JavaScriptSerializer js = new JavaScriptSerializer();
            API.Controllers.FactoryProcedureController factoryProcedureController = new API.Controllers.FactoryProcedureController();
            HSSFWorkbook hSSFWorkbook = InitializeWorkbookNew(directoryPath + generateFileName);
            var sheet1 = hSSFWorkbook.GetSheetAt(0);
            int index = 1;
            //获取工序名称的数量
            int name_index = 1;
            var row = GetExcelRow(sheet1, 0);
            List<string> nameList = new List<string>();
            while (true)
            {
                if (row == null)
                {
                    break;
                }
                var cell = GetExcelCell(row, name_index++);
                if (string.IsNullOrEmpty(Convert.ToString(cell)))
                {
                    break;
                }
                nameList.Add(Convert.ToString(cell));
            }


            List<string> messageList = new List<string>();
            while (true)
            {
                row = GetExcelRow(sheet1,index++);
                if (row == null)
                {
                    break;
                }
                YNFactoryProcedure yNFactoryProcedure = new YNFactoryProcedure();
                yNFactoryProcedure.cloth_factory_id = clothFactoryId;
                yNFactoryProcedure.name = Convert.ToString(GetExcelCell(row,0));
                if (string.IsNullOrEmpty(yNFactoryProcedure.name))
                {
                    string temp = "第" + index + "行【型号名称为空】上传失败";
                    messageList.Add(temp);
                    break;
                }
                if (factoryProcedureController.GetProcedureByName(yNFactoryProcedure.name, clothFactoryId) != null)
                {
                    string temp = "第" + index + "行【型号名称已经存在】上传失败";
                    messageList.Add(temp);
                    continue;
                }
                List<FactoryProcedureStepNumber> stepList = new List<FactoryProcedureStepNumber>();
                int id = 1;
                for (int i = 1; i < name_index; i++)
                {
                    string priceStr = Convert.ToString(GetExcelCell(row, i));
                    if (string.IsNullOrEmpty(priceStr))
                    {
                        continue;
                    }
                    FactoryProcedureStepNumber temp = new FactoryProcedureStepNumber();
                    temp.id = Convert.ToString(id++);
                    temp.stepName = nameList[i - 1];
                    temp.price = Convert.ToDecimal(priceStr);
                    temp.time = 0;
                    stepList.Add(temp);
                }
                yNFactoryProcedure.step = js.Serialize(stepList);
                yNFactoryProcedure.count = stepList.Count;
                yNFactoryProcedure.create_time = DateTime.Now;
                yNFactoryProcedure.modify_time = DateTime.Now;
                factoryProcedureController.Create(yNFactoryProcedure, clothFactoryId);
            }
            hSSFWorkbook.Close();
            return Json(new { code = 1, message = "上传结束", data = messageList }, JsonRequestBehavior.AllowGet);

        }

        private static IRow GetExcelRow(ISheet sheet, int index)
        {
            var row = sheet.GetRow(index);
            if (row == null)
            {
                row = sheet.CreateRow(index);
                row.CreateCell(0);
                row.CreateCell(1);
            }
            return row;
        }

        private static ICell GetExcelCell(IRow row, int index)
        {
            var cell = row.GetCell(index);
            if (cell == null)
            {
                cell = row.CreateCell(index);
            }
            return cell;
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
