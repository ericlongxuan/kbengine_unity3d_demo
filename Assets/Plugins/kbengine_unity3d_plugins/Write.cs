﻿// added by weichen

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace KBEngine
{
    public class Write : MonoBehaviour
    {
        private static FileStream FileWriter;
        private static UTF8Encoding encoding;
        private static Write _consoleLog;
        private static bool _AllDisplay;
        private static bool _LogDisplay;
        private static bool _WarningDisplay;
        private static bool _LogData;
        private static bool IsIDE;
        private FileInfo fileInfo;
        private string NowTime;

        public static Write console //开启单例
        {
            get
            {
                if (_consoleLog == null)
                    _consoleLog = new Write();
                return _consoleLog;
            }
        }

        /// <summary>
        ///     开始写入日志，参数一：是否写入Warning类型数据，默认不写入，参数二：是否写入Debug.Log类型数据，默认不写入，参数三：是否写入全部数据，默认不写入,参数四：是否将Log方法信息输出到控制台，默认输出
        /// </summary>
        /// <param name="WarningDisplay"></param>
        public void LogStart(bool WarningDisplay = false, bool LogDisplay = false, bool AllDisplay = false,
            bool LogData = true)
        {

            if ((FileWriter == null))
            {
                IsIDE = Application.isEditor; //获取当前场景运行环境
                _WarningDisplay = WarningDisplay;
                _LogDisplay = LogDisplay;
                _AllDisplay = AllDisplay;
                _LogData = LogData;
                if (IsIDE == true) //判断当前场景运行环境, if是Editor中则执行, do not run in the packaged app
                {
                    NowTime = DateTime.Now.ToString().Replace(" ", "_").Replace("/", "_").Replace(":", "_");
                    fileInfo = new FileInfo(Application.dataPath + "/../Logs/" + NowTime + "_Log.txt");
                    //设置Log文件输出地址
                    FileWriter = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    encoding = new UTF8Encoding();
                    Application.logMessageReceived += LogCallback;
                }
            }
        }

        /// <summary>
        ///     替代Debug.log写入Log信息
        /// </summary>
        /// <param name="_log"></param>
        /// <param name="con"></param>
        public static void Log(object _log)
        {
            if ((_LogDisplay == false) && (_AllDisplay == false))
            {
                if (_LogData)
                    Debug.Log(_log);
                if (IsIDE == true) //判断当前场景运行环境, if是Editor中则执行,
                {
                    try
                    {
                        var trace = new StackTrace(); //获取调用类信息
                        var ClassName = trace.GetFrame(1).GetMethod().DeclaringType.Name;
                        var WayName = trace.GetFrame(1).GetMethod().Name;
                        var log = DateTime.Now + " " + DateTime.Now.Millisecond + ", " + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds + ", [" + ClassName + "." + WayName + "]" + " " + ":" + " " + _log +
                                  Environment.NewLine;
                        FileWriter.Write(encoding.GetBytes(log), 0, encoding.GetByteCount(log));
                    }
                    catch (Exception)
                    {
                        Debug.Log("5");
                        Debug.Log("请检测是否调用了Console.LogStart方法,或者关闭控制台Log写入与所有数据写入项");
                    }

                }
            }
            else
            {
                Debug.Log("请检测是否调用了Console.LogStart方法,或者关闭控制台Log写入与所有数据写入项");
            }
        }

        private void LogCallback(string condition, string stackTrace, LogType type) //写入控制台数据
        {
            string content = null;
            if (_AllDisplay == false)
            {
                if (type.ToString() == "Warning")
                    if (_WarningDisplay == false)
                    {
                        condition = "";
                        stackTrace = "";
                        content = "";
                    }
                    else
                    {
                        content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " +
                                  condition +
                                  Environment.NewLine;
                    }

                if (type.ToString() == "Log")
                    if (_LogDisplay == false)
                    {
                        condition = "";
                        stackTrace = "";
                        content = "";
                    }
                    else
                    {
                        content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " +
                                  condition +
                                  Environment.NewLine;
                    }
                if (type.ToString() == "Exception")
                    content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " + condition +
                              Environment.NewLine;
            }
            else
            {
                content = DateTime.Now + " " + "[" + type + "]" + "[" + stackTrace + "]" + " " + ":" + " " + condition +
                          Environment.NewLine;
            }
            FileWriter.Write(encoding.GetBytes(content), 0, encoding.GetByteCount(content));
            FileWriter.Flush();
        }

        private void OnDestroy() //关闭写入
        {
            if ((FileWriter != null) && (IsIDE == false))
            {
                FileWriter.Close();
                Application.RegisterLogCallback(null);
            }
        }
    }
}