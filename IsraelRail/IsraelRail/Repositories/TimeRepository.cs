using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsraelRail.Repositories
{
    public interface ITime
    {
        DateTime NowInLocal();
    }

    public class TimeRepository : ITime
    {
        private readonly IConfiguration _config;
        private readonly TimeZoneInfo _timeZoneInfo;

        public TimeRepository(IConfiguration config)
        {
            _config = config;
            _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_config.GetValue<string>("AppSettings:TimeZone"));
        }

        public DateTime NowInLocal()
        {
            return TimeZoneInfo.ConvertTime(DateTime.Now, _timeZoneInfo);
        }
    }
}
