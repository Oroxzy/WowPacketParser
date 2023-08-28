using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V2_5_1_38707.Parsers
{
    public static class ArenaHandler
    {
        [Parser(Opcode.CMSG_ARENA_TEAM_ROSTER)]
        public static void HandleArenaTeamRoster(Packet packet)
        {
            packet.ReadUInt32("TeamIndex");
        }

        [Parser(Opcode.CMSG_ARENA_TEAM_QUERY)]
        public static void HandleArenaTeamQuery(Packet packet)
        {
            packet.ReadUInt32("TeamId");
        }

        public static void ReadArenaTeamMemberInfo(Packet packet, params object[] idx)
        {
            packet.ReadPackedGuid128("MemberGUID", idx);
            packet.ReadByte("Online", idx); // ???????
            packet.ReadInt32("Captain", idx);
            packet.ReadByte("Level", idx);
            packet.ReadByteE<Class>("Class", idx);
            packet.ReadUInt32("WeekGamesPlayed", idx);
            packet.ReadUInt32("WeekGamesWon", idx);
            packet.ReadUInt32("SeasonGamesPlayed", idx);
            packet.ReadUInt32("SeasonGamesWon", idx);
            packet.ReadUInt32("PersonalRating", idx);
            packet.ResetBitReader();
            var strLen = packet.ReadBits(6);
            var byte64 = packet.ReadBit("byte64", idx);
            var byte6C = packet.ReadBit("byte6C", idx);
            packet.ReadWoWString("Name", strLen, idx);
            if (byte64)
                packet.ReadSingle("dword60", idx);
            if (byte6C)
                packet.ReadSingle("dword68", idx);
        }

        [Parser(Opcode.SMSG_ARENA_TEAM_ROSTER)]
        public static void HandleArenaNoIdeaYet(Packet packet)
        {
            packet.ReadInt32("TeamId");
            packet.ReadUInt32("TeamSize");
            packet.ReadUInt32("TeamPlayed");
            packet.ReadUInt32("TeamWins");
            packet.ReadUInt32("SeasonPlayed");
            packet.ReadUInt32("SeasonWins");
            packet.ReadUInt32("TeamRating");
            packet.ReadUInt32("PlayerRating");
            var count = packet.ReadUInt32();
            if (ClientVersion.AddedInClassicVersion(1, 14, 2, 2, 5, 3))
                packet.ReadBit("UnkBit");
            for (var i = 0; i < count; i++)
                ReadArenaTeamMemberInfo(packet, i);
        }

        [Parser(Opcode.SMSG_QUERY_ARENA_TEAM_RESPONSE)]
        public static void HandleQueryArenaTeamResponse(Packet packet)
        {
            packet.ReadUInt32("TeamID");
            packet.ResetBitReader();
            bool hasData = packet.ReadBit("HasData");

            if (hasData)
            {
                packet.ReadUInt32("TeamID");
                packet.ReadUInt32("TeamSize");
                packet.ReadInt32("BackgroundColor");
                packet.ReadInt32("EmblemStyle");
                packet.ReadInt32("EmblemColor");
                packet.ReadInt32("BorderStyle");
                packet.ReadInt32("BorderColor");

                packet.ResetBitReader();
                uint nameLength = packet.ReadBits("TeamNameLength", 7);
                packet.ReadWoWString("TeamName", nameLength);
            }
        }

        [Parser(Opcode.CMSG_BATTLEMASTER_JOIN_ARENA)]
        public static void HandleBattlemasterJoinArena(Packet packet)
        {
            packet.ReadPackedGuid128("BattlemasterGuid");
            packet.ReadByte("TeamIndex");
            packet.ReadByteE<LfgRoleFlag>("Roles");
        }

        [Parser(Opcode.CMSG_BATTLEMASTER_JOIN_SKIRMISH)]
        public static void HandleBattlemasterJoinSkirmish(Packet packet)
        {
            packet.ReadPackedGuid128("BattlemasterGuid");
            packet.ReadByteE<LfgRoleFlag>("Roles");
            packet.ReadByte("TeamSize");
            packet.ReadBit("JoinAsGroup");
            packet.ReadBit("IsRequeue");
        }

        [Parser(Opcode.CMSG_ARENA_TEAM_REMOVE)]
        [Parser(Opcode.CMSG_ARENA_TEAM_LEADER)]
        public static void HandleArenaTeamRemove(Packet packet)
        {
            packet.ReadUInt32("TeamId");
            packet.ReadPackedGuid128("PlayerGUID");
        }

        [Parser(Opcode.SMSG_ARENA_TEAM_EVENT)]
        public static void HandleArenaTeamEvent(Packet packet)
        {
            packet.ReadByteE<ArenaEvent>("Event");
            uint len1 = packet.ReadBits(9);
            uint len2 = packet.ReadBits(9);
            uint len3 = packet.ReadBits(9);
            packet.ReadWoWString("Param1", len1);
            packet.ReadWoWString("Param2", len2);
            packet.ReadWoWString("Param3", len3);
        }

        [Parser(Opcode.SMSG_ARENA_TEAM_COMMAND_RESULT)]
        public static void HandleArenaTeamCommandResult(Packet packet)
        {
            packet.ReadByteE<ArenaTeamCommandTypes>("Action");
            packet.ReadByteE<ArenaTeamCommandErrors254>("Error");
            uint len1 = packet.ReadBits(7);
            uint len2 = packet.ReadBits(6);
            packet.ReadWoWString("TeamName", len1);
            packet.ReadWoWString("PlayerName", len2);
        }

        [Parser(Opcode.SMSG_ARENA_TEAM_INVITE)]
        public static void HandleArenaTeamInvite(Packet packet)
        {
            packet.ReadPackedGuid128("PlayerGuid");
            packet.ReadUInt32("PlayerVirtualAddress");
            packet.ReadPackedGuid128("TeamGuid");
            uint len1 = packet.ReadBits(6);
            uint len2 = packet.ReadBits(7);
            packet.ReadWoWString("PlayerName", len1);
            packet.ReadWoWString("TeamName", len2);
        }

        [Parser(Opcode.CMSG_ARENA_TEAM_ACCEPT)]
        [Parser(Opcode.CMSG_ARENA_TEAM_DECLINE)]
        public static void HandleArenaTeamAccept(Packet packet)
        {
            packet.ReadPackedGuid128("PlayerGuid");
            packet.ReadPackedGuid128("TeamGuid");
        }
    }
}
