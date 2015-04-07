//-----------------------------------------
// MPU6050 IIC测试程序
// 使用单片机STC89C51 
// 晶振：11.0592M
//-----------------------------------------
#include <REG52.H>	
#include <math.h>    //Keil library  
#include <stdio.h>   //Keil library	
typedef unsigned char  uchar;
typedef unsigned short ushort;
typedef unsigned int   uint;

//-----------------------------------------
//定义类型及变量
//-----------------------------------------
uchar dis[6];//存放整数转为字符串的字符串

//-----------------------------------------
//外部函数:串口和MPU6050
//-----------------------------------------
extern void InitMPU6050();
extern int GetData(uchar REG_Address);
extern void init_uart();	
extern void  SeriPushSend(uchar send_data);
//-----------------------------------------
//内部函数:延时、整数转字符串、发送到串口
//-----------------------------------------
void delay(unsigned int k);
void my_printf(uchar *s,int temp_data);
void SendData(int value);

//-----------------------------------------
//延时
//-----------------------------------------
void delay(unsigned int k)	
{						
	unsigned int i,j;				
	for(i=0;i<k;i++)
	{			
		for(j=0;j<121;j++);
	}						
}
//-----------------------------------------
//整数转字符串
//-----------------------------------------
void my_printf(uchar *s,int temp_data)
{
	if(temp_data<0)
	{
		temp_data=-temp_data;
		*s='-';
	}
	else *s=' ';
	*++s =temp_data/10000+0x30;
	temp_data=temp_data%10000;     //取余运算
	*++s =temp_data/1000+0x30;
	temp_data=temp_data%1000;     //取余运算
	*++s =temp_data/100+0x30;
	temp_data=temp_data%100;     //取余运算
	*++s =temp_data/10+0x30;
	temp_data=temp_data%10;      //取余运算
	*++s =temp_data+0x30; 	
}
//-----------------------------------------
//编码+发送到串口
//-----------------------------------------
void SendData(int value)
{ 
	uchar i;
	my_printf(dis, value);			//转换数据显示
	for(i=0;i<6;i++)
	{
    	SeriPushSend(dis[i]);
    }
}

//-----------------------------------------
//主程序
//-----------------------------------------
void main()
{ 
	delay(500);		//上电延时		
	init_uart();
	InitMPU6050();	//初始化MPU6050
	delay(150);
	while(1)
	{
		SeriPushSend('#');//换行，回车
		SendData(GetData(0x3B));	//X轴加速度
		SendData(GetData(0x3D));	//Y轴加速度
		SendData(GetData(0x3F));	//Z轴加速度
		SeriPushSend('$'); //结束
		delay(20);
	}
}