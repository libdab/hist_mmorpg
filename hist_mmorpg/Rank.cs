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
        /// Constructor for Rank taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Rank()
        {
        }

    }

    /// <summary>
    /// Struct storing data on title name
    /// </summary>
    public struct TitleName
    {
        /// <summary>
        /// Holds Language ID
        /// </summary>
        public string langID;
        /// <summary>
        /// Holds title name associated with specific language
        /// </summary>
        public string name;

        public TitleName(string lang, string nam)
        {
            this.langID = lang;
            this.name = nam;
        }
    }
}
