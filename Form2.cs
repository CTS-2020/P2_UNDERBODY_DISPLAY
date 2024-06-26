﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private readonly string _connectString;
        public string connectionString;
        public readonly PaddingValues _d19hPadding;
        public readonly PaddingValues _d20nPadding;
        public readonly PaddingValues _d27aPadding;
        public readonly PaddingValues _d66bPadding;
        private Screen currentScreen;
        public readonly int _RhScreen;
        private bool isMessageBoxShown;
        public readonly int _CheckScreenTimer;
        public Form2(PassIn passin)
        {
            _connectString = passin.ConnectionString;
            connectionString = _connectString;
            _d19hPadding = passin.D19H;
            _d20nPadding = passin.D20N;
            _d27aPadding = passin.D27A;
            _d66bPadding = passin.D66B;

            _CheckScreenTimer = passin.CheckScreenTimer;
            _RhScreen = passin.RH_Screen;
            Screen[] screens = Screen.AllScreens;
            currentScreen = screens[_RhScreen];
            isMessageBoxShown = false;
            // Start a timer to periodically check for screen changes
            Timer screenCheckTimer = new Timer();
            screenCheckTimer.Interval = _CheckScreenTimer; // Check every 1 second (adjust as needed)
            screenCheckTimer.Tick += ScreenCheckTimer_Tick;
            screenCheckTimer.Start();

            InitializeComponent();
            SetFormSizeToScreenResolution();
            InitializePart();
            InitializeTimer(passin.Timer);
            this.Resize += ArrowPanel_Resize;
        }

        private void ScreenCheckTimer_Tick(object sender, EventArgs e)
        {
            // Check if the current screen has changed
            Screen newScreen = Screen.FromControl(this);
            if (!newScreen.Equals(currentScreen))
            {
                // Handle screen change only if the message box is not already shown
                if (!isMessageBoxShown)
                {
                    // Show message box and update the flag
                    //MessageBox.Show("Form's screen has changed!");
                    isMessageBoxShown = true;
                    Application.Restart();
                }
                // Update the current screen
                //currentScreen = newScreen;
            }
            else
            {
                // Reset the flag when the form returns to the original screen
                isMessageBoxShown = false;
            }
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

        private void Form2_Load(object sender, EventArgs e)
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
            public static readonly string D20N = "1_R.png";
            public static readonly string D19H = "2_R.png";
            public static readonly string D27A = "3_R.png";
            public static readonly string D66B = "4_R.png";
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
                    command.Parameters.AddWithValue("@ip_formInd", SIDE.Right);
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
            topPanel.Height = TopPanelHeight;

            Panel bottomPanel = new Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = BottomPanelHeight;
            //bottomPanel.BackColor = Color.Green;

            PictureBox iconBox = new PictureBox();
            iconBox.SizeMode = PictureBoxSizeMode.StretchImage;
            //iconBox.Image = Image.FromFile(@"C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\Resources\Perodua_Logo.jpg");
            string imagePath = Path.Combine(Application.StartupPath, "Resources", "Perodua_Logo.jpg");

            iconBox.Image = Image.FromFile(imagePath);
            iconBox.Width = 150;
            //iconBox.Height = 60;
            iconBox.Dock = DockStyle.Left;

            Label titleLabel = new Label();
            titleLabel.Text = "MB#12 Underbody Inspection System";
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font(this.Font.FontFamily, 23, FontStyle.Bold);

            Label sideLabel = new Label();
            sideLabel.Text = SIDE.Right;
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
            ImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            ImageBox.Dock = DockStyle.Fill;
            ImageBox.Height = ImagePanelHeight;
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
                leftPaddingPercentage = _d20nPadding.RightImageLeftPadding;
                rightPaddingPercentage = _d20nPadding.RightImageRightPadding;
            }
            else if (model == "2")
            {
                leftPaddingPercentage = _d19hPadding.RightImageLeftPadding;
                rightPaddingPercentage = _d19hPadding.RightImageRightPadding;
            }
            else if (model == "3")
            {
                leftPaddingPercentage = _d27aPadding.RightImageLeftPadding;
                rightPaddingPercentage = _d27aPadding.RightImageRightPadding;
            }
            else if (model == "4")
            {
                leftPaddingPercentage = _d66bPadding.RightImageLeftPadding;
                rightPaddingPercentage = _d66bPadding.RightImageRightPadding;
            }
            int leftPadding = (int)(checkpointPanel.Width * leftPaddingPercentage);
            int rightPadding = (int)(checkpointPanel.Width * rightPaddingPercentage);
            int imageBoxLeftPadding = (int)(checkpointPanel.Width * IMAGE_PADDINGRATIO.FixRatio);
            int imageBoxRightPadding = (int)(checkpointPanel.Width * IMAGE_PADDINGRATIO.FixRatio);

            checkpointPanel.Padding = new Padding(leftPadding, 0, rightPadding, 0);
            ImagePanel.Padding = new Padding(imageBoxLeftPadding, 30, imageBoxRightPadding, 0);
        }
    }
}
