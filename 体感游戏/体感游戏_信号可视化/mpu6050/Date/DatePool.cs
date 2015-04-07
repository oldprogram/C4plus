using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicControl.Date
{
    /// <summary>
    /// 数据池，用来接收高速流入数据并获取最新有效信息
    /// 保证数据不产生影响
    /// </summary>
    public class DatePool
    {
        private int p_write;//正在写的位置
        private int pool_size;//池子容量
        private byte[] pool;//池子
        public int X, Y, Z;
      

        /// <summary>
        /// 构造函数
        /// 将读写位置置为0，申请一个大小为pool_size的char类型池子
        /// </summary>
        /// <param name="pool_size">池子大小</param>
        public DatePool(int pool_size)
        {
            p_write = 0;
            this.pool_size = pool_size;
            pool = new byte[pool_size];
            //new Thread(Run).Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>       
        //private void Run(object obj)
        //{
        //    int  p_read_from;
        //    int i = 0;
        //    int data_X, data_Y, data_Z;//

        //    while (true)
        //    {
        //        i = 0;//立刻将相应的40个字符复制出来
        //        p_read_from = p_write - 40;
        //        while (i < 40)
        //        {
        //            str[i] = pool[(p_read_from + pool_size) % pool_size];
        //            i++;
        //            p_read_from++;
        //        }
        //        i=39;
        //        while (i > 18 && str[i] != '$') i--;
        //        if (i == 18) continue;
        //        i--;
        //        data_Z = 0;
        //        for (int j = 4; j > -1; j--)
        //        {
        //            data_Z *= 10;
        //            data_Z += (str[i-j] - '0');
        //        }
        //        if (str[i -5] == '-') data_Z = -data_Z;
        //        i -= 6;

        //        data_Y = 0;
        //        for (int j = 4; j > -1; j--)
        //        {
        //            data_Y *= 10;
        //            data_Y += (str[i - j] - '0');
        //        }
        //        if (str[i - 5] == '-') data_Y = -data_Y;
        //        i -= 6;

        //        data_X = 0;
        //        for (int j = 4; j > -1; j--)
        //        {
        //            data_X *= 10;
        //            data_X += (str[i - j] - '0');
        //        }
        //        if (str[i - 5] == '-') data_X = -data_X;

        //        X = data_X;
        //        Y = data_Y;
        //        Z = data_Z;
        //    }
        //}
        public byte[] str = new byte[40];
        int p_read_from;
        int i = 0;
        int data_X, data_Y, data_Z;//
        /// <summary>
        /// 询问当前值
        /// </summary>
        /// <returns>如果解析到则返回真</returns>
        public bool ask()
        {
            i = 0;//立刻将相应的40个字符复制出来
            p_read_from = p_write - 40;
            while (i < 40)
            {
                str[i] = pool[(p_read_from + pool_size) % pool_size];
                i++;
                p_read_from++;
            }
            i = 39;
            while (i > 18 && str[i] != '$') i--;
            if (i == 18) return false;
            i--;
            data_Z = 0;
            for (int j = 4; j > -1; j--)
            {
                data_Z *= 10;
                data_Z += (str[i - j] - '0');
            }
            if (str[i - 5] == '-') data_Z = -data_Z;
            i -= 6;

            data_Y = 0;
            for (int j = 4; j > -1; j--)
            {
                data_Y *= 10;
                data_Y += (str[i - j] - '0');
            }
            if (str[i - 5] == '-') data_Y = -data_Y;
            i -= 6;

            data_X = 0;
            for (int j = 4; j > -1; j--)
            {
                data_X *= 10;
                data_X += (str[i - j] - '0');
            }
            if (str[i - 5] == '-') data_X = -data_X;

            X = data_X;
            Y = data_Y;
            Z = data_Z;
            return true;
        }

        /// <summary>
        /// 将数据输入数据池
        /// </summary>
        /// <param name="date">数据</param>
        /// <param name="length">长度</param>
        internal void push_back(byte[] date, int length)
        {
            for (int i = 0; i < length; i++)
            {
                pool[p_write++] = date[i];
                if (p_write == pool_size) p_write = 0;
            }
        }
    }
}
