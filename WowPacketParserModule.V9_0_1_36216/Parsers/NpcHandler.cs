using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;
using CoreParsers = WowPacketParser.Parsing.Parsers;


namespace WowPacketParserModule.V9_0_1_36216.Parsers
{
    public static class NpcHandler
    {
        [Parser(Opcode.SMSG_RUNEFORGE_LEGENDARY_CRAFTING_OPEN_NPC)]
        public static void HandleRuneforgeLegendaryCraftingOpenNpc(Packet packet)
        {
            packet.ReadPackedGuid128("GUID");
            packet.ReadBit("IsUpgrade"); // Correct?

            CoreParsers.NpcHandler.LastGossipOption.Reset();
            CoreParsers.NpcHandler.TempGossipOptionPOI.Reset();
        }

        [Parser(Opcode.SMSG_GOSSIP_POI)]
        public static void HandleGossipPoi(Packet packet)
        {
            PointsOfInterest gossipPOI = new PointsOfInterest();

            gossipPOI.ID = packet.ReadInt32("ID");

            if (ClientVersion.AddedInVersion(ClientVersionBuild.V9_1_0_39185))
            {
                Vector3 pos = packet.ReadVector3("Coordinates");
                gossipPOI.PositionX = pos.X;
                gossipPOI.PositionY = pos.Y;
                gossipPOI.PositionZ = pos.Z;
            }
            else
            {
                Vector2 pos = packet.ReadVector2("Coordinates");
                gossipPOI.PositionX = pos.X;
                gossipPOI.PositionY = pos.Y;
            }

            gossipPOI.Icon = packet.ReadInt32E<GossipPOIIcon>("Icon");
            gossipPOI.Importance = (uint)packet.ReadInt32("Importance");

            if (ClientVersion.AddedInVersion(ClientVersionBuild.V9_0_5_37503))
                gossipPOI.Unknown905 = packet.ReadInt32("Unknown905");

            packet.ResetBitReader();
            gossipPOI.Flags = packet.ReadBits("Flags", 14);
            uint bit84 = packet.ReadBits(6);
            gossipPOI.Name = packet.ReadWoWString("Name", bit84);

            var lastGossipOption = CoreParsers.NpcHandler.LastGossipOption;
            var tempGossipOptionPOI = CoreParsers.NpcHandler.TempGossipOptionPOI;

            lastGossipOption.ActionPoiId = gossipPOI.ID;
            tempGossipOptionPOI.ActionPoiId = gossipPOI.ID;

            if (ClientLocale.PacketLocale != LocaleConstant.enUS)
            {
                PointsOfInterestLocale localesPoi = new PointsOfInterestLocale
                {
                    ID = gossipPOI.ID.ToString(),
                    Name = gossipPOI.Name,
                };

                Storage.LocalesPointsOfInterest.Add(localesPoi, packet.TimeSpan);
            }
            else
            {
                Storage.GossipPOIs.Add(gossipPOI, packet.TimeSpan);
            }

            if (tempGossipOptionPOI.HasSelection)
            {
                if ((packet.TimeSpan - tempGossipOptionPOI.TimeSpan).Duration() <= TimeSpan.FromMilliseconds(2500))
                {
                    if (tempGossipOptionPOI.ActionMenuId != null)
                    {
                        Storage.GossipMenuOptionActions.Add(new GossipMenuOptionAction { MenuId = tempGossipOptionPOI.MenuId, OptionIndex = tempGossipOptionPOI.OptionIndex, ActionMenuId = tempGossipOptionPOI.ActionMenuId, ActionPoiId = gossipPOI.ID }, packet.TimeSpan);
                        //clear temp
                        tempGossipOptionPOI.Reset();
                    }
                }
                else
                {
                    lastGossipOption.Reset();
                    tempGossipOptionPOI.Reset();
                }
            }
        }
		
        [Parser(Opcode.SMSG_CHROMIE_TIME_OPEN_NPC)]
        public static void HandleChromieTimeOpenNpc(Packet packet)
        {
            packet.ReadPackedGuid128("GUID");
        }

        [Parser(Opcode.CMSG_CHROMIE_TIME_SELECT_EXPANSION)]
        public static void HandleChromieTimeSelectExpansion(Packet packet)
        {
            packet.ReadPackedGuid128("GUID");
            packet.ReadUInt32("Expansion");
        }

        [Parser(Opcode.SMSG_COVENANT_PREVIEW_OPEN_NPC)]
        public static void HandleGarrisonCovenantPreviewOpenNpc(Packet packet)
        {
            packet.ReadPackedGuid128("NpcGUID");
            packet.ReadInt32("CovenantID");
        }
		
        [Parser(Opcode.SMSG_ADVENTURE_MAP_OPEN_NPC)]
        public static void HandleAdventureMapOpenNpc(Packet packet)
        {
            packet.ReadPackedGuid128("NpcGUID");
            packet.ReadInt32("UiMapID");
        }
    }
}
