using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.UI;

namespace Stock_Recording
{
    public partial class XtraForm1 : DevExpress.XtraEditors.XtraForm
    {

        BLL.BLL_CS_StockRecoding bll = new BLL.BLL_CS_StockRecoding();
        PAL.rpt_StockRecording report = new PAL.rpt_StockRecording();
        string r_whcode;

        public XtraForm1()
        {
            InitializeComponent();
             
        }
        private void gridControl1_Click(object sender, EventArgs e)
        {
            var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null) return;

            int selectedRow = view.FocusedRowHandle;
            if (selectedRow >= 0)
            {
                object recievedptQty = view.GetRowCellValue(selectedRow, "GRN_Qty");


                view.SetRowCellValue(selectedRow, "Accepted_Qty", recievedptQty);

                string remark = XtraInputBox.Show("Enter remark for this row:", "Input Remark", "");
                if (remark != null)
                {
                    view.SetRowCellValue(selectedRow, "Remark", remark);
                }
                else
                {

                    view.SetRowCellValue(selectedRow, "Remark", "No remark provided");
                }
                
            }
        }



        private bool ValidateStockRecording(DataRow row)
        {
            try
            {
                decimal receivedQty = Convert.ToDecimal(row["GRN_Qty"] ?? 0);
                decimal balanceQty = Convert.ToDecimal(row["Bal_Qty"] ?? 0);
                string docType = row["Doc_Type"].ToString();
                DateTime? receivedDate = string.IsNullOrEmpty(row["GRN_Date"].ToString()) ?
                    (DateTime?)null : Convert.ToDateTime(row["GRN_Date"]);


                if (receivedQty == 0.00m)
                {
                    XtraMessageBox.Show("Received quantity cannot be Zero", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }


                //if (receivedQty > 0.00m && docType == "MRQ")
                //{
                //    if (receivedQty > balanceQty)
                //    {
                //        XtraMessageBox.Show("Received quantity cannot be greater than balance",
                //            "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        return false;
                //    }
                //}


                if (receivedDate.HasValue && receivedQty <= 0)
                {
                    XtraMessageBox.Show("Received quantity cannot be zero when date is provided",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }


                if (receivedQty > 0.00m && docType == "MRQ" && !receivedDate.HasValue)
                {
                    XtraMessageBox.Show("Date cannot be blank when received quantity is provided",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }


                if (receivedDate.HasValue && receivedDate > DateTime.Now)
                {
                    XtraMessageBox.Show("Received date cannot be greater than current date",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }


                row["GRN_Qty"] = receivedQty;

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Validation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



        private void XtraForm1_Load(object sender, EventArgs e)
        {

            //int currentYear = DateTime.Now.Year;
            //for (int i = currentYear; i >= 2010; i--)
            //{
            //    cmbYear.Properties.Items.Add(i.ToString());
            //}

            //cmbYear.Text = currentYear.ToString();


            //string type = cmb_type.Text;
            //string selectedYear = cmbYear.Text;

            //DataTable dtMain = bll.LoadStockRecording(type, selectedYear);

            //if (!dtMain.Columns.Contains("Checkbox"))
            //{
            //    dtMain.Columns.Add("Checkbox", typeof(bool));
            //}
            //if (!splashScreenManager1.IsSplashFormVisible)
            //    splashScreenManager1.ShowWaitForm();
            //gc_Stockecording.DataSource = dtMain;
            //splashScreenManager1.CloseWaitForm();
            //if (!splashScreenManager1.IsSplashFormVisible)
            //    splashScreenManager1.ShowWaitForm();
            //DataTable dtConfirm = bll.LoadStockRecording_C(selectedYear);
            //gc_StockRecording_C.DataSource = dtConfirm;
            //splashScreenManager1.CloseWaitForm();


            //--------------------------

            string type = "";
            string selectedYear = "";

            int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= 2010; i--)
            {
                cmbYear.Properties.Items.Add(i.ToString());
            }

            cmbYear.Text = currentYear.ToString();
             type = cmb_type.Text;
             selectedYear = cmbYear.Text;

            if (!splashScreenManager1.IsSplashFormVisible)
                splashScreenManager1.ShowWaitForm();

            gc_Stockecording.DataSource = bll.LoadStockRecording(type, selectedYear);

            gc_StockRecording_C.DataSource = bll.LoadStockRecording_C(selectedYear);

            splashScreenManager1.CloseWaitForm();




        }

        private void gc_StockRecording_C_Click(object sender, EventArgs e)
        {

        }

        private void labelControl4_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxEdit1_Properties_EditValueChanged(object sender, EventArgs e)
        {
            String type = cmb_type.Text;
            //gc_Stockecording.DataSource = bll.LoadStockRecording(type);

        }

        private void gv_Stockecording_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //------------- code
        }

        private void gv_Stockecording_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "Checkbox")
            {
                //bool isChecked = Convert.ToBoolean(e.Value);
                //if (isChecked)
                //{
                //    gv_Stockecording.SetRowCellValue(e.RowHandle, "Accepted_Qty", gv_Stockecording.GetFocusedRowCellValue("GRN_Qty"));
                //}

            }
        }


        private void gv_Stockecording_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Surftreat_Status")
            {
                string cellValue = e.CellValue?.ToString();
                if (cellValue == "Blasting and Painting")
                {
                    e.Appearance.BackColor = Color.Green;
                }

                else if (cellValue == "Only Blasting")
                {
                    e.Appearance.BackColor = Color.Yellow;
                }

                else if (cellValue == "Without Treatment")
                {
                    e.Appearance.BackColor = Color.Red;
                }
            }
        }

        private void gv_StockRecording_C_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column.FieldName == "Surface_Treatment_Status")
            {
                string cellValue = e.CellValue?.ToString();
                if (cellValue == "Blasting and Painting")
                {
                    e.Appearance.BackColor = Color.Green;
                }
                else if (cellValue == "Only Blasting")
                {
                    e.Appearance.BackColor = Color.Yellow;
                }

                else if (cellValue == "Without Treatment")
                {
                    e.Appearance.BackColor = Color.Red;
                }
            }
        }


        public class StockRecordingData
        {
            public string LocationCode { get; set; }
            public string DocumentType { get; set; }
            public string DocumentNo { get; set; }
            public string MaterialCode { get; set; }
            public string Line { get; set; }
            public string EngScheduleNo { get; set; }
            public decimal ReceivedQty { get; set; }
            public DateTime? Date { get; set; }
            public string Remarks { get; set; }
            public string WH_Code { get; set; }
            public string Doc_Type { get; set; }
            public string GRN_No { get; set; }
            public string Material_Code { get; set; }
            public string Schedule_No { get; set; }
            public decimal MRQ_Issue { get; set; }
            public string Remark { get; set; }
        }


        private void btn_save_Click(object sender, EventArgs e)
        {
          


            int rowCount = gv_Stockecording.DataRowCount;
            string type = cmb_type.Text;

            List<string> selectedDocNos = new List<string>();
            List<string> selectedLineNo = new List<string>();

            
            string lastLocationCode = "", lastDocumentType = "", lastDocumentNo = "",
                   lastMaterialCode = "", lastLine = "", lastEngScheduleNo = "";

            try
            {
                DataTable dt = gc_Stockecording.DataSource as DataTable;
                if (dt == null) return;

                for (int rowHandle = 0; rowHandle < rowCount; rowHandle++)
                {
                    object cellValue = gv_Stockecording.GetRowCellValue(rowHandle, "Checkbox");
                    bool isSelected = cellValue != null && bool.TryParse(cellValue.ToString(), out bool value) && value;

                    if (isSelected)
                    {
                        DataRow row = gv_Stockecording.GetDataRow(rowHandle);
                        if (row == null)
                            continue;

                        if (ValidateStockRecording(row))
                        {
                            decimal receivedQty = Convert.ToDecimal(gv_Stockecording.GetRowCellValue(rowHandle, "GRN_Qty") ?? 0);
                            if (receivedQty > 0)
                            {
                                string locationCode = gv_Stockecording.GetRowCellValue(rowHandle, "WH_Code").ToString();
                                string documentType = gv_Stockecording.GetRowCellValue(rowHandle, "Doc_Type").ToString();
                                string documentNo = gv_Stockecording.GetRowCellValue(rowHandle, "GRN_No").ToString();
                                string materialCode = gv_Stockecording.GetRowCellValue(rowHandle, "Material_Code").ToString();
                                string line = gv_Stockecording.GetRowCellValue(rowHandle, "line").ToString();
                                string engScheduleNo = gv_Stockecording.GetRowCellValue(rowHandle, "Schedule_No").ToString();

                                
                                lastLocationCode = locationCode;
                                lastDocumentType = documentType;
                                lastDocumentNo = documentNo;
                                lastMaterialCode = materialCode;
                                lastLine = line;
                                lastEngScheduleNo = engScheduleNo;

                                selectedDocNos.Add(documentNo);
                                selectedLineNo.Add(line);
                            }
                            else
                            {
                                XtraMessageBox.Show("No records to save. Please enter received quantities.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }

                
                if (selectedDocNos.Any() && selectedLineNo.Any())
                {
                    


                    PAL.rpt_StockRecording report = new PAL.rpt_StockRecording();
                    report.DataSource = bll.printReport(
                        lastLocationCode, lastDocumentType, lastDocumentNo,
                        lastMaterialCode, lastLine, lastEngScheduleNo,
                        selectedDocNos.Distinct().ToList(),
                        selectedLineNo.Distinct().ToList()
                    );

                    string time = DateTime.Now.ToString("HH:mm:ss");
                    string sdate = DateTime.Now.ToString("yyyy-MM-dd");
                    report.Parameters["paramTime"].Value = DateTime.Now.ToString("HH:mm");
                    report.Parameters["paramRdate"].Value = sdate;

                    ReportPrintTool printTool = new ReportPrintTool(report);
                    printTool.ShowRibbonPreview(UserLookAndFeel.Default);
                }

                
                for (int rowHandle = 0; rowHandle < rowCount; rowHandle++)
                {
                    object cellValue = gv_Stockecording.GetRowCellValue(rowHandle, "Checkbox");
                    bool isSelected = cellValue != null && bool.TryParse(cellValue.ToString(), out bool value) && value;

                    if (isSelected)
                    {
                        DataRow row = gv_Stockecording.GetDataRow(rowHandle);
                        if (row == null)
                            continue;

                        if (ValidateStockRecording(row))
                        {
                            decimal receivedQty = Convert.ToDecimal(gv_Stockecording.GetRowCellValue(rowHandle, "GRN_Qty") ?? 0);
                            if (receivedQty > 0)
                            {
                                string locationCode = gv_Stockecording.GetRowCellValue(rowHandle, "WH_Code").ToString();
                                string documentType = gv_Stockecording.GetRowCellValue(rowHandle, "Doc_Type").ToString();
                                string documentNo = gv_Stockecording.GetRowCellValue(rowHandle, "GRN_No").ToString();
                                string materialCode = gv_Stockecording.GetRowCellValue(rowHandle, "Material_Code").ToString();
                                string line = gv_Stockecording.GetRowCellValue(rowHandle, "line").ToString();
                                string engScheduleNo = gv_Stockecording.GetRowCellValue(rowHandle, "Schedule_No").ToString();
                                string date = gv_Stockecording.GetRowCellValue(rowHandle, "GRN_Date").ToString();
                                string receivedQtyStr = gv_Stockecording.GetRowCellValue(rowHandle, "GRN_Qty").ToString();
                                string remarks = gv_Stockecording.GetRowCellValue(rowHandle, "Remark").ToString();

                                bll.Insertstock1(locationCode, documentType, documentNo, materialCode, line, engScheduleNo, receivedQtyStr, date, remarks);
                            }
                        }
                    }
                }

                if (!splashScreenManager1.IsSplashFormVisible)
                    splashScreenManager1.ShowWaitForm();

                string selectedYear = cmbYear.SelectedItem?.ToString();
                gc_StockRecording_C.DataSource = bll.LoadStockRecording_C(selectedYear);

                gc_Stockecording.DataSource = bll.LoadStockRecording(type, selectedYear);

                splashScreenManager1.CloseWaitForm();

                XtraMessageBox.Show("Saved Successfully", "info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }





        private void labelControl1_Click(object sender, EventArgs e)
        {

        }

        private void labelControl2_Click(object sender, EventArgs e)
        {

        }

        private void labelControl3_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void btn_details_Click(object sender, EventArgs e)
        {
            string test = gv_Stockecording.GetFocusedRowCellValue("Material_Code").ToString();
            string lac = gv_Stockecording.GetFocusedRowCellValue("WH_Code").ToString();
            string docno = gv_Stockecording.GetFocusedRowCellValue("GRN_No").ToString();
            string doct = gv_Stockecording.GetFocusedRowCellValue("Doc_Type").ToString();
            string line = gv_Stockecording.GetFocusedRowCellValue("KRY_Remark").ToString();
            PAL.FRM fRM = new PAL.FRM(test, lac, docno, doct, line);
            fRM.ShowDialog();
        }

        private void btn_detail2_Click(object sender, EventArgs e)
        {
            string lac1 = gv_StockRecording_C.GetFocusedRowCellValue("Material_Code").ToString();
            string whcode = gv_StockRecording_C.GetFocusedRowCellValue("WH_Code").ToString();
            string docno1 = gv_StockRecording_C.GetFocusedRowCellValue("Document_No").ToString();
            string doct1 = gv_StockRecording_C.GetFocusedRowCellValue("Doc_Type").ToString();
            string line1 = gv_StockRecording_C.GetFocusedRowCellValue("KRY_Remark").ToString();
            PAL.FRM fRM = new PAL.FRM(lac1, whcode, docno1, doct1);
            fRM.ShowDialog();
        }

        private void btn_print_Click(object sender, EventArgs e)
        //{
        //    List<string> selectedDocNos = new List<string>();
        //    List<string> selectedLineNo = new List<string>();


        //    string lastLocationCode = "", lastDocumentType = "", lastDocumentNo = "",
        //           lastMaterialCode = "", lastLine = "", lastEngScheduleNo = "";
        //    try
        //    {
        //        int rowHandle = gv_StockRecording_C.FocusedRowHandle;

        //        if (rowHandle < 0)
        //        {
        //            XtraMessageBox.Show("Please select a row to print.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        DataRow row = gv_StockRecording_C.GetDataRow(rowHandle);
        //        if (row == null)
        //        {
        //            XtraMessageBox.Show("Selected row data not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }


        //        string locationCode = row["WH_Code "].ToString();
        //        string documentType = row["Doc_Type"].ToString();
        //        string documentNo = row["Document_No"].ToString();
        //        string materialCode = row["Material_Code"].ToString();
        //        string line = row["line"].ToString();
        //        string engScheduleNo = row["Schedule_No"].ToString();

        //        PAL.rpt_Print report = new PAL.rpt_Print();
        //        report.DataSource = bll.pntReport(
        //            locationCode, documentType, documentNo,
        //            materialCode, line, engScheduleNo,
        //            new List<string> { documentNo },
        //            new List<string> { line }
        //        );

        //        report.Parameters["paramTime"].Value = DateTime.Now.ToString("HH:mm");
        //        report.Parameters["paramRdate"].Value = DateTime.Now.ToString("yyyy-MM-dd");

        //        ReportPrintTool printTool = new ReportPrintTool(report);
        //        printTool.ShowRibbonPreview(UserLookAndFeel.Default);
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show($"Error while printing: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        {
            try
            {
                int rowCount = gv_StockRecording_C.DataRowCount;
                List<string> selectedDocNos = new List<string>();
                List<string> selectedLineNo = new List<string>();

                string lastLocationCode = "", lastDocumentType = "", lastDocumentNo = "",
                       lastMaterialCode = "", lastLine = "", lastEngScheduleNo = "";

                DataTable dt = gc_StockRecording_C.DataSource as DataTable;
                if (dt == null) return;

                for (int rowHandle = 0; rowHandle < rowCount; rowHandle++)
                {
                    object cellValue = gv_StockRecording_C.GetRowCellValue(rowHandle, "checkbox");
                    bool isSelected = cellValue != null && bool.TryParse(cellValue.ToString(), out bool value) && value;

                    if (isSelected)
                    {
                        DataRow row = gv_StockRecording_C.GetDataRow(rowHandle);
                        if (row == null)
                            continue;

                        string locationCode = gv_StockRecording_C.GetRowCellValue(rowHandle, "WH_Code").ToString();
                        string documentType = gv_StockRecording_C.GetRowCellValue(rowHandle, "Doc_Type").ToString();
                        string documentNo = gv_StockRecording_C.GetRowCellValue(rowHandle, "Document_No").ToString();
                        string materialCode = gv_StockRecording_C.GetRowCellValue(rowHandle, "Material_Code").ToString();
                        string line = gv_StockRecording_C.GetRowCellValue(rowHandle, "line").ToString();
                        string engScheduleNo = gv_StockRecording_C.GetRowCellValue(rowHandle, "Schedule_No").ToString();

                        lastLocationCode = locationCode;
                        lastDocumentType = documentType;
                        lastDocumentNo = documentNo;
                        lastMaterialCode = materialCode;
                        lastLine = line;
                        lastEngScheduleNo = engScheduleNo;

                        selectedDocNos.Add(documentNo);
                        selectedLineNo.Add(line);
                    }
                }

                if (selectedDocNos.Any() && selectedLineNo.Any())
                {
                    PAL.rpt_Print report = new PAL.rpt_Print();
                    report.DataSource = bll.pntReport(
                        lastLocationCode, lastDocumentType, lastDocumentNo,
                        lastMaterialCode, lastLine, lastEngScheduleNo,
                        selectedDocNos, selectedLineNo
                    );

                    report.Parameters["paramTime"].Value = DateTime.Now.ToString("HH:mm");
                    report.Parameters["paramRdate"].Value = DateTime.Now.ToString("yyyy-MM-dd");

                    ReportPrintTool printTool = new ReportPrintTool(report);
                    printTool.ShowRibbonPreview(UserLookAndFeel.Default);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Error while printing: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void labelControl5_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxEdit1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //string type = cmb_type.Text;
            //string selectedYear = cmbYear.Text;


            //DataTable dtMain = bll.LoadStockRecording(type, selectedYear);
            //if (!dtMain.Columns.Contains("Checkbox"))
            //{
            //    dtMain.Columns.Add("Checkbox", typeof(bool));
            //}
            //if (!splashScreenManager1.IsSplashFormVisible)
            //    splashScreenManager1.ShowWaitForm();

            //gc_Stockecording.DataSource = dtMain;

            //DataTable dtConfirm = bll.LoadStockRecording_C(selectedYear);
            //gc_StockRecording_C.DataSource = dtConfirm;

            //splashScreenManager1.CloseWaitForm();

        }

        private void cmbYear_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            string type = "";
            string selectedYear = "";

                 

            selectedYear = cmbYear.Text;


            if (!splashScreenManager1.IsSplashFormVisible)
                splashScreenManager1.ShowWaitForm();

            gc_Stockecording.DataSource = bll.LoadStockRecording(type, selectedYear);
            gc_StockRecording_C.DataSource = bll.LoadStockRecording_C(selectedYear);

            splashScreenManager1.CloseWaitForm();
        }
    }
}
  


   



