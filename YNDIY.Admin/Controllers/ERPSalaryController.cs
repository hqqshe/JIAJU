using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using YNDIY.API.Controllers;
using YNDIY.API.Models;

namespace YNDIY.Admin.Controllers
{
    public class ERPSalaryController : WebController
    {
        [HttpGet]
        public ActionResult Procedure()
        {
            return View();
        }
        public ActionResult SalayStatistics()
        {
            return View();
        }
  

        public object GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            object value = null;
            try
            {
                if (cell.CellType != CellType.Blank)
                {
                    switch (cell.CellType)
                    {
                        case CellType.Numeric:
                            // Date comes here
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                value = cell.DateCellValue;
                            }
                            else
                            {
                                // Numeric type
                                value = cell.NumericCellValue;
                            }
                            break;
                        case CellType.Boolean:
                            // Boolean type
                            value = cell.BooleanCellValue;
                            break;
                        case CellType.Formula:
                            value = cell.CellFormula;
                            break;
                        default:
                            // String type
                            value = cell.StringCellValue;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                value = "";
            }
            if (value == null)
            {
                value = "";
            }

            return value;
        }   
    }
}