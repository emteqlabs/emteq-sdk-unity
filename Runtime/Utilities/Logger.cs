using System;
using System.IO;
using UnityEngine;

namespace EmteqLabs
{
    public static class Logger
    {
        private static StreamWriter _logWriter;
        private const string LogHeader = "[EmteqPlugin]";
        
        public static void LogMessage(string logMessage, LogType logType = LogType.Log)
        {
            if (EmteqManager.Instance.ShowLogMessages)
            {
                if(Application.isEditor || Debug.isDebugBuild)
                {
                    switch (logType)
                    {
                        case LogType.Log:
                            Debug.LogFormat("{0}: {1}", LogHeader, logMessage);
                            break;
                        case LogType.Warning:
                            Debug.LogWarningFormat("{0}: {1}", LogHeader, logMessage);
                            break;
                        case LogType.Error:
                            Debug.LogErrorFormat("{0}: {1}", LogHeader, logMessage);
                            break;
                        case LogType.Exception:
                            Debug.LogException(new Exception(logMessage));
                            break;
                    }
                }
            }
            WriteToLogFile(logMessage, logType);
        }
        
        private static void WriteToLogFile(string logMessage, LogType logType)
        {
            if (_logWriter == null)
            {
                //TODO: create one log file per session and append it with session datetime
                _logWriter = new StreamWriter(Path.Combine(Application.persistentDataPath, "EmteqPlugin.log"));
                _logWriter.AutoFlush = true;
                Application.quitting += ApplicationOnquitting;
            }
            _logWriter.WriteLine("[{0}]: {1}: {2}", DateTime.Now, logType.ToString(), logMessage);
        }

        private static void ApplicationOnquitting()
        {
            Application.quitting -= ApplicationOnquitting;
            _logWriter.Dispose();
            _logWriter = null;
        }
    }
}