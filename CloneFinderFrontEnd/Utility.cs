using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CloneFinderFrontEnd
{
    public static class Utility
    {
        public static void PositionStartupForm(Form startupForm)
        {
            // Set the X/Y positioning if the
            // position is > (0,0) and
            // the upper left hand corner is actually on screen
            if (Properties.Settings.Default.LastXPosition >= 0 && Properties.Settings.Default.LastYPosition >= 0 &&
                Properties.Settings.Default.LastXPosition < Screen.FromControl(startupForm).Bounds.Width &&
                Properties.Settings.Default.LastYPosition < Screen.FromControl(startupForm).Bounds.Height) 
            {
                startupForm.Location = new Point(Properties.Settings.Default.LastXPosition, Properties.Settings.Default.LastYPosition);
            }
            // Set the startup form size
            // if it's greater than the minimum
            if (Properties.Settings.Default.LastHeight > startupForm.MinimumSize.Height) startupForm.Height = Properties.Settings.Default.LastHeight;
            if (Properties.Settings.Default.LastWidth > startupForm.MinimumSize.Width) startupForm.Width = Properties.Settings.Default.LastWidth;

        }

        public static void SaveMainFormPosition(Form mainForm)
        {
            Properties.Settings.Default.LastHeight = mainForm.Height;
            Properties.Settings.Default.LastWidth = mainForm.Width;
            Properties.Settings.Default.LastXPosition = mainForm.Location.X;
            Properties.Settings.Default.LastYPosition = mainForm.Location.Y;
            Properties.Settings.Default.Save();
        }
    }
}
