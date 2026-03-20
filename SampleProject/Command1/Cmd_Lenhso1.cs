#region Namespaces
// For Revit API
using Autodesk.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.PointClouds;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Xml.Linq;
using Application = Autodesk.Revit.ApplicationServices.Application;
using MessageBox = System.Windows.MessageBox;
#endregion

namespace Minhdh
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cmd_LenhSo1x : IExternalCommand
    {
        public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //-----------------------------------------------------
            // Code here
            try
            {
                // ► Yêu cầu user click chọn 1 đối tượng bất kỳ
                Reference pickedRef = uidoc.Selection.PickObject(ObjectType.Element, "👉 Hãy click chọn 1 đối tượng trong mô hình");

                // ► Lấy đối tượng từ Reference vừa pick
                Element pickedElement = doc.GetElement(pickedRef);

                // ► Lấy tên và ID của đối tượng
                //string tenDoiTuong = pickedElement.Name;
                //ElementId idDoiTuong = pickedElement.Id;

                // Lấy thông tin từ parameter "Height Offset From Level" và "Comments"
                double heightOffset = pickedElement.LookupParameter("Height Offset From Level").AsDouble();
                string comments = pickedElement.LookupParameter("Comments").AsValueString();

                double heightOffset_mm = UnitConverter.FeetToMm(heightOffset);  // Đổi sang mm để dễ hiểu hơn

                // Gán thông tin vào parameter
                double newHeightOffset_mm = heightOffset_mm + 100;  // Ví dụ: tăng thêm 100mm
                double newHeightOffset_feet = UnitConverter.MmToFeet(newHeightOffset_mm);  // Đổi ngược lại sang feet để lưu vào Revit

                // Cộng thêm 100mm vào giá trị gốc với đơn vị feet
                double newHeightOffset_feet_V2 = heightOffset + UnitConverter.MmToFeet(100);

                Transaction trans = new Transaction(doc);
                trans.Start("Cập nhật parameter");

                // Gán vào parameter "Height Offset From Level"
                pickedElement.LookupParameter("Height Offset From Level").Set(newHeightOffset_feet);

                pickedElement.LookupParameter("Comments").Set(comments + " - Giá trị mới");


                trans.Commit();

                //MessageBox.Show("Tên Đối Tượng Là: " + tenDoiTuong + "\nID Đối tượng là: " + idDoiTuong, "Tiêu đề ở đây!!!!!!");

                //Transaction trans = new Transaction(doc);
                //trans.Start("Testing");



                //trans.Commit();

                return Autodesk.Revit.UI.Result.Succeeded;  // Trả về kết quả thành công.
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // ► User bấm ESC — không làm gì cả, thoát bình thường
                return Result.Cancelled;
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
                MessageBox.Show(message, "Lỗi rồi bạn ơi!!!");
                return Result.Failed;
            }




        }

        //=============================================================
        // FUNCTION HERE

    }

    public static class UnitConverter
    {
        /// <summary>
        /// Đổi từ feet (internal unit của Revit) → mm
        /// Dùng khi mày lấy giá trị từ Revit ra để hiển thị cho người dùng
        /// </summary>
        public static double FeetToMm(double feet)
        {
            return UnitUtils.ConvertFromInternalUnits(feet, UnitTypeId.Millimeters);
        }

        /// <summary>
        /// Đổi từ mm → feet (internal unit của Revit)
        /// Dùng khi mày nhận giá trị mm từ người dùng, rồi đẩy vào Revit
        /// </summary>
        public static double MmToFeet(double mm)
        {
            return UnitUtils.ConvertToInternalUnits(mm, UnitTypeId.Millimeters);
        }
    }



}