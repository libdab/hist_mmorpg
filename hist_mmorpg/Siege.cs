﻿using System;
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
        /// Synchronises days for relevant objects associated with the siege
        /// </summary>
        public void syncDays()
        {
            // attacking army
            Character attackerLeader = this.getBesieger().getLeader();
            attackerLeader.adjustDays(attackerLeader.days - this.days);

            // defending garrison army
            Character garrisonLeader = this.getDefenderGarrison().getLeader();
            if (garrisonLeader != null)
            {
                garrisonLeader.adjustDays(garrisonLeader.days - this.days);
            }
            else
            {
                this.getDefenderGarrison().days = this.days;
            }

            // additional defending army
            Character defenderLeader = this.getDefenderAdditional().getLeader();
            defenderLeader.adjustDays(defenderLeader.days - this.days);
        }


    }
}
