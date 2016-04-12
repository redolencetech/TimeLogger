using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLogger
{
    class Logic
    {
        static DateTime now;
        static string dailyFolder;
        static string monthlyFolder;
        private static void buildPath(string inputPath)
        {
            FileInfo fileInfo = new FileInfo(inputPath);
            if (!fileInfo.Directory.Exists)
            {
                System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);
            }
        }
        private static string readFile(string fileToRead, bool wholeFile)
        {
            StreamReader file = new StreamReader(fileToRead);
            string r;
            if (wholeFile)
            {
                r = file.ReadToEnd().ToString();
            }
            else
            {
                r = file.ReadToEnd().ToString().Substring(0, 8);
            }
            file.Close();
            return r.Trim();
        }

        private static void writeFile(string filePath, string textToWrite)
        {
            var currFile = new StreamWriter(filePath, true);
            currFile.WriteLine(textToWrite.Trim());
            currFile.Close();
        }
        public static void writeOut(string toWrite)
        {
            now = DateTime.Now;
            string currMonth = string.Format("{0:MMMM}", now);
            string currDay = string.Format("{0:dd}", now);
            string filename = string.Format("{0:HHmmss}", now);
            monthlyFolder = string.Format("C:\\Temp\\Timesheets\\{0}", currMonth);
            dailyFolder = string.Format("{0}\\{1}", monthlyFolder, currDay);
            string filePath = string.Format("{0}\\Individual\\{1}.txt", dailyFolder, filename);
            string fileText = string.Format("{0} {1}", now.ToShortTimeString(), toWrite);
            buildPath(filePath);
            writeFile(filePath, fileText);
            summariseDay();
        }
        private static void summariseDay()
        {
            var files = from file in Directory.GetFiles(string.Format("{0}\\Individual", dailyFolder))
                        orderby file ascending
                        select file;

            string dailySummaryFilename = string.Format("{0}\\{1}", dailyFolder, string.Format("{0:dd} {1:MMMM}.txt", now, now));

            File.Delete(dailySummaryFilename);

            foreach (var file in files)
            {
                writeFile(dailySummaryFilename, readFile(file, true));
            }

            string oldestFile = readFile(files.First(), false);
            string newestFile = readFile(files.Last(), false);

            DateTime startTime = DateTime.Parse(oldestFile, CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.Parse(newestFile, CultureInfo.InvariantCulture);

            TimeSpan timeDiff = endTime - startTime;

            StringBuilder endString = new StringBuilder();

            endString.AppendLine("");
            endString.AppendLine("--------------------------------~");
            endString.AppendLine(string.Format("{0} - {1} hours, {2} minutes", timeDiff, timeDiff.Hours, timeDiff.Minutes));

            writeFile(dailySummaryFilename, endString.ToString());
            summariseMonth();
        }

        private static void summariseMonth()
        {
            var dirs = from dir in Directory.GetDirectories(monthlyFolder)
                       orderby dir ascending
                       select dir;
            
            string monthlySummaryFilename = string.Format("{0}\\{1}", monthlyFolder, string.Format("{0:MMMM}.txt", now));
            File.Delete(monthlySummaryFilename);

            foreach (var dir in dirs)
            {
                string tempFileName = Directory.GetFiles(dir).First();
                string wholeFile = readFile(tempFileName, true);
                string outString = string.Format("{0} - {1}", Path.GetFileNameWithoutExtension(tempFileName), wholeFile.Substring(wholeFile.IndexOf('~', 0), wholeFile.Length - wholeFile.IndexOf('~', 0)).Replace("~\r\n", ""));
                writeFile(monthlySummaryFilename, outString);
            }
        }
    }
}
