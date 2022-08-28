using DeepAI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NYZXUpscaler
{
    public partial class UI : Form
    {
        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private bool Bool;

        private Point point;

        public static string url = "";

        public UI()
        {
            InitializeComponent();
            base.Region = Region.FromHrgn(UI.CreateRoundRectRgn(0, 0, base.Width, base.Height, 20, 20));
        }

        private void sButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(label1.Text))
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK) { label1.Text = openFileDialog1.FileName; } else { }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to improve this image?", "Nyzx", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string imagepath = label1.Text.ToString();
                    DeepAI_API api = new DeepAI_API(apiKey: "f324a1e7-c056-4613-9e27-9cfb0ccad096");
                    StandardApiResponse resp = api.callStandardApi("waifu2x", new
                    {
                        image = File.OpenRead(imagepath),
                    });
                    JObject jobject = (JObject)JsonConvert.DeserializeObject((api.objectAsJsonString(resp)));
                    JToken jtoken = jobject["output_url"];
                    bool flag = string.IsNullOrEmpty((string)jtoken);
                    if (flag)
                    {
                        jtoken = 0;
                    }
                    url = string.Format("{0:n0}", jtoken);
                    Process.Start(url);
                    Clipboard.SetText(url);
                    pictureBox1.LoadAsync(url);
                    label1.Text = "";
                }
                else if (dialogResult == DialogResult.No)
                {
                    label1.Text = "";
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Bool)
            {
                base.Location = new Point(base.Location.X - this.point.X + e.X, base.Location.Y - this.point.Y + e.Y);
                base.Update();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Bool = false;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Bool = true;
            this.point = e.Location;
        }
    }
}
