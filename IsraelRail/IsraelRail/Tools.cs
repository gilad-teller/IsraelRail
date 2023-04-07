using IsraelRail.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsraelRail
{
    public static class Tools
    {
        public static DayOfWeek ParseToDayOfWeek(this string day)
        {
            int dayInt = int.Parse(day) - 1;
            return (DayOfWeek)dayInt;
        }

        public static Route SelectRoute(IEnumerable<Route> routes, DateTime dateTime, bool isDepart)
        {
            Route selectedRoute = null;
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
