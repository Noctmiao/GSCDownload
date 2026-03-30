using AntdUI;
using LsySkin;
using MultiPort;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GuidanceStsbilityCommsDownLoad.Communication;
using static System.Resources.ResXFileRef;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GuidanceStsbilityCommsDownLoad
{
    public partial class GSCDownload : AntdUI.Window
    {

        public bool IsWaitingForResponse;
        private DataPacketDownloadlist sentDataPacket;
        // communication
        private EnumCommunication communication;
        private CommunicationObject commObj;

        private AntList<DownloadListItem> downloadListItems;

        //datapacket length every time
        public int maxLength;
        public int tryLength;

        // memory structure
        private const int maxNumOfEntryItems = 10;
        private ushort startEntryItemNum = 0;
        private List<EntryItem> entryItemList;
        private List<FatItem> fatItemList;

        // memory info
        private uint memorySize;
        private uint memoryUsed;

        Timer timerReceive;


        public GSCDownload()
        {
            InitializeComponent();

            // load
            startEntryItemNum = 0;
            entryItemList = new List<EntryItem>();
            fatItemList = new List<FatItem>();

            button_dump.Enabled = false;
            button_clear.Enabled = false;
            timerReceive = new Timer();
            timerReceive.Interval = 100;
            timerReceive.Tick += new System.EventHandler(timer1_Tick);

            // table init
            Init_listView_ResistivityTool_downloadlist();
            // antdui init
            Config.IsLight = true;
            BackColor = Color.White;
            Config.ShowInWindow = true;

        }

        private void Init_listView_ResistivityTool_downloadlist()
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
        }
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
            downloadListItems.Clear();

            startEntryItemNum = 0;
            entryItemList.Clear();
            fatItemList.Clear();
            requestMemoryInfo();

            button_dump.Enabled = false;
            button_clear.Enabled = false;
        }
        private void requestMemoryInfo()
        {
            byte dst = ToolBoardAddress.GetToolBoardAddress(0, 0);// 接收的src
            int param = (int)(ToolSpecificObject.EnumMemoryParameter.GetMemoryInfo);
            byte[] data = null;

            CommandHeader.EnumCmdObject obj = CommandHeader.EnumCmdObject.ToolSpecific;
            //if (whichMemory == 2 || whichMemory == 6 || whichMemory == 0) obj = BWCommandHeader.BWEnumCmdObject.NearBitMemory;// 修改,obj会改变第5位 // 2.修改源地址：国家项目源地址
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
            byte dst = ToolBoardAddress.GetToolBoardAddress(0, 0);
            int param = (int)(ToolSpecificObject.EnumMemoryParameter.GetEntryTableItems);

            byte[] data = new byte[3];
            byte[] temp = Converter.UShortToBytes(startEntryItemNum);
            Array.Copy(temp, 0, data, 0, 2);
            temp = Converter.UShortToBytes(maxNumOfEntryItems);
            data[2] = temp[0];

            CommandHeader.EnumCmdObject obj = CommandHeader.EnumCmdObject.ToolSpecific;
            //if (whichMemory == 2 || whichMemory == 6 || whichMemory == 0) obj = BWCommandHeader.BWEnumCmdObject.NearBitMemory;// 2.修改源地址：国家项目源地址（获取列表地址）
            sentDataPacket = DataPacketDownloadlist.GenerateDataPacket(dst, obj, param, data);

            IsWaitingForResponse = true;
            Send(sentDataPacket.ToBytes());
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
            if (address.ToolAddress == EnumToolAddress.All)
            {
                    NearBitTool.ProcessDataPacket(dataPacket);
                UpdateDisplayNearBitData();
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
            timerReceive.Enabled = false;

            int param = receivedDataPacket.Command.Header.Parameter;

            // special handling for get RTC time
            if (receivedDataPacket.Command.Header.CmdObject == CommandHeader.EnumCmdObject.RTClock && param == (int)RTClockObject.RTClockParameter.GetRTClock)
            {
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
                case (int)(ToolSpecificObject.EnumMemoryParameter.GetMemoryInfo):
                    responseMemoryInfo(receivedDataPacket);

                    startEntryItemNum = 0;
                    if (entryItemList != null) entryItemList.Clear();
                    requestGetEntryTableItems();
                    break;
                //case (int)(BWToolSpecificObject.BWEnumMemoryParameter.GetEntryTableItems):
                //    if (responseGetEntryTableItems(receivedDataPacket) == maxNumOfEntryItems)
                //    {
                //        startEntryItemNum += maxNumOfEntryItems;
                //        requestGetEntryTableItems();
                //    }
                //    else
                //    {
                //        refreshDisplay();
                //        System.Diagnostics.Debug.WriteLine("Entry table has " + entryItemList.Count + " items!");
                //        buttonDump.Enabled = true;
                //        buttonClear.Enabled = true;
                //    }
                //    break;
                //case (int)(BWToolSpecificObject.BWEnumMemoryParameter.GetFatTableItems):
                //    if (responseGetFatTableItems(receivedDataPacket) != 1)
                //    {
                //        ;
                //    }
                //    else
                //    {
                //        dumpSize = (uint)entryItemList[entryItemSelectedIndexList[curEntryItemPos]].NumOfPages * pageSize;
                //        if (isDiagnostic == true) dumpSize = 2 * pageSize;      // all board info size = 333 bytes (2+4+31) * 9
                //        curDataPos = 0;
                //        dumpedData = new byte[dumpSize];

                //        // dump memory data from start to end memory addresses
                //        ushort requestDumpSize = maxDumpSize;
                //        if (dumpSize < maxDumpSize) requestDumpSize = (ushort)dumpSize;
                //        requestToolData(startAddress, requestDumpSize);
                //    }
                //    break;
                //case (int)(BWToolSpecificObject.BWEnumMemoryParameter.GetToolData):
                //    int actualDumpedSize = responseToolData(receivedDataPacket);
                //    curDataPos += actualDumpedSize;
                //    if (curDataPos >= dumpSize)
                //    {   // reaches the end of this entry item, process dumped memory data and dump next entry item;
                //        System.Diagnostics.Debug.WriteLine("Finish dump entry: " + entryItemSelectedIndexList[curEntryItemPos]);
                //        if (isDiagnostic == false) processDumpedDataOneRun();
                //        else processDiagnosticDataOneRun();

                //        //// set prograss bar
                //        //sizeDumped += calculateTotalMemorySizeToDoump(entryItemSelectedIndexList[curEntryItemPos]);
                //        //this.progressBar1.Value = sizeDumped * 100 / totalSizeToDump;

                //        curEntryItemPos++;
                //        if (curEntryItemPos >= entryItemSelectedIndexList.Count)
                //        {   // reaches the end of this dump
                //            updateProgressBar(totalSizeToDump);
                //            //IsWaitingForResponse = false;
                //            MessageBox.Show(MyStrings.String_Memory_dump_finished_successfully);
                //            return;
                //        }
                //        else
                //        {   // dump the next entry item
                //            dumpMemory(entryItemSelectedIndexList[curEntryItemPos]);
                //        }
                //    }
                //    else
                //    {
                //        //System.Diagnostics.Debug.WriteLine("curDataPos: " + curDataPos + ", total size: " + dumpSize);
                //        requestToolData((uint)(startAddress + curDataPos), maxDumpSize);
                //    }

                //    // set prograss bar
                //    sizeDumped += actualDumpedSize;
                //    updateProgressBar(sizeDumped);
                //    break;
                default:
                    break;
            }
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
}

