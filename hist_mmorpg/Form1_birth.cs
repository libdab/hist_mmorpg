using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using QuickGraph;
using CorrugatedIron;
using CorrugatedIron.Models;

namespace hist_mmorpg
{
    /// <summary>
    /// Partial class for Form1, containing functionality specific to pregnancy and childbirth
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Generates a new NPC based on parents' statistics
        /// </summary>
        /// <returns>NonPlayerCharacter or null</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public NonPlayerCharacter GenerateNewNPC(NonPlayerCharacter mummy, Character daddy)
        {
            NonPlayerCharacter newNPC = new NonPlayerCharacter();

            // charID
            newNPC.charID = Globals_Game.GetNextCharID();
            // first name
            newNPC.firstName = "Baby";
            // family name
            newNPC.familyName = daddy.familyName;
            // date of birth
            newNPC.birthDate = new Tuple<uint, byte>(Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason);
            // sex
            newNPC.isMale = this.GenerateSex();
            // nationality
            newNPC.nationality = daddy.nationality;
            // whether is alive
            newNPC.isAlive = true;
            // maxHealth
            newNPC.maxHealth = this.GenerateKeyCharacteristics(mummy.maxHealth, daddy.maxHealth);
            // virility
            newNPC.virility = this.GenerateKeyCharacteristics(mummy.virility, daddy.virility);
            // goTo queue
            newNPC.goTo = new Queue<Fief>();
            // language
            newNPC.language = daddy.language;
            // days left
            newNPC.days = 90;
            // stature modifier
            newNPC.statureModifier = 0;
            // management
            newNPC.management = this.GenerateKeyCharacteristics(mummy.management, daddy.management);
            // combat
            newNPC.combat = this.GenerateKeyCharacteristics(mummy.combat, daddy.combat);
            // skills
            newNPC.skills = this.GenerateSkillSetFromParents(mummy.skills, daddy.skills, newNPC.isMale);
            // if in keep
            newNPC.inKeep = mummy.inKeep;
            // if pregnant
            newNPC.isPregnant = false;
            // familyID
            newNPC.familyID = daddy.familyID;
            // spouse
            newNPC.spouse = null;
            // father
            newNPC.father = daddy.charID;
            // mother
            newNPC.mother = mummy.charID;
            // fiancee
            newNPC.fiancee = null;
            // location
            newNPC.location = null;
            // titles
            newNPC.myTitles = new List<string>();
            // armyID
            newNPC.armyID = null;
            // ailments
            newNPC.ailments = new Dictionary<string, Ailment>();
            // employer
            newNPC.employer = null;
            // salary/allowance
            newNPC.salary = 0;
            // lastOffer (will remain empty for family members)
            newNPC.lastOffer = new Dictionary<string, uint>();
            // inEntourage
            newNPC.inEntourage = false;
            // isHeir
            newNPC.isHeir = false;

            return newNPC;
        }

        /// <summary>
        /// Generates a random sex for a Character
        /// </summary>
        /// <returns>bool indicating whether is male</returns>
        public bool GenerateSex()
        {
            bool isMale = false;

            // generate random (0-1) to see if male or female
            if (Globals_Game.myRand.Next(0, 2) == 0)
            {
                isMale = true;
            }

            return isMale;
        }

        /// <summary>
        /// Generates a characteristic stat for a Character, based on parent stats
        /// </summary>
        /// <returns>Double containing characteristic stat</returns>
        /// <param name="mummyStat">The mother's characteristic stat</param>
        /// <param name="daddyStat">The father's characteristic stat</param>
        public Double GenerateKeyCharacteristics(Double mummyStat, Double daddyStat)
        {
            Double newStat = 0;

            // get average of parents' stats
            Double parentalAverage = (mummyStat + daddyStat) / 2;

            // generate random (0 - 100) to determine relationship of new stat to parentalAverage
            double randPercentage = Utility_Methods.GetRandomDouble(100);

            // calculate new stat
            if (randPercentage <= 35)
            {
                newStat = parentalAverage;
            }
            else if (randPercentage <= 52.5)
            {
                newStat = parentalAverage - 1;
            }
            else if (randPercentage <= 70)
            {
                newStat = parentalAverage + 1;
            }
            else if (randPercentage <= 80)
            {
                newStat = parentalAverage - 2;
            }
            else if (randPercentage <= 90)
            {
                newStat = parentalAverage + 2;
            }
            else if (randPercentage <= 95)
            {
                newStat = parentalAverage - 3;
            }
            else
            {
                newStat = parentalAverage + 3;
            }

            // make sure new stat falls within acceptable range
            if (newStat < 1)
            {
                newStat = 1;
            }
            else if (newStat > 9)
            {
                newStat = 9;
            }

            return newStat;
        }

        /// <summary>
        /// Generates a skill set for a Character, based on parent skills
        /// </summary>
        /// <returns>Array containing skill set</returns>
        /// <param name="mummySkills">The mother's skills</param>
        /// <param name="daddySkills">The father's skills</param>
        /// <param name="isMale">Whether character is a male</param>
        public Tuple<Skill, int>[] GenerateSkillSetFromParents(Tuple<Skill, int>[] mummySkills, Tuple<Skill, int>[] daddySkills, bool isMale)
        {
            // store all unique skillKeys from both parents
            List<string> uniqueSkillKeys = new List<string>();

            // mummy's skills
            for (int i = 0; i < mummySkills.Length; i++)
            {
                uniqueSkillKeys.Add(mummySkills[i].Item1.skillID);
            }

            // daddy's skills
            for (int i = 0; i < daddySkills.Length; i++)
            {
                if (!uniqueSkillKeys.Contains(daddySkills[i].Item1.skillID))
                {
                    uniqueSkillKeys.Add(daddySkills[i].Item1.skillID);
                }
            }

            // create new skills using uniqueSkillKeys
            Tuple<Skill, int>[] newSkills = Utility_Methods.GenerateSkillSet(uniqueSkillKeys);

            return newSkills;
        }

        /// <summary>
        /// Performs childbirth procedure
        /// </summary>
        /// <returns>Boolean indicating character death occurrence</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public void GiveBirth(NonPlayerCharacter mummy, Character daddy)
        {
            string description = "";

            // get head of family
            PlayerCharacter thisHeadOfFamily = daddy.GetHeadOfFamily();

            // generate new NPC (baby)
            NonPlayerCharacter weeBairn = this.GenerateNewNPC(mummy, daddy);

            // check for baby being stillborn
            bool isStillborn = weeBairn.CheckForDeath(true, false, false);

            if (!isStillborn)
            {
                // add baby to npcMasterList
                Globals_Game.npcMasterList.Add(weeBairn.charID, weeBairn);

                // set baby's location
                weeBairn.location = mummy.location;
                weeBairn.location.charactersInFief.Add(weeBairn);

                // add baby to family
                Globals_Client.myPlayerCharacter.myNPCs.Add(weeBairn);
            }
            else
            {
                weeBairn.isAlive = false;
            }

            // check for mother dying during childbirth
            bool mummyDied = mummy.CheckForDeath(true, true, isStillborn);

            // construct and send JOURNAL ENTRY

            // personae
            string[] childbirthPersonae = new string[] { thisHeadOfFamily.charID + "|headOfFamily", mummy.charID + "|mother", daddy.charID + "|father", weeBairn.charID + "|child" };

            // description
            description += "On this day of Our Lord " + mummy.firstName + " " + mummy.familyName;
            description += ", wife of " + daddy.firstName + " " + daddy.familyName + ", went into labour.";

            // mother and baby alive
            if ((!isStillborn) && (!mummyDied))
            {
                description += " Both the mother and her newborn ";
                if (weeBairn.isMale)
                {
                    description += "son";
                }
                else
                {
                    description += "daughter";
                }
                description += " are doing well and " + thisHeadOfFamily.firstName + " " + thisHeadOfFamily.familyName;
                description += " is delighted to welcome a new member into his family.";
            }

            // baby OK, mother dead
            if ((!isStillborn) && (mummyDied))
            {
                description += " The baby ";
                if (weeBairn.isMale)
                {
                    description += "boy";
                }
                else
                {
                    description += "girl";
                }
                description += " is doing well but sadly the mother died during childbirth. ";
                description += thisHeadOfFamily.firstName + " " + thisHeadOfFamily.familyName;
                description += " welcomes the new member into his family.";
            }

            // mother OK, baby dead
            if ((isStillborn) && (!mummyDied))
            {
                description += " The mother is doing well but sadly her newborn ";
                if (weeBairn.isMale)
                {
                    description += "son";
                }
                else
                {
                    description += "daughter";
                }
                description += " died during childbirth.";
            }

            // both mother and baby died
            if ((isStillborn) && (mummyDied))
            {
                description += " Tragically, both the mother and her newborn ";
                if (weeBairn.isMale)
                {
                    description += "son";
                }
                else
                {
                    description += "daughter";
                }
                description += " died of complications during the childbirth.";
            }

            // put together new journal entry
            JournalEntry childbirth = new JournalEntry(Globals_Game.GetNextJournalEntryID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, childbirthPersonae, "birth", descr: description);

            // add new journal entry to pastEvents
            Globals_Game.AddPastEvent(childbirth);

            // if appropriate, process mother's death
            if (mummyDied)
            {
                mummy.ProcessDeath("childbirth");
            }


            // display message
            if (Globals_Client.showMessages)
            {
                System.Windows.Forms.MessageBox.Show(description);
            }

            this.RefreshHouseholdDisplay();
        }

        /// <summary>
        /// Performs standard conditional checks before a pregnancy attempt
        /// </summary>
        /// <returns>bool indicating whether or not to proceed with pregnancy attempt</returns>
        /// <param name="husband">The husband</param>
        public bool ChecksBeforePregnancyAttempt(Character husband)
        {
            bool proceed = true;

            // check is married
            // get spouse
            Character wife = husband.GetSpouse();

            if (wife != null)
            {
                // check to make sure is in same fief
                if (!(wife.location == husband.location))
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You have to be in the same fief to do that!");
                    }
                    proceed = false;
                }

                else
                {
                    // make sure wife not already pregnant
                    if (wife.isPregnant)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(wife.firstName + " " + wife.familyName + " is already pregnant, milord.  Don't be so impatient!", "PREGNANCY ATTEMPT CANCELLED");
                        }
                        proceed = false;
                    }

                    // check if are kept apart by siege
                    else
                    {
                        if ((!String.IsNullOrWhiteSpace(husband.location.siege)) && (husband.inKeep != wife.inKeep))
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("I'm afraid the husband and wife are being separated by the ongoing siege.", "PREGNANCY ATTEMPT CANCELLED");
                            }
                            proceed = false;
                        }

                        else
                        {
                            // ensure player and spouse have at least 1 day remaining
                            double minDays = Math.Min(husband.days, wife.days);

                            if (minDays < 1)
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("Sorry, you don't have enough time left for this in the current season.", "PREGNANCY ATTEMPT CANCELLED");
                                }
                                proceed = false;
                            }
                            else
                            {
                                // ensure days are synchronised
                                if (husband.days != wife.days)
                                {
                                    if (husband.days != minDays)
                                    {
                                        if (husband is PlayerCharacter)
                                        {
                                            (husband as PlayerCharacter).AdjustDays(husband.days - minDays);
                                        }
                                        else
                                        {
                                            husband.AdjustDays(husband.days - minDays);
                                        }
                                    }
                                    else
                                    {
                                        wife.AdjustDays(wife.days - minDays);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                if (Globals_Client.showMessages)
                {
                    string whoThisIs = "";
                    if (husband == Globals_Client.myPlayerCharacter)
                    {
                        whoThisIs = "You are ";
                    }
                    else
                    {
                        whoThisIs = "This man is ";
                    }

                    System.Windows.Forms.MessageBox.Show(whoThisIs + "not married, my lord.", "PREGNANCY ATTEMPT CANCELLED");
                }
                proceed = false;
            }

            return proceed;
        }
    }
}
