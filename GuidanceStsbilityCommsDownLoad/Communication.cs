using MultiPort;
using neoCSNet2003;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GuidanceStsbilityCommsDownLoad.Communication;

namespace GuidanceStsbilityCommsDownLoad
{
    public class Communication
    {
        public enum EnumCommunication
        {
            Any = -1,
            RS232 = 0,
            CAN = 2,
        }
        public abstract class CommunicationObject
        {
            public abstract bool Open();
            public abstract void Send(byte[] data);
            public abstract byte[] Receive();
            public abstract void Close();
            public abstract String GetLastError();
        }
        public class ValueCan : CommunicationObject
        {
            protected int m_hObject;        // handle for device
            protected bool m_bPortOpen;     // tells the port status of the device

            protected String errorMsg = String.Empty;

            public ValueCan()
            {
                errorMsg = String.Empty;
            }
            public override bool Open()
            {
                // check if the port is already open
                if (m_bPortOpen == true)
                {
                    errorMsg = "Can is already open";
                    return true;
                }

                byte[] bNetwork = new byte[64];             // List of hardware IDs
                                                            // File NetworkID array
                for (int k = 0; k < 64; k++)
                {
                    bNetwork[k] = Convert.ToByte(k);
                }

                // Set the number of devices to find
                int numDevices = 1;
                NeoDevice ndNeoToOpen = new NeoDevice();    // Struct holding detected hardware information
                int iResult;

                // Search for connected hardware
                iResult = icsNeoDll.icsneoFindNeoDevices(65535, ref ndNeoToOpen, ref numDevices);
                if (iResult == 0 || numDevices < 1)
                {
                    numDevices = 1;// 初始必须为1
                    iResult = icsNeoDll.icsneoFindNeoDevices(0xFFFFBFFF, ref ndNeoToOpen, ref numDevices);// 更新valuecan3接口
                }

                if (iResult == 0)
                {
                    errorMsg = "Problem finding devices";
                    return false;
                }

                if (numDevices < 1)
                {
                    errorMsg = "No devices found";
                    return false;
                }

                // Open the first found device
                iResult = icsNeoDll.icsneoOpenNeoDevice(ref ndNeoToOpen, ref m_hObject, ref bNetwork[0], 1, 0);
                if (iResult == 1)
                {
                    errorMsg = "Port Opened OK!";
                }
                else
                {
                    errorMsg = "Problem Opening Port";
                    return false;
                }

                // Set device open flag
                m_bPortOpen = true;
                setBitRate();

                return true;
            }
            public override void Send(byte[] data)
            {
                if (data == null || data.Length == 0) return;

                // Has the user open neoVI yet?;
                if (m_bPortOpen == false)
                {
                    errorMsg = "ValueCAN is not opened";
                    return; // do not send messages if we haven't opened neoVI yet
                }

                DataPacketDownloadlist dataPacket = new DataPacketDownloadlist(data);
                //DataFrame dataFrame = new DataFrame();

                int length = data.Length;
                if (length <= 8)
                {   // single frame
                    sendDataFrame(data, dataPacket.DstAddr, dataPacket.SrcAddr, DataFrame.EnumFramePosition.Single);
                }
                else
                {
                    for (int pos = 0; pos < length; pos += 8)
                    {
                        int actualLength = 8;
                        DataFrame.EnumFramePosition framePos = DataFrame.EnumFramePosition.Middle;
                        if (pos == 0) framePos = DataFrame.EnumFramePosition.Start;
                        else if (pos + 8 >= length)
                        {
                            framePos = DataFrame.EnumFramePosition.End;
                            actualLength = length - pos;
                        }
                        else framePos = DataFrame.EnumFramePosition.Middle;

                        System.Diagnostics.Debug.Assert(actualLength > 0 && actualLength <= 8);
                        if (actualLength == 0 || actualLength > 8)
                        {
                            errorMsg = "Problem in sending data";
                            return;
                        }
                        byte[] frameData = new byte[actualLength];
                        Array.Copy(data, pos, frameData, 0, actualLength);
                        sendDataFrame(frameData, dataPacket.DstAddr, dataPacket.SrcAddr, framePos);
                    }
                }
            }
            public override byte[] Receive()
            {
                return null;
            }

            protected void sendDataFrame(byte[] frameData, byte dstAddr, byte srcAddr, DataFrame.EnumFramePosition framePos)
            {
                if (frameData == null || frameData.Length == 0 || frameData.Length > 8) return;

                long lResult;
                icsSpyMessage stMessagesTx = new icsSpyMessage();

                // load the message structure
                stMessagesTx.NetworkID = Convert.ToByte(eNETWORK_ID.NETID_HSCAN);
                stMessagesTx.StatusBitField = 0;            // Not Extended
                stMessagesTx.NumberBytesHeader = 11;
                stMessagesTx.ArbIDOrHeader = 1;           // The ArbID = 101 in hex string

                int length = frameData.Length;
                DataFrame dataFrame = new DataFrame();
                dataFrame.Priority = DataFrame.EnumPriority.High;
                // be careful: these two address are different, but this assignment still works
                // dataFrame.DstAddr: only lowerest 3 bits is effective
                // dataPacket.DstAddr: 1 byte, highest 5 bits is tool address, lowest 3 bits is board address
                // here we want to assign the lowest 3 bits (board address) to dataFrame.DstAddr

                dataFrame.DstAddr = dstAddr;
                //ToolBoardAddress toolBoardAddress = new ToolBoardAddress(dstAddr);
                //if (CurrentToolAddress == BWEnumToolAddress.Surface || toolBoardAddress.ToolAddress == CurrentToolAddress)
                //{// gamma 会选择这个,这里由于没有写出来姑且注掉了这一段，实际是存在判断的
                //    dataFrame.DstAddr = dstAddr;
                //}
                //else
                //{   // if not in the same sub network, send to FSK
                //    dataFrame.DstAddr = 0x07;
                //}
                dataFrame.SrcAddr = srcAddr;
                dataFrame.FramePos = framePos;
                dataFrame.M30 = 0;
                stMessagesTx.ArbIDOrHeader = dataFrame.ToInt();
                stMessagesTx.NumberBytesData = (byte)(length);
                stMessagesTx.Data1 = frameData[0];
                stMessagesTx.Data2 = (length > 1) ? frameData[1] : Convert.ToByte(0);
                stMessagesTx.Data3 = (length > 2) ? frameData[2] : Convert.ToByte(0);
                stMessagesTx.Data4 = (length > 3) ? frameData[3] : Convert.ToByte(0);
                stMessagesTx.Data5 = (length > 4) ? frameData[4] : Convert.ToByte(0);
                stMessagesTx.Data6 = (length > 5) ? frameData[5] : Convert.ToByte(0);
                stMessagesTx.Data7 = (length > 6) ? frameData[6] : Convert.ToByte(0);
                stMessagesTx.Data8 = (length > 7) ? frameData[7] : Convert.ToByte(0);

                // Transmit the assembled message
                System.Threading.Thread.Sleep(1);
                lResult = icsNeoDll.icsneoTxMessages(m_hObject, ref stMessagesTx, (int)eNETWORK_ID.NETID_HSCAN, 0);
                // Test the returned result
                if (lResult != 1)
                {
                    errorMsg = "Problem Transmitting Message";
                }
            }
            public override void Close()
            {
                int iResult;
                int numErrors = 0;

                //close the port
                iResult = icsNeoDll.icsneoClosePort(m_hObject, ref numErrors);
                if (iResult == 1)
                {
                    errorMsg = "Port Closed OK!";
                    //MessageBox.Show(errorMsg);
                }
                else
                {
                    errorMsg = "Problem Closing Port";
                    //MessageBox.Show(errorMsg);
                }

                //Clear device type and open flag
                m_bPortOpen = false;
            }
            public override String GetLastError()
            {
                String lastError = errorMsg;
                errorMsg = String.Empty;
                return lastError;
            }
            // bitRate = 250K for Tools, need override by SurfaceCan
            protected virtual void setBitRate()
            {
                int iBitRateToUse;
                int iNetworkID;
                int iResult;

                if (m_bPortOpen == false)
                {
                    errorMsg = "ValueCan is not opened";
                    return;  // do not read messages if we haven't opened neoVI yet
                }

                iNetworkID = Convert.ToInt32(eNETWORK_ID.NETID_HSCAN);

                iBitRateToUse = 250000;
                //iBitRateToUse = 100000;

                //Set the bit rate
                iResult = icsNeoDll.icsneoSetBitRate(m_hObject, iBitRateToUse, iNetworkID);
                if (iResult == 0)
                {
                    errorMsg = "Problem setting bit rate";
                }
                else
                {
                    errorMsg = "Bit rate set" + " " + iBitRateToUse.ToString(); ;
                }
            }
        }
        public class ToolsValueCan : ValueCan
        {
            private byte[][] dataPacketArray;
            private int[] dataPacketPos;
            private const int numberOfBoardAddress = 8;
            private const int numberOfDataPacketBytes = 65536;
            public ToolsValueCan()
                : base()
            {
                dataPacketPos = new int[numberOfBoardAddress];
                dataPacketArray = new byte[numberOfBoardAddress][];
                for (int i = 0; i < numberOfBoardAddress; i++)
                {
                    dataPacketPos[i] = 0;
                    dataPacketArray[i] = new byte[numberOfDataPacketBytes];
                }

                errorMsg = String.Empty;
            }
            public override byte[] Receive()
            {
                if (m_bPortOpen == false)
                {
                    errorMsg = "ValueCAN is not opened";
                    //MessageBox.Show(errorMsg);
                    return null;  // do not read messages if we haven't opened neoVI yet
                }

                // read the messages from the driver
                int numMsgs = 0;
                int numErrs = 0;
                icsSpyMessage[] stMessages = new icsSpyMessage[20000];   //TempSpace for messages
                int rc = icsNeoDll.icsneoGetMessages(m_hObject, ref stMessages[0], ref numMsgs, ref numErrs);
                // was the read successful?
                if (rc != 1)
                {
                    errorMsg = "Problem Reading Messages";
                    //MessageBox.Show(errorMsg);
                    return null;
                }

                byte[] dataPacketsToReturn = null;
                byte[] dataPacketsSeperator = { 0x58, 0x58, 0x58 };     // XXX
                for (int k = 0; k < numMsgs; k++)
                {
                    //Get the byte count
                    int NBytes = stMessages[k].NumberBytesData;
                    if (NBytes == 0) continue;

                    // Was it a CAN or other protocol
                    if (Convert.ToBoolean(stMessages[k].Protocol == Convert.ToInt32(ePROTOCOL.SPY_PROTOCOL_CAN)))
                    {
                        // list the arb id
                        int frameHeader = stMessages[k].ArbIDOrHeader;
                        DataFrame dataFrame = new DataFrame(frameHeader);
                        int srcAddr = dataFrame.SrcAddr;
                        System.Diagnostics.Debug.Assert(srcAddr >= 0 && srcAddr < numberOfBoardAddress);
                        if (srcAddr < 0 || srcAddr >= numberOfBoardAddress)
                        {
                            errorMsg = "Problem in receiveing data";
                            //MessageBox.Show(errorMsg);
                            return null;
                        }

                        byte[] dataReceived = new byte[NBytes];
                        if (NBytes >= 1) dataReceived[0] = stMessages[k].Data1;
                        if (NBytes >= 2) dataReceived[1] = stMessages[k].Data2;
                        if (NBytes >= 3) dataReceived[2] = stMessages[k].Data3;
                        if (NBytes >= 4) dataReceived[3] = stMessages[k].Data4;
                        if (NBytes >= 5) dataReceived[4] = stMessages[k].Data5;
                        if (NBytes >= 6) dataReceived[5] = stMessages[k].Data6;
                        if (NBytes >= 7) dataReceived[6] = stMessages[k].Data7;
                        if (NBytes >= 8) dataReceived[7] = stMessages[k].Data8;

                        switch (dataFrame.FramePos)
                        {
                            case DataFrame.EnumFramePosition.Single:
                                if (dataPacketsToReturn == null)
                                {
                                    dataPacketsToReturn = new byte[NBytes];
                                    Array.Copy(dataReceived, dataPacketsToReturn, NBytes);
                                }
                                else
                                {
                                    //dataPacketsToReturn += dataPacketsSeperator + hexString;
                                    byte[] temp = dataPacketsToReturn;
                                    dataPacketsToReturn = new byte[temp.Length + 3 + NBytes];
                                    Array.Copy(temp, 0, dataPacketsToReturn, 0, temp.Length);
                                    Array.Copy(dataPacketsSeperator, 0, dataPacketsToReturn, temp.Length, 3);
                                    Array.Copy(dataReceived, 0, dataPacketsToReturn, temp.Length + 3, NBytes);
                                }
                                dataPacketPos[srcAddr] = 0;
                                break;
                            case DataFrame.EnumFramePosition.Start:
                                // in general, dataPacketPos[dst?Addr] not must be 0, since there may be 
                                // previous data left over which didn't get ending stream
                                //System.Diagnostics.Debug.Assert(dataPacketPos[dst?Addr] == 0);
                                dataPacketPos[srcAddr] = 0;
                                Array.Copy(dataReceived, 0, dataPacketArray[srcAddr], dataPacketPos[srcAddr], NBytes);
                                dataPacketPos[srcAddr] += NBytes;
                                break;
                            case DataFrame.EnumFramePosition.Middle:
                                System.Diagnostics.Debug.Assert(dataPacketPos[srcAddr] + NBytes < numberOfDataPacketBytes);
                                Array.Copy(dataReceived, 0, dataPacketArray[srcAddr], dataPacketPos[srcAddr], NBytes);
                                dataPacketPos[srcAddr] += NBytes;
                                break;
                            case DataFrame.EnumFramePosition.End:
                                System.Diagnostics.Debug.Assert(dataPacketPos[srcAddr] + NBytes < numberOfDataPacketBytes);
                                Array.Copy(dataReceived, 0, dataPacketArray[srcAddr], dataPacketPos[srcAddr], NBytes);
                                int dataLength = dataPacketPos[srcAddr] + NBytes;
                                byte[] dataPacketBytes = new byte[dataLength];
                                Array.Copy(dataPacketArray[srcAddr], 0, dataPacketBytes, 0, dataLength);
                                //hexString = BWNSUtility.BWConverter.BytesToHexString(dataPacketBytes);
                                if (dataPacketsToReturn == null)
                                {
                                    dataPacketsToReturn = new byte[dataLength];
                                    Array.Copy(dataPacketBytes, dataPacketsToReturn, dataLength);
                                }
                                else
                                {
                                    //dataPacketsToReturn += dataPacketsSeperator + hexString;
                                    byte[] temp = dataPacketsToReturn;
                                    dataPacketsToReturn = new byte[temp.Length + 3 + dataLength];
                                    Array.Copy(temp, 0, dataPacketsToReturn, 0, temp.Length);
                                    Array.Copy(dataPacketsSeperator, 0, dataPacketsToReturn, temp.Length, 3);
                                    Array.Copy(dataPacketBytes, 0, dataPacketsToReturn, temp.Length + 3, dataLength);
                                }
                                dataPacketPos[srcAddr] = 0;
                                break;
                            default:
                                break;
                        }
                    }
                }

                return dataPacketsToReturn;
            }
        }
    }
}
