using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClipLab
{
    public class EllipseControl : Component
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn(int nL, int nT, int nR, int nB, int nWidthEllipse, int nHeightEllipse);

        private Control? control;
        private int cornerRadius = 25;
        public Control? TargetControl
        {
            get { return control; }
            set
            {
                control = value;
                if (control != null)
                    control.SizeChanged += (sender, eventArgs) => control.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height, cornerRadius, cornerRadius));
            }
        }
        public int CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                if (control != null)
                    control.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height, cornerRadius, cornerRadius));
            }
        }
    }
}
