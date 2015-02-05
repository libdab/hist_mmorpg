using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on a Skill
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
        public Dictionary<string, double> effects;

        /// <summary>
        /// Constructor for Skill
        /// </summary>
		/// <param name="id">String holding skill ID</param>
		/// <param name="nam">String holding skill name</param>
        /// <param name="effs">Dictionary(string, double) holding skill effects</param>
        public Skill(String id, String nam, Dictionary<string, double> effs)
        {
            // VALIDATION

            // ID
            // trim
            id = id.Trim();

            if (!Utility_Methods.ValidateSkillID(id))
            {
                throw new InvalidDataException("Skill ID must have the format 'skill_' followed by some numbers");
            }

            // NAM
            // trim and ensure 1st is uppercase
            nam = Utility_Methods.FirstCharToUpper(nam.Trim());

            if (!Utility_Methods.ValidateName(nam))
            {
                throw new InvalidDataException("Skill name must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
            }

            // EFFS
            // effect name
            string[] effNames = new string[effs.Count];
            effs.Keys.CopyTo(effNames, 0);

            for (int i = 0; i < effNames.Length; i++ )
            {
                if (!Utility_Methods.ValidateName(effNames[i]))
                {
                    throw new InvalidDataException("All Skill effect names must be 1-40 characters long and contain only valid characters (a-z and ') or spaces");
                }
            }

            // effect values
            double[] effVals = new double[effs.Count];
            effs.Values.CopyTo(effVals, 0);

            for (int i = 0; i < effVals.Length; i++)
            {
                if ((effVals[i] < -0.99) || (effVals[i] > 0.99))
                {
                    throw new InvalidDataException("All Skill effect values must be doubles between -0.99 and 0.99");
                }
            }

			this.skillID = id;
			this.name = nam;
            this.effects = effs;

        }

        /// <summary>
        /// Constructor for Skill taking no parameters.
        /// For use when de-serialising.
        /// </summary>
        public Skill()
		{
		}
    }
}
