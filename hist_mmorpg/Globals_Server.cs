using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing any required static variables for server-side
    /// </summary>
    public static class Globals_Server
    {
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
        public static Dictionary<string, Rank> rankMasterList = new Dictionary<string, Rank>();
        /// <summary>
        /// Holds keys for Rank objects (used when retrieving from database)
        /// </summary>
        public static List<String> rankKeys = new List<String>();
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
        /// Holds combat values for different troop types and nationalities
        /// Key = nationality & Value = combat value for knights, menAtArms, lightCavalry, yeomen, foot, rabble
        /// </summary>
        public static Dictionary<string, uint[]> combatValues = new Dictionary<string, uint[]>();
        /// <summary>
        /// Holds recruitment ratios for different troop types and nationalities
        /// Key = nationality & Value = % ratio for knights, menAtArms, lightCavalry, yeomen, foot, rabble
        /// </summary>
        public static Dictionary<string, double[]> recruitRatios = new Dictionary<string, double[]>();
        /// <summary>
        /// Holds probabilities for battle occurring at certain combat odds under certain conditions
        /// Key = 'odds', 'battle', 'pillage'
        /// Value = percentage likelihood of battle occurring
        /// </summary>
        public static Dictionary<string, double[]> battleProbabilities = new Dictionary<string, double[]>();
        /// <summary>
        /// Holds priorities for types of JournalEntry
        /// Key = JournalEntry type
        /// Value = 0-2 byte indicating priority level
        /// </summary>
        public static Dictionary<string[], byte> jEntryPriorities = new Dictionary<string[], byte>();

        /// <summary>
        /// Gets the next available newCharID, then increments it
        /// </summary>
        /// <returns>uint containing newCharID</returns>
        public static uint getNextCharID()
        {
            uint charID = Globals_Server.newCharID;
            Globals_Server.newCharID++;
            return charID;
        }

        /// <summary>
        /// Gets the next available newArmyID, then increments it
        /// </summary>
        /// <returns>string containing newArmyID</returns>
        public static string getNextArmyID()
        {
            string armyID = "Army_" + Globals_Server.newArmyID;
            Globals_Server.newArmyID++;
            return armyID;
        }

        /// <summary>
        /// Gets the next available newDetachmentID, then increments it
        /// </summary>
        /// <returns>string containing newDetachmentID</returns>
        public static string getNextDetachmentID()
        {
            string detachmentID = "Det_" + Globals_Server.newDetachmentID;
            Globals_Server.newDetachmentID++;
            return detachmentID;
        }

        /// <summary>
        /// Gets the next available newAilmentID, then increments it
        /// </summary>
        /// <returns>string containing newAilmentID</returns>
        public static string getNextAilmentID()
        {
            string ailmentID = "Ail_" + Globals_Server.newAilmentID;
            Globals_Server.newAilmentID++;
            return ailmentID;
        }

        /// <summary>
        /// Gets the next available newSiegeID, then increments it
        /// </summary>
        /// <returns>string containing newSiegeID</returns>
        public static string getNextSiegeID()
        {
            string siegeID = "Siege_" + Globals_Server.newSiegeID;
            Globals_Server.newSiegeID++;
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
            newID = Globals_Server.newJournalEntryID;
            // System.Windows.Forms.MessageBox.Show("jEntryID: " + newID.ToString());

            // increment newJournalEntryID
            Globals_Server.newJournalEntryID++;

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

            success = Globals_Server.scheduledEvents.addNewEntry(jEvent);

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

            success = Globals_Server.pastEvents.addNewEntry(jEvent);
            if (success)
            {
                Globals_Server.notifyObservers("newEvent|" + jEvent.jEntryID);
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
}
