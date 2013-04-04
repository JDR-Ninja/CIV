using System;
using System.Drawing;
using System.Drawing.Imaging;
using Videotron;

namespace CIV.Common
{
    /// <summary>
    /// Draw a pie chart using the given information.
    /// </summary>
    public class UsagePieChart
    {
        private VideotronAccount _account;
        private Color _combinedColor;
        private Color _combinedRemainingColor;

        public UsagePieChart(VideotronAccount account, Color combinedColor, Color combinedRemainingColor)
        {
            _account = account;
            _combinedColor = combinedColor;
            _combinedRemainingColor = combinedRemainingColor;
        }

        public Icon GenerateIcon()
        {
            return ConvertToIcon(Generate());
        }

        public Bitmap Generate()
        {
            int width = 16;
            int height = 16;
            decimal[] vals = new decimal[2];
            vals[0] = Convert.ToDecimal(_account.Combined);
            vals[1] = Convert.ToDecimal(_account.CombinedRemaining);

            // Create a new image and erase the background
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            SolidBrush brush = new SolidBrush(Color.Transparent);
            graphics.FillRectangle(brush, 0, 0, width, height);
            brush.Dispose();

            // Create brushes for coloring the pie chart
            SolidBrush[] brushes = new SolidBrush[2];
            brushes[0] = new SolidBrush(_combinedColor);
            brushes[1] = new SolidBrush(_combinedRemainingColor);

            // Sum the inputs to get the total
            decimal total = 0.0m;
            foreach (decimal val in vals)
                total += val;

            // Draw the pie chart
            float start = 0.0f;
            float end = 0.0f;
            decimal current = 0.0m;
            for (int i = 0; i < vals.Length; i++)
            {
                current += vals[i];
                start = end;
                end = (float)(current / total) * 360.0f;
                graphics.FillPie(brushes[i % 10], 0.0f, 0.0f, width, height, start, end - start);
            }

            // Clean up the brush resources
            foreach (SolidBrush cleanBrush in brushes)
                cleanBrush.Dispose();

            return bitmap;
        }

        /// <summary>
        /// Converts an image into an icon.
        /// </summary>
        /// <param name="img">The image that shall become an icon</param>
        /// <param name="size">The width and height of the icon. Standard
        /// sizes are 16x16, 32x32, 48x48, 64x64.</param>
        /// <param name="keepAspectRatio">Whether the image should be squashed into a
        /// square or whether whitespace should be put around it.</param>
        /// <returns>An icon!!</returns>
        private Icon ConvertToIcon(Image img)
        {
            bool keepAspectRatio = true;
            int size = 16;

            Bitmap square = new Bitmap(size, size); // create new bitmap
            Graphics g = Graphics.FromImage(square); // allow drawing to it

            int x, y, w, h; // dimensions for new image

            if (!keepAspectRatio || img.Height == img.Width)
            {
                // just fill the square
                x = y = 0; // set x and y to 0
                w = h = size; // set width and height to size
            }
            else
            {
                // work out the aspect ratio
                float r = (float)img.Width / (float)img.Height;

                // set dimensions accordingly to fit inside size^2 square
                if (r > 1)
                { // w is bigger, so divide h by r
                    w = size;
                    h = (int)((float)size / r);
                    x = 0; y = (size - h) / 2; // center the image
                }
                else
                { // h is bigger, so multiply w by r
                    w = (int)((float)size * r);
                    h = size;
                    y = 0; x = (size - w) / 2; // center the image
                }
            }

            // make the image shrink nicely by using HighQualityBicubic mode
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, x, y, w, h); // draw image with specified dimensions
            g.Flush(); // make sure all drawing operations complete before we get the icon

            // following line would work directly on any image, but then
            // it wouldn't look as nice.
            return Icon.FromHandle(square.GetHicon());
        }
    }
}