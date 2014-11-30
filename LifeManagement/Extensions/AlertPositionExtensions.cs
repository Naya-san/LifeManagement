using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using LifeManagement.Enums;

namespace LifeManagement.Extensions
{
    public static class AlertPositionExtensions
    {
        public static string ToLocalizedString(this AlertPosition alertPosition)
        {
            switch (alertPosition)
            {
                case AlertPosition.Simultaneously:
                    return Resources.ResourceScr.Simultaneously;
                case AlertPosition.B5M:
                    return Resources.ResourceScr.b5m;
                case AlertPosition.B10M:
                    return Resources.ResourceScr.b10m;
                case AlertPosition.B15M:
                    return Resources.ResourceScr.b15m;
                case AlertPosition.B1H:
                    return Resources.ResourceScr.b1h;
                case AlertPosition.B2H:
                    return Resources.ResourceScr.b2h;
                case AlertPosition.B1D:
                    return Resources.ResourceScr.b1d;
                case AlertPosition.B2D:
                    return Resources.ResourceScr.b2d;
                case AlertPosition.B30M:
                    return Resources.ResourceScr.b30m;
                case AlertPosition.Wb:
                    return Resources.ResourceScr.wb;
                default:
                    return Resources.ResourceScr.None;
            }
        }

        public static SelectList ToSelectList(this AlertPosition alert)
        {
            List<Pair> positions = new List<Pair>
            {
                new Pair(){ ID =(int) AlertPosition.None , Text = AlertPosition.None.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.Simultaneously, Text = AlertPosition.Simultaneously.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B5M , Text = AlertPosition.B5M.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B10M , Text = AlertPosition.B10M.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B15M , Text = AlertPosition.B15M.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B30M , Text = AlertPosition.B30M.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B1H, Text = AlertPosition.B1H.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B2H , Text = AlertPosition.B2H.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B1D , Text = AlertPosition.B1D.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.B2D , Text = AlertPosition.B2D.ToLocalizedString()},
                new Pair(){ ID =(int) AlertPosition.Wb , Text = AlertPosition.Wb.ToLocalizedString()}
            };
            return new SelectList(positions, "ID", "Text", (int)alert);

        }

        public static AlertPosition Parse(this AlertPosition alert, int code)
        {
            AlertPosition tmp;
            try
            {
                tmp = (AlertPosition)code;
            }
            catch (Exception)
            {
                tmp = AlertPosition.None;
            }
            return tmp;
        }
    }

    public class Pair
    {
        public int ID { set; get; }
        public String Text { set; get; }
    }
}