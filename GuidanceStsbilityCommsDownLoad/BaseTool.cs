using GuidanceStsbilityCommsDownLoad;
using MultiPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidanceStsbilityComms
{
    public class BaseTool
    {
        private int DataNumofBytes = 7;
        private byte Header = 0x66;
        private byte FuncByte;
        private readonly List<byte> ValidFuncReceived = new List<byte>() {
                0x0b,0x0c,0x0d,0x0e,0x0f,0x10,
                0x11,0x12,0x16
            };

        public bool IsGravityToolface// 工具面
        {
            get; set;
        }
        public string FuncName// 功能码对应名字
        {
            get
            {
                return DXCS[FuncByte];
            }
        }
        private Dictionary<byte, string> DXCS = new Dictionary<byte, string>()
        {
            {0x00,"静态井斜" },
            {0x01,"静态方位" },
            {0x02,"静态重力和" },
            {0x03,"静态磁场和" },
            {0x04,"重力工具面(parm)" },
            {0x05,"磁工具面(parm)" },
            {0x06,"静态井斜标准差" },
            {0x07,"静态方位标准差" },
            {0x08,"静态磁倾角" },
            {0x09,"静态测量期间平台平均转速" },
            {0x0A,"QMARK" },
            {0x0B,"X轴重力分量" },
            {0x0C,"Y轴重力分量" },
            {0x0D,"Z轴重力分量" },
            {0x0E,"X轴磁场分量" },
            {0x0F,"Y轴磁场分量" },
            {0x10,"Z轴磁场分量" },
            {0x11,"动态井斜" },
            {0x12,"动态方位" },
            {0x13,"动态井斜标准差" },
            {0x14,"动态方位标准差" },
            {0x15,"动态磁倾角" },
            {0x16,"磁通门转速" },
            {0x17,"轴向振动" },
            {0x18,"横向振动" },
            {0x19,"切向振动" },
            {0x1A,"测量模块温度" },
            {0x1B,"3.3V电源" },
            {0x1C,"5V电源" },
            {0x1D,"15/12V电源" },
            {0x1E,"不同温度里程持续时间计数1" },
            {0x1F,"测量固件版本2" },
            {0x20,"测量电路版本2" },
        };

        // 参数
        // 工具面,0-3600,数据范围0-360°,0.1
        private double toolface;
        public double Toolface
        {
            get { return toolface; }
            set { toolface = value / 10.0; }
        }
        // 陀螺转速,0-4000,数据范围-300~1700°,0.5
        private double gyrospeed;
        public double Gyrospeed
        {
            get { return gyrospeed; }
            set { gyrospeed = value / 2.0 - 300; }
        }
        // 定向参数,多种
        private double parameter;
        public double Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }

        public bool ProcessDataPacket(DataPacketReceived data)
        {
            if (data.Datalength != DataNumofBytes) return false;
            if (data.Startbyte != Header) return false;
            IsGravityToolface = data.IsGravityToolface;
            FuncByte = data.Funcbyte;
            //if (!ValidFuncReceived.Contains(FuncByte)) return false;// 只接收部分参数

            byte[] cmdData = data.Command;
            int toolfaceraw = (cmdData[0] << 4) | (cmdData[1] >> 4);
            Toolface = toolfaceraw;
            int gyrospeed = ((cmdData[1] & 0x0F) << 8) | cmdData[2];
            Gyrospeed = gyrospeed;
            int parameter = (cmdData[3] << 4) | (cmdData[4] >> 4);
            if (FuncByte == 0x00 || FuncByte == 0x08 || FuncByte == 0x11)
            {
                Parameter = parameter * 0.05;
            }
            else if (FuncByte == 0x01)
            {
                Parameter = parameter * 0.1;
            }
            else if (FuncByte == 0x03) Parameter = parameter * 0.025;
            else if (FuncByte == 0x04 || FuncByte == 0x05) Parameter = parameter * 1.5;
            else if (FuncByte == 0x02 || FuncByte == 0x06 || FuncByte == 0x13 || FuncByte == 0x14) Parameter = parameter * 0.001;
            else if (FuncByte == 0x07 || FuncByte == 0x1C || FuncByte == 0x1D) Parameter = parameter * 0.01;
            else if (FuncByte == 0x09) Parameter = parameter * 0.2;
            else if (FuncByte >= 0x0b && FuncByte <= 0x0d)
            {
                Parameter = parameter * 0.001 - 1100;
            }
            else if (FuncByte >= 0x0e && FuncByte <= 0x10)
            {
                Parameter = (parameter * 30 - 60000);
            }
            else if (FuncByte == 0x12)
            {
                Parameter = parameter * 0.1;
            }
            else if (FuncByte == 0x15) Parameter = parameter * 0.05 - 90;
            else if (FuncByte == 0x16)
            {
                Parameter = parameter * 1.2 - 300;
            }
            else if (FuncByte >= 0x17 && FuncByte <= 0x19) Parameter = parameter * 0.5;
            else if (FuncByte == 0x1A) Parameter = parameter - 50;
            else if (FuncByte == 0x1B) Parameter = parameter * 0.015;
            else
            {// 暂时这样
                Parameter = parameter;
            }

            return true;
        }

        public Dictionary<string, double> valuePairs => new Dictionary<string, double>()
        {
            { $"{(IsGravityToolface ? "重力":"磁")}工具面", Toolface },
            { "陀螺转速", Gyrospeed },
            { FuncName, Parameter }
        };
        public override string ToString()
        {
            return String.Concat(String.Join(",", valuePairs.Keys), ",", String.Join(",", valuePairs.Values));
        }
    }

    [Serializable]
    public struct BoardInfo
    {
        public EnumBoardAddress BoardAddress;
        public String BoardName;
        public String Address;
        public String PCBVersion;
        public String SerialNumber;
        public String Firmware;

        // length = 22
        //public BoardInfo(byte[] dataBytes)
        //{
        //    ToolBoardAddress address = new ToolBoardAddress(dataBytes[0]);
        //    BoardAddress = address.BoardAddress;
        //    BoardName = address.BoardAddressName;

        //    // get board info from dataBoardInfo
        //    String boardInfo = BoardInfoObject.GetBoardInfo(BoardInfoObject.BWEnumBoardInfoParameter.BoardParameter, dataBytes);
        //    String[] bis = boardInfo.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (bis.Length >= 4)
        //    {
        //        Address = bis[0];
        //        PCBVersion = bis[1];
        //        SerialNumber = bis[2];
        //        Firmware = bis[3];
        //    }
        //    else
        //    {
        //        Address = String.Empty;
        //        PCBVersion = String.Empty;
        //        SerialNumber = String.Empty;
        //        Firmware = String.Empty;
        //    }
        //}
    }
}
