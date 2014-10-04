using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// Holds season and year the siege started
        /// </summary>
        public String startDate { get; set; }
        /// <summary>
        /// Holds besieging army (armyID)
        /// </summary>
        public String besieger { get; set; }
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
        /// <param name="start">string holding season and year the siege started</param>
        /// <param name="bsgr">String holding besieging army (armyID)</param>
        /// <param name="defGarr">String holding defending garrison (armyID)</param>
        /// <param name="fief">String holding fief being besieged (fiefID)</param>
        /// <param name="day">double containing days left in current season</param>
        /// <param name="kpLvl">double containing the keep level at the start of the siege</param>
        /// <param name="totAtt">int containing total attacker casualties so far</param>
        /// <param name="totDef">int containing total defender casualties so far</param>
        /// <param name="totday">double containing days used by siege so far</param>
        /// <param name="defAdd">String holding additional defending army (armyID)</param>
        /// <param name="end">string holding season and year the siege ended</param>
        public Siege(String id, string start, string bsgr, string defGarr, string fief, double day, double kpLvl, int totAtt = 0, int totDef = 0, double totDay = 0, string defAdd = null, string end = null)
        {
            this.siegeID = id;
            this.startDate = start;
            this.besieger = bsgr;
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
        /// For use when de-serialising from Riak
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
            return Globals_Server.fiefMasterList[this.besiegedFief];
        }

        /// <summary>
        /// Gets the besieging army
        /// </summary>
        /// <returns>The besieging army</returns>
        public Army getBesieger()
        {
            return Globals_Server.armyMasterList[this.besieger];
        }

        /// <summary>
        /// Gets the defending garrison
        /// </summary>
        /// <returns>The defending garrison (Army)</returns>
        public Army getDefenderGarrison()
        {
            return Globals_Server.armyMasterList[this.defenderGarrison];
        }

        /// <summary>
        /// Gets the additional defending army
        /// </summary>
        /// <returns>The additional defending army</returns>
        public Army getDefenderAdditional()
        {
            Army thisDefenderAdditional = null;

            if (this.defenderAdditional != null)
            {
                thisDefenderAdditional = Globals_Server.armyMasterList[this.defenderAdditional];
            }

            return thisDefenderAdditional;
        }

        /// <summary>
        /// Synchronises days for component objects
        /// </summary>
        /// <param name="newDays">double indicating new value for days</param>
        public void syncDays(double newDays = 0)
        {
            Army besieger = this.getBesieger();
            Army defenderGarr = this.getDefenderGarrison();
            Army defenderAdd = this.getDefenderAdditional();
            bool defenderAttritonApplies = false;
            byte attritionChecks = 0;
            double difference = 0;

            // check to see if attrition checks are required
            if (newDays > 0)
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

                // adjust siege days to specified days
                this.days = newDays;
            }

            // ATTACKING ARMY
            Character attackerLeader = besieger.getLeader();
            if (attackerLeader != null)
            {
                // check to see if attackerLeader has more than 90 days (due to skills)
                if ((attackerLeader.days > 90) && (newDays == 0))
                {
                    this.days = attackerLeader.days;
                }

                attackerLeader.adjustDays(attackerLeader.days - this.days);
            }
            else
            {
                besieger.days = this.days;
            }

            // check for attrition if required
            if (attritionChecks > 0)
            {
                for (int i = 0; i < attritionChecks; i++)
                {
                    // calculate attrition
                    double attritionModifer = besieger.calcAttrition();
                    // apply attrition
                    besieger.applyTroopLosses(attritionModifer);
                }
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
                    for (int i = 0; i < attritionChecks; i++)
                    {
                        // calculate attrition
                        double attritionModifer = defenderGarr.calcAttrition();
                        // apply attrition
                        defenderGarr.applyTroopLosses(attritionModifer);
                    }
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
                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = defenderAdd.calcAttrition();
                            // apply attrition
                            defenderAdd.applyTroopLosses(attritionModifer);
                        }
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
            Character bailiff = this.getFief().bailiff;
            double bailiffManagement = 0;

            // get bailiff's management rating
            if (bailiff != null)
            {
                bailiffManagement = bailiff.management;
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

            // check if besieger still in field (i.e. has not been disbanded)
            if (this.besieger == null)
            {
                siegeEnded = true;
            }

            if (!siegeEnded)
            {
                // update days (any remaining days = no activity)
                this.days = 90;

                // synchronise days of all component objects
                this.syncDays();
            }

            return siegeEnded;
        }


    }
}
