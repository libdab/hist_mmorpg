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
        /// For use when de-serialising from Riak
        /// </summary>
        public Kingdom()
		{
		}

		/// <summary>
        /// Constructor for Kingdom using Kingdom_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
        /// <param name="kr">Kingdom_Riak object to use as source</param>
        public Kingdom(Kingdom_Riak kr)
			: base(kr: kr)
		{
            // nationality to be inserted later
            this.nationality = null;
        }
    }

    /// <summary>
    /// Class converting kingdom data into format suitable for Riak (JSON) storage
    /// </summary>
    public class Kingdom_Riak : Place_Riak
    {
        /// <summary>
        /// Holds Kingdom_Riak nationality (ID)
        /// </summary>
        public String nationality { get; set; }

		/// <summary>
        /// Constructor for Kingdom_Riak.
        /// For use when serialising to Riak
        /// </summary>
        /// <param name="king">Kingdom object to be used as source</param>
        public Kingdom_Riak(Kingdom king)
            : base(k: king)
		{
            this.nationality = king.nationality.natID;
		}

        /// <summary>
        /// Constructor for Kingdom_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Kingdom_Riak()
		{
		}
    }

}
