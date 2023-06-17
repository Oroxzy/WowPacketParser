using WowPacketParser.SQL;
using WowPacketParser.Enums;
using WowPacketParser.Misc;

namespace WowPacketParser.Store.Objects
{
    [DBTableName("creature_spell_scaling_damage_periodic")]
    public class CreatureSpellScalingDamagePeriodic : IDataModel
    {
        [DBFieldName("entry", true)]
        public uint Entry;

        [DBFieldName("level", true)]
        public uint Level;

        [DBFieldName("spell_id", true)]
        public uint SpellId;

        [DBFieldName("hits_count")]
        public uint HitsCount;

        [DBFieldName("damage_min")]
        public int DamageMin;

        [DBFieldName("damage_average")]
        public int DamageAverage;

        [DBFieldName("damage_max")]
        public int DamageMax;

        [DBFieldName("sniff_build", true)]
        public int SniffBuild = ClientVersion.BuildInt;
    }
}
