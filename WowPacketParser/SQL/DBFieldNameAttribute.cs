﻿using System;
using System.Text;
using WowPacketParser.Enums;
using WowPacketParser.Loading;
using WowPacketParser.Misc;

namespace WowPacketParser.SQL
{
    /// <summary>
    /// Field/column name
    /// Can be used with basic types and enums or
    ///  arrays ("dmg_min1, dmg_min2, dmg_min3" -> name = dmg_min, count = 3, startAtZero = false)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class DBFieldNameAttribute : Attribute
    {
        /// <summary>
        /// Column name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// True if field is a primary key
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// True if field shouldn't be quoted.
        /// </summary>
        public bool NoQuotes { get; set; }

        /// <summary>
        /// True if field is nullable or the default value should be NULL.
        /// </summary>
        public bool Nullable { get; set;}

        /// <summary>
        /// True if the field is only used when parsing multiple sniff files into 1 sql.
        /// </summary>
        public bool MultipleSniffsOnly { get; set; }

        /// <summary>
        /// True if the field is only used when exporting transports to database.
        /// </summary>
        public bool OnlyWhenSavingTransports { get; set; }

        /// <summary>
        /// Number of fields
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// True if counting should include 0
        /// </summary>
        public readonly bool StartAtZero;

        public readonly LocaleConstant? Locale;

        /// <summary>
        /// True if cctor that accepts multiple fields was used (name + count)
        /// </summary>
        private readonly bool _multipleFields;

        private readonly TargetedDbExpansion? _addedInVersion;

        private readonly TargetedDbExpansion? _removedInVersion;

        public TargetedDbType DbType =  TargetedDbType.WPP | TargetedDbType.TRINITY | TargetedDbType.VMANGOS | TargetedDbType.CMANGOS;

        /// <summary>
        /// matches any version
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = 1;
        }

        /// <summary>
        /// [addedInVersion, +inf[
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="locale">initial locale</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, LocaleConstant locale, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = 1;
            Locale = locale;
        }

        /// <summary>
        /// [addedInVersion, +inf[
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="addedInVersion">initial version</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, TargetedDbExpansion addedInVersion, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = 1;
            _addedInVersion = addedInVersion;
        }

        /// <summary>
        /// [addedInVersion, +inf[
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="addedInVersion">initial version</param>
        /// <param name="locale">initial locale</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, TargetedDbExpansion addedInVersion, LocaleConstant locale, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = 1;
            Locale = locale;
            _addedInVersion = addedInVersion;
        }

        /// <summary>
        /// [addedInVersion, removedInVersion[
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="addedInVersion">initial version</param>
        /// <param name="removedInVersion">final version</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, TargetedDbExpansion addedInVersion, TargetedDbExpansion removedInVersion, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = 1;
            _addedInVersion = addedInVersion;
            _removedInVersion = removedInVersion;

        }

        /// <summary>
        /// [addedInVersion, removedInVersion[
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="addedInVersion">initial version</param>
        /// <param name="removedInVersion">final version</param>
        /// <param name="locale">initial locale</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, TargetedDbExpansion addedInVersion, TargetedDbExpansion removedInVersion, LocaleConstant locale, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = 1;
            Locale = locale;
            _addedInVersion = addedInVersion;
            _removedInVersion = removedInVersion;
        }

        /// <summary>
        /// matches any version
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="count">number of fields</param>
        /// <param name="startAtZero">true if fields name start at 0</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, int count, bool startAtZero = false, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = count;
            StartAtZero = startAtZero;
            _multipleFields = true;
        }

        /// <summary>
        /// [addedInVersion, +inf[
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="addedInVersion">initial version</param>
        /// <param name="count">number of fields</param>
        /// <param name="startAtZero">true if fields name start at 0</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, TargetedDbExpansion addedInVersion, int count, bool startAtZero = false,
            bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = count;
            _addedInVersion = addedInVersion;

            StartAtZero = startAtZero;
            _multipleFields = true;
        }

        /// <summary>
        /// [addedInVersion, removedInVersion[
        /// </summary>
        /// <param name="name">database field name</param>
        /// <param name="addedInVersion">initial version</param>
        /// <param name="removedInVersion">final version</param>
        /// <param name="count">number of fields</param>
        /// <param name="startAtZero">true if fields name start at 0</param>
        /// <param name="isPrimaryKey">true if field is a primary key</param>
        public DBFieldNameAttribute(string name, TargetedDbExpansion addedInVersion, TargetedDbExpansion removedInVersion,
            int count, bool startAtZero = false, bool isPrimaryKey = false, bool noQuotes = false, bool nullable = false, bool multipleSniffsOnly = false)
        {
            Name = name;
            IsPrimaryKey = isPrimaryKey;
            NoQuotes = noQuotes;
            Nullable = nullable;
            MultipleSniffsOnly = multipleSniffsOnly;
            Count = count;
            _addedInVersion = addedInVersion;
            _removedInVersion = removedInVersion;

            StartAtZero = startAtZero;
            _multipleFields = true;
        }

        public bool IsVisible()
        {
            if (Locale != null && Locale != ClientLocale.PacketLocale)
                return false;

            if ((Settings.TargetedDbType & DbType) == 0)
                return false;

            if (OnlyWhenSavingTransports && !Settings.SaveTransports)
                return false;

            TargetedDbExpansion target = Settings.TargetedDbExpansion;

            if (_addedInVersion.HasValue && !_removedInVersion.HasValue)
                return target >= _addedInVersion.Value;

            if (_addedInVersion.HasValue && _removedInVersion.HasValue)
                return target >= _addedInVersion.Value && target < _removedInVersion.Value;

            if (MultipleSniffsOnly && !(!string.IsNullOrWhiteSpace(Settings.SQLFileName) && Settings.DumpFormatWithSQL()))
                return false;

            return true;
        }

        /// <summary>
        /// String representation of the field or group of fields
        /// </summary>
        /// <returns>name</returns>
        public override string ToString()
        {
            if (Name == null)
                return string.Empty;

            if (!_multipleFields)
                return SQLUtil.AddBackQuotes(Name);

            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= Count; i++)
            {
                result.Append(SQLUtil.AddBackQuotes(Name + (StartAtZero ? i - 1 : i)));
                if (i != Count)
                    result.Append(SQLUtil.CommaSeparator);
            }
            return result.ToString();
        }
    }
}
