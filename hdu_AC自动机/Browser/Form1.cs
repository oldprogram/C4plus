using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Browser
{
    public partial class Form1 : Form
    {
        private readonly int MOUSEEVENTF_LEFTDOWN = 0x2;
        private readonly int MOUSEEVENTF_LEFTUP = 0x4;
        [DllImport("user32")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);

        /// <summary>
        /// 0初始状态；1填写用户名和密码状态；2输入找到的代码；3查看是否AC；
        /// </summary>
        static int input_state = 0;
        /// <summary>
        /// 0初始状态；1移动鼠标聚焦name和password输入；2移动鼠标聚焦code输入
        /// </summary>
        int mouse_state = 0;
        /// <summary>
        /// 0初始页面；1登陆页面；2提交代码页面；3查看AC页面
        /// </summary>
        int page_state = 0;
        /// <summary>
        /// 0初始情况；1已搜索链接；2代码解析中；3代码解析完毕
        /// </summary>
        static int search_state = 0;
        static int Ti_num = 1000;

        public Form1()
        {
            InitializeComponent();
        }

        private void webBrowser1_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Text = webBrowser1.DocumentTitle.ToString();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                webBrowser1.Navigate(textBox1.Text);
            }
        }

        private void webBrowser1_Navigated(object sender,
           WebBrowserNavigatedEventArgs e)
        {
            textBox1.Text = webBrowser1.Url.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonBack.Enabled = false;
            buttonForward.Enabled = false;
            buttonStop.Enabled = false;

            textBox1.Text = "http://acm.hdu.edu.cn/submit.php?pid="+Ti_num.ToString();

            this.webBrowser1.CanGoBackChanged +=new EventHandler(webBrowser1_CanGoBackChanged);
            this.webBrowser1.CanGoForwardChanged +=new EventHandler(webBrowser1_CanGoForwardChanged);
            this.webBrowser1.DocumentTitleChanged +=new EventHandler(webBrowser1_DocumentTitleChanged);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
            textBox1.Text = webBrowser1.Url.ToString();
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
            textBox1.Text = webBrowser1.Url.ToString();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            webBrowser1.Stop();
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
            textBox1.Text = webBrowser1.Url.ToString();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            //page_state = 2;
            //webBrowser1.Navigate("http://acm.hdu.edu.cn/status.php");
            webBrowser1.Refresh();//refresh不会触发DocumentCompleted，用上面方法
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate(textBox1.Text);
        }

        private void webBrowser1_CanGoBackChanged(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoBack)
            {
                buttonBack.Enabled = true;
            }
            else
            {
                buttonBack.Enabled = false;
            }
        }

        private void webBrowser1_CanGoForwardChanged(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoForward)
            {
                buttonForward.Enabled = true;
            }
            else
            {
                buttonForward.Enabled = false;
            }
        }

        private void webBrowser1_Navigating(object sender,
           WebBrowserNavigatingEventArgs e)
        {
            buttonStop.Enabled = true;
        }

        private void webBrowser1_DocumentCompleted(object sender,
           WebBrowserDocumentCompletedEventArgs e)
        {
            if(Ti_num>5181)System.Environment.Exit(0);
            buttonStop.Enabled = false;
            //textBox2.Text = webBrowser1.DocumentText;
            switch (page_state){
                case 0: 
                    page_state++;
                    mouse_state = 1;
                    break;
                case 1:
                    page_state++;
                    input_state = 2;
                    if (AC_state == 3) search_state = 1;
                    else search_state = 0;
                    break;
                case 2: 
                    page_state++;
                    input_state = 3;
                    search_state = 0;
                    break;
                case 3:
                    //if (AC_state == -1)
                    //{
                    show += "reflash \r\n";
                    textBox2.Text = show;
                    input_state = 3;
                    search_state = 0;
                    //}
                    break;
                default: break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mouse_state != 0)
            {
                switch (mouse_state)
                {
                    case 1:
                        int pos_x = this.Left + webBrowser1.Right;
                        int pos_y = this.Top + webBrowser1.Top + 60;
                        SetCursorPos(pos_x, pos_y);                                                           //移动鼠标
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);     //发送鼠标信息
                        pos_y += 260;
                        SetCursorPos(pos_x, pos_y);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        pos_x = this.Left + 520;
                        pos_y = this.Top + 203;
                        SetCursorPos(pos_x, pos_y);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);     //发送鼠标信息
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        input_state = 1;
                        mouse_state = 0;
                        break;
                    case 2: 
                        pos_x = this.Left + webBrowser1.Right;
                        pos_y = this.Top + webBrowser1.Top + 60;
                        SetCursorPos(pos_x, pos_y);                                                           //移动鼠标
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);     //发送鼠标信息
                        pos_y += 260;
                        SetCursorPos(pos_x, pos_y);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        pos_x = this.Left + 520;
                        pos_y = this.Top + 203;
                        SetCursorPos(pos_x, pos_y);
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);     //发送鼠标信息
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        input_state = 2;
                        search_state = 3;
                        mouse_state = 0;
                        break;
                    case 3: break;
                    default: break;
                }
            }
            else if (input_state != 0)
            {
                switch(input_state){
                case 1:
                    SendKeys.Send("beautifulzzzz1");
                    SendKeys.Send("{TAB}");
                    SendKeys.Send("123456");
                    SendKeys.Send("{ENTER}");
                    input_state = 0;
                    break;
                case 2:
                    switch (search_state){
                    case 0: 
                        OK = false;
                        num = 0;
                        show = "";
                        Code = "";
                        new Thread(operator0).Start();
                        new Thread(op_getCode1).Start();
                        search_state = 2;
                        break;
                    case 1:
                        num++;
                        if (num > 9) 
                        {
                            show += "Can not solve!\r\n";
                            input_state = 3;
                            search_state = 2;
                            AC_state = 2;
                        }
                        Code = "";
                        new Thread(op_getCode1).Start();
                        search_state = 2;
                        break;
                    case 2:
                        textBox2.Text = show;
                        if (op1_state != 0)
                        {
                            if (Code.Equals("") != true)
                            {
                                mouse_state = 2;
                                input_state = 0;
                            }
                            else
                            {
                                input_state = 3;
                                search_state = 2;
                                AC_state = 3;
                            }
                        }
                        break;
                    case 3:
                        Clipboard.Clear();
                        Clipboard.SetData(DataFormats.Text, Code);//复制内容到剪切板
                        SendKeys.Send("^v");
                        SendKeys.Send("{TAB}");
                        SendKeys.Send("{ENTER}");
                        input_state = 0;
                        break;
                    default: break;
                    }
                    break;
                case 3:
                    switch (search_state){
                    case 0:
                        html = webBrowser1.DocumentText;
                        AC_state = 0;
                        new Thread(isAccept).Start();
                        search_state = 2;
                        break;
                    case 1: break;
                    case 2: 
                        textBox2.Text = show;
                        switch (AC_state){
                        case 0: 
                            break;
                        case 1://刷新status页面
                            //AC_state = -1;
                            input_state = 0;
                            page_state = 2;
                            webBrowser1.Navigate("http://acm.hdu.edu.cn/status.php");
                            break;
                        case 2://转到下一题
                            Ti_num++;
                            textBox1.Text = "http://acm.hdu.edu.cn/submit.php?pid=" + Ti_num.ToString();
                            input_state = 0;
                            page_state = 1;
                            AC_state = 0;
                            webBrowser1.Navigate(textBox1.Text);
                            break;
                        case 3://转到下一链接
                            page_state = 1;
                            input_state = 0;
                            webBrowser1.Navigate("http://acm.hdu.edu.cn/submit.php?pid=" + Ti_num.ToString());
                            break;
                        default: 
                            break;
                        }
                        break;
                    case 3: break;
                    default: break;
                    }
                    break;
                default:
                    break;
                }
            }
            
        }
       
        /// <summary>
        /// 判断是否AC
        /// </summary>
        /// <param name="obj"></param>
        private static string html = "";
        /// <summary>
        /// 0:初始状态；1:Queuing状态；2：Accepted状态；3：错误状态
        /// </summary>
        private static int AC_state = 0;
        private static void isAccept(object obj)
        {
            try
            {
                html = html.Substring(html.IndexOf("table_text"));
                html = html.Substring(html.IndexOf("table_header"));
                html = html.Substring(html.IndexOf("center") + 5);
                for (int i = 0; i < 15; i++)
                {
                    html = html.Substring(html.IndexOf("center") + 5);
                    string temp = html.Substring(0, html.IndexOf("</tr>"));
                    int find1 = temp.LastIndexOf("userstatus.php?user=") + 20;
                    int length1 = temp.Substring(find1).IndexOf('"');
                    int find2 = temp.IndexOf("font color=") + 11;
                    int length2 = 0;
                    while (find2 < temp.Length && temp[find2] != '>') find2++;
                    find2++;
                    while (find2 + length2 < temp.Length && temp[find2 + length2] != '<') length2++;
                    string user = temp.Substring(find1, length1);
                    string state = temp.Substring(find2, length2);
                    show += "user : " + user + "\r\n";
                    show += "state: " + state + "\r\n";
                    if (user.Equals("beautifulzzzz1"))
                    {
                        if (state.Equals("Queuing") || state.Equals("Compiling") || state.Equals("Running"))
                        {
                            AC_state = 1;
                        }
                        else if (state.Equals("Accepted"))
                        {
                            AC_state = 2;
                        }
                        else
                        {
                            AC_state = 3;
                        }
                        break;
                    }
                    //show += temp + "\r\n";
                }
            }
            catch(Exception e)
            {
                input_state = 3;
                search_state = 2;
                AC_state = 3;
            }
        }

        /// <summary>
        /// 从互联网获取代码
        /// </summary>
        private static string[] link = new string[20];
        private static bool OK = false;
        private static int num = 0;
        private static string show = "";
        private static string Code = "";
        private static void operator0(object obj)
        {
            try
            {
                show += "Is Find The Links...\r\n";
                System.Net.WebClient client = new WebClient();
                Stream strm = client.OpenRead("http://www.baidu.com/s?word=hdu%20" + Ti_num.ToString() + "&pn=0");
                StreamReader sr = new StreamReader(strm);
                string line;
                int num = 1;
                int find1 = -1;
                while ((line = sr.ReadLine()) != null)
                {
                    find1 = line.IndexOf("content_left");
                    if (find1 != -1) break;
                }
                if (find1 != -1)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int find2 = -1;
                        do
                        {
                            find2 = line.Substring(find1).IndexOf("result c-container ");
                            if (find2 != -1) break;
                            find1 = 0;
                        } while ((line = sr.ReadLine()) != null);
                        if (find2 == -1) break;
                        int find3 = -1;
                        do
                        {
                            find3 = line.Substring(find1 + find2).IndexOf("href");
                            if (find3 != -1) break;
                            find1 = find2 = 0;
                        } while ((line = sr.ReadLine()) != null);
                        if (find3 == -1) break;
                        int find4 = -1;
                        string temp = "";
                        do
                        {
                            find4 = line.Substring(find1 + find2 + find3 + 8).IndexOf("\"");
                            if (find4 != -1) break;
                            temp += line.Substring(find1 + find2 + find3 + 8);
                            find1 = find2 = find3 = 0;
                        } while ((line = sr.ReadLine()) != null);
                        if (find4 == -1) break;
                        temp += line.Substring(find1 + find2 + find3 + 8, find4);
                        //Console.WriteLine("[" + num.ToString() + "]" + temp);
                        link[i] = temp;
                        //show += ("[" + num.ToString() + "]" + temp + "\r\n");
                        num++;
                    }
                }
                strm.Close();
                //Console.Read();
                OK = true;
            }
            catch (Exception e)
            {
                input_state = 3;
                search_state = 2;
                AC_state = 2;
            }
        }

        /// <summary>
        /// 0:没好；1：线程结束；2：没找到代码线程结束
        /// </summary>
        private static int op1_state = 0;
        private static void op_getCode1(object obj)
        {
            try
            {
                op1_state = 0;
                while (OK == false) ;
                if (link.Equals("") == true)
                {
                    show += "No Link Can Use!\r\n";
                    op1_state = 2;
                    return;
                }
                show += ("[" + num.ToString() + "]" + link[num] + "\r\n");
                System.Net.WebClient client = new WebClient();
                Stream strm = null;
                try
                {
                    strm = client.OpenRead(link[num]);
                }
                catch (Exception e)
                {
                    op1_state = 2;
                    return;
                }
                StreamReader sr = new StreamReader(strm);
                string line, code = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.IndexOf("<body") != -1) break;
                }
                int find1 = -1;
                do
                {
                    find1 = line.IndexOf("#include");
                    if (find1 != -1) break;
                } while ((line = sr.ReadLine()) != null);
                if (find1 == -1)
                {
                    strm.Close();
                    op1_state = 2;
                    return;
                }
                int find2 = -1;
                do
                {
                    code += cut_ext(line.Substring(find1));
                    find2 = line.Substring(find1).IndexOf("main");
                    if (find2 != -1) break;
                    find1 = 0;
                } while ((line = sr.ReadLine()) != null);
                if (find2 == -1)
                {
                    strm.Close();
                    op1_state = 2;
                    return;
                }
                int proc_num = 0;
                string temp2;
                bool ok = false;
                bool first = true;
                do
                {
                    if (first) first = false;
                    else code += cut_ext(line);//防止重复
                    temp2 = line.Substring(find1 + find2);
                    for (int i = 0; i < temp2.Length; i++)
                    {
                        if (temp2[i] == '{') proc_num++;
                        else if (temp2[i] == '}')
                        {
                            proc_num--;
                            if (proc_num <= 0)
                            {
                                ok = true;
                                break;
                            }
                        }
                    }
                    if (ok) break;
                    find1 = find2 = 0;
                } while ((line = sr.ReadLine()) != null);
                if (proc_num == -1)
                {
                    strm.Close();
                    op1_state = 2;
                    return;
                }
                //Console.WriteLine(code);

                Code = code;
                op1_state = 1;
                strm.Close();
                return;
            }
            catch (Exception e)
            {
                op1_state = 2;
                return;
            }
            
        }

        /// <summary>
        /// 给一行把css去掉，把转译符转换，特殊的把<br />换成换行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string cut_ext(string line)
        {
            bool left = false;
            string code = "";
            line = line.Replace("<br />", "\n");
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '<')
                {
                    left = true;
                }
                else if (line[i] == '>')
                {
                    left = false;
                }
                else if (left == false)
                {
                   code += line[i];
                }
            }
            code = code.Replace("&lt;", "<");
            code = code.Replace("&gt;", ">");
            code = code.Replace("&quot;", "\"");
            code = code.Replace("&nbsp;", " ");
            code = code.Replace("&amp;", "&");
            code = code.Replace("&#43;", "+");
            code = code.Replace("&#39;", "'");
            code = code.Replace("/n", "\\n");
            show += code + "\r\n";
            return code + "\n";
        }
    }
}