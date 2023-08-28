using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V9_0_1_36216.Parsers
{
    public static class BattlegroundHandler
    {
        [Parser(Opcode.SMSG_SEASON_INFO)]
        public static void HandleSeasonInfo(Packet packet)
        {
            if (ClientVersion.AddedInVersion(9, 2, 0, 1, 14, 3, 2, 5, 4))
                packet.ReadInt32("MythicPlusDisplaySeasonID");

            packet.ReadInt32("MythicPlusMilestoneSeasonID");
            packet.ReadInt32("CurrentArenaSeason");
            packet.ReadInt32("PreviousArenaSeason");
            packet.ReadInt32("ConquestWeeklyProgressCurrencyID");
            packet.ReadInt32("PvpSeasonID");
            packet.ReadBit("WeeklyRewardChestsEnabled");
        }
    }
}
