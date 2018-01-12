using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Aptxcode
{
    partial class ShowBox : Form
    {
        private Aptx aptx = null;
        public ShowBox(string text)
        {
            InitializeComponent();
            textlog.Text = text;
            textlog.Select(0, 0);
            this.Text = "更新内容";
        }

        public ShowBox(Aptx aptx, string text, string title)
        {
            InitializeComponent();
            this.aptx = aptx;
            textlog.Text = text;
            textlog.Select(0, 0);
            this.Text = title;
        }

        private void ShowBox_Load(object sender, EventArgs e)
        {
            if (aptx != null)
                aptx.HandlePreview = this.Handle;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (aptx != null)
                aptx.HandlePreview = IntPtr.Zero;
        }
    }
}