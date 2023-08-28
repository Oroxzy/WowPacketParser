﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.SQL.Builders;
using WowPacketParser.Store;
using WowPacketParser.Store.Objects;

namespace WowPacketParser.SQL
{
    public static class Builder
    {
        private static StoreNameType FromObjectType(ObjectType type)
        {
            switch (type)
            {
                case ObjectType.Item:
                    return StoreNameType.Item;
                case ObjectType.Unit:
                    return StoreNameType.Unit;
                case ObjectType.Player:
                    return StoreNameType.Player;
                case ObjectType.GameObject:
                    return StoreNameType.GameObject;
                case ObjectType.Map:
                    return StoreNameType.Map;
                case ObjectType.Object:
                case ObjectType.Container:
                case ObjectType.DynamicObject:
                case ObjectType.Corpse:
                case ObjectType.AreaTrigger:
                case ObjectType.SceneObject:
                case ObjectType.Conversation:
                    return StoreNameType.None;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Update SQLDatabase.NameStores with names from Storage.ObjectNames
        /// </summary>
        private static void LoadNames()
        {
            foreach (var objectName in Storage.ObjectNames)
            {
                if (objectName.Item1.ObjectType != null && objectName.Item1.ID != null)
                {
                    var type = objectName.Item1.ObjectType.Value;
                    Dictionary<int, string> names;
                    if (!SQLDatabase.NameStores.TryGetValue(type, out names))
                    {
                        names = new Dictionary<int, string>();
                        SQLDatabase.NameStores.Add(type, names);
                    }

                    if (!names.ContainsKey(objectName.Item1.ID.Value))
                        names.Add(objectName.Item1.ID.Value, objectName.Item1.Name);
                }
            }
        }

        public static void DumpSQL(string prefix, string fileName, string header)
        {
            var startTime = DateTime.Now;

            LoadNames();

            var units = Storage.Objects.IsEmpty()
                ? new Dictionary<WowGuid, Unit>()                                                               // empty dict if there are no objects
                : Storage.Objects.Where(
                    obj =>
                        obj.Value.Item1.Type == ObjectType.Unit)
                    .OrderBy(pair => pair.Value.Item2)                                                          // order by spawn time
                    .ToDictionary(obj => obj.Key, obj => obj.Value.Item1 as Unit);

            var gameObjects = Storage.Objects.IsEmpty()
                ? new Dictionary<WowGuid, GameObject>()                                                         // empty dict if there are no objects
                : Storage.Objects.Where(obj => obj.Value.Item1.Type == ObjectType.GameObject)
                    .OrderBy(pair => pair.Value.Item2)                                                          // order by spawn time
                    .ToDictionary(obj => obj.Key, obj => obj.Value.Item1 as GameObject);

            foreach (var obj in Storage.Objects)
                obj.Value.Item1.LoadValuesFromUpdateFields();

            using (var store = new SQLFile(fileName, header))
            {
                var builderMethods = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => type.GetCustomAttributes(typeof (BuilderClassAttribute), true).Length > 0)
                    .SelectMany(x => x.GetMethods())
                    .Where(y => y.GetCustomAttributes().OfType<BuilderMethodAttribute>().Any())
                    .ToList();

                for (int i = 1; i <= builderMethods.Count; i++)
                {
                    var method = builderMethods[i - 1];
                    var attr = method.GetCustomAttribute<BuilderMethodAttribute>();

                    if (attr.CheckVersionMismatch)
                    {
                        if (!GetExpectedTargetDatabasesForExpansion(ClientVersion.Expansion).Contains(Settings.TargetedDbExpansion))
                        {
                            Trace.WriteLine(
                                $"{i}/{builderMethods.Count} - Error: Couldn't generate SQL output of {method.Name} since the targeted database and the sniff version don't match.");
                            continue;
                        }
                    }

                    var parameters = new List<object>();
                    if (attr.Units)
                        parameters.Add(units);

                    if (attr.Gameobjects)
                        parameters.Add(gameObjects);

                    Trace.WriteLine($"{i}/{builderMethods.Count} - Write {method.Name}");
                    try
                    {
                        store.WriteData(method.Invoke(null, parameters.ToArray()).ToString());
                    }
                    catch (TargetInvocationException e)
                    {
                        Trace.WriteLine($"{i}/{builderMethods.Count} - Error: Failed writing {method.Name}");
                        Trace.TraceError(e.InnerException?.ToString() ?? e.ToString());
                    }
                }

                Trace.WriteLine(store.AnythingWritten()
                    ? $"{prefix}: Saved file to '{fileName}'"
                    : "No SQL files created -- empty.");
                var endTime = DateTime.Now;
                var span = endTime.Subtract(startTime);
                Trace.WriteLine($"Finished SQL file in {span.ToFormattedString()}.");
            }
        }

        private static List<TargetedDbExpansion> GetExpectedTargetDatabasesForExpansion(ClientType expansion)
        {
            switch (expansion)
            {
                case ClientType.WorldOfWarcraft:
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.Zero };
                case ClientType.TheBurningCrusade:
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.TheBurningCrusade };
                case ClientType.WrathOfTheLichKing:
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.WrathOfTheLichKing };
                case ClientType.Cataclysm:
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.Cataclysm };
                case ClientType.WarlordsOfDraenor:
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.WarlordsOfDraenor };
                case ClientType.Legion:
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.Legion };
                case ClientType.BattleForAzeroth: // == ClientType.Classic
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.BattleForAzeroth, TargetedDbExpansion.Classic };
                case ClientType.Shadowlands: // == ClientType.BurningCrusadeClassic
                    return new List<TargetedDbExpansion> { TargetedDbExpansion.Shadowlands, TargetedDbExpansion.BurningCrusadeClassic, TargetedDbExpansion.WrathOfTheLichKingClassic };
                default:
                    return new List<TargetedDbExpansion>();
            }
        }
    }
}
