﻿using WowPacketParser.Misc;

namespace WowPacketParser.Store.Objects.UpdateFields
{
    public interface IUnitData
    {
        int DisplayID { get; }
        int NativeDisplayID { get; }
        int MountDisplayID { get; }
        uint[] NpcFlags { get; }
        WowGuid Charm { get; }
        WowGuid Summon { get; }
        WowGuid CharmedBy { get; }
        WowGuid SummonedBy { get; }
        WowGuid CreatedBy { get; }
        WowGuid Target { get; }
        WowGuid DemonCreator { get; }
        byte ClassId { get; }
        byte RaceId { get; }
        byte Sex { get; }
        int Level { get; }
        int ContentTuningID { get; }
        int ScalingLevelMin { get; }
        int ScalingLevelMax { get; }
        int ScalingLevelDelta { get; }
        int FactionTemplate { get; }
        byte DisplayPower { get; }
        long Health { get; }
        long MaxHealth { get; }
        uint HealthPercent { get; }
        int[] Power { get; }
        int[] MaxPower { get; }
        int Mana { get; }
        int MaxMana { get; }
        int[] Stats { get; }
        int[] StatPosBuff { get; }
        int[] StatNegBuff { get; }
        int[] Resistances { get; }
        int[] ResistanceBuffModsPositive { get; }
        int[] ResistanceBuffModsNegative { get; } 
        int BaseMana { get; }
        int BaseHealth { get; }
        IVisibleItem[] VirtualItems { get; }
        uint Flags { get; }
        uint Flags2 { get; }
        uint Flags3 { get; }
        uint DynamicFlags { get; }
        float MinDamage { get; }
        float MaxDamage { get; }
        float MinOffHandDamage { get; }
        float MaxOffHandDamage { get;}
        float MinRangedDamage { get; }
        float MaxRangedDamage { get; }
        int AttackPower { get; }
        int AttackPowerModPos { get; }
        int AttackPowerModNeg { get; }
        float AttackPowerMultiplier { get; }
        int RangedAttackPower { get; }
        int RangedAttackPowerModPos { get; }
        int RangedAttackPowerModNeg { get; }
        float RangedAttackPowerMultiplier { get; }
        uint[] AttackRoundBaseTime { get; }
        uint RangedAttackRoundBaseTime { get; }
        float BoundingRadius { get; }
        float CombatReach { get; }
        float ModHaste { get; }
        float ModRangedHaste { get; }
        uint AuraState { get; }
        int EmoteState { get; }
        byte StandState { get; }
        byte PetTalentPoints { get; }
        byte VisFlags { get; }
        byte AnimTier { get; }
        int CreatedBySpell { get; }
        byte SheatheState { get; }
        byte PvpFlags { get; }
        byte PetFlags { get; }
        byte ShapeshiftForm { get; }
        float HoverHeight { get; }
        int InteractSpellID { get; }
        IUnitChannel ChannelData { get; }
        WowGuid GuildGUID { get; }
        IUnitData Clone();
    }
}
