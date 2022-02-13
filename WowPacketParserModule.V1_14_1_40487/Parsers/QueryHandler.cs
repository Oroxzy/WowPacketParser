﻿using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;

namespace WowPacketParserModule.V1_14_1_40487.Parsers
{
    public static class QueryHandler
    {
        [Parser(Opcode.SMSG_QUERY_PLAYER_NAME_RESPONSE)]
        public static void HandleQueryPlayerNameResponse(Packet packet)
        {
            var hasData = packet.ReadByte("HasData");

            packet.ReadPackedGuid128("Player Guid");

            if (hasData == 0)
            {
                packet.ReadBit("IsDeleted");
                var nameLen = (int)packet.ReadBits(6);

                var count = new int[5];
                for (var i = 0; i < 5; ++i)
                    count[i] = (int)packet.ReadBits(7);

                for (var i = 0; i < 5; ++i)
                    packet.ReadWoWString("Name Declined", count[i], i);

                packet.ReadPackedGuid128("AccountID");
                packet.ReadPackedGuid128("BnetAccountID");
                WowGuid guid = packet.ReadPackedGuid128("Player Guid");

                packet.ReadUInt64("GuildClubMemberID");
                packet.ReadUInt32("VirtualRealmAddress");

                packet.ReadByteE<Race>("Race");
                packet.ReadByteE<Gender>("Gender");
                packet.ReadByteE<Class>("Class");
                packet.ReadByte("Level");
                packet.ReadByte("UnkBCC");

                string name = packet.ReadWoWString("Name", nameLen);

                StoreGetters.AddName(guid, name);
            }
        }

        [HasSniffData]
        [Parser(Opcode.SMSG_QUERY_CREATURE_RESPONSE)]
        public static void HandleCreatureQueryResponse(Packet packet)
        {
            var entry = packet.ReadEntry("Entry");

            CreatureTemplate creature = new CreatureTemplate
            {
                Entry = (uint)entry.Key
            };

            Bit hasData = packet.ReadBit();
            if (!hasData)
                return; // nothing to do

            packet.ResetBitReader();
            uint titleLen = packet.ReadBits(11);
            uint titleAltLen = packet.ReadBits(11);
            uint cursorNameLen = packet.ReadBits(6);
            creature.Civilian = packet.ReadBit("Civilian");
            creature.RacialLeader = packet.ReadBit("Leader");

            var stringLens = new int[4][];
            for (int i = 0; i < 4; i++)
            {
                stringLens[i] = new int[2];
                stringLens[i][0] = (int)packet.ReadBits(11);
                stringLens[i][1] = (int)packet.ReadBits(11);
            }

            for (var i = 0; i < 4; ++i)
            {
                if (stringLens[i][0] > 1)
                {
                    string name = packet.ReadDynamicString("Name", stringLens[i][0], i);
                    if (i == 0)
                        creature.Name = name;
                }
                if (stringLens[i][1] > 1)
                {
                    string nameAlt = packet.ReadDynamicString("NameAlt", stringLens[i][1], i);
                    if (i == 0)
                        creature.FemaleName = nameAlt;
                }
            }
            creature.TypeFlags = packet.ReadUInt32E<CreatureTypeFlag>("Type Flags");
            creature.TypeFlags2 = packet.ReadUInt32("Creature Type Flags 2");

            creature.Type = packet.ReadInt32E<CreatureType>("CreatureType");
            creature.Family = packet.ReadInt32E<CreatureFamily>("CreatureFamily");
            creature.Rank = packet.ReadInt32E<CreatureRank>("Classification");
            creature.PetSpellDataID = packet.ReadUInt32("PetSpellDataId");

            creature.KillCredits = new uint?[2];
            for (int i = 0; i < 2; ++i)
                creature.KillCredits[i] = (uint)packet.ReadInt32("ProxyCreatureID", i);

            creature.DisplayTotalCount = packet.ReadUInt32("DisplayIdCount");
            creature.DisplayTotalProbability = packet.ReadSingle("TotalProbability");

            creature.DisplayIDs = new uint?[4];
            for (uint i = 0; i < 4; ++i)
                creature.DisplayIDs[i] = 0;
            for (uint i = 0; i < creature.DisplayTotalCount; ++i)
            {
                if (i == 0)
                {
                    creature.DisplayIDs[i] = (uint)packet.ReadInt32("DisplayId1", i);
                    creature.DisplayScale1 = packet.ReadSingle("DisplayScale1", i);
                    creature.DisplayProbability1 = packet.ReadSingle("DisplayProbability1", i);
                }
                if (i == 1)
                {
                    creature.DisplayIDs[i] = (uint)packet.ReadInt32("DisplayId2", i);
                    creature.DisplayScale2 = packet.ReadSingle("DisplayScale2", i);
                    creature.DisplayProbability2 = packet.ReadSingle("DisplayProbability2", i);
                }

                if (i == 2)
                {
                    creature.DisplayIDs[i] = (uint)packet.ReadInt32("DisplayId3", i);
                    creature.DisplayScale3 = packet.ReadSingle("DisplayScale3", i);
                    creature.DisplayProbability3 = packet.ReadSingle("DisplayProbability3", i);
                }

                if (i == 3)
                {
                    creature.DisplayIDs[i] = (uint)packet.ReadInt32("DisplayId4", i);
                    creature.DisplayScale4 = packet.ReadSingle("DisplayScale4", i);
                    creature.DisplayProbability4 = packet.ReadSingle("DisplayProbability4", i);
                }
            }

            creature.HealthMultiplier = packet.ReadSingle("HpMulti");
            creature.ManaMultiplier = packet.ReadSingle("EnergyMulti");
            uint questItems = packet.ReadUInt32("QuestItems");
            creature.MovementID = (uint)packet.ReadInt32("CreatureMovementInfoID");
            creature.HealthScalingExpansion = packet.ReadInt32E<ClientType>("HealthScalingExpansion");
            creature.RequiredExpansion = packet.ReadInt32E<ClientType>("RequiredExpansion");
            creature.VignetteID = (uint)packet.ReadInt32("VignetteID");
            creature.UnitClass = (uint)packet.ReadInt32E<Class>("UnitClass");
            creature.DifficultyID = packet.ReadInt32("CreatureDifficultyID");
            creature.WidgetSetID = packet.ReadInt32("WidgetSetID");
            creature.WidgetSetUnitConditionID = packet.ReadInt32("WidgetSetUnitConditionID");

            if (titleLen > 1)
                creature.SubName = packet.ReadCString("Title");

            if (titleAltLen > 1)
                creature.TitleAlt = packet.ReadCString("TitleAlt");

            if (cursorNameLen > 1)
                creature.IconName = packet.ReadCString("CursorName");

            for (uint i = 0; i < questItems; ++i)
            {
                CreatureTemplateQuestItem questItem = new CreatureTemplateQuestItem
                {
                    CreatureEntry = (uint)entry.Key,
                    Idx = i,
                    ItemId = (uint)packet.ReadInt32<ItemId>("QuestItem", i)
                };

                Storage.CreatureTemplateQuestItems.Add(questItem, packet.TimeSpan);
            }

            packet.AddSniffData(StoreNameType.Unit, entry.Key, "QUERY_RESPONSE");

            Storage.CreatureTemplates.Add(creature, packet.TimeSpan);

            if (ClientLocale.PacketLocale != LocaleConstant.enUS)
            {
                CreatureTemplateLocale localesCreature = new CreatureTemplateLocale
                {
                    ID = (uint)entry.Key,
                    Name = creature.Name,
                    NameAlt = creature.FemaleName,
                    Title = creature.SubName,
                    TitleAlt = creature.TitleAlt
                };

                Storage.LocalesCreatures.Add(localesCreature, packet.TimeSpan);
            }

            ObjectName objectName = new ObjectName
            {
                ObjectType = StoreNameType.Unit,
                ID = entry.Key,
                Name = creature.Name
            };
            Storage.ObjectNames.Add(objectName, packet.TimeSpan);
        }

        [HasSniffData]
        [Parser(Opcode.SMSG_QUERY_GAME_OBJECT_RESPONSE, ClientVersionBuild.V1_14_0_39802, ClientVersionBuild.V1_14_1_40666)]
        public static void HandleGameObjectQueryResponse(Packet packet)
        {
            WowPacketParserModule.V8_0_1_27101.Parsers.GameObjectHandler.HandleGameObjectQueryResponse(packet);
        }

        [HasSniffData]
        [Parser(Opcode.SMSG_QUERY_GAME_OBJECT_RESPONSE, ClientVersionBuild.V1_14_1_40666)]
        public static void HandleGameObjectQueryResponse1141(Packet packet)
        {
            var entry = packet.ReadEntry("Entry");
            if (entry.Value) // entry is masked
                return;

            packet.ReadPackedGuid128("GUID");

            GameObjectTemplate gameObject = new GameObjectTemplate
            {
                Entry = (uint)entry.Key
            };

            packet.ReadBit("Allow");

            int dataSize = packet.ReadInt32("DataSize");
            if (dataSize == 0)
                return;

            gameObject.Type = packet.ReadInt32E<GameObjectType>("Type");

            gameObject.DisplayID = (uint)packet.ReadInt32("Display ID");

            var name = new string[4];
            for (int i = 0; i < 4; i++)
                name[i] = packet.ReadCString("Name", i);
            gameObject.Name = name[0];

            gameObject.IconName = packet.ReadCString("Icon Name");
            gameObject.CastCaption = packet.ReadCString("Cast Caption");
            gameObject.UnkString = packet.ReadCString("Unk String");

            gameObject.Data = new int?[35];
            for (int i = 0; i < gameObject.Data.Length; i++)
                gameObject.Data[i] = packet.ReadInt32("Data", i);

            gameObject.Size = packet.ReadSingle("Size");

            gameObject.QuestItems = packet.ReadByte("QuestItemsCount");
            for (uint i = 0; i < gameObject.QuestItems; i++)
            {
                GameObjectTemplateQuestItem questItem = new GameObjectTemplateQuestItem
                {
                    GameObjectEntry = (uint)entry.Key,
                    Idx = i,
                    ItemId = (uint)packet.ReadInt32<ItemId>("QuestItem", i)
                };

                Storage.GameObjectTemplateQuestItems.Add(questItem, packet.TimeSpan);
            }

            gameObject.ContentTuningId = packet.ReadInt32("ContentTuningId");

            packet.AddSniffData(StoreNameType.GameObject, entry.Key, "QUERY_RESPONSE");

            Storage.GameObjectTemplates.Add(gameObject, packet.TimeSpan);

            ObjectName objectName = new ObjectName
            {
                ObjectType = StoreNameType.GameObject,
                ID = entry.Key,
                Name = gameObject.Name
            };

            Storage.ObjectNames.Add(objectName, packet.TimeSpan);
        }
    }
}