using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private readonly string _connectString;
        public string connectionString;
        public Form2(string connectString)
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
            ArrowPanel_Resize(sender, e);
            this.Paint += Arrow_Draw;
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
            public static readonly string D20N = "1_R.jpg";
            public static readonly string D19H = "2_R.png";
            public static readonly string D27H = "3_R.png";
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

        BlueLabeledValue V2C1 = new BlueLabeledValue();
        BlueLabeledValue V2C2 = new BlueLabeledValue();
        BlueLabeledValue V2C3 = new BlueLabeledValue();
        BlueLabeledValue V2C4 = new BlueLabeledValue();
        BlueLabeledValue V2C5 = new BlueLabeledValue();
        BlueLabeledValue V2C6 = new BlueLabeledValue();
        BlueLabeledValue V2C7 = new BlueLabeledValue();
        BlueLabeledValue V2C8 = new BlueLabeledValue();
        BlueLabeledValue V2C9 = new BlueLabeledValue();
        BlueLabeledValue V2C10 = new BlueLabeledValue();

        LabeledValue V2C1Value = new LabeledValue();
        LabeledValue V2C2Value = new LabeledValue();
        LabeledValue V2C3Value = new LabeledValue();
        LabeledValue V2C4Value = new LabeledValue();
        LabeledValue V2C5Value = new LabeledValue();
        LabeledValue V2C6Value = new LabeledValue();
        LabeledValue V2C7Value = new LabeledValue();
        LabeledValue V2C8Value = new LabeledValue();
        LabeledValue V2C9Value = new LabeledValue();
        LabeledValue V2C10Value = new LabeledValue();

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
        private void InitializeLabels()
        {
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.ColumnCount = 10;
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.AutoSize = true;
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

            V2C1.Text = "V2C1";
            tableLayoutPanel.Controls.Add(V2C1, 0, 1);
            V2C2.Text = "V2C2";
            tableLayoutPanel.Controls.Add(V2C2, 1, 1);
            V2C3.Text = "V2C3";
            tableLayoutPanel.Controls.Add(V2C3, 2, 1);
            V2C4.Text = "V2C4";
            tableLayoutPanel.Controls.Add(V2C4, 3, 1);
            V2C5.Text = "V2C5";
            tableLayoutPanel.Controls.Add(V2C5, 4, 1);
            V2C6.Text = "V2C6";
            tableLayoutPanel.Controls.Add(V2C6, 5, 1);
            V2C7.Text = "V2C7";
            tableLayoutPanel.Controls.Add(V2C7, 6, 1);
            V2C8.Text = "V2C8";
            tableLayoutPanel.Controls.Add(V2C8, 7, 1);
            V2C9.Text = "V2C9";
            tableLayoutPanel.Controls.Add(V2C9, 8, 1);
            V2C10.Text = "V2C10";
            tableLayoutPanel.Controls.Add(V2C10, 9, 1);

            UpdateCheckpointPanel();
        }

        private void UpdateCheckpointPanel()
        {
            V2C1Value.SetText(dataTable.Rows[0]["v2c1"].ToString());
            V2C1Value.SetBackColor(dataTable.Rows[0]["v2c1"].ToString());
            tableLayoutPanel.Controls.Add(V2C1Value, 0, 0);

            V2C2Value.SetText(dataTable.Rows[0]["v2c2"].ToString());
            V2C2Value.SetBackColor(dataTable.Rows[0]["v2c2"].ToString());
            tableLayoutPanel.Controls.Add(V2C2Value, 1, 0);

            V2C3Value.SetText(dataTable.Rows[0]["v2c3"].ToString());
            V2C3Value.SetBackColor(dataTable.Rows[0]["v2c3"].ToString());
            tableLayoutPanel.Controls.Add(V2C3Value, 2, 0);

            V2C4Value.SetText(dataTable.Rows[0]["v2c4"].ToString());
            V2C4Value.SetBackColor(dataTable.Rows[0]["v2c4"].ToString());
            tableLayoutPanel.Controls.Add(V2C4Value, 3, 0);

            V2C5Value.SetText(dataTable.Rows[0]["v2c5"].ToString());
            V2C5Value.SetBackColor(dataTable.Rows[0]["v2c5"].ToString());
            tableLayoutPanel.Controls.Add(V2C5Value, 4, 0);

            V2C6Value.SetText(dataTable.Rows[0]["v2c6"].ToString());
            V2C6Value.SetBackColor(dataTable.Rows[0]["v2c6"].ToString());
            tableLayoutPanel.Controls.Add(V2C6Value, 5, 0);

            V2C7Value.SetText(dataTable.Rows[0]["v2c7"].ToString());
            V2C7Value.SetBackColor(dataTable.Rows[0]["v2c7"].ToString());
            tableLayoutPanel.Controls.Add(V2C7Value, 6, 0);

            V2C8Value.SetText(dataTable.Rows[0]["v2c8"].ToString());
            V2C8Value.SetBackColor(dataTable.Rows[0]["v2c8"].ToString());
            tableLayoutPanel.Controls.Add(V2C8Value, 7, 0);

            V2C9Value.SetText(dataTable.Rows[0]["v2c9"].ToString());
            V2C9Value.SetBackColor(dataTable.Rows[0]["v2c9"].ToString());
            tableLayoutPanel.Controls.Add(V2C9Value, 8, 0);

            V2C10Value.SetText(dataTable.Rows[0]["v2c10"].ToString());
            V2C10Value.SetBackColor(dataTable.Rows[0]["v2c10"].ToString());
            tableLayoutPanel.Controls.Add(V2C10Value, 9, 0);

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
            //iconBox.Image = Image.FromFile(@"C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Resources\Perodua_Logo.jpg");
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
            sideLabel.Text = SIDE.Right;
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
            ImagePanel.AutoSize = true;
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
            ImageBox.Padding = new Padding(0, 0, 0, 0);
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
                SetPaddingBasedOnModel(Model, 0.33, 0.35, 0.16, 0.16);
            }
            else if (Model == "2")
            {
                SetPaddingBasedOnModel(Model, 0.37, 0.345, 0.16, 0.16);
            }
            else if (Model == "3")
            {
                SetPaddingBasedOnModel(Model, 0.355, 0.325, 0.16, 0.16);
            }
            else if (Model == "4")
            {

            }

            int labelwidth = flowLayoutPanel.Width / 10;
            //for (int i = 1; i <= 10; i++)
            //{
            //    Label label = new Label();
            //    label.Margin = new Padding(0);
            //    label.Padding = new Padding(0);
            //    label.Font = new Font("Arial", 10);
            //    label.Text = (11 - i).ToString();
            //    label.AutoSize = false;
            //    label.Width = labelwidth;
            //    label.TextAlign = ContentAlignment.MiddleCenter;
            //    flowLayoutPanel.Controls.Add(label);
            //}

            ArrowPanel.Controls.Add(flowLayoutPanel);
            ImageBox.Invalidate();
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
                SetPaddingBasedOnModel(Model, 0.33, 0.35, 0.16, 0.16);
            }
            else if (Model == "2")
            {
                SetPaddingBasedOnModel(Model, 0.37, 0.345, 0.16, 0.16);
            }
            else if (Model == "3")
            {
                SetPaddingBasedOnModel(Model, 0.355, 0.325, 0.16, 0.16);
            }
            else if (Model == "4")
            {

            }

            int labelwidth = flowLayoutPanel.Width / 10;
            //for (int i = 1; i <= 10; i++)
            //{
            //    Label label = new Label();
            //    label.Margin = new Padding(0);
            //    label.Padding = new Padding(0);
            //    label.Font = new Font("Arial", 10);
            //    label.Text = (11 - i).ToString();
            //    label.AutoSize = false;
            //    label.Width = labelwidth;
            //    label.TextAlign = ContentAlignment.MiddleCenter;
            //    flowLayoutPanel.Controls.Add(label);
            //}

            ArrowPanel.Controls.Add(flowLayoutPanel);
            ImageBox.Invalidate();
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
}
