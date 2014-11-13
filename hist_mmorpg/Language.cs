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
        public String id { get; set; }
        /// <summary>
        /// Holds base language
        /// </summary>
        public BaseLanguage baseLanguage { get; set; }
        /// <summary>
        /// Holds language dialect code
        /// </summary>
        public int dialect { get; set; }

        /// <summary>
        /// Constructor for Language
        /// </summary>
        /// <param name="bLang">BaseLanguage for the language</param>
        /// <param name="dial">int holding language dialect code</param>
        public Language(BaseLanguage bLang, int dial)
        {
            this.baseLanguage = bLang;
            this.dialect = dial;
            this.id = this.baseLanguage.id + this.dialect;
        }

        /// <summary>
        /// Constructor for Language taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Language()
        {
        }

        /// <summary>
        /// Constructor for Language using Language_Riak.
        /// For use when de-serialising from Riak
        /// </summary>
        /// <param name="lr">Language_Riak object to use as source</param>
        public Language(Language_Riak lr)
        {
            this.id = lr.id;
            this.dialect = lr.dialect;
            // baseLanguage to be inserted later
            this.baseLanguage = null;
        }

        /// <summary>
        /// Gets the name of the language
        /// </summary>
        /// <returns>string containing the name</returns>
        public string getName()
        {
            return this.baseLanguage.name + " (dialect " + this.dialect + ")";
        }
    }

    /// <summary>
    /// Class storing base langauge data
    /// </summary>
    public class BaseLanguage
    {
        /// <summary>
        /// Holds base langauge ID
        /// </summary>
        public String id { get; set; }
        /// <summary>
        /// Holds base language name
        /// </summary>
        public String name { get; set; }

        /// <summary>
        /// Constructor for BaseLanguage
        /// </summary>
        /// <param name="id">String holding language ID</param>
        /// <param name="nam">String holding language name</param>
        public BaseLanguage(String id, String nam)
        {
            // validate nam length = 1-20
            if ((nam.Length < 1) || (nam.Length > 20))
            {
                throw new InvalidDataException("BaseLanguage name must be between 1 and 20 characters in length");
            }

            this.id = id;
            this.name = nam;
        }

        /// <summary>
        /// Constructor for BaseLanguage taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public BaseLanguage()
        {
        }
    }

    /// <summary>
    /// Class used to convert Language to/from format suitable for Riak (JSON)
    /// </summary>
    public class Language_Riak
    {
        /// <summary>
        /// Holds language ID
        /// </summary>
        public String id { get; set; }
        /// <summary>
        /// Holds base language
        /// </summary>
        public string baseLanguage { get; set; }
        /// <summary>
        /// Holds language dialect code
        /// </summary>
        public int dialect { get; set; }

        /// <summary>
        /// Constructor for Language_Riak
        /// </summary>
        /// <param name="pc">PlayerCharacter object to use as source</param>
        public Language_Riak(Language l)
        {
            this.id = l.id;
            this.dialect = l.dialect;
            this.baseLanguage = l.baseLanguage.id;
        }

        /// <summary>
        /// Constructor for Language_Riak taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Language_Riak()
        {
        }
    }
}
