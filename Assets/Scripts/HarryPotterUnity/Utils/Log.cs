using System;
using System.IO;
using UnityEngine;

namespace HarryPotterUnity.Utils
{
    public static class Log
    {
        private static readonly string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "HP-TCG.log");

        static Log()
        {
            File.WriteAllText(LogFilePath, string.Empty);

            using (var writer = new StreamWriter(LogFilePath))
            {
                writer.WriteLine("------------------------------------");
                writer.WriteLine("------- Harry Potter TCG Log -------");
                writer.WriteLine("------------------------------------");
                writer.WriteLine("Current Time: {0:MMM dd yyyy HH:mm:ss}", DateTime.Now);
                writer.WriteLine();
            }
        }

        public static void Write(string text)
        {
            using (var writer = new StreamWriter(LogFilePath, append: true))
            {
                string msg = string.Format("{0:HH:mm:ss} -- {1}", DateTime.Now, text);
                writer.WriteLine(msg);
            }

            Debug.Log(text);
        }
    }
}