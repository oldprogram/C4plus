using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace find10link
{
    class Program
    {
        /// <summary>
        /// 刚开始输入"find10link zju 1001 0"表示搜索zju的第1001个题目的链接从0开始10个
        /// 结果保存在results.txt中
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                System.Net.WebClient client = new WebClient();
                Stream strm = client.OpenRead("http://www.baidu.com/s?word=" + args[0] + "%20" + args[1] + "&pn=" + args[2]);
                StreamReader sr = new StreamReader(strm);
                string line;
                StreamWriter sw = new StreamWriter("results.txt");
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
                        sw.WriteLine("[" + num.ToString() + "]:" + temp);
                        Console.WriteLine("[" + num.ToString() + "]:" + temp);
                        num++;
                    }
                }
                sw.Close();
                strm.Close();
            }catch(Exception e){}
        }
    }
}
