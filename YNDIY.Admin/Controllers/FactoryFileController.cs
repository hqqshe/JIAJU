using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using YNDIY.API.Controllers;

namespace YNDIY.Admin.Controllers
{
    public class FactoryFileController : ParentController
    {
        public static string[] fileExt = { "xls", "xlsx" ,"jpg","png","pdf","doc","docx","wpt","wps","et","ett"};
        /// <summary>
        /// 工厂端上传文件
        /// </summary>
        /// <returns></returns>
        //public JsonResult upLoadFile()
        //{
        //    if (!checkSession())
        //    {
        //        return getLoginJsonMessage(0, "请登录");
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    HttpPostedFileBase uploadFile = Request.Files[0];
        //    //上传文件扩展名
        //    string uploadExt = uploadFile.FileName.Split('.').Last<string>();
        //    if (!fileExt.Contains(uploadExt))
        //    {
        //        return getLoginJsonMessage(0, "文件格式不正确，请重新上传");
        //    }
        //    string directoryPath = "D:\\factoryData\\" + clothFactoryId + "\\";
        //    //不存在文件夹则创建文件夹
        //    if (!Directory.Exists(directoryPath))
        //    {
        //        Directory.CreateDirectory(directoryPath);
        //    }
        //    string generateFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + uploadExt;
        //    using (FileStream fs = System.IO.File.Create(directoryPath + generateFileName))
        //    {
        //        //保存数据
        //        SaveFile(uploadFile.InputStream, fs);
        //    }

        //    if (!string.IsNullOrEmpty(Request.Form["id"]))
        //    {
        //        int id = Convert.ToInt32(Request.Form["id"]);
        //        int packageId = 0;
        //        if (!string.IsNullOrEmpty(Request.Form["packageId"]))
        //        {
        //            packageId = Convert.ToInt32(Request.Form["packageId"]);
        //        }

        //        //是否是上传大货的工艺单
        //        bool isBigOrder = false;
        //        bool.TryParse(Request.Form["isBigOrder"], out isBigOrder);
        //        if (isBigOrder)
        //        {
        //            var factoryBigOrderController = new API.Controllers.FactoryBigOrderController();
        //            var bigOrder = factoryBigOrderController.getBigOrderByOrderId(id, clothFactoryId);
        //            if (bigOrder == null)
        //            {
        //                return getLoginJsonMessage(0, "参数传递错误");
        //            }
        //            bigOrder.customer_order_file_path = generateFileName;
        //            factoryBigOrderController.SaveChanges();
        //        }
        //        else
        //        {
        //            API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //            YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
        //            if (yNFactoryOrder == null)
        //            {
        //                return getLoginJsonMessage(0, "参数传递错误");
        //            }
        //            //客户订单上传
        //            if (packageId == 0)
        //            {
        //                yNFactoryOrder.factory_consumer_order = generateFileName;
        //                factoryOrderController.SaveChanges();
        //            }
        //            //生产订单上传
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(yNFactoryOrder.factory_cloth_info))
        //                {
        //                    JavaScriptSerializer js = new JavaScriptSerializer();
        //                    List<FactoryClothInfo> clothList = js.Deserialize<List<FactoryClothInfo>>(yNFactoryOrder.factory_cloth_info);
        //                    for (int i = 0; i < clothList.Count; i++)
        //                    {
        //                        if (clothList[i].packageId == packageId)
        //                        {
        //                            clothList[i].factory_order_url = generateFileName;
        //                            break;
        //                        }
        //                    }
        //                    yNFactoryOrder.factory_cloth_info = js.Serialize(clothList);
        //                    factoryOrderController.SaveChanges();
        //                }
        //            }
        //        }
        //    }
        //    return Json(new { code = 1, message = "上传成功", url = generateFileName }, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// 工厂下载文件
        /// </summary>
        /// <returns></returns>
        public FileResult downLoadFile()
        {
            if (!checkSession())
            {
                return null;
            }
            int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            string fileName = Request.QueryString["fileName"];
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }
            string filePath = "D:\\factoryData\\" + clothFactoryId + "\\" + fileName;
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
            return null;
        }

        /// <summary>
        /// 条码数据
        /// </summary>
        /// <returns></returns>
        //public JsonResult downLoadBarCodeJsonData()
        //{
        //    if (!checkSession())
        //    {
        //        return getLoginJsonMessage(0, "请登陆");
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    int id = Convert.ToInt32(Request.QueryString["id"]);
        //    API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //    YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
        //    if (yNFactoryOrder == null)
        //    {
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(yNFactoryOrder.factory_cloth_info))
        //    {
        //        return null;
        //    }
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<FactoryClothInfo> clothList = js.Deserialize<List<FactoryClothInfo>>(yNFactoryOrder.factory_cloth_info);
        //    List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
        //    int index = 0;
        //    int clothFactoryBarType = Convert.ToInt32(Session["ClothFactoryBarType"]);
        //    if (clothFactoryBarType == 1)
        //    {
        //        Dictionary<string, object> data = new Dictionary<string, object>();
        //        data.Add("title", "订单号：" + yNFactoryOrder.order_id);
        //        data.Add("userName", yNFactoryOrder.members_name);
        //        List<Dictionary<string, object>> _dataList = new List<Dictionary<string, object>>();
        //        data.Add("data", _dataList);
        //        dataList.Add(data);
        //        for (int i = 0; i < clothList.Count; i++)
        //        {
        //            Dictionary<string, object> temp = new Dictionary<string, object>();
        //            temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码编号");
        //            temp.Add("code", clothList[i].packageBarCode);
        //            _dataList.Add(temp);
        //            index++;
        //            for (int k = 0; k < clothList[i].clothDetail.Count; k++)
        //            {
        //                Dictionary<string, object> temp1 = new Dictionary<string, object>();
        //                temp1.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码编号_" + clothList[i].clothDetail[k].clothId);
        //                temp1.Add("code", clothList[i].clothDetail[k].clothBarCode);
        //                _dataList.Add(temp1);
        //                index++;
        //            }
        //            Dictionary<string, object> _temp = new Dictionary<string, object>();
        //            _temp.Add("name", "换行");
        //            _temp.Add("code", "换行");
        //            _dataList.Add(_temp);
        //            index++;
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < clothList.Count; i++)
        //        {
        //            //for (int k = 0; k < clothList[i].stepList.Count; k++)
        //            //{
        //            //    if (clothList[i].stepList[k].stepName.Contains("质检"))
        //            //    {
        //            //        Dictionary<string, object> temp = new Dictionary<string, object>();
        //            //        temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 通过");
        //            //        temp.Add("code", k + "_" + "0" + "_" + clothList[i].packageBarCode);
        //            //        dataList.Add(temp);
        //            //        index++;
        //            //        Dictionary<string, object> temp1 = new Dictionary<string, object>();
        //            //        temp1.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 不通过");
        //            //        temp1.Add("code", k + "_" + "1" + "_" + clothList[i].packageBarCode);
        //            //        dataList.Add(temp1);
        //            //        index++;
        //            //    }
        //            //    else
        //            //    {
        //            //        Dictionary<string, object> temp = new Dictionary<string, object>();
        //            //        temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 通过");
        //            //        temp.Add("code", k + "_" + "0" + "_" + clothList[i].packageBarCode);
        //            //        dataList.Add(temp);
        //            //        index++;
        //            //    }
        //            //}

        //            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //            {
        //                Dictionary<string, object> data = new Dictionary<string, object>();
        //                data.Add("title", "订单号：" + yNFactoryOrder.order_id + "  衣服名称:" + clothList[i].clothName + "_" + (m + 1));
        //                data.Add("userName", yNFactoryOrder.members_name);
        //                List<Dictionary<string, object>> _dataList = new List<Dictionary<string, object>>();
        //                data.Add("data", _dataList);
        //                dataList.Add(data);

        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (clothList[i].stepList[k].stepName.Contains("质检"))
        //                    {
        //                        //Dictionary<string, object> temp = new Dictionary<string, object>();
        //                        //temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        //temp.Add("code", k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        //_dataList.Add(temp);
        //                        //index++;
        //                        //Dictionary<string, object> temp1 = new Dictionary<string, object>();
        //                        //temp1.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 不通过");
        //                        //temp1.Add("code", k + "_" + "1" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        //_dataList.Add(temp1);
        //                        //index++;
        //                    }
        //                    else
        //                    {
        //                        Dictionary<string, object> temp = new Dictionary<string, object>();
        //                        temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        temp.Add("code", k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        _dataList.Add(temp);
        //                        index++;
        //                    }
        //                }

        //                Dictionary<string, object> _temp = new Dictionary<string, object>();
        //                _temp.Add("name", "换行");
        //                _temp.Add("code", "换行");
        //                _dataList.Add(_temp);
        //                index++;

        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (clothList[i].stepList[k].stepName.Contains("质检"))
        //                    {
        //                        Dictionary<string, object> temp = new Dictionary<string, object>();
        //                        temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        temp.Add("code", k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        _dataList.Add(temp);
        //                        index++;
        //                        Dictionary<string, object> temp1 = new Dictionary<string, object>();
        //                        temp1.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 不通过");
        //                        temp1.Add("code", k + "_" + "1" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        _dataList.Add(temp1);
        //                        index++;
        //                    }
        //                    else
        //                    {
        //                        //Dictionary<string, object> temp = new Dictionary<string, object>();
        //                        //temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        //temp.Add("code", k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        //_dataList.Add(temp);
        //                        //index++;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Json(new { code = 1, message = "成功", data = dataList }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 条码数据
        /// </summary>
        /// <returns></returns>
        //public JsonResult downLoadBarCodeJsonData()
        //{
        //    if (!checkSession())
        //    {
        //        return getLoginJsonMessage(0, "请登陆");
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    int id = Convert.ToInt32(Request.QueryString["id"]);
        //    API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //    YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
        //    if (yNFactoryOrder == null)
        //    {
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(yNFactoryOrder.factory_cloth_info))
        //    {
        //        return null;
        //    }
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<FactoryClothInfo> clothList = js.Deserialize<List<FactoryClothInfo>>(yNFactoryOrder.factory_cloth_info);
        //    List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
        //    int index = 0;
        //    int clothFactoryBarType = Convert.ToInt32(Session["ClothFactoryBarType"]);
        //    if (clothFactoryBarType == 1)
        //    {
        //        for (int i = 0; i < clothList.Count; i++)
        //        {
        //            Dictionary<string, object> temp = new Dictionary<string, object>();
        //            temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码编号");
        //            temp.Add("code", clothList[i].packageBarCode);
        //            dataList.Add(temp);
        //            index++;
        //            for (int k = 0; k < clothList[i].clothDetail.Count; k++)
        //            {
        //                Dictionary<string, object> temp1 = new Dictionary<string, object>();
        //                temp1.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码编号_" + clothList[i].clothDetail[k].clothId);
        //                temp1.Add("code", clothList[i].clothDetail[k].clothBarCode);
        //                dataList.Add(temp1);
        //                index++;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < clothList.Count; i++)
        //        {
        //            for (int k = 0; k < clothList[i].stepList.Count; k++)
        //            {
        //                if (clothList[i].stepList[k].stepName.Contains("质检"))
        //                {
        //                    Dictionary<string, object> temp = new Dictionary<string, object>();
        //                    temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 通过");
        //                    temp.Add("code", k + "_" + "0" + "_" + clothList[i].packageBarCode);
        //                    dataList.Add(temp);
        //                    index++;
        //                    Dictionary<string, object> temp1 = new Dictionary<string, object>();
        //                    temp1.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 不通过");
        //                    temp1.Add("code", k + "_" + "1" + "_" + clothList[i].packageBarCode);
        //                    dataList.Add(temp1);
        //                    index++;
        //                }
        //                else
        //                {
        //                    Dictionary<string, object> temp = new Dictionary<string, object>();
        //                    temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 通过");
        //                    temp.Add("code", k + "_" + "0" + "_" + clothList[i].packageBarCode);
        //                    dataList.Add(temp);
        //                    index++;
        //                }
        //            }

        //            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //            {
        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (clothList[i].stepList[k].stepName.Contains("质检"))
        //                    {
        //                        Dictionary<string, object> temp = new Dictionary<string, object>();
        //                        temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        temp.Add("code", k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        dataList.Add(temp);
        //                        index++;
        //                        Dictionary<string, object> temp1 = new Dictionary<string, object>();
        //                        temp1.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 不通过");
        //                        temp1.Add("code", k + "_" + "1" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        dataList.Add(temp1);
        //                        index++;
        //                    }
        //                    else
        //                    {
        //                        Dictionary<string, object> temp = new Dictionary<string, object>();
        //                        temp.Add("name", clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        temp.Add("code", k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        dataList.Add(temp);
        //                        index++;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return Json(new { code = 1, message = "成功", data = dataList }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 下载条形码
        /// </summary>
        /// <returns></returns>
        //public FileResult downLoadBarCode()
        //{
        //    if (!checkSession())
        //    {
        //        return null;
        //    }
        //    int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
        //    int id = Convert.ToInt32(Request.QueryString["id"]);
        //    API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //    YNFactoryOrder yNFactoryOrder = factoryOrderController.getFactoryOrderByIdFactory(id, clothFactoryId);
        //    if (yNFactoryOrder == null)
        //    {
        //        return null;
        //    }
        //    if (string.IsNullOrEmpty(yNFactoryOrder.factory_cloth_info))
        //    {
        //        return null;
        //    }
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<FactoryClothInfo> clothList = js.Deserialize<List<FactoryClothInfo>>(yNFactoryOrder.factory_cloth_info);
        //    HSSFWorkbook hSSFWorkbook = InitializeWorkbook("YNDIY_ORDER_BARCODE.xls");
        //    HSSFSheet sheet1 = (HSSFSheet)hSSFWorkbook.GetSheetAt(0);
        //    int index = 0;
        //    int clothFactoryBarType = Convert.ToInt32(Session["ClothFactoryBarType"]);
        //    if (clothFactoryBarType == 1)
        //    {
        //        for (int i = 0; i < clothList.Count; i++)
        //        {
        //            sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 包条码编号");
        //            sheet1.GetRow(index).GetCell(1).SetCellValue(clothList[i].packageBarCode);
        //            index++;
        //            for (int k = 0; k < clothList[i].clothDetail.Count; k++)
        //            {
        //                sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码编号_" + clothList[i].clothDetail[k].clothId);
        //                sheet1.GetRow(index).GetCell(1).SetCellValue(clothList[i].clothDetail[k].clothBarCode);
        //                index++;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < clothList.Count; i++)
        //        {
        //            for (int k = 0; k < clothList[i].stepList.Count; k++)
        //            {
        //                if (clothList[i].stepList[k].stepName.Contains("质检"))
        //                {
        //                    sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 通过");
        //                    sheet1.GetRow(index).GetCell(1).SetCellValue(k + "_" + "0" + "_" + clothList[i].packageBarCode);
        //                    index++;
        //                    sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 不通过");
        //                    sheet1.GetRow(index).GetCell(1).SetCellValue(k + "_" + "1" + "_" + clothList[i].packageBarCode);
        //                    index++;
        //                }
        //                else
        //                {
        //                    sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 包条码 " + clothList[i].stepList[k].stepName + " 通过");
        //                    sheet1.GetRow(index).GetCell(1).SetCellValue(k + "_" + "0" + "_" + clothList[i].packageBarCode);
        //                    index++;
        //                }
        //            }

        //            for (int m = 0; m < clothList[i].clothDetail.Count; m++)
        //            {
        //                for (int k = 0; k < clothList[i].stepList.Count; k++)
        //                {
        //                    if (clothList[i].stepList[k].stepName.Contains("质检"))
        //                    {
        //                        sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        sheet1.GetRow(index).GetCell(1).SetCellValue(k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        index++;
        //                        sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 不通过");
        //                        sheet1.GetRow(index).GetCell(1).SetCellValue(k + "_" + "1" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        index++;
        //                    }
        //                    else
        //                    {
        //                        sheet1.GetRow(index).GetCell(0).SetCellValue(clothList[i].clothName + "_" + clothList[i].packageId + " 流水条码_" + clothList[i].clothDetail[m].clothId + " " + clothList[i].stepList[k].stepName + " 通过");
        //                        sheet1.GetRow(index).GetCell(1).SetCellValue(k + "_" + "0" + "_" + clothList[i].clothDetail[m].clothBarCode);
        //                        index++;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    string filename = "订单号" + ":" + yNFactoryOrder.order_id + ".xls";
        //    Response.ContentType = "application/vnd.ms-excel";
        //    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
        //    Response.Clear();

        //    MemoryStream file = new MemoryStream();
        //    hSSFWorkbook.Write(file);
        //    Response.BinaryWrite(file.GetBuffer());
        //    Response.End();
        //    hSSFWorkbook.Close();
        //    file.Close();
        //    return null;
        //}

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

      
        /// <summary>
        /// 下载生产计划单
        /// </summary>
        public ActionResult downloadOrderPlan(string startTime)
        {
            //if (!checkSession())
            //{
            //    return Content("请登录");
            //}
            //if (string.IsNullOrEmpty(startTime))
            //{
            //    return Content("生产计划日期不能为空");
            //}
            //int clothFactoryId = Convert.ToInt32(Session["ClothFactoryId"]);
            //API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
            //List<YNFactoryOrder> orderList = factoryOrderController.getFactoryOrderProduceListNew(0,startTime, clothFactoryId);

            //var hSSFWorkbook = InitializeWorkbook("YNDIY_ORDER_PLAN.xls");
            //var sheet1 = hSSFWorkbook.GetSheetAt(0);
            //int index = 3;
            //for (int i = 0; i < orderList.Count; i++)
            //{
            //    var row = GetExcelRow(sheet1, index++);
                //row.GetCell(0, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].shop_name);
                //row.GetCell(1, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].factory_delivery_day.ToString("yyyy-MM-dd"));
                //row.GetCell(2, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].produce_id);
                //row.GetCell(3, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].model_name);
                //row.GetCell(4, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].format);
                //row.GetCell(5, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].number);
                //row.GetCell(6, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].color);
                //row.GetCell(7, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].remarks);
                //row.GetCell(8, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].special_remarks);
                //row.GetCell(9, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(orderList[i].srart_produce_time);
                //JavaScriptSerializer js = new JavaScriptSerializer();
                //List<FactoryProcedureStepNumberOrder> stepList = new List<FactoryProcedureStepNumberOrder>();
                //if (!string.IsNullOrEmpty(orderList[i].model_detail))
                //{
                //    stepList = js.Deserialize<List<FactoryProcedureStepNumberOrder>>(orderList[i].model_detail);
                //}
                //int cell = 10;
                //for (int k = 0; k < stepList.Count; k++)
                //{
                //    string content = stepList[k].id +"-"+ stepList[k].stepName + "\r\n";
                //    if (stepList[k].userList != null && stepList[k].userList.Count > 0)
                //    {
                //        content += "["+stepList[k].userList[0].gonghao + "-" + stepList[k].userList[0].userName+"]";
                //    }
                //    row.GetCell(cell++, MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(content);
                //}
            //}
            //for (int i = index; i < 100; i++)
            //{
            //    sheet1.RemoveRow(sheet1.GetRow(i));
            //}
            //string filename = string.Format("{0}-生产计划表.xls", startTime);
            //Response.ContentType = "application/vnd.ms-excel";
            //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            //Response.Clear();

            //MemoryStream file = new MemoryStream();
            //hSSFWorkbook.Write(file);
            //Response.BinaryWrite(file.GetBuffer());
            //Response.End();
            //hSSFWorkbook.Close();
            //file.Close();
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
