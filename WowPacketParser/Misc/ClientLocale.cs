using System;
using System.IO;
using WowPacketParser.Enums;


namespace WowPacketParser.Misc
{
    public static class ClientLocale
    {
        public static string ClientLocaleString = "enUS";

        public static string PacketLocaleString = "enUS";

        public static LocaleConstant PacketLocale => GetLocaleConstantFromLocaleName(PacketLocaleString);

        public static LocaleConstant GetLocaleConstantFromLocaleName(string locale)
        {
            return (LocaleConstant)Enum.Parse(typeof(LocaleConstant), locale);
        }

        public static int GetLocaleIndexFromLocaleName(string locale)
        {
            return (int)GetLocaleConstantFromLocaleName(locale);
        }

        public static void SetLocale(string locale)
        {
            if (locale == string.Empty)
                throw new InvalidDataException("No Locale in packet");

            ClientLocaleString = locale;

            // enGB contains same data as enUS
            if (locale == "enGB")
                PacketLocaleString = "enUS";
            else
                PacketLocaleString = locale;
        }
    }
}
