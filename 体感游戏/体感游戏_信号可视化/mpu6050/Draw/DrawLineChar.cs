using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicControl.Draw
{
    public class DrawLineChart
    {
        int x0, y0, x1, y1, div_y, div_x;//左上角和右下角，y方向分为几份，x方向分为几份
        double unit_x, unit_y;//x方向单位距离，y方向单位距离
        public DrawLineChart(int x0,int y0,int x1,int y1,int div_y,int div_x)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
            this.div_x = div_x;
            this.div_y = div_y;
            unit_x = (x1 - x0)*1.0 / div_x;
            unit_y = (y1 - y0)*1.0 / div_y;
        }
        //生成几种颜色的笔
        Pen r_pen = new Pen(Color.Red);
        Pen b_pen = new Pen(Color.Blue);
        Pen g_pen = new Pen(Color.Green);
        Pen black_pen = new Pen(Color.Black);
        Rectangle point_rect;
        Graphics g;
        public void DrawLineChar(object sender, PaintEventArgs e,List<int> Q)
        {
            if (Q.Count == 0) return;
            //绘图
            g = e.Graphics;
            //横纵坐标
            g.DrawLine(black_pen, new Point(x0, (y0 + y1) / 2), new Point(x1, (y0 + y1) / 2));
            g.DrawLine(black_pen, new Point(x0, y0), new Point(x0, y1));
            //绘制第一个点（圈）
            point_rect = new Rectangle(x0 - 2, (y1 + y0) / 2 - Q[0] - 2, 4, 4);
            g.DrawEllipse(g_pen, point_rect);
            //绘制后面的点,并把相邻的点绘制好
            for (int i = 1; i < Q.Count; i++)
            {
                point_rect = new Rectangle((int)(x0 + unit_x * i-2), (y1+y0)/2 - Q[i]-2, 4, 4);
                g.DrawEllipse(g_pen, point_rect);
                g.DrawLine(r_pen, new Point((int)(x0 + unit_x * (i - 1)), (y1 + y0) / 2 - Q[i - 1]), new Point((int)(x0 + unit_x * i), (y1 + y0) / 2 - Q[i]));
            }
            //删除第一个点
            if (Q.Count > div_x) 
                Q.RemoveAt(0);
        }

        public void Draw3LineChar(object sender, PaintEventArgs e, List<int> P,List<int> Q,List<int> R)
        {
            if (P.Count == 0) return;
            //绘图
            g = e.Graphics;
            //横纵坐标
            g.DrawLine(black_pen, new Point(x0, (y0 + y1) / 2), new Point(x1, (y0 + y1) / 2));
            g.DrawLine(black_pen, new Point(x0, y0), new Point(x0, y1));
            int half=(y1+y0)/2;
            for (int i = 1; i < half; i++)
            {
                g.DrawLine(black_pen, new Point(x0, (int)(half + unit_y * i)), new Point(x0 + 4, (int)(half + unit_y * i)));
                g.DrawString((-i * 260 * unit_y).ToString(), new Font("微软雅黑", 6), Brushes.Black, new Point(x0, (int)(half + unit_y * i)));
                g.DrawLine(black_pen, new Point(x0, (int)(half - unit_y * i)), new Point(x0 + 4, (int)(half - unit_y * i)));
                g.DrawString((i * 260 * unit_y).ToString(), new Font("微软雅黑", 6), Brushes.Black, new Point(x0, (int)(half - unit_y * i)));
            }
            //绘制第一个点（圈）
            point_rect = new Rectangle(x0 - 1, (y1 + y0) / 2 - P[0] - 1, 2, 2);
            g.DrawEllipse(r_pen, point_rect);
            //
            point_rect = new Rectangle(x0 - 1, (y1 + y0) / 2 - Q[0] - 1, 2, 2);
            g.DrawEllipse(g_pen, point_rect);
            //
            point_rect = new Rectangle(x0 - 1, (y1 + y0) / 2 - R[0] - 1, 2, 2);
            g.DrawEllipse(b_pen, point_rect);
            //绘制后面的点,并把相邻的点绘制好
            for (int i = 1; i < P.Count; i++)
            {
                point_rect = new Rectangle((int)(x0 + unit_x * i - 1), (y1 + y0) / 2 - P[i] - 1, 2, 2);
                g.DrawEllipse(r_pen, point_rect);
                g.DrawLine(b_pen, new Point((int)(x0 + unit_x * (i - 1)), (y1 + y0) / 2 - P[i - 1]), new Point((int)(x0 + unit_x * i), (y1 + y0) / 2 - P[i]));
                //
                point_rect = new Rectangle((int)(x0 + unit_x * i - 1), (y1 + y0) / 2 - Q[i] - 1, 2, 2);
                g.DrawEllipse(g_pen, point_rect);
                g.DrawLine(r_pen, new Point((int)(x0 + unit_x * (i - 1)), (y1 + y0) / 2 - Q[i - 1]), new Point((int)(x0 + unit_x * i), (y1 + y0) / 2 - Q[i]));
                //
                point_rect = new Rectangle((int)(x0 + unit_x * i - 1), (y1 + y0) / 2 - R[i] - 1, 2, 2);
                g.DrawEllipse(b_pen, point_rect);
                g.DrawLine(g_pen, new Point((int)(x0 + unit_x * (i - 1)), (y1 + y0) / 2 - R[i - 1]), new Point((int)(x0 + unit_x * i), (y1 + y0) / 2 - R[i]));
            }
            //删除第一个点
            if (P.Count > div_x)
            {
                P.RemoveAt(0);
                Q.RemoveAt(0);
                R.RemoveAt(0);
            }
        }
    }
}
