using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

public class UpdateInfo
{
    public string version { get; set; }
    public string url { get; set; }
    public string desc { get; set; }
}

public class Updater
{
    // ⭐ 改成你的 version.json 地址（非常重要）
    //private static string versionUrl = "https://orange-lake-c42d.noctmew.workers.dev";
    private static string versionUrl = "https://gscdowndate-api-cplclhnzxx.cn-qingdao.fcapp.run";

    public static async Task CheckUpdateAsync()
    {
        try
        {
            string json;

            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                json = await client.DownloadStringTaskAsync(versionUrl);
            }

            /*UpdateInfo info = JsonConvert.DeserializeObject<UpdateInfo>(json);

            string currentVersion = Application.ProductVersion;

            if (new Version(info.version) > new Version(currentVersion))
            {
                DialogResult result = MessageBox.Show(
                    "发现新版本：" + info.version + "\n\n" + info.desc + "\n\n是否更新？",
                    "更新提示",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    await DownloadAndInstall(info.url);
                }
            }*/

            // 兼容两种格式（非常重要）
            dynamic rawObj = JsonConvert.DeserializeObject(json);

            string realJson;

            // ⭐ 如果有 body，说明是 FC 标准返回
            if (rawObj.body != null)
            {
                realJson = rawObj.body.ToString();
            }
            else
            {
                // ⭐ 如果没有 body，直接用原 JSON
                realJson = json;
            }

            // ⭐ 修改3：真正解析更新信息
            UpdateInfo info =
                JsonConvert.DeserializeObject<UpdateInfo>(realJson);

            string currentVersion =
                Application.ProductVersion;

            if (new Version(info.version) >
                new Version(currentVersion))
            {
                DialogResult result = MessageBox.Show(
                    "发现新版本：" + info.version +
                    "\n\n" +
                    info.desc +
                    "\n\n是否更新？",

                    "更新提示",

                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    await DownloadAndInstall(info.url);
                }
            }
        }
        catch (Exception ex)
        {
            // 建议静默，不影响用户使用
            Console.WriteLine("更新失败：" + ex.Message);
            //MessageBox.Show(
            //    "更新失败：\n" + ex.Message);
        }
    }

    private static async Task DownloadAndInstall(string url)
    {
        string tempFile = Path.Combine(Path.GetTempPath(), "update.exe");
        if (File.Exists(tempFile))// 防止重复下载
        {
            File.Delete(tempFile);
        }

        try
        {
            using (WebClient client = new WebClient())
            {
                // ⭐ 可选：显示下载进度
                client.DownloadProgressChanged += (s, e) =>
                {
                    Console.WriteLine("下载进度：" + e.ProgressPercentage + "%");
                };

                await client.DownloadFileTaskAsync(new Uri(url), tempFile);
            }

            // ⭐ 启动安装包（你的 NSIS 会自动升级）
            Process.Start(new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true
            });

            // ⭐ 关闭当前程序（必须）
            Application.Exit();
        }
        catch (Exception ex)
        {
            MessageBox.Show("下载更新失败：" + ex.Message);
        }
    }
}