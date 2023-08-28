using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.SQL;

namespace WowPacketParser.Store.Objects
{
    [DBTableName("trainer")]
    public sealed class Trainer : IDataModel
    {
        [DBFieldName("id", true, DbType = (TargetedDbType.WPP))]
        [DBFieldName("Id", true, DbType = (TargetedDbType.TRINITY))]
        public uint? Id;

        [DBFieldName("type", true, DbType = (TargetedDbType.WPP))]
        [DBFieldName("Type", DbType = (TargetedDbType.TRINITY))]
        public TrainerType? Type;

        [DBFieldName("greeting", true, DbType = (TargetedDbType.WPP))]
        [DBFieldName("Greeting", DbType = (TargetedDbType.TRINITY))]
        public string Greeting;

        [DBFieldName("sniff_build", DbType = (TargetedDbType.WPP))]
        [DBFieldName("VerifiedBuild", DbType = (TargetedDbType.TRINITY))]
        public int? VerifiedBuild = ClientVersion.BuildInt;
    }

    [DBTableName("trainer_locale")]
    public sealed class TrainerLocale : IDataModel
    {
        [DBFieldName("Id", true)]
        public uint Id;

        public uint TrainerEntry;

        [DBFieldName("locale", true)]
        public string Locale = ClientLocale.PacketLocaleString;

        [DBFieldName("Greeting")]
        public string Greeting;

        [DBFieldName("VerifiedBuild")]
        public int? VerifiedBuild = ClientVersion.BuildInt;
    }
}
