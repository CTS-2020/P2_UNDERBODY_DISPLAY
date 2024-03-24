using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private readonly string _connectString;
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
            PageSize = passin.PageSize;
            InitializeComponent();

            SetFormSizeToScreenResolution();

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
        private void InitialiseTopPaddingPanel()
        {
            Panel TopEMptyPanel = new Panel();
            TopEMptyPanel.Dock = DockStyle.Top;
            TopEMptyPanel.Height = 50;
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
            getLogData("", "", "", "", PageNumber, PageSize);

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
        public void getLogData(string model, string carID, string cameraPoint, string PointStatus, int pageNumber, int pageSize)
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
            filteredDataTable.Columns.Remove("Log Date");
            filteredDataTable.Columns.Remove("TotalPage");
            filteredDataTable.Columns.Remove("current_car_model");
            filteredDataTable.Columns.Remove("refresh_indicator");
            LogDataGridViewTable.DataSource = filteredDataTable;
            LogDataGridViewTable.CellFormatting += LogDataGridViewTable_CellFormatting;
        }

        //refresh click action
        private void UpdateLogGridViewTable(string carModel, string carID, string cameraPoint, string PointStatus)
        {
            PageNumber = 1;
            PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            getLogData(carModel, carID, cameraPoint, PointStatus, PageNumber, PageSize);
        }

        private void NextPage(string carModel, string carID, string cameraPoint, string PointStatus)
        {
            //PageNumber = PageNumber + 1;
            PageNumber = Math.Min(PageNumber + 1, TotalPage);
            PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            getLogData(carModel, carID, cameraPoint, PointStatus, PageNumber, PageSize);
        }

        private void PrevPage(string carModel, string carID, string cameraPoint, string PointStatus)
        {
            PageNumber = Math.Max(PageNumber - 1, 1);
            PageLabel.Text = PageNumber.ToString() + "/" + TotalPage.ToString();
            getLogData(carModel, carID, cameraPoint, PointStatus, PageNumber, PageSize);
        }

        public string selectedModel = "";
        public string input_carID = "";
        public string selectedCamera = "";
        public string selectedStatus = "";
        private void SearchBtn_Click(object sender, EventArgs e)
        {
            selectedModel = comboBoxModel.SelectedItem.ToString();
            input_carID = LeftInputTextBox.Text ?? "";
            selectedCamera = PointStatus.SelectedItem.ToString() == "" ? "" : CameraPoint.SelectedItem.ToString();
            selectedStatus = PointStatus.SelectedItem.ToString() == "OK" ? "1" : "0";
            UpdateLogGridViewTable(selectedModel, input_carID, selectedCamera, selectedStatus);
        }
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            selectedModel = "";
            input_carID = "";
            selectedCamera = "";
            selectedStatus = "";
            UpdateLogGridViewTable(selectedModel, input_carID, selectedCamera, selectedStatus);
        }
        private void PrevBtn_Click(object sender, EventArgs e)
        {
            PrevPage(selectedModel, input_carID, selectedCamera, selectedStatus);
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            NextPage(selectedModel, input_carID, selectedCamera, selectedStatus);
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
