using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on language
    /// </summary>
    public class Language
    {

        /// <summary>
        /// Holds language ID
        /// </summary>
        public String languageID { get; set; }
        /// <summary>
        /// Holds language name
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// Constructor for Language
        /// </summary>
        /// <param name="id">String holding language ID</param>
        /// <param name="nam">String holding language name</param>
        public Language(String id, String nam)
        {

            // TODO: validate id = string B,C,D,E,F,G,H,I,L/1-3

            // validate nam length = 1-20
            if ((nam.Length < 1) || (nam.Length > 20))
            {
                throw new InvalidDataException("Language name must be between 1 and 20 characters in length");
            }

            this.languageID = id;
            this.name = nam;

        }

        /// <summary>
        /// Constructor for Language taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Language()
        {
        }
    }
}
