using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private readonly string _connectString;
        public readonly string _logDirectory;
        public readonly string _logFilePrefix;
        public string connectionString;
        public int PageSize;
        public int PageNumber = 1;
        private string[] modelArray = { "", "D20N", "D19H", "D27H", "D66B" };
        private string[] CameraArray = { "","v1c1", "v1c2", "v1c3", "v1c4","v1c5", "v1c6", "v1c7", "v1c8", "v1c9", "v1c10",
        "v2c1", "v2c2", "v2c3", "v2c4","v2c5", "v2c6", "v2c7", "v2c8", "v2c9", "v2c10",};
        private string[] PointStatusArray = { "", "OK", "NG" };
        public Form3(PassIn passin)
        {
            _connectString = passin.ConnectionString;
            connectionString = _connectString;
            _logDirectory = passin.LogDirectory;
            _logFilePrefix = passin.LogFilePrefix;
            PageSize = passin.PageSize;
            InitializeComponent();

            SetFormSizeToScreenResolution();
            InitialiseDatePanel();
            InitialiseCameraPointSearchPanel();
            InitializeSearchPart();
            InitialiseTopPaddingPanel();
            InitialNavigateButotn();
            InitialLogTable();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        InputBox LeftInputTextBox = new InputBox();
        DateTimePicker DateFromPicker = new DateTimePicker();
        DateTimePicker DateToPicker = new DateTimePicker();
        ComboBox comboBoxModel = new ComboBox();
        private void InitializeSearchPart()
        {
            Panel SearchPanel = new Panel();
            SearchPanel.Dock = DockStyle.Top;
            SearchPanel.Height = 50;
            //SearchPanel.BackColor = Color.Pink;

            Panel EmptyPanel = new Panel();
            EmptyPanel.Dock = DockStyle.Left;
            //EmptyPanel.BackColor = Color.Black;
            EmptyPanel.Width = 200;

            Panel EmptyPanel2 = new Panel();
            EmptyPanel2.Dock = DockStyle.Right;
            //EmptyPanel2.BackColor = Color.Black;
            EmptyPanel2.Width = 200;

            ButtonStyle filterBtn = new ButtonStyle();
            filterBtn.Text = "Search";
            filterBtn.Click += new EventHandler(SearchBtn_Click);
            //filterBtn.Margin = new Padding(0, 0, 20, 0);
            EmptyPanel2.Controls.Add(filterBtn);

            Panel leftPanelInSearch = new Panel();
            leftPanelInSearch.Width = 400;
            //leftPanelInSearch.BackColor = Color.Purple;
            leftPanelInSearch.Dock = DockStyle.Left;

            Panel RightPanelInSearch = new Panel();
            RightPanelInSearch.Width = leftPanelInSearch.Width;
            //RightPanelInSearch.BackColor = Color.White;
            RightPanelInSearch.Dock = DockStyle.Right;

            LabelDockLeft CarID = new LabelDockLeft();
            LabelDockLeft CarModel = new LabelDockLeft();
            CarID.Text = "Car ID: ";
            CarModel.Text = "Car Model: ";

            int labelWidth = (int)(leftPanelInSearch.Width * 0.3);
            int textBoxWidth = leftPanelInSearch.Width - labelWidth;
            CarID.Width = labelWidth;
            //CarID.BackColor = Color.Red;
            CarModel.Width = labelWidth;
            //CarModel.BackColor = Color.Red;
            LeftInputTextBox.Width = textBoxWidth;
            comboBoxModel.Dock = DockStyle.Right;
            comboBoxModel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxModel.Width = textBoxWidth;
            comboBoxModel.DataSource = modelArray;

            leftPanelInSearch.Controls.Add(LeftInputTextBox);
            leftPanelInSearch.Controls.Add(CarID);
            RightPanelInSearch.Controls.Add(comboBoxModel);
            RightPanelInSearch.Controls.Add(CarModel);

            SearchPanel.Controls.Add(leftPanelInSearch);
            SearchPanel.Controls.Add(EmptyPanel);
            SearchPanel.Controls.Add(RightPanelInSearch);
            SearchPanel.Controls.Add(EmptyPanel2);

            this.Controls.Add(SearchPanel);
        }

        ComboBox CameraPoint = new ComboBox();
        ComboBox PointStatus = new ComboBox();
        private void InitialiseCameraPointSearchPanel()
        {
            Panel CameraSearchPanel = new Panel();
            CameraSearchPanel.Dock = DockStyle.Top;
            CameraSearchPanel.Height = 50;
            //CameraSearchPanel.BackColor = Color.Pink;

            Panel EmptyCameraPanel = new Panel();
            EmptyCameraPanel.Dock = DockStyle.Left;
            //EmptyCameraPanel.BackColor = Color.Black;
            EmptyCameraPanel.Width = 200;

            Panel EmptyCameraPanel2 = new Panel();
            EmptyCameraPanel2.Dock = DockStyle.Right;
            //EmptyCameraPanel2.BackColor = Color.Black;
            EmptyCameraPanel2.Width = 200;

            Panel leftPanelInCameraSearch = new Panel();
            leftPanelInCameraSearch.Width = 400;
            //leftPanelInCameraSearch.BackColor = Color.Purple;
            leftPanelInCameraSearch.Dock = DockStyle.Left;

            Panel RightPanelInCameraSearch = new Panel();
            RightPanelInCameraSearch.Width = leftPanelInCameraSearch.Width;
            //RightPanelInCameraSearch.BackColor = Color.White;
            RightPanelInCameraSearch.Dock = DockStyle.Right;

            LabelDockLeft CameraPointLabel = new LabelDockLeft();
            LabelDockLeft PointStatusLabel = new LabelDockLeft();
            CameraPointLabel.Text = "Camera Point: ";
            PointStatusLabel.Text = "Status: ";

            int labelWidth = (int)(leftPanelInCameraSearch.Width * 0.3);
            int ComboBoxWidth = leftPanelInCameraSearch.Width - labelWidth;
            CameraPointLabel.Width = labelWidth;
            //CameraPointLabel.BackColor = Color.Red;
            PointStatusLabel.Width = labelWidth;
            //PointStatusLabel.BackColor = Color.Red;
            CameraPoint.Width = ComboBoxWidth;
            CameraPoint.Dock = DockStyle.Left;
            CameraPoint.DropDownStyle = ComboBoxStyle.DropDownList;
            CameraPoint.DataSource = CameraArray;
            PointStatus.Dock = DockStyle.Right;
            PointStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            PointStatus.Width = ComboBoxWidth;
            PointStatus.DataSource = PointStatusArray;

            leftPanelInCameraSearch.Controls.Add(CameraPoint);
            leftPanelInCameraSearch.Controls.Add(CameraPointLabel);
            RightPanelInCameraSearch.Controls.Add(PointStatus);
            RightPanelInCameraSearch.Controls.Add(PointStatusLabel);

            CameraSearchPanel.Controls.Add(leftPanelInCameraSearch);
            CameraSearchPanel.Controls.Add(EmptyCameraPanel);
            CameraSearchPanel.Controls.Add(RightPanelInCameraSearch);
            CameraSearchPanel.Controls.Add(EmptyCameraPanel2);

            //TopEMptyPanel.Controls.Add();


            this.Controls.Add(CameraSearchPanel);
        }
        CheckBox checkDate = new CheckBox();
        private void InitialiseDatePanel()
        {
            Panel DateSearchPanel = new Panel();
            DateSearchPanel.Dock = DockStyle.Top;
            DateSearchPanel.Height = 50;

            Panel EmptyDatePanel = new Panel();
            EmptyDatePanel.Dock = DockStyle.Left;
            //EmptyCameraPanel.BackColor = Color.Black;
            EmptyDatePanel.Width = 200;

            Panel EmptyDatePanel2 = new Panel();
            EmptyDatePanel2.Dock = DockStyle.Right;
            //EmptyCameraPanel2.BackColor = Color.Black;
            EmptyDatePanel2.Width = 200;

            checkDate.Dock = DockStyle.Left;
            Panel leftPanelInDateSearch = new Panel();
            leftPanelInDateSearch.Width = 400;
            //leftPanelInCameraSearch.BackColor = Color.Purple;
            leftPanelInDateSearch.Dock = DockStyle.Left;

            Panel RightPanelInDateSearch = new Panel();
            RightPanelInDateSearch.Width = leftPanelInDateSearch.Width;
            //RightPanelInCameraSearch.BackColor = Color.White;
            RightPanelInDateSearch.Dock = DockStyle.Right;

            LabelDockLeft DateFromLabel = new LabelDockLeft();
            LabelDockLeft DateToLabel = new LabelDockLeft();
            LabelDockLeft DateCheckLabel = new LabelDockLeft();
            DateFromLabel.Text = "Date From: ";
            DateToLabel.Text = "Date To: ";
            DateCheckLabel.Text = "Check \nDate";
            DateFromPicker.Dock = DockStyle.Left;
            DateToPicker.Dock = DockStyle.Right;

            EmptyDatePanel2.Controls.Add(checkDate);
            EmptyDatePanel2.Controls.Add(DateCheckLabel);

            int labelWidth = (int)(leftPanelInDateSearch.Width * 0.3);
            int datepickerWidth = (leftPanelInDateSearch.Width - labelWidth);
            DateFromLabel.Width = labelWidth;
            DateFromPicker.Width = datepickerWidth;
            DateToPicker.Width = datepickerWidth;

            leftPanelInDateSearch.Controls.Add(DateFromPicker);
            leftPanelInDateSearch.Controls.Add(DateFromLabel);
            RightPanelInDateSearch.Controls.Add(DateToPicker);
            RightPanelInDateSearch.Controls.Add(DateToLabel);

            DateSearchPanel.Controls.Add(leftPanelInDateSearch);
            DateSearchPanel.Controls.Add(EmptyDatePanel);
            DateSearchPanel.Controls.Add(RightPanelInDateSearch);
            DateSearchPanel.Controls.Add(EmptyDatePanel2);

            this.Controls.Add(DateSearchPanel);
        }
        private void InitialiseTopPaddingPanel()
        {
            Panel TopEMptyPanel = new Panel();
            TopEMptyPanel.Dock = DockStyle.Top;
            TopEMptyPanel.Height = 30;
            //TopEMptyPanel.BackColor = Color.Yellow;

            this.Controls.Add(TopEMptyPanel);
        }
        private void SetFormSizeToScreenResolution()
        {
            this.Width = 1920;
            this.Height = 1080;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        DataGridView LogDataGridViewTable = new DataGridView();
        Label PageLabel = new Label();
        public void InitialLogTable()
        {
            LogDataGridViewTable.Dock = DockStyle.Bottom;
            LogDataGridViewTable.Height = 500;
            LogDataGridViewTable.RowHeadersVisible = true;
            LogDataGridViewTable.ColumnHeadersVisible = true;
            LogDataGridViewTable.ReadOnly = true;
            getLogData("", "", "", "", PageNumber, PageSize, DateFrom, DateTo);

            PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            PageLabel.Dock = DockStyle.Left;
            PageLabel.TextAlign = ContentAlignment.BottomLeft;
            PageLabel.Font = new Font(this.Font.FontFamily, 13);
            NextPrevBtnPanel.Controls.Add(PageLabel);

            this.Controls.Add(LogDataGridViewTable);
        }

        Panel NextPrevBtnPanel = new Panel();
        public void InitialNavigateButotn()
        {

            NextPrevBtnPanel.Height = 50;
            NextPrevBtnPanel.Dock = DockStyle.Bottom;

            ButtonStyle ExcelBtn = new ButtonStyle();
            ExcelBtn.Text = "Export Excel";
            ExcelBtn.Click += new EventHandler(ExcelBtn_Click);
            NextPrevBtnPanel.Controls.Add(ExcelBtn);

            ButtonStyle RefreshBtn = new ButtonStyle();
            RefreshBtn.Text = "Refresh";
            RefreshBtn.Click += new EventHandler(RefreshBtn_Click);
            NextPrevBtnPanel.Controls.Add(RefreshBtn);

            ButtonStyle PrevBtn = new ButtonStyle();
            PrevBtn.Text = "Previous";
            PrevBtn.Click += new EventHandler(PrevBtn_Click);
            NextPrevBtnPanel.Controls.Add(PrevBtn);

            ButtonStyle NextBtn = new ButtonStyle();
            NextBtn.Text = "Next";
            NextBtn.Click += new EventHandler(NextBtn_Click);
            NextPrevBtnPanel.Controls.Add(NextBtn);

            this.Controls.Add(NextPrevBtnPanel);
        }

        DataTable dataTable;
        public int TotalPage;
        public void getLogData(string model, string carID, string cameraPoint, string PointStatus, int pageNumber, int pageSize, string DateFrom, string DateTo)
        {
            if (dataTable != null) dataTable.Clear();
            string storedProcedureName = "PERODUA_GET_LogRecord";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ip_Model", model);
                    command.Parameters.AddWithValue("@ip_carID", carID);
                    command.Parameters.AddWithValue("@ip_cameraPoint", cameraPoint);
                    command.Parameters.AddWithValue("@ip_PointResult", PointStatus);
                    command.Parameters.AddWithValue("@ip_pageNumber", pageNumber);
                    command.Parameters.AddWithValue("@pageSize", pageSize);
                    command.Parameters.AddWithValue("@ip_dateFrom", DateFrom);
                    command.Parameters.AddWithValue("@ip_dateTo", DateTo);
                    connection.Open();
                    dataTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                string totalPageValue = dataTable.Rows.Count > 0 ? dataTable.Rows[0]["TotalPage"].ToString() : "0";
                TotalPage = int.Parse(totalPageValue);
                PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            }
            DataTable filteredDataTable = dataTable.Copy();
            //filteredDataTable.Columns.Remove("Log Date");
            filteredDataTable.Columns.Remove("TotalPage");
            filteredDataTable.Columns.Remove("UID");
            filteredDataTable.Columns.Remove("current_car_model");
            filteredDataTable.Columns.Remove("refresh_indicator");
            LogDataGridViewTable.DataSource = filteredDataTable;
            LogDataGridViewTable.DataBindingComplete += LogDataGridViewTable_DataBindingComplete;

            //LogDataGridViewTable.Columns["Car ID"].Frozen = true;
            //LogDataGridViewTable.Columns["Car Model"].Frozen = true;
            //LogDataGridViewTable.Columns["Log Date"].Frozen = true;

            LogDataGridViewTable.CellFormatting += LogDataGridViewTable_CellFormatting;
        }

        private void LogDataGridViewTable_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Freeze the desired columns after data binding is complete
            LogDataGridViewTable.Columns["Car ID"].Frozen = true;
            LogDataGridViewTable.Columns["Car Model"].Frozen = true;
            LogDataGridViewTable.Columns["Log Date"].Frozen = true;
        }

        //refresh click action
        private void UpdateLogGridViewTable(string carModel, string carID, string cameraPoint, string PointStatus, string DateFrom, string DateTo)
        {
            PageNumber = 1;
            PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            getLogData(carModel, carID, cameraPoint, PointStatus, PageNumber, PageSize, DateFrom, DateTo);
        }

        private void NextPage(string carModel, string carID, string cameraPoint, string PointStatus, string DateFrom, string DateTo)
        {
            //PageNumber = PageNumber + 1;
            PageNumber = Math.Min(PageNumber + 1, TotalPage);
            PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            getLogData(carModel, carID, cameraPoint, PointStatus, PageNumber, PageSize, DateFrom, DateTo);
        }

        private void PrevPage(string carModel, string carID, string cameraPoint, string PointStatus, string DateFrom, string DateTo)
        {
            PageNumber = Math.Max(PageNumber - 1, 1);
            PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            getLogData(carModel, carID, cameraPoint, PointStatus, PageNumber, PageSize, DateFrom, DateTo);
        }

        public string selectedModel = "";
        public string input_carID = "";
        public string selectedCamera = "";
        public string selectedStatus = "";
        public string DateFrom = "";
        public string DateTo = "";
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            selectedModel = comboBoxModel.SelectedItem.ToString();
            input_carID = LeftInputTextBox.Text ?? "";
            selectedCamera = PointStatus.SelectedItem.ToString() == "" ? "" : CameraPoint.SelectedItem.ToString();
            selectedStatus = PointStatus.SelectedItem.ToString() == "OK" ? "1" : "0";
            DateFrom = checkDate.Checked ? DateFromPicker.Value.ToString("yyyy/MM/dd") : "";
            DateTo = checkDate.Checked ? DateToPicker.Value.ToString("yyyy/MM/dd") : "";
            UpdateLogGridViewTable(selectedModel, input_carID, selectedCamera, selectedStatus, DateFrom, DateTo);
        }
        private void ExcelBtn_Click(object sender, EventArgs e)
        {
            if (!checkDate.Checked)
            {
                MessageBox.Show("Please Check Date.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if ((DateToPicker.Value - DateFromPicker.Value).Days >= 31)
            {
                MessageBox.Show("Date Range must be less than 31 days.", "Date Range Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (checkDate.Checked)
            {
                DataTable excelDt = new DataTable();
                string selectedModel = "";
                string input_carID = "";
                string selectedCamera = "";
                string selectedStatus = "";
                string DateFrom = checkDate.Checked ? DateFromPicker.Value.ToString("yyyy/MM/dd") : "";
                string DateTo = checkDate.Checked ? DateToPicker.Value.ToString("yyyy/MM/dd") : "";
                string storedProcedureName = "PERODUA_GET_LogRecord";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ip_Model", selectedModel);
                        command.Parameters.AddWithValue("@ip_carID", input_carID);
                        command.Parameters.AddWithValue("@ip_cameraPoint", selectedCamera);
                        command.Parameters.AddWithValue("@ip_PointResult", selectedStatus);
                        command.Parameters.AddWithValue("@ip_pageNumber", 1);
                        command.Parameters.AddWithValue("@pageSize", 100000);
                        command.Parameters.AddWithValue("@ip_dateFrom", DateFrom);
                        command.Parameters.AddWithValue("@ip_dateTo", DateTo);
                        connection.Open();
                        excelDt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(excelDt);
                        }
                    }
                }
                LogExcel(excelDt);
            }


        }
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            selectedModel = "";
            input_carID = "";
            selectedCamera = "";
            selectedStatus = "";
            DateFrom = "";
            DateTo = "";
            UpdateLogGridViewTable(selectedModel, input_carID, selectedCamera, selectedStatus, DateFrom, DateTo);
        }
        private void PrevBtn_Click(object sender, EventArgs e)
        {
            PrevPage(selectedModel, input_carID, selectedCamera, selectedStatus, DateFrom, DateTo);
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            NextPage(selectedModel, input_carID, selectedCamera, selectedStatus, DateFrom, DateTo);
        }

        private void LogDataGridViewTable_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Check if the cell is in the specified columns (v1c1 to v1c10 and v2c1 to v2c10)
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 &&
                ((e.ColumnIndex >= LogDataGridViewTable.Columns["v1c1"].Index && e.ColumnIndex <= LogDataGridViewTable.Columns["v1c10"].Index) ||
                 (e.ColumnIndex >= LogDataGridViewTable.Columns["v2c1"].Index && e.ColumnIndex <= LogDataGridViewTable.Columns["v2c10"].Index)))
            {
                // Get the cell value
                object cellValue = LogDataGridViewTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // Check the cell value and set the back color accordingly
                if (cellValue != null && cellValue.ToString() == "OK")
                {
                    e.CellStyle.BackColor = COLOR.green;
                }
                else if (cellValue != null && cellValue.ToString() == "NG")
                {
                    e.CellStyle.BackColor = COLOR.red;
                }
                else
                {
                    // Default cell color (no color change)
                    e.CellStyle.BackColor = LogDataGridViewTable.DefaultCellStyle.BackColor;
                }
            }
        }

        private void LogExcel(DataTable logDataTable)
        {
            string fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}{_logFilePrefix}.xlsx";
            string filePath = Path.Combine(_logDirectory, fileName);

            //MessageBox.Show($"Exporting to Excel ({fileName})", "Export Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogForm dialogForm = new DialogForm($"Exporting to Excel ({fileName})");
            dialogForm.Show();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet;
                if (package.Workbook.Worksheets.Count == 0)
                {
                    worksheet = package.Workbook.Worksheets.Add("Log");
                    // Write the headers
                    worksheet.Cells[1, 1].Value = "Time";
                    worksheet.Cells[1, 2].Value = "Car ID";
                    worksheet.Cells[1, 3].Value = "Model";
                    worksheet.Cells[1, 4].Value = "v1c1";
                    worksheet.Cells[1, 5].Value = "v1c2";
                    worksheet.Cells[1, 6].Value = "v1c3";
                    worksheet.Cells[1, 7].Value = "v1c4";
                    worksheet.Cells[1, 8].Value = "v1c5";
                    worksheet.Cells[1, 9].Value = "v1c6";
                    worksheet.Cells[1, 10].Value = "v1c7";
                    worksheet.Cells[1, 11].Value = "v1c8";
                    worksheet.Cells[1, 12].Value = "v1c9";
                    worksheet.Cells[1, 13].Value = "v1c10";
                    worksheet.Cells[1, 14].Value = "v2c1";
                    worksheet.Cells[1, 15].Value = "v2c2";
                    worksheet.Cells[1, 16].Value = "v2c3";
                    worksheet.Cells[1, 17].Value = "v2c4";
                    worksheet.Cells[1, 18].Value = "v2c5";
                    worksheet.Cells[1, 19].Value = "v2c6";
                    worksheet.Cells[1, 20].Value = "v2c7";
                    worksheet.Cells[1, 21].Value = "v2c8";
                    worksheet.Cells[1, 22].Value = "v2c9";
                    worksheet.Cells[1, 23].Value = "v2c10";
                }
                else
                {
                    worksheet = package.Workbook.Worksheets[0];
                }

                int lastRow = worksheet.Dimension?.Rows ?? 1;
                int newRow = lastRow + 1;

                foreach (DataRow logData in logDataTable.Rows)
                {
                    // Write the data
                    if (DateTime.TryParse(logData["Log Date"].ToString(), out DateTime logDate))
                    {
                        worksheet.Cells[newRow, 1].Value = logDate;
                        worksheet.Cells[newRow, 1].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
                    }
                    else
                    {
                        worksheet.Cells[newRow, 1].Value = logData["Log Date"];
                    }
                    worksheet.Cells[newRow, 2].Value = logData["Car ID"];
                    worksheet.Cells[newRow, 3].Value = logData["Car Model"];
                    worksheet.Cells[newRow, 4].Value = logData["v1c1"];
                    worksheet.Cells[newRow, 5].Value = logData["v1c2"];
                    worksheet.Cells[newRow, 6].Value = logData["v1c3"];
                    worksheet.Cells[newRow, 7].Value = logData["v1c4"];
                    worksheet.Cells[newRow, 8].Value = logData["v1c5"];
                    worksheet.Cells[newRow, 9].Value = logData["v1c6"];
                    worksheet.Cells[newRow, 10].Value = logData["v1c7"];
                    worksheet.Cells[newRow, 11].Value = logData["v1c8"];
                    worksheet.Cells[newRow, 12].Value = logData["v1c9"];
                    worksheet.Cells[newRow, 13].Value = logData["v1c10"];
                    worksheet.Cells[newRow, 14].Value = logData["v2c1"];
                    worksheet.Cells[newRow, 15].Value = logData["v2c2"];
                    worksheet.Cells[newRow, 16].Value = logData["v2c3"];
                    worksheet.Cells[newRow, 17].Value = logData["v2c4"];
                    worksheet.Cells[newRow, 18].Value = logData["v2c5"];
                    worksheet.Cells[newRow, 19].Value = logData["v2c6"];
                    worksheet.Cells[newRow, 20].Value = logData["v2c7"];
                    worksheet.Cells[newRow, 21].Value = logData["v2c8"];
                    worksheet.Cells[newRow, 22].Value = logData["v2c9"];
                    worksheet.Cells[newRow, 23].Value = logData["v2c10"];

                    for (int col = 4; col <= 23; col++)
                    {
                        string cellValue = worksheet.Cells[newRow, col].Text;
                        if (cellValue == "OK")
                        {
                            worksheet.Cells[newRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[newRow, col].Style.Fill.BackgroundColor.SetColor(Color.Green);
                        }
                        else if (cellValue == "NG")
                        {
                            worksheet.Cells[newRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[newRow, col].Style.Fill.BackgroundColor.SetColor(Color.Red);
                        }
                    }

                    newRow++;
                }

                package.Save();
            }
            //MessageBox.Show($"Exporting finished. ({fileName})", "Export Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dialogForm.UpdateStatus($"Exporting finished ({fileName}).");
        }

    }

    public class ButtonStyle : Button
    {
        public ButtonStyle()
        {
            this.Dock = DockStyle.Right;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Width = 100;
        }
    }
    public class LabelDockLeft : Label
    {
        public LabelDockLeft()
        {
            this.Dock = DockStyle.Left;
            this.TextAlign = ContentAlignment.TopCenter;
            this.Font = new Font(this.Font.FontFamily, 13);
        }
    }
    public class InputBox : TextBox
    {
        public InputBox()
        {
            this.Dock = DockStyle.Left;
            //this.BackColor = Color.Yellow;
        }
    }
}
