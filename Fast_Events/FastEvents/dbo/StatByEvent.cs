﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastEvents.dbo
{
    public class StatByEvent : Interfaces.IObjectWithId
    {
        public long Id { get; set; }
        public int Views { get; set; }
    }
}