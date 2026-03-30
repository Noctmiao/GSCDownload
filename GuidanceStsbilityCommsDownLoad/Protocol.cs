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
}
