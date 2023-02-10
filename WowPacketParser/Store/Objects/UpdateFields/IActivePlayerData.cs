using WowPacketParser.Misc;

namespace WowPacketParser.Store.Objects.UpdateFields
{
    public interface IActivePlayerData
    {
        WowGuid FarsightObject { get; }
        ulong Coinage { get; }
        int XP { get; }
        ISkillInfo Skill { get; }
        float DodgePercentage { get; }
        float CritPercentage { get; }
        float RangedCritPercentage { get; }
        float OffhandCritPercentage { get; }
        float SpellCritPercentage { get;}
        IActivePlayerData Clone();
    }
}
