﻿using GFramework.Bases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFramework.LogWriters
{
    using Bases;
    using Enums;
    using Holders;

    public class FileLogWriter : BaseLogWriter
    {
        string dateString;
        public FileLogWriter()
        {
            DateTime startdate = Process.GetCurrentProcess().StartTime;
            dateString = startdate.ToString("dd_MM_yyyy_HH_mm_ss");
        }

        string SafeName(string name) => name.Replace("<", "[").Replace(">", "]");

        public override void Write(LogHolder log)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "Log", SafeName(log.Name), dateString + ".txt");
            string dirPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            File.AppendAllText(filePath, $"{log.Time} - {log.Type.ToString().ToUpperInvariant()} - {log.Name}: {log.Message}" + Environment.NewLine);
        }
    }
}