using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing any required static variables
    /// </summary>
    public static class Globals
    {
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
        /// Holds combat values for different troop types and nationalities
        /// Key = nationality & Value = combat value for knights, menAtArms, lightCavalry, yeomen, foot, rabble
        /// </summary>
        public static Dictionary<string, uint[]> combatValues = new Dictionary<string, uint[]>();

        /// <summary>
        /// Gets the next available newCharID, then increments it
        /// </summary>
        /// <returns>uint containing newCharID</returns>
        public static uint getNextCharID()
        {
            uint charID = Globals.newCharID;
            Globals.newCharID++;
            return charID;
        }

        /// <summary>
        /// Gets the next available newArmyID, then increments it
        /// </summary>
        /// <returns>uint containing newArmyID</returns>
        public static uint getNextArmyID()
        {
            uint armyID = Globals.newArmyID;
            Globals.newArmyID++;
            return armyID;
        }

        /// <summary>
        /// Gets the next available newDetachmentID, then increments it
        /// </summary>
        /// <returns>string containing newDetachmentID</returns>
        public static string getNextDetachmentID()
        {
            string armyID = "D" + Globals.newDetachmentID;
            Globals.newDetachmentID++;
            return armyID;
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
    }
}
