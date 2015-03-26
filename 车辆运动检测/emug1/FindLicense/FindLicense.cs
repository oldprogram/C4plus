using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace emug1
{
    class FindLicense
    {
        public int car_num = 0;
        public Image<Bgr, Byte> getLicense(Image<Bgr, Byte> image)
        {
            //Bgr color = image[0, 0];
            //double a=color.Blue;
            
            
            Image<Gray, Byte> B_R = image[0]-image[2];
            Image<Gray, Byte> dest = new Image<Gray, Byte>(B_R.Size);
            CvInvoke.cvThreshold(B_R, dest, 70, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);
            CvInvoke.cvCanny(dest, dest, 20, 90, 3);
            

            byte[,,] img = dest.Data;
            int most_top = dest.Height;
            int most_down = 0;
            int most_left = dest.Width;
            int most_right = 0;
            for (int i = 0; i < dest.Height; i++)
            {
                for (int j = 0; j < dest.Width; j++)
                {
                    if (img[i, j, 0] == 255)
                    {
                        if (i < most_top) most_top = i;
                        if (i > most_down) most_down = i;
                        if (j < most_left) most_left = j;
                        if (j > most_right) most_right = j;
                    }
                }
            }
            Rectangle rect = new Rectangle(most_left, most_top, most_right - most_left, most_down - most_top);
            //if (25 < rect.Height && rect.Height < 55 && 80 < rect.Width && rect.Width < 160)
            if (rect.Height > 0 && rect.Width > 0)
                image.GetSubRect(rect).Save("BR//rr" + car_num++.ToString() + ".jpg");
            if (rect.Height > 0 && rect.Width > 0)
                return image.GetSubRect(rect);
            return null;
        }
    }
}
