﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace MicroVision.Core.Events
{
    public class StartCaptureEvent : PubSubEvent
    {
    }
    public class StopCaptureEvent: PubSubEvent { }
}
