namespace ClienteDesktop.Helpers.ImageHelper
{
    public class ImageHelper
    {
        // RESIZES AN IMAGE TO A SPECIFIED SIZE WITH HIGH QUALITY
        public Image ResizeImage(Image image, Size size)
        {
            Bitmap newBitmap = new Bitmap(size.Width, size.Height);
            
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                
                g.DrawImage(image, 0, 0, size.Width, size.Height);
            }
            
            return newBitmap;
        }
    }
}