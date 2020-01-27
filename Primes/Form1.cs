using System;
using System.Threading;
using System.Windows.Forms;

namespace Primes
{
    public partial class Form1 : Form
    {
        private readonly Preferences preferences = new Preferences();
        private Skia skia;
        public Form1()
        {
            this.InitializeComponent();
            this.Load += this.Form1_Load;
            this.FormClosing += this.Form1_FormClosing;
            this.RestoreLastWindowPosition();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.RestoreLastWindowPosition();
            this.skia = new Skia(this.Width, this.Height);
            Primes.GetPrimesViaEratosthenesSieve();
            Helpers.Print(this, $"Form1_Load");
            this.skia.TryDraw();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.preferences.Model.WindowHeight = this.Height;
            this.preferences.Model.WindowWidth = this.Width;
            this.preferences.Model.WindowTop = this.Top;
            this.preferences.Model.WindowLeft = this.Left;
            this.preferences.Model.WindowState = this.WindowState;

            this.preferences.Save();
        }

        private void RestoreLastWindowPosition()
        {
            this.Height = this.preferences.Model.WindowHeight;
            this.Width = this.preferences.Model.WindowWidth;
            this.Top = this.preferences.Model.WindowTop;
            this.Left = this.preferences.Model.WindowLeft;
            this.WindowState = this.preferences.Model.WindowState;
        }
    }
}
