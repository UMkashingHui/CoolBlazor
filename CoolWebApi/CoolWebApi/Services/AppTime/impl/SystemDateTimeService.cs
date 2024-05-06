using System;
using CoolWebApi.Services.AppTime;

namespace CoolWebApi.Services.AppTime.impl
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}