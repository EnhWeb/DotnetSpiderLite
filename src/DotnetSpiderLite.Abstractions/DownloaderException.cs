﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetSpiderLite
{
    /// <summary>
    ///  Exception
    /// </summary>
    public class DownloaderException : Exception
    {
        public DownloaderException() : base()
        {
        }

        public DownloaderException(string message) : base(message)
        {
        }

        public DownloaderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
