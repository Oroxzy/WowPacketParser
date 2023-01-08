using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;

namespace WowPacketParserModule.V5_4_0_17359.Parsers
{
    public static class GameObjectHandler
    {
        [HasSniffData]
        [Parser(Opcode.SMSG_QUERY_GAME_OBJECT_RESPONSE)]
        public static void HandleGameObjectQueryResponse(Packet packet)
        {
            packet.ReadByte("Unk1 Byte");
            var entry = packet.ReadEntry("Entry");
            if (entry.Value) // entry is masked
                return;

            GameObjectTemplate gameObject = new GameObjectTemplate
            {
                Entry = (uint)entry.Key
            };

            int unk1 = packet.ReadInt32("Unk1 UInt32");
            if (unk1 == 0)
                return;

            gameObject.Type = packet.ReadInt32E<GameObjectType>("Type");
            gameObject.DisplayID = packet.ReadUInt32("Display ID");

            var name = new string[4];
            for (int i = 0; i < 4; i++)
                name[i] = packet.ReadCString("Name", i);
            gameObject.Name = name[0];

            gameObject.IconName = packet.ReadCString("Icon Name");
            gameObject.CastCaption = packet.ReadCString("Cast Caption");
            gameObject.UnkString = packet.ReadCString("Unk String");

            gameObject.Data = new int?[32];
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
                    ItemId = packet.ReadUInt32<ItemId>("QuestItem", i)
                };

                Storage.GameObjectTemplateQuestItems.Add(questItem, packet.TimeSpan);
            }

            packet.ReadUInt32E<ClientType>("Expansion");

            packet.AddSniffData(StoreNameType.GameObject, entry.Key, "QUERY_RESPONSE");

            if (ClientLocale.PacketLocale != LocaleConstant.enUS)
            {
                GameObjectTemplateLocale localesGameObject = new GameObjectTemplateLocale
                {
                    ID = (uint)entry.Key,
                    Name = gameObject.Name,
                    CastBarCaption = gameObject.CastCaption,
                    Unk1 = gameObject.UnkString,
                };

                Storage.LocalesGameObjects.Add(localesGameObject, packet.TimeSpan);
            }
            else
            {
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

        [Parser(Opcode.CMSG_QUERY_GAME_OBJECT)]
        public static void HandleGameObjectQuery(Packet packet)
        {
            var guid = new byte[8];

            packet.ReadInt32("Entry");

            guid = packet.StartBitStream(2, 4, 3, 7, 0, 6, 1, 5);
            packet.ParseBitStream(guid, 1, 7, 2, 3, 6, 5, 4, 0);

            packet.WriteGuid("GUID", guid);
        }
    }
}
