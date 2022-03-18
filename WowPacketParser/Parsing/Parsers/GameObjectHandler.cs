using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;

namespace WowPacketParser.Parsing.Parsers
{
    public static class GameObjectHandler
    {
        [Parser(Opcode.CMSG_QUERY_GAME_OBJECT)]
        public static void HandleGameObjectQuery(Packet packet)
        {
            var entry = packet.ReadInt32<GOId>("Entry");
            var guid = packet.ReadGuid("GUID");

            if (guid.HasEntry() && (entry != guid.GetEntry()))
                packet.AddValue("Error", "Entry does not match calculated GUID entry");
        }

        [HasSniffData]
        [Parser(Opcode.SMSG_QUERY_GAME_OBJECT_RESPONSE)]
        public static void HandleGameObjectQueryResponse(Packet packet)
        {
            var entry = packet.ReadEntry("Entry");

            if (entry.Value) // entry is masked
                return;

            GameObjectTemplate gameObject = new GameObjectTemplate
            {
                Entry = (uint)entry.Key,
                Type = packet.ReadInt32E<GameObjectType>("Type"),
                DisplayID = packet.ReadUInt32("Display ID")
            };

            var name = new string[4];
            for (int i = 0; i < 4; i++)
                name[i] = packet.ReadCString("Name", i);
            gameObject.Name = name[0];

            gameObject.IconName = packet.ReadCString("Icon Name");
            if (ClientVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
            {
                gameObject.CastCaption = packet.ReadCString("Cast Caption");
                gameObject.UnkString = packet.ReadCString("Unk String");
            }

            gameObject.Data = new int?[ClientVersion.AddedInVersion(ClientVersionBuild.V4_0_6_13596) ? 32 : 24];
            for (int i = 0; i < gameObject.Data.Length; i++)
                gameObject.Data[i] = packet.ReadInt32("Data", i);

            if (ClientVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056)) // not sure when it was added exactly - did not exist in 2.4.1 sniff
                gameObject.Size = packet.ReadSingle("Size");

            if (ClientVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
            {
                uint count = (uint)(ClientVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192) ? 6 : 4);
                for (uint i = 0; i < count; i++)
                {
                    uint itemId = packet.ReadUInt32<ItemId>("QuestItem", i);
                    if (itemId != 0)
                    {
                        gameObject.QuestItems++;
                        GameObjectTemplateQuestItem questItem = new GameObjectTemplateQuestItem
                        {
                            GameObjectEntry = (uint)entry.Key,
                            Idx = i,
                            ItemId = itemId
                        };

                        Storage.GameObjectTemplateQuestItems.Add(questItem, packet.TimeSpan);
                    }
                }
            }

            if (ClientVersion.AddedInVersion(ClientVersionBuild.V4_0_6_13596))
                gameObject.RequiredLevel = packet.ReadInt32("RequiredLevel");

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

        [Parser(Opcode.SMSG_DESTRUCTIBLE_BUILDING_DAMAGE)]
        public static void HandleDestructibleBuildingDamage(Packet packet)
        {
            packet.ReadPackedGuid("GO GUID");
            packet.ReadPackedGuid("Vehicle GUID");
            packet.ReadPackedGuid("Player GUID");
            packet.ReadInt32("Damage");
            packet.ReadInt32<SpellId>("Spell ID");
        }

        [Parser(Opcode.CMSG_GAME_OBJ_USE)]
        [Parser(Opcode.CMSG_GAME_OBJ_REPORT_USE)]
        public static void HandleGOUse(Packet packet)
        {
            WowGuid guid = packet.ReadGuid("GUID");
            Storage.StoreGameObjectUse(guid, packet.Time);
            packet.AddSniffData(StoreNameType.GameObject, (int)guid.GetEntry(), "USE");
        }
        
        [Parser(Opcode.SMSG_PAGE_TEXT)]
        [Parser(Opcode.SMSG_GAME_OBJECT_RESET_STATE)]
        public static void HandleGOMisc(Packet packet)
        {
            packet.ReadGuid("GUID");
        }

        [Parser(Opcode.SMSG_GAME_OBJECT_DESPAWN)]
        public static void HandleGODespawnAnim(Packet packet)
        {
            WowGuid guid = packet.ReadGuid("GUID");
            Storage.StoreGameObjectDespawnAnim(guid, packet.Time);
        }

        [Parser(Opcode.SMSG_GAME_OBJECT_CUSTOM_ANIM)]
        public static void HandleGOCustomAnim(Packet packet)
        {
            WowGuid guid = packet.ReadGuid("GUID");
            GameObjectCustomAnim animData = new GameObjectCustomAnim();
            animData.AnimId = packet.ReadInt32("Anim");
            animData.UnixTimeMs = (ulong)Utilities.GetUnixTimeMsFromDateTime(packet.Time);
            Storage.StoreGameObjectCustomAnim(guid, animData, packet.SniffId);
            packet.AddSniffData(StoreNameType.GameObject, (int)guid.GetEntry(), "CUSTOM_ANIM");
        }

        [Parser(Opcode.SMSG_GAME_OBJECT_ACTIVATE_ANIM_KIT)] // 4.3.4
        public static void HandleGameObjectActivateAnimKit(Packet packet)
        {
            var guid = packet.StartBitStream(5, 1, 0, 4, 7, 2, 3, 6);
            packet.ParseBitStream(guid, 5, 1, 0, 3, 4, 6, 2, 7);
            packet.WriteGuid("Guid", guid);
            packet.ReadInt32("Anim");
        }
    }
}
