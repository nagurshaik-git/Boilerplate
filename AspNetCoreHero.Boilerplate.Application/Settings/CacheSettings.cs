﻿namespace AspNetCoreHero.Boilerplate.Application.Settings;

public class CacheSettings
{
    public int AbsoluteExpirationInHours { get; set; }
    public int SlidingExpirationInMinutes { get; set; }
}
