using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            string currMonth = string.Format("{0:MMMM}", now);
            string currDay = string.Format("{0:dd}", now);
            string filename = string.Format("{0:hhmmss}", now);
            string filePath = string.Format("C:\\Temp\\Timesheets\\{0}\\{1}\\Individual\\{2}.txt", currMonth, currDay, filename);
            buildPath(filePath);
            StreamWriter currFile = new StreamWriter(filePath);
            currFile.WriteLine("{0} Locked", now.ToShortTimeString());

        }

        private void build()
        {
            DateTime now = DateTime.Now;

        }

        private static void buildPath(string inputPath)
        {
            FileInfo fileInfo = new FileInfo(inputPath);
            if (!fileInfo.Directory.Exists)
            {
                System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);
            }
        }
    }
}
