using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuidanceStsbilityCommsDownLoad
{
    public class Utility
    {
        public const String LWDDebugFileName = "Debug.log";

        // method
        public static String DefaultDirectory()
        {
            // String myDocumentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String myDocumentDirectory = Environment.CurrentDirectory;
            String myProjectDataDirectory = Path.Combine(myDocumentDirectory + @"\Logs", Application.ProductName + " " + Application.ProductVersion);
            return myProjectDataDirectory;
        }
        public static String GetFileFullName(String fileName)
        {
            String myProjectDataDirectory = DefaultDirectory();
            if (!File.Exists(myProjectDataDirectory)) Directory.CreateDirectory(myProjectDataDirectory);
            String myFileFullName = Path.Combine(myProjectDataDirectory, fileName);
            return myFileFullName;
        }
        public static void DebugLog(String msg)
        {
            //String programDir = Path.GetDirectoryName(Application.ExecutablePath);
            //String fileFullName = Path.Combine(programDir, BWUtility.BWLWDDebugFileName);
            String fileFullName = Utility.GetFileFullName(Utility.LWDDebugFileName);
            FileStream fs = new FileStream(fileFullName, FileMode.OpenOrCreate);
            int maxSize = 1000000;
            int minSize = 100000;
            if (fs.Length > maxSize)
            {
                byte[] data = new byte[minSize];
                fs.Seek(minSize, SeekOrigin.End);
                fs.Read(data, 0, minSize);
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(data, 0, minSize);
                fs.SetLength(minSize);
            }
            fs.Close();

            StreamWriter sw = new StreamWriter(fileFullName, true);
            sw.WriteLine(DateTime.Now + ": " + msg);
            sw.Close();
        }
    }
    public enum EnumGscCurves
    {
        功能码 = 0,
        is重力 = 1,
        工具面 = 2,
        陀螺转速 = 3,
        定向参数 = 4,
    }
}
