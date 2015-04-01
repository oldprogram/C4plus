//****************************************
// MPU6050 IIC测试程序
// 使用单片机STC89C51 
// 晶振：11.0592M
//****************************************
#include <REG52.H>	
#include <math.h>    //Keil library  
#include <stdio.h>   //Keil library	
typedef unsigned char  uchar;
typedef unsigned short ushort;
typedef unsigned int   uint;

//****************************************
//定义类型及变量
//****************************************
uchar dis[6];							//显示数字(-511至512)的字符数组
int	dis_data;						//变量
//****************************************
//函数声明
//****************************************
void  delay(unsigned int k);										//延时						
void  lcd_printf(uchar *s,int temp_data);
int GetData(uchar REG_Address);
void InitMPU6050();

//****************************************
//整数转字符串
//****************************************
void lcd_printf(uchar *s,int temp_data)
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
//****************************************

void  SeriPushSend(uchar send_data)
{
    SBUF=send_data;  
	while(!TI);TI=0;	  
}
//****************************************
//延时
//****************************************
void delay(unsigned int k)	
{						
	unsigned int i,j;				
	for(i=0;i<k;i++)
	{			
		for(j=0;j<121;j++);
	}						
}



//**************************************
//编码+发送到串口
//**************************************
void SendData(int value)
{ 
	uchar i;
	lcd_printf(dis, value);			//转换数据显示
	for(i=0;i<6;i++)
	{
    SeriPushSend(dis[i]);
    }
}


void init_uart()
{
	TMOD=0x21;				
	TH1=0xfd;				
	TL1=0xfd;		
		
	SCON=0x50;
	PS=1;      //串口中断设为高优先级别
	TR0=1;	   //启动定时器			
	TR1=1;
	ET0=1;     //打开定时器0中断			
	ES=1;	
	EA=1;
}

//*********************************************************
//主程序
//*********************************************************
void main()
{ 
//int X,Y,Z;
	delay(500);		//上电延时		
	init_uart();
	InitMPU6050();	//初始化MPU6050
	delay(150);
	while(1)
	{
		SeriPushSend('#');//换行，回车
//		X=GetData(0x3B)/100;
//		Y=GetData(0x3D)/100;
//		Z=GetData(0x3F)/100;
//		SendData(X*X+Y*Y+Z*Z);
		SendData(GetData(0x3B));	//X轴加速度
		SendData(GetData(0x3D));	//Y轴加速度
		SendData(GetData(0x3F));	//Z轴加速度
//		Display10BitData(GetData(GYRO_XOUT_H),2,1);	//显示X轴角速度
//		Display10BitData(GetData(GYRO_YOUT_H),7,1);	//显示Y轴角速度
//		Display10BitData(GetData(GYRO_ZOUT_H),12,1);	//显示Z轴角速度
		SeriPushSend('$'); //结束
		delay(20);
	}
}