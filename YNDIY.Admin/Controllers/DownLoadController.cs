using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.SS.UserModel;
using System.Net;
using System.Drawing;
using System.IO;
using System.Text;
using YNDIY.API.Models;
using System.Web.Script.Serialization;
using ThoughtWorks.QRCode.Codec;
using System.Drawing.Imaging;

namespace YNDIY.Admin.Controllers
{
    public class DownLoadController : ParentController
    {
        /// <summary>
        /// 下载工厂生产单
        /// </summary>
        /// <returns></returns>
        //public FileResult DownLoadOrderInfo()
        //{
        //    string factoryOrderId = Request.QueryString["factoryOrderId"];
        //    int factoryOrderIdInt = Convert.ToInt32(factoryOrderId);
        //    int shopId = 0;
        //    API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //    YNFactoryOrder yNFactoryOrder = null;
        //    if (checkSession() && Session["ShopId"] != null)
        //    {
        //        shopId = Convert.ToInt32(Session["ShopId"]);
        //        yNFactoryOrder = factoryOrderController.getFactoryOrderById(factoryOrderIdInt, shopId);
        //        if (yNFactoryOrder == null)
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        string token = Request.QueryString["token"];
        //        shopId = Convert.ToInt32(Request.QueryString["shopId"]);
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            return null;
        //        }
        //        yNFactoryOrder = factoryOrderController.getFactoryOrderById(factoryOrderIdInt, shopId, token);
        //        if (yNFactoryOrder == null)
        //        {
        //            return null;
        //        }
        //    }

        //    API.Controllers.OrderController orderController = new API.Controllers.OrderController();
        //    API.Controllers.OrderInfoController orderInfoController = new API.Controllers.OrderInfoController();
        //    API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
        //    API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<ClothHandingInfo> clothHandingInfoList = new List<ClothHandingInfo>();
        //    if (!string.IsNullOrEmpty(yNFactoryOrder.cloth_info))
        //    {
        //        clothHandingInfoList = js.Deserialize<List<ClothHandingInfo>>(yNFactoryOrder.cloth_info);
        //    }
        //    List<YNOrderInfo> orderInfoList = new List<YNOrderInfo>();
        //    for (int i = 0; i < clothHandingInfoList.Count; i++)
        //    {
        //        YNOrderInfo yNOrderInfo = orderInfoController.getOrderInfoById(clothHandingInfoList[i].orderInfoId, shopId);
        //        orderInfoList.Add(yNOrderInfo);
        //    }
        //    YNOrder yNOrder = orderController.GetOrderByID(yNFactoryOrder.order_id, shopId);
        //    YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(yNFactoryOrder.cloth_factory_id);
        //    YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(shopId);
        //    YNShopInfo parentShopInfo = shopInfoController.GetShopInfoByID(yNShopInfo.parent_shop_id);

        //    HSSFWorkbook hSSFWorkbook = InitializeWorkbook("YNDIY_ORDER_GONGCHANG_NEW.xls");
        //    string rengongLiangTi = yNOrder.jingShenChai_data;
        //    JingShenChaiTaior jingShenChaiTaior = new JingShenChaiTaior();
        //    if (!string.IsNullOrEmpty(rengongLiangTi))
        //    {
        //        jingShenChaiTaior = js.Deserialize<JingShenChaiTaior>(rengongLiangTi);
        //    }
        //    //转化为工厂接受尺寸
        //    jingShenChaiTaior = jingShenChaiTaior.convertUnity(yNClothFactoryInfo.unit_type);
        //    //体型
        //    string user_tixing = yNOrder.user_tixing;
        //    string[] tixing = user_tixing.Split(',');
        //    //string userTixing = "";
        //    for (int i = 0; i < tixing.Length; i++)
        //    {
        //        tixing[i] = API.Controllers.OrderController.getTiXing(tixing[i]);
        //    }
        //    int tableCount = 0;
        //    for (int i = 0; i < orderInfoList.Count; i++)
        //    {
        //        JingShenChaiTaior yifuTailor = new JingShenChaiTaior();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].yifu_data))
        //        {
        //            yifuTailor = js.Deserialize<JingShenChaiTaior>(orderInfoList[i].yifu_data);
        //        }
        //        //转化为工厂接受尺寸
        //        yifuTailor = yifuTailor.convertUnity(yNClothFactoryInfo.unit_type);
        //        hSSFWorkbook.SetSheetName(tableCount, yNOrder.members_name + "_" + orderInfoList[i].cloth_name + "_" + (i + 1));
        //        HSSFSheet sheet1 = (HSSFSheet)hSSFWorkbook.GetSheetAt(tableCount);
        //        tableCount++;
        //        sheet1.GetRow(0).GetCell(0).SetCellValue(parentShopInfo.shop_name);
        //        sheet1.GetRow(1).GetCell(1).SetCellValue(yNOrder.members_name);
        //        sheet1.GetRow(1).GetCell(3).SetCellValue(yNShopInfo.shop_name);
        //        sheet1.GetRow(1).GetCell(6).SetCellValue(yNOrder.order_date.ToString("yyyy/MM/dd"));
        //        sheet1.GetRow(1).GetCell(8).SetCellValue(yNOrder.create_time.ToString("yyyy/MM/dd"));
        //        sheet1.GetRow(2).GetCell(1).SetCellValue(yNOrder.id);
        //        sheet1.GetRow(3).GetCell(3).SetCellValue(orderInfoList[i].cloth_name);
        //        sheet1.GetRow(3).GetCell(6).SetCellValue(orderInfoList[i].number);
        //        //sheet1.GetRow(3).GetCell(7).SetCellValue(orderInfoList[i]);
        //        sheet1.GetRow(3).GetCell(12).SetCellValue(yNOrder.weight + "KG");
        //        sheet1.GetRow(2).GetCell(12).SetCellValue(yNOrder.height + "CM");
        //        int count = tixing.Length > 5 ? 5 : tixing.Length;
        //        for (int k = 0; k < count; k++)
        //        {
        //            sheet1.GetRow(7).GetCell(8 + k).SetCellValue(tixing[k]);
        //        }
        //        //设置样衣
        //        List<YangyiInfo> yangyiInfoList = new List<YangyiInfo>();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].yangyi_info))
        //        {
        //            yangyiInfoList = js.Deserialize<List<YangyiInfo>>(orderInfoList[i].yangyi_info);
        //        }
        //        sheet1.GetRow(8).GetCell(10).SetCellValue(yangyiInfoList.Count > 0 ? yangyiInfoList[0].name : "");
        //        //设置版型调整
        //        List<ClothModifyInfo> clothModifyInfoList = new List<ClothModifyInfo>();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].cloth_modify_info))
        //        {
        //            clothModifyInfoList = js.Deserialize<List<ClothModifyInfo>>(orderInfoList[i].cloth_modify_info);
        //        }
        //        List<ClothModifyInfo> clothModifyInfoListNew = new List<ClothModifyInfo>();
        //        for (int k = 0; k < clothModifyInfoList.Count; k++)
        //        {
        //            if (string.IsNullOrEmpty(clothModifyInfoList[k].name) || string.IsNullOrEmpty(clothModifyInfoList[k].detail))
        //            {
        //                continue;
        //            }
        //            clothModifyInfoListNew.Add(clothModifyInfoList[k]);
        //        }
        //        int _count = clothModifyInfoListNew.Count > 6 ? 6 : clothModifyInfoListNew.Count;
        //        int _row_index = 9;
        //        for (int k = 0; k < _count; k++)
        //        {
        //            if (k == 5)
        //            {
        //                _row_index = 23;
        //            }
        //            else
        //            {
        //                _row_index = 9 + k * 3;
        //            }
        //            sheet1.GetRow(_row_index).GetCell(7).SetCellValue(clothModifyInfoListNew[k].name);
        //            sheet1.GetRow(_row_index).GetCell(9).SetCellValue(clothModifyInfoListNew[k].detail);
        //        }
                    
        //        //设置面料
        //        List<FabricPurchaseInfo> fabricPurchaseInfoList = new List<FabricPurchaseInfo>();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].fabricPurchase_info))
        //        {
        //            fabricPurchaseInfoList = js.Deserialize<List<FabricPurchaseInfo>>(orderInfoList[i].fabricPurchase_info);
        //        }
        //        string zhuLiao = "";
        //        string fuLiao = "";
        //        for (int k = 0; k < fabricPurchaseInfoList.Count; k++)
        //        {
        //            if (fabricPurchaseInfoList[k].Type == 0)
        //            {
        //                zhuLiao += fabricPurchaseInfoList[k].Name + "(" + convertFabricUnit(fabricPurchaseInfoList[k].UnitType, 0, fabricPurchaseInfoList[k].Number) + "米)" + "  ";
        //            }
        //            else
        //            {
        //                fuLiao += fabricPurchaseInfoList[k].Name + "(" + convertFabricUnit(fabricPurchaseInfoList[k].UnitType, 0, fabricPurchaseInfoList[k].Number) + "米)" + "  ";
        //            }
        //        }
        //        sheet1.GetRow(4).GetCell(1).SetCellValue(zhuLiao);
        //        sheet1.GetRow(5).GetCell(1).SetCellValue(fuLiao);
        //        sheet1.GetRow(6).GetCell(1).SetCellValue(clothHandingInfoList[i].remarks);
        //        //设置量体数据
        //        string unit_name = "";
        //        if (yNClothFactoryInfo.unit_type == 0)
        //        {
        //            unit_name = "CM";
        //        }
        //        else if (yNClothFactoryInfo.unit_type == 1)
        //        {
        //            unit_name = "市寸";
        //        }
        //        else if (yNClothFactoryInfo.unit_type == 2)
        //        {
        //            unit_name = "英寸";
        //        }
        //        sheet1.GetRow(7).GetCell(2).SetCellValue("净体  " + unit_name);
        //        sheet1.GetRow(7).GetCell(3).SetCellValue("成衣  " + unit_name);
        //        int rowIndex = 8;
        //        JingShenChaiTaior tempTailor = new JingShenChaiTaior();
        //        if (API.Controllers.ClothTypeNameController.shangYi.Contains(orderInfoList[i].clothtype_id))
        //        {
        //            tempTailor = jingShenChaiTaior.getYiFuTailor();
        //        }
        //        else if (API.Controllers.ClothTypeNameController.xiaYi.Contains(orderInfoList[i].clothtype_id))
        //        {
        //            tempTailor = jingShenChaiTaior.getKuZiTailor();
        //        }
        //        else
        //        {
        //            tempTailor = jingShenChaiTaior;
        //        }
        //        foreach (System.Reflection.PropertyInfo p in yifuTailor.GetType().GetProperties())
        //        {
        //            if (p.Name.Contains("0") || p.Name.Contains("1"))
        //            {
        //                object value = p.GetValue(yifuTailor);
        //                object valueNew = p.GetValue(tempTailor);
        //                if ((value == null || value == "") && (valueNew == null || valueNew == ""))
        //                {
        //                    continue;
        //                }
        //                sheet1.GetRow(rowIndex).GetCell(1).SetCellValue(yifuTailor.getName(p.Name));
        //                sheet1.GetRow(rowIndex).GetCell(2).SetCellValue(valueNew == null ? "" : valueNew.ToString());
        //                sheet1.GetRow(rowIndex).GetCell(3).SetCellValue(value == null ? "" : value.ToString());
        //                rowIndex++;
        //            }
        //        }
        //        //设置款式细节
        //        ClothDetail clothDetail = new ClothDetail();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].style_detail))
        //        {
        //            try
        //            {
        //                clothDetail = js.Deserialize<ClothDetail>(orderInfoList[i].style_detail);
        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }
        //        int _style_count = 8;
        //        if (clothDetail.style != null)
        //        {
        //            for (int k = 0; k < clothDetail.style.Count; k++)
        //            {
        //                sheet1.GetRow(_style_count).GetCell(0).SetCellValue(clothDetail.style[k]);
        //                _style_count++;
        //            }
        //        }
        //        //设置图片
        //        //设置款式正面图
        //        if (!string.IsNullOrEmpty(orderInfoList[i].cloth_font_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(orderInfoList[i].cloth_font_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 1, 26, 4, 39);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置款式背面图
        //        if (!string.IsNullOrEmpty(orderInfoList[i].cloth_back_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(orderInfoList[i].cloth_back_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 4, 26, 9, 39);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置草图
        //        if (!string.IsNullOrEmpty(orderInfoList[i].design_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(orderInfoList[i].design_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 9, 26, 13, 39);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }

        //        //设置人体正面图
        //        if (!string.IsNullOrEmpty(yNOrder.user_font_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(yNOrder.user_font_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 1, 39, 4, 52);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置人体背面图
        //        if (!string.IsNullOrEmpty(yNOrder.user_back_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(yNOrder.user_back_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 4, 39, 9, 52);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置人体侧面图
        //        if (!string.IsNullOrEmpty(yNOrder.user_side_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(yNOrder.user_side_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 9, 39, 13, 52);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置联系人信息
        //        sheet1.GetRow(52).GetCell(0).SetCellValue("联系人:" + yNShopInfo.link_man);
        //        sheet1.GetRow(52).GetCell(2).SetCellValue("联系电话:" + yNShopInfo.phone);
        //        sheet1.GetRow(52).GetCell(7).SetCellValue("联系地址:" + yNShopInfo.address_detail);
        //    }

            //删除多余的表格数据
            //for (int i = tableCount; i < 10; i++)
            //{
            //    hSSFWorkbook.RemoveSheetAt(tableCount);
            //}
            //string filename = yNShopInfo.shop_name + "_" + yNOrder.members_name + ".xls";
            //Response.ContentType = "application/vnd.ms-excel";
            //Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            //Response.Clear();

            //MemoryStream file = new MemoryStream();
            //hSSFWorkbook.Write(file);
            //Response.BinaryWrite(file.GetBuffer());
            //Response.End();
            //hSSFWorkbook.Close();
            //file.Close();
            //return null;
        //}

        /// <summary>
        /// 下载工厂生产单
        /// </summary>
        /// <returns></returns>
        //public FileResult DownLoadOrderInfoOld()
        //{
        //    string factoryOrderId = Request.QueryString["factoryOrderId"];
        //    int factoryOrderIdInt = Convert.ToInt32(factoryOrderId);
        //    int shopId = 0;
        //    API.Controllers.FactoryOrderController factoryOrderController = new API.Controllers.FactoryOrderController();
        //    YNFactoryOrder yNFactoryOrder = null;
        //    if (checkSession() && Session["ShopId"] != null)
        //    {
        //        shopId = Convert.ToInt32(Session["ShopId"]);
        //        yNFactoryOrder = factoryOrderController.getFactoryOrderById(factoryOrderIdInt, shopId);
        //        if (yNFactoryOrder == null)
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        string token = Request.QueryString["token"];
        //        shopId = Convert.ToInt32(Request.QueryString["shopId"]);
        //        if (string.IsNullOrEmpty(token))
        //        {
        //            return null;
        //        }
        //        yNFactoryOrder = factoryOrderController.getFactoryOrderById(factoryOrderIdInt, shopId,token);
        //        if (yNFactoryOrder == null)
        //        {
        //            return null;
        //        }
        //    }
            
        //    API.Controllers.OrderController orderController = new API.Controllers.OrderController();
        //    API.Controllers.OrderInfoController orderInfoController = new API.Controllers.OrderInfoController();
        //    API.Controllers.ClothFactoryInfoController clothFactoryInfoController = new API.Controllers.ClothFactoryInfoController();
        //    API.Controllers.ShopInfoController shopInfoController = new API.Controllers.ShopInfoController();
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    List<ClothHandingInfo> clothHandingInfoList = new List<ClothHandingInfo>();
        //    if (!string.IsNullOrEmpty(yNFactoryOrder.cloth_info))
        //    {
        //        clothHandingInfoList = js.Deserialize<List<ClothHandingInfo>>(yNFactoryOrder.cloth_info);
        //    }
        //    List<YNOrderInfo> orderInfoList = new List<YNOrderInfo>();
        //    for (int i = 0; i < clothHandingInfoList.Count; i++)
        //    {
        //        YNOrderInfo yNOrderInfo = orderInfoController.getOrderInfoById(clothHandingInfoList[i].orderInfoId, shopId);
        //        orderInfoList.Add(yNOrderInfo);
        //    }
        //    YNOrder yNOrder = orderController.GetOrderByID(yNFactoryOrder.order_id,shopId);
        //    YNClothFactoryInfo yNClothFactoryInfo = clothFactoryInfoController.GetClothFactoryInfoByID(yNFactoryOrder.cloth_factory_id);
        //    YNShopInfo yNShopInfo = shopInfoController.GetShopInfoByID(shopId);

        //    HSSFWorkbook hSSFWorkbook = InitializeWorkbook("YNDIY_ORDER_GONGCHANG.xls");
        //    string rengongLiangTi = yNOrder.jingShenChai_data;
        //    JingShenChaiTaior jingShenChaiTaior = new JingShenChaiTaior();
        //    if (!string.IsNullOrEmpty(rengongLiangTi))
        //    {
        //        jingShenChaiTaior = js.Deserialize<JingShenChaiTaior>(rengongLiangTi);
        //    }
        //    //转化为工厂接受尺寸
        //    jingShenChaiTaior = jingShenChaiTaior.convertUnity(yNClothFactoryInfo.unit_type);
        //    //体型
        //    string user_tixing = yNOrder.user_tixing;
        //    string[] tixing = user_tixing.Split(',');
        //    string userTixing = "";
        //    for(int i=0;i<tixing.Length;i++){
        //        tixing[i] = API.Controllers.OrderController.getTiXing(tixing[i]);
        //        userTixing += tixing[i] + "  ";
        //    }
        //    int tableCount = 0;
        //    for (int i = 0; i < orderInfoList.Count; i++)
        //    {
        //        JingShenChaiTaior yifuTailor = new JingShenChaiTaior();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].yifu_data))
        //        {
        //            yifuTailor = js.Deserialize<JingShenChaiTaior>(orderInfoList[i].yifu_data);
        //        }
        //        //转化为工厂接受尺寸
        //        yifuTailor = yifuTailor.convertUnity(yNClothFactoryInfo.unit_type);
        //        hSSFWorkbook.SetSheetName(tableCount, yNOrder.members_name + "_" + orderInfoList[i].cloth_name+"_"+(i+1));
        //        HSSFSheet sheet1 = (HSSFSheet)hSSFWorkbook.GetSheetAt(tableCount);
        //        tableCount++;
        //        sheet1.GetRow(0).GetCell(0).SetCellValue(yNShopInfo.shop_name + "生产制单");
        //        sheet1.GetRow(1).GetCell(1).SetCellValue(yNOrder.id);
        //        sheet1.GetRow(1).GetCell(3).SetCellValue(yNOrder.members_name);
        //        sheet1.GetRow(1).GetCell(5).SetCellValue(yNOrder.order_date.ToString("yyyy/MM/dd"));
        //        sheet1.GetRow(1).GetCell(8).SetCellValue(yNOrder.height+"CM");
        //        sheet1.GetRow(1).GetCell(9).SetCellValue(yNOrder.weight+"KG");
        //        sheet1.GetRow(3).GetCell(0).SetCellValue(orderInfoList[i].cloth_name);
        //        sheet1.GetRow(3).GetCell(1).SetCellValue(orderInfoList[i].number);
        //        sheet1.GetRow(3).GetCell(2).SetCellValue(userTixing);
        //        //设置面料
        //        List<FabricPurchaseInfo> fabricPurchaseInfoList = new List<FabricPurchaseInfo>();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].fabricPurchase_info))
        //        {
        //            fabricPurchaseInfoList = js.Deserialize<List<FabricPurchaseInfo>>(orderInfoList[i].fabricPurchase_info);
        //        }
        //        string zhuLiao = "";
        //        string fuLiao = "";
        //        for (int k = 0; k < fabricPurchaseInfoList.Count; k++)
        //        {
        //            if (fabricPurchaseInfoList[k].Type == 0)
        //            {
        //                zhuLiao += fabricPurchaseInfoList[k].Name + "(" + convertFabricUnit(fabricPurchaseInfoList[k].UnitType, 0, fabricPurchaseInfoList[k].Number) + "米)" + "  ";
        //            }
        //            else
        //            {
        //                fuLiao += fabricPurchaseInfoList[k].Name + "(" + convertFabricUnit(fabricPurchaseInfoList[k].UnitType, 0, fabricPurchaseInfoList[k].Number) + "米)" + "  ";
        //            }
        //        }
        //        sheet1.GetRow(4).GetCell(1).SetCellValue(zhuLiao);
        //        sheet1.GetRow(5).GetCell(1).SetCellValue(fuLiao);
        //        //设置量体数据
        //        string unit_name = "";
        //        if (yNClothFactoryInfo.unit_type == 0)
        //        {
        //            unit_name = "CM";
        //        }
        //        else if (yNClothFactoryInfo.unit_type == 1)
        //        {
        //            unit_name = "市寸";
        //        }
        //        else if (yNClothFactoryInfo.unit_type == 2)
        //        {
        //            unit_name = "英寸";
        //        }
        //        sheet1.GetRow(6).GetCell(2).SetCellValue("净体  " + unit_name);
        //        sheet1.GetRow(6).GetCell(3).SetCellValue("成衣  " + unit_name);
        //        int rowIndex = 7;
        //        JingShenChaiTaior tempTailor = new JingShenChaiTaior();
        //        if (API.Controllers.ClothTypeNameController.shangYi.Contains(orderInfoList[i].clothtype_id))
        //        {
        //            tempTailor = jingShenChaiTaior.getYiFuTailor();
        //        }
        //        else if (API.Controllers.ClothTypeNameController.xiaYi.Contains(orderInfoList[i].clothtype_id))
        //        {
        //            tempTailor = jingShenChaiTaior.getKuZiTailor();
        //        }else{
        //            tempTailor = jingShenChaiTaior;
        //        }
        //        foreach (System.Reflection.PropertyInfo p in yifuTailor.GetType().GetProperties())
        //        {
        //            if (p.Name.Contains("0") || p.Name.Contains("1"))
        //            {
        //                object value = p.GetValue(yifuTailor);
        //                object valueNew = p.GetValue(tempTailor);
        //                if ((value == null || value == "") && (valueNew == null || valueNew==""))
        //                {
        //                    continue;
        //                }
        //                sheet1.GetRow(rowIndex).GetCell(1).SetCellValue(yifuTailor.getName(p.Name));
        //                sheet1.GetRow(rowIndex).GetCell(2).SetCellValue(valueNew==null?"":valueNew.ToString());
        //                sheet1.GetRow(rowIndex).GetCell(3).SetCellValue(value==null?"":value.ToString());
        //                rowIndex++;
        //            }
        //        }
        //        //设置款式细节
        //        ClothDetail clothDetail = new ClothDetail();
        //        if (!string.IsNullOrEmpty(orderInfoList[i].style_detail))
        //        {
        //            try
        //            {
        //                clothDetail = js.Deserialize<ClothDetail>(orderInfoList[i].style_detail);
        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }
        //        int _style_count = 7;
        //        if (clothDetail.style != null)
        //        {
        //            for (int k = 0; k < clothDetail.style.Count; k++)
        //            {
        //                sheet1.GetRow(_style_count).GetCell(0).SetCellValue(clothDetail.style[k]);
        //                _style_count++;
        //            }
        //        }
        //        //设置备注
        //        sheet1.GetRow(43).GetCell(0).SetCellValue(clothDetail.remarks);
        //        //设置图片
        //        //设置款式正面图
        //        if (!string.IsNullOrEmpty(orderInfoList[i].cloth_font_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(orderInfoList[i].cloth_font_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 4, 7, 7, 15);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置款式背面图
        //        if (!string.IsNullOrEmpty(orderInfoList[i].cloth_back_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(orderInfoList[i].cloth_back_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 4, 15, 7, 24);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置草图
        //        if (!string.IsNullOrEmpty(orderInfoList[i].design_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(orderInfoList[i].design_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 4, 24, 7, 33);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }

        //        //设置人体正面图
        //        if (!string.IsNullOrEmpty(yNOrder.user_font_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(yNOrder.user_font_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 7, 7, 10, 15);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置人体背面图
        //        if (!string.IsNullOrEmpty(yNOrder.user_back_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(yNOrder.user_back_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 7, 15, 10, 24);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置人体侧面图
        //        if (!string.IsNullOrEmpty(yNOrder.user_side_image))
        //        {
        //            byte[] imgData = getImageDateFromUrl(yNOrder.user_side_image);
        //            int pictureIdx = hSSFWorkbook.AddPicture(imgData, PictureType.JPEG);
        //            HSSFPatriarch patriarch = (HSSFPatriarch)sheet1.CreateDrawingPatriarch();
        //            HSSFClientAnchor anchor = new HSSFClientAnchor(0, 0, 0, 0, 7, 24, 10, 33);
        //            HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
        //        }
        //        //设置联系人信息
        //        sheet1.GetRow(46).GetCell(0).SetCellValue("联系人:" + yNShopInfo.link_man);
        //        sheet1.GetRow(46).GetCell(2).SetCellValue("联系电话:" + yNShopInfo.phone);
        //        sheet1.GetRow(46).GetCell(5).SetCellValue("联系地址:" + yNShopInfo.address_detail);
        //    }

        //    //删除多余的表格数据
        //    for (int i = tableCount; i < 5; i++)
        //    {
        //        hSSFWorkbook.RemoveSheetAt(tableCount);
        //    }
        //    string filename = yNShopInfo.shop_name + "_" + yNOrder.members_name +".xls";
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
        /// 根据图片地址获取图片内容
        /// </summary>
        /// <param name="url"></param>
        private byte[] getImageDateFromUrl(string url)
        {
            try
            {
                Uri mUri = new Uri(url);
                HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(mUri);
                mRequest.Method = "GET";
                mRequest.Timeout = 500;
                HttpWebResponse mResponse = (HttpWebResponse)mRequest.GetResponse();
                Stream mStream = mResponse.GetResponseStream();
                MemoryStream memoryStream = new MemoryStream();
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = mStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                return memoryStream.GetBuffer();
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
