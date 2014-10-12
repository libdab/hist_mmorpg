using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class allowing storage of game events (past and future)
    /// </summary>
    public class Journal
    {
        /// <summary>
        /// Holds entries
        /// </summary>
        public SortedList<double, JournalEntry> entries = new SortedList<double, JournalEntry>();
        /// <summary>
        /// Indicates presence of new (unread) entries
        /// </summary>
        public bool areNewEntries = false;
        /// <summary>
        /// Priority level of new (unread) entries
        /// </summary>
        public byte priority = 0;

        /// <summary>
        /// Constructor for Journal
        /// </summary>
        /// <param name="entList">SortedList<double, JournalEntry> holding entries</param>
        public Journal(SortedList<double, JournalEntry> entList = null)
        {
            if (entList != null)
            {
                this.entries = entList;
            }
        }
        
        /// <summary>
        /// Returns any entries matching search criteria (year, season)
        /// </summary>
        /// <returns>SortedList of JournalEntrys</returns>
        /// <param name="yr">Year to search for</param>
        /// <param name="seas">Season to search for</param>
        public SortedList<double, JournalEntry> getEventsOnDate(uint yr = 9999, Byte seas = 99)
        {
            SortedList<double, JournalEntry> matchingEntries = new SortedList<double, JournalEntry>();

            // determine scope of search
            String scope = "";
            // if no year specified, return events from all years and seasons
            if (yr == 9999)
            {
                scope = "all";
            }
            // if a year is specified
            else
            {
                // if no season specified, return events from all seasons in the specified year
                if (seas == 99)
                {
                    scope = "yr";
                }
                // if a season is specified, return events from specified season in the specified year
                else
                {
                    scope = "seas";
                }
            }

            switch (scope)
            {
                case "seas":
                    foreach (KeyValuePair<double, JournalEntry> jEntry in this.entries)
                    {
                        // year and season must match
                        if (jEntry.Value.year == yr)
                        {
                            if (jEntry.Value.season == seas)
                            {
                                matchingEntries.Add(jEntry.Key, jEntry.Value);
                            }
                        }
                    }
                    break;
                case "yr":
                    foreach (KeyValuePair<double, JournalEntry> jEntry in this.entries)
                    {
                        // year must match
                        if (jEntry.Value.year == yr)
                        {
                            matchingEntries.Add(jEntry.Key, jEntry.Value);
                        }
                    }
                    break;
                default:
                    foreach (KeyValuePair<double, JournalEntry> jEntry in this.entries)
                    {
                        // get all events
                        matchingEntries.Add(jEntry.Key, jEntry.Value);
                    }
                    break;
            }

            return matchingEntries;
        }

        /// <summary>
        /// Adds a new JournalEntry to the Journal
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="min">The JournalEntry to be added</param>
        public bool addNewEntry(JournalEntry jEntry)
        {
            bool success = false;

            if (jEntry.jEntryID > 0)
            {
                try
                {
                    // add entry
                    this.entries.Add(jEntry.jEntryID, jEntry);
                    success = true;
                }
                catch (System.ArgumentException ae)
                {
                    System.Windows.Forms.MessageBox.Show(ae.Message + "\r\nPlease check for duplicate jEventID.");
                }
            }

            return success;

        }

    }

    /// <summary>
    /// Class containing details of a Journal entry
    /// </summary>
    public class JournalEntry
    {
        /// <summary>
        /// Holds JournalEntry ID
        /// </summary>
        public double jEntryID { get; set; }
        /// <summary>
        /// Holds event year
        /// </summary>
        public uint year { get; set; }
        /// <summary>
        /// Holds event season
        /// </summary>
        public byte season { get; set; }
        /// <summary>
        /// Holds main objects (IDs) associated with event and their role
        /// </summary>
        public String[] personae { get; set; }
        /// <summary>
        /// Holds type of event (e.g. battle, birth)
        /// </summary>
        public String type { get; set; }
        /// <summary>
        /// Holds location of event (fiefID)
        /// </summary>
        public String location { get; set; }
        /// <summary>
        /// Holds description of event
        /// </summary>
        public String description { get; set; }

        /// <summary>
        /// Constructor for JournalEntry
        /// </summary>
        /// <param name="id">double holding JournalEntry ID</param>
        /// <param name="yr">uint holding event year</param>
        /// <param name="seas">byte holding event season</param>
        /// <param name="pers">String[] holding main objects (IDs) associated with event and thier role</param>
        /// <param name="typ">String holding type of event</param>
        /// <param name="loc">String holding location of event (fiefID)</param>
        /// <param name="descr">String holding description of event</param>
        public JournalEntry(double id, uint yr, byte seas, String[] pers, String typ, String loc = null, String descr = null)
        {
            this.jEntryID = id;
            this.year = yr;
            this.season = seas;
            this.personae = pers;
            this.type = typ;
            if (loc != null)
            {
                this.location = loc;
            }
            if (descr != null)
            {
                this.description = descr;
            }
        }

        /// <summary>
        /// Returns a string containing the details of a JournalEntry
        /// </summary>
        /// <returns>JournalEntry details</returns>
        public string getJournalEntryDetails()
        {
            string entryText = "";

            // ID
            entryText += "ID: " + this.jEntryID + "\r\n\r\n";

            // year and season
            entryText += "Date: " + Globals_Server.clock.seasons[this.season] + ", " + this.year + "\r\n\r\n";

            // type
            entryText += "Type: " + this.type + "\r\n\r\n";

            // personae
            entryText += "Personae:\r\n";
            for (int i = 0; i < this.personae.Length; i++)
            {
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');
                Character thisCharacter = null;

                // get character
                if (thisPersonaeSplit[0] != null)
                {
                    if (Globals_Server.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                    {
                        thisCharacter = Globals_Server.pcMasterList[thisPersonaeSplit[0]];
                    }
                    else if (Globals_Server.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                    {
                        thisCharacter = Globals_Server.npcMasterList[thisPersonaeSplit[0]];
                    }
                }

                entryText += thisCharacter.firstName + " " + thisCharacter.familyName
                    + " (" + thisPersonaeSplit[1] + ")\r\n";
            }
            entryText += "\r\n";

            // location
            if (this.location != null)
            {
                Fief thisFief = Globals_Server.fiefMasterList[this.location];
                entryText += "Location: " + thisFief.name + " (" + this.location + ")\r\n\r\n";
            }

            // description
            if (this.description != null)
            {
                Fief thisFief = Globals_Server.fiefMasterList[this.location];
                entryText += "Description:\r\n" + this.description + "\r\n\r\n";
            }

            return entryText;
        }

        /// <summary>
        /// Check the level of priority for the JournalEntry
        /// </summary>
        /// <returns>byte indicating the level of priority</returns>
        /// <param name="jEntry">The JournalEntry</param>
        public byte checkEventForPriority()
        {
            byte priority = 0;

            // get player's role
            string thisRole = "";
            for (int i = 0; i < this.personae.Length; i++)
            {
                string[] personaeSplit = this.personae[i].Split('|');
                if (personaeSplit[0].Equals(Globals_Client.myChar.charID))
                {
                    // if player made proposal, reduce priority
                    thisRole = personaeSplit[1];
                }
            }

            // get priority
            foreach (KeyValuePair <string[], byte> priorityEntry in Globals_Server.jEntryPriorities)
            {
                if (priorityEntry.Key[0] == this.type)
                {
                    // if player made proposal, reduce priority
                    if (thisRole.Equals(priorityEntry.Key[1]))
                    {
                        priority = priorityEntry.Value;
                    }
                }
            }

            return priority;
        }

        /// <summary>
        /// Check to see if the JournalEntry is of interest to the player
        /// </summary>
        /// <returns>bool indicating whether the JournalEntry is of interest</returns>
        public bool checkEventForInterest()
        {
            bool isOfInterest = false;

            for (int i = 0; i < this.personae.Length; i++)
            {
                string thisPersonae = this.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');
                if (thisPersonaeSplit[0].Equals(Globals_Client.myChar.charID))
                {
                    isOfInterest = true;
                    break;
                }
            }

            return isOfInterest;
        }

    }
}
