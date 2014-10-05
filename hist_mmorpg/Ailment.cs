using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on ailments effecting character health
    /// </summary>
    public class Ailment
    {
        /// <summary>
        /// Holds ailment ID
        /// </summary>
        public String ailmentID { get; set; }
        /// <summary>
        /// Holds ailment description
        /// </summary>
        public String description { get; set; }
        /// <summary>
        /// Holds ailment date
        /// </summary>
        public string when { get; set; }
        /// <summary>
        /// Holds current ailment effect
        /// </summary>
        public uint effect { get; set; }
        /// <summary>
        /// Holds minimum ailment effect
        /// </summary>
        public uint minimumEffect { get; set; }

        /// <summary>
        /// Constructor for Ailment
        /// </summary>
        /// <param name="id">String holding ailment ID</param>
        /// <param name="descr">string holding ailment description</param>
        /// <param name="wh">string holding ailment date</param>
        /// <param name="eff">uint holding current ailment effect</param>
        /// <param name="minEff">uint holding minimum ailment effect</param>
        public Ailment(String id, string descr, string wh, uint eff, uint minEff)
        {
            this.ailmentID = id;
            this.description = descr;
            this.when = wh;
            this.effect = eff;
            this.minimumEffect = minEff;
        }

        /// <summary>
        /// Updates the ailment, reducing effect where approprite
        /// </summary>
        /// <returns>bool indicating whether ailment should be deleted</returns>
        public bool updateAilment()
        {
            bool deleteAilment = false;

            // reduce effect, if appropriate
            if (effect > minimumEffect)
            {
                effect--;
            }

            // remove effect if has reached 0
            if (effect == 0)
            {
                deleteAilment = true;
            }

            return deleteAilment;
        }
    }
}
