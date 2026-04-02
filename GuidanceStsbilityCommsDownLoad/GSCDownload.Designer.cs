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
            this.tabs_menu.SuspendLayout();
            this.download.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel_head.SuspendLayout();
            this.pageHeader2.SuspendLayout();
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
            this.pageHeader1.TabIndex = 0;
            this.pageHeader1.Text = "pageHeader1";
            // 
            // tabs_menu
            // 
            this.tabs_menu.Controls.Add(this.download);
            this.tabs_menu.Controls.Add(this.exportdata);
            this.tabs_menu.Cursor = System.Windows.Forms.Cursors.Hand;
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
            // 
            // exportdata
            // 
            this.exportdata.Location = new System.Drawing.Point(-1272, -1130);
            this.exportdata.Name = "exportdata";
            this.exportdata.Size = new System.Drawing.Size(636, 565);
            this.exportdata.TabIndex = 1;
            this.exportdata.Text = "原始数据";
            // 
            // GSCDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 636);
            this.Controls.Add(this.tabs_menu);
            this.Controls.Add(this.pageHeader1);
            this.Name = "GSCDownload";
            this.Text = "Form1";
            this.tabs_menu.ResumeLayout(false);
            this.download.ResumeLayout(false);
            this.download.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel_head.ResumeLayout(false);
            this.flowLayoutPanel_head.PerformLayout();
            this.pageHeader2.ResumeLayout(false);
            this.pageHeader2.PerformLayout();
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
    }
}

