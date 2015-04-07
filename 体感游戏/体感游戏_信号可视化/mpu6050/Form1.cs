using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;               //端口
using System.Text.RegularExpressions;//正则表达式
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using MusicControl.Draw;
using MusicControl.Date;//控制DllImport()。。。。

namespace MusicControl
{
    public partial class Form1 : Form
    {
        public DrawLineChart chart;//绘制折线图
        public List<int> P;//存放数据X轴加速度
        public List<int> Q;//存放数据Y轴加速度
        public List<int> R;//存放数据Z轴加速度

        public DatePool datepool;//数据池


        public Form1()
        {
            InitializeComponent();
            //折线图初始化
            chart = new DrawLineChart(20,0,450,300,10,300);
            P = new List<int>();
            Q = new List<int>();
            R = new List<int>();
            //数据池初始化
            datepool = new DatePool(100000);

            //Get all port list for selection
            //获得所有的端口列表，并显示在列表内
            PortList.Items.Clear();
            string[] Ports = SerialPort.GetPortNames();

            for (int i = 0; i < Ports.Length; i++)
            {
                string s = Ports[i].ToUpper();
                Regex reg = new Regex("[^COM\\d]", RegexOptions.IgnoreCase | RegexOptions.Multiline);//正则表达式
                s = reg.Replace(s, "");

                PortList.Items.Add(s);
            }
            if (Ports.Length >1) PortList.SelectedIndex = 1;
        }

        //Create a serial port for Connection
        SerialPort Connection = new SerialPort();
        private void btn_link_Click(object sender, EventArgs e)
        {
            if (!Connection.IsOpen)
            {
                //Start
                //Status = "正在连接...";
                Connection = new SerialPort();
                btn_link.Enabled = false;
                Connection.PortName = PortList.SelectedItem.ToString();
                Connection.Open();
                Connection.ReadTimeout = 10000;
                Connection.DataReceived += new SerialDataReceivedEventHandler(PortDataReceived);
                //Status = "连接成功";
                timer1.Start();
            }
        }

        //接收串口数据
        int data_X, data_Y, data_Z;
        string getString = "";
        int length = 20;
       
        private void PortDataReceived(object o, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[length];
            int num=Connection.Read(data, 0, length);
            datepool.push_back(data,num);//实际接收的不一定是length，之前一直错
            Connection.DiscardInBuffer();
            Connection.DiscardOutBuffer();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!datepool.ask())return;
            getString = System.Text.Encoding.Default.GetString(datepool.str);
            data_X = datepool.X;
            data_Y = datepool.Y;
            data_Z = datepool.Z;
            X.Text = data_X.ToString();
            Y.Text = data_Y.ToString();
            Z.Text = data_Z.ToString();
            listBox1.Items.Add(getString);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            P.Add(data_X / 260);
            Q.Add(data_Y / 260);
            R.Add(data_Z / 260);
            time1 = DateTime.Now;
            ////http://zhidao.baidu.com/link?url=d2NlgPajuMD2mxJfWMp9-Ry6lXkX7G9_yKRCk6Iq7aN_4c7cETZRCdLFQcajRIS6v7qqHdpPKGK-Z6ljo52VrbUbWtA57o1PTYVnXQiAeam
            pictureBox1.Refresh();//先执行refresh()进行picture重绘，然后执行下一句
            time2 = DateTime.Now;
        }

        TimeSpan spend1, spend2, spend3;
        DateTime time1, time2;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
            //////chart.DrawLineChar(sender, e, Q);
            chart.Draw3LineChar(sender, e, P, Q, R);
            
           
            spend1 = time1 - time2;
            T1.Text = spend1.Milliseconds.ToString();
        }

        int cutPic_num = 0;
        private void btn_save_Click(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bit, pictureBox1.ClientRectangle);
            bit.Save("Picture"+cutPic_num++.ToString()+".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            bit.Dispose();
        }
    }
}
