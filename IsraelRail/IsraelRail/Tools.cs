using IsraelRail.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsraelRail
{
    public class Tools
    {
        public static TimeSpan? StopDelay(Trainposition pos, Delay delay)
        {
            return DelayFromPosition(pos) ?? DelayFromDelay(delay) ?? null;
        }

        public static TimeSpan? DelayFromPosition(Trainposition pos)
        {
            if (pos == null)
            {
                return null;
            }
            int minutes = pos.DifMin * (pos.DifType == "AHEAD" ? -1 : 1);
            return TimeSpan.FromMinutes(minutes);
        }

        private static TimeSpan? DelayFromDelay(Delay delay)
        {
            TimeSpan? ts = null;
            if (delay != null)
            {
                if (TimeSpan.TryParseExact(delay.Min, "mmmm", null, out TimeSpan temp))
                {
                    ts = temp;
                }
            }
            return ts;
        }

        public static DateTime NowInIsrael
        {
            get { return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Israel Standard Time"); }
        }

        public static Models.ViewModels.Route SelectRoute(IEnumerable<Models.ViewModels.Route> routes, DateTime dateTime, bool isDepart)
        {
            Models.ViewModels.Route selectedRoute = null;
            if (isDepart)
            {
                selectedRoute = routes.FirstOrDefault(x => x.Trains.FirstOrDefault().OrigintStop.StopTime.FirstOrDefault() >= dateTime) ?? routes.LastOrDefault();
            }
            else
            {
                selectedRoute = routes.LastOrDefault(x => x.Trains.LastOrDefault().DestinationStop.StopTime.FirstOrDefault() <= dateTime) ?? routes.FirstOrDefault();
            }
            return selectedRoute;
        }
    }
}
