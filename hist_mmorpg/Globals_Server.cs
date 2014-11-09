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
        /// Holds next value for game ID
        /// </summary>
        public static uint newGameID = 1;
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
        /// Holds types of game victory conditions
        /// </summary>
        public static Dictionary<uint, string> victoryConditionTypes = new Dictionary<uint, string>();

        /// <summary>
        /// Gets the next available newGameID, then increments it
        /// </summary>
        /// <returns>string containing newGameID</returns>
        public static string getNextGameID()
        {
            string gameID = "Game_" + Globals_Server.newGameID;
            Globals_Server.newGameID++;
            return gameID;
        }

    }
}
