using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

    namespace netFteo.Drawing
{
    // Класс с визуальным компонентом должен быть первым в файле, чтобы дизайнер VisualStudio его показывал
        public class FteoImage : System.Windows.Forms.PictureBox // System.Drawing.Graphics  - закрытый класс, наследование запрещено
    {
        public System.Drawing.Graphics Body;
        public string ClassDefinition = "netfteo image for drawable classes ";
        public FteoImage()
        {
          //  this.Height = 127; // Размер Объекта, не картинки
          //  this.Width = 127;

            this.Image = new System.Drawing.Bitmap(130,130); // Размер Image здесь
            System.Drawing.Graphics ge = System.Drawing.Graphics.FromImage(this.Image); // this.Image - на чем рисуем
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 1);
            System.Drawing.Font font2 = new System.Drawing.Font("Arial", 8);
            ge.DrawEllipse(pen, 32, 32, 10, 10);
            ge.DrawRectangle(pen, 0, 0, this.Width-1, this.Height-1);
            ge.DrawString("2017@Fixosoft \n FteoImage class", font2, System.Drawing.Brushes.Black, 1, 2);
            this.SizeChanged += FteoImage_SizeChanged;
        }

        private void FteoImage_SizeChanged(object sender, EventArgs e)
        {
            this.Image = new System.Drawing.Bitmap(130, 130); // а так разрешение картинки ???
            System.Drawing.Graphics ge = System.Drawing.Graphics.FromImage(this.Image); // this.Image - на чем рисуем
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 1);
            System.Drawing.Font font2 = new System.Drawing.Font("Arial", 8);
            ge.DrawRectangle(pen, 1,1, this.Image.Width -5, this.Image.Height - 5);
            ge.DrawString("Fimage "+ this.Image.Width.ToString()+ "x" +this.Image.Width.ToString() , font2, System.Drawing.Brushes.Black, 1, 2);
        }
    }

        public class FteoJpeg  
        {
            //=== get encoder info
            private ImageCodecInfo getEncoderInfo(string mimeType)
            {
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

                for (int j = 0; j < encoders.Length; ++j)
                {
                    if (encoders[j].MimeType.ToLower() == mimeType.ToLower())
                    {
                        return encoders[j];
                    }
                }

                return null;
            }

            public Image resizeImage(Image image, int maxWidth, int maxHeight)
            {
                int newWidth;
                int newHeight;
                //set the resolution, 72 is usually good enough for displaying images on monitors
                float imageResolution = 72;

                //set the compression level. higher compression = better quality = bigger images
                long compressionLevel = 80L;


                //check if the with or height of the image exceeds the maximum specified, if so calculate the new dimensions
                if (image.Width > maxWidth || image.Height > maxHeight)
                {
                    double ratioX = (double)maxWidth / image.Width;
                    double ratioY = (double)maxHeight / image.Height;
                    double ratio = Math.Min(ratioX, ratioY);

                    newWidth = (int)(image.Width * ratio);
                    newHeight = (int)(image.Height * ratio);
                }
                else
                {
                    newWidth = image.Width;
                    newHeight = image.Height;
                }

                //start the resize with a new image
                Bitmap newImage = new Bitmap(newWidth, newHeight);

                //set the new resolution
                newImage.SetResolution(imageResolution, imageResolution);

                //start the resizing
                using (var graphics = Graphics.FromImage(newImage))
                {
                    //set some encoding specs
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                }

                //save the image to a memorystream to apply the compression level
                using (MemoryStream ms = new MemoryStream())
                {
                    System.Drawing.Imaging.EncoderParameters encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                    encoderParameters.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compressionLevel);
                    newImage.Save(ms, getEncoderInfo("image/jpeg"), encoderParameters);

                    //save the image as byte array here if you want the return type to be a Byte Array instead of Image
                    //byte[] imageAsByteArray = ms.ToArray();
                }

                //return the image
                return newImage;
            }
        }
}
