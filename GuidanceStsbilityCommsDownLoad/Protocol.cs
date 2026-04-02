using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static GuidanceStsbilityCommsDownLoad.DataFrame;
using static System.Resources.ResXFileRef;

namespace GuidanceStsbilityCommsDownLoad
{
    public class Protocol
    {
    }
    public class DataFrame
    {
        public enum EnumPriority
        {
            Highest = 0x00,
            High = 0x01,
            Middle = 0x02,
            Low = 0x03,
        }
        // 分包位置
        public enum EnumFramePosition
        {
            Start = 0x00,
            Middle = 0x01,
            End = 0x02,
            Single = 0x03,
        }
        // 11 bits
        // priority dstAddr     srcAddr framePos    M30
        // 2        3           3       2           1
        private const int numberOfBits = 11;
        private BitArray dataFrameBitArray;

        public EnumPriority Priority
        {
            get
            {
                return (EnumPriority)ExBitArray.GetBitsValue(dataFrameBitArray, 0, 2);
            }
            set
            {
                ExBitArray.SetBitsValue(dataFrameBitArray, 0, 2, (int)value);
            }
        }

        public int DstAddr
        {
            get
            {
                return ExBitArray.GetBitsValue(dataFrameBitArray, 2, 3);
            }

            set
            {
                ExBitArray.SetBitsValue(dataFrameBitArray, 2, 3, (int)value);
            }
        }
        public int SrcAddr
        {
            get
            {
                return ExBitArray.GetBitsValue(dataFrameBitArray, 5, 3);
            }

            set
            {
                ExBitArray.SetBitsValue(dataFrameBitArray, 5, 3, (int)value);
            }
        }
        public EnumFramePosition FramePos
        {
            get
            {
                return (EnumFramePosition)ExBitArray.GetBitsValue(dataFrameBitArray, 8, 2);
            }

            set
            {
                ExBitArray.SetBitsValue(dataFrameBitArray, 8, 2, (int)value);
            }
        }
        public int M30
        {
            get
            {
                return ExBitArray.GetBitsValue(dataFrameBitArray, 10, 1);
            }

            set
            {
                ExBitArray.SetBitsValue(dataFrameBitArray, 10, 1, (int)value);
            }
        }
        public DataFrame()
        {
            dataFrameBitArray = new BitArray(numberOfBits);
        }
        public DataFrame(int frame)
        {
            dataFrameBitArray = new BitArray(numberOfBits);
            ExBitArray.SetBitsValue(dataFrameBitArray, 0, numberOfBits, frame);
        }
        public static class ExBitArray
        {
            // 从array[start]开始对count个bit进行提取
            public static int GetBitsValue(BitArray bitArray, int start, int count)
            {
                if (bitArray == null || bitArray.Count == 0) return 0;
                if (start < 0 || start + count > bitArray.Count) return 0;
                if (count <= 0 || count > 32) return 0;

                int value = 0;
                int multiplier = 1;
                for (int i = start + count - 1; i >= start; i--)
                {
                    if (bitArray.Get(i) == true)
                    {
                        value += multiplier;
                    }
                    multiplier *= 2;
                }

                return value;
            }

            public static void SetBitsValue(BitArray bitArray, int start, int count, int value)
            {
                if (bitArray == null || bitArray.Count == 0) return;
                if (start < 0 || start + count > bitArray.Count) return;
                if (count <= 0 || count > 32) return;

                int multiplier = 1;
                for (int i = start + count - 1; i >= start; i--)
                {
                    bitArray.Set(i, (value & multiplier) == multiplier);
                    multiplier *= 2;
                }
            }
        }
        public int ToInt()
        {
            if (dataFrameBitArray == null || dataFrameBitArray.Length != numberOfBits) return 0;

            return ExBitArray.GetBitsValue(dataFrameBitArray, 0, numberOfBits);
        }
    }
    public class ToolBoardAddress
    {
        private const int numberOfBits = 8;
        private BitArray address;

        public ToolBoardAddress(byte b)
        {
            address = new BitArray(numberOfBits);
            ExBitArray.SetBitsValue(address, 0, 8, b);
        }
        public EnumToolAddress ToolAddress
        {
            get
            {
                int addr = ExBitArray.GetBitsValue(address, 0, 5);
                return (EnumToolAddress)addr;
            }

            set
            {
                ExBitArray.SetBitsValue(address, 0, 5, (int)value);
            }
        }
        public EnumBoardAddress BoardAddress
        {
            get
            {
                int addr = ExBitArray.GetBitsValue(address, 0, 8);
                switch (addr)
                {
                    case 0x00: return EnumBoardAddress.NoBoard;
                    case 0x08: return EnumBoardAddress.NoBoard;
                    case 0x09: return EnumBoardAddress.AllBoards;
                    case 0x0A: return EnumBoardAddress.DrillersDisplay;
                    case 0x0B: return EnumBoardAddress.SurfaceBox;
                    case 0x0D: return EnumBoardAddress.USBDongle;
                    case 0x0E: return EnumBoardAddress.Computer;
                    case 0x0F: return EnumBoardAddress.FSKModem;
                    case 0x10: return EnumBoardAddress.NoBoard;
                    case 0x11: return EnumBoardAddress.Memory1;
                    case 0x12: return EnumBoardAddress.MWDBatteryControl;
                    case 0x13: return EnumBoardAddress.MWD;
                    case 0x14: return EnumBoardAddress.LWDMaster;
                    case 0x17: return EnumBoardAddress.FSKModem;
                    case 0x18: return EnumBoardAddress.NoBoard;
                    case 0x19: return EnumBoardAddress.DataProcess;
                    case 0x1A: return EnumBoardAddress.SignalGenerator;
                    case 0x1B: return EnumBoardAddress.Memory2;
                    case 0x1F: return EnumBoardAddress.FSKModem;
                    case 0x20: return EnumBoardAddress.NoBoard;
                    case 0x21: return EnumBoardAddress.Gamma;
                    case 0x22: return EnumBoardAddress.RSSConvert;
                    case 0x27: return EnumBoardAddress.FSKModem;
                    case 0x28: return EnumBoardAddress.NoBoard;
                    case 0x29: return EnumBoardAddress.DataProcess;
                    case 0x2A: return EnumBoardAddress.SignalGenerator;
                    case 0x2B: return EnumBoardAddress.Memory2;
                    case 0x2F: return EnumBoardAddress.FSKModem;
                    case 0x30: return EnumBoardAddress.NoBoard;
                    case 0x31: return EnumBoardAddress.DNSonicDAQ;
                    case 0x32: return EnumBoardAddress.DNNeutronDAQ;
                    case 0x33: return EnumBoardAddress.DNDensityDAQ;
                    case 0x37: return EnumBoardAddress.FSKModem;
                    case 0x38: return EnumBoardAddress.NoBoard;
                    case 0x39: return EnumBoardAddress.DataProcess;
                    case 0x3A: return EnumBoardAddress.NBBatteryControl;
                    case 0x3B: return EnumBoardAddress.NBAzimuthalGamma;
                    case 0x3C: return EnumBoardAddress.NBShorthopTX;
                    case 0x3D: return EnumBoardAddress.NBShorthopRX;
                    case 0x3F: return EnumBoardAddress.FSKModem;
                    case 0x40: return EnumBoardAddress.NoBoard;
                    case 0x41: return EnumBoardAddress.RSControl;
                    case 0x42: return EnumBoardAddress.RSDownLink;
                    case 0x43: return EnumBoardAddress.RSShorthop;
                    case 0x44: return EnumBoardAddress.RSDirectional;
                    case 0x45: return EnumBoardAddress.RSMotorCtrl;
                    case 0x47: return EnumBoardAddress.FSKModem;
                    case 0x48: return EnumBoardAddress.NoBoard;
                    case 0x49: return EnumBoardAddress.PressVib;
                    case 0x4F: return EnumBoardAddress.FSKModem;
                }
                return EnumBoardAddress.NoBoard;
            }

            set
            {
                ExBitArray.SetBitsValue(address, 5, 3, (int)value);
            }
        }
        public String BoardAddressName
        {
            get
            {
                int addr = ExBitArray.GetBitsValue(address, 0, 8);
                switch (addr)
                {
                    case 0x00: return "NoBoard";
                    case 0x08: return "NoBoard";
                    case 0x09: return "AllBoards";
                    case 0x0A: return "DrillersDisplay";
                    case 0x0B: return "SurfaceBox";
                    case 0x0D: return "USBDongle";
                    case 0x0E: return "Computer";
                    case 0x0F: return "FSKModem";
                    case 0x10: return "NoBoard";
                    case 0x11: return "Memory1";
                    case 0x12: return "BatteryControl";
                    case 0x13: return "MWD";
                    case 0x14: return "LWDMaster";
                    case 0x17: return "FSKModem";
                    case 0x18: return "NoBoard";
                    case 0x19: return "DataProcess";
                    case 0x1A: return "SignalGenerator";
                    case 0x1B: return "Memory2";
                    case 0x1F: return "FSKModem";
                    case 0x20: return "NoBoard";
                    case 0x21: return "Gamma";
                    case 0x22: return "RSSConvert";
                    case 0x27: return "FSKModem";
                    case 0x28: return "NoBoard";
                    case 0x29: return "DataProcess";
                    case 0x2A: return "SignalGenerator";
                    case 0x2B: return "Memory2";
                    case 0x2F: return "FSKModem";
                    case 0x30: return "NoBoard";
                    case 0x31: return "DNSonicDAQ";
                    case 0x32: return "DNNeutronDAQ";
                    case 0x33: return "DNDensityDAQ";
                    case 0x37: return "FSKModem";
                    case 0x38: return "NoBoard";
                    case 0x39: return "DataProcess";
                    case 0x3A: return "BatteryControl";
                    case 0x3B: return "AziGamma";
                    case 0x3C: return "ShorthopTX";
                    case 0x3D: return "ShorthopRX";
                    case 0x3F: return "FSKModem";
                    case 0x40: return "NoBoard";
                    case 0x41: return "Control";
                    case 0x42: return "RSDownLink";
                    case 0x43: return "RSShorthop";
                    case 0x44: return "RSDirectional";
                    case 0x45: return "RSMotorCtrl";
                    case 0x47: return "FSKModem";
                    case 0x48: return "NoBoard";
                    case 0x49: return "PressVib";
                    case 0x4F: return "FSKModem";
                }
                return "NoBoard";
            }
        }
        public byte Address
        {
            get
            {
                int addr = ExBitArray.GetBitsValue(address, 0, 8);
                return (byte)addr;
            }

            set
            {
                ExBitArray.SetBitsValue(address, 0, 8, (int)value);
            }
        }
        public ToolBoardAddress(EnumToolAddress toolAddr, EnumBoardAddress boardAddr)
        {
            address = new BitArray(numberOfBits);
            ToolAddress = toolAddr;
            BoardAddress = boardAddr;
        }
        public static byte GetToolBoardAddress(EnumToolAddress toolAddr, EnumBoardAddress boardAddr)
        {
            ToolBoardAddress a = new ToolBoardAddress(toolAddr, boardAddr);
            return a.Address;
        }
    }

    public class RTClockObject
    {
        public enum RTClockParameter
        {
            SetRTClock = 0x00,     // Request: 6 bytes (2 time, 2 msec)
            // Response: 0: success, 1: invalid data
            GetRTClock = 0x01,     // Response: 6 bytes (2 time, 2 msec)
        }
        /// <summary>
        /// converts byte array of 6 elements to data time
        /// </summary>
        /// <param name="data">data.Length must be larger than 6, first 2 bytes is useless, next 4 byte is seconds</param>
        /// <returns></returns>
        public static DateTime BytesToDateTime(byte[] data)
        {
            DateTime dateTime = new DateTime(1970, 1, 1);
            if (data == null || data.Length == 0) return dateTime;
            // data.Length must be larger than 6, first 2 bytes is useless, next 4 byte is seconds
            if (data.Length < 6) return dateTime;

            int pos = 0;
            int seconds = Converter.BytesToInt(data, pos);
            int milliseconds = Converter.BytesToShort(data, pos + 4);
            if (seconds < 0 || milliseconds < 0) return dateTime;

            dateTime = dateTime.AddSeconds(seconds + milliseconds / 1000.0);

            return dateTime;
        }
    }
    public class ToolSpecificObject
    {
        public enum EnumMemoryParameter
        {
            Clear = 0x00,     // Request: 10 bytes (?)
            GetMemoryInfo = 0x01,     // Response: 8 bytes (4 total memory size, 4 memory been used)
            GetEntryTableItems = 0x02,     // Request: 3 bytes (2 start item index, 1 # of items, max = 20); 
            // Response: N bytes: (2 start item index, 1 # of returned items, 4th - Nth 8 bytes per item)
            GetFatTableItems = 0x03,     // Request: 3 bytes (2 start item index, 1 # of items, max = 40); 
            // Response: N bytes: (2 start item index, 1 # of returned items, 4th - Nth 4 bytes per item)
            GetToolData = 0x04,     // Request: 6 bytes (4 start address, 2 data size)
            // Response: N bytes (4 start address, 2 data size, 7th - Nth data)
            SetToolData = 0x05,     // Request: N bytes
        }
    }
    public class LoggingObject
    {
        public enum EnumLoggingParameterDensity
        {
            StartDataAcquisition = 0x00,        // Response: 0: success
            StopDataAcquisition = 0x01,         // Response: 0: success
            SetParameters = 0x02,
            GetParameters = 0x03,
            GetAllData = 0x04,                  // Response: 1 WORD = 2 bytes
            BinPackInfo = 0x05,
            GetRawData = 0x09,                  // Response: 1 + 100 WORD = 202 bytes
            GetDensityBin = 0x0A,
            DeteqCmd = 0x0F,                    // Request: 3 byte
        }
    }
    public enum EnumToolAddress
    {
        All = 0x00,
        Surface = 0x01,
        Bootloader = 0x01,
        MWD = 0x02,
        Resisitivity = 0x03,
        Gamma = 0x04,
        BWRX = 0x05,
        BWDN = 0x06,
        BWNB = 0x07,
        BWRS = 0x08,
        BWPV = 0X09,
    }
    public enum EnumBoardAddress
    {
        NoBoard = 0x00,
        AllBoards = 0x01,
        Memory1 = 0x01,
        DataProcess = 0x01,
        Gamma = 0x01,
        DNSonicDAQ = 0x01,
        RSControl = 0x01,
        PressVib = 0x01,

        DrillersDisplay = 0x02,
        SignalGenerator = 0x02,
        RSSConvert = 0x02,
        DNNeutronDAQ = 0x02,
        NBBatteryControl = 0x03,// 修改
        MWDBatteryControl = 0x02,
        RSDownLink = 0x02,

        SurfaceBox = 0x03,
        MWD = 0x03,
        Memory2 = 0x03,
        NBAzimuthalGamma = 0x03,
        DNDensityDAQ = 0x03,
        RSShorthop = 0x03,

        LWDMaster = 0x04,
        NBShorthopTX = 0x04,
        RSDirectional = 0x04,

        USBDongle = 0x05,
        NBShorthopRX = 0x05,
        RSMotorCtrl = 0x05,

        Computer = 0x06,
        FSKModem = 0x07,
    }
    public class Crc16
    {
        const ushort polynomial = 0xA001;
        ushort[] table = new ushort[256];

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            //ushort crc = ComputeChecksum(bytes);
            ushort crc = (ushort)ComputeChecksum(bytes, bytes.Length, 0);
            return BitConverter.GetBytes(crc);
        }

        public Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }

        int[] crc16LUT = {
            0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7,
            0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF,
            0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6,
            0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE,
            0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485,
            0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D,
            0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4,
            0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC,
            0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823,
            0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B,
            0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12,
            0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A,
            0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41,
            0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49,
            0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70,
            0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78,
            0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F,
            0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067,
            0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E,
            0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256,
            0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D,
            0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
            0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C,
            0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634,
            0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB,
            0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3,
            0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A,
            0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92,
            0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9,
            0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1,
            0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8,
            0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0
        }; // crc16LUT

        int ComputeChecksum(byte[] pData, int byLen, int wReg)
        {
            int i;

            wReg = 0xFFFF;
            for (i = 0; i < byLen; i++)
            {
                //crcReg = (crcReg << 8) ^ crc16LUT[((BYTE)(crcReg >> 8)) ^ crcData[i]];
                wReg = (wReg >> 8) ^ crc16LUT[((byte)(wReg & 0xff)) ^ pData[i]];
            }
            return wReg;

        }
    }
}
