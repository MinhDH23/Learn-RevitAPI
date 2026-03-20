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

                    // ??? vuongldt: code này phải được đưa vào try/catch để tránh lỗi có chứa kí tự chữ sẽ không convert được sang int

                    // Chuyển giá trị "RevSheet" từ string sang int
                    //int revSheet_int = Convert.ToInt32(revSheet);

                    // Tách phần số ở đầu chuỗi (ví dụ: "01A" => "01")
                    string numberPart = "";
                    foreach (char c in revSheet)
                    {
                        if (char.IsDigit(c))
                            numberPart += c;
                        else
                            break; // dừng lại khi gặp ký tự không phải số đầu tiên
                    }

                    // Nếu không tìm thấy số, bỏ qua đối tượng này
                    if (string.IsNullOrEmpty(numberPart))
                    {
                        TaskDialog.Show("Lỗi dữ liệu", $"Không tách được số từ RevSheet: '{revSheet}' trên đối tượng {doiTuong.Id}");
                        continue;
                    }

                    // Cộng thêm 1
                    int revSheet_int = int.Parse(numberPart);
                    int new_RevSheet_int = revSheet_int + 1;

                    // Format lại thành 2 chữ số
                    string new_RevSheet = new_RevSheet_int.ToString("D2");

                    // ??? vuongldt: Thêm điều kiện if chỗ này để kiểm tra xem chuỗi hiện tại có chứa dấu '-' hay không,
                    // nếu không có thì sẽ tạo chuỗi mới theo format "RevSheet" + "- " + giá trị mới của "RevSheet"

                    // Kiểm tra chuỗi có chứa dấu '-' hay không
                    string new_PtaAcceptanceStamp;

                    if (ptaAcceptanceStamp.Contains("-"))
                    {
                        string[] splitStamp = ptaAcceptanceStamp.Split(new char[] { '-' }, 2);
                        string baseStamp = splitStamp[0].Trim();
                        new_PtaAcceptanceStamp = baseStamp + "- " + new_RevSheet_int.ToString("D2");
                    }
                    else
                    {
                        new_PtaAcceptanceStamp = ptaAcceptanceStamp + "- " + new_RevSheet_int.ToString("D2");
                    }

                    // Gán vào parameter "RevSheet"
                    doiTuong.LookupParameter(para_RevSheet).Set(new_RevSheet);

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