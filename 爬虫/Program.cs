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
            Stream strm = client.OpenRead("http://www.baidu.com/s?word=hdu%201202&pn=0");
            StreamReader sr = new StreamReader(strm);
            string line;
            StreamWriter sw = new StreamWriter("D:\\1.txt");
            int num = 1;

            while ((line = sr.ReadLine()) != null)
            {
                for (int i = 0; i < line.Length - 7; i++)//找line中的http开头的链接
                {
                    if (line[i] == 'h')
                    {
                        if (line[i + 1] == 't' && line[i + 2] == 't' && line[i + 3] == 'p' && line[i + 4] == ':')
                        {
                            int last = i + 7;
                            while (last < line.Length && line[last++] != '"') ;
                            last--;
                            if (line[last] == '"')
                            {
                                string temp = line.Substring(i, last - i);
                                int find_blog = temp.IndexOf("blog");
                                int find_baidu = temp.IndexOf("baidu");
                                if (find_blog != -1 && find_baidu==-1)
                                {
                                    temp = temp.Replace("\\", "");
                                    Console.WriteLine("["+num.ToString()+"]"+temp);
                                    sw.WriteLine("[" + num.ToString() + "]" + temp);
                                    client.DownloadFile(temp, "D:\\download"+num.ToString() + ".html");
                                    num++;
                                }
                            }
                        }
                    }
                }
               // sw.WriteLine("\n--------------------------------------------------------------\n");
            }
            sw.Close();
            strm.Close();
        }
    }
}
