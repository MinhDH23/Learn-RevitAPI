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
using static System.Windows.Forms.AxHost;
using Application = Autodesk.Revit.ApplicationServices.Application;
using MessageBox = System.Windows.MessageBox;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
#endregion

namespace Minhdh
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cmd_LenhSo2x : IExternalCommand
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
                // Khai báo tên parameter
                string para_RevSheet = "RevSheet";
                string para_PTAAcceptanceStamp = "PTA Acceptance Stamp";

                ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();

                if (selectedIds.Count == 0)
                {
                    TaskDialog.Show("Thông báo", "Bạn chưa chọn đối tượng nào!");
                    return Result.Failed;
                }

                Transaction trans = new Transaction(doc);
                trans.Start("Cập nhật parameter");

                //Definition: PTA Acceptance Stamp
                foreach (ElementId id in selectedIds)
                {
                    // Lấy về đối tượng sheet từ ID
                    Element doiTuong = doc.GetElement(id);
                    // Xử lý với element (ví dụ: in tên)

                    // Lấy giá trị hiện tại của parameter "PTA Acceptance Stamp"
                    string ptaAcceptanceStamp = doiTuong.LookupParameter(para_PTAAcceptanceStamp).AsValueString();

                    // Lấy giá trị hiện tại của parameter "RevSheet"
                    string revSheet = doiTuong.LookupParameter(para_RevSheet).AsValueString();

                    // vuongldt: code này phải được đưa vào try/catch để tránh lỗi có chứa kí tự chữ sẽ không convert được sang int
                    //
                    //
                    // Chuyển giá trị "RevSheet" từ string sang int
                    int revSheet_int = Convert.ToInt32(revSheet);

                    // Cộng thêm 1 vào giá trị "RevSheet"
                    int new_RevSheet_int = revSheet_int + 1;

                    // Tách chuỗi PTA Acceptance Stamp tại dấu '-'
                    string[] splitStamp = ptaAcceptanceStamp.Split(new char[] { '-' }, 2);
                    string baseStamp = splitStamp[0].Trim();


                    // Tạo chuỗi mới cho parameter "PTA Acceptance Stamp" bằng cách nối giá trị cũ với giá trị mới của "RevSheet"
                    string new_PtaAcceptanceStamp = baseStamp + "- " + new_RevSheet_int.ToString("D2");
                                        
                    // Gán vào parameter "RevSheet"
                    doiTuong.LookupParameter(para_RevSheet).Set(new_RevSheet_int.ToString("D2"));
                    
                    // Gán vào parameter "PTA Acceptance Stamp"
                    doiTuong.LookupParameter(para_PTAAcceptanceStamp).Set(new_PtaAcceptanceStamp);
                                        
                }
                trans.Commit();






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


 


}