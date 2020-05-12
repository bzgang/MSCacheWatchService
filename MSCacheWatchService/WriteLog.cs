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
        static string fileFullPath = "";
        static string strFileFolder = "";
        static string fileName = "";
        /// <summary>
        /// 判断文件日志文件是否超过固定大小，如果超过，则新启动一个日志文件
        /// </summary>
        /// <param name="fileFullPath">现有日志文件全路径</param>
        /// <returns></returns>
        public static string getFileName(string fileFullPath)
        { 
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileFullPath);
            if (!File.Exists(fileFullPath))
            {
                return fileInfo.Name;
            }
            String fileNameNoExt = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);//获取文件名称，不包含扩展名
            long fileLangth = (fileInfo.Length / 1024) <= 1 ? 1 : (fileInfo.Length / 1024); //文件大小kb
            if (fileLangth >= 1500) //对比文件大小，1500为1500kb，超过则重命名新的文件名称
            {
                if (fileNameNoExt.Contains("_"))
                {
                    fileName = fileNameNoExt.Split('_')[0] + "_" + (Int32.Parse(fileNameNoExt.Split('_')[1]) + 1) + ".txt";
                }
                else
                {
                    fileName = fileNameNoExt + "_1" + ".txt";
                }
                string newFilePath = strFileFolder + "\\" + fileName;
                if (File.Exists(newFilePath))
                {
                    fileName = getFileName(newFilePath);//递归返回文件名称
                }
            }
            return fileName;
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="LogContent">日志内容</param>
        public static void WriteLogToTxt(string LogContent)
        {
            //日志所在路径及文件夹：System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase当前程序运行目录，log 存储日志文件夹
            strFileFolder = string.Format(@"{0}{1}", System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "log");
            if (!System.IO.Directory.Exists(strFileFolder))
                System.IO.Directory.CreateDirectory(strFileFolder);
            string ffPath = strFileFolder + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";//日志文件名称：日期格式
            fileFullPath = strFileFolder + "\\" + getFileName(ffPath);
            StreamWriter sw;
            if (!File.Exists(fileFullPath))
            {
                sw = File.CreateText(fileFullPath);
            }
            else
            {
                sw = File.AppendText(fileFullPath);

            } 
            string logMsg = string.Format("时间：{0},日志内容：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), LogContent);
            sw.WriteLine(logMsg);
            sw.WriteLine("-----------------------------------------------------------------------");
            sw.Close();
        }
    }
}
