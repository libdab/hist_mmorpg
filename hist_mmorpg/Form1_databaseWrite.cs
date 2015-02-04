using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using QuickGraph;
using CorrugatedIron;
using CorrugatedIron.Models;

namespace hist_mmorpg
{
    /// <summary>
    /// Partial class for Form1, containing functionality specific to writing to the back-end database
    /// </summary>
    partial class Form1
    {

        /// <summary>
        /// Writes all game objects to the database
        /// </summary>
        /// <param name="gameID">ID of game (used for database bucket)</param>
        public void DatabaseWrite(string gameID)
        {
            // ========= write CLOCK
            this.DatabaseWrite_Clock(gameID, Globals_Game.clock);

            // ========= write GLOBALS_SERVER/GAME DICTIONARIES
            this.DatabaseWrite_Dictionary(gameID, "combatValues", Globals_Server.combatValues);
            this.DatabaseWrite_Dictionary(gameID, "recruitRatios", Globals_Server.recruitRatios);
            this.DatabaseWrite_Dictionary(gameID, "battleProbabilities", Globals_Server.battleProbabilities);
            this.DatabaseWrite_Dictionary(gameID, "gameTypes", Globals_Server.gameTypes);
            this.DatabaseWrite_Dictionary(gameID, "ownershipChallenges", Globals_Game.ownershipChallenges);
            // convert jEntryPriorities prior to writing
            Dictionary<string, byte> jEntryPriorities_serialised = this.jEntryPriorities_serialise(Globals_Game.jEntryPriorities);
            this.DatabaseWrite_Dictionary(gameID, "jEntryPriorities", jEntryPriorities_serialised);

            // ========= write JOURNALS
            this.DatabaseWrite_Journal(gameID, "serverScheduledEvents", Globals_Game.scheduledEvents);
            this.DatabaseWrite_Journal(gameID, "serverPastEvents", Globals_Game.pastEvents);
            this.DatabaseWrite_Journal(gameID, "clientPastEvents", Globals_Client.myPastEvents);

            // ========= write GLOBALS_GAME/CLIENT/SERVER CHARACTER VARIABLES
            // Globals_Client.myPlayerCharacter
            if (Globals_Client.myPlayerCharacter != null)
            {
                this.DatabaseWrite_String(gameID, "myPlayerCharacter", Globals_Client.myPlayerCharacter.charID);
            }
            // Globals_Game.sysAdmin
            if (Globals_Game.sysAdmin != null)
            {
                this.DatabaseWrite_String(gameID, "sysAdmin", Globals_Game.sysAdmin.charID);
            }
            // Globals_Game.kingOne
            if (Globals_Game.kingOne != null)
            {
                this.DatabaseWrite_String(gameID, "kingOne", Globals_Game.kingOne.charID);
            }
            // Globals_Game.kingTwo
            if (Globals_Game.kingTwo != null)
            {
                this.DatabaseWrite_String(gameID, "kingTwo", Globals_Game.kingTwo.charID);
            }
            // Globals_Game.princeOne
            if (Globals_Game.princeOne != null)
            {
                this.DatabaseWrite_String(gameID, "princeOne", Globals_Game.princeOne.charID);
            }
            // Globals_Game.princeTwo
            if (Globals_Game.princeTwo != null)
            {
                this.DatabaseWrite_String(gameID, "princeTwo", Globals_Game.princeTwo.charID);
            }
            // Globals_Game.heraldOne
            if (Globals_Game.heraldOne != null)
            {
                this.DatabaseWrite_String(gameID, "heraldOne", Globals_Game.heraldOne.charID);
            }
            // Globals_Game.heraldTwo
            if (Globals_Game.heraldTwo != null)
            {
                this.DatabaseWrite_String(gameID, "heraldTwo", Globals_Game.heraldTwo.charID);
            }

            // ========= write GLOBALS_GAME/CLIENT/SERVER newID VARIABLES
            // newCharID
            this.DatabaseWrite_newID(gameID, "newCharID", Globals_Game.newCharID);
            // newArmyID
            this.DatabaseWrite_newID(gameID, "newArmyID", Globals_Game.newArmyID);
            // newDetachmentID
            this.DatabaseWrite_newID(gameID, "newDetachmentID", Globals_Game.newDetachmentID);
            // newAilmentID
            this.DatabaseWrite_newID(gameID, "newAilmentID", Globals_Game.newAilmentID);
            // newSiegeID
            this.DatabaseWrite_newID(gameID, "newSiegeID", Globals_Game.newSiegeID);
            // newJournalEntryID
            this.DatabaseWrite_newID(gameID, "newJournalEntryID", Globals_Game.newJournalEntryID);
            // gameType
            this.DatabaseWrite_newID(gameID, "gameType", Globals_Game.gameType);
            // duration
            this.DatabaseWrite_newID(gameID, "duration", Globals_Game.duration);
            // startYear
            this.DatabaseWrite_newID(gameID, "startYear", Globals_Game.startYear);
            // newGameID
            this.DatabaseWrite_newID(gameID, "newGameID", Globals_Server.newGameID);
            // newOwnChallengeID
            this.DatabaseWrite_newID(gameID, "newOwnChallengeID", Globals_Game.newOwnChallengeID);

            // ========= write GLOBALS_GAME/CLIENT/SERVER BOOL VARIABLES
            // Globals_Game.loadFromDatabase
            this.DatabaseWrite_Bool(gameID, "loadFromDatabase", Globals_Game.loadFromDatabase);
            // Globals_Game.loadFromCSV
            this.DatabaseWrite_Bool(gameID, "loadFromCSV", Globals_Game.loadFromCSV);
            // Globals_Game.writeToDatabase
            this.DatabaseWrite_Bool(gameID, "writeToDatabase", Globals_Game.writeToDatabase);
            // Globals_Game.statureCapInForce
            this.DatabaseWrite_Bool(gameID, "statureCapInForce", Globals_Game.statureCapInForce);
            // Globals_Client.showMessages
            this.DatabaseWrite_Bool(gameID, "showMessages", Globals_Client.showMessages);
            // Globals_Client.showDebugMessages
            this.DatabaseWrite_Bool(gameID, "showDebugMessages", Globals_Client.showDebugMessages);

            // ========= write SKILLS
            // clear existing key list
            if (Globals_Game.skillKeys.Count > 0)
            {
                Globals_Game.skillKeys.Clear();
            }

            // write each object in skillMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Skill> pair in Globals_Game.skillMasterList)
            {
                bool success = this.DatabaseWrite_Skill(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.skillKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "skillKeys", Globals_Game.skillKeys);

            // ========= write BASELANGUAGES
            // clear existing key list
            if (Globals_Game.baseLangKeys.Count > 0)
            {
                Globals_Game.baseLangKeys.Clear();
            }

            // write each object in baseLanguageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, BaseLanguage> pair in Globals_Game.baseLanguageMasterList)
            {
                bool success = this.DatabaseWrite_BaseLanguage(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.baseLangKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "baseLangKeys", Globals_Game.baseLangKeys);

            // ========= write LANGUAGES
            // clear existing key list
            if (Globals_Game.langKeys.Count > 0)
            {
                Globals_Game.langKeys.Clear();
            }

            // write each object in languageMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Language> pair in Globals_Game.languageMasterList)
            {
                bool success = this.DatabaseWrite_Language(gameID, l: pair.Value);
                if (success)
                {
                    Globals_Game.langKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "langKeys", Globals_Game.langKeys);

            // ========= write NATIONALITY OBJECTS
            // clear existing key list
            if (Globals_Game.nationalityKeys.Count > 0)
            {
                Globals_Game.nationalityKeys.Clear();
            }

            // write each object in nationalityMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Nationality> pair in Globals_Game.nationalityMasterList)
            {
                bool success = this.DatabaseWrite_Nationality(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.nationalityKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "nationalityKeys", Globals_Game.nationalityKeys);

            // ========= write RANKS
            // clear existing key list
            if (Globals_Game.rankKeys.Count > 0)
            {
                Globals_Game.rankKeys.Clear();
            }

            // write each object in rankMasterList, whilst also repopulating key list
            foreach (KeyValuePair<byte, Rank> pair in Globals_Game.rankMasterList)
            {
                bool success = this.DatabaseWrite_Rank(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.rankKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "rankKeys", Globals_Game.rankKeys);

            // ========= write POSITIONS
            // clear existing key list
            if (Globals_Game.positionKeys.Count > 0)
            {
                Globals_Game.positionKeys.Clear();
            }

            // write each object in positionMasterList, whilst also repopulating key list
            foreach (KeyValuePair<byte, Position> pair in Globals_Game.positionMasterList)
            {
                bool success = this.DatabaseWrite_Position(gameID, p: pair.Value);
                if (success)
                {
                    Globals_Game.positionKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "positionKeys", Globals_Game.positionKeys);

            // ========= write NPCs
            // clear existing key list
            if (Globals_Game.npcKeys.Count > 0)
            {
                Globals_Game.npcKeys.Clear();
            }

            // write each object in npcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, NonPlayerCharacter> pair in Globals_Game.npcMasterList)
            {
                bool success = this.DatabaseWrite_NPC(gameID, npc: pair.Value);
                if (success)
                {
                    Globals_Game.npcKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "npcKeys", Globals_Game.npcKeys);

            // ========= write PCs
            // clear existing key list
            if (Globals_Game.pcKeys.Count > 0)
            {
                Globals_Game.pcKeys.Clear();
            }

            // write each object in pcMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, PlayerCharacter> pair in Globals_Game.pcMasterList)
            {
                bool success = this.DatabaseWrite_PC(gameID, pc: pair.Value);
                if (success)
                {
                    Globals_Game.pcKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "pcKeys", Globals_Game.pcKeys);

            // ========= write KINGDOMS
            // clear existing key list
            if (Globals_Game.kingKeys.Count > 0)
            {
                Globals_Game.kingKeys.Clear();
            }

            // write each object in kingdomMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Kingdom> pair in Globals_Game.kingdomMasterList)
            {
                bool success = this.DatabaseWrite_Kingdom(gameID, k: pair.Value);
                if (success)
                {
                    Globals_Game.kingKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "kingKeys", Globals_Game.kingKeys);

            // ========= write PROVINCES
            // clear existing key list
            if (Globals_Game.provKeys.Count > 0)
            {
                Globals_Game.provKeys.Clear();
            }

            // write each object in provinceMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Province> pair in Globals_Game.provinceMasterList)
            {
                bool success = this.DatabaseWrite_Province(gameID, p: pair.Value);
                if (success)
                {
                    Globals_Game.provKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "provKeys", Globals_Game.provKeys);

            // ========= write TERRAINS
            // clear existing key list
            if (Globals_Game.terrKeys.Count > 0)
            {
                Globals_Game.terrKeys.Clear();
            }

            // write each object in terrainMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Terrain> pair in Globals_Game.terrainMasterList)
            {
                bool success = this.DatabaseWrite_Terrain(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.terrKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "terrKeys", Globals_Game.terrKeys);

            // ========= write VICTORYDATA OBJECTS
            // clear existing key list
            if (Globals_Game.victoryDataKeys.Count > 0)
            {
                Globals_Game.victoryDataKeys.Clear();
            }

            // write each object in victoryData, whilst also repopulating key list
            foreach (KeyValuePair<string, VictoryData> pair in Globals_Game.victoryData)
            {
                bool success = this.DatabaseWrite_VictoryData(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.victoryDataKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "victoryDataKeys", Globals_Game.victoryDataKeys);

            // ========= write FIEFS
            // clear existing key list
            if (Globals_Game.fiefKeys.Count > 0)
            {
                Globals_Game.fiefKeys.Clear();
            }

            // write each object in fiefMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Fief> pair in Globals_Game.fiefMasterList)
            {
                bool success = this.DatabaseWrite_Fief(gameID, f: pair.Value);
                if (success)
                {
                    Globals_Game.fiefKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "fiefKeys", Globals_Game.fiefKeys);

            // ========= write ARMIES
            // clear existing key list
            if (Globals_Game.armyKeys.Count > 0)
            {
                Globals_Game.armyKeys.Clear();
            }

            // write each object in armyMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Army> pair in Globals_Game.armyMasterList)
            {
                bool success = this.DatabaseWrite_Army(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.armyKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "armyKeys", Globals_Game.armyKeys);

            // ========= write SIEGES
            // clear existing key list
            if (Globals_Game.siegeKeys.Count > 0)
            {
                Globals_Game.siegeKeys.Clear();
            }

            // write each object in siegeMasterList, whilst also repopulating key list
            foreach (KeyValuePair<string, Siege> pair in Globals_Game.siegeMasterList)
            {
                bool success = this.DatabaseWrite_Siege(gameID, pair.Value);
                if (success)
                {
                    Globals_Game.siegeKeys.Add(pair.Key);
                }
            }

            // write key list to database
            this.DatabaseWrite_KeyList(gameID, "siegeKeys", Globals_Game.siegeKeys);

            // ========= write MAP (edges collection)
            this.DatabaseWrite_MapEdges(gameID, map: Globals_Game.gameMap);

        }

        /// <summary>
        /// Writes a key list (List object) to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="k">key of key list</param>
        /// <param name="kl">key list to write</param>
        public bool DatabaseWrite_KeyList<T>(string gameID, string k, List<T> kl)
        {

            var rList = new RiakObject(gameID, k, kl);
            var putListResult = rClient.Put(rList);

            if (!putListResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Key list " + rList.Key + " to bucket " + rList.Bucket);
                }
            }

            return putListResult.IsSuccess;
        }

        /// <summary>
        /// Writes a GameClock object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="gc">GameClock to write</param>
        public bool DatabaseWrite_Clock(string gameID, GameClock gc)
        {
            var rClock = new RiakObject(gameID, "gameClock", gc);
            var putClockResult = rClient.Put(rClock);

            if (!putClockResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: GameClock to bucket " + rClock.Bucket);
                }
            }

            return putClockResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Dictionary object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="dictionary">Dictionary to write</param>
        public bool DatabaseWrite_Dictionary<T>(string gameID, string key, T dictionary)
        {
            var rDict = new RiakObject(gameID, key, dictionary);
            var putDictResult = rClient.Put(rDict);

            if (!putDictResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Dictionary " + key + " to bucket " + rDict.Bucket);
                }
            }

            return putDictResult.IsSuccess;
        }

        /// <summary>
        /// Serialises jEntryPriorities Dictionary
        /// </summary>
        /// <returns>Dictionary(string, byte) for database storage</returns>
        /// <param name="dictToConvert">The Dictionary to convert</param>
        public Dictionary<string, byte> jEntryPriorities_serialise(Dictionary<string[], byte> dictToConvert)
        {
            Dictionary<string, byte> dictOut = new Dictionary<string, byte>();

            if (dictToConvert.Count > 0)
            {
                foreach (KeyValuePair<string[], byte> thisEntry in dictToConvert)
                {
                    dictOut.Add(thisEntry.Key[0] + "|" + thisEntry.Key[1], thisEntry.Value);
                }
            }

            return dictOut;
        }

        /// <summary>
        /// Writes a Journal object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="journal">Journal to write</param>
        public bool DatabaseWrite_Journal(string gameID, string key, Journal journal)
        {
            var rJournal = new RiakObject(gameID, key, journal);
            var putJournalResult = rClient.Put(rJournal);

            if (!putJournalResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Journal " + key + " to bucket " + rJournal.Bucket);
                }
            }

            return putJournalResult.IsSuccess;
        }

        /// <summary>
        /// Writes a string variable to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="pcID">string to write</param>
        public bool DatabaseWrite_String(string gameID, string key, string pcID)
        {
            pcID = "\"" + pcID + "\"";

            var rString = new RiakObject(gameID, key, pcID);
            var putStringResult = rClient.Put(rString);

            if (!putStringResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: String variable " + key + " to bucket " + rString.Bucket);
                }
            }

            return putStringResult.IsSuccess;
        }

        /// <summary>
        /// Writes a newID variable to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="newID">newID to write</param>
        public bool DatabaseWrite_newID(string gameID, string key, uint newID)
        {
            var rCharVar = new RiakObject(gameID, key, newID);
            var putCharVarResult = rClient.Put(rCharVar);

            if (!putCharVarResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: newID variable " + key + " to bucket " + rCharVar.Bucket);
                }
            }

            return putCharVarResult.IsSuccess;
        }

        /// <summary>
        /// Writes a bool variable to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="key">Database key to use</param>
        /// <param name="thisBool">bool to write</param>
        public bool DatabaseWrite_Bool(string gameID, string key, bool thisBool)
        {
            var rBool = new RiakObject(gameID, key, thisBool);
            var putBoolResult = rClient.Put(rBool);

            if (!putBoolResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Bool variable " + key + " to bucket " + rBool.Bucket);
                }
            }

            return putBoolResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Skill object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="s">Skill to write</param>
        public bool DatabaseWrite_Skill(string gameID, Skill s)
        {
            var rSkill = new RiakObject(gameID, s.skillID, s);
            var putSkillResult = rClient.Put(rSkill);

            if (!putSkillResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Skill " + rSkill.Key + " to bucket " + rSkill.Bucket);
                }
            }

            return putSkillResult.IsSuccess;
        }

        /// <summary>
        /// Writes a BaseLanguage object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="bl">BaseLanguage to write</param>
        public bool DatabaseWrite_BaseLanguage(string gameID, BaseLanguage bl)
        {

            var rBaseLanguage = new RiakObject(gameID, bl.id, bl);
            var putBaseLanguageResult = rClient.Put(rBaseLanguage);

            if (!putBaseLanguageResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: BaseLanguage " + rBaseLanguage.Key + " to bucket " + rBaseLanguage.Bucket);
                }
            }

            return putBaseLanguageResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Language or Language_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="l">Language to write</param>
        /// <param name="ls">Language_Serialised to write</param>
        public bool DatabaseWrite_Language(string gameID, Language l = null, Language_Serialised ls = null)
        {
            if (l != null)
            {
                // convert Language into Language_Serialised
                ls = this.Language_serialise(l);
            }

            // write Language_Serialised to database
            var rLanguage = new RiakObject(gameID, ls.id, ls);
            var putLanguageResult = rClient.Put(rLanguage);

            if (!putLanguageResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Language " + rLanguage.Key + " to bucket " + rLanguage.Bucket);
                }
            }

            return putLanguageResult.IsSuccess;
        }

        /// <summary>
        /// serialises a Language object
        /// </summary>
        /// <returns>Language_Serialised object</returns>
        /// <param name="l">Language to be converted</param>
        public Language_Serialised Language_serialise(Language l)
        {
            Language_Serialised langSerialisedOut = null;
            langSerialisedOut = new Language_Serialised(l);
            return langSerialisedOut;
        }

        /// <summary>
        /// Writes a Nationality object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="n">Nationality to write</param>
        public bool DatabaseWrite_Nationality(string gameID, Nationality n)
        {

            var rNationality = new RiakObject(gameID, n.natID, n);
            var putNationalityResult = rClient.Put(rNationality);

            if (!putNationalityResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Nationality " + rNationality.Key + " to bucket " + rNationality.Bucket);
                }
            }

            return putNationalityResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Rank object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="r">Rank to write</param>
        public bool DatabaseWrite_Rank(string gameID, Rank r)
        {

            var rRank = new RiakObject(gameID, r.id.ToString(), r);
            var putRankResult = rClient.Put(rRank);

            if (!putRankResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Rank " + rRank.Key + " to bucket " + rRank.Bucket);
                }
            }

            return putRankResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Position or Position_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="p">Position to write</param>
        /// <param name="ps">Position_Serialised to write</param>
        public bool DatabaseWrite_Position(string gameID, Position p = null, Position_Serialised ps = null)
        {
            if (p != null)
            {
                // convert Position into Position_Serialised
                ps = this.Position_serialise(p);
            }

            var rPos = new RiakObject(gameID, ps.id.ToString(), ps);
            var putPosResult = rClient.Put(rPos);

            if (!putPosResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Position " + rPos.Key + " to bucket " + rPos.Bucket);
                }
            }

            return putPosResult.IsSuccess;
        }

        /// <summary>
        /// serialises a Position object
        /// </summary>
        /// <returns>Position_Serialised object</returns>
        /// <param name="p">Position to be converted</param>
        public Position_Serialised Position_serialise(Position p)
        {
            Position_Serialised posOut = null;
            posOut = new Position_Serialised(p);
            return posOut;
        }

        /// <summary>
        /// Writes a NonPlayerCharacter or NonPlayerCharacter_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="npc">NonPlayerCharacter to write</param>
        /// <param name="npcs">NonPlayerCharacter_Serialised to write</param>
        public bool DatabaseWrite_NPC(string gameID, NonPlayerCharacter npc = null, NonPlayerCharacter_Serialised npcs = null)
        {
            if (npc != null)
            {
                // convert NonPlayerCharacter into NonPlayerCharacter_Serialised
                npcs = this.NPC_serialise(npc);
            }

            // write NonPlayerCharacter_Serialised to database
            var rNPC = new RiakObject(gameID, npcs.charID, npcs);
            var putNPCresult = rClient.Put(rNPC);

            if (!putNPCresult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: NPC " + rNPC.Key + " to bucket " + rNPC.Bucket);
                }
            }

            return putNPCresult.IsSuccess;
        }

        /// <summary>
        /// Serialises a NonPlayerCharacter object
        /// </summary>
        /// <returns>NonPlayerCharacter_Serialised object</returns>
        /// <param name="npc">NonPlayerCharacter to be converted</param>
        public NonPlayerCharacter_Serialised NPC_serialise(NonPlayerCharacter npc)
        {
            NonPlayerCharacter_Serialised npcSerialisedOut = null;
            npcSerialisedOut = new NonPlayerCharacter_Serialised(npc);
            return npcSerialisedOut;
        }

        /// <summary>
        /// Writes a PlayerCharacter or PlayerCharacter_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="pc">PlayerCharacter to write</param>
        /// <param name="pcs">PlayerCharacter_Serialised to write</param>
        public bool DatabaseWrite_PC(string gameID, PlayerCharacter pc = null, PlayerCharacter_Serialised pcs = null)
        {
            if (pc != null)
            {
                // convert PlayerCharacter into PlayerCharacter_Serialised
                pcs = this.PC_serialise(pc);
            }

            // write PlayerCharacter_Serialised to database
            var rPC = new RiakObject(gameID, pcs.charID, pcs);
            var putPCresult = rClient.Put(rPC);

            if (!putPCresult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: PC " + rPC.Key + " to bucket " + rPC.Bucket);
                }
            }

            return putPCresult.IsSuccess;
        }

        /// <summary>
        /// Serialises a PlayerCharacter object
        /// </summary>
        /// <returns>PlayerCharacter_Serialised object</returns>
        /// <param name="pc">PlayerCharacter to be converted</param>
        public PlayerCharacter_Serialised PC_serialise(PlayerCharacter pc)
        {
            PlayerCharacter_Serialised pcSerialisedOut = null;
            pcSerialisedOut = new PlayerCharacter_Serialised(pc);
            return pcSerialisedOut;
        }

        /// <summary>
        /// Writes a Kingdom or Kingdom_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="k">Kingdom to write</param>
        /// <param name="ks">Kingdom_Serialised to write</param>
        public bool DatabaseWrite_Kingdom(string gameID, Kingdom k = null, Kingdom_Serialised ks = null)
        {
            if (k != null)
            {
                // convert Kingdom into Kingdom_Serialised
                ks = this.Kingdom_serialise(k);
            }

            var rKing = new RiakObject(gameID, ks.id, ks);
            var putKingResult = rClient.Put(rKing);

            if (!putKingResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Kingdom " + rKing.Key + " to bucket " + rKing.Bucket);
                }
            }

            return putKingResult.IsSuccess;
        }

        /// <summary>
        /// Serialises a Kingdom object
        /// </summary>
        /// <returns>Kingdom_Serialised object</returns>
        /// <param name="k">Kingdom to be converted</param>
        public Kingdom_Serialised Kingdom_serialise(Kingdom k)
        {
            Kingdom_Serialised kingOut = null;
            kingOut = new Kingdom_Serialised(k);
            return kingOut;
        }

        /// <summary>
        /// Writes a Province or Province_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="p">Province to write</param>
        /// <param name="ps">Province_Serialised to write</param>
        public bool DatabaseWrite_Province(string gameID, Province p = null, Province_Serialised ps = null)
        {
            if (p != null)
            {
                // convert Province into Province_Serialised
                ps = this.Province_serialise(p);
            }

            var rProv = new RiakObject(gameID, ps.id, ps);
            var putProvResult = rClient.Put(rProv);

            if (!putProvResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Province " + rProv.Key + " to bucket " + rProv.Bucket);
                }
            }

            return putProvResult.IsSuccess;
        }

        /// <summary>
        /// Serialises a Province object
        /// </summary>
        /// <returns>Province_Serialised object</returns>
        /// <param name="p">Province to be converted</param>
        public Province_Serialised Province_serialise(Province p)
        {
            Province_Serialised oOut = null;
            oOut = new Province_Serialised(p);
            return oOut;
        }

        /// <summary>
        /// Writes a Terrain object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="t">Terrain to write</param>
        public bool DatabaseWrite_Terrain(string gameID, Terrain t)
        {

            var rTerrain = new RiakObject(gameID, t.id, t);
            var putTerrainResult = rClient.Put(rTerrain);

            if (!putTerrainResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Terrain " + rTerrain.Key + " to bucket " + rTerrain.Bucket);
                }
            }

            return putTerrainResult.IsSuccess;
        }

        /// <summary>
        /// Writes a VictoryData object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="vicDat">VictoryData to write</param>
        public bool DatabaseWrite_VictoryData(string gameID, VictoryData vicDat)
        {

            var rVictoryData = new RiakObject(gameID, vicDat.playerID, vicDat);
            var putVictoryDataResult = rClient.Put(rVictoryData);

            if (!putVictoryDataResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: VictoryData " + rVictoryData.Key + " to bucket " + rVictoryData.Bucket);
                }
            }

            return putVictoryDataResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Fief or Fief_Serialised object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="f">Fief to write</param>
        /// <param name="fs">Fief_Serialised to write</param>
        public bool DatabaseWrite_Fief(string gameID, Fief f = null, Fief_Serialised fs = null)
        {
            if (f != null)
            {
                // convert Fief to Fief_Serialised
                fs = this.Fief_serialise(f);
            }

            var rFief = new RiakObject(gameID, fs.id, fs);
            var putFiefResult = rClient.Put(rFief);

            if (!putFiefResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Fief " + rFief.Key + " to bucket " + rFief.Bucket);
                }
            }

            return putFiefResult.IsSuccess;
        }

        /// <summary>
        /// Serialises a Fief object
        /// </summary>
        /// <returns>Fief_Serialised object</returns>
        /// <param name="f">Fief to be converted</param>
        public Fief_Serialised Fief_serialise(Fief f)
        {
            Fief_Serialised fOut = null;
            fOut = new Fief_Serialised(f);
            return fOut;
        }

        /// <summary>
        /// Writes an Army object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="a">Army to write</param>
        public bool DatabaseWrite_Army(string gameID, Army a)
        {
            var rArmy = new RiakObject(gameID, a.armyID, a);
            var putArmyResult = rClient.Put(rArmy);

            if (!putArmyResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Army " + rArmy.Key + " to bucket " + rArmy.Bucket);
                }
            }

            return putArmyResult.IsSuccess;
        }

        /// <summary>
        /// Writes a Siege object to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="s">Siege to write</param>
        public bool DatabaseWrite_Siege(string gameID, Siege s)
        {
            var rSiege = new RiakObject(gameID, s.siegeID, s);
            var putSiegeResult = rClient.Put(rSiege);

            if (!putSiegeResult.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Siege " + rSiege.Key + " to bucket " + rSiege.Bucket);
                }
            }

            return putSiegeResult.IsSuccess;
        }

        /// <summary>
        /// Writes a HexMapGraph edges collection to the database
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="gameID">Game (bucket) to write to</param>
        /// <param name="map">HexMapGraph containing edges collection to write</param>
        /// <param name="edges">Edges collection to write</param>
        public bool DatabaseWrite_MapEdges(string gameID, HexMapGraph map = null, List<TaggedEdge<string, string>> edges = null)
        {
            if (map != null)
            {
                // convert Language into Language_Serialised
                edges = this.EdgeCollection_serialise(map.myMap.Edges.ToList());
            }

            var rMapE = new RiakObject(gameID, "mapEdges", edges);
            var putMapResultE = rClient.Put(rMapE);

            if (!putMapResultE.IsSuccess)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Write failed: Map edges collection " + rMapE.Key + " to bucket " + rMapE.Bucket);
                }
            }

            return putMapResultE.IsSuccess;
        }

        /// <summary>
        /// Serialises HexMapGraph edges collection
        /// </summary>
        /// <returns>Serialised edges collection</returns>
        /// <param name="edgesIn">Edges collection to be converted</param>
        public List<TaggedEdge<string, string>> EdgeCollection_serialise(List<TaggedEdge<Fief, string>> edgesIn)
        {
            List<TaggedEdge<string, string>> edgesOut = new List<TaggedEdge<string, string>>();

            foreach (TaggedEdge<Fief, string> element in edgesIn)
            {
                // convert each Fief object to string ID
                edgesOut.Add(this.EdgeFief_serialise(element));
            }

            return edgesOut;
        }

        /// <summary>
        /// Serialises HexMapGraph edge object
        /// </summary>
        /// <returns>Serialised edge</returns>
        /// <param name="te">HexMapGraph edge to be converted</param>
        public TaggedEdge<string, string> EdgeFief_serialise(TaggedEdge<Fief, string> te)
        {
            TaggedEdge<string, string> edgeOut = new TaggedEdge<string, string>(te.Source.id, te.Target.id, te.Tag);
            return edgeOut;
        }
    }
}
