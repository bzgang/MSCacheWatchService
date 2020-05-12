using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCacheWatchService
{
    public class WriteLog
    {
        public static void WriteLogToTxt(string LogContent)
        {
            string strFileFolder = string.Format(@"{0}{1}", System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, DateTime.Now.ToString("yyyy-MM-dd"));
            if (!System.IO.Directory.Exists(strFileFolder))
                System.IO.Directory.CreateDirectory(strFileFolder);
            string fileFullPath = strFileFolder + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            StreamWriter sw;
            if (!File.Exists(fileFullPath))
            {
                sw = File.CreateText(fileFullPath);
            }
            else
            {
                sw = File.AppendText(fileFullPath);
            }
            //sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //sw.WriteLine("-----------------------------------------------------------");
            string logMsg = string.Format("时间：{0},日志内容：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), LogContent);
            sw.WriteLine(logMsg);
            sw.WriteLine("-----------------------------------------------------------------------");
            sw.Close();
        }
    }
}
