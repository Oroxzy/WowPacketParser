﻿using WowPacketParser.Enums;
using WowPacketParser.SQL;
using WowPacketParser.Store.Objects.UpdateFields;
using WowPacketParser.Store.Objects.UpdateFields.LegacyImplementation;

namespace WowPacketParser.Store.Objects
{
    public sealed class SceneObject : WoWObject, IDataModel
    {
        public ISceneObjectData SceneObjectData;

        public SceneObject() : base()
        {
            SceneObjectData = new SceneObjectData(this);
        }

        public bool CanBeSaved()
        {
            return ObjectData.EntryID != 0
                && SceneObjectData.ScriptPackageID != 0
                && (SceneType)SceneObjectData.SceneType != SceneType.PetBattle;
        }

        public SceneTemplate CreateSceneTemplate()
        {
            var template = new SceneTemplate();
            template.SceneID = (uint)ObjectData.EntryID;
            template.Flags = 0;
            template.ScriptPackageID = (uint)SceneObjectData.ScriptPackageID;
            template.Encrypted = false;
            return template;
        }
    }
}