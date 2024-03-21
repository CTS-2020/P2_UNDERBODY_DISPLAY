
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using static WindowsFormsApp1.Form1;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using System.Xml.Linq;
using System.Linq;
using System.Reflection;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly string _connectString;
        public string connectionString;
        public Form1(string connectString)
        {
            _connectString = connectString;
            connectionString = _connectString;
            InitializeComponent();
            SetFormSizeToScreenResolution();
            InitializePart();
            InitializeTimer();
            this.Resize += ArrowPanel_Resize;
        }

        private Timer dataUpdateTimer;
        private void InitializeTimer()
        {
            dataUpdateTimer = new Timer();
            dataUpdateTimer.Interval = 1000;
            dataUpdateTimer.Tick += RefreshTimer_Tick;
            dataUpdateTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            GetRefreshInd();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //int rowCount = tableLayoutPanel.RowCount;
            //int columnCount = tableLayoutPanel.ColumnCount;
            //int columnWidth = tableLayoutPanel.Width / columnCount;

            //// Wait for the form to be fully loaded
            //TableLayoutPanelCellPosition cellPosition = new TableLayoutPanelCellPosition(0, 0);

            //// Calculate center point at the top border of the first cell
            //int cellWidth = tableLayoutPanel.GetColumnWidths()[cellPosition.Column];
            //int cellHeight = tableLayoutPanel.GetRowHeights()[cellPosition.Row];
            //Point centerPoint = new Point(cellWidth / 2, cellHeight * cellPosition.Row);
            ArrowPanel_Resize(sender, e);
            this.Paint += Arrow_Draw;
            //Arrow_Draw(sender, e);
            //this.Paint += Arrow_Draw;
        }

        private void SetFormSizeToScreenResolution()
        {
            // Get the primary screen resolution
            Rectangle resolution = Screen.PrimaryScreen.Bounds;

            // Set the size of the form to match the screen resolution
            this.Width = resolution.Width;
            this.Height = resolution.Height;

            // Optionally, you can center the form on the screen
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public static class SIDE
        {
            public static readonly string Left = "LH";
            public static readonly string Right = "RH";
        }

        public static class MODEL_IMAGE
        {
            public static readonly string D20N = "1_L.jpg";
            public static readonly string D19H = "2_L.png";
            public static readonly string D27H = "3_L.png";
            public static readonly string D66B = "";
        }

        public static class MODEL_LOGO
        {
            public static readonly string D20N = "Myvi_Logo.jpg";
            public static readonly string D19H = "Aruz_Logo.jpg";
            public static readonly string D27H = "Alza_Logo.png";
            public static readonly string D66B = "";
        }

        public static class COLOR
        {
            public static readonly Color green = ColorTranslator.FromHtml("#00B050");
            public static readonly Color blue = ColorTranslator.FromHtml("#00B0F0");
            public static readonly Color red = ColorTranslator.FromHtml("#FF0000");
        }


        public static readonly Dictionary<string, string> MODEL_NAME = new Dictionary<string, string>()
        {
            { "1", "D20N"},
            { "2", "D19H"},
            { "3", "D27H"},
            { "4", "D66B"}
        };


        public string mapModelImage(string modelNumber, string type)
        {
            Dictionary<string, Dictionary<string, string>> modelTypeMap = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "Image", new Dictionary<string, string>()
                    {
                        { "1", MODEL_IMAGE.D20N },
                        { "2", MODEL_IMAGE.D19H },
                        { "3", MODEL_IMAGE.D27H },
                        { "4", MODEL_IMAGE.D66B }
                    }
                },
                {
                    "Logo", new Dictionary<string, string>()
                    {
                        { "1", MODEL_LOGO.D20N },
                        { "2", MODEL_LOGO.D19H },
                        { "3", MODEL_LOGO.D27H },
                        { "4", MODEL_LOGO.D66B }
                    }
                }
            };
            if (modelTypeMap.ContainsKey(type))
            {
                if (modelTypeMap[type].ContainsKey(modelNumber))
                {
                    return modelTypeMap[type][modelNumber];
                }
            }
            return null;
        }

        DataSet ds;
        DataTable dataTable;
        public string Model;
        public string CarID;

        PictureBox modelBox = new PictureBox();
        TableLayoutPanel DetailsPanel = new TableLayoutPanel();
        ModelLabeledValue ModelID = new ModelLabeledValue();
        ModelLabeledValue VehicleID = new ModelLabeledValue();
        ModelLabeledValue Date = new ModelLabeledValue();
        ModelLabeledValue Time = new ModelLabeledValue();
        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
        PictureBox ImageBox = new PictureBox();

        BlueLabeledValue V1C1 = new BlueLabeledValue();
        BlueLabeledValue V1C2 = new BlueLabeledValue();
        BlueLabeledValue V1C3 = new BlueLabeledValue();
        BlueLabeledValue V1C4 = new BlueLabeledValue();
        BlueLabeledValue V1C5 = new BlueLabeledValue();
        BlueLabeledValue V1C6 = new BlueLabeledValue();
        BlueLabeledValue V1C7 = new BlueLabeledValue();
        BlueLabeledValue V1C8 = new BlueLabeledValue();
        BlueLabeledValue V1C9 = new BlueLabeledValue();
        BlueLabeledValue V1C10 = new BlueLabeledValue();

        LabeledValue V1C1Value = new LabeledValue();
        LabeledValue V1C2Value = new LabeledValue();
        LabeledValue V1C3Value = new LabeledValue();
        LabeledValue V1C4Value = new LabeledValue();
        LabeledValue V1C5Value = new LabeledValue();
        LabeledValue V1C6Value = new LabeledValue();
        LabeledValue V1C7Value = new LabeledValue();
        LabeledValue V1C8Value = new LabeledValue();
        LabeledValue V1C9Value = new LabeledValue();
        LabeledValue V1C10Value = new LabeledValue();

        private void GetData()
        {
            if (dataTable != null) dataTable.Clear();
            string storedProcedureName = "PERODUA_GET_Details";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    dataTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    Model = dataTable.Rows[0]["current_car_model"].ToString();
                    CarID = dataTable.Rows[0]["car_id"].ToString();
                    InsLog(dataTable);
                }
            }
            UpdateCheckpointPanel();
            UpdateImage();
            UpdateDetails();
        }

        DataTable refreshIndDataTable = new DataTable();
        public bool refreshInd;
        private void GetRefreshInd()
        {
            if (dataTable != null) dataTable.Clear();
            string storedProcedureName = "PERODUA_GET_RefreshInd";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    refreshIndDataTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(refreshIndDataTable);
                    }
                    refreshInd = bool.Parse(refreshIndDataTable.Rows[0]["refresh_indicator"].ToString());
                }
            }

            if (refreshInd == true) GetData();
        }

        private void InsLog(DataTable data)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("car_id", typeof(string));
            dt.Columns.Add("current_car_model", typeof(string));
            dt.Columns.Add("refresh_indicator", typeof(bool));
            dt.Columns.Add("v1c1", typeof(bool));
            dt.Columns.Add("v1c2", typeof(bool));
            dt.Columns.Add("v1c3", typeof(bool));
            dt.Columns.Add("v1c4", typeof(bool));
            dt.Columns.Add("v1c5", typeof(bool));
            dt.Columns.Add("v1c6", typeof(bool));
            dt.Columns.Add("v1c7", typeof(bool));
            dt.Columns.Add("v1c8", typeof(bool));
            dt.Columns.Add("v1c9", typeof(bool));
            dt.Columns.Add("v1c10", typeof(bool));
            dt.Columns.Add("v2c1", typeof(bool));
            dt.Columns.Add("v2c2", typeof(bool));
            dt.Columns.Add("v2c3", typeof(bool));
            dt.Columns.Add("v2c4", typeof(bool));
            dt.Columns.Add("v2c5", typeof(bool));
            dt.Columns.Add("v2c6", typeof(bool));
            dt.Columns.Add("v2c7", typeof(bool));
            dt.Columns.Add("v2c8", typeof(bool));
            dt.Columns.Add("v2c9", typeof(bool));
            dt.Columns.Add("v2c10", typeof(bool));

            DataRow dr = dt.NewRow();
            dr["car_id"] = data.Rows[0]["car_id"];
            dr["current_car_model"] = data.Rows[0]["current_car_model"];
            dr["refresh_indicator"] = data.Rows[0]["refresh_indicator"];
            dr["v1c1"] = data.Rows[0]["v1c1"].ToString() == "OK" ? 1 : 0;
            dr["v1c2"] = data.Rows[0]["v1c2"].ToString() == "OK" ? 1 : 0;
            dr["v1c3"] = data.Rows[0]["v1c3"].ToString() == "OK" ? 1 : 0;
            dr["v1c4"] = data.Rows[0]["v1c4"].ToString() == "OK" ? 1 : 0;
            dr["v1c5"] = data.Rows[0]["v1c5"].ToString() == "OK" ? 1 : 0;
            dr["v1c6"] = data.Rows[0]["v1c6"].ToString() == "OK" ? 1 : 0;
            dr["v1c7"] = data.Rows[0]["v1c7"].ToString() == "OK" ? 1 : 0;
            dr["v1c8"] = data.Rows[0]["v1c8"].ToString() == "OK" ? 1 : 0;
            dr["v1c9"] = data.Rows[0]["v1c9"].ToString() == "OK" ? 1 : 0;
            dr["v1c10"] = data.Rows[0]["v1c10"].ToString() == "OK" ? 1 : 0;
            dr["v2c1"] = data.Rows[0]["v2c1"].ToString() == "OK" ? 1 : 0;
            dr["v2c2"] = data.Rows[0]["v2c2"].ToString() == "OK" ? 1 : 0;
            dr["v2c3"] = data.Rows[0]["v2c3"].ToString() == "OK" ? 1 : 0;
            dr["v2c4"] = data.Rows[0]["v2c4"].ToString() == "OK" ? 1 : 0;
            dr["v2c5"] = data.Rows[0]["v2c5"].ToString() == "OK" ? 1 : 0;
            dr["v2c6"] = data.Rows[0]["v2c6"].ToString() == "OK" ? 1 : 0;
            dr["v2c7"] = data.Rows[0]["v2c7"].ToString() == "OK" ? 1 : 0;
            dr["v2c8"] = data.Rows[0]["v2c8"].ToString() == "OK" ? 1 : 0;
            dr["v2c9"] = data.Rows[0]["v2c9"].ToString() == "OK" ? 1 : 0;
            dr["v2c10"] = data.Rows[0]["v2c10"].ToString() == "OK" ? 1 : 0;

            dt.Rows.Add(dr);
            ds.Tables.Add(dt);
            string dsXml = ds.GetXml();
            string storedProcedureName = "PERODUA_INS_RecordLog";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ip_xml", dsXml);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateDetails()
        {
            modelBox.Controls.Clear();
            modelBox.SizeMode = PictureBoxSizeMode.StretchImage;
            string imageName = mapModelImage(Model, "Logo");
            string imagePath = Path.Combine(Application.StartupPath, "Resources", imageName);
            modelBox.Image = Image.FromFile(imagePath);
            //modelBox.Image = Image.FromFile(@"C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Resources\" + mapModelImage(Model, "Logo"));
            modelBox.Dock = DockStyle.Right;
            modelBox.Width = 200;

            DetailsPanel.Controls.Clear();
            DetailsPanel.Dock = DockStyle.Left;
            DetailsPanel.Padding = new Padding(10, 0, 0, 5);
            DetailsPanel.Width = 280;
            DetailsPanel.RowCount = 2;
            DetailsPanel.ColumnCount = 2;
            DetailsPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            for (int i = 0; i < 2; i++)
            {
                DetailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                DetailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            }
            DateTime currentDateTime = DateTime.Now;
            string formattedDate = currentDateTime.ToString("dd MMM yyyy").ToUpper();
            string formattedTime = currentDateTime.ToString("HH:mm tt");

            ModelID.Controls.Clear();
            ModelID.Text = "MODEL: " + MODEL_NAME[Model];
            VehicleID.Controls.Clear();
            VehicleID.Text = "VEHICLE ID: " + CarID;
            Date.Controls.Clear();
            Date.Text = "DATE: " + formattedDate;
            Time.Controls.Clear();
            Time.Text = "TIME: " + formattedTime;

            DetailsPanel.Controls.Add(ModelID, 0, 0);
            DetailsPanel.Controls.Add(Date, 1, 0);
            DetailsPanel.Controls.Add(VehicleID, 0, 1);
            DetailsPanel.Controls.Add(Time, 1, 1);
        }

        //private Label[] labels;
        //private Label[] labels2;

        private void InitializeLabels()
        {
            // Initialize the labels array with the appropriate number of labels
            //labels = new Label[10];
            //labels2 = new Label[10];

            // Create and configure labels
            //for (int i = 0; i < 10; i++)
            //{
            //    labels[i] = new Label();
            //    labels[i].Dock = DockStyle.Fill;
            //    labels[i].TextAlign = ContentAlignment.MiddleCenter;
            //    labels[i].Font = new Font("Arial", 13f, FontStyle.Regular);

            //    labels2[i] = new Label();
            //    labels2[i].Dock = DockStyle.Fill;
            //    labels2[i].TextAlign = ContentAlignment.MiddleCenter;
            //    labels2[i].Font = new Font("Arial", 14f, FontStyle.Bold);

            //    labels[i].Text = "v1c" + (i + 1);
            //    tableLayoutPanel.Controls.Add(labels[i], i, 1);
            //}

            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.ColumnCount = 10;
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.AutoSize = true;
            //for (int i = 0; i < 10; i++)
            //{
            //    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); // Adjust the column width as needed
            //}
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            V1C1.Text = "V1C1";
            tableLayoutPanel.Controls.Add(V1C1, 0, 1);
            V1C2.Text = "V1C2";
            tableLayoutPanel.Controls.Add(V1C2, 1, 1);
            V1C3.Text = "V1C3";
            tableLayoutPanel.Controls.Add(V1C3, 2, 1);
            V1C4.Text = "V1C4";
            tableLayoutPanel.Controls.Add(V1C4, 3, 1);
            V1C5.Text = "V1C5";
            tableLayoutPanel.Controls.Add(V1C5, 4, 1);
            V1C6.Text = "V1C6";
            tableLayoutPanel.Controls.Add(V1C6, 5, 1);
            V1C7.Text = "V1C7";
            tableLayoutPanel.Controls.Add(V1C7, 6, 1);
            V1C8.Text = "V1C8";
            tableLayoutPanel.Controls.Add(V1C8, 7, 1);
            V1C9.Text = "V1C9";
            tableLayoutPanel.Controls.Add(V1C9, 8, 1);
            V1C10.Text = "V1C10";
            tableLayoutPanel.Controls.Add(V1C10, 9, 1);

            UpdateCheckpointPanel();
        }

        private void UpdateCheckpointPanel()
        {
            //for (int col = 0; col < 10; col++)
            //{
            //    Control controlToRemove = tableLayoutPanel.GetControlFromPosition(col, 0);
            //    if (controlToRemove != null)
            //    {
            //        tableLayoutPanel.Controls.Remove(controlToRemove);
            //        controlToRemove.Dispose(); // Optional: Dispose the control to release resources
            //    }
            //    Label newLabel = new Label();
            //    newLabel.Dock = DockStyle.Fill;
            //    newLabel.TextAlign = ContentAlignment.MiddleCenter;
            //    newLabel.Font = new Font("Arial", 14f, FontStyle.Bold);

            //    newLabel.Text = dataTable.Rows[0][labels[col].Text].ToString() == "True" ? "OK" : "NG";
            //    newLabel.BackColor = dataTable.Rows[0][labels[col].Text].ToString() == "True" ? Color.Green : Color.Red;
            //    tableLayoutPanel.Controls.Add(newLabel, col, 0);
            //}
            V1C1Value.SetText(dataTable.Rows[0]["v1c1"].ToString());
            V1C1Value.SetBackColor(dataTable.Rows[0]["v1c1"].ToString());
            tableLayoutPanel.Controls.Add(V1C1Value, 0, 0);

            V1C2Value.SetText(dataTable.Rows[0]["v1c2"].ToString());
            V1C2Value.SetBackColor(dataTable.Rows[0]["v1c2"].ToString());
            tableLayoutPanel.Controls.Add(V1C2Value, 1, 0);

            V1C3Value.SetText(dataTable.Rows[0]["v1c3"].ToString());
            V1C3Value.SetBackColor(dataTable.Rows[0]["v1c3"].ToString());
            tableLayoutPanel.Controls.Add(V1C3Value, 2, 0);

            V1C4Value.SetText(dataTable.Rows[0]["v1c4"].ToString());
            V1C4Value.SetBackColor(dataTable.Rows[0]["v1c4"].ToString());
            tableLayoutPanel.Controls.Add(V1C4Value, 3, 0);

            V1C5Value.SetText(dataTable.Rows[0]["v1c5"].ToString());
            V1C5Value.SetBackColor(dataTable.Rows[0]["v1c5"].ToString());
            tableLayoutPanel.Controls.Add(V1C5Value, 4, 0);

            V1C6Value.SetText(dataTable.Rows[0]["v1c6"].ToString());
            V1C6Value.SetBackColor(dataTable.Rows[0]["v1c6"].ToString());
            tableLayoutPanel.Controls.Add(V1C6Value, 5, 0);

            V1C7Value.SetText(dataTable.Rows[0]["v1c7"].ToString());
            V1C7Value.SetBackColor(dataTable.Rows[0]["v1c7"].ToString());
            tableLayoutPanel.Controls.Add(V1C7Value, 6, 0);

            V1C8Value.SetText(dataTable.Rows[0]["v1c8"].ToString());
            V1C8Value.SetBackColor(dataTable.Rows[0]["v1c8"].ToString());
            tableLayoutPanel.Controls.Add(V1C8Value, 7, 0);

            V1C9Value.SetText(dataTable.Rows[0]["v1c9"].ToString());
            V1C9Value.SetBackColor(dataTable.Rows[0]["v1c9"].ToString());
            tableLayoutPanel.Controls.Add(V1C9Value, 8, 0);

            V1C10Value.SetText(dataTable.Rows[0]["v1c10"].ToString());
            V1C10Value.SetBackColor(dataTable.Rows[0]["v1c10"].ToString());
            tableLayoutPanel.Controls.Add(V1C10Value, 9, 0);

            ArrowPanel_Resize2();
        }

        private void InitializePart()
        {
            GetData();
            UpdateDetails();

            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 75;

            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 80;
            //bottomPanel.BackColor = Color.Green;

            PictureBox iconBox = new PictureBox();
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            string imagePath = Path.Combine(Application.StartupPath, "Resources", "Perodua_Logo.jpg");

            iconBox.Image = Image.FromFile(imagePath);
            iconBox.Width = 90;
            iconBox.Height = 60;
            iconBox.Dock = DockStyle.Left;

            Label titleLabel = new Label();
            titleLabel.Text = "MB#12 Underbody Inspection System";
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font("Arial", 16f, FontStyle.Bold);

            Label sideLabel = new Label();
            sideLabel.Text = SIDE.Left;
            sideLabel.TextAlign = ContentAlignment.MiddleCenter;
            sideLabel.Dock = DockStyle.Right;
            sideLabel.BackColor = Color.Red;
            sideLabel.ForeColor = Color.White;
            sideLabel.Font = new Font("Arial", 16f, FontStyle.Bold);

            topPanel.Controls.Add(iconBox);
            topPanel.Controls.Add(titleLabel);
            topPanel.Controls.Add(sideLabel);

            bottomPanel.Controls.Add(DetailsPanel);
            bottomPanel.Controls.Add(modelBox);

            this.Controls.Add(topPanel);
            InitializeImage();
            InitializeArrowPanel();
            InitializeCheckPointPanel();
            this.Controls.Add(bottomPanel);
        }

        private void InitializeCheckPointPanel()
        {
            Panel checkpointPanel = new Panel();
            checkpointPanel.Dock = DockStyle.Bottom;
            checkpointPanel.Height = 80;
            //checkpointPanel.BackColor = Color.Black;
            checkpointPanel.Padding = new Padding((int)(((double)checkpointPanel.Width) * 1.5), 0, (int)(((double)checkpointPanel.Width) * 1.5), 0);

            InitializeLabels();
            checkpointPanel.Controls.Add(tableLayoutPanel);
            this.Controls.Add(checkpointPanel);
        }

        Panel ImagePanel = new Panel();
        private void InitializeImage()
        {
            ImagePanel.Dock = DockStyle.Fill;
            //ImagePanel.BackColor = Color.Green;
            //ImagePanel.AutoSize = true;
            UpdateImage();
            ImagePanel.Controls.Add(ImageBox);
            this.Controls.Add(ImagePanel);
        }

        private void UpdateImage()
        {
            ImageBox.Controls.Clear();
            ImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            ImageBox.Dock = DockStyle.Fill;
            //ImageBox.BackColor = Color.Red;
            ImageBox.Padding = new Padding(0);
            ImageBox.Margin = new Padding(0);
            string imageName = mapModelImage(Model, "Image");
            string imagePath = Path.Combine(Application.StartupPath, "Resources", imageName);
            ImageBox.Image = Image.FromFile(imagePath);
        }

        FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
        Panel ArrowPanel = new Panel();
        private void InitializeArrowPanel()
        {
            ArrowPanel.Dock = DockStyle.Bottom;
            //ArrowPanel.BackColor = Color.Gray;
            ArrowPanel.Height = 50;
            this.Controls.Add(ArrowPanel);
        }

        private void ArrowPanel_Resize(object sender, EventArgs e)
        {
            //int width = flowLayoutPanel.Width / 10;
            flowLayoutPanel.Controls.Clear();
            //flowLayoutPanel.BackColor = Color.White;
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.Margin = new Padding(0);
            flowLayoutPanel.Padding = new Padding(0);

            if (Model == "1")
            {
                SetPaddingBasedOnModel(Model, 0.33, 0.34, 0.16, 0.16);
            }
            else if (Model == "2")
            {
                SetPaddingBasedOnModel(Model, 0.30, 0.405, 0.16, 0.16);
            }
            else if (Model == "3")
            {
                SetPaddingBasedOnModel(Model, 0.29, 0.385, 0.16, 0.16);
            }
            else if (Model == "4")
            {

            }

            //ArrowPanel.Padding = new Padding((int)(((double)ArrowPanel.Width) * 2.1), 0, (int)(((double)ArrowPanel.Width) * 2.2), 0);
            int labelwidth = flowLayoutPanel.Width / 10;
            //for (int i = 1; i <= 10; i++)
            //{
            //    Label label = new Label();
            //    label.Margin = new Padding(0);
            //    label.Padding = new Padding(0);
            //    label.Font = new Font("Arial", 10);
            //    label.Text = i.ToString();
            //    label.AutoSize = false; // Disable auto size to set custom width
            //    label.Width = labelwidth; // Set the width of the label
            //    label.TextAlign = ContentAlignment.MiddleCenter; // Center-align text
            //    flowLayoutPanel.Controls.Add(label);
            //}

            ArrowPanel.Controls.Add(flowLayoutPanel);
            ImageBox.Invalidate();
            //Arrow_Draw(sender, e);
            //this.Paint += Arrow_Draw;
        }

        private void ArrowPanel_Resize2()
        {
            //int width = flowLayoutPanel.Width / 10;
            flowLayoutPanel.Controls.Clear();
            //flowLayoutPanel.BackColor = Color.White;
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel.Margin = new Padding(0);
            flowLayoutPanel.Padding = new Padding(0);

            if (Model == "1")
            {
                SetPaddingBasedOnModel(Model, 0.33, 0.34, 0.16, 0.16);
            }
            else if (Model == "2")
            {
                SetPaddingBasedOnModel(Model, 0.30, 0.405, 0.16, 0.16);
            }
            else if (Model == "3")
            {
                SetPaddingBasedOnModel(Model, 0.29, 0.385, 0.16, 0.16);
            }
            else if (Model == "4")
            {

            }

            //ArrowPanel.Padding = new Padding((int)(((double)ArrowPanel.Width) * 2.1), 0, (int)(((double)ArrowPanel.Width) * 2.2), 0);
            int labelwidth = flowLayoutPanel.Width / 10;
            //for (int i = 1; i <= 10; i++)
            //{
            //    Label label = new Label();
            //    label.Margin = new Padding(0);
            //    label.Padding = new Padding(0);
            //    label.Font = new Font("Arial", 10);
            //    label.Text = i.ToString();
            //    label.AutoSize = false; // Disable auto size to set custom width
            //    label.Width = labelwidth; // Set the width of the label
            //    label.TextAlign = ContentAlignment.MiddleCenter; // Center-align text
            //    flowLayoutPanel.Controls.Add(label);
            //}

            ArrowPanel.Controls.Add(flowLayoutPanel);
            ImageBox.Invalidate();
            //Arrow_Draw(sender, e);
            //this.Paint += Arrow_Draw;
        }

        private void SetPaddingBasedOnModel(string model, double leftPaddingPercentage, double rightPaddingPercentage, double imageBoxLeftPaddingPercentage, double imageBoxRightPaddingPercentage)
        {
            int leftPadding = (int)(ArrowPanel.Width * leftPaddingPercentage);
            int rightPadding = (int)(ArrowPanel.Width * rightPaddingPercentage);
            int imageBoxLeftPadding = (int)(ArrowPanel.Width * imageBoxLeftPaddingPercentage);
            int imageBoxRightPadding = (int)(ArrowPanel.Width * imageBoxRightPaddingPercentage);

            ArrowPanel.Padding = new Padding(leftPadding, 0, rightPadding, 0);
            ImagePanel.Padding = new Padding(imageBoxLeftPadding, 30, imageBoxRightPadding, 0);
        }

        public Point centerFlowPoint;
        public Point centerTablePoint;
        private void Arrow_Draw(object sender, PaintEventArgs e)
        {

            int cellWidth = tableLayoutPanel.Width / tableLayoutPanel.ColumnCount;
            int cellHeight = tableLayoutPanel.Height / tableLayoutPanel.RowCount;
            int borderX = tableLayoutPanel.Location.X + cellWidth / 2;
            int borderY = tableLayoutPanel.Location.Y + cellHeight;
            centerTablePoint = new Point(borderX, borderY);

            if (flowLayoutPanel.Controls.Count > 0)
            {
                Control firstControl = flowLayoutPanel.Controls[0];
                int centerX = firstControl.Location.X + (firstControl.Width / 2);
                int centerY = firstControl.Location.Y + (firstControl.Height / 2);
                centerFlowPoint = new Point(centerX, centerY);
            }

            e.Graphics.DrawLine(Pens.Black, centerTablePoint, centerFlowPoint);
        }

    }
    public class LabeledValue : Label
    {
        public LabeledValue()
        {
            // Set default properties
            this.Dock = DockStyle.Fill;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Font = new Font("Arial", 14f, FontStyle.Bold);
            this.Padding = new Padding(3);
            this.Margin = new Padding(0);
        }

        public void SetBackColor(string value)
        {
            this.BackColor = value == "True" || value == "OK" ? COLOR.green : COLOR.red;
        }

        public void SetText(string value)
        {
            this.Text = value == "True" || value == "OK" ? "OK" : "NG";
        }
    }

    public class BlueLabeledValue : Label
    {
        public BlueLabeledValue()
        {
            // Set default properties
            this.Dock = DockStyle.Fill;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Font = new Font("Arial", 13f, FontStyle.Regular);
            this.BackColor = COLOR.blue;
            this.Padding = new Padding(3);
            this.Margin = new Padding(0);
        }
    }

    public class ModelLabeledValue : Label
    {
        public ModelLabeledValue()
        {
            // Set default properties
            this.Dock = DockStyle.Fill;
            this.TextAlign = ContentAlignment.MiddleLeft;
        }
    }

}
