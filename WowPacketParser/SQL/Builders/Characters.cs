﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WowPacketParser.Enums;
using WowPacketParser.Enums.Version;
using WowPacketParser.Misc;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;

namespace WowPacketParser.SQL.Builders
{
    [BuilderClass]
    public static class Characters
    {
        private static Random random = new Random();
        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string randomString = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString.Substring(0, 1) + randomString.Substring(1).ToLower();
        }

        [BuilderMethod]
        public static string CharactersBuilder()
        {
            if (!Settings.SqlTables.characters && !Settings.SqlTables.player)
                return string.Empty;

            StringBuilder result = new StringBuilder();
            uint maxDbGuid = 0;
            uint itemGuidCounter = 0;
            var characterRows = new RowList<CharacterTemplate>();
            var characterInventoryRows = new RowList<CharacterInventory>();
            var characterItemInstaceRows = new RowList<CharacterItemInstance>();
            var characterReputationRows = new RowList<CharacterReputation>();
            var characterSkillRows = new RowList<CharacterSkill>();
            var characterSpellRows = new RowList<CharacterSpell>();
            var guildMemberRows = new RowList<GuildMember>();
            var playerRows = new RowList<PlayerTemplate>();
            var playerGuidValuesRows = new RowList<CreatureGuidValues>();
            var playerAttackLogRows = new RowList<UnitMeleeAttackLog>();
            var playerAttackStartRows = new RowList<CreatureAttackToggle>();
            var playerAttackStopRows = new RowList<CreatureAttackToggle>();
            var playerAurasUpdateRows = new RowList<CreatureAurasUpdate>();
            var playerCreate1Rows = new RowList<PlayerCreate1>();
            var playerCreate2Rows = new RowList<PlayerCreate2>();
            var playerDestroyRows = new RowList<PlayerDestroy>();
            var playerEmoteRows = new RowList<CreatureEmote>();
            var playerEquipmentValuesUpdateRows = new RowList<CreatureEquipmentValuesUpdate>();
            var playerGuidValuesUpdateRows = new RowList<CreatureGuidValuesUpdate>();
            var playerValuesUpdateRows = new RowList<CreatureValuesUpdate>();
            var playerSpeedUpdateRows = new RowList<CreatureSpeedUpdate>();
            var playerServerMovementRows = new RowList<ServerSideMovement>();
            var playerServerMovementSplineRows = new RowList<ServerSideMovementSpline>();
            Dictionary<WowGuid, uint> accountIdDictionary = new Dictionary<WowGuid, uint>();
            foreach (var objPair in Storage.Objects)
            {
                if (objPair.Key.GetObjectType() != ObjectType.Player)
                    continue;

                Player player = objPair.Value.Item1 as Player;
                if (player == null)
                    continue;

                if (!player.IsActivePlayer && Settings.SkipOtherPlayers)
                    continue;

                Row<CharacterTemplate> row = new Row<CharacterTemplate>();

                row.Data.Guid = "@PGUID+" + player.DbGuid;
                if (accountIdDictionary.ContainsKey(player.PlayerDataOriginal.WowAccount))
                    row.Data.Account = "@ACCID+" + accountIdDictionary[player.PlayerDataOriginal.WowAccount];
                else
                {
                    uint id = (uint)accountIdDictionary.Count;
                    accountIdDictionary.Add(player.PlayerDataOriginal.WowAccount, id);
                    row.Data.Account = "@ACCID+" + id;
                }

                row.Data.Name = Settings.RandomizePlayerNames ? GetRandomString(8) : StoreGetters.GetName(objPair.Key);
                row.Data.Race = player.UnitDataOriginal.RaceId;
                row.Data.Class = player.UnitDataOriginal.ClassId;
                row.Data.Gender = player.UnitDataOriginal.Sex;
                row.Data.Level = (uint)player.UnitDataOriginal.Level;
                row.Data.XP = player.PlayerDataOriginal.Experience;
                row.Data.Money = player.PlayerDataOriginal.Money;
                row.Data.PlayerBytes = player.PlayerDataOriginal.PlayerBytes1;
                row.Data.PlayerBytes2 = player.PlayerDataOriginal.PlayerBytes2;
                row.Data.PlayerFlags = player.PlayerDataOriginal.PlayerFlags;
                MovementInfo moveData = player.OriginalMovement == null ? player.Movement : player.OriginalMovement;
                if (moveData != null)
                {
                    row.Data.PositionX = moveData.Position.X;
                    row.Data.PositionY = moveData.Position.Y;
                    row.Data.PositionZ = moveData.Position.Z;
                    row.Data.Orientation = moveData.Orientation;
                }
                row.Data.Map = player.Map;
                row.Data.Health = (uint)player.UnitDataOriginal.MaxHealth;
                row.Data.Power1 = (uint)player.UnitDataOriginal.MaxMana;

                Store.Objects.UpdateFields.IVisibleItem[] visibleItems = player.PlayerDataOriginal.VisibleItems;

                for (int i = 0; i < 19; i++)
                {
                    int itemId = visibleItems[i].ItemID;
                    ushort enchantId = visibleItems[i].ItemVisual;

                    Row<CharacterInventory> inventoryRow = new Row<CharacterInventory>();
                    inventoryRow.Data.Guid = row.Data.Guid;
                    inventoryRow.Data.Bag = 0;
                    inventoryRow.Data.Slot = (uint)i;
                    inventoryRow.Data.ItemGuid = "@IGUID+" + itemGuidCounter;
                    inventoryRow.Data.ItemTemplate = (uint)itemId;
                    characterInventoryRows.Add(inventoryRow);

                    Row<CharacterItemInstance> itemInstanceRow = new Row<CharacterItemInstance>();
                    itemInstanceRow.Data.Guid = "@IGUID+" + itemGuidCounter;
                    itemInstanceRow.Data.ItemEntry = (uint)itemId;
                    itemInstanceRow.Data.OwnerGuid = row.Data.Guid;
                    characterItemInstaceRows.Add(itemInstanceRow);

                    itemGuidCounter++;

                    if (row.Data.EquipmentCache.Length > 0)
                        row.Data.EquipmentCache += " ";

                    row.Data.EquipmentCache += itemId + " " + enchantId;
                }

                characterRows.Add(row);

                if (maxDbGuid < player.DbGuid)
                    maxDbGuid = player.DbGuid;

                // Character wasn't actually seen in game, so there is no replay data.
                // Object was constructed from characters enum packet (before enter world).
                if (moveData == null)
                    continue;

                if (Settings.SqlTables.player)
                {
                    Row<PlayerTemplate> playerRow = new Row<PlayerTemplate>();
                    playerRow.Data.Guid = row.Data.Guid;
                    playerRow.Data.Name = row.Data.Name;
                    playerRow.Data.Race = row.Data.Race;
                    playerRow.Data.Class = row.Data.Class;
                    playerRow.Data.Gender = row.Data.Gender;
                    playerRow.Data.Level = row.Data.Level;
                    playerRow.Data.XP = row.Data.XP;
                    playerRow.Data.Money = row.Data.Money;
                    playerRow.Data.PlayerBytes = row.Data.PlayerBytes;
                    playerRow.Data.PlayerBytes2 = row.Data.PlayerBytes2;
                    playerRow.Data.PlayerFlags = row.Data.PlayerFlags;
                    playerRow.Data.PvPRank = player.PlayerDataOriginal.PvPRank;
                    playerRow.Data.PositionX = row.Data.PositionX;
                    playerRow.Data.PositionY = row.Data.PositionY;
                    playerRow.Data.PositionZ = row.Data.PositionZ;
                    playerRow.Data.Orientation = row.Data.Orientation;
                    playerRow.Data.Map = row.Data.Map;
                    playerRow.Data.DisplayID = (uint)player.UnitDataOriginal.DisplayID;
                    playerRow.Data.NativeDisplayID = (uint)player.UnitDataOriginal.NativeDisplayID;
                    playerRow.Data.MountDisplayID = (uint)player.UnitDataOriginal.MountDisplayID;
                    playerRow.Data.FactionTemplate = (uint)player.UnitDataOriginal.FactionTemplate;
                    playerRow.Data.UnitFlags = player.UnitDataOriginal.Flags;
                    playerRow.Data.UnitFlags2 = player.UnitDataOriginal.Flags2;
                    playerRow.Data.CurHealth = (uint)player.UnitDataOriginal.Health;
                    playerRow.Data.MaxHealth = (uint)player.UnitDataOriginal.MaxHealth;
                    playerRow.Data.CurMana = (uint)player.UnitDataOriginal.Mana;
                    playerRow.Data.MaxMana = (uint)player.UnitDataOriginal.MaxMana;
                    playerRow.Data.AuraState = player.UnitDataOriginal.AuraState;
                    playerRow.Data.EmoteState = (uint)player.UnitDataOriginal.EmoteState;
                    playerRow.Data.StandState = player.UnitDataOriginal.StandState;
                    //playerRow.Data.PetTalentPoints = player.UnitDataOriginal.PetTalentPoints;
                    playerRow.Data.VisFlags = player.UnitDataOriginal.VisFlags;
                    //playerRow.Data.AnimTier = player.UnitDataOriginal.AnimTier;
                    playerRow.Data.SheatheState = player.UnitDataOriginal.SheatheState;
                    playerRow.Data.PvpFlags = player.UnitDataOriginal.PvpFlags;
                    //playerRow.Data.PetFlags = player.UnitDataOriginal.PetFlags;
                    playerRow.Data.ShapeshiftForm = player.UnitDataOriginal.ShapeshiftForm;
                    playerRow.Data.SpeedWalk = moveData.WalkSpeed / MovementInfo.DEFAULT_WALK_SPEED;
                    playerRow.Data.SpeedRun = moveData.RunSpeed / MovementInfo.DEFAULT_RUN_SPEED;
                    playerRow.Data.SpeedRunBack = moveData.RunBackSpeed / MovementInfo.DEFAULT_RUN_BACK_SPEED;
                    playerRow.Data.SpeedSwim = moveData.SwimSpeed / MovementInfo.DEFAULT_SWIM_SPEED;
                    playerRow.Data.SpeedSwimBack = moveData.SwimBackSpeed / MovementInfo.DEFAULT_SWIM_BACK_SPEED;
                    playerRow.Data.SpeedFly = moveData.FlightSpeed / MovementInfo.DEFAULT_FLY_SPEED;
                    playerRow.Data.SpeedFlyBack = moveData.FlightBackSpeed / MovementInfo.DEFAULT_FLY_BACK_SPEED;
                    playerRow.Data.Scale = player.ObjectDataOriginal.Scale;
                    playerRow.Data.BoundingRadius = player.UnitDataOriginal.BoundingRadius;
                    playerRow.Data.CombatReach = player.UnitDataOriginal.CombatReach;
                    playerRow.Data.ModMeleeHaste = player.UnitDataOriginal.ModHaste;
                    playerRow.Data.MainHandAttackTime = player.UnitDataOriginal.AttackRoundBaseTime[0];
                    playerRow.Data.OffHandAttackTime = player.UnitDataOriginal.AttackRoundBaseTime[1];
                    playerRow.Data.RangedAttackTime = player.UnitDataOriginal.RangedAttackRoundBaseTime;
                    playerRow.Data.ChannelSpellId = (uint)player.UnitDataOriginal.ChannelData.SpellID;
                    playerRow.Data.ChannelVisualId = (uint)player.UnitDataOriginal.ChannelData.SpellVisual.SpellXSpellVisualID;
                    playerRow.Data.Auras = player.GetAurasString(false);
                    playerRow.Data.EquipmentCache = row.Data.EquipmentCache;
                    playerRows.Add(playerRow);
                }

                if (Settings.SqlTables.player_guid_values)
                {
                    if (!player.UnitDataOriginal.Charm.IsEmpty() ||
                        !player.UnitDataOriginal.Summon.IsEmpty() ||
                        !player.UnitDataOriginal.CharmedBy.IsEmpty() ||
                        !player.UnitDataOriginal.SummonedBy.IsEmpty() ||
                        !player.UnitDataOriginal.CreatedBy.IsEmpty() ||
                        !player.UnitDataOriginal.DemonCreator.IsEmpty() ||
                        !player.UnitDataOriginal.Target.IsEmpty())
                    {
                        Row<CreatureGuidValues> guidsRow = new Row<CreatureGuidValues>();
                        guidsRow.Data.GUID = row.Data.Guid;
                        Storage.GetObjectDbGuidEntryType(player.UnitDataOriginal.Charm, out guidsRow.Data.CharmGuid, out guidsRow.Data.CharmId, out guidsRow.Data.CharmType);
                        Storage.GetObjectDbGuidEntryType(player.UnitDataOriginal.Summon, out guidsRow.Data.SummonGuid, out guidsRow.Data.SummonId, out guidsRow.Data.SummonType);
                        Storage.GetObjectDbGuidEntryType(player.UnitDataOriginal.CharmedBy, out guidsRow.Data.CharmedByGuid, out guidsRow.Data.CharmedById, out guidsRow.Data.CharmedByType);
                        Storage.GetObjectDbGuidEntryType(player.UnitDataOriginal.SummonedBy, out guidsRow.Data.SummonedByGuid, out guidsRow.Data.SummonedById, out guidsRow.Data.SummonedByType);
                        Storage.GetObjectDbGuidEntryType(player.UnitDataOriginal.CreatedBy, out guidsRow.Data.CreatedByGuid, out guidsRow.Data.CreatedById, out guidsRow.Data.CreatedByType);
                        Storage.GetObjectDbGuidEntryType(player.UnitDataOriginal.DemonCreator, out guidsRow.Data.DemonCreatorGuid, out guidsRow.Data.DemonCreatorId, out guidsRow.Data.DemonCreatorType);
                        Storage.GetObjectDbGuidEntryType(player.UnitDataOriginal.Target, out guidsRow.Data.TargetGuid, out guidsRow.Data.TargetId, out guidsRow.Data.TargetType);
                        playerGuidValuesRows.Add(guidsRow);
                    }
                }

                if (Settings.SqlTables.player_attack_log)
                {
                    if (Storage.UnitAttackLogs.ContainsKey(objPair.Key))
                    {
                        foreach (var attack in Storage.UnitAttackLogs[objPair.Key])
                        {
                            Row<UnitMeleeAttackLog> attackRow = new Row<UnitMeleeAttackLog>();
                            attackRow.Data = attack;
                            attackRow.Data.GUID = row.Data.Guid;
                            Storage.GetObjectDbGuidEntryType(attack.Victim, out attackRow.Data.VictimGuid, out attackRow.Data.VictimId, out attackRow.Data.VictimType);
                            attackRow.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(attack.Time);
                            playerAttackLogRows.Add(attackRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_attack_start)
                {
                    if (Storage.UnitAttackStartTimes.ContainsKey(objPair.Key))
                    {
                        foreach (var attack in Storage.UnitAttackStartTimes[objPair.Key])
                        {
                            Row<CreatureAttackToggle> attackRow = new Row<CreatureAttackToggle>();
                            attackRow.Data.GUID = row.Data.Guid;
                            Storage.GetObjectDbGuidEntryType(attack.victim, out attackRow.Data.VictimGuid, out attackRow.Data.VictimId, out attackRow.Data.VictimType);
                            attackRow.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(attack.time);
                            playerAttackStartRows.Add(attackRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_attack_stop)
                {
                    if (Storage.UnitAttackStopTimes.ContainsKey(objPair.Key))
                    {
                        foreach (var attack in Storage.UnitAttackStopTimes[objPair.Key])
                        {
                            Row<CreatureAttackToggle> attackRow = new Row<CreatureAttackToggle>();
                            attackRow.Data.GUID = row.Data.Guid;
                            Storage.GetObjectDbGuidEntryType(attack.victim, out attackRow.Data.VictimGuid, out attackRow.Data.VictimId, out attackRow.Data.VictimType);
                            attackRow.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(attack.time);
                            playerAttackStopRows.Add(attackRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_create1_time)
                {
                    if (Storage.ObjectCreate1Times.ContainsKey(objPair.Key))
                    {
                        foreach (var createTime in Storage.ObjectCreate1Times[objPair.Key])
                        {
                            var create1Row = new Row<PlayerCreate1>();
                            create1Row.Data.GUID = row.Data.Guid;
                            create1Row.Data.Map = createTime.Map;
                            create1Row.Data.PositionX = createTime.MoveInfo.Position.X;
                            create1Row.Data.PositionY = createTime.MoveInfo.Position.Y;
                            create1Row.Data.PositionZ = createTime.MoveInfo.Position.Z;
                            create1Row.Data.Orientation = createTime.MoveInfo.Orientation;
                            if (createTime.MoveInfo.TransportGuid != null && !createTime.MoveInfo.TransportGuid.IsEmpty())
                            {
                                create1Row.Data.TransportGuid = Storage.GetObjectDbGuid(createTime.MoveInfo.TransportGuid);
                                create1Row.Data.TransportPositionX = createTime.MoveInfo.TransportOffset.X;
                                create1Row.Data.TransportPositionY = createTime.MoveInfo.TransportOffset.Y;
                                create1Row.Data.TransportPositionZ = createTime.MoveInfo.TransportOffset.Z;
                                create1Row.Data.TransportOrientation = createTime.MoveInfo.TransportOffset.O;
                            }
                            create1Row.Data.MoveTime = createTime.MoveInfo.MoveTime;
                            create1Row.Data.MoveFlags = createTime.MoveInfo.Flags;
                            create1Row.Data.MoveFlags2 = createTime.MoveInfo.FlagsExtra;
                            create1Row.Data.SwimPitch = createTime.MoveInfo.SwimPitch;
                            create1Row.Data.FallTime = createTime.MoveInfo.FallTime;
                            create1Row.Data.JumpHorizontalSpeed = createTime.MoveInfo.JumpHorizontalSpeed;
                            create1Row.Data.JumpVerticalSpeed = createTime.MoveInfo.JumpVerticalSpeed;
                            create1Row.Data.JumpCosAngle = createTime.MoveInfo.JumpCosAngle;
                            create1Row.Data.JumpSinAngle = createTime.MoveInfo.JumpSinAngle;
                            create1Row.Data.SplineElevation = createTime.MoveInfo.SplineElevation;
                            create1Row.Data.UnixTimeMs = createTime.UnixTimeMs;
                            playerCreate1Rows.Add(create1Row);
                        }
                    }
                }

                if (Settings.SqlTables.player_create2_time)
                {
                    if (Storage.ObjectCreate2Times.ContainsKey(objPair.Key))
                    {
                        foreach (var createTime in Storage.ObjectCreate2Times[objPair.Key])
                        {
                            var create2Row = new Row<PlayerCreate2>();
                            create2Row.Data.GUID = row.Data.Guid;
                            create2Row.Data.Map = createTime.Map;
                            create2Row.Data.PositionX = createTime.MoveInfo.Position.X;
                            create2Row.Data.PositionY = createTime.MoveInfo.Position.Y;
                            create2Row.Data.PositionZ = createTime.MoveInfo.Position.Z;
                            create2Row.Data.Orientation = createTime.MoveInfo.Orientation;
                            if (createTime.MoveInfo.TransportGuid != null && !createTime.MoveInfo.TransportGuid.IsEmpty())
                            {
                                create2Row.Data.TransportGuid = Storage.GetObjectDbGuid(createTime.MoveInfo.TransportGuid);
                                create2Row.Data.TransportPositionX = createTime.MoveInfo.TransportOffset.X;
                                create2Row.Data.TransportPositionY = createTime.MoveInfo.TransportOffset.Y;
                                create2Row.Data.TransportPositionZ = createTime.MoveInfo.TransportOffset.Z;
                                create2Row.Data.TransportOrientation = createTime.MoveInfo.TransportOffset.O;
                            }
                            create2Row.Data.MoveTime = createTime.MoveInfo.MoveTime;
                            create2Row.Data.MoveFlags = createTime.MoveInfo.Flags;
                            create2Row.Data.MoveFlags2 = createTime.MoveInfo.FlagsExtra;
                            create2Row.Data.SwimPitch = createTime.MoveInfo.SwimPitch;
                            create2Row.Data.FallTime = createTime.MoveInfo.FallTime;
                            create2Row.Data.JumpHorizontalSpeed = createTime.MoveInfo.JumpHorizontalSpeed;
                            create2Row.Data.JumpVerticalSpeed = createTime.MoveInfo.JumpVerticalSpeed;
                            create2Row.Data.JumpCosAngle = createTime.MoveInfo.JumpCosAngle;
                            create2Row.Data.JumpSinAngle = createTime.MoveInfo.JumpSinAngle;
                            create2Row.Data.SplineElevation = createTime.MoveInfo.SplineElevation;
                            create2Row.Data.UnixTimeMs = createTime.UnixTimeMs;
                            playerCreate2Rows.Add(create2Row);
                        }
                    }
                }

                if (Settings.SqlTables.player_destroy_time)
                {
                    if (Storage.ObjectDestroyTimes.ContainsKey(objPair.Key))
                    {
                        foreach (var createTime in Storage.ObjectDestroyTimes[objPair.Key])
                        {
                            var destroyRow = new Row<PlayerDestroy>();
                            destroyRow.Data.GUID = row.Data.Guid;
                            destroyRow.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(createTime);
                            playerDestroyRows.Add(destroyRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_emote)
                {
                    if (Storage.Emotes.ContainsKey(objPair.Key))
                    {
                        foreach (var emote in Storage.Emotes[objPair.Key])
                        {
                            var emoteRow = new Row<CreatureEmote>();
                            emoteRow.Data = emote;
                            emoteRow.Data.GUID = row.Data.Guid;
                            playerEmoteRows.Add(emoteRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_equipment_values_update)
                {
                    if (Storage.UnitEquipmentValuesUpdates.ContainsKey(objPair.Key))
                    {
                        foreach (var update in Storage.UnitEquipmentValuesUpdates[objPair.Key])
                        {
                            Row<CreatureEquipmentValuesUpdate> updateRow = new Row<CreatureEquipmentValuesUpdate>();
                            updateRow.Data = update;
                            updateRow.Data.GUID = row.Data.Guid;
                            updateRow.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(update.time);
                            playerEquipmentValuesUpdateRows.Add(updateRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_guid_values_update)
                {
                    if (Storage.UnitGuidValuesUpdates.ContainsKey(objPair.Key))
                    {
                        foreach (var update in Storage.UnitGuidValuesUpdates[objPair.Key])
                        {
                            Row<CreatureGuidValuesUpdate> updateRow = new Row<CreatureGuidValuesUpdate>();
                            updateRow.Data.GUID = row.Data.Guid;
                            updateRow.Data.FieldName = update.FieldName;
                            Storage.GetObjectDbGuidEntryType(update.guid, out updateRow.Data.ObjectGuid, out updateRow.Data.ObjectId, out updateRow.Data.ObjectType);
                            updateRow.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(update.time);
                            playerGuidValuesUpdateRows.Add(updateRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_auras_update)
                {
                    if (Storage.UnitAurasUpdates.ContainsKey(objPair.Key))
                    {
                        uint updateId = 0;
                        foreach (var update in Storage.UnitAurasUpdates[objPair.Key])
                        {
                            updateId++;
                            foreach (var aura in update.Item1)
                            {
                                var updateRow = new Row<CreatureAurasUpdate>();
                                updateRow.Data.GUID = row.Data.Guid;
                                updateRow.Data.UpdateId = updateId;
                                updateRow.Data.Slot = aura.Slot;
                                updateRow.Data.SpellId = aura.SpellId;
                                updateRow.Data.VisualId = aura.VisualId;
                                updateRow.Data.AuraFlags = aura.AuraFlags;
                                updateRow.Data.ActiveFlags = aura.ActiveFlags;
                                updateRow.Data.Level = aura.Level;
                                updateRow.Data.Charges = aura.Charges;
                                updateRow.Data.ContentTuningId = aura.ContentTuningId;
                                updateRow.Data.Duration = aura.Duration;
                                updateRow.Data.MaxDuration = aura.MaxDuration;
                                Storage.GetObjectDbGuidEntryType(aura.CasterGuid, out updateRow.Data.CasterGuid, out updateRow.Data.CasterId, out updateRow.Data.CasterType);
                                updateRow.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(update.Item2);
                                playerAurasUpdateRows.Add(updateRow);
                            }
                        }
                    }
                }

                if (Settings.SqlTables.player_values_update)
                {
                    if (Storage.UnitValuesUpdates.ContainsKey(objPair.Key))
                    {
                        foreach (var update in Storage.UnitValuesUpdates[objPair.Key])
                        {
                            var updateRow = new Row<CreatureValuesUpdate>();
                            updateRow.Data = update;
                            updateRow.Data.GUID = row.Data.Guid;
                            playerValuesUpdateRows.Add(updateRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_speed_update)
                {
                    if (Storage.UnitSpeedUpdates.ContainsKey(objPair.Key))
                    {
                        foreach (var update in Storage.UnitSpeedUpdates[objPair.Key])
                        {
                            var updateRow = new Row<CreatureSpeedUpdate>();
                            updateRow.Data = update;
                            updateRow.Data.GUID = row.Data.Guid;
                            playerSpeedUpdateRows.Add(updateRow);
                        }
                    }
                }

                if (Settings.SqlTables.player_chat)
                {
                    foreach (var text in Storage.CharacterTexts)
                    {
                        if (text.Item1.SenderGUID == objPair.Key)
                        {
                            text.Item1.Guid = "@PGUID+" + player.DbGuid;
                            text.Item1.SenderName = row.Data.Name;
                        }
                    }
                }

                if (Settings.SqlTables.player_movement_server)
                {
                    foreach (ServerSideMovementSpline waypoint in player.CombatMovementSplines)
                    {
                        var movementSplineRow = new Row<ServerSideMovementSpline>();
                        movementSplineRow.Data = waypoint;
                        movementSplineRow.Data.GUID = "@PGUID+" + player.DbGuid;
                        playerServerMovementSplineRows.Add(movementSplineRow);
                    }

                    foreach (ServerSideMovement waypoint in player.CombatMovements)
                    {
                        if (waypoint == null)
                            break;

                        var movementRow = new Row<ServerSideMovement>();
                        movementRow.Data = waypoint;
                        movementRow.Data.GUID = "@PGUID+" + player.DbGuid;
                        if (waypoint.TransportGuid != null && !waypoint.TransportGuid.IsEmpty())
                            movementRow.Data.TransportGUID = Storage.GetObjectDbGuid(waypoint.TransportGuid);
                        playerServerMovementRows.Add(movementRow);
                    }
                }

                if (Settings.SqlTables.character_reputation)
                {
                    if (Storage.CharacterReputations.ContainsKey(objPair.Key))
                    {
                        foreach (var repData in Storage.CharacterReputations[objPair.Key])
                        {
                            if (repData.Standing != 0 || repData.Flags != 0)
                            {
                                var repRow = new Row<CharacterReputation>();
                                repRow.Data.Guid = "@PGUID+" + player.DbGuid;
                                repRow.Data.Faction = repData.Faction;
                                repRow.Data.Standing = repData.Standing;
                                repRow.Data.Flags = repData.Flags;
                                characterReputationRows.Add(repRow);
                            }
                        }
                    }
                }

                if (Settings.SqlTables.character_skills)
                {
                    if (ClientVersion.Expansion == ClientType.Classic)
                    {
                        int skillsField = UpdateFields.GetUpdateField(ActivePlayerField.ACTIVE_PLAYER_FIELD_SKILL_LINEID);
                        if (skillsField > 0)
                        {
                            const uint PLAYER_MAX_SKILLS = 256;
                            const uint SKILL_FIELD_ARRAY_SIZE = 256 / 4 * 2;
                            const uint SKILL_ID_OFFSET = 0;
                            const uint SKILL_STEP_OFFSET = SKILL_ID_OFFSET + SKILL_FIELD_ARRAY_SIZE;
                            const uint SKILL_RANK_OFFSET = SKILL_STEP_OFFSET + SKILL_FIELD_ARRAY_SIZE;
                            const uint SUBSKILL_START_RANK_OFFSET = SKILL_RANK_OFFSET + SKILL_FIELD_ARRAY_SIZE;
                            const uint SKILL_MAX_RANK_OFFSET = SUBSKILL_START_RANK_OFFSET + SKILL_FIELD_ARRAY_SIZE;

                            for (uint i = 0; i < PLAYER_MAX_SKILLS; ++i)
                            {
                                uint field = i / 2;
                                uint offset = i & 1; // i % 2

                                uint skillId = 0;
                                uint skillStep = 0;
                                uint skillRank = 0;
                                uint skillMaxRank = 0;

                                UpdateField value;
                                if (player.UpdateFields.TryGetValue((int)(skillsField + SKILL_ID_OFFSET + field), out value))
                                    skillId = (value.UInt32Value >> (offset == 1 ? 16 : 0)) & 0xFFFF;

                                if (player.UpdateFields.TryGetValue((int)(skillsField + SKILL_STEP_OFFSET + field), out value))
                                    skillStep = (value.UInt32Value >> (offset == 1 ? 16 : 0)) & 0xFFFF;

                                if (player.UpdateFields.TryGetValue((int)(skillsField + SKILL_RANK_OFFSET + field), out value))
                                    skillRank = (value.UInt32Value >> (offset == 1 ? 16 : 0)) & 0xFFFF;

                                if (player.UpdateFields.TryGetValue((int)(skillsField + SKILL_MAX_RANK_OFFSET + field), out value))
                                    skillMaxRank = (value.UInt32Value >> (offset == 1 ? 16 : 0)) & 0xFFFF;

                                if (skillId != 0 && skillMaxRank != 0)
                                {
                                    var skillRow = new Row<CharacterSkill>();
                                    skillRow.Data.Guid = "@PGUID+" + player.DbGuid;
                                    skillRow.Data.Skill = skillId;
                                    skillRow.Data.Value = skillRank;
                                    skillRow.Data.Max = skillMaxRank;
                                    characterSkillRows.Add(skillRow);
                                }
                            }
                        }
                    }
                }

                if (Settings.SqlTables.character_spell)
                {
                    if (Storage.CharacterSpells.ContainsKey(objPair.Key))
                    {
                        foreach (var spellId in Storage.CharacterSpells[objPair.Key])
                        {
                            var spellRow = new Row<CharacterSpell>();
                            spellRow.Data.Guid = "@PGUID+" + player.DbGuid;
                            spellRow.Data.Spell = spellId;
                            spellRow.Data.Active = 1;
                            spellRow.Data.Disabled = 0;
                            characterSpellRows.Add(spellRow);
                        }
                    }
                }

                if (Settings.SqlTables.guild)
                {
                    if (player.UnitData.GuildGUID.Low != 0)
                    {
                        var guildRow = new Row<GuildMember>();
                        guildRow.Data.GuildGUID = player.UnitDataOriginal.GuildGUID.Low;
                        guildRow.Data.Guid = "@PGUID+" + player.DbGuid;
                        guildRow.Data.GuildRank = player.PlayerDataOriginal.GuildRankID;
                        guildMemberRows.Add(guildRow);
                    }
                }
            }

            if (Settings.SqlTables.characters && characterRows.Count != 0)
            {
                var characterDelete = new SQLDelete<CharacterTemplate>(Tuple.Create("@PGUID+0", "@PGUID+" + maxDbGuid));
                result.Append(characterDelete.Build());
                var characterSql = new SQLInsert<CharacterTemplate>(characterRows, false);
                result.Append(characterSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.character_inventory && characterInventoryRows.Count != 0)
            {
                var inventoryDelete = new SQLDelete<CharacterInventory>(Tuple.Create("@IGUID+0", "@IGUID+" + itemGuidCounter));
                result.Append(inventoryDelete.Build());
                var inventorySql = new SQLInsert<CharacterInventory>(characterInventoryRows, false);
                result.Append(inventorySql.Build());
                result.AppendLine();

                var itemInstanceDelete = new SQLDelete<CharacterItemInstance>(Tuple.Create("@IGUID+0", "@IGUID+" + itemGuidCounter));
                result.Append(itemInstanceDelete.Build());
                var itemInstanceSql = new SQLInsert<CharacterItemInstance>(characterItemInstaceRows, false);
                result.Append(itemInstanceSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.character_reputation && characterReputationRows.Count != 0)
            {
                var repSql = new SQLInsert<CharacterReputation>(characterReputationRows, false);
                result.Append(repSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.character_skills && characterSkillRows.Count != 0)
            {
                var skillsSql = new SQLInsert<CharacterSkill>(characterSkillRows, false);
                result.Append(skillsSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.character_spell && characterSpellRows.Count != 0)
            {
                var spellsSql = new SQLInsert<CharacterSpell>(characterSpellRows, false);
                result.Append(spellsSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.guild && guildMemberRows.Count != 0)
            {
                var guildSql = new SQLInsert<GuildMember>(guildMemberRows, false);
                result.Append(guildSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player && playerRows.Count != 0)
            {
                var playerDelete = new SQLDelete<PlayerTemplate>(Tuple.Create("@PGUID+0", "@PGUID+" + maxDbGuid));
                result.Append(playerDelete.Build());
                var playerSql = new SQLInsert<PlayerTemplate>(playerRows, false);
                result.Append(playerSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_guid_values && playerGuidValuesRows.Count != 0)
            {
                var guidValuesDelete = new SQLDelete<CreatureGuidValues>(Tuple.Create("@PGUID+0", "@PGUID+" + maxDbGuid));
                guidValuesDelete.tableNameOverride = "player_guid_values";
                result.Append(guidValuesDelete.Build());
                var guidValuesSql = new SQLInsert<CreatureGuidValues>(playerGuidValuesRows, false, false, "player_guid_values");
                result.Append(guidValuesSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_active_player && Storage.PlayerActiveCreateTime.Count != 0)
            {
                var activePlayersRows = new RowList<CharacterActivePlayer>();
                foreach (var itr in Storage.PlayerActiveCreateTime)
                {
                    Row<CharacterActivePlayer> row = new Row<CharacterActivePlayer>();
                    row.Data.Guid = Storage.GetObjectDbGuid(itr.Guid);
                    row.Data.UnixTime = (uint)Utilities.GetUnixTimeFromDateTime(itr.Time);
                    activePlayersRows.Add(row);
                }
                var activePlayersSql = new SQLInsert<CharacterActivePlayer>(activePlayersRows, true);
                result.Append(activePlayersSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_create1_time && playerCreate1Rows.Count != 0)
            {
                var createSql = new SQLInsert<PlayerCreate1>(playerCreate1Rows, false);
                result.Append(createSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_create2_time && playerCreate2Rows.Count != 0)
            {
                var createSql = new SQLInsert<PlayerCreate2>(playerCreate2Rows, false);
                result.Append(createSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_destroy_time && playerDestroyRows.Count != 0)
            {
                var destroySql = new SQLInsert<PlayerDestroy>(playerDestroyRows, false);
                result.Append(destroySql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_movement_client && Storage.PlayerMovements.Count != 0)
            {
                uint moveCounter = 0;
                var movementRows = new RowList<ClientSideMovement>();
                foreach (var movement in Storage.PlayerMovements)
                {
                    if (Storage.Objects.ContainsKey(movement.Guid))
                    {
                        Player player = Storage.Objects[movement.Guid].Item1 as Player;
                        if (player == null)
                            continue;

                        if (Settings.SkipOtherPlayers && !player.IsActivePlayer &&
                           (movement.OpcodeDirection != Direction.ClientToServer))
                            continue;

                        Row<ClientSideMovement> row = new Row<ClientSideMovement>();
                        row.Data.Guid = "@PGUID+" + player.DbGuid;
                        row.Data.MoveFlags = movement.MoveInfo.Flags;
                        row.Data.MoveFlags2 = movement.MoveInfo.FlagsExtra;
                        row.Data.MoveTime = movement.MoveInfo.MoveTime;
                        row.Data.Map = movement.Map;
                        row.Data.PositionX = movement.MoveInfo.Position.X;
                        row.Data.PositionY = movement.MoveInfo.Position.Y;
                        row.Data.PositionZ = movement.MoveInfo.Position.Z;
                        row.Data.Orientation = movement.MoveInfo.Orientation;
                        if (movement.MoveInfo.TransportGuid != null && !movement.MoveInfo.TransportGuid.IsEmpty())
                        {
                            row.Data.TransportGuid = Storage.GetObjectDbGuid(movement.MoveInfo.TransportGuid);
                            row.Data.TransportPositionX = movement.MoveInfo.TransportOffset.X;
                            row.Data.TransportPositionY = movement.MoveInfo.TransportOffset.Y;
                            row.Data.TransportPositionZ = movement.MoveInfo.TransportOffset.Z;
                            row.Data.TransportOrientation = movement.MoveInfo.TransportOffset.O;
                        }
                        row.Data.SwimPitch = movement.MoveInfo.SwimPitch;
                        row.Data.FallTime = movement.MoveInfo.FallTime;
                        row.Data.JumpHorizontalSpeed = movement.MoveInfo.JumpHorizontalSpeed;
                        row.Data.JumpVerticalSpeed = movement.MoveInfo.JumpVerticalSpeed;
                        row.Data.JumpCosAngle = movement.MoveInfo.JumpCosAngle;
                        row.Data.JumpSinAngle = movement.MoveInfo.JumpSinAngle;
                        row.Data.SplineElevation = movement.MoveInfo.SplineElevation;
                        row.Data.PacketId = moveCounter++;
                        row.Data.Opcode = Opcodes.GetOpcodeName(movement.Opcode, movement.OpcodeDirection);
                        row.Data.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(movement.Time);
                        movementRows.Add(row);
                    }
                }

                var movementSql = new SQLInsert<ClientSideMovement>(movementRows, false);
                result.Append(movementSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_movement_server && playerServerMovementRows.Count != 0)
            {
                var movementSql = new SQLInsert<ServerSideMovement>(playerServerMovementRows, false, false, "player_movement_server");
                result.Append(movementSql.Build());
                result.AppendLine();

                var movementSplineSql = new SQLInsert<ServerSideMovementSpline>(playerServerMovementSplineRows, false, false, "player_movement_server_spline");
                result.Append(movementSplineSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_attack_log && playerAttackLogRows.Count != 0)
            {
                var attackLogSql = new SQLInsert<UnitMeleeAttackLog>(playerAttackLogRows, false, false, "player_attack_log");
                result.Append(attackLogSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_attack_start && playerAttackStartRows.Count != 0)
            {
                var attackStartSql = new SQLInsert<CreatureAttackToggle>(playerAttackStartRows, false, false, "player_attack_start");
                result.Append(attackStartSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_attack_stop && playerAttackStopRows.Count != 0)
            {
                var attackStopSql = new SQLInsert<CreatureAttackToggle>(playerAttackStopRows, false, false, "player_attack_stop");
                result.Append(attackStopSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_emote && playerEmoteRows.Count != 0)
            {
                var emoteSql = new SQLInsert<CreatureEmote>(playerEmoteRows, false, false, "player_emote");
                result.Append(emoteSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_equipment_values_update && playerEquipmentValuesUpdateRows.Count != 0)
            {
                var equipmentUpdateSql = new SQLInsert<CreatureEquipmentValuesUpdate>(playerEquipmentValuesUpdateRows, false, false, "player_equipment_values_update");
                result.Append(equipmentUpdateSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_guid_values_update && playerGuidValuesUpdateRows.Count != 0)
            {
                var guidsUpdateSql = new SQLInsert<CreatureGuidValuesUpdate>(playerGuidValuesUpdateRows, false, false, "player_guid_values_update");
                result.Append(guidsUpdateSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_auras_update && playerAurasUpdateRows.Count != 0)
            {
                var aurasUpdateSql = new SQLInsert<CreatureAurasUpdate>(playerAurasUpdateRows, false, false, "player_auras_update");
                result.Append(aurasUpdateSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_values_update && playerValuesUpdateRows.Count != 0)
            {
                var valuesUpdateSql = new SQLInsert<CreatureValuesUpdate>(playerValuesUpdateRows, false, false, "player_values_update");
                result.Append(valuesUpdateSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_speed_update && playerSpeedUpdateRows.Count != 0)
            {
                var speedUpdateSql = new SQLInsert<CreatureSpeedUpdate>(playerSpeedUpdateRows, false, false, "player_speed_update");
                result.Append(speedUpdateSql.Build());
                result.AppendLine();
            }

            if (Settings.SqlTables.player_chat && !Storage.CharacterTexts.IsEmpty())
            {
                foreach (var text in Storage.CharacterTexts)
                {
                    if (text.Item1.Guid == null)
                    {
                        text.Item1.Guid = "0";
                        if (String.IsNullOrEmpty(text.Item1.SenderName) && !text.Item1.SenderGUID.IsEmpty())
                            text.Item1.SenderName = StoreGetters.GetName(text.Item1.SenderGUID);
                    }
                    if (text.Item1.ChannelName == null)
                        text.Item1.ChannelName = "";
                }
                result.Append(SQLUtil.Compare(Storage.CharacterTexts, SQLDatabase.Get(Storage.CharacterTexts), t => t.SenderName, false));
            }

            return result.ToString();
        }

        [BuilderMethod]
        public static string PlayerClassLevelStats()
        {
            if (!Settings.SqlTables.player_classlevelstats && !Settings.SqlTables.player_levelstats)
                return string.Empty;

            foreach (var objPair in Storage.Objects)
            {
                if (objPair.Key.GetObjectType() != ObjectType.Player)
                    continue;

                Player player = objPair.Value.Item1 as Player;
                if (player == null)
                    continue;

                Storage.SavePlayerStats(player, true);
            }

            string result = "";

            if (Settings.SqlTables.player_classlevelstats &&
                !Storage.PlayerClassLevelStats.IsEmpty())
            {
                var templateDb = SQLDatabase.Get(Storage.PlayerClassLevelStats, Settings.TDBDatabase);

                result += SQLUtil.Compare(Storage.PlayerClassLevelStats, templateDb, StoreNameType.None);
            }

            if (Settings.SqlTables.player_levelstats &&
                !Storage.PlayerLevelStats.IsEmpty())
            {
                var templateDb = SQLDatabase.Get(Storage.PlayerLevelStats, Settings.TDBDatabase);

                result += SQLUtil.Compare(Storage.PlayerLevelStats, templateDb, StoreNameType.None);
            }

            return result;
        }

        [BuilderMethod]
        public static string PlayerLevelupInfos()
        {
            if (Storage.PlayerLevelupInfos.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.player_levelup_info)
                return string.Empty;

            var rows = new RowList<PlayerLevelupInfo>();
            foreach (var info in Storage.PlayerLevelupInfos)
            {
                if (info.Item1.GUID == null)
                    continue;
                if (!Storage.Objects.ContainsKey(info.Item1.GUID))
                    continue;

                Player player = Storage.Objects[info.Item1.GUID].Item1 as Player;
                if (player == null)
                    continue;

                Row<PlayerLevelupInfo> row = new Row<PlayerLevelupInfo>();
                row.Data = info.Item1;
                row.Data.RaceId = player.UnitData.RaceId;
                row.Data.ClassId = player.UnitData.ClassId;
                rows.Add(row);
            }

            var sql = new SQLInsert<PlayerLevelupInfo>(rows);
            return sql.Build();
        }
    }
}
