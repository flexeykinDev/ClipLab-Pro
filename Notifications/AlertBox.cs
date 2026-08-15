using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipLab.Notifications
{
    public partial class AlertBox : Form
    {
        public AlertBox()
        {
            InitializeComponent();
        }


        public Color BackColorAlertBox
        {
            get { return this.BackColor; }
            set { this.BackColor = value; }
        }
        public Color ColorAlertBox
        {
            get { return LinAlertBox.BackColor; }
            set { LinAlertBox.BackColor = lblTitleAlertBox.ForeColor = lblTextAlertBox.ForeColor = value; }
        }
        public Image IconeAlertBox
        {
            get { return picAlertBox.Image; }
            set { picAlertBox.Image = value; }
        }
        public string TitleAlertBox
        {
            get { return lblTitleAlertBox.Text; }
            set { lblTitleAlertBox.Text = value; }
        }
        public string TextAlertBox
        {
            get { return lblTextAlertBox.Text; }
            set { lblTextAlertBox.Text = value; }
        }
        private void PositionAlertBox()
        {
            int xPos = 0; int yPos = 0;
            xPos = Screen.GetWorkingArea(this).Width;
            yPos = Screen.GetWorkingArea(this).Height;
            this.Location = new Point(xPos - this.Width, yPos - this.Height);

        }

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            LinAlertBox.Width += 2;
            if (LinAlertBox.Width >= 500)
                this.Close();
        }

        private void AlertBox_Load(object sender, EventArgs e)
        {
            PositionAlertBox();
            timerAnimation.Start();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
