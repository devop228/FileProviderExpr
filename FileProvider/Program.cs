﻿using System;
using com.zhusmelb.Util.Logging;
using Microsoft.Extensions.FileProviders;

namespace FileProvider
{
    public class Program
    {
        private static readonly ILogger _log 
            = LogHelper.GetLogger(typeof(Program).FullName);

        public static void Main(string[] args) {
            var provider = new PhysicalFileProvider(".");
        }
    }
}
