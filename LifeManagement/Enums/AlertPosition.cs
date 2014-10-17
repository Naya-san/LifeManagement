using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LifeManagement.Enums
{
    public enum AlertPosition
    {
        None = -1,
        Simultaneously = 0,
        B5M = 5,
        B10M = 10,
        B15M = 15,
        B30M = 30,
        B1H = 60,
        B2H = 120,
        B1D = 1440,
        B2D = 2880,
        Wb = 10080
    }
}