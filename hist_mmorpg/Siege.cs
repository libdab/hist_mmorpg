using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on a siege
    /// </summary>
    public class Siege
    {
        /// <summary>
        /// Holds siege ID
        /// </summary>
        public String siegeID { get; set; }
        /// <summary>
        /// Holds year the siege started
        /// </summary>
        public uint startYear { get; set; }
        /// <summary>
        /// Holds season the siege started
        /// </summary>
        public byte startSeason { get; set; }
        /// <summary>
        /// Holds besieging player (charID)
        /// </summary>
        public String besiegingPlayer { get; set; }
        /// <summary>
        /// Holds defending player (charID)
        /// </summary>
        public String defendingPlayer { get; set; }
        /// <summary>
        /// Holds besieging army (armyID)
        /// </summary>
        public String besiegerArmy { get; set; }
        /// <summary>
        /// Holds defending garrison (armyID)
        /// </summary>
        public String defenderGarrison { get; set; }
        /// <summary>
        /// Holds fief being besieged (fiefID)
        /// </summary>
        public String besiegedFief { get; set; }
        /// <summary>
        /// Holds days left in current season
        /// </summary>
        public double days { get; set; }
        /// <summary>
        /// Holds the keep level at the start of the siege
        /// </summary>
        public double startKeepLevel { get; set; }
        /// <summary>
        /// Holds total casualties suffered so far by attacker
        /// </summary>
        public int totalCasualtiesAttacker { get; set; }
        /// <summary>
        /// Holds total casualties suffered so far by defender
        /// </summary>
        public int totalCasualtiesDefender { get; set; }
        /// <summary>
        /// Holds total duration of siege so far (days)
        /// </summary>
        public double totalDays { get; set; }
        /// <summary>
        /// Holds additional defending army (armyID)
        /// </summary>
        public String defenderAdditional { get; set; }
        /// <summary>
        /// Holds season and year the siege ended
        /// </summary>
        public String endDate { get; set; }

        /// <summary>
        /// Constructor for Siege
        /// </summary>
		/// <param name="id">String holding ID of siege</param>
        /// <param name="startYr">uint holding year the siege started</param>
        /// <param name="startSeas">byte holding season the siege started</param>
        /// <param name="bsgPlayer">String holding besieging player (charID)</param>
        /// <param name="defPlayer">String holding defending player (charID)</param>
        /// <param name="bsgArmy">String holding besieging army (armyID)</param>
        /// <param name="defGarr">String holding defending garrison (armyID)</param>
        /// <param name="fief">String holding fief being besieged (fiefID)</param>
        /// <param name="day">double containing days left in current season</param>
        /// <param name="kpLvl">double containing the keep level at the start of the siege</param>
        /// <param name="totAtt">int containing total attacker casualties so far</param>
        /// <param name="totDef">int containing total defender casualties so far</param>
        /// <param name="totday">double containing days used by siege so far</param>
        /// <param name="defAdd">String holding additional defending army (armyID)</param>
        /// <param name="end">string holding season and year the siege ended</param>
        public Siege(String id, uint startYr, byte startSeas, string bsgPlayer, string defPlayer, string bsgArmy,
            string defGarr, string fief, double day, double kpLvl, int totAtt = 0, int totDef = 0, double totDay = 0,
            string defAdd = null, string end = null)
        {
            // VALIDATION

            // ID
            // trim and ensure 1st is uppercase
            id = Globals_Game.firstCharToUpper(id.Trim());

            if (!Globals_Game.validateSiegeID(id))
            {
                throw new InvalidDataException("Siege ID must have the format 'Siege_' followed by some numbers");
            }

            // STARTSEAS
            if (!Globals_Game.validateSeason(startSeas))
            {
                throw new InvalidDataException("Siege startSeason must be a byte between 0-3");
            }

            // BSGPLAYER
            // trim and ensure 1st is uppercase
            bsgPlayer = Globals_Game.firstCharToUpper(bsgPlayer.Trim());

            if (!Globals_Game.validateCharacterID(bsgPlayer))
            {
                throw new InvalidDataException("Siege besiegingPlayer id must have the format 'Char_' followed by some numbers");
            }

            // DEFPLAYER
            // trim and ensure 1st is uppercase
            defPlayer = Globals_Game.firstCharToUpper(defPlayer.Trim());

            if (!Globals_Game.validateCharacterID(defPlayer))
            {
                throw new InvalidDataException("Siege defendingPlayer id must have the format 'Char_' followed by some numbers");
            }

            // BSGARMY
            // trim and ensure 1st is uppercase
            bsgArmy = Globals_Game.firstCharToUpper(bsgArmy.Trim());

            if (!Globals_Game.validateArmyID(bsgArmy))
            {
                throw new InvalidDataException("Siege besiegingArmy id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
            }

            // DEFGARR
            // trim and ensure 1st is uppercase
            defGarr = Globals_Game.firstCharToUpper(defGarr.Trim());

            if (!Globals_Game.validateArmyID(defGarr))
            {
                throw new InvalidDataException("Siege defendingGarrison id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
            }

            // FIEF

            // DAY

            // KPLVL

            // TOTATT

            // TOTDEF

            // TOTDAY

            // DEFADD
            if (!String.IsNullOrWhiteSpace(defAdd))
            {
                // trim and ensure 1st is uppercase
                defAdd = Globals_Game.firstCharToUpper(defAdd.Trim());

                if (!Globals_Game.validateArmyID(defAdd))
                {
                    throw new InvalidDataException("Siege defenderAdditonal id must have the format 'Army_' or 'GarrisonArmy_' followed by some numbers");
                }
            }

            this.siegeID = id;
            this.startYear = startYr;
            this.startSeason = startSeas;
            this.besiegingPlayer = bsgPlayer;
            this.defendingPlayer = defPlayer;
            this.besiegerArmy = bsgArmy;
            this.defenderGarrison = defGarr;
            this.besiegedFief = fief;
            this.days = day;
            this.startKeepLevel = kpLvl;
            this.totalCasualtiesAttacker = totAtt;
            this.totalCasualtiesDefender = totDef;
            this.totalDays = totDay;
            this.defenderAdditional = defAdd;
            this.endDate = end;
        }

        /// <summary>
        /// Constructor for Siege taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Siege()
		{
		}
		
        /// <summary>
        /// Gets the fief being besieged
        /// </summary>
        /// <returns>The besieged fief</returns>
        public Fief getFief()
        {
            Fief besiegedFief = null;

            if (!String.IsNullOrWhiteSpace(this.besiegedFief))
            {
                if (Globals_Game.fiefMasterList.ContainsKey(this.besiegedFief))
                {
                    besiegedFief = Globals_Game.fiefMasterList[this.besiegedFief];
                }
            }

            return besiegedFief;
        }

        /// <summary>
        /// Gets the besieging army
        /// </summary>
        /// <returns>The besieging army</returns>
        public Army getBesiegingArmy()
        {
            Army besieger = null;

            if (!String.IsNullOrWhiteSpace(this.besiegerArmy))
            {
                if (Globals_Game.armyMasterList.ContainsKey(this.besiegerArmy))
                {
                    besieger = Globals_Game.armyMasterList[this.besiegerArmy];
                }
            }

            return besieger;
        }

        /// <summary>
        /// Gets the defending garrison
        /// </summary>
        /// <returns>The defending garrison (Army)</returns>
        public Army getDefenderGarrison()
        {
            Army defenderGarrison = null;

            if (!String.IsNullOrWhiteSpace(this.defenderGarrison))
            {
                if (Globals_Game.armyMasterList.ContainsKey(this.defenderGarrison))
                {
                    defenderGarrison = Globals_Game.armyMasterList[this.defenderGarrison];
                }
            }

            return defenderGarrison;
        }

        /// <summary>
        /// Gets the additional defending army
        /// </summary>
        /// <returns>The additional defending army</returns>
        public Army getDefenderAdditional()
        {
            Army thisDefenderAdditional = null;

            if (!String.IsNullOrWhiteSpace(this.defenderAdditional))
            {
                if (Globals_Game.armyMasterList.ContainsKey(this.defenderAdditional))
                {
                    thisDefenderAdditional = Globals_Game.armyMasterList[this.defenderAdditional];
                }
            }

            return thisDefenderAdditional;
        }

        /// <summary>
        /// Gets the defending player
        /// </summary>
        /// <returns>The defending player</returns>
        public PlayerCharacter getDefendingPlayer()
        {
            PlayerCharacter defendingPlyr = null;

            if (!String.IsNullOrWhiteSpace(this.defendingPlayer))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.defendingPlayer))
                {
                    defendingPlyr = Globals_Game.pcMasterList[this.defendingPlayer];
                }
            }

            return defendingPlyr;
        }

        /// <summary>
        /// Gets the besieging player
        /// </summary>
        /// <returns>The besieging player</returns>
        public PlayerCharacter getBesiegingPlayer()
        {
            PlayerCharacter besiegingPlyr = null;

            if (!String.IsNullOrWhiteSpace(this.besiegingPlayer))
            {
                if (Globals_Game.pcMasterList.ContainsKey(this.besiegingPlayer))
                {
                    besiegingPlyr = Globals_Game.pcMasterList[this.besiegingPlayer];
                }
            }

            return besiegingPlyr;
        }

        /// <summary>
        /// Synchronises days for component objects
        /// </summary>
        /// <param name="newDays">double indicating new value for days</param>
        /// <param name="checkForAttrition">bool indicating whether to check for attrition</param>
        public void syncDays(double newDays, bool checkForAttrition = true)
        {
            Army besieger = this.getBesiegingArmy();
            Army defenderGarr = this.getDefenderGarrison();
            Army defenderAdd = this.getDefenderAdditional();
            bool defenderAttritonApplies = false;
            byte attritionChecks = 0;
            double difference = 0;

            // check to see if attrition checks are required
            // NOTE: no check required for seasonal update
            if (checkForAttrition)
            {
                // if the siege has had to 'wait' for some days
                if (this.days > newDays)
                {
                    // get number of days difference
                    difference = this.days - newDays;

                    // work out number of attrition checks needed
                    attritionChecks = Convert.ToByte(difference / 7);

                    // check if attrition has kicked in for defending forces
                    defenderAttritonApplies = this.checkAttritionApplies();
                }

            }

            // adjust siege days to specified days
            this.days = newDays;

            // ATTACKING ARMY
            Character attackerLeader = besieger.getLeader();
            if (attackerLeader != null)
            {
                attackerLeader.adjustDays(attackerLeader.days - this.days);
            }
            else
            {
                besieger.days = this.days;
            }

            // check for attrition if required
            if (attritionChecks > 0)
            {
                uint totalAttackTroopsLost = 0;
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = besieger.calcAttrition();
                    // apply attrition
                    totalAttackTroopsLost += besieger.applyTroopLosses(attritionModifer);
                }
                // update total attacker siege losses
                this.totalCasualtiesAttacker += Convert.ToInt32(totalAttackTroopsLost);
            }

            // DEFENDING GARRISON
            Character garrisonLeader = defenderGarr.getLeader();
            if (garrisonLeader != null)
            {
                garrisonLeader.adjustDays(garrisonLeader.days - this.days);
            }
            else
            {
                defenderGarr.days = this.days;
            }

            // check for attrition if required
            if (defenderAttritonApplies)
            {
                if (attritionChecks > 0)
                {
                    uint totalDefendTroopsLost = 0;
                    for (int i = 0; i < attritionChecks; i++)
                    {
                        // calculate attrition
                        double attritionModifer = defenderGarr.calcAttrition();
                        // apply attrition
                        totalDefendTroopsLost += defenderGarr.applyTroopLosses(attritionModifer);
                    }
                    // update total defender siege losses
                    this.totalCasualtiesDefender += Convert.ToInt32(totalDefendTroopsLost);
                }
            }

            // ADDITIONAL DEFENDING ARMY
            if (defenderAdd != null)
            {
                Character defAddLeader = defenderAdd.getLeader();
                if (defAddLeader != null)
                {
                    defAddLeader.adjustDays(defAddLeader.days - this.days);
                }
                else
                {
                    defenderAdd.days = this.days;
                }

                // check for attrition if required
                if (defenderAttritonApplies)
                {
                    if (attritionChecks > 0)
                    {
                        uint totalDefendTroopsLost = 0;
                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = defenderAdd.calcAttrition();
                            // apply attrition
                            totalDefendTroopsLost += defenderAdd.applyTroopLosses(attritionModifer);
                        }
                        // update total defender siege losses
                        this.totalCasualtiesDefender += Convert.ToInt32(totalDefendTroopsLost);
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if attrition applies to the defending forces (based on bailiff management rating)
        /// </summary>
        /// <returns>bool indicating whether attrition applies</returns>
        public bool checkAttritionApplies()
        {
            bool attritionApplies = false;
            Character thisBailiff = this.getFief().bailiff;
            double bailiffManagement = 0;

            // get bailiff's management rating
            if (thisBailiff != null)
            {
                bailiffManagement = thisBailiff.management;
            }
            else
            {
                bailiffManagement = 4;
            }

            // check to see if attrition needs to be applied
            if ((this.totalDays / 60) > bailiffManagement)
            {
                attritionApplies = true;
            }

            return attritionApplies;
        }

        /// <summary>
        /// Updates siege at the end/beginning of the season
        /// </summary>
        /// <returns>bool indicating whether the siege has been dismantled</returns>
        public bool updateSiege()
        {
            bool siegeEnded = false;
            Character besiegerLeader = this.getBesiegingArmy().getLeader();

            // check if besieger still in field (i.e. has not been disbanded)
            if (String.IsNullOrWhiteSpace(this.besiegerArmy))
            {
                siegeEnded = true;
            }

            else
            {
                // DAYS
                // base allowance
                double newDays = 90;

                if (besiegerLeader != null)
                {
                    // set days to besieger leader's days (may be effected by skills)
                    newDays = besiegerLeader.days;
                }

                // synchronise days of all component objects
                this.syncDays(newDays, false);
            }

            return siegeEnded;
        }

        /// <summary>
        /// Ends the siege
        /// </summary>
        /// <param name="s">String containing circumstances under which the siege ended</param>
        public void siegeEnd(string circumstance)
        {
            // get principle objects
            PlayerCharacter defendingPlayer = this.getDefendingPlayer();
            Army defenderGarrison = this.getDefenderGarrison();
            Character defenderLeader = defenderGarrison.getLeader();
            PlayerCharacter besiegingPlayer = this.getBesiegingPlayer();
            Army defenderAdditional = this.getDefenderAdditional();
            Character addDefendLeader = null;
            if (defenderAdditional != null)
            {
                addDefendLeader = defenderAdditional.getLeader();
            }
            Fief besiegedFief = this.getFief();
            Character besiegingArmyLeader = this.getBesiegingArmy().getLeader();

            // =================== construct and send JOURNAL ENTRY
            // ID
            uint entryID = Globals_Game.getNextJournalEntryID();

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add("all|all");
            tempPersonae.Add(defendingPlayer.charID + "|fiefOwner");
            tempPersonae.Add(besiegingPlayer.charID + "|attackerOwner");
            tempPersonae.Add(besiegingArmyLeader.charID + "|attackerLeader");
            // get defenderLeader
            if (defenderLeader != null)
            {
                tempPersonae.Add(defenderLeader.charID + "|defenderGarrisonLeader");
            }
            // get additional defending leader
            if (addDefendLeader != null)
            {
                tempPersonae.Add(addDefendLeader.charID + "|defenderAdditionalLeader");
            }
            string[] siegePersonae = tempPersonae.ToArray();

            // location
            string siegeLocation = besiegedFief.id;

            // description
            string siegeDescription = "";
            if (circumstance == null)
            {
                siegeDescription = "On this day of Our Lord the forces of ";
                siegeDescription += besiegingPlayer.firstName + " " + besiegingPlayer.familyName;
                siegeDescription += " abandoned the siege of " + besiegedFief.name;
                siegeDescription += ". The ownership of this fief is retained by ";
                siegeDescription += defendingPlayer.firstName + " " + defendingPlayer.familyName + ".";
            }
            else
            {
                siegeDescription = circumstance;
            }

            // put together new journal entry
            JournalEntry siegeResult = new JournalEntry(entryID, Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, siegePersonae, "siegeEnd", loc: siegeLocation, descr: siegeDescription);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(siegeResult);

            // disband garrison
            defenderGarrison.disbandArmy();
            defenderGarrison = null;

            // disband additional defending army
            if (defenderAdditional != null)
            {
                defenderAdditional.disbandArmy();
                defenderAdditional = null;
            }

            // remove from PCs
            besiegingPlayer.mySieges.Remove(this.siegeID);
            defendingPlayer.mySieges.Remove(this.siegeID);

            // remove from fief
            besiegedFief.siege = null;

            // sync days of all effected objects (to remove influence of attacking leader's skills)
            // work out proportion of seasonal allowance remaining
            double daysProportion = 0;
            if (besiegingArmyLeader != null)
            {
                daysProportion = this.days / besiegingArmyLeader.getDaysAllowance();
            }
            else
            {
                daysProportion = this.days / 90;
            }

            // iterate through characters in fief keep
            foreach (Character thisChar in besiegedFief.charactersInFief)
            {
                if (thisChar.inKeep)
                {
                    // reset character's days to reflect days spent in siege
                    thisChar.days = thisChar.getDaysAllowance() * daysProportion;
                }
            }

            // remove from master list
            Globals_Game.siegeMasterList.Remove(this.siegeID);
        }


    }
}
