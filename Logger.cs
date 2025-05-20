using System;
using System.IO;
using UnityEngine;

namespace PureVesselSkills
{
    public static class MyLogger
    {
        private static readonly string logFilePath = "/home/hien/DATA/Mods/PureVesselSkills/log.txt";
        private static bool initialized = false;

        public static void Log(string message)
        {
            if (!initialized)
            {
                InitLogFile();
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string fullMessage = $"[{timestamp}] {message}";

            try
            {
                File.AppendAllText(logFilePath, fullMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Logger failed: {ex.Message}");
            }
        }

        private static void InitLogFile()
        {
            try
            {
                File.WriteAllText(logFilePath, $"[Log started: {DateTime.Now}]\n");
                initialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize log file: {ex.Message}");
            }
        }
    }
}
