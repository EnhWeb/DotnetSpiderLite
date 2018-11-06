﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetSpiderLite.Core
{
    public enum SpiderState
    {
        Init = 1,
        Running = 2,
        Paused = 4,
        Finished = 8,
        Exited = 16
    }
}