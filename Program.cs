
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.IO;
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
        private static readonly string CrashLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crashlog.txt");

        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                LogException("UI Thread Exception", e.Exception);
                MessageBox.Show("An unexpected error occurred. Check crashlog.txt for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var exception = e.ExceptionObject as Exception;
                LogException("Non-UI Thread Exception", exception);
                MessageBox.Show("An unexpected error occurred. Check crashlog.txt for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            };

            try
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                //the appsetting to change is in bin folder not in here.
                //C:\vsFolder\20240312_PERODUA_UnderbodyIPC_Display\WindowsFormsApp1\bin\Debug
                string connectionString = configuration.GetConnectionString("PeroduaDatabase");
                string LogDirectory = configuration.GetSection("LogDirectory").Value;
                string LogFilePrefix = configuration.GetSection("LogFilePrefix").Value;
                int Timer = int.Parse(configuration.GetSection("Timer").Value);
                int CheckScreenTimer = int.Parse(configuration.GetSection("CheckScreenTimer").Value);
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
                passin.LH_Screen = LH_Screen;
                passin.RH_Screen = RH_Screen;
                passin.Log_Screen = Log_Screen;
                passin.CheckScreenTimer = CheckScreenTimer;

                //ConnectDB connectDB = new ConnectDB(connectionString);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                // Create and start a new thread for Form1
                Screen[] screens = Screen.AllScreens;

                if (screens.Length >= 2)
                {
                    if (!IsValidScreenIndex(LH_Screen, screens))
                    {
                        throw new InvalidOperationException($"Invalid screen index {LH_Screen} for LH_Screen. Available screens: {screens.Length}.");
                    }
                    else if (!IsValidScreenIndex(RH_Screen, screens))
                    {
                        throw new InvalidOperationException($"Invalid screen index {RH_Screen} for RH_Screen. Available screens: {screens.Length}.");
                    }
                    else if (!IsValidScreenIndex(Log_Screen, screens))
                    {
                        throw new InvalidOperationException($"Invalid screen index {Log_Screen} for Log_Screen. Available screens: {screens.Length}.");
                    }

                    // Create and start a new thread for Form1
                    Thread form1Thread = new Thread(() =>
                    {
                        try
                        {
                            Form1 form1 = new Form1(passin);
                            // Position Form1 on the primary screen
                            form1.WindowState = FormWindowState.Maximized;
                            form1.StartPosition = FormStartPosition.CenterScreen;
                            form1.Location = screens[LH_Screen].WorkingArea.Location;
                            form1.FormClosed += (sender, e) => Application.Exit();
                            Application.Run(form1);
                        }
                        catch (Exception ex)
                        {
                            LogException("Form1 Thread Exception", ex);
                            MessageBox.Show("An error occurred in Form1. Check crashlog.txt for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    });
                    form1Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                    form1Thread.Start();

                    // Create and start a new thread for Form2
                    Thread form2Thread = new Thread(() =>
                    {
                        try
                        {
                            Form2 form2 = new Form2(passin);
                            // Position Form2 on the second screen
                            form2.WindowState = FormWindowState.Maximized;
                            form2.StartPosition = FormStartPosition.CenterScreen;
                            form2.Location = screens[RH_Screen].WorkingArea.Location;
                            form2.FormClosed += (sender, e) => Application.Exit();
                            Application.Run(form2);
                        }
                        catch (Exception ex)
                        {
                            LogException("Form2 Thread Exception", ex);
                            MessageBox.Show("An error occurred in Form2. Check crashlog.txt for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }

                    });
                    form2Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                    form2Thread.Start();

                    Thread form3Thread = new Thread(() =>
                    {
                        try
                        {
                            Form3 form3 = new Form3(passin);
                            // Position Form2 on the second screen
                            form3.WindowState = FormWindowState.Maximized;
                            form3.StartPosition = FormStartPosition.CenterScreen;
                            form3.Location = screens[Log_Screen].WorkingArea.Location;
                            form3.FormClosed += (sender, e) => Application.Exit();
                            Application.Run(form3);
                        }
                        catch (Exception ex)
                        {
                            LogException("Form3 Thread Exception", ex);
                            MessageBox.Show("An error occurred in Form3. Check crashlog.txt for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }

                    });
                    form3Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                    form3Thread.Start();

                    form1Thread.Join();
                    form2Thread.Join();
                    form3Thread.Join();
                }
                else
                {
                    // If there's only one screen available, show forms on the primary screen
                    try
                    {
                        Application.Run(new Form1(passin));
                        Application.Run(new Form2(passin));
                        Application.Run(new Form3(passin));
                    }
                    catch (Exception ex)
                    {
                        LogException("Single Screen Forms Exception", ex);
                        MessageBox.Show("An error occurred while running forms. Check crashlog.txt for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("Main Method Exception", ex);
                MessageBox.Show("A critical error occurred during application startup. Check crashlog.txt for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        static bool IsValidScreenIndex(int screenPosition, Screen[] screens)
        {
            int index = (int)screenPosition;
            return index >= 0 && index < screens.Length;
        }
        public static PaddingValues GetPaddingValues(IConfiguration configuration, string imageType)
        {
            try
            {
                return new PaddingValues
                {
                    LeftImageLeftPadding = GetAndParsePaddingValue(configuration, "LeftImage", imageType, "LeftPadding"),
                    LeftImageRightPadding = GetAndParsePaddingValue(configuration, "LeftImage", imageType, "RightPadding"),
                    RightImageLeftPadding = GetAndParsePaddingValue(configuration, "RightImage", imageType, "LeftPadding"),
                    RightImageRightPadding = GetAndParsePaddingValue(configuration, "RightImage", imageType, "RightPadding")
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load padding values for image type {imageType}.", ex);
            }

        }

        public static double GetAndParsePaddingValue(IConfiguration configuration, string section, string imageType, string paddingType)
        {
            string value = configuration.GetSection($"Padding:{section}:{imageType}:{paddingType}").Value;
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Configuration value for padding (Padding:{section}:{imageType}:{paddingType}) is missing or empty.");
            }
            if (!double.TryParse(value, out double result))
            {
                throw new InvalidOperationException($"Configuration value for padding (Padding:{section}:{imageType}:{paddingType}) is not a valid double: {value}.");
            }
            return double.Parse(value);
        }

        private static void LogException(string context, Exception ex)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{context}]:\n{ex?.ToString()}\n\n";
                File.AppendAllText(CrashLogPath, logMessage);
            }
            catch
            {
                // Silent fail to avoid crashing due to logging issues
            }
        }
    }

    public class PassIn
    {
        public string ConnectionString { get; set; }
        public string LogDirectory { get; set; }
        public string LogFilePrefix { get; set; }
        public int Timer { get; set; }
        public int PageSize { get; set; }
        public int LH_Screen { get; set; }
        public int RH_Screen { get; set; }
        public int Log_Screen { get; set; }
        public int CheckScreenTimer { get; set; }
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
