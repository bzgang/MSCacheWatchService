using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MSCacheWatchService
{
    public partial class MSCacheWatchService : ServiceBase
    {
        mscacheWeb.MemoryCacheService MSCache = new mscacheWeb.MemoryCacheService();
        //  MSCacheService.MemoryCacheServiceClient MSCache = new MSCacheService.MemoryCacheServiceClient();
        public MSCacheWatchService()
        {
            InitializeComponent();
            base.ServiceName = "MSCacheWatchService";
        }

        protected override void OnStart(string[] args)
        {
            WriteLog.WriteLogToTxt("1、开始执行Windows服务（OnStart）..." + DateTime.Now.ToString());
            setTimes();
        }
        System.Timers.Timer aTimer;
        protected override void OnStop()
        {
            WriteLog.WriteLogToTxt("end、结束Windows服务（OnStop）..." + DateTime.Now.ToString());
            this.aTimer.Enabled = false;
        }
        public void setTimes()
        {
            WriteLog.WriteLogToTxt("2、开始执行定时任务（setTimes）..." + DateTime.Now.ToString());
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(TimeEvent);
            // 设置引发时间的时间间隔 此处设置为１秒（１０００毫秒）
            string cachePeriod = ConfigurationManager.AppSettings["SetCachePeriod"].ToString();
            aTimer.Interval = Convert.ToInt32(cachePeriod) * 1000;
            aTimer.Enabled = true; 
        }

        private void TimeEvent(object sender, ElapsedEventArgs e)
        {
            WriteLog.WriteLogToTxt("3、进入定时任务执行方法：（TimeEvent）..." + DateTime.Now.ToString());

            if (IsRunCache())
            {
                WriteLog.WriteLogToTxt("【批量执行】开始批量执行更新(写入)缓存..." );
                MSCache.AllUserCacheInit();
            }

            string msg = MSCache.testGetCache("keyNameFile");
            WriteLog.WriteLogToTxt("【1】、获取测试缓存内容keyNameFile--:" + msg);
            if (msg.Contains("没有相关缓存"))
            {
                MSCache.testFileAddCache("keyNameFile", "File-TestCache-测试缓存信息录入...【测试】" );
                WriteLog.WriteLogToTxt("【Warning!!!缓存失效(或没有找到缓存信息)..】" + msg + ",开始创建相关缓存信息...");
            }
 
        }

        /// <summary>
        /// 判断是否满足换成执行逻辑
        /// </summary>
        /// <returns></returns>
        public bool IsRunCache()
        {
            bool isRun = false;
            string cacheType = ConfigurationManager.AppSettings["SetCacheType"].ToString(); //执行周期类型，月、周、天、定时(每个几秒执行一次)
            string cacheDay = ConfigurationManager.AppSettings["SetCacheDay"].ToString();//每月执行时间【1-30】
            string cacheWeekDay = ConfigurationManager.AppSettings["SetCacheWeekDay"].ToString();

            string cacheTime = ConfigurationManager.AppSettings["SetCacheTime"].ToString();

            string currentWeek = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek);//格式化日期为中文格式：星期一

            string currentWeekDay = DateTime.Now.DayOfWeek.ToString("D");//当前星期：数字1-7 
            string currentDay = DateTime.Now.Day.ToString(); //当前月中的天数，数字1-30
            string currentTime = DateTime.Now.ToString("HH:mm");//当前时间：时分

            //WriteLog.WriteLogToTxt("cacheType:" + cacheType + ",cacheDay：" + cacheDay + ",cacheWeekDay"
            //    + cacheWeekDay + ",cacheTime:" + cacheTime + ",currentWeek:" + currentWeek
            //    + ",currentWeekDay:" + currentWeekDay + ",currentDay:" + currentDay + ",currentTime:" + currentTime);

            switch (cacheType)
            {
                case "month": //按每月固定时间执行
                    if (currentDay == cacheDay && cacheTime.IndexOf(currentTime) >= 0)
                    {
                        WriteLog.WriteLogToTxt("1、按每月固定时间执行--开始批量执行更新(写入)缓存...");
                        isRun = true;
                    }
                    break;
                case "week"://按每周固定时间执行
                    if (currentWeekDay == cacheWeekDay && cacheTime.IndexOf(currentTime) >= 0)
                    {
                        WriteLog.WriteLogToTxt("2、按每周固定时间执行--开始批量执行更新(写入)缓存...");
                        isRun = true;
                    }
                    break;
                case "day"://每天执行
                    if (cacheTime.IndexOf(currentTime) >= 0)
                    {
                        WriteLog.WriteLogToTxt("3、每天执行--开始批量执行更新(写入)缓存...");
                        isRun = true;
                    }
                    break;
                case "time":
                    WriteLog.WriteLogToTxt("4、定时执行--开始批量执行更新(写入)缓存...");
                    isRun = true;
                    break;
                default: isRun = false; break;
            }

            return isRun;

        }
    }
}
