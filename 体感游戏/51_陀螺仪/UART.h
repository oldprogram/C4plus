#include <REG52.H>


typedef unsigned char  uchar;
typedef unsigned short ushort;
typedef unsigned int   uint;

void init_uart();						//串口初始化函数
void SeriPushSend(uchar send_data);	//串口发送函数
