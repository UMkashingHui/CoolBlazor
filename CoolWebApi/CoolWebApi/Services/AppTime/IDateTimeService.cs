using System;

namespace CoolWebApi.Services.AppTime
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}