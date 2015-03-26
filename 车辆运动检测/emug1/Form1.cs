using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Emgu.CV;//PS:调用的Emgu dll  
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using System.Collections.Generic;
using Emgu.CV.CvEnum;

namespace emug1
{
    public partial class Form1 : Form
    {
        private Capture _capture2 = null;
        private MotionHistory _motionHistory;
        private FindLicense _findLicense = null;
        MCvFont _myFont = new MCvFont(FONT.CV_FONT_HERSHEY_SCRIPT_COMPLEX, 2, 2);
        Rectangle _srcRect;
        public Form1()
        {
            InitializeComponent();
            _findLicense = new FindLicense();
            _srcRect = new Rectangle();
        }

        
        private IBGFGDetector<Bgr> _forgroundDetector;

        Image<Bgr, Byte> image1 = null,src=null,cutcar=null;
        List<Point> Car = new List<Point>(20);//定义一个用于跟踪汽车的list
        private void ProcessFrame(object sender, EventArgs e)
        {
            src = _capture2.QueryFrame();
            //src.Save("pic/b" + car_num.ToString() + ".bmp");
            if (src == null) return;
            else image1=src.Resize(0.20, 0);
            using (MemStorage storage = new MemStorage()) //create storage for motion components
            {
                if (_forgroundDetector == null)
                {
                    //_forgroundDetector = new BGCodeBookModel<Bgr>();
                    _forgroundDetector = new FGDetector<Bgr>(Emgu.CV.CvEnum.FORGROUND_DETECTOR_TYPE.FGD);
                    //_forgroundDetector = new BGStatModel<Bgr>(image, Emgu.CV.CvEnum.BG_STAT_TYPE.FGD_STAT_MODEL);
                }

                //用新的图片更新前景侦查器
                _forgroundDetector.Update(image1);
                //update the motion history，用新的前景侦查器的Mask更新motion的历史信息
                _motionHistory.Update(_forgroundDetector.ForgroundMask);
                

                #region get a copy of the motion mask and enhance its color
                double[] minValues, maxValues;
                Point[] minLoc, maxLoc;
                _motionHistory.Mask.MinMax(out minValues, out maxValues, out minLoc, out maxLoc);
                Image<Gray, Byte> motionMask = _motionHistory.Mask.Mul(255.0 / maxValues[0]);
                #endregion

                //create the motion image 
                Image<Bgr, Byte> motionImage = new Image<Bgr, byte>(motionMask.Size);
                //display the motion pixels in blue (first channel)
                motionImage[0] = motionMask;

                //Threshold to define a motion area, reduce the value to detect smaller motion
                double minArea = 1000;

                storage.Clear(); //clear the storage
                Seq<MCvConnectedComp> motionComponents = _motionHistory.GetMotionComponents(storage);

                //iterate through each of the motion component
                int i = 0;
                foreach (MCvConnectedComp comp in motionComponents)
                {
                    //reject the components that have small area;
                    if (comp.area < minArea) continue;

                    // find the angle and motion pixel count of the specific area
                    double angle, motionPixelCount;
                    _motionHistory.MotionInfo(comp.rect, out angle, out motionPixelCount);

                    //reject the area that contains too few motion
                    if (motionPixelCount < comp.area * 0.05) continue;
                    //这里进行汽车跟踪位置的更新
                    Point mid = new Point((comp.rect.Left + comp.rect.Right) >> 1, (comp.rect.Top + comp.rect.Bottom) >> 1);
                    
                    int minx = 1000000;
                    int posx = -1;
                    for (int f = 0; f < Car.Count; f++)
                    {
                        int dis = (Car[f].Y - mid.Y) * (Car[f].Y - mid.Y) + (Car[f].X - mid.X) * (Car[f].X - mid.X);
                        if(dis<minx)
                        {
                            minx = dis;
                            posx = f;
                        }
                    }
                    if (posx != -1)
                    {
                        Car[posx] = mid;
                    }
                    else
                    {
                        Car.Add(mid);
                    }

                    //原图被缩小，所以这里给放大
                    _srcRect.X = comp.rect.Left * 5;
                    _srcRect.Y = comp.rect.Top * 5;
                    _srcRect.Width = comp.rect.Width * 5;
                    _srcRect.Height = comp.rect.Height * 5;
                    cutcar = src.GetSubRect(_srcRect);
                    //cutcar.Save("pic//" + car_num++.ToString() + ".jpg");
                    imageBox(i).Image = cutcar;
                    imageBox(i+10).Image = _findLicense.getLicense(cutcar);
                    i++;
                    //Draw each individual motion in red
                    DrawMotion(motionImage, image1, comp.rect, angle, new Bgr(Color.Red));
                }

                // find and draw the overall motion angle
                double overallAngle, overallMotionPixelCount;
                _motionHistory.MotionInfo(motionMask.ROI, out overallAngle, out overallMotionPixelCount);
                DrawMotion(motionImage, image1, motionMask.ROI, overallAngle, new Bgr(Color.Green));

                //Display the amount of motions found on the current image
                UpdateText(String.Format("Total Motions found: {0}; Motion Pixel count: {1}", motionComponents.Total, overallMotionPixelCount));

                //Display the image of the motion
                capturedImageBox.Image = image1;
                forgroundImageBox.Image = _forgroundDetector.ForgroundMask;//前景
                motionImageBox.Image = motionImage;
            }
        }

        private Emgu.CV.UI.ImageBox imageBox(int i)
        {
            switch (i)
            {
                case 0:return imageBox_0;
                case 1: return imageBox_1;
                case 2: return imageBox_2;
                case 3: return imageBox_3;
                case 4: return imageBox_4;
                case 5: return imageBox_5;
                case 6: return imageBox_6;
                case 7: return imageBox_7;
                case 8: return imageBox_8;
                case 9: return imageBox_9;
                case 10: return imageBox_10;
                case 11: return imageBox_11;
                case 12: return imageBox_12;
                case 13: return imageBox_13;
                case 14: return imageBox_14;
                case 15: return imageBox_15;
                case 16: return imageBox_16;
                case 17: return imageBox_17;
                case 18: return imageBox_18;
                case 19: return imageBox_19;
            }
            return null;
        }

        private void UpdateText(String text)
        {
            if (InvokeRequired && !IsDisposed)
            {
                Invoke((Action<String>)UpdateText, text);
            }
            else
            {
                label1.Text = text;
            }
        }

        /// <summary>
        /// 绘制运动图
        /// </summary>
        /// <param name="image"></param>
        /// <param name="motionRegion"></param>
        /// <param name="angle"></param>
        /// <param name="color"></param>
        private void DrawMotion(Image<Bgr, Byte> image, Image<Bgr, Byte> image1, Rectangle motionRegion, double angle, Bgr color)
        {
            float circleRadius = (motionRegion.Width + motionRegion.Height) >> 2;
            Point center = new Point(motionRegion.X + (motionRegion.Width >> 1), motionRegion.Y + (motionRegion.Height >> 1));

            CircleF circle = new CircleF(
               center,
               circleRadius);

            int xDirection = (int)(Math.Cos(angle * (Math.PI / 180.0)) * circleRadius);
            int yDirection = (int)(Math.Sin(angle * (Math.PI / 180.0)) * circleRadius);
            Point pointOnCircle = new Point(
                center.X + xDirection,
                center.Y - yDirection);
            LineSegment2D line = new LineSegment2D(center, pointOnCircle);

            //把矩形框超出底边界1/8的地方的框绘制出
            //if (motionRegion.Y + motionRegion.Height < 7*(image1.Height >> 3) &&
            //    Math.Cos(angle * (Math.PI / 180.0)) > 0.0002)
            //{
            //    image1.Draw(motionRegion, color, 3);
            //}
            image.Draw(circle, color, 1);
            image.Draw(line, color, 2);

            for (int f = 0; f < Car.Count; f++)
            {
                image1.Draw(new Rectangle(Car[f].X - 10, Car[f].Y - 10, 20, 20), new Bgr(f*10,f*5,f*8), 3);
                CvInvoke.cvPutText(image1, f.ToString(), Car[f], ref _myFont, new MCvScalar(255,0,0));
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Idle -= new EventHandler(ProcessFrame);
        }

        /// <summary>
        /// 打开播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 播放视频ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FilterIndex = 5;
            openFileDialog.Filter = "AVI文件|*.avi|RMVB文件|*.rmvb|WMV文件|*.wmv|MKV文件|*.mkv|所有文件|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Application.Idle += new EventHandler(ProcessFrame);  //播放的函数句柄
                _capture2 = new Capture(openFileDialog.FileName);
                _motionHistory = new MotionHistory(
                    1.0, //in second, the duration of motion history you wants to keep
                    0.05, //in second, maxDelta for cvCalcMotionGradient
                    0.5); //in second, minDelta for cvCalcMotionGradient
            }
        }
    }
}
