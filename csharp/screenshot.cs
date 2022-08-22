using System;
using System.Drawing;

namespace Shot
{
    public class Shot
    {
        public static void Main()
        {
            Rectangle bounds = new System.Drawing.Rectangle(0, 0, 1920, 1080);
            Bitmap bmp = new System.Drawing.Bitmap(bounds.Width, bounds.Height);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            bmp.Save("shot.bmp");
            graphics.Dispose();
            bmp.Dispose();
        }
    }
}