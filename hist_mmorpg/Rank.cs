using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on rank and title
    /// </summary>
    public class Rank
    {
        /// <summary>
        /// Holds rank ID
        /// </summary>
        public byte id { get; set; }
        /// <summary>
        /// Holds title name in various languages
        /// </summary>
        public TitleName[] title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte stature { get; set; }

        /// <summary>
        /// Constructor for Rank
        /// </summary>
        /// <param name="id">byte holding rank ID</param>
        /// <param name="ti">TitleName[] holding title name in various languages</param>
        /// <param name="stat">byte holding base stature for rank</param>
        public Rank(byte id, TitleName[] ti, byte stat)
        {

            // TODO: validation

            // validate stature
            if (stat < 1)
            {
                stat = 1;
            }
            else if (stat > 6)
            {
                stat = 6;
            }

            this.id = id;
            this.title = ti;
            this.stature = stat;

        }

        /// <summary>
        /// Constructor for Rank using Position_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
        /// <param name="pr">Position_Riak object to use as source</param>
        public Rank(Position_Riak pr)
        {
            this.id = pr.id;
            this.title = pr.title;
            this.stature = pr.stature;
        }

        /// <summary>
        /// Constructor for Rank taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Rank()
        {
        }

        /// <summary>
        /// Gets the correct name for the rank depending on the specified Language
        /// </summary>
        /// <returns>string containing the name</returns>
        /// <param name="l">The Language to be used</param>
        public string getName(Language l)
        {
            string rankName = null;
            bool nameFound = false;

            // iterate through TitleNames and get correct name
            foreach (TitleName titleName in this.title)
            {
                if (titleName.langID == l.id)
                {
                    rankName = titleName.name;
                    nameFound = true;
                    break;
                }
            }

            // if no name found for specified language
            if (!nameFound)
            {
                // iterate through TitleNames and get generic name
                foreach (TitleName titleName in this.title)
                {
                    if ((titleName.langID.Equals("generic")) || (titleName.langID.Contains("lang_E")))
                    {
                        rankName = titleName.name;
                        nameFound = true;
                        break;
                    }
                }
            }

            // if still no name found
            if (!nameFound)
            {
                // get first name
                rankName = this.title[0].name;
            }

            return rankName;
        }

    }

    /// <summary>
    /// Class storing data on positions of power
    /// </summary>
    public class Position : Rank
    {
        /// <summary>
        /// Holds ID of the office holder
        /// </summary>
        public string officeHolder { get; set; }
        /// <summary>
        /// Holds nationality associated with the position
        /// </summary>
        public Nationality nationality { get; set; }

        /// <summary>
        /// Constructor for Position
        /// </summary>
        /// <param name="holder">string holding ID of the office holder</param>
        /// <param name="nat">Nationality associated with the position</param>
        public Position(byte id, TitleName[] ti, byte stat, string holder, Nationality nat)
            : base(id, ti, stat)
        {
            this.officeHolder = holder;
            this.nationality = nat;
        }

        /// <summary>
        /// Constructor for Position using Position_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
        /// <param name="pr">Position_Riak object to use as source</param>
        public Position(Position_Riak pr)
            : base(pr: pr)
        {
            this.officeHolder = pr.officeHolder;
            // nationality to be inserted later
            this.nationality = null;
        }

        /// <summary>
        /// Constructor for Position taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Position()
        {
        }

        /// <summary>
        /// Inserts the supplied PlayerCharacter's ID into the Position's officeHolder variable 
        /// </summary>
        /// <param name="newPositionHolder">PlayerCharacter being assigned to the Position</param>
        public void bestowPosition(PlayerCharacter newPositionHolder)
        {
            PlayerCharacter oldPositionHolder = null;

            // remove existing holder if necessary
            if (!String.IsNullOrWhiteSpace(this.officeHolder))
            {
                // get current holder
                if (Globals_Game.pcMasterList.ContainsKey(this.officeHolder))
                {
                    oldPositionHolder = Globals_Game.pcMasterList[this.officeHolder];
                }

                // remove from position
                this.removePosition(oldPositionHolder);
            }

            // assign position
            this.officeHolder = newPositionHolder.charID;

            // update stature
            newPositionHolder.adjustStatureModifier(this.stature);

            // CREATE JOURNAL ENTRY
            // get interested parties
            bool success = true;
            PlayerCharacter king = this.getKingdom().owner;

            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add("all|all");
            tempPersonae.Add(king.charID + "|king");
            tempPersonae.Add(newPositionHolder.charID + "|newPositionHolder");
            if (oldPositionHolder != null)
            {
                tempPersonae.Add(oldPositionHolder.charID + "|oldPositionHolder");
            }
            string[] thisPersonae = tempPersonae.ToArray();

            // type
            string type = "grantPosition";

            // description
            string description = "On this day of Our Lord the position of " + this.title[0].name;
            description += " was granted by His Majesty " + king.firstName + " " + king.familyName + " to ";
            description += newPositionHolder.firstName + " " + newPositionHolder.familyName;
            if (oldPositionHolder != null)
            {
                description += "; This has necessitated the removal of ";
                description += oldPositionHolder.firstName + " " + oldPositionHolder.familyName + " from the position";
            }
            description += ".";

            // create and add a journal entry to the pastEvents journal
            JournalEntry thisEntry = new JournalEntry(entryID, year, season, thisPersonae, type, descr: description);
            success = Globals_Game.addPastEvent(thisEntry);
        }

        /// <summary>
        /// Removes the supplied PlayerCharacter's ID from the Position's officeHolder variable 
        /// </summary>
        /// <param name="pc">PlayerCharacter being removed from the Position</param>
        public void removePosition(PlayerCharacter pc)
        {
            // remove from position
            this.officeHolder = null;

            // update stature
            pc.adjustStatureModifier(this.stature * -1);
        }

        /// <summary>
        /// Gets the Kingdom associated with the position 
        /// </summary>
        /// <returns>The Kingdom</returns>
        public Kingdom getKingdom()
        {
            Kingdom thisKingdom = null;

            foreach (KeyValuePair<string, Kingdom> kingdomEntry in Globals_Game.kingdomMasterList)
            {
                if (kingdomEntry.Value.nationality == this.nationality)
                {
                    thisKingdom = kingdomEntry.Value;
                    break;
                }
            }

            return thisKingdom;
        }
    }

    /// <summary>
    /// Class used to convert Position to/from format suitable for Riak (JSON)
    /// </summary>
    public class Position_Riak
    {
        /// <summary>
        /// Holds ID
        /// </summary>
        public byte id { get; set; }
        /// <summary>
        /// Holds title name in various languages
        /// </summary>
        public TitleName[] title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte stature { get; set; }
        /// <summary>
        /// Holds ID of the office holder
        /// </summary>
        public string officeHolder { get; set; }
        /// <summary>
        /// Holds ID of Nationality associated with the position
        /// </summary>
        public string nationality { get; set; }

        /// <summary>
        /// Constructor for Position_Riak
        /// </summary>
        /// <param name="pos">Position object to use as source</param>
        public Position_Riak(Position pos)
        {
            this.id = pos.id;
            this.title = pos.title;
            this.stature = pos.stature;
            this.officeHolder = pos.officeHolder;
            this.nationality = pos.nationality.natID;
        }

        /// <summary>
        /// Constructor for Position_Riak taking seperate values.
        /// For creating Position_Riak from CSV file.
        /// </summary>
        /// <param name="id">byte holding Position ID</param>
        /// <param name="ti">title name in various languages</param>
        /// <param name="stat">byte holding stature for this position</param>
        /// <param name="holder">string ID of the office holder</param>
        /// <param name="nat">string holding ID of Nationality associated with the position</param>
        public Position_Riak(byte id, TitleName[] ti, byte stat, string holder, string nat)
        {
            this.id = id;
            this.title = ti;
            this.stature = stat;
            this.officeHolder = holder;
            this.nationality = nat;
        }

        /// <summary>
        /// Constructor for Position_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Position_Riak()
        {
        }
    }

    /// <summary>
    /// Struct storing data on title name
    /// </summary>
    public struct TitleName
    {
        /// <summary>
        /// Holds Language ID or "generic"
        /// </summary>
        public string langID;
        /// <summary>
        /// Holds title name associated with specific language
        /// </summary>
        public string name;

        /// <summary>
        /// Constructor for TitleName
        /// </summary>
        /// <param name="lang">string holding Language ID</param>
        /// <param name="nam">string holding title name associated with specific language</param>
        public TitleName(string lang, string nam)
        {
            this.langID = lang;
            this.name = nam;
        }
    }
}
