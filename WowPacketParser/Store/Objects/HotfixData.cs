﻿using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.SQL;

namespace WowPacketParser.Store.Objects
{
    [DBTableName("hotfix_data")]
    public sealed class HotfixData : IDataModel
    {
        [DBFieldName("Id", true)]
        public uint? ID;

        [DBFieldName("UniqueID", TargetedDatabase.Shadowlands)]
        public uint? UniqueID;

        [DBFieldName("TableHash", true)]
        public DB2Hash? TableHash;

        [DBFieldName("RecordId", true)]
        public int? RecordID;

        [DBFieldName("Deleted", TargetedDbExpansion.Zero, TargetedDbExpansion.BattleForAzeroth)]
        public bool? Deleted;

        [DBFieldName("Status", TargetedDbExpansion.Shadowlands)]
        public HotfixStatus? Status;

        [DBFieldName("VerifiedBuild")]
        public int? VerifiedBuild = ClientVersion.BuildInt;
    }
}
