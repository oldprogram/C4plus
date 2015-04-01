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
using MusicControl.Draw;//控制DllImport()。。。。

namespace MusicControl
{
    public partial class Form1 : Form
    {
        public DrawLineChart chart;
        public List<int> P;
        public List<int> Q;
        public List<int> R;
        public Form1()
        {
            InitializeComponent();
            chart = new DrawLineChart(20,0,400,300,10,10);
            P = new List<int>();
            for (int i = 0; i < 20; i++) P.Add(i * 10);
            Q = new List<int>();
            for (int i = 0; i < 20; i++) Q.Add(i * 10);
            R = new List<int>();
            for (int i = 0; i < 20; i++) R.Add(i * 10);
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
        bool getData = false;
        string getString = "";
        int length = 20;
        private void PortDataReceived(object o, SerialDataReceivedEventArgs e)
        { 
            byte[] data = new byte[length];
            Connection.Read(data, 0, length);
            if (length == 1)
            {
                if (data[0] == '$') length = 20;
                return;
            }
            if (data[0] != '#' || data[length - 1] != '$')
            {
                length = 1;
                return;
            }
            time1 = DateTime.Now;
            data_X = 0;
            for(int i=2;i<7;i++)
            {
                data_X *= 10;
                data_X += (data[i] - '0');
            }
            if (data[1] == '-') data_X = -data_X;
            data_Y = 0;
            for (int i = 8; i < 13; i++)
            {
                data_Y *= 10;
                data_Y += (data[i] - '0');
            }
            if (data[7] == '-') data_Y = -data_Y;
            data_Z = 0;
            for (int i = 14; i < 19; i++)
            {
                data_Z *= 10;
                data_Z += (data[i] - '0');
            }
            if (data[13] == '-') data_Z = -data_Z;
            getString = System.Text.Encoding.Default.GetString(data);
            getData = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (getData == false) return;
            getData = false;
            time2 = DateTime.Now;
            T1.Text = (time2 - time1).Milliseconds.ToString();
            //time1 = DateTime.Now;
            //从得到新的date到重新绘制折线图完毕低于1ms
            //下面是用来测试的，原本一直为0，我以为计算时差的写错了呢
            //int n = 400000;
            //for (int i = 0; i < n; i++)
            //    for (int j = 0; j < 100; j++) ;
            X.Text = data_X.ToString();
            Y.Text = data_Y.ToString();
            Z.Text = data_Z.ToString();
            listBox1.Items.Add(getString);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            P.Add(data_X / 140);
            Q.Add(data_Y / 140);
            R.Add(data_Z / 140);
            //http://zhidao.baidu.com/link?url=d2NlgPajuMD2mxJfWMp9-Ry6lXkX7G9_yKRCk6Iq7aN_4c7cETZRCdLFQcajRIS6v7qqHdpPKGK-Z6ljo52VrbUbWtA57o1PTYVnXQiAeam
            pictureBox1.Refresh();//先执行refresh()进行picture重绘，然后执行下一句
        }

        DateTime time1, time2;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            ////chart.DrawLineChar(sender, e, Q);
            chart.Draw3LineChar(sender, e, P, Q, R);
            //T1.Text += "2";
            //time2 = DateTime.Now;
            //T1.Text = (time2-time1).Milliseconds.ToString();
        }
    }
}
