using GuidanceStsbilityComms;
using MultiPort;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuidanceStsbilityCommsDownLoad
{
    [Serializable]
    public struct MemoryDataOneRun
    {
        public int RunNumber;
        public String Description;

        public List<BoardInfo> BoardInfoList;
        public List<BoardParam> BoardParamList;

        public uint StartTime;
        public uint EndTime;
        public List<Dataset> TimeDatasetList;
        public List<Dataset> DepthDatasetList;
    }
    [Serializable]
    public struct BoardParam
    {
        // 1  2  3  4  5  6
        // XX XX XX XX XX XX data XX XX
        // d  s  len   o  pr      crc
        public byte[] DataBytes;

        public byte Dst;
        public byte Src;
        public int Len;         // = Data.Length + 8
        public int CmdObj;      // ex: 0x05, 0x07, 0x87
        public int Param;
        public byte[] Data;

        public BoardParam(byte[] data)
        {
            if (data == null || data.Length == 0) DataBytes = null;
            else
            {
                DataBytes = new byte[data.Length];
                Array.Copy(data, DataBytes, data.Length);
            }

            BytesToData(DataBytes, out Dst, out Src, out Len, out CmdObj, out Param, out Data);
        }

        public BoardParam(byte[] data, int start, int length)
        {
            if (data == null || data.Length == 0 ||
                start < 0 || length <= 0 || start + length >= data.Length) DataBytes = null;
            else
            {
                DataBytes = new byte[length];
                Array.Copy(data, start, DataBytes, 0, length);
            }

            BytesToData(DataBytes, out Dst, out Src, out Len, out CmdObj, out Param, out Data);
        }

        private static void BytesToData(byte[] dataBytes, out byte dst, out byte src, out int len, out int cmdObj, out int param, out byte[] data)
        {
            dst = 0x00;
            src = 0x00;
            len = 0;
            cmdObj = 0;
            param = 0;
            data = null;

            if (dataBytes == null || dataBytes.Length == 0) return;
            if (dataBytes.Length < 8) return;

            // 1  2  3  4  5  6
            // XX XX XX XX XX XX data XX XX
            // d  s  len   o  pr      crc
            DataPacketDownloadlist dataPacket = new DataPacketDownloadlist(dataBytes);
            dst = dataPacket.DstAddr;
            src = dataPacket.SrcAddr;
            len = dataPacket.PayLoad;
            if (dataPacket.Command != null && dataPacket.Command.Header != null)
            {
                cmdObj = (int)(dataPacket.Command.Header.CmdObject);
                param = dataPacket.Command.Header.Parameter;
                data = dataPacket.Command.Data;
            }
        }
    }
}
