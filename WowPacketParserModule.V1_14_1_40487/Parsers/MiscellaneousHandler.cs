using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V1_14_1_40487.Parsers
{
    public static class MiscellaneousHandler
    {
        [Parser(Opcode.SMSG_UNK_SPELL_DEBUG1)]
        public static void HandleUnkDebug1(Packet packet)
        {
            packet.ReadPackedGuid128("CasterGuid");
            packet.ReadPackedGuid128("TargetGuid");
            packet.ReadInt32("SpellID");
            packet.ReadSingle("UnkFloat4");
            packet.ReadSingle("UnkFloat5");
            packet.ReadInt32("UnkInt6");
            packet.ReadByte("UnkByte7");
        }
    }
}
