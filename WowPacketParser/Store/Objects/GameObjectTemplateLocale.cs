using WowPacketParser.Misc;
using WowPacketParser.SQL;
using WowPacketParser.Enums;

namespace WowPacketParser.Store.Objects
{
    [DBTableName("gameobject_template_locale")]
    public sealed class GameObjectTemplateLocale : IDataModel
    {
        [DBFieldName("entry", true, DbType = (TargetedDbType.WPP | TargetedDbType.VMANGOS | TargetedDbType.CMANGOS))]
        [DBFieldName("ID", true, DbType = (TargetedDbType.TRINITY))]
        public uint? ID;

        [DBFieldName("locale", true)]
        public string Locale = ClientLocale.PacketLocaleString;

        [DBFieldName("name", DbType = (TargetedDbType.WPP | TargetedDbType.TRINITY | TargetedDbType.VMANGOS | TargetedDbType.CMANGOS))]
        public string Name;

        [DBFieldName("cast_bar_caption", DbType = (TargetedDbType.WPP))]
        [DBFieldName("castBarCaption", DbType = (TargetedDbType.TRINITY))]
        public string CastBarCaption;

        [DBFieldName("unk1", DbType = (TargetedDbType.WPP | TargetedDbType.TRINITY | TargetedDbType.VMANGOS | TargetedDbType.CMANGOS))]
        public string Unk1;

        [DBFieldName("sniff_build", DbType = (TargetedDbType.WPP))]
        [DBFieldName("VerifiedBuild", DbType = (TargetedDbType.TRINITY))]
        public int? VerifiedBuild = ClientVersion.BuildInt;
    }
}