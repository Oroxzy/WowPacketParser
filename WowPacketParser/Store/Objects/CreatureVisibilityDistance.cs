using WowPacketParser.SQL;
using WowPacketParser.Enums;
using WowPacketParser.Misc;

namespace WowPacketParser.Store.Objects
{
    [DBTableName("creature_visibility_distance")]
    public class CreatureVisibilityDistance : ITableWithSniffIdList
    {
        [DBFieldName("entry", true)]
        public uint Entry;

        [DBFieldName("map", true)]
        public uint Map;

        [DBFieldName("distance", true)]
        public uint Distance;
    }
}
