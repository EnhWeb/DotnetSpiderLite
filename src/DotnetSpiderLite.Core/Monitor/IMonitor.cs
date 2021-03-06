﻿using DotnetSpiderLite.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpiderLite.Monitor
{
    /// <summary>
    ///  爬虫监控
    /// </summary>
    public interface IMonitor
    {
        ILogger Logger { get; set; }


        /// <summary>
        ///  上报爬虫状态
        /// </summary>
        void Report(MonitorData data);

    }
}
