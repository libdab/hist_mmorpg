using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing any required static variables for client-side
    /// </summary>
    public static class Globals_Client
    {
        /// <summary>
        /// Holds PlayerCharacter associated with the player
        /// </summary>
        public static PlayerCharacter myChar;
        /// <summary>
        /// Holds Character to view in UI
        /// </summary>
        public static Character charToView;
        /// <summary>
        /// Holds Fief to view in UI
        /// </summary>
        public static Fief fiefToView;
        /// <summary>
        /// Holds Army to view in UI
        /// </summary>
        public static Army armyToView;
        /// <summary>
        /// Holds Siege to view in UI
        /// </summary>
        public static Siege siegeToView;
        /// <summary>
        /// Holds UI container being currently displayed
        /// </summary>
        public static ContainerControl containerToView;
        /// <summary>
        /// Holds HexMapGraph for this game
        /// </summary>
        public static HexMapGraph gameMap;
        /// <summary>
        /// Holds GameClock for this game
        /// </summary>
        public static GameClock clock { get; set; }
    }
}
