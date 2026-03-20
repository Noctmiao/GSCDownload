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
            this.flowLayoutPanel1 = new AntdUI.In.FlowLayoutPanel();
            this.button1 = new AntdUI.Button();
            this.button2 = new AntdUI.Button();
            this.button3 = new AntdUI.Button();
            this.exportdata = new AntdUI.TabPage();
            this.pageHeader2 = new AntdUI.PageHeader();
            this.button_BlueCAN = new AntdUI.Button();
            this.label1 = new AntdUI.Label();
            this.tabs_menu.SuspendLayout();
            this.download.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.pageHeader1.Size = new System.Drawing.Size(547, 35);
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
            this.tabs_menu.Size = new System.Drawing.Size(547, 601);
            this.tabs_menu.Style = styleLine1;
            this.tabs_menu.TabIndex = 1;
            this.tabs_menu.Text = "tabs1";
            // 
            // download
            // 
            this.download.Controls.Add(this.panel1);
            this.download.Controls.Add(this.flowLayoutPanel1);
            this.download.Dock = System.Windows.Forms.DockStyle.Fill;
            this.download.Location = new System.Drawing.Point(0, 36);
            this.download.Name = "download";
            this.download.Size = new System.Drawing.Size(547, 565);
            this.download.TabIndex = 0;
            this.download.Text = "下载";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 81);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(547, 484);
            this.panel1.TabIndex = 1;
            this.panel1.Text = "panel1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.pageHeader2);
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.button3);
            this.flowLayoutPanel1.Controls.Add(this.button2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(547, 81);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 34);
            this.button1.TabIndex = 1;
            this.button1.Text = "获取数据列表";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(223, 44);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "清空内存";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(142, 44);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "下载";
            // 
            // exportdata
            // 
            this.exportdata.Location = new System.Drawing.Point(-1094, -1130);
            this.exportdata.Name = "exportdata";
            this.exportdata.Size = new System.Drawing.Size(547, 565);
            this.exportdata.TabIndex = 1;
            this.exportdata.Text = "原始数据";
            // 
            // pageHeader2
            // 
            this.pageHeader2.Controls.Add(this.label1);
            this.pageHeader2.Controls.Add(this.button_BlueCAN);
            this.pageHeader2.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.pageHeader2.HandCursor = System.Windows.Forms.Cursors.Arrow;
            this.pageHeader2.Location = new System.Drawing.Point(3, 3);
            this.pageHeader2.Name = "pageHeader2";
            this.pageHeader2.Size = new System.Drawing.Size(547, 35);
            this.pageHeader2.SubGap = 0;
            this.pageHeader2.TabIndex = 4;
            this.pageHeader2.Text = "数据发送";
            this.pageHeader2.UseTitleFont = true;
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
            this.button_BlueCAN.Padding = new System.Windows.Forms.Padding(0, 0, 0, 13);
            this.button_BlueCAN.Size = new System.Drawing.Size(126, 48);
            this.button_BlueCAN.TabIndex = 0;
            this.button_BlueCAN.Text = "BlueCAN";
            // 
            // label1
            // 
            this.label1.AutoSizeMode = AntdUI.TAutoSize.Width;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(232, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 35);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // GSCDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 636);
            this.Controls.Add(this.tabs_menu);
            this.Controls.Add(this.pageHeader1);
            this.Name = "GSCDownload";
            this.Text = "Form1";
            this.tabs_menu.ResumeLayout(false);
            this.download.ResumeLayout(false);
            this.download.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pageHeader2.ResumeLayout(false);
            this.pageHeader2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.PageHeader pageHeader1;
        private AntdUI.Tabs tabs_menu;
        private AntdUI.TabPage download;
        private AntdUI.TabPage exportdata;
        private AntdUI.In.FlowLayoutPanel flowLayoutPanel1;
        private AntdUI.Panel panel1;
        private AntdUI.Button button1;
        private AntdUI.Button button2;
        private AntdUI.Button button3;
        private AntdUI.PageHeader pageHeader2;
        private AntdUI.Button button_BlueCAN;
        private AntdUI.Label label1;
    }
}

