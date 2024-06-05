
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

            string connectionString = configuration.GetConnectionString("PeroduaDatabase");
            string LogDirectory = configuration.GetSection("LogDirectory").Value;
            string LogFilePrefix = configuration.GetSection("LogFilePrefix").Value;
            int Timer = int.Parse(configuration.GetSection("Timer").Value);
            int PageSize = int.Parse(configuration.GetSection("PageSize").Value);
            int LH_Screen = int.Parse(configuration.GetSection("LH_Screen").Value);
            int RH_Screen = int.Parse(configuration.GetSection("RH_Screen").Value);
            int Log_Screen = int.Parse(configuration.GetSection("Log_Screen").Value);
            int StartupTimer = int.Parse(configuration.GetSection("StartupTimer").Value);
            var d66bPaddingValues = GetPaddingValues(configuration, "D66B");
            var d20nPaddingValues = GetPaddingValues(configuration, "D20N");
            var d19hPaddingValues = GetPaddingValues(configuration, "D19H");
            var d27aPaddingValues = GetPaddingValues(configuration, "D27A");

            Thread.Sleep(StartupTimer);

            PassIn passin = new PassIn();
            passin.ConnectionString = connectionString;
            passin.Timer = Timer;
            passin.PageSize = PageSize;
            passin.LogDirectory = LogDirectory;
            passin.LogFilePrefix = LogFilePrefix;
            passin.D20N = d20nPaddingValues;
            passin.D27A = d27aPaddingValues;
            passin.D19H = d19hPaddingValues;
            passin.D66B = d66bPaddingValues;

            //ConnectDB connectDB = new ConnectDB(connectionString);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Create and start a new thread for Form1
            Screen[] screens = Screen.AllScreens;

            // Check if there are at least two screens available
            if (screens.Length >= 2)
            {


                // Create and start a new thread for Form1
                Thread form1Thread = new Thread(() =>
                {
                    Form1 form1 = new Form1(passin);
                    // Position Form1 on the primary screen
                    form1.WindowState = FormWindowState.Maximized;
                    form1.StartPosition = FormStartPosition.CenterScreen;
                    form1.Location = screens[LH_Screen].WorkingArea.Location;
                    form1.FormClosed += (sender, e) => Application.Exit();
                    Application.Run(form1);
                });
                form1Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                form1Thread.Start();

                // Create and start a new thread for Form2
                Thread form2Thread = new Thread(() =>
                {
                    Form2 form2 = new Form2(passin);
                    // Position Form2 on the second screen
                    form2.WindowState = FormWindowState.Maximized;
                    form2.StartPosition = FormStartPosition.CenterScreen;
                    form2.Location = screens[RH_Screen].WorkingArea.Location;
                    form2.FormClosed += (sender, e) => Application.Exit();
                    Application.Run(form2);
                });
                form2Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                form2Thread.Start();

                Thread form3Thread = new Thread(() =>
                {
                    Form3 form3 = new Form3(passin);
                    // Position Form2 on the second screen
                    form3.WindowState = FormWindowState.Maximized;
                    form3.StartPosition = FormStartPosition.CenterScreen;
                    form3.Location = screens[Log_Screen].WorkingArea.Location;
                    form3.FormClosed += (sender, e) => Application.Exit();
                    Application.Run(form3);
                });
                form3Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                form3Thread.Start();

                form1Thread.Join();
                form2Thread.Join();
                form3Thread.Join();
            }
            else
            {
                // If there's only one screen available, show both forms on the primary screen
                Application.Run(new Form1(passin));
                Application.Run(new Form2(passin));
                Application.Run(new Form3(passin));
            }
        }

        public static PaddingValues GetPaddingValues(IConfiguration configuration, string imageType)
        {
            return new PaddingValues
            {
                LeftImageLeftPadding = GetAndParsePaddingValue(configuration, "LeftImage", imageType, "LeftPadding"),
                LeftImageRightPadding = GetAndParsePaddingValue(configuration, "LeftImage", imageType, "RightPadding"),
                RightImageLeftPadding = GetAndParsePaddingValue(configuration, "RightImage", imageType, "LeftPadding"),
                RightImageRightPadding = GetAndParsePaddingValue(configuration, "RightImage", imageType, "RightPadding")
            };
        }

        public static double GetAndParsePaddingValue(IConfiguration configuration, string section, string imageType, string paddingType)
        {
            string value = configuration.GetSection($"Padding:{section}:{imageType}:{paddingType}").Value;
            return double.Parse(value);
        }
    }

    public class PassIn
    {
        public string ConnectionString { get; set; }
        public string LogDirectory { get; set; }
        public string LogFilePrefix { get; set; }
        public int Timer { get; set; }
        public int PageSize { get; set; }
        public PaddingValues D20N { get; set; }
        public PaddingValues D19H { get; set; }
        public PaddingValues D27A { get; set; }
        public PaddingValues D66B { get; set; }
    }

    public class PaddingValues
    {
        public double LeftImageLeftPadding { get; set; }
        public double LeftImageRightPadding { get; set; }
        public double RightImageLeftPadding { get; set; }
        public double RightImageRightPadding { get; set; }
    }
}
