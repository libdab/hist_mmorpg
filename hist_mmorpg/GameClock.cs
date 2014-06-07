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
        public string[] seasons = new string[4] { "Spring", "Summer", "Autumn", "Winter" };
        /// <summary>
        /// Holds current year
        /// </summary>
        public uint currentYear { get; set; }
        /// <summary>
        /// Holds current season
        /// </summary>
        public uint currentSeason { get; set; }

        /// <summary>
        /// Constructor for GameClock
        /// </summary>
        /// <param name="yr">uint holding starting year</param>
        /// <param name="s">int holding current season (default: 0)</param>
        public GameClock(uint yr, uint s = 0)
        {
            this.currentYear = yr;
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
