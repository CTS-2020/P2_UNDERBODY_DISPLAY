
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Windows.Forms;

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
                    Form1 form1 = new Form1(connectionString);
                    // Position Form1 on the primary screen
                    form1.WindowState = FormWindowState.Maximized;
                    form1.StartPosition = FormStartPosition.CenterScreen;
                    form1.Location = screens[0].WorkingArea.Location;
                    form1.FormClosed += (sender, e) => Application.Exit();
                    Application.Run(form1);
                });
                form1Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                form1Thread.Start();

                // Create and start a new thread for Form2
                Thread form2Thread = new Thread(() =>
                {
                    Form2 form2 = new Form2(connectionString);
                    // Position Form2 on the second screen
                    form2.WindowState = FormWindowState.Maximized;
                    form2.StartPosition = FormStartPosition.Manual;
                    form2.Location = screens[1].WorkingArea.Location;
                    form2.FormClosed += (sender, e) => Application.Exit();
                    Application.Run(form2);
                });
                form2Thread.SetApartmentState(ApartmentState.STA); // Set the apartment state to STA (required for Windows Forms)
                form2Thread.Start();

                form1Thread.Join();
                form2Thread.Join();
            }
            else
            {
                // If there's only one screen available, show both forms on the primary screen
                Application.Run(new Form1(connectionString));
                Application.Run(new Form2(connectionString));
            }
        }
    }
}
