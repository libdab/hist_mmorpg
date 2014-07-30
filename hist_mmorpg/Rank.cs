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
        public String rankID { get; set; }
        /// <summary>
        /// Holds title in various languages.
        /// Tuple Item1: Language code
        /// Tuple Item2: corresponding title
        /// </summary>
        public Tuple<String, String>[] title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte stature { get; set; }

        /// <summary>
        /// Constructor for Rank
        /// </summary>
        /// <param name="id">String holding rank ID</param>
        /// <param name="ti">Tuple holding title in various languages</param>
        /// <param name="stat">byte holding base stature for rank</param>
        public Rank(String id, Tuple<String, String>[] ti, byte stat)
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

            this.rankID = id;
            this.title = ti;
            this.stature = stat;

        }

        /// <summary>
        /// Constructor for Rank taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Rank()
        {
        }

    }
}
