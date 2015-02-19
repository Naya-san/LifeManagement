using LifeManagement.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LifeManagement.Extensions
{
    public static class ComplexityExtensions
    {
        public static string ToLocalizedString(this Complexity complexity)
        {
            switch (complexity)
            {
                case Complexity.Hight:
                    return Resources.ResourceScr.High;
                case Complexity.Medium:
                    return Resources.ResourceScr.Medium;
                case Complexity.Low:
                    return Resources.ResourceScr.Low;
                default:
                    return Resources.ResourceScr.None;
            }
        }

        public static SelectList ToSelectList(this Complexity complexity)
        {
            var positions = new List<Pair>
            {
                new Pair(){ ID =(int) Complexity.None , Text = Complexity.None.ToLocalizedString()},
                new Pair(){ ID =(int) Complexity.Low, Text = Complexity.Low.ToLocalizedString()},
                new Pair(){ ID =(int) Complexity.Medium, Text = Complexity.Medium.ToLocalizedString()},
                new Pair(){ ID =(int) Complexity.Hight, Text = Complexity.Hight.ToLocalizedString()}
            };
            return new SelectList(positions, "ID", "Text", (int)complexity);
        }
    }
}