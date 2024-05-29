using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DialogForm : Form
    {
        private Label statusLabel;
        private Button okButton;
        private Timer timer;
        private int dotCount;

        public DialogForm(string initialMessage)
        {
            this.Text = "Export Status";
            this.Size = new System.Drawing.Size(300, 150);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;

            statusLabel = new Label
            {
                Text = initialMessage,
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            okButton = new Button
            {
                Text = "OK",
                Dock = DockStyle.Bottom,
                Height = 30,
                Enabled = false // Initially disabled, will enable when export is done
            };
            okButton.Click += OkButton_Click;

            timer = new Timer
            {
                Interval = 500 // Update the message every 500 milliseconds
            };
            timer.Tick += Timer_Tick;

            this.Controls.Add(statusLabel);
            this.Controls.Add(okButton);

            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            dotCount = (dotCount + 1) % 4;
            string dots = new string('.', dotCount);
            statusLabel.Text = $"Exporting to Excel{dots}";
        }

        public void UpdateStatus(string newMessage)
        {
            timer.Stop();
            statusLabel.Text = newMessage;
            okButton.Enabled = true; // Enable the button when the status is updated
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
