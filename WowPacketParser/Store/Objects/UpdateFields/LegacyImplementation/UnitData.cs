﻿using System.Collections.Generic;
using System.Linq;
using WowPacketParser.Enums;
using WowPacketParser.Misc;

namespace WowPacketParser.Store.Objects.UpdateFields.LegacyImplementation
{
    public class UnitData : IUnitData
    {
        private WoWObject Object { get; }
        private Dictionary<int, UpdateField> UpdateFields => Object.UpdateFields;

        public UnitData(WoWObject obj)
        {
            Object = obj;
        }

        private WowGuid GetGuidValue(UnitField field)
        {
            if (Enums.Version.UpdateFields.GetUpdateField(field) < 0)
                return WowGuid64.Empty;

            if (!ClientVersion.AddedInVersion(ClientType.WarlordsOfDraenor))
            {
                var parts = UpdateFields.GetArray<UnitField, uint>(field, 2);
                return new WowGuid64(Utilities.MAKE_PAIR64(parts[0], parts[1]));
            }
            else
            {
                var parts = UpdateFields.GetArray<UnitField, uint>(field, 4);
                return new WowGuid128(Utilities.MAKE_PAIR64(parts[0], parts[1]), Utilities.MAKE_PAIR64(parts[2], parts[3]));
            }
        }

        public int DisplayID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_DISPLAYID);

        public int NativeDisplayID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_NATIVEDISPLAYID);

        public int MountDisplayID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_MOUNTDISPLAYID);

        public uint[] NpcFlags
        {
            get
            {
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicVanillaClientVersionBuild(ClientVersion.Build))
                    return UpdateFields.GetArray<UnitField, uint>(UnitField.UNIT_NPC_FLAGS, 2);
                else
                    return new uint[] { UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_NPC_FLAGS), 0 };
            }
        }

        public int[] Stats
        {
            get
            {
                UnitField statsBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_STAT) > 0)
                    statsBegin = UnitField.UNIT_FIELD_STAT;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_STAT0) > 0)
                    statsBegin = UnitField.UNIT_FIELD_STAT0;
                else
                    return new int[] { 0, 0, 0, 0, 0 };

                int size = 5;
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicClientVersionBuild(ClientVersion.Build))
                    size = 4;

                return UpdateFields.GetArray<UnitField, int>(statsBegin, size);
            }
        }

        public int[] StatPosBuff
        {
            get
            {
                UnitField statsBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POSSTAT) > 0)
                    statsBegin = UnitField.UNIT_FIELD_POSSTAT;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POSSTAT0) > 0)
                    statsBegin = UnitField.UNIT_FIELD_POSSTAT0;
                else
                    return new int[] { 0, 0, 0, 0, 0 };

                int size = 5;
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicClientVersionBuild(ClientVersion.Build))
                    size = 4;

                return UpdateFields.GetArray<UnitField, int>(statsBegin, size);
            }
        }

        public int[] StatNegBuff
        {
            get
            {
                UnitField statsBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_NEGSTAT) > 0)
                    statsBegin = UnitField.UNIT_FIELD_NEGSTAT;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_NEGSTAT0) > 0)
                    statsBegin = UnitField.UNIT_FIELD_NEGSTAT0;
                else
                    return new int[] { 0, 0, 0, 0, 0 };

                int size = 5;
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicClientVersionBuild(ClientVersion.Build))
                    size = 4;

                return UpdateFields.GetArray<UnitField, int>(statsBegin, size);
            }
        }

        public WowGuid Charm => GetGuidValue(UnitField.UNIT_FIELD_CHARM);

        public WowGuid Summon => GetGuidValue(UnitField.UNIT_FIELD_SUMMON);

        public WowGuid CharmedBy => GetGuidValue(UnitField.UNIT_FIELD_CHARMEDBY);

        public WowGuid SummonedBy => GetGuidValue(UnitField.UNIT_FIELD_SUMMONEDBY);

        public WowGuid CreatedBy => GetGuidValue(UnitField.UNIT_FIELD_CREATEDBY);

        public WowGuid DemonCreator => GetGuidValue(UnitField.UNIT_FIELD_DEMON_CREATOR);

        public WowGuid Target => GetGuidValue(UnitField.UNIT_FIELD_TARGET);

        public byte RaceId => (byte)(UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) & 0xFF);
        public byte ClassId => (byte)((UpdateFields.GetValue<UnitField, uint?>(UnitField.UNIT_FIELD_BYTES_0).GetValueOrDefault((uint)Class.Warrior << 8) >> 8) & 0xFF);

        public byte Sex => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V5_4_0_17359)
                ? ((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) >> 24) & 0xFF)
                : ((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) >> 16) & 0xFF));

        public int Level => UpdateFields.GetValue<UnitField, int?>(UnitField.UNIT_FIELD_LEVEL).GetValueOrDefault(1);

        public int ContentTuningID => ClientVersion.AddedInVersion(ClientType.BattleForAzeroth)
                ? UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_CONTENT_TUNING_ID)
                : UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SANDBOX_SCALING_ID);

        public int ScalingLevelMin => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SCALING_LEVEL_MIN);

        public int ScalingLevelMax => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SCALING_LEVEL_MAX);

        public int ScalingLevelDelta => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SCALING_LEVEL_DELTA);

        public int FactionTemplate => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_FACTIONTEMPLATE);

        public int BaseHealth => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_BASE_HEALTH);

        public long Health => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_HEALTH);

        public long MaxHealth => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_MAXHEALTH);

        public uint HealthPercent => (uint)(((float)Health / (float)MaxHealth) * 100);

        public byte DisplayPower
        {
            get
            {
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_DISPLAY_POWER) > 0)
                    return (byte)UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_DISPLAY_POWER);

                return (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) >> 24) & 0xFF);
            }
        }

        public int BaseMana => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_BASE_MANA);

        public int[] Power
        {
            get
            {
                UnitField powersBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER) > 0)
                    powersBegin = UnitField.UNIT_FIELD_POWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER1) > 0)
                    powersBegin = UnitField.UNIT_FIELD_POWER1;
                else
                {
                    int[] powers = new int[ClientVersion.GetPowerCountForClientVersion()];
                    return powers;
                }

                int size = ClientVersion.GetPowerCountForClientVersion();

                return UpdateFields.GetArray<UnitField, int>(powersBegin, size);
            }
        }

        public int[] MaxPower
        {
            get
            {
                UnitField powersBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER) > 0)
                    powersBegin = UnitField.UNIT_FIELD_MAXPOWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER1) > 0)
                    powersBegin = UnitField.UNIT_FIELD_MAXPOWER1;
                else
                {
                    int[] powers = new int[ClientVersion.GetPowerCountForClientVersion()];
                    return powers;
                }

                int size = ClientVersion.GetPowerCountForClientVersion();

                return UpdateFields.GetArray<UnitField, int>(powersBegin, size);
            }
        }

        public int Mana
        {
            get
            {
                UnitField manaField;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER) > 0)
                    manaField = UnitField.UNIT_FIELD_POWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER1) > 0)
                    manaField = UnitField.UNIT_FIELD_POWER1;
                else
                    return 0;

                return UpdateFields.GetValue<UnitField, int>(manaField);
            }
        }

        public int MaxMana
        {
            get
            {
                UnitField manaField;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER) > 0)
                    manaField = UnitField.UNIT_FIELD_MAXPOWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER1) > 0)
                    manaField = UnitField.UNIT_FIELD_MAXPOWER1;
                else
                    return 0;

                return UpdateFields.GetValue<UnitField, int>(manaField);
            }
        }

        public IVisibleItem[] VirtualItems
        {
            get
            {
                if (ClientVersion.AddedInVersion(ClientType.Legion))
                {
                    var raw = UpdateFields.GetArray<UnitField, uint>(UnitField.UNIT_VIRTUAL_ITEM_SLOT_ID, 6);
                    var items = new VisibleItem[3];
                    for (var i = 0; i < 3; ++i)
                    {
                        items[i] = new VisibleItem
                        {
                            ItemID = (int)raw[i * 2],
                            ItemAppearanceModID = (ushort)(raw[i * 2 + 1] & 0xFFFF),
                            ItemVisual = (ushort)((raw[i * 2 + 1] >> 16) & 0xFFFF)
                        };
                    }
                    return items;
                }
                else if (ClientVersion.InVersion(ClientVersionBuild.Zero, ClientVersionBuild.V3_0_2_9056))
                {
                    return UpdateFields.GetArray<UnitField, int>(UnitField.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY, 3)
                        .Select(rawId => new VisibleItem { ItemID = rawId }).ToArray();
                }
                else
                    return UpdateFields.GetArray<UnitField, int>(UnitField.UNIT_VIRTUAL_ITEM_SLOT_ID, 3)
                        .Select(rawId => new VisibleItem { ItemID = rawId }).ToArray();
            }
        }

        public class UnitChannel : IUnitChannel
        {
            public int SpellID { get; set; }
            public ISpellCastVisual SpellVisual { get; set; }
        }

        public class SpellCastVisual : ISpellCastVisual
        {
            public int SpellXSpellVisualID { get; set; }
        }

        public IUnitChannel ChannelData
        {
            get
            {
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_CHANNEL_SPELL) > 0)
                {
                    int spellId = UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_CHANNEL_SPELL);
                    int visualId = UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_CHANNEL_SPELL_X_SPELL_VISUAL);
                    var channelData = new UnitChannel();
                    channelData.SpellID = spellId;
                    channelData.SpellVisual = new SpellCastVisual { SpellXSpellVisualID = visualId };
                    return channelData;
                }
                else
                {
                    var channelArray = UpdateFields.GetArray<UnitField, int>(UnitField.UNIT_FIELD_CHANNEL_DATA, 2);
                    var channelData = new UnitChannel();
                    channelData.SpellID = channelArray[0];
                    channelData.SpellVisual = new SpellCastVisual { SpellXSpellVisualID = channelArray[1] };
                    return channelData;
                }
            }
        }

        public uint Flags => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_FLAGS);

        public uint Flags2 => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_FLAGS_2);

        public uint Flags3 => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_FLAGS_3);

        public uint DynamicFlags => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_DYNAMIC_FLAGS);

        public uint[] AttackRoundBaseTime => UpdateFields.GetArray<UnitField, uint?>(UnitField.UNIT_FIELD_BASEATTACKTIME, 2)
            .Select(maybeAttackTime => maybeAttackTime.GetValueOrDefault(2000)).ToArray();

        public uint RangedAttackRoundBaseTime => UpdateFields.GetValue<UnitField, uint?>(UnitField.UNIT_FIELD_RANGEDATTACKTIME).GetValueOrDefault(2000);

        public float BoundingRadius => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_BOUNDINGRADIUS).GetValueOrDefault(0.306f);

        public float CombatReach => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_COMBATREACH).GetValueOrDefault(1.5f);

        public float ModHaste => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_MOD_HASTE).GetValueOrDefault(1.0f);

        public float ModRangedHaste => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_MOD_RANGED_HASTE).GetValueOrDefault(1.0f);

        public uint AuraState => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_AURASTATE);

        public int EmoteState => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_NPC_EMOTESTATE);

        public byte StandState => (byte)(UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) & 0xFF);

        public byte PetTalentPoints => (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 8) & 0xFF);

        public byte VisFlags => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 16) & 0xFF)
                : (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 24) & 0xFF));

        public byte AnimTier => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 24) & 0xFF)
                : 0);

        public int CreatedBySpell => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_CREATED_BY_SPELL);

        public byte SheatheState => (byte)(UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) & 0xFF);

        public byte DebuffLimit => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) &&
                                          ClientVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 8) & 0xFF)
                : 0);

        public byte PvpFlags => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 8) & 0xFF)
                : 0);

        public byte PetFlags => (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 16) & 0xFF);

        public byte ShapeshiftForm => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 24) & 0xFF)
                : (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 16) & 0xFF));

        public float HoverHeight => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_HOVERHEIGHT).GetValueOrDefault(1.0f);

        public int InteractSpellID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_INTERACT_SPELLID);

        public WowGuid GuildGUID => GetGuidValue(UnitField.UNIT_FIELD_GUILD_GUID);

        public class VisibleItem : IVisibleItem
        {
            public int ItemID { get; set; }
            public ushort ItemAppearanceModID { get; set; }
            public ushort ItemVisual { get; set; }

            public IVisibleItem Clone() { return (IVisibleItem)MemberwiseClone(); }
        }

        public IUnitData Clone() { return new UnitData(Object); }
    }
    public class OriginalUnitData : IUnitData
    {
        private WoWObject Object { get; }
        private Dictionary<int, UpdateField> UpdateFields => Object.OriginalUpdateFields;

        public OriginalUnitData(WoWObject obj)
        {
            Object = obj;
        }

        private WowGuid GetGuidValue(UnitField field)
        {
            if (Enums.Version.UpdateFields.GetUpdateField(field) < 0)
                return WowGuid64.Empty;

            if (!ClientVersion.AddedInVersion(ClientType.WarlordsOfDraenor))
            {
                var parts = UpdateFields.GetArray<UnitField, uint>(field, 2);
                return new WowGuid64(Utilities.MAKE_PAIR64(parts[0], parts[1]));
            }
            else
            {
                var parts = UpdateFields.GetArray<UnitField, uint>(field, 4);
                return new WowGuid128(Utilities.MAKE_PAIR64(parts[0], parts[1]), Utilities.MAKE_PAIR64(parts[2], parts[3]));
            }
        }

        public int DisplayID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_DISPLAYID);

        public int NativeDisplayID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_NATIVEDISPLAYID);

        public int MountDisplayID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_MOUNTDISPLAYID);

        public uint[] NpcFlags
        {
            get
            {
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicVanillaClientVersionBuild(ClientVersion.Build))
                    return UpdateFields.GetArray<UnitField, uint>(UnitField.UNIT_NPC_FLAGS, 2);
                else
                    return new uint[] { UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_NPC_FLAGS), 0 };
            }
        }

        public int[] Stats
        {
            get
            {
                UnitField statsBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_STAT) > 0)
                    statsBegin = UnitField.UNIT_FIELD_STAT;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_STAT0) > 0)
                    statsBegin = UnitField.UNIT_FIELD_STAT0;
                else
                    return new int[] { 0, 0, 0, 0, 0 };

                int size = 5;
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicClientVersionBuild(ClientVersion.Build))
                    size = 4;

                return UpdateFields.GetArray<UnitField, int>(statsBegin, size);
            }
        }

        public int[] StatPosBuff
        {
            get
            {
                UnitField statsBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POSSTAT) > 0)
                    statsBegin = UnitField.UNIT_FIELD_POSSTAT;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POSSTAT0) > 0)
                    statsBegin = UnitField.UNIT_FIELD_POSSTAT0;
                else
                    return new int[] { 0, 0, 0, 0, 0 };

                int size = 5;
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicClientVersionBuild(ClientVersion.Build))
                    size = 4;

                return UpdateFields.GetArray<UnitField, int>(statsBegin, size);
            }
        }

        public int[] StatNegBuff
        {
            get
            {
                UnitField statsBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_NEGSTAT) > 0)
                    statsBegin = UnitField.UNIT_FIELD_NEGSTAT;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_NEGSTAT0) > 0)
                    statsBegin = UnitField.UNIT_FIELD_NEGSTAT0;
                else
                    return new int[] { 0, 0, 0, 0, 0 };

                int size = 5;
                if (ClientVersion.AddedInVersion(ClientType.Legion) && !ClientVersion.IsClassicClientVersionBuild(ClientVersion.Build))
                    size = 4;

                return UpdateFields.GetArray<UnitField, int>(statsBegin, size);
            }
        }

        public WowGuid Charm => GetGuidValue(UnitField.UNIT_FIELD_CHARM);

        public WowGuid Summon => GetGuidValue(UnitField.UNIT_FIELD_SUMMON);

        public WowGuid CharmedBy => GetGuidValue(UnitField.UNIT_FIELD_CHARMEDBY);

        public WowGuid SummonedBy => GetGuidValue(UnitField.UNIT_FIELD_SUMMONEDBY);

        public WowGuid CreatedBy => GetGuidValue(UnitField.UNIT_FIELD_CREATEDBY);

        public WowGuid DemonCreator => GetGuidValue(UnitField.UNIT_FIELD_DEMON_CREATOR);

        public WowGuid Target => GetGuidValue(UnitField.UNIT_FIELD_TARGET);

        public byte RaceId => (byte)(UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) & 0xFF);
        public byte ClassId => (byte)((UpdateFields.GetValue<UnitField, uint?>(UnitField.UNIT_FIELD_BYTES_0).GetValueOrDefault((uint)Class.Warrior << 8) >> 8) & 0xFF);

        public byte Sex => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V5_4_0_17359)
                ? ((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) >> 24) & 0xFF)
                : ((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) >> 16) & 0xFF));

        public int Level => UpdateFields.GetValue<UnitField, int?>(UnitField.UNIT_FIELD_LEVEL).GetValueOrDefault(1);

        public int ContentTuningID => ClientVersion.AddedInVersion(ClientType.BattleForAzeroth)
                ? UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_CONTENT_TUNING_ID)
                : UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SANDBOX_SCALING_ID);

        public int ScalingLevelMin => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SCALING_LEVEL_MIN);

        public int ScalingLevelMax => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SCALING_LEVEL_MAX);

        public int ScalingLevelDelta => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_SCALING_LEVEL_DELTA);

        public int FactionTemplate => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_FACTIONTEMPLATE);

        public int BaseHealth => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_BASE_HEALTH);

        public long Health => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_HEALTH);

        public long MaxHealth => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_MAXHEALTH);

        public uint HealthPercent => (uint)(((float)Health / (float)MaxHealth) * 100);

        public byte DisplayPower
        {
            get
            {
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_DISPLAY_POWER) > 0)
                    return (byte)UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_DISPLAY_POWER);

                return (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_0) >> 24) & 0xFF);
            }
        }

        public int BaseMana => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_BASE_MANA);

        public int[] Power
        {
            get
            {
                UnitField powersBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER) > 0)
                    powersBegin = UnitField.UNIT_FIELD_POWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER1) > 0)
                    powersBegin = UnitField.UNIT_FIELD_POWER1;
                else
                {
                    int[] powers = new int[ClientVersion.GetPowerCountForClientVersion()];
                    return powers;
                }

                int size = ClientVersion.GetPowerCountForClientVersion();

                return UpdateFields.GetArray<UnitField, int>(powersBegin, size);
            }
        }

        public int[] MaxPower
        {
            get
            {
                UnitField powersBegin;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER) > 0)
                    powersBegin = UnitField.UNIT_FIELD_MAXPOWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER1) > 0)
                    powersBegin = UnitField.UNIT_FIELD_MAXPOWER1;
                else
                {
                    int[] powers = new int[ClientVersion.GetPowerCountForClientVersion()];
                    return powers;
                }

                int size = ClientVersion.GetPowerCountForClientVersion();

                return UpdateFields.GetArray<UnitField, int>(powersBegin, size);
            }
        }

        public int Mana
        {
            get
            {
                UnitField manaField;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER) > 0)
                    manaField = UnitField.UNIT_FIELD_POWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_POWER1) > 0)
                    manaField = UnitField.UNIT_FIELD_POWER1;
                else
                    return 0;

                return UpdateFields.GetValue<UnitField, int>(manaField);
            }
        }

        public int MaxMana
        {
            get
            {
                UnitField manaField;
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER) > 0)
                    manaField = UnitField.UNIT_FIELD_MAXPOWER;
                else if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_FIELD_MAXPOWER1) > 0)
                    manaField = UnitField.UNIT_FIELD_MAXPOWER1;
                else
                    return 0;

                return UpdateFields.GetValue<UnitField, int>(manaField);
            }
        }

        public IVisibleItem[] VirtualItems
        {
            get
            {
                if (ClientVersion.AddedInVersion(ClientType.Legion))
                {
                    var raw = UpdateFields.GetArray<UnitField, uint>(UnitField.UNIT_VIRTUAL_ITEM_SLOT_ID, 6);
                    var items = new VisibleItem[3];
                    for (var i = 0; i < 3; ++i)
                    {
                        items[i] = new VisibleItem
                        {
                            ItemID = (int)raw[i * 2],
                            ItemAppearanceModID = (ushort)(raw[i * 2 + 1] & 0xFFFF),
                            ItemVisual = (ushort)((raw[i * 2 + 1] >> 16) & 0xFFFF)
                        };
                    }
                    return items;
                }
                else if (ClientVersion.InVersion(ClientVersionBuild.Zero, ClientVersionBuild.V3_0_2_9056))
                {
                    return UpdateFields.GetArray<UnitField, int>(UnitField.UNIT_VIRTUAL_ITEM_SLOT_DISPLAY, 3)
                        .Select(rawId => new VisibleItem { ItemID = rawId }).ToArray();
                }
                else
                    return UpdateFields.GetArray<UnitField, int>(UnitField.UNIT_VIRTUAL_ITEM_SLOT_ID, 3)
                        .Select(rawId => new VisibleItem { ItemID = rawId }).ToArray();
            }
        }

        public class UnitChannel : IUnitChannel
        {
            public int SpellID { get; set; }
            public ISpellCastVisual SpellVisual { get; set; }
        }

        public class SpellCastVisual : ISpellCastVisual
        {
            public int SpellXSpellVisualID { get; set; }
        }

        public IUnitChannel ChannelData
        {
            get
            {
                if (Enums.Version.UpdateFields.GetUpdateField(UnitField.UNIT_CHANNEL_SPELL) > 0)
                {
                    int spellId = UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_CHANNEL_SPELL);
                    int visualId = UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_CHANNEL_SPELL_X_SPELL_VISUAL);
                    var channelData = new UnitChannel();
                    channelData.SpellID = spellId;
                    channelData.SpellVisual = new SpellCastVisual { SpellXSpellVisualID = visualId };
                    return channelData;
                }
                else
                {
                    var channelArray = UpdateFields.GetArray<UnitField, int>(UnitField.UNIT_FIELD_CHANNEL_DATA, 2);
                    var channelData = new UnitChannel();
                    channelData.SpellID = channelArray[0];
                    channelData.SpellVisual = new SpellCastVisual { SpellXSpellVisualID = channelArray[1] };
                    return channelData;
                }
            }
        }

        public uint Flags => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_FLAGS);

        public uint Flags2 => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_FLAGS_2);

        public uint Flags3 => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_FLAGS_3);

        public uint DynamicFlags => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_DYNAMIC_FLAGS);

        public uint[] AttackRoundBaseTime => UpdateFields.GetArray<UnitField, uint?>(UnitField.UNIT_FIELD_BASEATTACKTIME, 2)
            .Select(maybeAttackTime => maybeAttackTime.GetValueOrDefault(2000)).ToArray();

        public uint RangedAttackRoundBaseTime => UpdateFields.GetValue<UnitField, uint?>(UnitField.UNIT_FIELD_RANGEDATTACKTIME).GetValueOrDefault(2000);

        public float BoundingRadius => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_BOUNDINGRADIUS).GetValueOrDefault(0.306f);

        public float CombatReach => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_COMBATREACH).GetValueOrDefault(1.5f);

        public float ModHaste => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_MOD_HASTE).GetValueOrDefault(1.0f);

        public float ModRangedHaste => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_MOD_RANGED_HASTE).GetValueOrDefault(1.0f);

        public uint AuraState => UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_AURASTATE);

        public int EmoteState => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_NPC_EMOTESTATE);

        public byte StandState => (byte)(UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) & 0xFF);

        public byte PetTalentPoints => (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 8) & 0xFF);

        public byte VisFlags => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 16) & 0xFF)
                : (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 24) & 0xFF));

        public byte AnimTier => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 24) & 0xFF)
                : 0);

        public int CreatedBySpell => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_CREATED_BY_SPELL);

        public byte SheatheState => (byte)(UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) & 0xFF);

        public byte DebuffLimit => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) &&
                                          ClientVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 8) & 0xFF)
                : 0);

        public byte PvpFlags => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 8) & 0xFF)
                : 0);

        public byte PetFlags => (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 16) & 0xFF);

        public byte ShapeshiftForm => (byte)(ClientVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089)
                ? (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_2) >> 24) & 0xFF)
                : (byte)((UpdateFields.GetValue<UnitField, uint>(UnitField.UNIT_FIELD_BYTES_1) >> 16) & 0xFF));

        public float HoverHeight => UpdateFields.GetValue<UnitField, float?>(UnitField.UNIT_FIELD_HOVERHEIGHT).GetValueOrDefault(1.0f);

        public int InteractSpellID => UpdateFields.GetValue<UnitField, int>(UnitField.UNIT_FIELD_INTERACT_SPELLID);

        public WowGuid GuildGUID => GetGuidValue(UnitField.UNIT_FIELD_GUILD_GUID);

        public class VisibleItem : IVisibleItem
        {
            public int ItemID { get; set; }
            public ushort ItemAppearanceModID { get; set; }
            public ushort ItemVisual { get; set; }

            public IVisibleItem Clone() { return (IVisibleItem)MemberwiseClone(); }
        }

        public IUnitData Clone() { return new OriginalUnitData(Object); }
    }
}
