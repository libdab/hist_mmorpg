using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace hist_mmorpg
{
    public static class Utility_Methods
    {
        /// <summary>
        /// Creates a JournalEntry for the attention of the game sysAdmin
        /// </summary>
        /// <returns>random double</returns>
        public static JournalEntry createSysAdminJentry()
        {
            JournalEntry jEntry = null;

            if (Globals_Game.sysAdmin != null)
            {
                // ID
                uint jEntryID = Globals_Game.getNextJournalEntryID();

                // date
                uint year = Globals_Game.clock.currentYear;
                byte season = Globals_Game.clock.currentSeason;

                // personae
                string sysAdminEntry = Globals_Game.sysAdmin.charID + "|sysAdmin";
                string[] jEntryPersonae = new string[] { sysAdminEntry };

                // description
                string description = "To be added";

                // create and send a proposal (journal entry)
                jEntry = new JournalEntry(jEntryID, year, season, jEntryPersonae, "CSV_importError", descr: description);
            }

            return jEntry;
        }

        /// <summary>
        /// Generates a random double, specifying maximum and (optional) minimum values
        /// </summary>
        /// <returns>random double</returns>
        /// <param name="max">maximum value</param>
        /// <param name="min">minimum value</param>
        public static double GetRandomDouble(double max, double min = 0)
        {
            return Globals_Game.myRand.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Checks whether the supplied integer is odd or even
        /// </summary>
        /// <returns>bool indicating whether odd</returns>
        /// <param name="value">Integer to be checked</param>
        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        /// <summary>
        /// Checks that a JournalEntry personae entry is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the personae entry is valid</returns>
        /// <param name="id">The personae entry to be validated</param>
        public static bool validateJentryPersonae(string personae)
        {
            bool isValid = true;

            // split using'|'
            string[] persSplit = personae.Split('|');
            if (persSplit.Length != 2)
            {
                isValid = false;
            }

            // 1st section must be valid character ID or 'all'
            else if ((!persSplit[0].Equals("all")) && (!Utility_Methods.validateCharacterID(persSplit[0])))
            {
                isValid = false;
            }

            // 2nd section must be all letters
            else if (!Utility_Methods.checkStringValid("letters", persSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that an OwnershipChallenge id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validateChallengeID(string id)
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'Challenge'
            else if (!idSplit[0].Equals("Challenge"))
            {
                isValid = false;
            }

            // must end with numbers
            else if (!Utility_Methods.checkStringValid("numbers", idSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that an Ailment id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validateAilmentID(string id)
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'Ail'
            else if (!idSplit[0].Equals("Ail"))
            {
                isValid = false;
            }

            // must end with numbers
            else if (!Utility_Methods.checkStringValid("numbers", idSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a Skill id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validateSkillID(string id)
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'skill'
            else if (!idSplit[0].Equals("skill"))
            {
                isValid = false;
            }

            // must end with numbers
            else if (!Utility_Methods.checkStringValid("numbers", idSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a days value is in the correct range
        /// </summary>
        /// <returns>bool indicating whether the value is valid</returns>
        /// <param name="stat">The value to be validated</param>
        public static bool validateDays(double days)
        {
            bool isValid = true;

            // check is between 0-109
            if ((days < 0) || (days > 109))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a character statistic (combat, management, stature, virility, maxHealth, skill level) is in the correct range
        /// </summary>
        /// <returns>bool indicating whether the statistic is valid</returns>
        /// <param name="stat">The statistic to be validated</param>
        /// <param name="lowerLimit">The lower limit for the statistic to be validated (optional)</param>
        public static bool validateCharacterStat(double stat, double lowerLimit = 1)
        {
            bool isValid = true;

            // check is between 1-9
            if ((stat < lowerLimit) || (stat > 9))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a season is in the correct range
        /// </summary>
        /// <returns>bool indicating whether the season is valid</returns>
        /// <param name="season">The season to be validated</param>
        public static bool validateSeason(byte season)
        {
            bool isValid = true;

            if ((season < 0) || (season > 3))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a Terrain id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validateTerrainID(string id)
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'terr'
            else if (!idSplit[0].Equals("terr"))
            {
                isValid = false;
            }

            // must end with letters
            else if (!Utility_Methods.checkStringValid("letters", idSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a Language id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        /// <param name="langType">The type of id to be validated (lang, baseLang)</param>
        public static bool validateLanguageID(string id, string langType = "lang")
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'lang'
            else if (!idSplit[0].ToLower().Equals("lang"))
            {
                isValid = false;
            }

            else if (langType.Equals("baseLang"))
            {
                // 2nd section must be letters
                if (!Utility_Methods.checkStringValid("letters", idSplit[1]))
                {
                    isValid = false;
                }
            }

            else
            {
                // 1st character of 2nd section must be letter
                if (!Utility_Methods.checkStringValid("letters", idSplit[1].Substring(0, 1)))
                {
                    isValid = false;
                }

                // last character of 2nd section must be number
                else if (!Utility_Methods.checkStringValid("numbers", idSplit[1].Substring(idSplit[1].Length - 1, 1)))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a Siege id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validateSiegeID(string id)
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'Siege'
            else if (!idSplit[0].Equals("Siege"))
            {
                isValid = false;
            }

            // must end with numbers
            else if (!Utility_Methods.checkStringValid("numbers", idSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that an Army id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validateArmyID(string id)
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'Army' or 'GarrisonArmy'
            else if ((!idSplit[0].Equals("Army")) && (!idSplit[0].Equals("GarrisonArmy")))
            {
                isValid = false;
            }

            // must end with numbers
            else if (!Utility_Methods.checkStringValid("numbers", idSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks a fief double property (keepLevel, industry, fields, loyalty, bailiffDaysInFief) is in the correct range
        /// </summary>
        /// <returns>bool indicating whether the double is valid</returns>
        /// <param name="input">The double to be validated</param>
        /// <param name="upperLimit">The upper limit of the double to be validated (optional)</param>
        public static bool validateFiefDouble(double input, double upperLimit = -1)
        {
            bool isValid = true;

            // check is >= 0
            if (input < 0)
            {
                isValid = false;
            }

            else if (upperLimit != -1)
            {
                if (input > upperLimit)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a nationality id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="nat">The id to be validated</param>
        public static bool validateNationalityID(string nat)
        {
            bool isValid = true;

            // 1-3 in length
            if ((nat.Length < 1) || (nat.Length > 3))
            {
                isValid = false;
            }

            // letters only
            if (!Utility_Methods.checkStringValid("letters", nat))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that taxrate is in the correct range
        /// </summary>
        /// <returns>bool indicating whether the taxrate is valid</returns>
        /// <param name="tx">The taxrate to be validated</param>
        public static bool validatePercentage(double tx)
        {
            bool isValid = true;

            if ((tx < 0) || (tx > 100))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a name is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the name is valid</returns>
        /// <param name="name">The name to be validated</param>
        public static bool validateName(string name)
        {
            bool isValid = true;

            // ensure is 1-30 in length
            if ((name.Length < 1) || (name.Length > 30))
            {
                isValid = false;
            }

            // ensure only contains correct characters
            else if (!(Regex.IsMatch(name, "^[a-zA-Z'\\s-]+$")))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a Place id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validatePlaceID(string id)
        {
            bool isValid = true;

            // ensure is 5 in length
            if (id.Length != 5)
            {
                isValid = false;
            }

            // ensure 1st is letter
            else if (!Utility_Methods.checkStringValid("letters", id.Substring(0, 1)))
            {
                isValid = false;
            }

            // ensure ends in 2 numbers
            else if (!Utility_Methods.checkStringValid("numbers", id.Substring(3)))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks that a Character id is in the correct format
        /// </summary>
        /// <returns>bool indicating whether the id is valid</returns>
        /// <param name="id">The id to be validated</param>
        public static bool validateCharacterID(string id)
        {
            bool isValid = true;

            // split and ensure has correct format
            string[] idSplit = id.Split('_');
            if (idSplit.Length != 2)
            {
                isValid = false;
            }

            // must start with 'Char'
            else if (!idSplit[0].Equals("Char"))
            {
                isValid = false;
            }

            // must end with numbers
            else if (!Utility_Methods.checkStringValid("numbers", idSplit[1]))
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Checks to see if a string meets the specified conditions (all letters, all numbers)
        /// </summary>
        /// <returns>bool indicating whether the string fulfils the conditions</returns>
        /// <param name="matchType">Type of pattern to match (letters, numbers)</param>
        /// <param name="input">string to be converted</param>
        public static bool checkStringValid(string matchType, string input)
        {
            switch (matchType)
            {
                case "letters":
                    return Regex.IsMatch(input, @"^[a-zA-Z]+$");
                case "numbers":
                    int myNumber;
                    return int.TryParse(input, out myNumber);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts the first letter of a string to uppercase
        /// </summary>
        /// <returns>Converted string</returns>
        /// <param name="input">string to be converted</param>
        public static string firstCharToUpper(string input)
        {
            string output = "";

            if (!String.IsNullOrEmpty(input))
            {
                input = input.First().ToString().ToUpper() + input.Substring(1);
            }

            output = input;

            return output;
        }

        /// <summary>
        /// Generates a random skill set for a Character
        /// </summary>
        /// <returns>Tuple<Skill, int>[] for use with a Character object</returns>
        public static Tuple<Skill, int>[] generateSkillSet()
        {

            // create array of skills between 2-3 in length
            Tuple<Skill, int>[] skillSet = new Tuple<Skill, int>[Globals_Game.myRand.Next(2, 4)];

            // populate array of skills with randomly chosen skills
            // 1) make temporary copy of skillKeys
            List<string> skillKeysCopy = new List<string>(Globals_Game.skillKeys);

            // 2) choose random skill, and assign random skill level
            for (int i = 0; i < skillSet.Length; i++)
            {
                // choose random skill
                int randSkill = Globals_Game.myRand.Next(0, skillKeysCopy.Count - 1);

                // assign random skill level
                int randSkillLevel = Globals_Game.myRand.Next(1, 10);

                // create Skill tuple
                skillSet[i] = new Tuple<Skill, int>(Globals_Game.skillMasterList[skillKeysCopy[randSkill]], randSkillLevel);

                // remove skill from skillKeysCopy to ensure isn't chosen again
                skillKeysCopy.RemoveAt(randSkill);
            }

            return skillSet;

        }

    }
}
