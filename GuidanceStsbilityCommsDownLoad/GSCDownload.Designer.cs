namespace GuidanceStsbilityCommsDownLoad
{
    partial class GSCDownload
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            AntdUI.Tabs.StyleLine styleLine1 = new AntdUI.Tabs.StyleLine();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GSCDownload));
            this.pageHeader1 = new AntdUI.PageHeader();
            this.tabs_menu = new AntdUI.Tabs();
            this.download = new AntdUI.TabPage();
            this.panel1 = new AntdUI.Panel();
            this.listViewRuns = new System.Windows.Forms.ListView();
            this.colRunNumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStartTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEndTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBar1 = new AntdUI.Progress();
            this.progress_memoryused = new AntdUI.Progress();
            this.flowLayoutPanel_head = new AntdUI.In.FlowLayoutPanel();
            this.pageHeader2 = new AntdUI.PageHeader();
            this.label_CAN = new AntdUI.Label();
            this.button_BlueCAN = new AntdUI.Button();
            this.button_readmemory = new AntdUI.Button();
            this.button_dump = new AntdUI.Button();
            this.button_clear = new AntdUI.Button();
            this.exportdata = new AntdUI.TabPage();
            this.listViewRawData = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gridPanel1 = new AntdUI.GridPanel();
            this.comboBoxRawOrCalcData = new AntdUI.Select();
            this.label4 = new AntdUI.Label();
            this.textBoxEnd = new AntdUI.Input();
            this.label3 = new AntdUI.Label();
            this.comboBoxSrcToolFilter = new AntdUI.Select();
            this.label1 = new AntdUI.Label();
            this.textBoxStart = new AntdUI.Input();
            this.label2 = new AntdUI.Label();
            this.gridPanel2 = new AntdUI.GridPanel();
            this.buttonExportToExcel2 = new AntdUI.Button();
            this.buttonRawFile = new AntdUI.UploadDragger();
            this.pageHeader3 = new AntdUI.PageHeader();
            this.tabs_menu.SuspendLayout();
            this.download.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel_head.SuspendLayout();
            this.pageHeader2.SuspendLayout();
            this.exportdata.SuspendLayout();
            this.gridPanel1.SuspendLayout();
            this.gridPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageHeader1
            // 
            this.pageHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pageHeader1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.pageHeader1.Location = new System.Drawing.Point(0, 0);
            this.pageHeader1.Name = "pageHeader1";
            this.pageHeader1.ShowButton = true;
            this.pageHeader1.Size = new System.Drawing.Size(636, 35);
            this.pageHeader1.SubText = "GSCDownload";
            this.pageHeader1.TabIndex = 0;
            this.pageHeader1.Text = "全旋转内存下载";
            // 
            // tabs_menu
            // 
            this.tabs_menu.Controls.Add(this.download);
            this.tabs_menu.Controls.Add(this.exportdata);
            this.tabs_menu.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabs_menu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs_menu.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.tabs_menu.Location = new System.Drawing.Point(0, 35);
            this.tabs_menu.Name = "tabs_menu";
            this.tabs_menu.Pages.Add(this.download);
            this.tabs_menu.Pages.Add(this.exportdata);
            this.tabs_menu.Size = new System.Drawing.Size(636, 601);
            this.tabs_menu.Style = styleLine1;
            this.tabs_menu.TabIndex = 1;
            this.tabs_menu.Text = "tabs1";
            // 
            // download
            // 
            this.download.Controls.Add(this.panel1);
            this.download.Controls.Add(this.flowLayoutPanel_head);
            this.download.Dock = System.Windows.Forms.DockStyle.Fill;
            this.download.Location = new System.Drawing.Point(0, 36);
            this.download.Name = "download";
            this.download.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.download.Size = new System.Drawing.Size(636, 565);
            this.download.TabIndex = 0;
            this.download.Text = "下载";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listViewRuns);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.progress_memoryused);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 49);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(636, 516);
            this.panel1.TabIndex = 1;
            this.panel1.Text = "panel1";
            // 
            // listViewRuns
            // 
            this.listViewRuns.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colRunNumber,
            this.colStartTime,
            this.colEndTime,
            this.colDuration,
            this.colSize});
            this.listViewRuns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewRuns.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listViewRuns.FullRowSelect = true;
            this.listViewRuns.HideSelection = false;
            this.listViewRuns.Location = new System.Drawing.Point(10, 44);
            this.listViewRuns.Name = "listViewRuns";
            this.listViewRuns.Size = new System.Drawing.Size(616, 439);
            this.listViewRuns.TabIndex = 13;
            this.listViewRuns.UseCompatibleStateImageBehavior = false;
            this.listViewRuns.View = System.Windows.Forms.View.Details;
            // 
            // colRunNumber
            // 
            this.colRunNumber.Text = "编号";
            this.colRunNumber.Width = 66;
            // 
            // colStartTime
            // 
            this.colStartTime.Text = "开始时间";
            this.colStartTime.Width = 140;
            // 
            // colEndTime
            // 
            this.colEndTime.Text = "终止时间";
            this.colEndTime.Width = 140;
            // 
            // colDuration
            // 
            this.colDuration.Text = "时间间隔（秒）";
            this.colDuration.Width = 100;
            // 
            // colSize
            // 
            this.colSize.Text = "占用字节";
            this.colSize.Width = 93;
            // 
            // progressBar1
            // 
            this.progressBar1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar1.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.progressBar1.Location = new System.Drawing.Point(10, 10);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.progressBar1.Size = new System.Drawing.Size(616, 34);
            this.progressBar1.Steps = 27;
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Text = "速度000byte/s,剩余00min00s";
            this.progressBar1.UseSystemText = true;
            this.progressBar1.UseTextCenter = true;
            // 
            // progress_memoryused
            // 
            this.progress_memoryused.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.progress_memoryused.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progress_memoryused.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.progress_memoryused.Location = new System.Drawing.Point(10, 483);
            this.progress_memoryused.Name = "progress_memoryused";
            this.progress_memoryused.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.progress_memoryused.Size = new System.Drawing.Size(616, 23);
            this.progress_memoryused.TabIndex = 1;
            this.progress_memoryused.Text = "已用内存";
            this.progress_memoryused.TextUnit = "";
            this.progress_memoryused.UseSystemText = true;
            // 
            // flowLayoutPanel_head
            // 
            this.flowLayoutPanel_head.AutoSize = true;
            this.flowLayoutPanel_head.Controls.Add(this.pageHeader2);
            this.flowLayoutPanel_head.Controls.Add(this.button_readmemory);
            this.flowLayoutPanel_head.Controls.Add(this.button_dump);
            this.flowLayoutPanel_head.Controls.Add(this.button_clear);
            this.flowLayoutPanel_head.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel_head.Location = new System.Drawing.Point(0, 5);
            this.flowLayoutPanel_head.Name = "flowLayoutPanel_head";
            this.flowLayoutPanel_head.Size = new System.Drawing.Size(636, 44);
            this.flowLayoutPanel_head.TabIndex = 0;
            // 
            // pageHeader2
            // 
            this.pageHeader2.Controls.Add(this.label_CAN);
            this.pageHeader2.Controls.Add(this.button_BlueCAN);
            this.pageHeader2.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.pageHeader2.HandCursor = System.Windows.Forms.Cursors.Arrow;
            this.pageHeader2.Location = new System.Drawing.Point(3, 3);
            this.pageHeader2.Name = "pageHeader2";
            this.pageHeader2.Size = new System.Drawing.Size(405, 38);
            this.pageHeader2.SubGap = 0;
            this.pageHeader2.TabIndex = 4;
            this.pageHeader2.Text = "数据发送";
            this.pageHeader2.UseTitleFont = true;
            // 
            // label_CAN
            // 
            this.label_CAN.AutoSizeMode = AntdUI.TAutoSize.Width;
            this.label_CAN.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_CAN.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.label_CAN.Location = new System.Drawing.Point(232, 0);
            this.label_CAN.Name = "label_CAN";
            this.label_CAN.Size = new System.Drawing.Size(6, 38);
            this.label_CAN.TabIndex = 1;
            this.label_CAN.Text = "-";
            // 
            // button_BlueCAN
            // 
            this.button_BlueCAN.AutoSizeMode = AntdUI.TAutoSize.Height;
            this.button_BlueCAN.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_BlueCAN.Ghost = true;
            this.button_BlueCAN.IconRatio = 1F;
            this.button_BlueCAN.IconSvg = "SettingOutlined";
            this.button_BlueCAN.Location = new System.Drawing.Point(106, 0);
            this.button_BlueCAN.Name = "button_BlueCAN";
            this.button_BlueCAN.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.button_BlueCAN.Size = new System.Drawing.Size(126, 48);
            this.button_BlueCAN.TabIndex = 0;
            this.button_BlueCAN.Text = "BlueCAN";
            this.button_BlueCAN.Click += new System.EventHandler(this.button_BlueCAN_Click);
            // 
            // button_readmemory
            // 
            this.button_readmemory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_readmemory.AutoSizeMode = AntdUI.TAutoSize.Auto;
            this.button_readmemory.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.button_readmemory.Location = new System.Drawing.Point(411, 5);
            this.button_readmemory.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.button_readmemory.Name = "button_readmemory";
            this.button_readmemory.Size = new System.Drawing.Size(96, 36);
            this.button_readmemory.TabIndex = 5;
            this.button_readmemory.Text = "获取数据列表";
            this.button_readmemory.Type = AntdUI.TTypeMini.Primary;
            this.button_readmemory.Click += new System.EventHandler(this.button_readmemory_Click);
            // 
            // button_dump
            // 
            this.button_dump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_dump.AutoSizeMode = AntdUI.TAutoSize.Auto;
            this.button_dump.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.button_dump.Location = new System.Drawing.Point(507, 5);
            this.button_dump.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.button_dump.Name = "button_dump";
            this.button_dump.Size = new System.Drawing.Size(48, 36);
            this.button_dump.TabIndex = 6;
            this.button_dump.Text = "下载";
            this.button_dump.Type = AntdUI.TTypeMini.Primary;
            this.button_dump.Click += new System.EventHandler(this.button_dump_Click);
            // 
            // button_clear
            // 
            this.button_clear.AutoSizeMode = AntdUI.TAutoSize.Auto;
            this.button_clear.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.button_clear.Location = new System.Drawing.Point(555, 5);
            this.button_clear.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(72, 36);
            this.button_clear.TabIndex = 7;
            this.button_clear.Text = "清空内存";
            this.button_clear.Type = AntdUI.TTypeMini.Error;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // exportdata
            // 
            this.exportdata.Controls.Add(this.listViewRawData);
            this.exportdata.Controls.Add(this.gridPanel1);
            this.exportdata.Controls.Add(this.gridPanel2);
            this.exportdata.Controls.Add(this.pageHeader3);
            this.exportdata.Location = new System.Drawing.Point(-1272, -1130);
            this.exportdata.Name = "exportdata";
            this.exportdata.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.exportdata.Size = new System.Drawing.Size(636, 565);
            this.exportdata.TabIndex = 1;
            this.exportdata.Text = "原始数据";
            // 
            // listViewRawData
            // 
            this.listViewRawData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listViewRawData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewRawData.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listViewRawData.FullRowSelect = true;
            this.listViewRawData.HideSelection = false;
            this.listViewRawData.Location = new System.Drawing.Point(10, 161);
            this.listViewRawData.Name = "listViewRawData";
            this.listViewRawData.Size = new System.Drawing.Size(616, 394);
            this.listViewRawData.TabIndex = 7;
            this.listViewRawData.UseCompatibleStateImageBehavior = false;
            this.listViewRawData.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 39;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "RawData";
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Valid";
            this.columnHeader3.Width = 46;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Time";
            this.columnHeader4.Width = 87;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            this.columnHeader5.Width = 43;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Data";
            this.columnHeader6.Width = 253;
            // 
            // gridPanel1
            // 
            this.gridPanel1.Controls.Add(this.comboBoxRawOrCalcData);
            this.gridPanel1.Controls.Add(this.label4);
            this.gridPanel1.Controls.Add(this.textBoxEnd);
            this.gridPanel1.Controls.Add(this.label3);
            this.gridPanel1.Controls.Add(this.comboBoxSrcToolFilter);
            this.gridPanel1.Controls.Add(this.label1);
            this.gridPanel1.Controls.Add(this.textBoxStart);
            this.gridPanel1.Controls.Add(this.label2);
            this.gridPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridPanel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gridPanel1.Location = new System.Drawing.Point(10, 94);
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Size = new System.Drawing.Size(616, 67);
            this.gridPanel1.Span = "80 35% 35% 25%;\r\n80 35% 35% 25%;";
            this.gridPanel1.TabIndex = 2;
            this.gridPanel1.Text = "gridPanel1";
            // 
            // comboBoxRawOrCalcData
            // 
            this.comboBoxRawOrCalcData.Items.AddRange(new object[] {
            "Raw Data",
            "Calc Data"});
            this.comboBoxRawOrCalcData.Location = new System.Drawing.Point(455, 34);
            this.comboBoxRawOrCalcData.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxRawOrCalcData.Name = "comboBoxRawOrCalcData";
            this.comboBoxRawOrCalcData.Size = new System.Drawing.Size(134, 34);
            this.comboBoxRawOrCalcData.TabIndex = 11;
            this.comboBoxRawOrCalcData.SelectedIndexChanged += new AntdUI.IntEventHandler(this.comboBoxRawOrCalcData_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(271, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(182, 28);
            this.label4.TabIndex = 10;
            this.label4.Text = "选择数据";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxEnd
            // 
            this.textBoxEnd.Location = new System.Drawing.Point(80, 34);
            this.textBoxEnd.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxEnd.Name = "textBoxEnd";
            this.textBoxEnd.Size = new System.Drawing.Size(188, 34);
            this.textBoxEnd.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 28);
            this.label3.TabIndex = 9;
            this.label3.Text = "结束时间";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxSrcToolFilter
            // 
            this.comboBoxSrcToolFilter.Items.AddRange(new object[] {
            "All",
            "Gsc"});
            this.comboBoxSrcToolFilter.Location = new System.Drawing.Point(455, 0);
            this.comboBoxSrcToolFilter.Margin = new System.Windows.Forms.Padding(0);
            this.comboBoxSrcToolFilter.Name = "comboBoxSrcToolFilter";
            this.comboBoxSrcToolFilter.Size = new System.Drawing.Size(134, 34);
            this.comboBoxSrcToolFilter.TabIndex = 6;
            this.comboBoxSrcToolFilter.SelectedIndexChanged += new AntdUI.IntEventHandler(this.comboBoxSrcToolFilter_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(271, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 28);
            this.label1.TabIndex = 7;
            this.label1.Text = "选择属性";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxStart
            // 
            this.textBoxStart.Location = new System.Drawing.Point(80, 0);
            this.textBoxStart.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxStart.Name = "textBoxStart";
            this.textBoxStart.Size = new System.Drawing.Size(188, 34);
            this.textBoxStart.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 28);
            this.label2.TabIndex = 8;
            this.label2.Text = "开始时间";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gridPanel2
            // 
            this.gridPanel2.Controls.Add(this.buttonExportToExcel2);
            this.gridPanel2.Controls.Add(this.buttonRawFile);
            this.gridPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.gridPanel2.Location = new System.Drawing.Point(10, 43);
            this.gridPanel2.Name = "gridPanel2";
            this.gridPanel2.Size = new System.Drawing.Size(616, 51);
            this.gridPanel2.Span = "80% 20%";
            this.gridPanel2.TabIndex = 6;
            this.gridPanel2.Text = "gridPanel2";
            // 
            // buttonExportToExcel2
            // 
            this.buttonExportToExcel2.Location = new System.Drawing.Point(496, 3);
            this.buttonExportToExcel2.Name = "buttonExportToExcel2";
            this.buttonExportToExcel2.Size = new System.Drawing.Size(117, 45);
            this.buttonExportToExcel2.TabIndex = 7;
            this.buttonExportToExcel2.Text = "导出到Excel";
            this.buttonExportToExcel2.Type = AntdUI.TTypeMini.Primary;
            this.buttonExportToExcel2.Click += new System.EventHandler(this.buttonExportToExcel2_Click);
            // 
            // buttonRawFile
            // 
            this.buttonRawFile.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonRawFile.IconRatio = 1.2F;
            this.buttonRawFile.IconSvg = "";
            this.buttonRawFile.Location = new System.Drawing.Point(3, 3);
            this.buttonRawFile.Name = "buttonRawFile";
            this.buttonRawFile.Size = new System.Drawing.Size(487, 45);
            this.buttonRawFile.TabIndex = 1;
            this.buttonRawFile.Text = "Click or drag file to this area to upload";
            this.buttonRawFile.TextDesc = "原始内存文件(.dump)";
            this.buttonRawFile.DragChanged += new AntdUI.IControl.DragEventHandler(this.buttonRawFile_DragChanged);
            // 
            // pageHeader3
            // 
            this.pageHeader3.Dock = System.Windows.Forms.DockStyle.Top;
            this.pageHeader3.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.pageHeader3.HandCursor = System.Windows.Forms.Cursors.Arrow;
            this.pageHeader3.Location = new System.Drawing.Point(10, 5);
            this.pageHeader3.Name = "pageHeader3";
            this.pageHeader3.Size = new System.Drawing.Size(616, 38);
            this.pageHeader3.SubGap = 0;
            this.pageHeader3.TabIndex = 5;
            this.pageHeader3.Text = "文件解析";
            this.pageHeader3.UseTitleFont = true;
            // 
            // GSCDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 636);
            this.Controls.Add(this.tabs_menu);
            this.Controls.Add(this.pageHeader1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 400);
            this.Name = "GSCDownload";
            this.Text = "GSCDownload";
            this.Load += new System.EventHandler(this.GSCDownload_Load);
            this.tabs_menu.ResumeLayout(false);
            this.download.ResumeLayout(false);
            this.download.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel_head.ResumeLayout(false);
            this.flowLayoutPanel_head.PerformLayout();
            this.pageHeader2.ResumeLayout(false);
            this.pageHeader2.PerformLayout();
            this.exportdata.ResumeLayout(false);
            this.gridPanel1.ResumeLayout(false);
            this.gridPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.PageHeader pageHeader1;
        private AntdUI.Tabs tabs_menu;
        private AntdUI.TabPage download;
        private AntdUI.TabPage exportdata;
        private AntdUI.Panel panel1;
        private AntdUI.PageHeader pageHeader2;
        private AntdUI.Button button_BlueCAN;
        private AntdUI.Label label_CAN;
        private AntdUI.Button button_readmemory;
        private AntdUI.Button button_dump;
        private AntdUI.Button button_clear;
        private AntdUI.In.FlowLayoutPanel flowLayoutPanel_head;
        private AntdUI.Progress progress_memoryused;
        private AntdUI.Progress progressBar1;
        private System.Windows.Forms.ListView listViewRuns;
        private System.Windows.Forms.ColumnHeader colRunNumber;
        private System.Windows.Forms.ColumnHeader colStartTime;
        private System.Windows.Forms.ColumnHeader colEndTime;
        private System.Windows.Forms.ColumnHeader colDuration;
        private System.Windows.Forms.ColumnHeader colSize;
        private AntdUI.GridPanel gridPanel1;
        private AntdUI.UploadDragger buttonRawFile;
        private AntdUI.PageHeader pageHeader3;
        private AntdUI.Input textBoxStart;
        private AntdUI.Label label1;
        private AntdUI.Select comboBoxSrcToolFilter;
        private AntdUI.Label label2;
        private AntdUI.GridPanel gridPanel2;
        private AntdUI.Button buttonExportToExcel2;
        private AntdUI.Select comboBoxRawOrCalcData;
        private AntdUI.Label label4;
        private AntdUI.Input textBoxEnd;
        private AntdUI.Label label3;
        private System.Windows.Forms.ListView listViewRawData;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
    }
}

