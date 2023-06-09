using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V2_5_1_38707.Parsers
{
    public static class TicketHandler
    {
        public static void ReadCliSupportTicketHeader(Packet packet, params object[] idx)
        {
            packet.ReadInt32<MapId>("MapID", idx);
            packet.ReadVector3("Position", idx);
            packet.ReadSingle("Facing", idx);
        }

        public static void ReadCliSupportTicketChatLine(Packet packet, params object[] idx)
        {
                packet.ReadTime64("Timestamp", idx);

                var textLength = packet.ReadBits("TextLength", 12, idx);
                packet.ResetBitReader();
                packet.ReadWoWString("Text", textLength, idx);
        }

        public static void ReadCliSupportTicketChatLog(Packet packet, params object[] idx)
        {
            var linesCount = packet.ReadUInt32("LinesCount", idx);
            var hasReportLineIndex = packet.ReadBit("HasReportLineIndex", idx);
            packet.ResetBitReader();

            for (int i = 0; i < linesCount; ++i)
                ReadCliSupportTicketChatLine(packet, idx, "Lines", i);

            if (hasReportLineIndex)
                packet.ReadUInt32("ReportLineIndex", idx);
        }

        public static void ReadCliSupportTicketMailInfo(Packet packet, params object[] idx)
        {
            packet.ReadInt32("MailID", idx);

            var bodyLength = packet.ReadBits("MailBodyLength", 13, idx);
            var subjectLength = packet.ReadBits("MailSubjectLength", 9, idx);

            packet.ResetBitReader();

            packet.ReadWoWString("MailBody", bodyLength, idx);
            packet.ReadWoWString("MailSubject", subjectLength, idx);
        }

        public static void ReadCliSupportTicketCalendarEventInfo(Packet packet, params object[] idx)
        {
            packet.ReadUInt64("EventID", idx); // order not confirmed
            packet.ReadUInt64("InviteID", idx); // order not confirmed

            var eventTitleLength = packet.ReadBits("EventTitleLength", 8, idx);

            packet.ResetBitReader();

            packet.ReadWoWString("EventTitle", eventTitleLength, idx);
        }

        public static void ReadCliSupportTicketPetInfo(Packet packet, params object[] idx)
        {
            packet.ReadPackedGuid128("PetID", idx);

            var petNameLength = packet.ReadBits("PetNameLength", 8, idx);

            packet.ResetBitReader();

            packet.ReadWoWString("PetName", petNameLength, idx);
        }

        public static void ReadCliSupportTicketGuildInfo(Packet packet, params object[] idx)
        {
            var guildNameLength = packet.ReadBits("GuildNameLength", 8, idx); // 7 or 8 ?
            packet.ResetBitReader();

            packet.ReadPackedGuid128("GuildID", idx);

            packet.ReadWoWString("GuildName", guildNameLength, idx);
        }

        public static void ReadCliSupportTicketLFGListSearchResult(Packet packet, params object[] idx)
        {
            WowPacketParserModule.V6_0_2_19033.Parsers.LfgHandler.ReadCliRideTicket(packet, "RideTicket", idx);
            packet.ReadUInt32("GroupFinderActivityID", idx);
            packet.ReadPackedGuid128("LastTitleAuthorGuid", idx);
            packet.ReadPackedGuid128("LastDescriptionAuthorGuid", idx);
            packet.ReadPackedGuid128("LastVoiceChatAuthorGuid", idx);
            packet.ReadPackedGuid128("UnkGUID", idx);
            packet.ReadPackedGuid128("UnkGUID", idx);

            var length88 = packet.ReadBits(8);
            var length217 = packet.ReadBits(11);
            var length1242 = packet.ReadBits(8);

            packet.ResetBitReader();

            packet.ReadWoWString("Title", length88, idx);
            packet.ReadWoWString("Description", length217, idx);
            packet.ReadWoWString("VoiceChat", length1242, idx);
        }

        public static void ReadCliSupportTicketLFGListApplicant(Packet packet, params object[] idx)
        {
            WowPacketParserModule.V6_0_2_19033.Parsers.LfgHandler.ReadCliRideTicket(packet, "RideTicket", idx);

            var length = packet.ReadBits(9);
            packet.ResetBitReader();
            packet.ReadWoWString("Comment", length, idx);
        }

        public static void ReadHorusChatLine(Packet packet, params object[] indexes)
        {
            packet.ReadUInt32("Timestamp", indexes);
            packet.ReadPackedGuid128("AuthorGUID", indexes);

            var hasClubID = packet.ReadBit();
            var hasChannelGUID = packet.ReadBit();
            var hasRealmAddress = packet.ReadBit();
            var hasSlashCmd = packet.ReadBit();
            var textLen = packet.ReadBits(12);

            if (hasClubID)
                packet.ReadUInt64("ClubID", indexes);
            if (hasChannelGUID)
                packet.ReadPackedGuid128("ChannelGUID", indexes);
            if (hasRealmAddress)
            {
                packet.ReadUInt32("VirtualRealmAddress", indexes);
                packet.ReadUInt16("field_4", indexes);
                packet.ReadByte("field_6", indexes);
            }
            if (hasSlashCmd)
                packet.ReadInt32("SlashCmd", indexes);

            packet.ReadWoWString("Text", textLen, indexes);
        }

        public static void ReadHorusChatLog(Packet packet, params object[] indexes)
        {
            var lines = packet.ReadUInt32();
            for (int i = 0; i < lines; i++)
                ReadHorusChatLine(packet, i, indexes);
        }

        public static void ReadClubFinderResult(Packet packet, params object[] indexes)
        {
            packet.ReadUInt64("ClubFinderPostingID", indexes);
            packet.ReadUInt64("ClubID ", indexes);
            packet.ReadPackedGuid128("ClubFinderGUID", indexes);
            var nameLen = packet.ReadBits(12);
            packet.ReadWoWString("ClubName", nameLen, indexes);
        }

        public static void ReadUnused910(Packet packet, params object[] indexes)
        {
            var len = packet.ReadBits(7);
            packet.ReadPackedGuid128("field_104", indexes);
            packet.ReadWoWString("field_0", len, indexes);
        }

        [Parser(Opcode.CMSG_SUPPORT_TICKET_SUBMIT_COMPLAINT)]
        public static void HandleSubmitComplaints(Packet packet)
        {
            ReadCliSupportTicketHeader(packet, "Header");
            packet.ReadPackedGuid128("TargetCharacterGUID");
            ReadCliSupportTicketChatLog(packet, "ChatLog");

            packet.ReadBitsE<CliComplaintType>("ComplaintType", 5);

            var noteLength = packet.ReadBits("NoteLength", 10);

            var hasMailInfo = packet.ReadBit("HasMailInfo");
            var hasCalendarInfo = packet.ReadBit("HasCalendarInfo");
            var hasPetInfo = packet.ReadBit("HasPetInfo");
            var hasGuildInfo = packet.ReadBit("HasGuildInfo");
            var hasLFGListSearchResult = packet.ReadBit("HasLFGListSearchResult");
            var hasLFGListApplicant = packet.ReadBit("HasLFGListApplicant");
            var hasClubMessage = packet.ReadBit("HasClubMessage");
            var hasClubFinderResult = packet.ReadBit("HasClubFinderResult");
            var hasUnkBit = packet.ReadBit("UnkBit");

            packet.ResetBitReader();

            if (hasClubMessage)
            {
                packet.ReadBit("IsUsingPlayerVoice");
                packet.ResetBitReader();
            }

            ReadHorusChatLog(packet, "Horus");

            packet.ReadWoWString("Note", noteLength);

            if (hasMailInfo)
                ReadCliSupportTicketMailInfo(packet, "MailInfo");

            if (hasCalendarInfo)
                ReadCliSupportTicketCalendarEventInfo(packet, "CalendarInfo");

            if (hasPetInfo)
                ReadCliSupportTicketPetInfo(packet, "PetInfo");

            if (hasGuildInfo)
                ReadCliSupportTicketGuildInfo(packet, "GuidInfo");

            if (hasLFGListSearchResult)
                ReadCliSupportTicketLFGListSearchResult(packet, "LFGListSearchResult");

            if (hasLFGListApplicant)
                ReadCliSupportTicketLFGListApplicant(packet, "LFGListApplicant");

            if (hasClubFinderResult)
                ReadClubFinderResult(packet, "ClubFinderResult");

            if (hasUnkBit)
                ReadUnused910(packet, "Unused910");
        }
    }
}
