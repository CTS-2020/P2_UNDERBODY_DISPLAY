
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly string _connectString;
        public string connectionString;
        public readonly string _logDirectory;
        public readonly string _logFilePrefix;
        public readonly PaddingValues _d19hPadding;
        public readonly PaddingValues _d20nPadding;
        public readonly PaddingValues _d27aPadding;
        public readonly PaddingValues _d66bPadding;
        public Form1(PassIn passin)
        {
            _connectString = passin.ConnectionString;
            connectionString = _connectString;
            _logDirectory = passin.LogDirectory;
            _logFilePrefix = passin.LogFilePrefix;
            _d19hPadding = passin.D19H;
            _d20nPadding = passin.D20N;
            _d27aPadding = passin.D27A;
            _d66bPadding = passin.D66B;
            InitializeComponent();
            SetFormSizeToScreenResolution();
            InitializePart();
            InitializeTimer(passin.Timer);
            this.Resize += ArrowPanel_Resize;

        }

        private Timer dataUpdateTimer;
        private void InitializeTimer(int timerDuration)
        {
            dataUpdateTimer = new Timer();
            dataUpdateTimer.Interval = timerDuration;
            dataUpdateTimer.Tick += RefreshTimer_Tick;
            dataUpdateTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            GetRefreshInd();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ArrowPanel_Resize(sender, e);
        }

        public int TopPanelHeight;
        public int BottomPanelHeight;
        public int CheckpointPanelHeight;
        public int ImagePanelHeight;
        private void SetFormSizeToScreenResolution()
        {

            this.Width = 1920;
            this.Height = 1080;

            TopPanelHeight = (int)(this.Height * HEIGHT_RATIO.Ratio_10);
            BottomPanelHeight = (int)(this.Height * HEIGHT_RATIO.Ratio_15);
            CheckpointPanelHeight = (int)(this.Height * HEIGHT_RATIO.Ratio_15);
            ImagePanelHeight = (int)(this.Height * HEIGHT_RATIO.Ratio_50);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public static class MODEL_IMAGE
        {
            public static readonly string D20N = "1_L.png";
            public static readonly string D19H = "2_L.png";
            public static readonly string D27A = "3_L.png";
            public static readonly string D66B = "4_L.png";
        }

        public string mapModelImage(string modelNumber, string type)
        {
            Dictionary<string, Dictionary<string, string>> modelTypeMap = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "Image", new Dictionary<string, string>()
                    {
                        { "1", MODEL_IMAGE.D20N },
                        { "2", MODEL_IMAGE.D19H },
                        { "3", MODEL_IMAGE.D27A },
                        { "4", MODEL_IMAGE.D66B }
                    }
                },
                {
                    "Logo", new Dictionary<string, string>()
                    {
                        { "1", MODEL_LOGO.D20N },
                        { "2", MODEL_LOGO.D19H },
                        { "3", MODEL_LOGO.D27A },
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

        public bool LogInd = false;
        private void GetData()
        {
            if (dataTable != null) dataTable.Clear();
            string storedProcedureName = "PERODUA_GET_Details";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ip_formInd", SIDE.Left);
                    connection.Open();
                    dataTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    Model = dataTable.Rows[0]["current_car_model"].ToString();
                    CarID = dataTable.Rows[0]["car_id"].ToString();
                    if (LogInd) InsLog(dataTable);

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

            if (refreshInd == true)
            {
                LogInd = true;
                GetData();
            }
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

            LogInd = false;
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
            modelBox.Width = 400;

            DetailsPanel.Controls.Clear();
            DetailsPanel.Dock = DockStyle.Left;
            DetailsPanel.Padding = new Padding(10, 0, 0, 5);
            DetailsPanel.Width = 400;
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
            ModelID.Text = "MODEL: " + MODEL_CONST.MODEL_NAME[Model];
            VehicleID.Controls.Clear();
            VehicleID.Text = "VEHICLE ID: \n" + CarID;
            Date.Controls.Clear();
            Date.Text = "DATE: \n" + formattedDate;
            Time.Controls.Clear();
            Time.Text = "TIME: " + formattedTime;

            DetailsPanel.Controls.Add(ModelID, 0, 0);
            DetailsPanel.Controls.Add(Date, 1, 0);
            DetailsPanel.Controls.Add(VehicleID, 0, 1);
            DetailsPanel.Controls.Add(Time, 1, 1);
        }
        private void InitializeLabels()
        {
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.ColumnCount = 10;
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
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
            topPanel.Height = TopPanelHeight;

            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = BottomPanelHeight;
            //bottomPanel.BackColor = Color.Green;

            PictureBox iconBox = new PictureBox();
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            string imagePath = Path.Combine(Application.StartupPath, "Resources", "Perodua_Logo.jpg");

            iconBox.Image = Image.FromFile(imagePath);
            iconBox.Width = 150;
            iconBox.Dock = DockStyle.Left;

            Label titleLabel = new Label();
            titleLabel.Text = "MB#12 Underbody Inspection System";
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font(this.Font.FontFamily, 23, FontStyle.Bold);

            Label sideLabel = new Label();
            sideLabel.Text = SIDE.Left;
            sideLabel.TextAlign = ContentAlignment.MiddleCenter;
            sideLabel.Dock = DockStyle.Right;
            sideLabel.BackColor = Color.Red;
            sideLabel.ForeColor = Color.White;
            sideLabel.Font = new Font(this.Font.FontFamily, 18, FontStyle.Bold);
            sideLabel.Width = 150;

            topPanel.Controls.Add(iconBox);
            topPanel.Controls.Add(titleLabel);
            topPanel.Controls.Add(sideLabel);

            bottomPanel.Controls.Add(DetailsPanel);
            bottomPanel.Controls.Add(modelBox);

            this.Controls.Add(topPanel);
            InitializeImage();
            //InitializeArrowPanel();
            InitializeCheckPointPanel();
            this.Controls.Add(bottomPanel);
        }
        Panel checkpointPanel = new Panel();
        private void InitializeCheckPointPanel()
        {
            checkpointPanel.Dock = DockStyle.Bottom;
            checkpointPanel.Height = CheckpointPanelHeight;
            //checkpointPanel.BackColor = Color.Black;
            //checkpointPanel.Padding = new Padding((int)(((double)checkpointPanel.Width) * 1.5), 0, (int)(((double)checkpointPanel.Width) * 1.5), 0);

            InitializeLabels();
            checkpointPanel.Controls.Add(tableLayoutPanel);
            this.Controls.Add(checkpointPanel);
        }

        Panel ImagePanel = new Panel();
        private void InitializeImage()
        {
            ImagePanel.Dock = DockStyle.Bottom;
            ImagePanel.Height = ImagePanelHeight;
            //ImagePanel.BackColor = Color.Green;
            //ImagePanel.AutoSize = true;
            UpdateImage();
            ImagePanel.Controls.Add(ImageBox);
            this.Controls.Add(ImagePanel);
        }

        private void UpdateImage()
        {
            ImageBox.Controls.Clear();
            ImageBox.Height = ImagePanelHeight;
            ImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            ImageBox.Dock = DockStyle.Fill;
            //ImageBox.BackColor = Color.Red;
            ImageBox.Padding = new Padding(0);
            ImageBox.Margin = new Padding(0);
            string imageName = mapModelImage(Model, "Image");
            string imagePath = Path.Combine(Application.StartupPath, "Resources", imageName);
            ImageBox.Image = Image.FromFile(imagePath);
        }

        private void ArrowPanel_Resize(object sender, EventArgs e)
        {
            SetPaddingBasedOnModel(Model);
        }

        private void ArrowPanel_Resize2()
        {
            SetPaddingBasedOnModel(Model);
        }

        private void SetPaddingBasedOnModel(string model)
        {
            double leftPaddingPercentage = 0;
            double rightPaddingPercentage = 0;
            if (model == "1")
            {
                leftPaddingPercentage = _d20nPadding.LeftImageLeftPadding;
                rightPaddingPercentage = _d20nPadding.LeftImageRightPadding;
            }
            else if (model == "2")
            {
                leftPaddingPercentage = _d19hPadding.LeftImageLeftPadding;
                rightPaddingPercentage = _d19hPadding.LeftImageRightPadding;
            }
            else if (model == "3")
            {
                leftPaddingPercentage = _d27aPadding.LeftImageLeftPadding;
                rightPaddingPercentage = _d27aPadding.LeftImageRightPadding;
            }
            else if (model == "4")
            {
                leftPaddingPercentage = _d66bPadding.LeftImageLeftPadding;
                rightPaddingPercentage = _d66bPadding.LeftImageRightPadding;
            }
            int leftPadding = (int)(checkpointPanel.Width * leftPaddingPercentage);
            int rightPadding = (int)(checkpointPanel.Width * rightPaddingPercentage);
            int imageBoxLeftPadding = (int)(checkpointPanel.Width * IMAGE_PADDINGRATIO.FixRatio);
            int imageBoxRightPadding = (int)(checkpointPanel.Width * IMAGE_PADDINGRATIO.FixRatio);

            checkpointPanel.Padding = new Padding(leftPadding, 0, rightPadding, 0);
            ImagePanel.Padding = new Padding(imageBoxLeftPadding, 30, imageBoxRightPadding, 0);
        }
    }
    public class LabeledValue : Label
    {
        public LabeledValue()
        {
            this.Dock = DockStyle.Fill;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Font = new Font(this.Font.FontFamily, 16, FontStyle.Bold);
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
            this.Dock = DockStyle.Fill;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Font = new Font(this.Font.FontFamily, 14, FontStyle.Bold);
            this.BackColor = COLOR.blue;
            this.Padding = new Padding(3);
            this.Margin = new Padding(0);
        }
    }

    public class ModelLabeledValue : Label
    {
        public ModelLabeledValue()
        {
            this.Dock = DockStyle.Fill;
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.Font = new Font(this.Font.FontFamily, 16, FontStyle.Bold);
        }
    }
    public static class SIDE
    {
        public static readonly string Left = "LH";
        public static readonly string Right = "RH";
    }
    public static class HEIGHT_RATIO
    {
        public static readonly double Ratio_10 = 0.1;
        public static readonly double Ratio_15 = 0.15;
        public static readonly double Ratio_50 = 0.5;
    }
    public static class IMAGE_PADDINGRATIO
    {
        public static readonly double FixRatio = 0.16;
    }

    public static class MODEL_LOGO
    {
        public static readonly string D20N = "Myvi_Logo.jpg";
        public static readonly string D19H = "Aruz_Logo.jpg";
        public static readonly string D27A = "Alza_Logo.png";
        public static readonly string D66B = "D66B_Logo.png";
    }

    public static class COLOR
    {
        public static readonly Color green = ColorTranslator.FromHtml("#00B050");
        public static readonly Color blue = ColorTranslator.FromHtml("#00B0F0");
        public static readonly Color red = ColorTranslator.FromHtml("#FF0000");
    }


    public static class MODEL_CONST
    {
        public static readonly Dictionary<string, string> MODEL_NAME = new Dictionary<string, string>()
        {
            { "1", "D20N"},
            { "2", "D19H"},
            { "3", "D27A"},
            { "4", "D66B"}
        };
    }
}
