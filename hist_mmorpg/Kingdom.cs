using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on kingdom
    /// </summary>
    public class Kingdom : Place
    {
        /// <summary>
        /// Holds Kingdom nationality
        /// </summary>
        public Nationality nationality { get; set; }

        /// <summary>
        /// Constructor for Kingdom
        /// </summary>
        /// <param name="nat">Kingdom's Nationality object</param>
        public Kingdom(String id, String nam, Nationality nat, String tiHo = null, PlayerCharacter own = null, Rank r = null)
        : base(id, nam, tiHo, own, r)
        {
            this.nationality = nat;
        }

        /// <summary>
        /// Constructor for Kingdom taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Kingdom()
		{
		}

		/// <summary>
        /// Constructor for Kingdom using Kingdom_Serialised object.
        /// For use when de-serialising.
        /// </summary>
        /// <param name="ks">Kingdom_Serialised object to use as source</param>
        public Kingdom(Kingdom_Serialised ks)
			: base(ks: ks)
		{
            // nationality to be inserted later
            this.nationality = null;
        }

        /// <summary>
        /// Processes functions involved in lodging a new ownership (and kingship) challenge
        /// </summary>
        public void lodgeOwnershipChallenge()
        {
            bool proceed = true;

            // ensure aren't current owner
            if (Globals_Client.myPlayerCharacter == this.owner)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("You are already the King of " + this.name + "!");
                }
            }

            else
            {
                // create and send new OwnershipChallenge
                OwnershipChallenge newChallenge = new OwnershipChallenge(Globals_Game.getNextOwnChallengeID(), Globals_Client.myPlayerCharacter.charID, "kingdom", this.id);
                proceed = Globals_Game.addOwnershipChallenge(newChallenge);
            }

            if (proceed)
            {
                // create and send journal entry
                // get interested parties
                PlayerCharacter currentOwner = this.owner;

                // ID
                uint entryID = Globals_Game.getNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // location
                string entryLoc = this.id;

                // journal entry personae
                string allEntry = "all|all";
                string currentOwnerEntry = currentOwner.charID + "|king";
                string challengerEntry = Globals_Client.myPlayerCharacter.charID + "|pretender";
                string[] entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                // entry type
                string entryType = "depose_new";

                // journal entry description
                string description = "On this day of Our Lord a challenge for the crown of " + this.name + " (" + this.id + ")";
                description += " has COMMENCED.  " + Globals_Client.myPlayerCharacter.firstName + " " + Globals_Client.myPlayerCharacter.familyName + " seeks to press his claim ";
                description += "and depose the current king, His Highness " + currentOwner.firstName + " " + currentOwner.familyName + ", King of " + this.name + ".";

                // create and send a proposal (journal entry)
                JournalEntry myEntry = new JournalEntry(entryID, year, season, entryPersonae, entryType, descr: description, loc: entryLoc);
                Globals_Game.addPastEvent(myEntry);
            }
        }

        /// <summary>
        /// Transfers ownership of the kingdom (and the kingship) to the specified PlayerCharacter
        /// </summary>
        /// <param name="newOwner">The new owner</param>
        public void transferOwnership(PlayerCharacter newOwner)
        {
            // get current title holder
            Character titleHolder = this.getTitleHolder();

            // remove from current title holder's titles
            titleHolder.myTitles.Remove(this.id);

            // add to newOwner's titles
            newOwner.myTitles.Add(this.id);

            // update kingdom titleHolder property
            this.titleHolder = newOwner.charID;

            // update Globals_Game king variable
            if (Globals_Game.kingOne == this.owner)
            {
                Globals_Game.kingOne = newOwner;
            }
            else if (Globals_Game.kingTwo == this.owner)
            {
                Globals_Game.kingTwo = newOwner;
            }

            // update kingdom owner property
            this.owner = newOwner;
        }
    }

    /// <summary>
    /// Class converting kingdom data into serialised format (JSON)
    /// </summary>
    public class Kingdom_Serialised : Place_Serialised
    {
        /// <summary>
        /// Holds nationality (ID)
        /// </summary>
        public String nationality { get; set; }

		/// <summary>
        /// Constructor for Kingdom_Serialised.
        /// For use when serialising.
        /// </summary>
        /// <param name="king">Kingdom object to be used as source</param>
        public Kingdom_Serialised(Kingdom king)
            : base(k: king)
		{
            this.nationality = king.nationality.natID;
		}

        /// <summary>
        /// Constructor for Kingdom_Serialised taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Kingdom_Serialised()
		{
		}

        /// <summary>
        /// Constructor for Kingdom_Serialised taking seperate values.
        /// For creating Kingdom_Serialised from CSV file.
        /// </summary>
        /// <param name="nat">Kingdom's Nationality object</param>
        public Kingdom_Serialised(String id, String nam, byte r, string nat, String tiHo = null, string own = null)
            : base(id, nam, r, own: own, tiHo: tiHo)
        {
            // VALIDATION
            bool isValid = true;

            // NAT
            // trim and ensure 1st is uppercase
            nat = Globals_Game.firstCharToUpper(nat.Trim());

            isValid = Globals_Game.validateNationalityID(nat);
            if (!isValid)
            {
                throw new InvalidDataException("Kingdom_Serialised nationality ID must be 1-3 characters long, and consist entirely of letters");
            }

            this.nationality = nat;
        }
    }

}
