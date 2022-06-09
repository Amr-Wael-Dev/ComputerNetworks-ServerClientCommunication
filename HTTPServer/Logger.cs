using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");

        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 

            // بتاخد سترينج و بتضيفه على اللي موجود و بتقفل الفايل لوحدها AppendAllText
            File.AppendAllText("log.txt", "Datetime: " + DateTime.Now.AddHours(2).ToString() + "\nMessage: " + ex.Message + "\n");
        }
    }
}
