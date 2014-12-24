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
        /// Responds to the click event of the familyGetSpousePregBt button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyGetSpousePregBtn_Click(object sender, EventArgs e)
        {
            // get spouse
            Character mySpouse = Globals_Client.myPlayerCharacter.getSpouse();

            // perform standard checks
            if (this.checksBeforePregnancyAttempt(Globals_Client.myPlayerCharacter))
            {
                // ensure are both in/out of keep
                mySpouse.inKeep = Globals_Client.myPlayerCharacter.inKeep;

                // attempt pregnancy
                bool pregnant = Globals_Client.myPlayerCharacter.getSpousePregnant(mySpouse);
            }

            // refresh screen
            this.refreshCurrentScreen();

            /*
            // test event scheduled in clock
            List<JournalEntry> myEvents = new List<JournalEntry>();
            myEvents = Globals_Client.clock.scheduledEvents.getEventsOnDate();
            if (myEvents.Count > 0)
            {
                foreach (JournalEntry jEvent in myEvents)
                {
                    System.Windows.Forms.MessageBox.Show("Year: " + jEvent.year + " | Season: " + jEvent.season + " | Who: " + jEvent.personae + " | What: " + jEvent.type);
                }
            } */
        }

        /// <summary>
        /// Generates a new NPC based on parents' statistics
        /// </summary>
        /// <returns>NonPlayerCharacter or null</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public NonPlayerCharacter generateNewNPC(NonPlayerCharacter mummy, Character daddy)
        {
            NonPlayerCharacter newNPC = new NonPlayerCharacter();

            // charID
            newNPC.charID = Globals_Game.getNextCharID();
            // first name
            newNPC.firstName = "Baby";
            // family name
            newNPC.familyName = daddy.familyName;
            // date of birth
            newNPC.birthDate = new Tuple<uint, byte>(Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason);
            // sex
            newNPC.isMale = this.generateSex();
            // nationality
            newNPC.nationality = daddy.nationality;
            // whether is alive
            newNPC.isAlive = true;
            // maxHealth
            newNPC.maxHealth = this.generateKeyCharacteristics(mummy.maxHealth, daddy.maxHealth);
            // virility
            newNPC.virility = this.generateKeyCharacteristics(mummy.virility, daddy.virility);
            // goTo queue
            newNPC.goTo = new Queue<Fief>();
            // language
            newNPC.language = daddy.language;
            // days left
            newNPC.days = 90;
            // stature modifier
            newNPC.statureModifier = 0;
            // management
            newNPC.management = this.generateKeyCharacteristics(mummy.management, daddy.management);
            // combat
            newNPC.combat = this.generateKeyCharacteristics(mummy.combat, daddy.combat);
            // skills
            newNPC.skills = this.generateSkillSetFromParents(mummy.skills, daddy.skills, newNPC.isMale);
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
        public bool generateSex()
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
        public Double generateKeyCharacteristics(Double mummyStat, Double daddyStat)
        {
            Double newStat = 0;

            // get average of parents' stats
            Double parentalAverage = (mummyStat + daddyStat) / 2;

            // generate random (0 - 100) to determine relationship of new stat to parentalAverage
            double randPercentage = Globals_Game.GetRandomDouble(100);

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
        public Tuple<Skill, int>[] generateSkillSetFromParents(Tuple<Skill, int>[] mummySkills, Tuple<Skill, int>[] daddySkills, bool isMale)
        {
            // create a List to temporarily hold skills
            // will convert to array at end of method
            List<Tuple<Skill, int>> newSkillsList = new List<Tuple<Skill, int>>();

            // number of skills to return
            int numSkills = 0;

            // need to compare parent's skills to see how many match (could effect no. of child skills)
            int matchingSkills = 0;
            int totalSkillsAvail = 0;

            // iterate through parents' skills identifying matches
            for (int i = 0; i < mummySkills.Length; i++)
            {
                for (int ii = 0; ii < daddySkills.Length; ii++)
                {
                    if (mummySkills[i].Item1.skillID.Equals(daddySkills[ii].Item1.skillID))
                    {
                        matchingSkills++;
                    }
                }
            }

            // get total skill pool available from both parents
            totalSkillsAvail = (mummySkills.Length + daddySkills.Length) - matchingSkills;

            // if are only 2 skills in total, can only be 2 child skills
            if (totalSkillsAvail == 2)
            {
                numSkills = 2;
            }
            else
            {
                // generate random (2-3) to see how many skills child will have
                numSkills = Globals_Game.myRand.Next(2, 4);
            }

            // if are only 2 skills in parents' skill pool (i.e. both parents have same skills)
            // then use highest level skills (enhanced)
            if (totalSkillsAvail == 2)
            {
                for (int i = 0; i < mummySkills.Length; i++)
                {
                    for (int j = 0; j < daddySkills.Length; j++)
                    {
                        if (mummySkills[i].Item1.skillID.Equals(daddySkills[j].Item1.skillID))
                        {
                            // get highest of duplicate skills' level
                            int maxLevel = Math.Max(mummySkills[i].Item2, daddySkills[j].Item2);

                            // adjust the skill level upwards
                            int newSkillLevel = 0;
                            if (maxLevel > 6)
                            {
                                newSkillLevel = 9;
                            }
                            else
                            {
                                newSkillLevel = maxLevel + 2;
                            }

                            // creat new skill item
                            Tuple<Skill, int> mySkill = new Tuple<Skill, int>(mummySkills[i].Item1, newSkillLevel);

                            // add to temporary list
                            newSkillsList.Add(mySkill);

                            break;
                        }
                    }
                }
            }

            // if are more than 2 skills in parents' skill pool
            else
            {
                Tuple<Skill, int> mySkill;

                // decide which parent to use first (in case have to choose 2 skills from one parent)
                Tuple<Skill, int>[] firstSkillSet = null;
                Tuple<Skill, int>[] lastSkillSet = null;

                // use same sex parent first
                if (isMale)
                {
                    firstSkillSet = daddySkills;
                    lastSkillSet = mummySkills;
                }
                else
                {
                    firstSkillSet = mummySkills;
                    lastSkillSet = daddySkills;
                }

                // to hold chosen skill
                int chosenSkill = 0;
                // to hold previous chosen skill, to allow comparison
                int PrevChosenSkill = 0;

                // get a skill from the first parent
                chosenSkill = Globals_Game.myRand.Next(0, firstSkillSet.Length);

                // creat new skill item
                mySkill = new Tuple<Skill, int>(firstSkillSet[chosenSkill].Item1, firstSkillSet[chosenSkill].Item2);
                // add to temporary list
                newSkillsList.Add(mySkill);
                // record which skill was chosen in case comparison needed
                PrevChosenSkill = chosenSkill;

                // if child is to have 3 skills
                if (numSkills == 3)
                {
                    do
                    {
                        // get another skill from the first parent
                        chosenSkill = Globals_Game.myRand.Next(0, firstSkillSet.Length);

                        // creat new skill item
                        mySkill = new Tuple<Skill, int>(firstSkillSet[chosenSkill].Item1, firstSkillSet[chosenSkill].Item2);
                        // add to temporary list
                        newSkillsList.Add(mySkill);

                        // do chosen skill doesn't match the first
                    } while (chosenSkill == PrevChosenSkill);

                }

                // get a skill from the other parent
                chosenSkill = Globals_Game.myRand.Next(0, lastSkillSet.Length);

                // check to see if already have skill in newSkillsList
                bool duplicate = false;
                // to hold any duplicate skill items
                Tuple<Skill, int> duplicateItem = null;

                // iterate through existing skills list checking for duplicates
                foreach (Tuple<Skill, int> element in newSkillsList)
                {
                    if (lastSkillSet[chosenSkill].Item1.skillID.Equals(element.Item1.skillID))
                    {
                        duplicate = true;
                        // record duplicate skill item
                        duplicateItem = element;
                    }
                }

                // if the last chosen skill was a duplicate
                if (duplicate)
                {
                    // get highest of duplicate skills' level
                    int maxLevel = Math.Max(duplicateItem.Item2, lastSkillSet[chosenSkill].Item2);

                    // adjust the skill level upwards
                    int newSkillLevel = 0;
                    if (maxLevel > 6)
                    {
                        newSkillLevel = 9;
                    }
                    else
                    {
                        newSkillLevel = maxLevel + 2;
                    }

                    // remove the duplicate item from the list
                    newSkillsList.Remove(duplicateItem);

                    // create a new skill item with enhanced skill level
                    mySkill = new Tuple<Skill, int>(duplicateItem.Item1, newSkillLevel);
                }

                // if the last chosen skill was not a duplicate
                else
                {
                    // copy chosen skill into new skill item
                    mySkill = new Tuple<Skill, int>(lastSkillSet[chosenSkill].Item1, lastSkillSet[chosenSkill].Item2);
                }

                // add to temporary list
                newSkillsList.Add(mySkill);
            }

            // create new skills array from temporary list
            Tuple<Skill, int>[] newSkills = newSkillsList.ToArray();

            return newSkills;
        }

        /// <summary>
        /// Performs childbirth procedure
        /// </summary>
        /// <returns>Boolean indicating character death occurrence</returns>
        /// <param name="mummy">The new NPC's mother</param>
        /// <param name="daddy">The new NPC's father</param>
        public void giveBirth(NonPlayerCharacter mummy, Character daddy)
        {
            string description = "";

            // get head of family
            PlayerCharacter thisHeadOfFamily = daddy.getHeadOfFamily();

            // generate new NPC (baby)
            NonPlayerCharacter weeBairn = this.generateNewNPC(mummy, daddy);

            // check for baby being stillborn
            bool isStillborn = weeBairn.checkForDeath(true, false, false);

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
            bool mummyDied = mummy.checkForDeath(true, true, isStillborn);

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
            JournalEntry childbirth = new JournalEntry(Globals_Game.getNextJournalEntryID(), Globals_Game.clock.currentYear, Globals_Game.clock.currentSeason, childbirthPersonae, "birth", descr: description);

            // add new journal entry to pastEvents
            Globals_Game.addPastEvent(childbirth);

            // if appropriate, process mother's death
            if (mummyDied)
            {
                mummy.processDeath("childbirth");
            }


            // display message
            if (Globals_Client.showMessages)
            {
                System.Windows.Forms.MessageBox.Show(description);
            }

            this.refreshHouseholdDisplay();
        }

        /// <summary>
        /// Performs standard conditional checks before a pregnancy attempt
        /// </summary>
        /// <returns>bool indicating whether or not to proceed with pregnancy attempt</returns>
        /// <param name="husband">The husband</param>
        public bool checksBeforePregnancyAttempt(Character husband)
        {
            bool proceed = true;

            // check is married
            if (!String.IsNullOrWhiteSpace(husband.spouse))
            {
                // get spouse
                Character wife = husband.getSpouse();

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
                                if (husband.days != minDays)
                                {
                                    if (husband is PlayerCharacter)
                                    {
                                        (husband as PlayerCharacter).adjustDays(husband.days - minDays);
                                    }
                                    else
                                    {
                                        husband.adjustDays(husband.days - minDays);
                                    }
                                }
                                else
                                {
                                    wife.adjustDays(wife.days - minDays);
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
                    System.Windows.Forms.MessageBox.Show("This man is not married.", "PREGNANCY ATTEMPT CANCELLED");
                }
                proceed = false;
            }

            return proceed;
        }

        /// <summary>
        /// Responds to the click event of the familyNpcSpousePregBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyNpcSpousePregBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get spouse
                Character mySpouse = Globals_Client.charToView.getSpouse();

                // perform standard checks
                if (this.checksBeforePregnancyAttempt(Globals_Client.charToView))
                {
                    // ensure are both in/out of keep
                    mySpouse.inKeep = Globals_Client.charToView.inKeep;

                    // attempt pregnancy
                    bool pregnant = Globals_Client.charToView.getSpousePregnant(mySpouse);
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No character selected!");
                }
            }
        }
    }
}
