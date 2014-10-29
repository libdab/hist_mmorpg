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
        /// Holds Province to view in UI
        /// </summary>
        public static Province provinceToView;
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
        /// Holds past events
        /// </summary>
        public static Journal myPastEvents = new Journal();
        /// <summary>
        /// Holds current set of events being displayed in UI
        /// </summary>
        public static SortedList<uint, JournalEntry> eventSetToView = new SortedList<uint, JournalEntry>();
        /// <summary>
        /// Holds index position of currently displayed entry in eventSetToView
        /// </summary>
        public static int jEntryToView;
        /// <summary>
        /// Holds highest index position in eventSetToView
        /// </summary>
        public static int jEntryMax;
        /// <summary>
        /// Holds bool indicating whether or not to display popup messages
        /// </summary>
        public static bool showMessages = true;
        /// <summary>
        /// Holds bool indicating whether or not to display popup debug messages
        /// </summary>
        public static bool showDebugMessages = false;

    }

}
