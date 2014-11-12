﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing any required game-wide static variables and related methods
    /// </summary>
    public static class Globals_Game
    {
        /// <summary>
        /// Holds data for all players required for the calculation of individual victory
        /// </summary>
        public static Dictionary<string, VictoryData> victoryData = new Dictionary<string,VictoryData>();
        /// <summary>
        /// Holds keys for VictoryData objects (used when retrieving from database)
        /// </summary>
        public static List<String> victoryDataKeys = new List<String>();
        /// <summary>
        /// Holds PlayerCharacter associated with the position of sysAdmin for the game
        /// </summary>
        public static PlayerCharacter sysAdmin;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of king for kingdom one
        /// </summary>
        public static PlayerCharacter kingOne;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of king for kingdom two
        /// </summary>
        public static PlayerCharacter kingTwo;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of prince for kingdom one
        /// </summary>
        public static PlayerCharacter princeOne;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of prince for kingdom two
        /// </summary>
        public static PlayerCharacter princeTwo;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of herald for kingdom one
        /// </summary>
        public static PlayerCharacter heraldOne;
        /// <summary>
        /// Holds PlayerCharacter associated with the position of herald for kingdom two
        /// </summary>
        public static PlayerCharacter heraldTwo;
        /// <summary>
        /// List of registered observers
        /// </summary>
        private static List<Form1> registeredObservers = new List<Form1>();
        /// <summary>
        /// Holds all NonPlayerCharacter objects
        /// </summary>
        public static Dictionary<string, NonPlayerCharacter> npcMasterList = new Dictionary<string, NonPlayerCharacter>();
        /// <summary>
        /// Holds keys for NonPlayerCharacter objects (used when retrieving from database)
        /// </summary>
        public static List<String> npcKeys = new List<String>();
        /// <summary>
        /// Holds all PlayerCharacter objects
        /// </summary>
        public static Dictionary<string, PlayerCharacter> pcMasterList = new Dictionary<string, PlayerCharacter>();
        /// <summary>
        /// Holds keys for PlayerCharacter objects (used when retrieving from database)
        /// </summary>
        public static List<String> pcKeys = new List<String>();
        /// <summary>
        /// Holds all Fief objects
        /// </summary>
        public static Dictionary<string, Fief> fiefMasterList = new Dictionary<string, Fief>();
        /// <summary>
        /// Holds keys for Fief objects (used when retrieving from database)
        /// </summary>
        public static List<String> fiefKeys = new List<String>();
        /// <summary>
        /// Holds all Province objects
        /// </summary>
        public static Dictionary<string, Province> provinceMasterList = new Dictionary<string, Province>();
        /// <summary>
        /// Holds keys for Province objects (used when retrieving from database)
        /// </summary>
        public static List<String> provKeys = new List<String>();
        /// <summary>
        /// Holds all Kingdom objects
        /// </summary>
        public static Dictionary<string, Kingdom> kingdomMasterList = new Dictionary<string, Kingdom>();
        /// <summary>
        /// Holds keys for Kingdom objects (used when retrieving from database)
        /// </summary>
        public static List<String> kingKeys = new List<String>();
        /// <summary>
        /// Holds all Rank objects
        /// </summary>
        public static Dictionary<byte, Rank> rankMasterList = new Dictionary<byte, Rank>();
        /// <summary>
        /// Holds keys for Rank objects (used when retrieving from database)
        /// </summary>
        public static List<byte> rankKeys = new List<byte>();
        /// <summary>
        /// Holds all Terrain objects
        /// </summary>
        public static Dictionary<string, Terrain> terrainMasterList = new Dictionary<string, Terrain>();
        /// <summary>
        /// Holds keys for Terrain objects (used when retrieving from database)
        /// </summary>
        public static List<String> terrKeys = new List<String>();
        /// <summary>
        /// Holds all Language objects
        /// </summary>
        public static Dictionary<string, Language> languageMasterList = new Dictionary<string, Language>();
        /// <summary>
        /// Holds keys for Language objects (used when retrieving from database)
        /// </summary>
        public static List<String> langKeys = new List<String>();
        /// <summary>
        /// Holds all Skill objects
        /// </summary>
        public static Dictionary<string, Skill> skillMasterList = new Dictionary<string, Skill>();
        /// <summary>
        /// Holds keys for Skill objects (used when retrieving from database)
        /// </summary>
        public static List<String> skillKeys = new List<String>();
        /// <summary>
        /// Holds all army objects
        /// </summary>
        public static Dictionary<string, Army> armyMasterList = new Dictionary<string, Army>();
        /// <summary>
        /// Holds keys for army objects (used when retrieving from database)
        /// </summary>
        public static List<String> armyKeys = new List<String>();
        /// <summary>
        /// Holds all siege objects
        /// </summary>
        public static Dictionary<string, Siege> siegeMasterList = new Dictionary<string, Siege>();
        /// <summary>
        /// Holds keys for siege objects (used when retrieving from database)
        /// </summary>
        public static List<String> siegeKeys = new List<String>();
        /// <summary>
        /// Holds all nationality objects
        /// </summary>
        public static Dictionary<string, Nationality> nationalityMasterList = new Dictionary<string, Nationality>();
        /// <summary>
        /// Holds keys for nationality objects (used when retrieving from database)
        /// </summary>
        public static List<string> nationalityKeys = new List<string>();
        /// <summary>
        /// Holds all position objects
        /// </summary>
        public static Dictionary<byte, Position> positionMasterList = new Dictionary<byte, Position>();
        /// <summary>
        /// Holds keys for position objects (used when retrieving from database)
        /// </summary>
        public static List<byte> positionKeys = new List<byte>();
        /// <summary>
        /// Holds Character_Riak objects with existing goTo queues (used during initial load)
        /// </summary>
        public static List<Character_Riak> goToList = new List<Character_Riak>();
        /// <summary>
        /// Holds random for use with various methods
        /// </summary>
        public static Random myRand = new Random();
        /// <summary>
        /// Holds next value for character ID
        /// </summary>
        public static uint newCharID = 6300;
        /// <summary>
        /// Holds next value for army ID
        /// </summary>
        public static uint newArmyID = 1;
        /// <summary>
        /// Holds next value for detachment ID
        /// </summary>
        public static uint newDetachmentID = 1;
        /// <summary>
        /// Holds next value for ailment ID
        /// </summary>
        public static uint newAilmentID = 1;
        /// <summary>
        /// Holds next value for siege ID
        /// </summary>
        public static uint newSiegeID = 1;
        /// <summary>
        /// Holds next value for JournalEntry ID
        /// </summary>
        public static uint newJournalEntryID = 1;
        /// <summary>
        /// Holds HexMapGraph for this game
        /// </summary>
        public static HexMapGraph gameMap;
        /// <summary>
        /// Holds GameClock for this game
        /// </summary>
        public static GameClock clock { get; set; }
        /// <summary>
        /// Holds scheduled events
        /// </summary>
        public static Journal scheduledEvents = new Journal();
        /// <summary>
        /// Holds past events
        /// </summary>
        public static Journal pastEvents = new Journal();
        /// <summary>
        /// Holds priorities for types of JournalEntry
        /// Key = JournalEntry type
        /// Value = 0-2 byte indicating priority level
        /// </summary>
        public static Dictionary<string[], byte> jEntryPriorities = new Dictionary<string[], byte>();
        /// <summary>
        /// Holds type of game  (sets victory conditions)
        /// </summary>
        public static uint gameType = 0;
        /// <summary>
        /// Holds duration (number of turns) for the current game
        /// </summary>
        public static uint duration = 100;
        /// <summary>
        /// Holds start year for current game
        /// </summary>
        public static uint startYear { get; set; }
        /// <summary>
        /// Holds bool indicating whether or not to load initial object data from database on startup
        /// </summary>
        public static bool loadFromDatabase = false;
        /// <summary>
        /// Holds bool indicating whether or not to write current object data to database on exit
        /// </summary>
        public static bool writeToDatabase = false;

        /// <summary>
        /// Gets the game's end date (year)
        /// </summary>
        /// <returns>uint containing end year</returns>
        public static uint getGameEndDate()
        {
            return Globals_Game.startYear + Globals_Game.duration;
        }
        
        /// <summary>
        /// Gets the current scores for all players
        /// </summary>
        /// <returns>SortedList<double, string> containing current scores</returns>
        public static SortedList<double, string> getCurrentScores()
        {
            SortedList<double, string> currentScores = new SortedList<double, string>();
            double thisScore = 0;

            foreach (KeyValuePair<string, VictoryData> scoresEntry in Globals_Game.victoryData)
            {
                // reset score
                thisScore = 0;

                // get score
                thisScore += scoresEntry.Value.calcStatureScore();
                thisScore += scoresEntry.Value.calcPopulationScore();
                thisScore += scoresEntry.Value.calcFiefScore();

                // add to list
                currentScores.Add(thisScore, scoresEntry.Value.playerID);
            }

            return currentScores;
        }
        
        /// <summary>
        /// Gets the total population for all fiefs in the game
        /// </summary>
        /// <returns>int containing total population</returns>
        public static int getTotalPopulation()
        {
            int totPop = 0;

            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                totPop += fiefEntry.Value.population;
            }

            return totPop;
        }

        /// <summary>
        /// Gets the total number of fiefs in the game
        /// </summary>
        /// <returns>int containing number of fiefs</returns>
        public static int getTotalFiefs()
        {
            return Globals_Game.fiefMasterList.Count;
        }

        /// <summary>
        /// Gets the next available newCharID, then increments it
        /// </summary>
        /// <returns>uint containing newCharID</returns>
        public static uint getNextCharID()
        {
            uint charID = Globals_Game.newCharID;
            Globals_Game.newCharID++;
            return charID;
        }

        /// <summary>
        /// Gets the next available newArmyID, then increments it
        /// </summary>
        /// <returns>string containing newArmyID</returns>
        public static string getNextArmyID()
        {
            string armyID = "Army_" + Globals_Game.newArmyID;
            Globals_Game.newArmyID++;
            return armyID;
        }

        /// <summary>
        /// Gets the next available newDetachmentID, then increments it
        /// </summary>
        /// <returns>string containing newDetachmentID</returns>
        public static string getNextDetachmentID()
        {
            string detachmentID = "Det_" + Globals_Game.newDetachmentID;
            Globals_Game.newDetachmentID++;
            return detachmentID;
        }

        /// <summary>
        /// Gets the next available newAilmentID, then increments it
        /// </summary>
        /// <returns>string containing newAilmentID</returns>
        public static string getNextAilmentID()
        {
            string ailmentID = "Ail_" + Globals_Game.newAilmentID;
            Globals_Game.newAilmentID++;
            return ailmentID;
        }

        /// <summary>
        /// Gets the next available newSiegeID, then increments it
        /// </summary>
        /// <returns>string containing newSiegeID</returns>
        public static string getNextSiegeID()
        {
            string siegeID = "Siege_" + Globals_Game.newSiegeID;
            Globals_Game.newSiegeID++;
            return siegeID;
        }

        /// <summary>
        /// Gets the next available newJournalEntryID, then increments it
        /// </summary>
        /// <returns>uint containing newJournalEntryID</returns>
        public static uint getNextJournalEntryID()
        {
            uint newID = 0;

            // get newJournalEntryID
            newID = Globals_Game.newJournalEntryID;
            // System.Windows.Forms.MessageBox.Show("jEntryID: " + newID.ToString());

            // increment newJournalEntryID
            Globals_Game.newJournalEntryID++;

            return newID;
        }

        /// <summary>
        /// Generates a random double, specifying maximum and (optional) minimum values
        /// </summary>
        /// <returns>random double</returns>
        /// <param name="max">maximum value</param>
        /// <param name="min">minimum value</param>
        public static double GetRandomDouble(double max, double min = 0)
        {
            return myRand.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Adds a new JournalEntry to the scheduledEvents Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public static bool addScheduledEvent(JournalEntry jEvent)
        {
            bool success = false;

            success = Globals_Game.scheduledEvents.addNewEntry(jEvent);

            return success;

        }

        /// <summary>
        /// Adds a new JournalEntry to the pastEvents Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public static bool addPastEvent(JournalEntry jEvent)
        {
            bool success = false;

            success = Globals_Game.pastEvents.addNewEntry(jEvent);
            if (success)
            {
                Globals_Game.notifyObservers("newEvent|" + jEvent.jEntryID);
            }

            return success;

        }

        /// <summary>
        /// Adds an observer (Form1 object) to the list of registered observers
        /// </summary>
        /// <param name="obs">Observer to be added</param>
        public static void registerObserver(Form1 obs)
        {
            // add new observer to list
            registeredObservers.Add(obs);
        }

        /// <summary>
        /// Removes an observer (Form1 object) from the list of registered observers
        /// </summary>
        /// <param name="obs">Observer to be removed</param>
        public static void removeObserver(Form1 obs)
        {
            // remove observer from list
            registeredObservers.Remove(obs);
        }

        /// <summary>
        /// Notifies all observers (Form1 objects) in the list of registered observers
        /// that a change has occured to the data
        /// </summary>
        /// <param name="info">String indicating the nature of the change</param>
        public static void notifyObservers(String info)
        {
            // iterate through list of observers
            foreach (Form1 obs in registeredObservers)
            {
                // call observer's update method to perform the required actions
                // based on the string passed
                obs.update(info);
            }
        }
    }

    /// <summary>
    /// Class storing data on which to base individual victory
    /// </summary>
    public class VictoryData
    {
        /// <summary>
        /// Holds player ID
        /// </summary>
        public string playerID;
        /// <summary>
        /// Holds PlayerCharacter ID
        /// </summary>
        public string playerCharacterID;
        /// <summary>
        /// Holds player's stature at start of game
        /// </summary>
        public double startStature;
        /// <summary>
        /// Holds player's current stature
        /// </summary>
        public double currentStature;
        /// <summary>
        /// Holds percentage of population under player's control at start of game
        /// </summary>
        public double startPopulation;
        /// <summary>
        /// Holds percentage of population currently under player's control
        /// </summary>
        public double currentPopulation;
        /// <summary>
        /// Holds percentage of fiefs under player's control at start of game
        /// </summary>
        public double startFiefs;
        /// <summary>
        /// Holds percentage of fiefs currently under player's control
        /// </summary>
        public double currentFiefs;

        /// <summary>
        /// Constructor for VictoryData
        /// </summary>
        /// <param name="player">string holding Language ID</param>
        /// <param name="pc">string holding PlayerCharacter ID</param>
        /// <param name="stat">double player's stature at start of game</param>
        /// <param name="pop">double holding percentage of population under player's control at start of game</param>
        /// <param name="fiefs">double holding percentage of fiefs under player's control at start of game</param>
        public VictoryData(string player, string pc, double stat, double pop, double fiefs)
        {
            this.playerID = player;
            this.playerCharacterID = pc;
            this.startStature = stat;
            this.currentStature = 0;
            this.startPopulation = pop;
            this.currentPopulation = 0;
            this.startFiefs = fiefs;
            this.currentFiefs = 0;
        }

        /// <summary>
        /// Constructor for VictoryData taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public VictoryData()
        {
        }

        /// <summary>
        /// Updates the current data
        /// </summary>
        public void updateData()
        {
            // get PlayerCharacter
            PlayerCharacter thisPC = null;
            if (Globals_Game.pcMasterList.ContainsKey(this.playerCharacterID))
            {
                thisPC = Globals_Game.pcMasterList[this.playerCharacterID];
            }

            // update data
            if (thisPC != null)
            {
                // stature
                this.currentStature = thisPC.calculateStature();

                // population governed
                this.currentPopulation = thisPC.getPopulationPercentage();

                // fiefs owned
                this.currentFiefs = thisPC.getFiefsPercentage();
            }
        }
        
        /// <summary>
        /// Calculates the current stature score
        /// </summary>
        /// <returns>double containing the stature score</returns>
        public double calcStatureScore()
        {
            double statScore = 0;

            statScore = this.currentStature + (this.currentStature - this.startStature);

            return statScore;
        }

        /// <summary>
        /// Calculates the current population  score
        /// </summary>
        /// <returns>double containing the population score</returns>
        public double calcPopulationScore()
        {
            double popScore = 0;

            popScore = (this.currentPopulation + (this.currentPopulation - this.startPopulation)) / 10;

            return popScore;
        }

        /// <summary>
        /// Calculates the current fief score
        /// </summary>
        /// <returns>double containing the fief score</returns>
        public double calcFiefScore()
        {
            double fiefScore = 0;

            fiefScore = (this.currentFiefs + (this.currentFiefs - this.startFiefs)) / 10;

            return fiefScore;
        }
    }
}