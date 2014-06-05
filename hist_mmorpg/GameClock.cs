using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing season data
    /// </summary>
    public class GameClock
    {
        /// <summary>
        /// Holds seasons
        /// </summary>
        public string[] seasons = new string[4];
        /// <summary>
        /// Holds current season
        /// </summary>
        public int currentSeason { get; set; }

        public GameClock(int s = 0)
        {
            this.currentSeason = s;
        }

        /// <summary>
        /// Calculates travel modifier for season
        /// </summary>
        /// <returns>double containing travel modifier</returns>
        public double calcSeasonTravMod()
        {
            double travelModifier = 0;

            switch (this.currentSeason)
            {
                case 0:
                    travelModifier = 1.5;
                    break;
                case 3:
                    travelModifier = 2;
                    break;
                default:
                    travelModifier = 1;
                    break;
            }

            return travelModifier;
        }

        public void advanceSeason()
        {
            if (this.currentSeason == 3)
            {
                this.currentSeason = 0;
            }
            else
            {
                this.currentSeason++;
            }
        }
    }
}
