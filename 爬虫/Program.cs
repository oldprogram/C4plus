using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Net.WebClient client = new WebClient();
            Stream strm = client.OpenRead("http://www.baidu.com/s?word=hdu%201097&pn=0");
            StreamReader sr = new StreamReader(strm);
            string line;
            StreamWriter sw = new StreamWriter("D:\\过程.txt");
            StreamWriter sw2 = new StreamWriter("D:\\结果.txt");
            int num = 1;
            int find1 = -1;
            while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine("1: " + line);
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
                        sw.WriteLine("2: " + line);
                        find2 = line.Substring(find1).IndexOf("result c-container ");
                        if (find2 != -1) break;
                        find1 = 0;
                    } while ((line = sr.ReadLine()) != null);
                    if (find2 == -1) break;

                    int find3 = -1;
                    do
                    {
                        sw.WriteLine("3: " + line);
                        find3 = line.Substring(find1 + find2).IndexOf("href");
                        if (find3 != -1) break;
                        find1 = find2 = 0;
                    } while ((line = sr.ReadLine()) != null);
                    if (find3 == -1) break;

                    int find4 = -1;
                    string temp = "";
                    do
                    {
                        sw.WriteLine("4: " + line);
                        find4 = line.Substring(find1 + find2 + find3 + 8).IndexOf("\"");
                        if (find4 != -1) break;
                        temp += line.Substring(find1 + find2 + find3 + 8);
                        find1 = find2 = find3 = 0;
                    } while ((line = sr.ReadLine()) != null);
                    if (find4 == -1) break;

                    temp += line.Substring(find1 + find2 + find3 + 8, find4);

                    Console.WriteLine("[" + num.ToString() + "]" + temp);
                    sw2.WriteLine("[" + num.ToString() + "]" + temp);
                    op_getCode1(temp, num);
                    op_getCode2(num);
                    client.DownloadFile(temp, "D:\\download" + num.ToString() + ".html");
                    num++;
                    //find1 = find1 + find2 + find3 + find4;
                }
            }

            sw.Close();
            sw2.Close();
            strm.Close();
            Console.Read();
        }

        private static bool op_getCode1(string temp, int num)
        {
            System.Net.WebClient client = new WebClient();
            Stream strm = client.OpenRead(temp);
            StreamReader sr = new StreamReader(strm);
            StreamWriter sw = new StreamWriter("D:\\op_getCode1_["+num.ToString()+"]过程.txt");
           // StreamWriter sw2 = new StreamWriter("D:\\op_getCode1_[" + num.ToString() + "]结果.txt");
            string line;
            
            while ((line = sr.ReadLine()) != null)
            {
               if(line.IndexOf("<body")!=-1)break;
            }

            int find1 = -1;
            do{
                find1 = line.IndexOf("#include");
                if (find1 != -1) break;
            } while ((line = sr.ReadLine()) != null);
            if (find1 == -1)
            {
                sw.Close();
            //    sw2.Close();
                strm.Close();
                return false;
            }
        
            int find2 = -1;
            do{
                sw.WriteLine(line);
                find2 = line.Substring(find1).IndexOf("main");
                if (find2 != -1) break;
                find1 = 0;
            } while ((line = sr.ReadLine()) != null);
            if (find2 == -1)
            {
                sw.Close();
              //  sw2.Close();
                strm.Close();
                return false;
            }
    
            int proc_num = 0;
            string temp2;
            bool ok = false;
            bool first = true;
            do{
                if (first)first = false;
                else sw.WriteLine(line);//防止重复
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
                sw.Close();
                //sw2.Close();
                strm.Close();
                return false;
            }

            sw.Close();
          //  sw2.Close();
            strm.Close();
            return true;
        }

        private static void op_getCode2(int num)
        {
            FileStream fs = new FileStream("D:\\op_getCode1_[" + num.ToString() + "]过程.txt",FileMode.Open,FileAccess.Read,FileShare.None);
            StreamReader sr = new StreamReader(fs);
            StreamWriter sw = new StreamWriter("D:\\op_getCode2_[" + num.ToString() + "]结果.txt");
            string line;
            bool left = false;
            while ((line = sr.ReadLine()) != null)
            {
                string code = "";
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
                sw.WriteLine(code);
                Console.WriteLine(code);
            }
            sw.Close();
            sr.Close();
        }
   

    }
}
