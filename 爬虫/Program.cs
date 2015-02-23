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
            StreamWriter sw = new StreamWriter("D:\\过程.txt");
            StreamWriter sw2 = new StreamWriter("D:\\结果.txt");
            int num = 1;
            int find1=-1;
            while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine("1: " + line);
                find1 = line.IndexOf("content_left");
                if(find1!=-1)break;
            }
            if(find1!=-1)
            {
                for (int i = 0; i < 10; i++)
                {
                    int find2 = -1;
                    do{
                        sw.WriteLine("2: " + line);
                        find2 = line.Substring(find1).IndexOf("result c-container ");
                        if (find2 != -1) break;
                        find1 = 0;
                    } while ((line = sr.ReadLine()) != null);
                    if (find2 == -1) break;

                    int find3 = -1;
                    do{
                        sw.WriteLine("3: " + line);
                        find3 = line.Substring(find1+find2).IndexOf("href");
                        if (find3 != -1) break;
                        find1 = find2 = 0;
                    } while ((line = sr.ReadLine()) != null);
                    if (find3 == -1) break;

                    int find4 = -1;
                    string temp = "";
                    do{
                        sw.WriteLine("4: " + line);
                        find4 = line.Substring(find1 + find2 + find3 + 8).IndexOf("\"");
                        if (find4 != -1) break;
                        temp += line.Substring(find1 + find2 + find3 + 8);
                        find1 = find2 = find3 = 0;
                    } while ((line = sr.ReadLine()) != null);
                    if (find4 == -1) break;

                    temp += line.Substring(find1+find2+find3 + 8, find4);
                    
                    Console.WriteLine("[" + num.ToString() + "]" + temp);
                    sw2.WriteLine("[" + num.ToString() + "]" + temp);
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
    }
}
