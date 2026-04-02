using AntdUI;
using BWNSLWDTools;
using GuidanceStsbilityComms;
using LsySkin;
using MultiPort;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GuidanceStsbilityCommsDownLoad.Communication;
using static System.Resources.ResXFileRef;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace GuidanceStsbilityCommsDownLoad
{
    public partial class GSCDownload : AntdUI.Window
    {
        private List<int> entryItemSelectedIndexList;   // indices of entry items selected to dump
        private int curEntryItemPos;                    // between [0x3, entryItemSelectedIndexList.Count - 1]
        // for progress bar
        private int totalSizeToDump;
        private int sizeDumped;
        private DateTime dumpStartTime;
        // output
        // foreach entryItem to dump
        private uint startAddress;      // start address for each entry item
        private uint dumpSize;          // size of current dump = endAddress - startAddress
        private int curDataPos;         // points to within dumpedData
        private byte[] dumpedData;      // buffer of size = dumpSize
        // for all selected entryItems to dump
        private List<MemoryDataOneRun> memoryDataRunList;
        private List<Dataset> timeDatasetList;

        // 32 raw resistivity datasets
        private List<Dataset> timeRawDatasetList;

        public bool IsWaitingForResponse;
        private DataPacketDownloadlist sentDataPacket;
        // communication
        private EnumCommunication communication;
        private CommunicationObject commObj;

        //private AntList<DownloadListItem> downloadListItems;

        //datapacket length every time
        public int maxLength;
        public int tryLength;

        // memory structure
        private const int maxNumOfEntryItems = 10;
        private ushort startEntryItemNum = 0;
        private ushort startFatItemNum = 0;
        private List<EntryItem> entryItemList;
        private List<FatItem> fatItemList;

        private const int maxDumpSize = 256;    // max num of bytes to dump each time
        private const int pageSize = 256;       // 256 bytes per page in memory

        // memory info
        private uint memorySize;
        private uint memoryUsed;

        // retry
        private int timeOut = 1000;      // 1000 ms
        private int numOfTry;

        // tools
        public static BaseTool GSCTool;


        System.Windows.Forms.Timer timerReceive;// 原lwdtools主界面
        System.Windows.Forms.Timer timerDump1;
        System.Windows.Forms.Timer timerDump2;

        private static bool isDiagnostic = false;

        public GSCDownload()
        {
            InitializeComponent();

            // load
            startEntryItemNum = 0;
            entryItemList = new List<EntryItem>();
            fatItemList = new List<FatItem>();

            button_dump.Enabled = false;
            button_clear.Enabled = false;
            timerReceive = new System.Windows.Forms.Timer();
            timerReceive.Interval = 100;
            timerReceive.Tick += new System.EventHandler(timerReceive_Tick);

            timerDump1 = new System.Windows.Forms.Timer();
            timerDump1.Interval = 100;
            timerDump1.Enabled = false;
            timerDump1.Tick += new System.EventHandler(timer1_Tick);

            timerDump2 = new System.Windows.Forms.Timer();
            timerDump2.Interval = 100;
            timerDump2.Enabled = false;

            // table init
            //Init_listView_ResistivityTool_downloadlist();
            // antdui init
            Config.IsLight = true;
            BackColor = Color.White;
            Config.ShowInWindow = true;

        }

        /*private void Init_listView_ResistivityTool_downloadlist()
        {
            // columns
            listView_ResistivityTool_downloadlist.Columns = new ColumnCollection()
            {
                new Column("Number", "编号", ColumnAlign.Center)
                {
                    Width = "90"
                },
                new Column("Starttime", "开始时间")
                {
                    Width = "140"
                },
                new Column("Endtime", "终止时间")
                {
                    Width = "140"
                },
                new Column("Timeinterval", "时间间隔（秒）")
                {
                    Width="110"
                },
                new Column("Bytes", "占用字节")
                {
                    Width = "110"
                }
            };
            // bind date
            downloadListItems = new AntList<DownloadListItem>();
            listView_ResistivityTool_downloadlist.Binding(downloadListItems);
        }*/
        private void button_BlueCAN_Click(object sender, EventArgs e)
        {
            using (new CursorWait())
            {
                communication = EnumCommunication.CAN;
                initializeCommunicationObject();
            }
        }
        private void initializeCommunicationObject()
        {
            if (commObj != null) commObj.Close();

            switch (communication)
            {
                case EnumCommunication.RS232:
                    //commObj = new BWToolsSerialPort(serialPort1);
                    break;
                case EnumCommunication.CAN:
                    commObj = new ToolsValueCan();
                    //setCurrentToolAddressInValueCan();
                    break;
                default:
                    //if (commObj == null) commObj = new BWToolsValueCan();
                    //setCurrentToolAddressInValueCan();
                    break;
            }

            if (commObj != null) commObj.Open();
            String lastErr = commObj.GetLastError();
            if (lastErr != String.Empty)
            {
                // 不要弹出，放到信息栏
                string text = "Status: " + lastErr;
                label_CAN.Text = text;
            }

            // automatically find out which sub-network computer connects
            //findTools();
        }
        private void setCurrentToolAddressInValueCan()
        {
            if (communication == EnumCommunication.CAN)
            {
                ToolsValueCan canCommObj = (ToolsValueCan)commObj;
                //if (canCommObj != null)
                //{
                //    ToolBoardAddress toolBoardAddress = new ToolBoardAddress(computerAddress);
                //    canCommObj.CurrentToolAddress = toolBoardAddress.ToolAddress;
                //}
            }
        }
        private void button_readmemory_Click(object sender, EventArgs e)
        {
            //LWDTools.mainForm.comboBoxSubNetwork.SelectedIndex = 1;// 0x16
            this.BeginInvoke(new System.Action(() => { memorySelectionChanged(); }));
        }
        private void memorySelectionChanged()
        {
            listViewRuns.Items.Clear();

            startEntryItemNum = 0;
            entryItemList.Clear();
            fatItemList.Clear();
            requestMemoryInfo();

            button_dump.Enabled = false;
            button_clear.Enabled = false;
        }
        byte SendDst = 0x13;
        private void requestMemoryInfo()
        {
            //byte dst = ToolBoardAddress.GetToolBoardAddress(0, 0);// 接收的src
            byte dst = SendDst;// 接收的src
            int param = (int)(ToolSpecificObject.EnumMemoryParameter.GetMemoryInfo);
            byte[] data = null;

            CommandHeader.EnumCmdObject obj = CommandHeader.EnumCmdObject.NearBitMemory;
            sentDataPacket = DataPacketDownloadlist.GenerateDataPacket(dst, obj, param, data);

            //byte[] datasend = new byte[] { 0x13, 0x0e, 0x08, 0x00, 0x09, 0x10, 0x00, 0x00 };
            //ushort checksum = (ushort)DataPacketDownloadlist.ComputeChecksum2(datasend, 6);
            //byte[] checkSumCalc = BitConverter.GetBytes(checksum);
            //Array.Copy(checkSumCalc, 0, datasend, 6, 2);

            IsWaitingForResponse = true;
            Send(sentDataPacket.ToBytes());
        }
        private void responseMemoryInfo(DataPacketDownloadlist receivedDataPacket)
        {
            if (receivedDataPacket == null ||
                receivedDataPacket.Command == null ||
                receivedDataPacket.Command.Data == null ||
                receivedDataPacket.Command.Data.Length < 8) return;

            byte[] data = receivedDataPacket.Command.Data;
            memorySize = Converter.BytesToUInt(data, 0);
            if (data[4] == 0xFF && data[5] == 0xFF && data[6] == 0xFF && data[7] == 0xFF) memoryUsed = 0;
            else memoryUsed = Converter.BytesToUInt(data, 4);

            progress_memoryused.Text = String.Format("{0}K/{1}K", memoryUsed / 1024, memorySize / 1024);
        }
        private void requestGetEntryTableItems()
        {
            byte dst = SendDst;// 接收的src
            int param = (int)(ToolSpecificObject.EnumMemoryParameter.GetEntryTableItems);

            byte[] data = new byte[3];
            byte[] temp = Converter.UShortToBytes(startEntryItemNum);
            Array.Copy(temp, 0, data, 0, 2);
            temp = Converter.UShortToBytes(maxNumOfEntryItems);
            data[2] = temp[0];

            CommandHeader.EnumCmdObject obj = CommandHeader.EnumCmdObject.NearBitMemory;
            sentDataPacket = DataPacketDownloadlist.GenerateDataPacket(dst, obj, param, data);

            IsWaitingForResponse = true;
            Send(sentDataPacket.ToBytes());
        }// 请求下载列表的数据
        private void requestGetFatTableItems()
        {
            byte dst = SendDst;// 接收的src
            int param = (int)(ToolSpecificObject.EnumMemoryParameter.GetFatTableItems);

            byte[] data = new byte[3];
            byte[] temp = Converter.UShortToBytes(startFatItemNum);
            Array.Copy(temp, 0, data, 0, 2);
            temp = Converter.UShortToBytes(1);
            data[2] = temp[0];

            CommandHeader.EnumCmdObject obj = CommandHeader.EnumCmdObject.NearBitMemory;
            //if (whichMemory == 2 || whichMemory == 6 || whichMemory == 0) obj = BWCommandHeader.BWEnumCmdObject.NearBitMemory;// 2.修改源地址：国家项目源地址
            sentDataPacket = DataPacketDownloadlist.GenerateDataPacket(dst, obj, param, data);

            IsWaitingForResponse = true;
            Send(sentDataPacket.ToBytes());
        }// 在下载具体数据前的请求
        private void requestToolData(uint startAddr, ushort length)// 下载请求
        {
            byte dst = SendDst;
            int param = (int)(ToolSpecificObject.EnumMemoryParameter.GetToolData);

            byte[] data = new byte[6];
            byte[] temp = null;
            temp = Converter.UIntToBytes(startAddr);
            Array.Copy(temp, 0, data, 0, 4);
            temp = Converter.UShortToBytes(length);
            Array.Copy(temp, 0, data, 4, 2);

            CommandHeader.EnumCmdObject obj = CommandHeader.EnumCmdObject.ToolSpecific;
            //if (whichMemory == 2 || whichMemory == 6 || whichMemory == 0) obj = BWCommandHeader.BWEnumCmdObject.NearBitMemory;// 2.修改源地址：国家项目源地址
            sentDataPacket = DataPacketDownloadlist.GenerateDataPacket(dst, obj, param, data);

            IsWaitingForResponse = true;
            Send(sentDataPacket.ToBytes());

            timerDump1.Interval = timeOut;
            numOfTry = 1;
            timerDump1.Enabled = true;
        }

        // send and receive
        public void Send(byte[] dataPacket)
        {
            if (commObj != null)
            {
                //hexString = "DA FC 00 07 FC 02 D9";
                //dataPacket = BWNSUtility.BWConverter.HEXStringToBytes(hexString);
                commObj.Send(dataPacket);

                if (commObj.GetLastError() != String.Empty)
                {
                    //SetStatusBarText(commObj.GetLastError(), BWEnumStatusBar.Status);
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsWaitingForResponse == true)
            {
                //if (numOfTry < numOfMaxTry)
                //{
                //    if (LWDTools != null) LWDTools.Send(sentDataPacket.ToBytes());
                //    numOfTry++;
                //}
                //else
                //{
                //    IsWaitingForResponse = false;
                //    timer1.Enabled = false;
                //    MessageBox.Show(MyStrings.String_Failed_to_dump_memory);
                //}
            }
        }

        private void timerReceive_Tick(object sender, EventArgs e)
        {
            if (commObj == null) return;
            Receive();
        }
        public String Receive()
        {
            if (commObj == null) return String.Empty;

            byte[] temp = commObj.Receive();
            String hexString = bytesToHexString(temp);
            String lastError = commObj.GetLastError();
            if (lastError != String.Empty)
            {
                //SetStatusBarText(lastError, BWEnumStatusBar.Status);
            }

            if (hexString == String.Empty) return String.Empty;

            //// for test
            //hexString = "0E 1B 0C 00 8A 00 00 FF 01 00 30 8F";

            processReceivedData(hexString);

            return hexString;
        }
        /// <summary>
        /// converts a byte[] to a HexString
        /// </summary>
        /// <param name="data">multiple data packets separated by "XXX"</param>
        /// <returns></returns>
        private String bytesToHexString(byte[] data)
        {
            if (data == null || data.Length == 0) return String.Empty;

            String hexString = String.Empty;

            int pos = 0;
            int index = 0;
            while (pos < data.Length)
            {
                int packetLength = 256 * data[pos + 3] + data[pos + 2];
                if (packetLength == 0 || pos + packetLength > data.Length)
                {
                    byte[] packetData1 = new byte[data.Length];
                    Array.Copy(data, pos, packetData1, 0, data.Length);
                    pos += data.Length;
                    continue;

                }

                byte[] packetData = new byte[packetLength];
                Array.Copy(data, pos, packetData, 0, packetLength);
                pos += packetLength;

                if (hexString == String.Empty) hexString = Converter.BytesToHexString(packetData);
                else hexString += "XXX" + Converter.BytesToHexString(packetData);

                if (pos + 2 < data.Length && data[pos] == 0x58 && data[pos + 1] == 0x58 && data[pos + 2] == 0x58) pos += 3;
            }

            return hexString;
        }
        /// <summary>
        /// received data may contains more than one data packet, seperated by "XXX";
        /// each data packet is in hex string format
        /// </summary>
        /// <param name="hexString"></param>
        private void processReceivedData(String hexString)
        {
            if (hexString == String.Empty) return;

            // process multiple data packets seperated by "XXX"
            String[] sprt = new String[] { "XXX" };
            String[] hexStrings = hexString.Split(sprt, StringSplitOptions.RemoveEmptyEntries);
            maxLength = hexStrings.Length;
            tryLength = 0;

            foreach (String s in hexStrings)
            {
                String cmd = Converter.AddSpacesToHexString(s);
                byte[] dataPacketIn = Converter.HEXStringToBytes(cmd);
                DataPacketDownloadlist dataPacket = new DataPacketDownloadlist(dataPacketIn);
                if (dataPacket != null && dataPacket.Command != null && dataPacket.Command.Header != null)
                {
                    if (dataPacket.Command.Header.IsResponse == false) cmd = "TX -> " + cmd;
                    else cmd = "RX -> " + cmd;
                }
                //setText(cmd, mainForm.textBoxMessage);
                //commandsLog(cmd, commandsLogFileName, maxCommandsLogFileSize);

                processSingleDataPacket(s);
                tryLength++;
            }
        }
        private void processSingleDataPacket(String hexString)
        {
            if (hexString == String.Empty) return;

            hexString = Converter.AddSpacesToHexString(hexString);
            byte[] dataPacketIn = Converter.HEXStringToBytes(hexString);
            DataPacketDownloadlist dataPacket = new DataPacketDownloadlist(dataPacketIn);
            byte srcAddr = dataPacket.SrcAddr;
            ToolBoardAddress address = new ToolBoardAddress(srcAddr);
            ToolCommand cmd = dataPacket.Command;
            CommandHeader header = cmd.Header;
            int len = dataPacket.PayLoad;
            byte[] data = cmd.Data;

            // !! special handling for density bin package 2/1/2017 -YL
            if (header.IsResponse == false) return;

            // notify any object that is waiting for response
            BroadcastMessageReceived(dataPacket); // 刷新下载列表

            //System.Diagnostics.Debug.Assert((data != null) && (data.Length == len - 8));
            /*if (address.ToolAddress == EnumToolAddress.All)
            {
                GSCTool.ProcessDataPacket(dataPacket);// 上面运行完下载列表请求后确实直接来着了，但是在这里一个if都没进
                UpdateDisplayNearBitData();// 上面没进就导致没有数据需要更新，下面也没有运行
            }*/
        }
        /// <summary>
        /// process and create a single run dumped data: memData1r, add to memoryDataList.
        /// append new memData1r.timeDatasetList to timeDatasetList
        /// </summary>
        private void processDumpedDataOneRun()
        {
            MemoryDataOneRun memData1r = new MemoryDataOneRun();
            int curEntryItemNum = entryItemSelectedIndexList[curEntryItemPos];
            memData1r.RunNumber = curEntryItemNum + 1;
            memData1r.StartTime = entryItemList[curEntryItemNum].StartTime;
            memData1r.EndTime = entryItemList[curEntryItemNum].EndTime;

            saveDumpedDataOneRun(memData1r, dumpedData);

            memData1r.TimeDatasetList = ProcessMemoryData.ProcessDumpedData(dumpedData);
            memData1r.BoardInfoList = ProcessMemoryData.BoardInfoList;
            memData1r.BoardParamList = ProcessMemoryData.BoardParamList;

            if (memoryDataRunList == null) memoryDataRunList = new List<MemoryDataOneRun>();
            memoryDataRunList.Add(memData1r);

            if (timeDatasetList == null) timeDatasetList = new List<Dataset>();
            DatasetUtility.Merge(timeDatasetList, memData1r.TimeDatasetList, DataMergeOption.append);
        }
        // raw memory data
        private void saveDumpedDataOneRun(MemoryDataOneRun mem1r, byte[] dumpedData1r)
        {
            String memoryDir = "GSC";
            String myProjectDataDirectory = Path.Combine(Environment.CurrentDirectory + @"\Logs", Application.ProductName + " " + Application.ProductVersion);
            if (!File.Exists(myProjectDataDirectory)) Directory.CreateDirectory(myProjectDataDirectory);
            String myFileFullName = Path.Combine(myProjectDataDirectory, memoryDir);
            memoryDir = myFileFullName;
            Directory.CreateDirectory(memoryDir);

            String startTime = Converter.SecondsToLongTime2(mem1r.StartTime);
            String fileName = Path.Combine(memoryDir, "Run " + mem1r.RunNumber.ToString() + " " + startTime) + ".dump";

            FileStream fs = new FileStream(fileName, FileMode.Create);
            try
            {
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(dumpedData);
                bw.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                String debugMsg = "Info: BWMemoryDumpDlg::saveDumpedDataOneRun(): " + "Fail to save dumped data!" + " " + ex.Message;
                Utility.DebugLog(debugMsg);
                if (fs != null) fs.Close();
            }
        }

        // broadcast message received, notify any object waiting for response
        public void BroadcastMessageReceived(DataPacketDownloadlist dataPacket)
        {// 通知等待响应的对象，刷新下载列表
            //if (bootloaderDlg != null &&
            //    bootloaderToolStripMenuItem.Enabled == false &&
            //    bootloaderDlg.IsWaitingForResponse == true)
            //{
            //    bootloaderDlg.HandleResponse(dataPacket);
            //}
            //else if (airHangTestDlg != null &&
            //   airHangTestToolStripMenuItem.Enabled == false &&
            //    airHangTestDlg.IsWaitingForResponse == true)
            //{
            //    airHangTestDlg.HandleResponse(dataPacket);
            //}
            //else if (SetToolSettingsDlg != null &&
            //    //toolSettingsToolStripMenuItem.Enabled == false &&
            //    SetToolSettingsDlg.IsWaitingForResponse == true)
            //{
            //    SetToolSettingsDlg.HandleResponse(dataPacket);
            //}
            if (IsWaitingForResponse == true)
            {
                HandleResponse(dataPacket);
            }
            //else if (resistivityCalibrationDlg != null &&
            //    resistivityCalibrationDlg.IsWaitingForResponse == true)
            //{
            //    resistivityCalibrationDlg.HandleResponse(dataPacket);
            //}
            //else if (NBBatteryControlSettingsDlg != null &&
            //    NBBatteryControlSettingsDlg.IsWaitingForResponse == true)
            //{
            //    NBBatteryControlSettingsDlg.HandleResponse(dataPacket);
            //}
            //else if (aziGamCalibDlg != null &&
            //    aziGamCalibDlg.IsWaitingForResponse == true)
            //{
            //    aziGamCalibDlg.HandleResponse(dataPacket);
            //}
            //else if (aziGamCalibDlg_pt != null &&
            //    aziGamCalibDlg_pt.IsWaitingForResponse == true)
            //{
            //    aziGamCalibDlg_pt.HandleResponse(dataPacket);
            //}
            //else if (dirResCalibDlg != null &&
            //    dirResCalibDlg.IsWaitingForResponse == true)
            //{
            //    dirResCalibDlg.HandleResponse(dataPacket);
            //}
            //else if (pwdCalibDlg != null &&
            //    pwdCalibDlg.IsWaitingForResponse == true)
            //{
            //    pwdCalibDlg.HandleResponse(dataPacket);
            //}
            //else if (mwdCalibDlg != null &&
            //    mwdCalibDlg.IsWaitingForResponse == true)
            //{
            //    mwdCalibDlg.HandleResponse(dataPacket);
            //}
            //else if (densityTestDlg != null &&
            //    densityTestDlg.IsWaitingForResponse == true)
            //{
            //    densityTestDlg.HandleResponse(dataPacket);
            //}
            //else if (neutronTestDlg != null &&
            //    neutronTestDlg.IsWaitingForResponse == true)
            //{
            //    neutronTestDlg.HandleResponse(dataPacket);
            //}
            //else if (rsCalibDlg != null &&
            //    rsCalibDlg.IsWaitingForResponse == true)
            //{
            //    rsCalibDlg.HandleResponse(dataPacket);
            //}
            //else if (nuclearCalibDlg != null &&
            //    nuclearCalibDlg.IsWaitingForResponse == true)
            //{
            //    nuclearCalibDlg.HandleResponse(dataPacket);
            //}
            //else if (selectToolDlg != null &&
            //    selectToolDlg.IsWaitingForResponse == true)
            //{
            //    selectToolDlg.HandleResponse(dataPacket);
            //}
            //else if (rsSettingsDlg != null &&
            //    rsSettingsDlg.IsWaitingForResponse == true)
            //{
            //    rsSettingsDlg.HandleResponse(dataPacket);
            //}
            //else if (packetInterpDlg != null &&
            //    packetInterpDlg.IsWaitingForResponse == true)
            //{
            //    packetInterpDlg.HandleResponse(dataPacket);
            //}
            //else if (acceleCalibDlg != null &&
            //    acceleCalibDlg.IsWaitingForResponse == true)
            //{
            //    acceleCalibDlg.HandleResponse(dataPacket);
            //}
            //else if (autoDiagnosisDlg != null &&
            //    autoDiagnosisDlg.IsWaitingForResponse == true)
            //{
            //    autoDiagnosisDlg.HandleResponse(dataPacket);
            //}
        }
        public void HandleResponse(DataPacketDownloadlist receivedDataPacket)
        {
            if (DataPacketDownloadlist.VerifyDataPacket(sentDataPacket, receivedDataPacket) == false) return;

            IsWaitingForResponse = false;
            timerDump1.Enabled = false;

            int param = receivedDataPacket.Command.Header.Parameter;

            // special handling for get RTC time
            if (receivedDataPacket.Command.Header.CmdObject == CommandHeader.EnumCmdObject.RTClock && param == (int)RTClockObject.RTClockParameter.GetRTClock)
            {// 方位伽马第一次运行到这里CmdObject是M30Data
                byte[] data = receivedDataPacket.Command.Data;
                DateTime dt = RTClockObject.BytesToDateTime(data);
                MessageBox.Show(string.Format("{0:MM/dd/yyyy HH:mm:ss}", dt));
                return;
            }

            switch (param)
            {
                //case (int)(ToolSpecificObject.BWEnumMemoryParameter.Clear):
                //    if (responseClearMemory(receivedDataPacket) == true)
                //    {
                //        MessageBox.Show(MyStrings.String_Clear_memory_successful);
                //        progressBar1.Value = progressBar1.Maximum;
                //        timer2.Enabled = false;
                //        enableControls(true);

                //        requestMemoryInfo();
                //    }
                //    else
                //    {   // ignore the first response, which is incorrect
                //        if (isFirstResponse == true)
                //        {
                //            isFirstResponse = false;
                //            IsWaitingForResponse = true;    // keep waiting
                //        }
                //        else MessageBox.Show(MyStrings.String_Failed_to_dump_memory);
                //    }
                //    break;
                case (int)(ToolSpecificObject.EnumMemoryParameter.GetMemoryInfo):// 第一次运行到这里
                    responseMemoryInfo(receivedDataPacket);

                    startEntryItemNum = 0;
                    if (entryItemList != null) entryItemList.Clear();
                    requestGetEntryTableItems();
                    break;
                case (int)(ToolSpecificObject.EnumMemoryParameter.GetEntryTableItems):// 第二次运行到这里；第三次也是
                    if (responseGetEntryTableItems(receivedDataPacket) == maxNumOfEntryItems)
                    {// 走这里
                        startEntryItemNum += maxNumOfEntryItems;
                        requestGetEntryTableItems();
                    }
                    else
                    {// 最后会走这里，我理解为10个读一次直到读到最后一个
                        refreshDisplay();
                        System.Diagnostics.Debug.WriteLine("Entry table has " + entryItemList.Count + " items!");
                        button_dump.Enabled = true;
                        button_clear.Enabled = true;
                    }
                    break;
                case (int)(ToolSpecificObject.EnumMemoryParameter.GetFatTableItems):// 下载数据请求发送后接收到的回数
                    if (responseGetFatTableItems(receivedDataPacket) != 1)
                    {
                        ;
                    }
                    else
                    {
                        dumpSize = (uint)entryItemList[entryItemSelectedIndexList[curEntryItemPos]].NumOfPages * pageSize;
                        if (isDiagnostic == true) dumpSize = 2 * pageSize;      // all board info size = 333 bytes (2+4+31) * 9
                        curDataPos = 0;
                        dumpedData = new byte[dumpSize];

                        // dump memory data from start to end memory addresses
                        ushort requestDumpSize = maxDumpSize;
                        if (dumpSize < maxDumpSize) requestDumpSize = (ushort)dumpSize;
                        requestToolData(startAddress, requestDumpSize);
                    }
                    break;
                case (int)(ToolSpecificObject.EnumMemoryParameter.GetToolData):
                    int actualDumpedSize = responseToolData(receivedDataPacket);
                    curDataPos += actualDumpedSize;
                    if (curDataPos >= dumpSize)
                    {   // reaches the end of this entry item, process dumped memory data and dump next entry item;
                        System.Diagnostics.Debug.WriteLine("Finish dump entry: " + entryItemSelectedIndexList[curEntryItemPos]);
                        if (isDiagnostic == false) processDumpedDataOneRun();
                        //else processDiagnosticDataOneRun();

                        //// set prograss bar
                        //sizeDumped += calculateTotalMemorySizeToDoump(entryItemSelectedIndexList[curEntryItemPos]);
                        //this.progressBar1.Value = sizeDumped * 100 / totalSizeToDump;

                        curEntryItemPos++;
                        if (curEntryItemPos >= entryItemSelectedIndexList.Count)
                        {   // reaches the end of this dump
                            updateProgressBar(totalSizeToDump);
                            //IsWaitingForResponse = false;
                            MessageBox.Show(MyStrings.String_Memory_dump_finished_successfully);// marker 可以替换
                            return;
                        }
                        else
                        {   // dump the next entry item
                            dumpMemory(entryItemSelectedIndexList[curEntryItemPos]);
                        }
                    }
                    else
                    {
                        //System.Diagnostics.Debug.WriteLine("curDataPos: " + curDataPos + ", total size: " + dumpSize);
                        requestToolData((uint)(startAddress + curDataPos), maxDumpSize);
                    }

                    // set prograss bar
                    sizeDumped += actualDumpedSize;
                    updateProgressBar(sizeDumped);
                    break;
                default:
                    break;
            }
        }

        private int responseGetEntryTableItems(DataPacketDownloadlist receivedDataPacket)
        {
            if (receivedDataPacket == null ||
                receivedDataPacket.Command == null ||
                receivedDataPacket.Command.Data == null ||
                receivedDataPacket.Command.Data.Length < 3) return -1;

            Byte[] data = receivedDataPacket.Command.Data;

            byte[] temp = new byte[2];
            temp[0] = data[2];
            int numOfEntryItemReturned = Converter.BytesToUShort(temp, 0);
            if (numOfEntryItemReturned == 0) return 0;

            System.Diagnostics.Debug.Assert((data.Length - 3) % 16 == 0);
            numOfEntryItemReturned = (data.Length - 3) / 16;

            int validEntryItemCount = 0;
            int pos = 3;
            for (int i = 0; i < numOfEntryItemReturned; i++)
            {
                if (pos + 16 > data.Length) break;

                // check CRC error校验需要修改
                Crc16 crc16 = new Crc16();
                byte[] temp2 = new byte[6];
                // start CRC
                Array.Copy(data, pos, temp2, 0, 6);
                byte[] checkSumCalc = crc16.ComputeChecksumBytes(temp2);
                if ((checkSumCalc[0] != data[pos + 6]) || (checkSumCalc[1] != data[pos + 7]))
                {
                    pos += 16;
                    continue;
                }
                // end CRC
                Array.Copy(data, pos + 8, temp2, 0, 6);
                checkSumCalc = crc16.ComputeChecksumBytes(temp2);
                if ((checkSumCalc[0] != data[pos + 14]) || (checkSumCalc[1] != data[pos + 15]))
                {
                    pos += 16;
                    continue;
                }

                // check if the last 8 bytes is 0xFF's
                //bool isLastItem = true;
                //for (int j = pos + 8; j < pos + 16; j++)
                //{
                //    if (data[j] != 0xFF)
                //    {
                //        isLastItem = false;
                //        break;
                //    }
                //}
                //if (isLastItem == true)
                //{
                //    break;
                //}

                EntryItem entryItem = new EntryItem();
                entryItem.StartTime = Converter.BytesToUInt(data, pos);
                pos += 4;
                entryItem.FatItemNum = Converter.BytesToUShort(data, pos);
                pos += 2;
                entryItem.CrcStart = Converter.BytesToUShort(data, pos);
                pos += 2;
                entryItem.EndTime = Converter.BytesToUInt(data, pos);
                pos += 4;
                entryItem.NumOfPages = Converter.BytesToUShort(data, pos);
                pos += 2;
                entryItem.CrcEnd = Converter.BytesToUShort(data, pos);
                pos += 2;

                // check EndTime for validality
                DateTime dateTime = new DateTime(1970, 1, 1);
                dateTime = dateTime.AddSeconds(entryItem.EndTime);
                if (dateTime.Year <= 2050)
                {
                    if (entryItemList == null) entryItemList = new List<EntryItem>();
                    entryItemList.Add(entryItem);
                    validEntryItemCount++;
                }
                //else
                //{
                //    int stop = 2;
                //}
            }

            return numOfEntryItemReturned;
            //return validEntryItemCount;
        }
        private int responseGetFatTableItems(DataPacketDownloadlist receivedDataPacket)
        {
            if (receivedDataPacket == null ||
                receivedDataPacket.Command == null ||
                receivedDataPacket.Command.Data == null ||
                receivedDataPacket.Command.Data.Length < 3) return -1;

            byte[] data = receivedDataPacket.Command.Data;

            byte[] temp = new byte[2];
            temp[0] = data[2];
            int numOfFatItemReturned = Converter.BytesToUShort(temp, 0);
            if (numOfFatItemReturned == 0) return 0;

            System.Diagnostics.Debug.Assert((data.Length - 3) % 8 == 0);
            numOfFatItemReturned = (data.Length - 3) / 8;   // should be = 1

            int pos = 3;
            if (pos + 8 > data.Length) return 0;
            FatItem fatItem = new FatItem();
            fatItem.StartTime = Converter.BytesToUInt(data, pos);
            pos += 4;
            fatItem.Address = Converter.BytesToUInt(data, pos);

            startAddress = fatItem.Address;

            return numOfFatItemReturned;
        }
        private int responseToolData(DataPacketDownloadlist receivedDataPacket)//请求数据回应
        {
            if (receivedDataPacket == null ||
                receivedDataPacket.Command == null ||
                receivedDataPacket.Command.Data == null ||
                receivedDataPacket.Command.Data.Length == 0) return -1;

            byte[] data = receivedDataPacket.Command.Data;

            // start address (4 bytes), length (2 bytes), see protocol
            Array.Copy(data, 6, dumpedData, curDataPos, data.Length - 6);
            //curDataPos += data.Length - 6;

            return data.Length - 6;
        }
        private void refreshDisplay()
        {
            listViewRuns.Items.Clear();

            if (entryItemList == null || entryItemList.Count == 0) return;

            ListViewItem item;
            int entryNum = 0;
            foreach (EntryItem entryItem in entryItemList)
            {
                entryNum++;
                item = new ListViewItem(entryNum.ToString());
                listViewRuns.Items.Add(item);

                item.SubItems.Add(Converter.SecondsToLongTime(entryItem.StartTime, false, true));
                item.SubItems.Add(Converter.SecondsToLongTime(entryItem.EndTime, false, true));
                item.SubItems.Add((entryItem.EndTime - entryItem.StartTime).ToString());
                item.SubItems.Add((entryItem.NumOfPages * pageSize).ToString());

                //String line = String.Format("{0} {1} {2} {3}", entryNum, entryItem.StartTime, entryItem.EndTime, entryItem.NumOfPages * 256);
                //System.Diagnostics.Debug.WriteLine(line);
                //BWUtility.DebugLog(line);
                //String debugMsg = "Info: BWMemoryDumpDlg::refreshDisplay(): " + line;
                //BWUtility.DebugLog(debugMsg);
            }
        }
        private void updateProgressBar(int sizeDumped)
        {
            // set prograss bar
            this.progressBar1.Value = sizeDumped * 100 / totalSizeToDump;// marker,0-1

            TimeSpan ts = DateTime.Now - dumpStartTime;
            double speed = (double)sizeDumped / ts.TotalSeconds;                 // bytes/second
            int remainingTime = (int)((totalSizeToDump - sizeDumped) / speed);   // seconds
            String msg = String.Format("速度{0}byte/s,剩余{1}min{2}s", (int)speed * 8, remainingTime / 60, remainingTime % 60);
            progressBar1.Text = msg;
        }

        private void button_dump_Click(object sender, EventArgs e)
        {
            if (entryItemList == null || entryItemList.Count == 0) return;

            //// diagnostic
            //String msg = "buttonDump_Click";
            //System.Diagnostics.Debug.WriteLine(msg);

            if (listViewRuns.SelectedIndices == null || listViewRuns.SelectedIndices.Count == 0)
            {
                MessageBox.Show(MyStrings.String_Please_select_a_run_number);// marker
                return;
            }

            //// diagnostic
            //System.Diagnostics.Debug.WriteLine("entryItemList.Count = " + entryItemList.Count);
            //System.Diagnostics.Debug.WriteLine("selected entries:");

            entryItemSelectedIndexList = new List<int>();
            for (int i = 0; i < listViewRuns.SelectedIndices.Count; i++)
            {
                int index = listViewRuns.SelectedIndices[i];
                if (index >= 0 && index < entryItemList.Count)
                {
                    entryItemSelectedIndexList.Add(index);
                }
            }

            isDiagnostic = false;

            totalSizeToDump = calculateTotalMemorySizeToDoump();
            sizeDumped = 0;
            dumpStartTime = DateTime.Now;

            clear();
            progressBar1.Value = 0;// 0-1
            curEntryItemPos = 0;
            dumpMemory(entryItemSelectedIndexList[curEntryItemPos]);
        }
        private void dumpMemory(int entryItemNum)
        {
            // diagnostic
            System.Diagnostics.Debug.WriteLine("Dump memory, entry #: " + entryItemNum);

            startFatItemNum = entryItemList[entryItemNum].FatItemNum;
            requestGetFatTableItems();
        }
        /// <summary>
        /// clear memoryDataList and timeDatasetList
        /// </summary>
        private void clear()
        {
            if (memoryDataRunList == null) memoryDataRunList = new List<MemoryDataOneRun>();
            memoryDataRunList.Clear();

            if (timeDatasetList == null) timeDatasetList = new List<Dataset>();
            timeDatasetList.Clear();

            if (timeRawDatasetList == null) timeRawDatasetList = new List<Dataset>();
            timeRawDatasetList.Clear();
        }
        /// <summary>
        /// calculates the total size of the selected memory data to dump.
        /// used for progress bar
        /// </summary>
        /// <returns></returns>
        private int calculateTotalMemorySizeToDoump()
        {
            if (entryItemList == null || entryItemList.Count == 0) return 0;
            //if (fatItemList == null || fatItemList.Count == 0) return 0;

            int totalSize = 0;
            foreach (int k in entryItemSelectedIndexList)
            {
                totalSize += calculateTotalMemorySizeToDoump(k);
            }

            return totalSize;
        }
        private int calculateTotalMemorySizeToDoump(int k)
        {
            if (entryItemList == null || k >= entryItemList.Count) return 0;

            int size = entryItemList[k].NumOfPages * pageSize;

            return size;
        }
    }


    public class DownloadListItem
    {
        // 下载列表数据
        public string Number { get; set; }
        public string Starttime { get; set; }
        public string Endtime { get; set; }
        public string Timeinterval { get; set; }
        public string Bytes { get; set; }
    }
    public struct EntryItem
    {
        public uint StartTime;
        public ushort FatItemNum;
        public ushort CrcStart;
        public uint EndTime;
        public ushort NumOfPages;
        public ushort CrcEnd;
    }
    public struct FatItem
    {
        public uint StartTime;
        public uint Address;
    }
    public struct MemoryDataPacket
    {
        // 1  2  3  4  5  6  7  8  9  10 11 12
        //       time        d  s  len   o  pr data   crc
        // 5A 5A XX XX XX XX XX XX XX XX XX XX ...... XX XX
        public byte[] DataBytes;

        public bool IsValid;
        public String Time;
        public EnumToolAddress Type;
        public EnumBoardAddress SubType;
        public double[] RawData;
        public double[] CalcData;
        public String ErrMsg;
        public MemoryDataPacket(byte[] data, int start, int length)
        {
            if (data == null || data.Length == 0 ||
                start < 0 || length <= 0 || start + length >= data.Length) DataBytes = null;
            else
            {
                DataBytes = new byte[length];
                Array.Copy(data, start, DataBytes, 0, length);
            }

            BytesToData(DataBytes, out IsValid, out Time, out Type, out SubType, out RawData, out CalcData, out ErrMsg);
        }
        private static void BytesToData(byte[] dataBytes, out bool isValid, out String time, out EnumToolAddress type,
            out EnumBoardAddress subType, out double[] rawData, out double[] calcData, out String errMsg)
        {
            isValid = false;
            time = String.Empty;
            type = EnumToolAddress.All;
            subType = EnumBoardAddress.AllBoards;
            rawData = null;
            calcData = null;
            errMsg = String.Empty;

            if (dataBytes == null || dataBytes.Length == 0) return;
            if (dataBytes.Length < 6 + 8) return;

            //       1  2  3  4  5  6  7  8  9  10
            // 5A 5A XX XX XX XX XX XX XX XX XX XX data XX XX
            //       time        d  s  len   o  pr      crc
            // check src tool address
            byte toolAddr = dataBytes[7];   // 0x5A 0x5A + 4 byte time + 1 byte dst addr (memory 1 or 2)
            byte len = dataBytes[8];        // 0x34, 0x48, 0x0C
            byte obj = dataBytes[10];       // 0x87
            int dataLength = 0;

            if (toolAddr == ToolBoardAddress.GetToolBoardAddress(EnumToolAddress.BWNB, EnumBoardAddress.NBBatteryControl))
            {// marker
                //if (len != (BWLWDToolsNew.NearBitTool.Status.NumOfBytesInMem + 8) || obj != 0x87)
                //{
                //    isValid = false;
                //    errMsg = "Length doesn't match! (NBBC, 0x2B)";
                //    return;
                //}
                //dataLength = BWLWDToolsNew.NearBitTool.Status.NumOfBytesInMem + 12;
                //type = BWEnumToolAddress.BWNB;
                //subType = BWEnumBoardAddress.NBBatteryControl;
            }

            if (dataLength > dataBytes.Length - 2)
            {   // reaches end of data
                isValid = false;
                errMsg = "Data length incorrect! (Mwd 58, Res 78, Gam 18)";
                return;
            }

            int seconds = Converter.BytesToInt(dataBytes, 2);
            time = Converter.SecondsToLongTime(seconds, false, true);
            byte[] dataPacketBytes = new byte[dataLength - 4];
            Array.Copy(dataBytes, 6, dataPacketBytes, 0, dataLength - 4);
            DataPacketGSC dataPacket = new DataPacketGSC(dataPacketBytes);// marker

            if (dataPacket.isValidData() == true)
            {
                isValid = true;

                if (type == EnumToolAddress.BWNB)
                {// marker
                    //rawData = BWLWDToolsNew.NearBitTool.ProcessMemoryData(subType, seconds, dataPacket.Command.Data, dataPacket.Command.Header.Return);
                    //if (rawData != null && rawData.Length > 0)
                    //{
                    //    calcData = new double[rawData.Length];
                    //    Array.Copy(rawData, calcData, rawData.Length);
                    //}
                }
            }
        }
    }
    public static class ProcessMemoryData
    {
        public static List<MemoryDataPacket> DataPacketList;
        public static List<BoardInfo> BoardInfoList;
        public static List<BoardParam> BoardParamList;
        /// <summary>
        /// The input should be dumped from a single run memory data.
        /// It will create and append the memory data in a datasetList,
        /// which includes 8 resistivity, 7 mwd and 1 gamma curves.
        /// 
        /// ===============================================================
        ///                 DataLength  ActualLength    #ofRawData  #ofData
        /// Mwd             56          44              11          7
        /// Res             76          64              32          8
        /// Gamma           16          4               1           1
        /// NearBit
        ///     Status      47          35              16          16
        ///     Res         38          26              11          3
        ///     DirGA       32          20              10          3
        /// NearBit RX      100         88              22          22
        /// Pwd             36          24              6           6
        /// DirRes          122         110             45
        /// Nuclear
        ///     Neutron     140         128             32
        ///     Sonic       44          32              14
        ///     Density     236         224             72
        /// RSS             72          60              15
        /// =================================================================
        /// 
        /// </summary>
        /// <param name="dumpedData"></param>
        /// <returns>all the curves in a single run</returns>
        public static List<Dataset> ProcessDumpedData(byte[] dumpedData)
        {
            if (dumpedData == null || dumpedData.Length == 0) return null;

            List<Dataset> datasetList = new List<Dataset>();
            DataPacketList = new List<MemoryDataPacket>();
            BoardInfoList = new List<BoardInfo>();
            BoardParamList = new List<BoardParam>();

            int curPos = 0;
            while (curPos < dumpedData.Length - 2)
            {
                int startPos = findSeparators(dumpedData, curPos);
                startPos += 2;

                if (startPos + 10 >= dumpedData.Length)
                {   // reaches end of data
                    break;
                }

                // marker 此处要修改为新数据包样式，理论上是 5A 5A TIME(共4) 起始符 功能码 数据 CRC校验(共7)
                //       1  2  3  4  5  6  7  8  9  10
                // 5A 5A XX XX XX XX XX XX XX XX XX XX data XX XX
                //       time        d  s  len   o  pr      crc
                // check src tool address
                byte toolAddr = dumpedData[startPos + 5];   // 4 byte time + 1 byte dst addr (memory 1 or 2)
                byte len = dumpedData[startPos + 6];        // 
                byte obj = dumpedData[startPos + 8];        // 0x87
                byte param = dumpedData[startPos + 9];      // 
                int dataLength = 0;

                //if (obj == 0x05)
                //{   // board info
                //    int boardInfoDataLength = 22;
                //    if (len != (boardInfoDataLength + 8))
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = boardInfoDataLength + 12;
                //}
                //else if (toolAddr == ToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.MWD, BWEnumBoardAddress.MWD))
                //{
                //    if (len != (BWLWDToolsNew.MwdTool.NumOfBytesInMem + 8) || obj != 0x87) // 国家项目
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.MwdTool.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.Resisitivity, BWEnumBoardAddress.DataProcess))
                //{
                //    if (len != (BWLWDToolsNew.ResTool.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.ResTool.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.Gamma, BWEnumBoardAddress.Gamma))
                //{
                //    if (len != (BWLWDToolsNew.GaTool.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.GaTool.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBBatteryControl))
                //{
                //    if (len != (BWLWDToolsNew.NearBitTool.Status.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.NearBitTool.Status.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.DataProcess))
                //{
                //    if (len != (BWLWDToolsNew.NearBitTool.ResData.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.NearBitTool.ResData.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWNB, BWEnumBoardAddress.NBAzimuthalGamma))
                //{
                //    if (len != (BWLWDToolsNew.NearBitTool.GaData.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.NearBitTool.GaData.NumOfBytesInMem + 12;
                //}
                if (toolAddr == ToolBoardAddress.GetToolBoardAddress(EnumToolAddress.BWNB, EnumBoardAddress.NBShorthopRX))
                {// marker 长度等要注意
                    if (len != 7 || obj != 0x87)
                    {
                        curPos++;
                        continue;
                    }
                    dataLength = 7 + 4;
                }
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWPV, BWEnumBoardAddress.PressVib))
                //{
                //    if (len != (BWPwdData.NumOfBytesInMem + 8) || obj != 0x87) // 24.12.8 PWD下载
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWPwdData.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWRX, BWEnumBoardAddress.DataProcess))
                //{
                //    if (len != (BWLWDToolsNew.DirResTool.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.DirResTool.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWDN, BWEnumBoardAddress.DNNeutronDAQ))
                //{
                //    if (param == 0x30)
                //    {   // correction setting
                //        if (len != (BWNeutronCorrectionSetting.NumOfBytes + 8) || (obj != 0x87 && obj != 0x07))
                //        {
                //            curPos++;
                //            continue;
                //        }
                //        dataLength = BWNeutronCorrectionSetting.NumOfBytes + 12;
                //    }
                //    // nuclear master data on master/second memory
                //    else if (param == 0x40)
                //    {
                //        if (len != (BWLWDToolsNew.NuclearTool.MasterData.NumOfBytesInMem + 8) || obj != 0x87)
                //        {
                //            curPos++;
                //            continue;
                //        }
                //        dataLength = BWLWDToolsNew.NuclearTool.MasterData.NumOfBytesInMem + 12;
                //    }
                //    // neutron bin  data on nuclear memory
                //    else if (param == 0xA0)
                //    {
                //        if (len != (BWLWDToolsNew.NuclearTool.NeutronBin.NumOfBytesInMem + 8) || obj != 0x87)
                //        {
                //            curPos++;
                //            continue;
                //        }

                //        dataLength = BWLWDToolsNew.NuclearTool.NeutronBin.NumOfBytesInMem + 12;
                //    }
                //    else
                //    {
                //        curPos++;
                //        continue;
                //    }
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWDN, BWEnumBoardAddress.DNSonicDAQ))
                //{
                //    if (param == 0x30)
                //    {   // correction setting
                //        if (len != (BWSonicCorrectionSetting.NumOfBytes + 8) || (obj != 0x87 && obj != 0x07))
                //        {
                //            curPos++;
                //            continue;
                //        }
                //        dataLength = BWSonicCorrectionSetting.NumOfBytes + 12;
                //    }
                //    else if (param == 0xA0)
                //    {
                //        if (len != (BWLWDToolsNew.NuclearTool.SonicBin.NumOfBytesInMem + 8) || obj != 0x87)
                //        {
                //            curPos++;
                //            continue;
                //        }
                //        dataLength = BWLWDToolsNew.NuclearTool.SonicBin.NumOfBytesInMem + 12;
                //    }
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWDN, BWEnumBoardAddress.DNDensityDAQ))
                //{
                //    if (param == 0x30)
                //    {   // correction setting
                //        if (len != (BWDensityCorrectionSetting.NumOfBytes + 8) || (obj != 0x87 && obj != 0x07))
                //        {
                //            curPos++;
                //            continue;
                //        }
                //        dataLength = BWDensityCorrectionSetting.NumOfBytes + 12;
                //    }
                //    else if (param == 0xA0)
                //    {
                //        if (len != (BWLWDToolsNew.NuclearTool.DensityBin.NumOfBytesInMem + 8) || obj != 0x87)
                //        {
                //            curPos++;
                //            continue;
                //        }
                //        dataLength = BWLWDToolsNew.NuclearTool.DensityBin.NumOfBytesInMem + 12;
                //    }
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWRS, BWEnumBoardAddress.RSShorthop))
                //{
                //    // RSS shorthop data on master/second memory
                //    if (len != (BWLWDToolsNew.RssTool.Shorthop.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.RssTool.Shorthop.NumOfBytesInMem + 12;
                //}
                //else if (toolAddr == BWToolBoardAddress.GetToolBoardAddress(BWEnumToolAddress.BWRS, BWEnumBoardAddress.RSDownLink))
                //{
                //    // RSS downlink data on RSS memory itself
                //    if (len != (BWLWDToolsNew.RssTool.Downlink.NumOfBytesInMem + 8) || obj != 0x87)
                //    {
                //        curPos++;
                //        continue;
                //    }
                //    dataLength = BWLWDToolsNew.RssTool.Downlink.NumOfBytesInMem + 12;
                //}
                else
                {   // tool address incorrect, move curPos, continue
                    curPos++;
                    continue;
                }

                if (startPos + dataLength >= dumpedData.Length)
                {   // reaches end of data
                    break;
                }
                int endPos = startPos + dataLength;

                byte[] temp = new byte[endPos - startPos];
                Array.Copy(dumpedData, startPos, temp, 0, endPos - startPos);

                int time = Converter.BytesToInt(temp, 0);
                byte[] dataPacketBytes = new byte[temp.Length - 4];
                Array.Copy(temp, 4, dataPacketBytes, 0, temp.Length - 4);// 去掉时间

                // time consuming process
                processSingleDataPacket(dataPacketBytes, time, datasetList);
                curPos = endPos;

                //String msg = String.Format("curPos = {0}, {1}, {2}, {3}",
                //    curPos, datasetList[0].TotalRecords, datasetList[1].TotalRecords, datasetList[10].TotalRecords);
                //System.Diagnostics.Debug.WriteLine(msg);

                if (startPos - 2 >= 0)
                {   // create raw data list
                    //byte[] temp2 = new byte[dataLength + 2];        // 5A 5A + temp 
                    //Array.Copy(dumpedData, startPos - 2, temp2, 0, dataLength + 2);
                    // time consuming process
                    MemoryDataPacket packet = new MemoryDataPacket(dumpedData, startPos - 2, dataLength + 2);
                    DataPacketList.Add(packet);
                }
            }

            //String line = BWConverter.BytesToHexString(dumpedData);
            //line = BWConverter.AddSpacesToHexString(line);
            //System.Diagnostics.Debug.WriteLine(line);

            return datasetList;
        }
        /// <summary>
        /// find the index of the separator 0x5A0x5A in a byte array, starting at startPos
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startPos"></param>
        /// <returns>if none found, the returned index == startPos</returns>
        public static int findSeparators(byte[] data, int startPos)
        {
            int curPos = startPos;
            while (curPos < data.Length - 2)
            {
                if (data[curPos] != 0x5A || data[curPos + 1] != 0x5A)
                {
                    curPos++;
                    continue;
                }

                break;
            }

            return curPos;
        }
        /// <summary>
        /// It will append one packet data to corresponding dataset
        /// Resistivity and Mwd data are logged in a file, GA data is not logged
        /// </summary>
        /// <param name="dataPacketBytes">data in a single dataPacket, without time</param>
        /// <param name="time"></param>
        /// <param name="datasetList"></param>
        private static void processSingleDataPacket(byte[] dataPacketBytes, int time, List<Dataset> datasetList)
        {
            if (dataPacketBytes == null || dataPacketBytes.Length != 7) return;
            if (datasetList == null) return;

            DataPacketGSC dataPacket = new DataPacketGSC(dataPacketBytes);// 新协议
            String line = dataPacket.ToHexString();

            if (dataPacket.isValidData() == false)
            {
                System.Diagnostics.Debug.WriteLine("Invalid " + line);

                String msg = MyStrings.String_Invalid_memroy_data_packet + " " + dataPacket.ToHexString();
                //if (ShowMessageBox == true)
                //{
                //    MessageBox.Show(msg, MyStrings.String_Invalid_memroy_data, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    ShowMessageBox = false;
                //}

                String debugMsg = "Info: BWProcessMemoryData::processSingleDataPacket(): " + msg;
                Utility.DebugLog(debugMsg);

                return;
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Valid datapacket: " + line);
            }

            /*byte srcAddr = dataPacket.SrcAddr;
            BWToolBoardAddress address = new BWToolBoardAddress(srcAddr);
            BWToolCommand cmd = dataPacket.Command;
            BWCommandHeader header = cmd.Header;
            int ret = header.Return;
            int len = dataPacket.PayLoad;
            byte[] data = cmd.Data;

            //System.Diagnostics.Debug.Assert((data != null) && (data.Length == len - 8));
            if (header.CmdObject == BWCommandHeader.BWEnumCmdObject.BoardInfo)
            {
                BWBoardInfo info = new BWBoardInfo(data);
                BoardInfoList.Add(info);

                return;
            }

            if (address.ToolAddress == EnumToolAddress.BWNB)
            {
                double[] gscData = BWLWDToolsNew.MwdTool.ProcessMemoryData(time, data, ret);// 这是解析出的数据的数量，暂时不明白为啥要在这里解析一次（这里不是保存文件吗？）
                System.Diagnostics.Debug.Assert(mwdData != null && mwdData.Length == 13);
                if (mwdData == null || mwdData.Length != 13) return;
                appendMwdData(time, mwdData, ret, datasetList);
            }*/
        }
    }
}

