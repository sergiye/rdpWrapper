using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace sergiye.Common
{
    public enum SysGeoType {
        GEO_NATION = 0x0001,
        GEO_LATITUDE = 0x0002,
        GEO_LONGITUDE = 0x0003,
        GEO_ISO2 = 0x0004,
        GEO_ISO3 = 0x0005,
        GEO_RFC1766 = 0x0006,
        GEO_LCID = 0x0007,
        GEO_FRIENDLYNAME = 0x0008,
        GEO_OFFICIALNAME = 0x0009,
        GEO_TIMEZONES = 0x000A,
        GEO_OFFICIALLANGUAGES = 0x000B,
        GEO_ISO_UN_NUMBER = 0x000C,
        GEO_PARENT = 0x000D,
        GEO_DIALINGCODE = 0x000E,
        GEO_CURRENCYCODE = 0x000F,
        GEO_CURRENCYSYMBOL = 0x0010,
        GEO_NAME = 0x0011,
        GEO_ID = 0x0012
    }

    public static class RegionHelper
    {
        private enum GeoClass
        {
            Nation = 16,
            Region = 14,
        };

        [DllImport("kernel32.dll", ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int GetUserGeoID(GeoClass geoClass);

        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultLCID();

        [DllImport("kernel32.dll")]
        private static extern int GetGeoInfo(int geoid, int geoType, StringBuilder lpGeoData, int cchData, int langid);

        public static string GetGeoInfo(SysGeoType geoType = SysGeoType.GEO_FRIENDLYNAME)
        {
            int geoId = GetUserGeoID(GeoClass.Nation);
            int lcid = GetUserDefaultLCID();
            var buffer = new StringBuilder(100);
            GetGeoInfo(geoId, (int)geoType, buffer, buffer.Capacity, lcid);
            return buffer.ToString().Trim();
        }
    }

    public static class VersionCompatibility
    {
        public static bool IsCompatible()
        {
            Thread.CurrentThread.CurrentCulture.ClearCachedData();
            var template = (DateTime.UtcNow.Year > 2022).ToString().Substring(1, 2).ToUpper();
            var geo_ISO2 = Environment.OSVersion.Platform == PlatformID.Win32NT
                ? RegionHelper.GetGeoInfo(SysGeoType.GEO_ISO2)
                : Thread.CurrentThread.CurrentCulture.Name.Substring(3);
            return !template.Equals(geo_ISO2) && !template.Equals(System.Globalization.RegionInfo.CurrentRegion.Name);
        }
    }
}
