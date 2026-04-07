using GuidanceStsbilityCommsDownLoad;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GuidanceStsbilityCommsDownLoad.DataFrame;
using static MultiPort.CommandHeader;

namespace MultiPort
{
    public class DataPacketGSC
    {// 指令:起始符1，功能码1，内容N，CRC校验1
        protected byte[] databytes;// 传入的数据包
        protected int datalength;//设置数据长度
        public int Datalength// 数据长度
        {
            get { return databytes.Length; }
        }
        public byte Startbyte// 起始符,0x60(receive)或0x66(send)
        {
            get { return databytes[0]; }
            set
            {
                databytes[0] = value;
                Checksum();
            }
        }
        public bool IsGravityToolface// 从左到右第2位是1则为重力工具面，否则为磁力工具面
        {
            get
            {
                return (databytes[1] & 0x40) == 0x40;
            }
            set
            {
                if (value)
                {
                    databytes[1] |= 0x40;
                }
                else
                {
                    databytes[1] &= 0xBF;
                }
            }
        }// 重力or磁力工具面
        public byte Funcbyte// 功能码，1
        {
            get
            {
                if (isValidData() == false) return 0x00;
                return (byte)(databytes[1] & 0xBF);// 不返回工具面
            }
            set
            {
                databytes[1] &= 0xC0;// 清除原始工具面
                databytes[1] |= value;
                Checksum();
            }
        }
        public byte[] Command
        {
            get
            {
                if (!isValidData() || datalength <= 3) return null;

                byte[] data = new byte[datalength - 2];
                Array.Copy(databytes, 2, data, 0, datalength - 2);
                return data;
            }
            set
            {
                if (value == null || value.Length != datalength - 2) return;
                Array.Copy(value, 0, databytes, 2, value.Length);
                Checksum();
            }
        }

        /// <summary>
        /// check dataPacket length against payload
        /// </summary>
        /// <returns></returns>
        public bool isValidData()
        {
            if (databytes == null ||
                databytes.Length != datalength) return false;

            return true;
        }
        /// <summary>
        /// 校验位,默认占用最后一个字节
        /// </summary>
        /// <returns></returns>
        protected virtual byte Checksum()
        {
            byte checksum = ComputeChecksum();
            return checksum;
        }
        // 计算和校验，取低八位
        private byte ComputeChecksum()
        {
            int sum = 0;
            for (int i = 0; i < datalength - 1; i++)
            {
                sum += databytes[i];
            }

            return (byte)(sum & 0xFF);
        }
        public DataPacketGSC(int datalength)
        {
            this.datalength = datalength;
            databytes = new byte[datalength];
        }
        public DataPacketGSC(bool isGravityToolface, byte funcbyte, byte startbyte = 0x60, int datalength = 3)
        {
            this.datalength = datalength;
            databytes = new byte[datalength];
            IsGravityToolface = isGravityToolface;
            Funcbyte = funcbyte;
            Startbyte = startbyte;
        }
        public DataPacketGSC(byte[] databytesIn)
        {
            this.datalength = databytesIn.Length;

            if (databytesIn == null || databytesIn.Length != datalength)
                return;
            else
            {
                databytes = new byte[datalength];
                Array.Copy(databytesIn, databytes, datalength);
            }
        }

        public string HexByteToString()
        {
            string res = "";
            foreach (byte b in databytes)
            {
                res += b.ToString("X2");
            }
            return res;
        }
        public byte[] ToBytes()
        {
            return databytes;
        }
        public String ToHexString()
        {
            String hex = String.Empty;

            hex = Converter.BytesToHexString(databytes);
            hex = Converter.AddSpacesToHexString(hex);

            return hex;
        }
    }


    public class DataPacketSend : DataPacketGSC
    {
        public DataPacketSend(bool isGravityToolface, byte funcbyte, byte startbyte = 0x60, int datalength = 3) : base(isGravityToolface, funcbyte, startbyte, datalength)
        {
        }

        protected override byte Checksum()
        {
            byte sum = base.Checksum();
            databytes[datalength - 1] = sum;
            return sum;
        }
    }

    public class DataPacketReceived : DataPacketGSC
    {
        public DataPacketReceived(byte[] databytesIn) : base(databytesIn)
        {
        }

        // 校验和：起始符至数据域最低位之和的低4位
        // 高4位不变
        protected override byte Checksum()
        {
            byte highbyte = databytes[datalength - 1];
            byte lowbyte = (byte)(base.Checksum() + (databytes[datalength - 1] >> 4));// 7个字节的高4位作为低4位加进去
            byte res = (byte)((highbyte & 0xF0) | (lowbyte & 0x0F));

            // 修改databyte
            databytes[databytes.Length - 1] = (byte)res;
            return res;
        }
    }

    public class DataPacketDownloadlist
    {
        // 8 - 65535 bytes
        // DstAddr  SrcAddr PayLoad  Command Data       CRC
        // 1 byte   1 byte  2 bytes  2-65529 bytes      2 bytes
        private const int minNumBytes = 8;
        private const int maxNumBytes = 65535;
        private byte[] dataPacket;
        public byte DstAddr
        {
            get
            {
                byte b = 0x00;
                if (isValidLength() == false) return b;
                return dataPacket[0];
            }
            set
            {
                if (isValidLength() == false) defaultDataPacket(minNumBytes);
                dataPacket[0] = value;
                checkSum();
            }
        }
        public byte SrcAddr
        {
            get
            {
                byte b = 0x00;
                if (isValidLength() == false) return b;
                return dataPacket[1];
            }
            set
            {
                if (isValidLength() == false) defaultDataPacket(minNumBytes);
                dataPacket[1] = value;
                checkSum();
            }
        }
        public ToolCommand Command
        {
            get
            {
                if (isValidLength() == false) return new ToolCommand();
                int payLoad = (int)PayLoad;
                byte[] headerBytes = new byte[2];
                headerBytes[0] = dataPacket[4];
                headerBytes[1] = dataPacket[5];
                CommandHeader header = new CommandHeader(headerBytes);
                byte[] data = null;
                if (payLoad > minNumBytes)
                {
                    data = new byte[payLoad - minNumBytes];
                    Array.Copy(dataPacket, 6, data, 0, payLoad - minNumBytes);
                }
                ToolCommand cmd = new ToolCommand(header, data);
                return cmd;
            }
            set
            {
                if (value == null) return;
                CommandHeader header = value.Header;
                byte[] data = value.Data;
                if (header == null) return;
                byte[] headerBytes = header.ToBytes();
                if (headerBytes == null || headerBytes.Length != 2) return;
                int dataLength = (data == null) ? 0 : data.Length;

                if (dataPacket == null || dataPacket.Length < minNumBytes + dataLength)
                {
                    dataPacket = new byte[8 + dataLength];
                }
                Array.Copy(headerBytes, 0, dataPacket, 4, 2);
                if (dataLength > 0) Array.Copy(data, 0, dataPacket, 6, dataLength);
            }
        }
        public DataPacketDownloadlist(byte[] dataPacketIn)
        {
            if (dataPacketIn == null || dataPacketIn.Length == 0)
            {
                dataPacket = new byte[minNumBytes];
                PayLoad = minNumBytes;
            }
            else
            {
                dataPacket = new byte[dataPacketIn.Length];
                Array.Copy(dataPacketIn, dataPacket, dataPacketIn.Length);
            }
        }
        public DataPacketDownloadlist(byte dst, byte src, int len, ToolCommand cmd)
        {
            // set the length first, otherwise dataPacket will messed up
            // 
            PayLoad = len;
            Command = cmd;
            DstAddr = dst;
            SrcAddr = src;
        }
        /// <summary>
        /// check to see if the received dataPacket is the response to the sent dataPacket.
        /// The result does not depend on which is request and which is response dataPacket.
        /// </summary>
        /// <param name="sent">sent dataPacket</param>
        /// <param name="received">received dataPacket</param>
        /// <returns></returns>
        public static bool VerifyDataPacket(DataPacketDownloadlist sent, DataPacketDownloadlist received)
        {
            if (sent == null || received == null) return false;

            //if (sent.SrcAddr != received.DstAddr ||
            //    sent.DstAddr != received.SrcAddr) return false;

            // don't check send.SrcAddr, because it has been changed when the packet was sent
            //if (sent.DstAddr != received.SrcAddr) return false;

            if (sent.Command.Header.IsResponse == received.Command.Header.IsResponse ||
                sent.Command.Header.CmdObject != received.Command.Header.CmdObject ||
                sent.Command.Header.Parameter != received.Command.Header.Parameter) return false;

            return true;

        }
        public int PayLoad
        {
            get
            {
                int b = minNumBytes;
                if (isValidLength() == false) return b;
                return dataPacket[2] + dataPacket[3] * 256;
            }
            set
            {
                if (isValidLength() == false) defaultDataPacket(value);
                dataPacket[2] = Convert.ToByte(value % 256);
                dataPacket[3] = Convert.ToByte(value / 256);
                checkSum();
            }
        }
        /// <summary>
        /// check dataPacket length against payload
        /// </summary>
        /// <returns></returns>
        private bool isValidLength()
        {
            if (dataPacket == null ||
                dataPacket.Length < minNumBytes ||
                dataPacket.Length > maxNumBytes) return false;
            int payLoad = dataPacket[2] + dataPacket[3] * 256;
            if (dataPacket.Length != payLoad) return false;

            return true;
        }
        public static DataPacketDownloadlist GenerateDataPacket(byte dst, CommandHeader.EnumCmdObject cmdObject, int param, byte[] data)
        {
            //byte src = ToolBoardAddress.GetToolBoardAddress(EnumToolAddress.Surface, EnumBoardAddress.Computer);    // (0x0E) Surface Computer
            byte src = 0x3e;    // (0x0E) Surface Computer

            int len = 8 + ((data == null) ? 0 : data.Length);
            int ret = 0;
            CommandHeader header = new CommandHeader(false, cmdObject, param, ret);
            ToolCommand toolCmd = new ToolCommand(header, data);
            DataPacketDownloadlist dataPacket = new DataPacketDownloadlist(dst, src, len, toolCmd);

            return dataPacket;
        }
       /* /// <summary>
        /// update check sum, and returns it. 校验位，最后两字节，格式：0xxx（低八位） 0xxx
        /// </summary>
        /// <returns>check sum as a 2 bytes array</returns>
        private byte[] checkSum()
        {
            if (isValidLength() == false) return null;

            int len = (int)PayLoad;
            byte[] data = new byte[len - 2];// 前面所有位数
            byte[] checkSum = new byte[2];// 校验位
            Array.Copy(dataPacket, 0, data, 0, len - 2);
            Array.Copy(dataPacket, len - 2, checkSum, 0, 2);

            //ushort crc = (ushort)ComputeChecksum(data, data.Length, 0);
            ushort checksum = (ushort)ComputeChecksum2(data, data.Length);
            byte[] checkSumCalc = BitConverter.GetBytes(checksum);
            Array.Copy(checkSumCalc, 0, dataPacket, len - 2, 2);
            return checkSumCalc;
        }*/
        /// <summary>
        /// update check sum, and returns it
        /// </summary>
        /// <returns>check sum as a 2 bytes array</returns>
        private byte[] checkSum()
        {
            if (isValidLength() == false) return null;

            int len = (int)PayLoad;
            byte[] data = new byte[len - 2];
            byte[] checkSum = new byte[2];
            Array.Copy(dataPacket, 0, data, 0, len - 2);
            Array.Copy(dataPacket, len - 2, checkSum, 0, 2);

            Crc16 crc16 = new Crc16();
            byte[] checkSumCalc = crc16.ComputeChecksumBytes(data);
            Array.Copy(checkSumCalc, 0, dataPacket, len - 2, 2);
            return checkSumCalc;
        }
        // 字节和校验
        public static int ComputeChecksum2(byte[] pData, int byLen)
        {
            ushort sum = 0; // 初始和为0
            for (int i = 0; i < byLen; i++)
            {
                sum += pData[i]; // 逐字节累加（自动取模 0xFFFF）
            }
            return sum;
        }
        private void defaultDataPacket(int length)
        {
            if (length < minNumBytes || length > maxNumBytes) length = minNumBytes;
            dataPacket = new byte[length];
            dataPacket[2] = Convert.ToByte(length % 256);
            dataPacket[3] = Convert.ToByte(length / 256);
        }
        public byte[] ToBytes()
        {
            return dataPacket;
        }
    }

    public class ToolCommand
    {
        // 2 - 65529 bytes
        // Header   Data
        // 2 bytes  0-65527 bytes
        private byte[] command;
        public CommandHeader Header
        {
            get
            {
                if (command == null || command.Length < 2) return new CommandHeader();
                return new CommandHeader(command);
            }
            set
            {
                byte[] b = value.ToBytes();
                if (b == null || b.Length < 2) return;
                if (command == null || command.Length < 2) command = new byte[2];
                command[0] = b[0];
                command[1] = b[1];
            }
        }
        public byte[] Data
        {
            get
            {
                if (command == null || command.Length <= 2) return null;
                byte[] data = new byte[command.Length - 2];
                Array.Copy(command, 2, data, 0, command.Length - 2);
                return data;
            }
            set
            {
                if (value == null || value.Length == 0) return;
                if (command == null || command.Length < 2)
                {
                    command = new byte[2 + value.Length];
                    Array.Copy(value, 0, command, 2, value.Length);
                }
                else if (command.Length < 2 + value.Length)
                {
                    byte[] temp = new byte[2];
                    Array.Copy(command, temp, 2);
                    command = new byte[2 + value.Length];
                    Array.Copy(temp, command, 2);
                    Array.Copy(value, 0, command, 2, value.Length);
                }
                else
                {
                    Array.Copy(value, 0, command, 2, value.Length);
                }
            }
        }
        public ToolCommand()
        {
            command = new byte[2];
        }
        public ToolCommand(CommandHeader header, byte[] data)
        {
            if (header == null || data == null)
            {
                command = new byte[2];
                Header = header;
                return;
            }

            command = new byte[2 + data.Length];
            Header = header;
            Data = data;
        }
    }
    public class CommandHeader
    {
        /// <summary>
        /// Also make changes in the static functions if enum changes
        /// </summary>
        public enum EnumCmdObject
        {
            RS232 = 0x00,
            CAN = 0x01,
            USB = 0x02,
            FSKModem = 0x03,
            MudPulser = 0x04,
            BoardInfo = 0x05,
            RTClock = 0x06,
            Logging = 0x07,
            //LWDMaster   = 0x07,         // LWDMaster = Logging ??
            Diagnostic = 0x08,
            M30Data = 0x09,
            Bootloader = 0x0A,
            ToolSpecific = 0x0B,
            NearBitMemory = 0x09,         // NearBitMemory = ToolSpecific + 1
            DirectionalCalib = 0x0D,
            //DAP = 0x13,// 国家项目源地址
        }

        // 16 bits
        // Response Reserve Object  Parameter   Return
        // 1 bit    2 bits  5 bits  4 bits      4 bits
        private const int length = 16;
        private BitArray commandHeader;

        public bool IsResponse
        {
            get { return commandHeader.Get(0); }
            set { commandHeader.Set(0, value); }
        }
        public EnumCmdObject CmdObject
        {
            get { return (EnumCmdObject)ExBitArray.GetBitsValue(commandHeader, 3, 5); }
            set { ExBitArray.SetBitsValue(commandHeader, 3, 5, (int)value); }
        }
        public int Parameter
        {
            get { return ExBitArray.GetBitsValue(commandHeader, 8, 4); }
            set { ExBitArray.SetBitsValue(commandHeader, 8, 4, (int)value); }
        }
        public int Return
        {
            get { return ExBitArray.GetBitsValue(commandHeader, 12, 4); }
            set { ExBitArray.SetBitsValue(commandHeader, 12, 4, (int)value); }
        }
        public CommandHeader()
        {
            commandHeader = new BitArray(length);
            IsResponse = false;
            CmdObject = EnumCmdObject.BoardInfo;
        }
        public CommandHeader(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                commandHeader = new BitArray(length);
                return;
            }

            byte[] b = new byte[2];
            b[0] = bytes[0];
            if (bytes.Length >= 2) b[1] = bytes[1];

            // commandHeader = new BitArray(b); -- incorrect
            commandHeader = new BitArray(length);
            ExBitArray.SetBitsValue(commandHeader, 0, 8, b[0]);
            ExBitArray.SetBitsValue(commandHeader, 8, 8, b[1]);
        }
        public CommandHeader(bool isResponse, EnumCmdObject cmdObj,
            int param, int ret)
        {
            commandHeader = new BitArray(length);
            IsResponse = isResponse;
            CmdObject = cmdObj;
            Parameter = param;
            Return = ret;
        }
        public byte[] ToBytes()
        {
            if (commandHeader == null || commandHeader.Length < 16) return null;

            byte[] b = new byte[2];
            int value = ExBitArray.GetBitsValue(commandHeader, 0, 8);
            b[0] = (byte)value;
            value = ExBitArray.GetBitsValue(commandHeader, 8, 8);
            b[1] = (byte)value;

            return b;
        }
    }
}
