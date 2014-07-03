using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{

    /// <summary>
    /// Class storing data on skill
    /// </summary>
    public class Skill
    {

		/// <summary>
		/// Holds skill ID
		/// </summary>
		public String skillID { get; set; }
        /// <summary>
        /// Holds skill name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds skill effects
        /// </summary>
        public Dictionary<string, int> effects;

        /// <summary>
        /// Constructor for Skill
        /// </summary>
		/// <param name="id">String holding skill ID</param>
		/// <param name="nam">String holding skill name</param>
        /// <param name="effs">Dictionary<string name, int effect> holding skill effects</param>
		public Skill(String id, String nam, Dictionary<string, int> effs)
        {

            // validate nam length = 1-20
            if ((nam.Length < 1) || (nam.Length > 20))
            {
                throw new InvalidDataException("Skill name must be between 1 and 20 characters in length");
            }

			this.skillID = id;
			this.name = nam;
            this.effects = effs;

        }

		public Skill()
		{
		}
    }
}
