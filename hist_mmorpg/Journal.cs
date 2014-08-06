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
        /// Holds events
        /// </summary>
        public List<JournalEvent> events = new List<JournalEvent>();

        /// <summary>
        /// Constructor for Journal
        /// </summary>
        /// <param name="evList">List<JournalEvent> holding events</param>
        public Journal(List<JournalEvent> evList = null)
        {
            if (evList != null)
            {
                this.events = evList;
            }
        }

        /// <summary>
        /// Returns any events matching search criteria (year, season)
        /// </summary>
        /// <returns>List of JournalEvents</returns>
        /// <param name="yr">Year to search for</param>
        /// <param name="seas">Season to search for</param>
        public List<JournalEvent> getEventsOnDate(uint yr = 9999, Byte seas = 99)
        {
            List<JournalEvent> matchingEvents = new List<JournalEvent>();

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
                    foreach (JournalEvent jEvent in this.events)
                    {
                        // year and season must match
                        if (jEvent.year == yr)
                        {
                            if (jEvent.season == seas)
                            {
                                matchingEvents.Add(jEvent);
                            }
                        }
                    }
                    break;
                case "yr":
                    foreach (JournalEvent jEvent in this.events)
                    {
                        // year must match
                        if (jEvent.year == yr)
                        {
                            matchingEvents.Add(jEvent);
                        }
                    }
                    break;
                default:
                    foreach (JournalEvent jEvent in this.events)
                    {
                        // get all events
                        matchingEvents.Add(jEvent);
                    }
                    break;
            }

            return matchingEvents;
        }

    }

    /// <summary>
    /// Class containing details of game event
    /// </summary>
    public class JournalEvent
    {
        /// <summary>
        /// Holds event year
        /// </summary>
        public uint year { get; set; }
        /// <summary>
        /// Holds event season
        /// </summary>
        public byte season { get; set; }
        /// <summary>
        /// Holds main persons (charIDs) associated with event
        /// </summary>
        public String personae { get; set; }
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
        /// Constructor for JournalEvent
        /// </summary>
        /// <param name="yr">uint holding event year</param>
        /// <param name="seas">byte holding event season</param>
        /// <param name="pers">String holding main persons (charIDs) associated with event</param>
        /// <param name="typ">String holding type of event (e.g. battle, birth)</param>
        /// <param name="loc">String holding location of event (fiefID)</param>
        /// <param name="descr">String holding description of event</param>
        public JournalEvent(uint yr, byte seas, String pers, String typ, String loc = null, String descr = null)
        {
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

    }
}
