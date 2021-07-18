﻿using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.SQL;

namespace WowPacketParser.Store.Objects
{
    [DBTableName("dynamicobject")]
    public sealed class DynamicObjectSpawn : IDataModel
    {
        [DBFieldName("guid", true, true)]
        public string GUID;

        [DBFieldName("map", false, false, true)]
        public uint? Map;

        [DBFieldName("position_x")]
        public float? PositionX;

        [DBFieldName("position_y")]
        public float? PositionY;

        [DBFieldName("position_z")]
        public float? PositionZ;

        [DBFieldName("orientation")]
        public float? Orientation;

        [DBFieldName("transport_guid", false, true, OnlyWhenSavingTransports = true, DbType = (TargetedDbType.WPP))]
        public string TransportGuid = "0";

        [DBFieldName("transport_x", OnlyWhenSavingTransports = true, DbType = (TargetedDbType.WPP))]
        public float TransportPositionX;

        [DBFieldName("transport_y", OnlyWhenSavingTransports = true, DbType = (TargetedDbType.WPP))]
        public float TransportPositionY;

        [DBFieldName("transport_z", OnlyWhenSavingTransports = true, DbType = (TargetedDbType.WPP))]
        public float TransportPositionZ;

        [DBFieldName("transport_o", OnlyWhenSavingTransports = true, DbType = (TargetedDbType.WPP))]
        public float TransportOrientation;

        [DBFieldName("caster_guid", false, true)]
        public string CasterGuid;

        [DBFieldName("caster_id")]
        public uint CasterId;

        [DBFieldName("caster_type")]
        public string CasterType;

        [DBFieldName("spell_id")]
        public uint? SpellId;

        [DBFieldName("visual_id")]
        public uint? VisualId;

        [DBFieldName("radius")]
        public float? Radius;

        [DBFieldName("type")]
        public byte? Type;

        [DBFieldName("cast_time")]
        public uint? CastTime;
    }

    [DBTableName("dynamicobject_create1_time")]
    public sealed class DynamicObjectCreate1 : IDataModel
    {
        [DBFieldName("unixtimems", true)]
        public ulong UnixTimeMs;

        [DBFieldName("guid", true, true)]
        public string GUID;

        [DBFieldName("map")]
        public uint? Map;

        [DBFieldName("position_x")]
        public float? PositionX;

        [DBFieldName("position_y")]
        public float? PositionY;

        [DBFieldName("position_z")]
        public float? PositionZ;

        [DBFieldName("orientation")]
        public float? Orientation;
    }

    [DBTableName("dynamicobject_create2_time")]
    public sealed class DynamicObjectCreate2 : IDataModel
    {
        [DBFieldName("unixtimems", true)]
        public ulong UnixTimeMs;

        [DBFieldName("guid", true, true)]
        public string GUID;

        [DBFieldName("map")]
        public uint? Map;

        [DBFieldName("position_x")]
        public float? PositionX;

        [DBFieldName("position_y")]
        public float? PositionY;

        [DBFieldName("position_z")]
        public float? PositionZ;

        [DBFieldName("orientation")]
        public float? Orientation;
    }

    [DBTableName("dynamicobject_destroy_time")]
    public sealed class DynamicObjectDestroy : IDataModel
    {
        [DBFieldName("unixtimems", true)]
        public ulong UnixTimeMs;

        [DBFieldName("guid", true, true)]
        public string GUID;
    }
}
