using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Store;

namespace WowPacketParser.SQL.Builders
{
    [BuilderClass]
    public static class Locales
    {
        [BuilderMethod]
        public static string LocalesCreature()
        {
            if (Storage.LocalesCreatures.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.creature_template_locale)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesCreatures)
                {
                    result += "UPDATE `locales_creature` SET `name_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Name) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                    result += "UPDATE `locales_creature` SET `subname_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Title) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var templatesDb = SQLDatabase.Get(new RowList<Store.Objects.CreatureTemplateLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesCreatures, templatesDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesGameObject()
        {
            if (Storage.LocalesGameObjects.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.gameobject_template_locale)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesGameObjects)
                {
                    result += "UPDATE `locales_gameobject` SET `name_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Name) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var templatesDb = SQLDatabase.Get(new RowList<Store.Objects.GameObjectTemplateLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesGameObjects, templatesDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesQuest()
        {
            if (Storage.LocalesQuests.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.locales_quest)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesQuests)
                {
                    result += "UPDATE `locales_quest` SET `Title_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.LogTitle) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                    result += "UPDATE `locales_quest` SET `Details_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.QuestDescription) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                    result += "UPDATE `locales_quest` SET `Objectives_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.LogDescription) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                    result += "UPDATE `locales_quest` SET `EndText_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.AreaDescription) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var templatesDb = SQLDatabase.Get(new RowList<Store.Objects.LocalesQuest>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesQuests, templatesDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesQuestOfferReward()
        {
            if (Storage.LocalesQuestOfferRewards.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.locales_quest)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesQuestOfferRewards)
                {
                    result += "UPDATE `locales_quest` SET `OfferRewardText_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.RewardText) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var offersDb = SQLDatabase.Get(new RowList<Store.Objects.QuestOfferRewardLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesQuestOfferRewards, offersDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesQuestObjectives()
        {
            if (Storage.LocalesQuestObjectives.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.locales_quest_objectives)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesQuestObjectives)
                {
                    result += "UPDATE `locales_quest` SET `ObjectiveText" + (locale.Item1.StorageIndex + 1) + "_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Description) + "' WHERE `entry`=" + locale.Item1.QuestId + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var templatesDb = SQLDatabase.Get(new RowList<Store.Objects.QuestObjectivesLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesQuestObjectives, templatesDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesQuestGreeting()
        {
            if (Storage.LocalesQuestGreeting.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.locales_quest)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesQuestGreeting)
                {
                    result += "UPDATE `quest_greeting` SET `content_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Greeting) + "' WHERE `entry`=" + locale.Item1.ID + " && `type`=" + locale.Item1.Type + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var greetingDb = SQLDatabase.Get(new RowList<Store.Objects.QuestGreetingLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesQuestGreeting, greetingDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesQuestRequestItems()
        {
            if (Storage.LocalesQuestRequestItems.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.locales_quest)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesQuestRequestItems)
                {
                    result += "UPDATE `locales_quest` SET `RequestItemsText_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.CompletionText) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var requestitemsDb = SQLDatabase.Get(new RowList<Store.Objects.QuestRequestItemsLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesQuestRequestItems, requestitemsDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesPageText()
        {
            if (Storage.LocalesPageText.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.page_text_locale)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesPageText)
                {
                    result += "UPDATE `locales_page_text` SET `Text_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Text) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var pagetextDb = SQLDatabase.Get(new RowList<Store.Objects.PageTextLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesPageText, pagetextDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesTrainer()
        {
            if (Storage.LocalesTrainer.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.trainer_locale)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesTrainer)
                {
                    result += "UPDATE `npc_trainer_greeting` SET `content_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Greeting) + "' WHERE `entry`=" + locale.Item1.TrainerEntry + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var trainerDb = SQLDatabase.Get(new RowList<Store.Objects.TrainerLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesTrainer, trainerDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string LocalesPointsOfInterest()
        {
            if (Storage.LocalesPointsOfInterest.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.points_of_interest_locale)
                return string.Empty;

            if (Settings.TargetedDbType == TargetedDbType.VMANGOS)
            {
                string result = "SET NAMES 'utf8';" + Environment.NewLine;
                foreach (var locale in Storage.LocalesPointsOfInterest)
                {
                    result += "UPDATE `locales_points_of_interest` SET `icon_name_loc" + ClientLocale.GetLocaleIndexFromLocaleName(locale.Item1.Locale) + "`='" + SQLUtil.EscapeString(locale.Item1.Name) + "' WHERE `entry`=" + locale.Item1.ID + ";" + Environment.NewLine;
                }
                return result;
            }

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var pointsOfInterestDb = SQLDatabase.Get(new RowList<Store.Objects.PointsOfInterestLocale>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.LocalesPointsOfInterest, pointsOfInterestDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string PlayerChoiceLocale()
        {
            if (Storage.PlayerChoiceLocales.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.playerchoice_locale)
                return string.Empty;

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var playerChoiceDb = SQLDatabase.Get(new RowList<Store.Objects.PlayerChoiceLocaleTemplate>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.PlayerChoiceLocales, playerChoiceDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }

        [BuilderMethod]
        public static string PlayerChoiceResponseLocale()
        {
            if (Storage.PlayerChoiceResponseLocales.IsEmpty())
                return string.Empty;

            if (!Settings.SqlTables.playerchoice_locale)
                return string.Empty;

            // pass empty list, because we want to select the whole db table (faster than select only needed columns)
            var playerChoiceResponseDb = SQLDatabase.Get(new RowList<Store.Objects.PlayerChoiceResponseLocaleTemplate>());

            return "SET NAMES 'utf8';" + Environment.NewLine + SQLUtil.Compare(Storage.PlayerChoiceResponseLocales, playerChoiceResponseDb, StoreNameType.None) + Environment.NewLine + "SET NAMES 'latin1';";
        }
    }
}
