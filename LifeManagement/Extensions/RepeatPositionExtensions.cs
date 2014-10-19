using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LifeManagement.Enums;

namespace LifeManagement.Extensions
{
    public static class RepeatPositionExtensions
    {
        public static string ToLocalizedString(this RepeatPosition repeat)
        {
            switch (repeat)
            {
                case RepeatPosition.Ed:
                    return Resources.ResourceScr.Ed;
                case RepeatPosition.Ew:
                    return Resources.ResourceScr.Ew;
                case RepeatPosition.E2w:
                    return Resources.ResourceScr.E2w;
                case RepeatPosition.Em:
                    return Resources.ResourceScr.Em;
                case RepeatPosition.Ey:
                    return Resources.ResourceScr.Ey;
                default:
                    return Resources.ResourceScr.None;
            }
        }

        public static SelectList ToSelectList(this RepeatPosition position)
        {
            var positions = new List<Pair>
            {
                new Pair(){ ID =(int) AlertPosition.None , Text = AlertPosition.None.ToLocalizedString()},
                new Pair(){ ID =(int) RepeatPosition.Ed, Text = RepeatPosition.Ed.ToLocalizedString()},
                new Pair(){ ID =(int) RepeatPosition.Ew , Text = RepeatPosition.Ew.ToLocalizedString()},
                new Pair(){ ID =(int) RepeatPosition.E2w , Text = RepeatPosition.E2w.ToLocalizedString()},
                new Pair(){ ID =(int) RepeatPosition.Em , Text = RepeatPosition.Em.ToLocalizedString()},
                new Pair(){ ID =(int) RepeatPosition.Ey , Text = RepeatPosition.Ey.ToLocalizedString()}
            };
            return new SelectList(positions, "ID", "Text", (int)position);

        }
    }
}
