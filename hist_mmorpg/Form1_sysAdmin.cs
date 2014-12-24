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
    /// Partial class for Form1, containing functionality specific to the position of sysAdmin
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Responds to the click event of the adminEditCancelBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditCancelBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // save specified object
            switch (objectType)
            {
                case "PC":
                    this.refreshCharEdit();
                    break;

                case "NPC":
                    this.refreshCharEdit();
                    break;

                case "Fief":
                    this.refreshPlaceEdit();
                    break;

                case "Province":
                    this.refreshPlaceEdit();
                    break;

                case "Kingdom":
                    this.refreshPlaceEdit();
                    break;

                case "Skill":
                    this.refreshSkillEdit();
                    break;

                case "Army":
                    this.refreshArmyEdit();
                    break;

                default:
                    break;
            }

            // clear ID box
            this.adminEditTextBox.Clear();
        }

        /// <summary>
        /// Creates UI display for list of skill effects in the edit skill screen
        /// </summary>
        public void setUpEditSkillEffectList()
        {
            // add necessary columns
            this.adminEditSkillEffsListView.Columns.Add("Effect Name", -2, HorizontalAlignment.Left);
            this.adminEditSkillEffsListView.Columns.Add("Level", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Responds to the click event of any of the 'edit object' MenuItems
        /// displaying the appropriate screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditObjectMenuItem_Click(object sender, EventArgs e)
        {
            // get MenuItem
            ToolStripMenuItem thisItem = (sender as ToolStripMenuItem);
            string objectType = thisItem.Tag.ToString();

            // display edit object screen
            Globals_Client.containerToView = this.adminEditContainer;
            Globals_Client.containerToView.BringToFront();

            // set get/save button tag to identify object type (for retrieving and saving object)
            this.adminEditGetBtn.Tag = objectType;
            this.adminEditSaveBtn.Tag = objectType;
            this.adminEditCancelBtn.Tag = objectType;

            // clear previous object ID from TextBox
            this.adminEditTextBox.Text = "";

            // change admin edit control properties to match object type
            this.adminEditGetBtn.Text = "Get " + objectType;
            this.adminEditSaveBtn.Text = "Save " + objectType;
            this.adminEditLabel.Text = objectType + " ID";

            // display appropriate panel
            switch (objectType)
            {
                case "PC":
                    // clear previous data
                    this.refreshCharEdit();
                    this.adminEditCharIDTextBox.ReadOnly = true;
                    // display edit character panel
                    this.adminEditCharContainer.BringToFront();
                    // display edit pc panel
                    this.adminEditCharPcPanel.BringToFront();
                    break;
                case "NPC":
                    // clear previous data
                    this.refreshCharEdit();
                    this.adminEditCharIDTextBox.ReadOnly = true;
                    // display edit character panel
                    this.adminEditCharContainer.BringToFront();
                    // display edit npc panel
                    this.adminEditCharNpcPanel.BringToFront();
                    break;
                case "Fief":
                    // clear previous data
                    this.refreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit fief panel
                    this.adminEditFiefPanel.BringToFront();
                    break;
                case "Province":
                    // clear previous data
                    this.refreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit province panel
                    this.adminEditProvPanel.BringToFront();
                    break;
                case "Kingdom":
                    // clear previous data
                    this.refreshPlaceEdit();
                    this.adminEditPlaceIdTextBox.ReadOnly = true;
                    // display edit place panel
                    this.adminEditPlaceContainer.BringToFront();
                    // display edit kingdom panel
                    this.adminEditKingPanel.BringToFront();
                    break;
                case "Skill":
                    // clear previous data
                    this.refreshSkillEdit();
                    this.adminEditSkillIdTextBox.ReadOnly = true;
                    // display edit skill panel
                    this.adminEditSkillPanel.BringToFront();
                    break;
                case "Army":
                    // clear previous data
                    this.refreshArmyEdit();
                    this.adminEditArmyIdTextBox.ReadOnly = true;
                    // display edit army panel
                    this.adminEditArmyPanel.BringToFront();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Responds to the click event of any of the adminEditGetBtn button
        /// retrieving the specified object
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditGetBtn_Click(object sender, EventArgs e)
        {
            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // get specified object
            switch (objectType)
            {
                case "PC":
                    // get PC
                    PlayerCharacter thisPC = null;
                    if (Globals_Game.pcMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisPC = Globals_Game.pcMasterList[this.adminEditTextBox.Text];
                    }

                    // display PC details
                    if (thisPC != null)
                    {
                        this.refreshCharEdit(thisPC);
                    }
                    break;
                case "NPC":
                    // get NPC
                    NonPlayerCharacter thisNPC = null;
                    if (Globals_Game.npcMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisNPC = Globals_Game.npcMasterList[this.adminEditTextBox.Text];
                    }

                    // display NPC details
                    if (thisNPC != null)
                    {
                        this.refreshCharEdit(thisNPC);
                    }
                    break;

                case "Fief":
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisFief = Globals_Game.fiefMasterList[this.adminEditTextBox.Text];
                    }

                    // display fief details
                    if (thisFief != null)
                    {
                        this.refreshPlaceEdit(thisFief);
                    }
                    break;

                case "Province":
                    // get province
                    Province thisProv = null;
                    if (Globals_Game.provinceMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisProv = Globals_Game.provinceMasterList[this.adminEditTextBox.Text];
                    }

                    // display province details
                    if (thisProv != null)
                    {
                        this.refreshPlaceEdit(thisProv);
                    }
                    break;

                case "Kingdom":
                    // get kingdom
                    Kingdom thiskingdom = null;
                    if (Globals_Game.kingdomMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thiskingdom = Globals_Game.kingdomMasterList[this.adminEditTextBox.Text];
                    }

                    // display kingdom details
                    if (thiskingdom != null)
                    {
                        this.refreshPlaceEdit(thiskingdom);
                    }
                    break;

                case "Skill":
                    // get skill
                    Skill thisSkill = null;
                    if (Globals_Game.skillMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisSkill = Globals_Game.skillMasterList[this.adminEditTextBox.Text];
                    }

                    // display skill details
                    if (thisSkill != null)
                    {
                        this.refreshSkillEdit(thisSkill);
                    }
                    break;

                case "Army":
                    // get army
                    Army thisArmy = null;
                    if (Globals_Game.armyMasterList.ContainsKey(this.adminEditTextBox.Text))
                    {
                        thisArmy = Globals_Game.armyMasterList[this.adminEditTextBox.Text];
                    }

                    // display army details
                    if (thisArmy != null)
                    {
                        this.refreshArmyEdit(thisArmy);
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Refreshes the edit Character display, displaying details of the specified Character
        /// and clearing any previously displayed data
        /// </summary>
        /// <param name="ch">The Character to be displayed</param>
        public void refreshCharEdit(Character ch = null)
        {
            // clear previous data
            this.disableControls(this.adminEditCharContainer.Panel1);
            this.disableControls(this.adminEditCharPcPanel);
            this.disableControls(this.adminEditCharNpcPanel);

            // display character data, if provided
            if (ch != null)
            {
                // id
                this.adminEditCharIDTextBox.Text = ch.charID;
                // firstname
                this.adminEditChar1stnameTextBox.Text = ch.firstName;
                // surname
                this.adminEditCharSurnameTextBox.Text = ch.familyName;
                // birth yr
                this.adminEditCharByearTextBox.Text = ch.birthDate.Item1.ToString();
                // birth season
                this.adminEditCharBseasTextBox.Text = ch.birthDate.Item2.ToString();
                // sex
                if (ch.isMale)
                {
                    this.adminEditCharSexCheckBox.Checked = true;
                }
                else
                {
                    this.adminEditCharSexCheckBox.Checked = false;
                }
                // nationality
                this.adminEditCharNatTextBox.Text = ch.nationality.natID;
                // language
                this.adminEditCharLangTextBox.Text = ch.language.id;
                // days
                this.adminEditCharDaysTextBox.Text = ch.days.ToString();
                // stature modifier
                this.adminEditCharStatTextBox.Text = ch.statureModifier.ToString();
                // maxHealth
                this.adminEditCharHeaTextBox.Text = ch.maxHealth.ToString();
                // virility
                this.adminEditCharVirTextBox.Text = ch.virility.ToString();
                // management
                this.admineditCharManTextBox.Text = ch.management.ToString();
                // combat
                this.admineditCharComTextBox.Text = ch.combat.ToString();
                // skills
                this.adminEditCharSkNameTextBox1.Text = ch.skills[0].Item1.skillID;
                this.adminEditCharSkLvlTextBox1.Text = ch.skills[0].Item2.ToString();
                this.adminEditCharSkNameTextBox2.Text = ch.skills[1].Item1.skillID;
                this.adminEditCharSkLvlTextBox2.Text = ch.skills[1].Item2.ToString();
                if (ch.skills.Length > 2)
                {
                    this.adminEditCharSkNameTextBox3.Text = ch.skills[2].Item1.skillID;
                    this.adminEditCharSkLvlTextBox3.Text = ch.skills[2].Item2.ToString();
                }
                // location
                this.adminEditCharLocTextBox.Text = ch.location.id;
                // inKeep
                if (ch.inKeep)
                {
                    this.adminEditCharInKpCheckBox.Checked = true;
                }
                else
                {
                    this.adminEditCharInKpCheckBox.Checked = false;
                }

                // PC data
                if (ch is PlayerCharacter)
                {
                    // playerID
                    if (!String.IsNullOrWhiteSpace((ch as PlayerCharacter).playerID))
                    {
                        this.adminEditCharPIDTextBox.Text = (ch as PlayerCharacter).playerID;
                    }
                    // home fief
                    this.adminEditCharHomeTextBox.Text = (ch as PlayerCharacter).homeFief;
                    // ancestral home fief
                    this.adminEditCharAncHomeTextBox.Text = (ch as PlayerCharacter).ancestralHomeFief;
                    // purse
                    this.adminEditCharPurseTextBox.Text = (ch as PlayerCharacter).purse.ToString();
                    // outlawed
                    if ((ch as PlayerCharacter).outlawed)
                    {
                        this.adminEditCharOutlCheckBox.Checked = true;
                    }
                    else
                    {
                        this.adminEditCharOutlCheckBox.Checked = false;
                    }
                }

                // NPC data
                else
                {
                    // wage
                    this.adminEditCharWageTextBox.Text = (ch as NonPlayerCharacter).salary.ToString();
                }
            }
        }

        /// <summary>
        /// Refreshes the edit place display, displaying details of the specified place
        /// and clearing any previously displayed data
        /// </summary>
        /// <param name="p">The Place to be displayed</param>
        public void refreshPlaceEdit(Place p = null)
        {
            // clear previous data
            this.disableControls(this.adminEditPlaceContainer.Panel1);
            this.disableControls(this.adminEditFiefPanel);
            this.disableControls(this.adminEditProvPanel);
            this.disableControls(this.adminEditKingPanel);

            // display place data, if provided
            if (p != null)
            {
                // id
                this.adminEditPlaceIdTextBox.Text = p.id;
                // name
                this.adminEditPlaceNameTextBox.Text = p.name;
                // owner
                this.adminEditPlaceOwnTextBox.Text = p.owner.charID;
                // title holder
                this.adminEditPlaceTiHoTextBox.Text = p.titleHolder;
                // rank
                this.adminEditPlaceRankTextBox.Text = p.rank.id.ToString();

                // Fief data
                if (p is Fief)
                {
                    // province ID
                    this.adminEditFiefProvTextBox.Text = (p as Fief).province.id;
                    // ancestral owner
                    this.adminEditFiefAncownTextBox.Text = (p as Fief).ancestralOwner.charID;
                    // bailiff
                    if ((p as Fief).bailiff != null)
                    {
                        this.adminEditFiefBailTextBox.Text = (p as Fief).bailiff.charID;
                    }
                    // bailiff days
                    this.adminEditFiefBaildaysTextBox.Text = (p as Fief).bailiffDaysInFief.ToString();
                    // population
                    this.adminEditFiefPopTextBox.Text = (p as Fief).population.ToString();
                    // fields
                    this.adminEditFiefFldTextBox.Text = (p as Fief).fields.ToString();
                    // industry
                    this.adminEditFiefIndTextBox.Text = (p as Fief).industry.ToString();
                    // tax rate
                    this.adminEditFiefTaxTextBox.Text = (p as Fief).taxRate.ToString();
                    // tax rate next
                    this.adminEditFiefTaxnxtTextBox.Text = (p as Fief).taxRateNext.ToString();
                    // next officials spend
                    this.adminEditFiefOffnxtTextBox.Text = (p as Fief).officialsSpendNext.ToString();
                    // next garrison spend
                    this.adminEditFiefGarrnxtTextBox.Text = (p as Fief).garrisonSpendNext.ToString();
                    // next infrastructure spend
                    this.adminEditFiefInfnxtTextBox.Text = (p as Fief).infrastructureSpendNext.ToString();
                    // next keep spend
                    this.adminEditFiefKpnxtTextBox.Text = (p as Fief).keepSpendNext.ToString();
                    // keep level
                    this.adminEditFiefKplvlTextBox.Text = (p as Fief).keepLevel.ToString();
                    // loyalty
                    this.adminEditFiefLoyTextBox.Text = (p as Fief).loyalty.ToString();
                    // status
                    this.adminEditFiefStatTextBox.Text = (p as Fief).status.ToString();
                    // language
                    this.adminEditFiefLangTextBox.Text = (p as Fief).language.id;
                    // treasury
                    this.admineditFiefTreasTextBox.Text = (p as Fief).treasury.ToString();
                    // hasRecruited
                    if ((p as Fief).hasRecruited)
                    {
                        this.adminEditFiefRecrCheckBox.Checked = true;
                    }
                    else
                    {
                        this.adminEditFiefRecrCheckBox.Checked = false;
                    }
                    // isPillaged
                    if ((p as Fief).isPillaged)
                    {
                        this.adminEditFiefPillCheckBox.Checked = true;
                    }
                    else
                    {
                        this.adminEditFiefPillCheckBox.Checked = false;
                    }
                }

                // province data
                else if (p is Province)
                {
                    // overlord tax rate
                    this.adminEditProvTaxTextBox.Text = (p as Province).taxRate.ToString();
                    // kingdom
                    this.adminEditProvKgdmTextBox.Text = (p as Province).kingdom.id;
                }

                // kingdom data
                else
                {
                    // nationality
                    this.adminEditKingNatTextBox.Text = (p as Kingdom).nationality.natID;
                }
            }
        }

        /// <summary>
        /// Refreshes the edit Skill display, displaying details of the specified Skill
        /// and clearing any previously displayed data
        /// </summary>
        /// <param name="s">The Skill to be displayed</param>
        public void refreshSkillEdit(Skill s = null)
        {
            // clear previous data
            this.disableControls(this.adminEditSkillPanel);

            // display skill data, if provided
            if (s != null)
            {
                // id
                this.adminEditSkillIdTextBox.Text = s.skillID;
                // name
                this.adminEditSkillNameTextBox.Text = s.name;
                // effects - iterates through skill effects adding information to ListView
                this.refreshSkillEffectsList(s.effects);
            }
        }

        /// <summary>
        /// Refreshes the skill effects list on the edit Skill display
        /// </summary>
        /// <param name="effects">The effects to be displayed</param>
        public void refreshSkillEffectsList(Dictionary<string, Double> effects)
        {
            // clear existing data
            this.adminEditSkillEffsListView.Items.Clear();

            if (effects.Count > 0)
            {
                // iterate through skill effects adding information to ListView
                foreach (KeyValuePair<string, double> effectEntry in effects)
                {
                    ListViewItem skillItem = null;

                    // effect name
                    skillItem = new ListViewItem(effectEntry.Key);

                    // effect level
                    skillItem.SubItems.Add(effectEntry.Value.ToString());

                    if (skillItem != null)
                    {
                        // add item to ListView
                        this.adminEditSkillEffsListView.Items.Add(skillItem);
                    }

                }
            }
        }

        /// <summary>
        /// Refreshes the edit Army display, displaying details of the specified Army
        /// and clearing any previously displayed data
        /// </summary>
        /// <param name="a">The Army to be displayed</param>
        public void refreshArmyEdit(Army a = null)
        {
            // clear previous data
            this.disableControls(this.adminEditArmyPanel);

            // display army data, if provided
            if (a != null)
            {
                // id
                this.adminEditArmyIdTextBox.Text = a.armyID;
                // owner
                this.adminEditArmyOwnTextBox.Text = a.owner;
                // leader
                if (!String.IsNullOrWhiteSpace(a.leader))
                {
                    this.adminEditArmyLdrTextBox.Text = a.leader;
                }
                // location
                this.adminEditArmyLocTextBox.Text = a.location;
                // days
                this.adminEditArmyDaysTextBox.Text = a.days.ToString();
                // aggression
                this.adminEditArmyAggrTextBox.Text = a.aggression.ToString();
                // combat odds
                this.adminEditArmyOddsTextBox.Text = a.combatOdds.ToString();
                // isMaintained
                if (a.isMaintained)
                {
                    this.adminEditArmyMaintCheckBox.Checked = true;
                }
                else
                {
                    this.adminEditArmyMaintCheckBox.Checked = false;
                }
            }
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of any of the adminEditSkillEffsListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditSkillEffsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.adminEditSkillEffsListView.SelectedItems.Count > 0)
            {
                // display selected skill for editing
                this.adminEditSkillEffTextBox.Text = this.adminEditSkillEffsListView.SelectedItems[0].SubItems[0].Text;
                this.adminEditSkillEfflvlTextBox.Text = this.adminEditSkillEffsListView.SelectedItems[0].SubItems[1].Text;
            }
        }

        /// <summary>
        /// Responds to the click event of any of the 'edit skill effects' buttons
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditSkillEffBtn_Click(object sender, EventArgs e)
        {
            string effName = null;
            double effLvl = 0;
            bool effectsChanged = false;

            // get button and tag
            Button thisButton = (sender as Button);
            string operation = thisButton.Tag.ToString();

            try
            {
                // get effects collection
                Dictionary<string, double> effects = new Dictionary<string, double>();
                for (int i = 0; i < this.adminEditSkillEffsListView.Items.Count; i++)
                {
                    effects.Add(this.adminEditSkillEffsListView.Items[i].SubItems[0].Text,
                        Convert.ToDouble(this.adminEditSkillEffsListView.Items[i].SubItems[1].Text));
                }

                // get selected effect
                effName = this.adminEditSkillEffTextBox.Text;
                if (!String.IsNullOrWhiteSpace(effName))
                {
                    effLvl = Convert.ToDouble(this.adminEditSkillEfflvlTextBox.Text);
                }

                if (effLvl > 0)
                {
                    // perform operation
                    switch (operation)
                    {
                        // change selected effect
                        case "chaEffect":
                            // check effect present in collection
                            if (effects.ContainsKey(effName))
                            {
                                effects[effName] = effLvl;
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " does not exist.  Operation cancelled.");
                                }
                            }
                            break;
                        case "addEffect":
                            // check effect present in collection
                            if (!effects.ContainsKey(effName))
                            {
                                effects.Add(effName, effLvl);
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " already exists.  Operation cancelled.");
                                }
                            }
                            break;
                        case "delEffect":
                            // check effect present in collection
                            if (effects.ContainsKey(effName))
                            {
                                effects.Remove(effName);
                                effectsChanged = true;
                            }
                            else
                            {
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("The effect " + effName + " does not exist.  Operation cancelled.");
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (effectsChanged)
                    {
                        this.refreshSkillEffectsList(effects);
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Effects updated.");
                        }
                    }
                }

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the adminEditSaveBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adminEditSaveBtn_Click(object sender, EventArgs e)
        {
            bool success = false;

            // get button
            Button thisButton = (sender as Button);
            string objectType = thisButton.Tag.ToString();

            // save specified object
            switch (objectType)
            {
                case "PC":
                    success = this.saveCharEdit(objectType);
                    break;

                case "NPC":
                    success = this.saveCharEdit(objectType);
                    break;

                case "Fief":
                    success = this.savePlaceEdit(objectType);
                    break;

                case "Province":
                    success = this.savePlaceEdit(objectType);
                    break;

                case "Kingdom":
                    success = this.savePlaceEdit(objectType);
                    break;

                case "Skill":
                    success = this.saveSkillEdit();
                    break;

                case "Army":
                    success = this.saveArmyEdit();
                    break;

                default:
                    break;
            }

            if (success)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Object saved.");
                }
            }
        }

        /// <summary>
        /// Saves the Skill currently being edited in the SysAdmin interface
        /// </summary>
        /// <returns>bool indicating success</returns>
        public bool saveSkillEdit()
        {
            bool success = false;

            try
            {
                // get data from edit form
                string id = this.adminEditSkillIdTextBox.Text;
                string name = this.adminEditSkillNameTextBox.Text;
                Dictionary<string, double> effects = new Dictionary<string, double>();

                for (int i = 0; i < this.adminEditSkillEffsListView.Items.Count; i++)
                {
                    effects.Add(this.adminEditSkillEffsListView.Items[i].SubItems[0].Text,
                        Convert.ToDouble(this.adminEditSkillEffsListView.Items[i].SubItems[1].Text));
                }

                // create new skill
                Skill thisSkill = new Skill(id, name, effects);

                // replace existing skill in skillMasterList
                Globals_Game.skillMasterList[id] = thisSkill;

                success = true;

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }

            return success;
        }

        /// <summary>
        /// Saves the Army currently being edited in the SysAdmin interface
        /// </summary>
        /// <returns>bool indicating success</returns>
        public bool saveArmyEdit()
        {
            bool success = false;
            Army thisArmy = null;

            try
            {
                // get data from edit form
                string id = this.adminEditArmyIdTextBox.Text;
                string owner = this.adminEditArmyOwnTextBox.Text;
                string ldr = this.adminEditArmyLdrTextBox.Text;
                string loc = this.adminEditArmyLocTextBox.Text;
                double days = Convert.ToDouble(this.adminEditArmyDaysTextBox.Text);
                byte aggr = Convert.ToByte(this.adminEditArmyAggrTextBox.Text);
                byte odds = Convert.ToByte(this.adminEditArmyOddsTextBox.Text);
                bool maint = false;
                if (this.adminEditArmyMaintCheckBox.Checked)
                {
                    maint = true;
                }

                // get original object
                if (Globals_Game.armyMasterList.ContainsKey(id))
                {
                    thisArmy = Globals_Game.armyMasterList[id];
                }

                // replace original object properties
                if (thisArmy != null)
                {
                    thisArmy.owner = owner;
                    thisArmy.leader = ldr;
                    thisArmy.location = loc;
                    thisArmy.days = days;
                    thisArmy.aggression = aggr;
                    thisArmy.combatOdds = odds;
                    thisArmy.isMaintained = maint;
                }

                success = true;

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }

            return success;
        }

        /// <summary>
        /// Saves the Character currently being edited in the SysAdmin interface
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="objectType">The type of Character to be saved</param>
        public bool saveCharEdit(string objectType)
        {
            bool success = false;
            PlayerCharacter thisPC = null;
            NonPlayerCharacter thisNPC = null;
            Character charToSave = null;
            Nationality thisNat = null;
            Language thisLang = null;
            Skill thisSkill1 = null;
            Skill thisSkill2 = null;
            Skill thisSkill3 = null;
            Fief thisFief = null;

            try
            {
                // get original object
                string id = this.adminEditCharIDTextBox.Text;
                if (objectType.Equals("PC"))
                {
                    if (Globals_Game.pcMasterList.ContainsKey(id))
                    {
                        thisPC = Globals_Game.pcMasterList[id];
                    }

                    if (thisPC != null)
                    {
                        // get data from edit form and replace original data
                        // playerID
                        string pID = this.adminEditCharPIDTextBox.Text;
                        thisPC.playerID = pID;

                        // home fief
                        string home = this.adminEditCharHomeTextBox.Text;
                        thisPC.homeFief = home;

                        // ancestral home
                        string ancHome = this.adminEditCharAncHomeTextBox.Text;
                        thisPC.ancestralHomeFief = ancHome;

                        // purse
                        uint pur = Convert.ToUInt32(this.adminEditCharPurseTextBox.Text);
                        thisPC.purse = pur;

                        // outlawed
                        bool outl = false;
                        if (this.adminEditCharOutlCheckBox.Checked)
                        {
                            outl = true;
                        }
                        thisPC.outlawed = outl;
                    }
                }

                else if (objectType.Equals("NPC"))
                {
                    if (Globals_Game.npcMasterList.ContainsKey(id))
                    {
                        thisNPC = Globals_Game.npcMasterList[id];
                    }

                    if (thisNPC != null)
                    {
                        // get data from edit form and replace original data
                        // wage
                        uint wa = Convert.ToUInt32(this.adminEditCharWageTextBox.Text);
                        thisNPC.salary = wa;
                    }

                }

                // get generic character data from edit form and replace original data
                if (thisPC != null)
                {
                    charToSave = thisPC;
                }
                else if (thisNPC != null)
                {
                    charToSave = thisNPC;
                }

                if (charToSave != null)
                {
                    // firstname
                    string fname = this.adminEditChar1stnameTextBox.Text;
                    charToSave.firstName = fname;

                    // surname
                    string lname = this.adminEditCharSurnameTextBox.Text;
                    charToSave.familyName = lname;

                    // birthdate
                    Tuple<uint, byte> bDate = new Tuple<uint, byte>(Convert.ToUInt32(this.adminEditCharByearTextBox.Text),
                        Convert.ToByte(this.adminEditCharBseasTextBox.Text));
                    charToSave.birthDate = bDate;

                    // sex
                    bool male = false;
                    if (this.adminEditCharSexCheckBox.Checked)
                    {
                        male = true;
                    }
                    charToSave.isMale = male;

                    // nationality
                    string natID = this.adminEditCharNatTextBox.Text;
                    if (Globals_Game.nationalityMasterList.ContainsKey(natID))
                    {
                        thisNat = Globals_Game.nationalityMasterList[natID];
                    }
                    if (thisNat != null)
                    {
                        charToSave.nationality = thisNat;
                    }

                    // language
                    string langID = this.adminEditCharLangTextBox.Text;
                    if (Globals_Game.languageMasterList.ContainsKey(langID))
                    {
                        thisLang = Globals_Game.languageMasterList[langID];
                    }
                    if (thisLang != null)
                    {
                        charToSave.language = thisLang;
                    }

                    // days
                    double days = Convert.ToDouble(this.adminEditCharDaysTextBox.Text);
                    charToSave.days = days;

                    // stature modifier
                    double stat = Convert.ToDouble(this.adminEditCharStatTextBox.Text);
                    charToSave.statureModifier = stat;

                    // maxhealth
                    double hea = Convert.ToDouble(this.adminEditCharHeaTextBox.Text);
                    charToSave.maxHealth = hea;

                    // virility
                    double vir = Convert.ToDouble(this.adminEditCharVirTextBox.Text);
                    charToSave.virility = vir;

                    // management
                    double man = Convert.ToDouble(this.admineditCharManTextBox.Text);
                    charToSave.management = man;

                    // combat
                    double com = Convert.ToDouble(this.admineditCharComTextBox.Text);
                    charToSave.combat = com;

                    // skills
                    string skill1id = this.adminEditCharSkNameTextBox1.Text;
                    if (Globals_Game.skillMasterList.ContainsKey(skill1id))
                    {
                        thisSkill1 = Globals_Game.skillMasterList[skill1id];
                    }
                    string skill2id = this.adminEditCharSkNameTextBox2.Text;
                    if (Globals_Game.skillMasterList.ContainsKey(skill2id))
                    {
                        thisSkill2 = Globals_Game.skillMasterList[skill2id];
                    }
                    string skill3id = this.adminEditCharSkNameTextBox3.Text;
                    if (Globals_Game.skillMasterList.ContainsKey(skill3id))
                    {
                        thisSkill3 = Globals_Game.skillMasterList[skill3id];
                    }
                    int skill1Lvl = 0;
                    if (!String.IsNullOrWhiteSpace(this.adminEditCharSkLvlTextBox1.Text))
                    {
                        skill1Lvl = Convert.ToInt32(this.adminEditCharSkLvlTextBox1.Text);
                    }
                    int skill2Lvl = 0;
                    if (!String.IsNullOrWhiteSpace(this.adminEditCharSkLvlTextBox2.Text))
                    {
                        skill2Lvl = Convert.ToInt32(this.adminEditCharSkLvlTextBox2.Text);
                    }
                    int skill3Lvl = 0;
                    if (!String.IsNullOrWhiteSpace(this.adminEditCharSkLvlTextBox3.Text))
                    {
                        skill3Lvl = Convert.ToInt32(this.adminEditCharSkLvlTextBox3.Text);
                    }
                    List<Tuple<Skill, int>> tempSkills = new List<Tuple<Skill, int>>();
                    if ((thisSkill1 != null) && ((skill1Lvl >= 1) && (skill1Lvl <= 9)))
                    {
                        Tuple<Skill, int> thisTuple = new Tuple<Skill, int>(thisSkill1, skill1Lvl);
                        tempSkills.Add(thisTuple);
                    }
                    if ((thisSkill2 != null) && ((skill2Lvl >= 1) && (skill2Lvl <= 9)))
                    {
                        Tuple<Skill, int> thisTuple = new Tuple<Skill, int>(thisSkill2, skill2Lvl);
                        tempSkills.Add(thisTuple);
                    }
                    if ((thisSkill3 != null) && ((skill3Lvl >= 1) && (skill3Lvl <= 9)))
                    {
                        Tuple<Skill, int> thisTuple = new Tuple<Skill, int>(thisSkill3, skill3Lvl);
                        tempSkills.Add(thisTuple);
                    }
                    if (tempSkills.Count >= 2)
                    {
                        charToSave.skills = tempSkills.ToArray();
                    }

                    // location
                    string locID = this.adminEditCharLocTextBox.Text;
                    if (Globals_Game.fiefMasterList.ContainsKey(locID))
                    {
                        thisFief = Globals_Game.fiefMasterList[locID];
                    }
                    if (thisFief != null)
                    {
                        charToSave.location = thisFief;
                    }

                    // inKeep
                    bool inK = false;
                    if (this.adminEditCharInKpCheckBox.Checked)
                    {
                        inK = true;
                    }
                    charToSave.inKeep = inK;
                }

                success = true;

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }

            return success;
        }

        /// <summary>
        /// Saves the Place currently being edited in the SysAdmin interface
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="objectType">The type of Place to be saved</param>
        public bool savePlaceEdit(string objectType)
        {
            bool success = false;
            PlayerCharacter owner = null;
            PlayerCharacter ancOwn = null;
            Character bail = null;
            Nationality thisNat = null;
            Language thisLang = null;
            Fief thisFief = null;
            Province thisProv = null;
            Kingdom thisKing = null;
            Rank thisRank = null;
            Place placeToSave = null;

            try
            {
                // get original object
                string id = this.adminEditPlaceIdTextBox.Text;
                if (objectType.Equals("Fief"))
                {
                    if (Globals_Game.fiefMasterList.ContainsKey(id))
                    {
                        thisFief = Globals_Game.fiefMasterList[id];
                    }

                    if (thisFief != null)
                    {
                        // get data from edit form and replace original data
                        // province
                        string provID = this.adminEditFiefProvTextBox.Text;
                        if (Globals_Game.provinceMasterList.ContainsKey(provID))
                        {
                            thisProv = Globals_Game.provinceMasterList[provID];
                        }
                        if (thisProv != null)
                        {
                            thisFief.province = thisProv;
                        }

                        // ancestral owner
                        string ancOwnID = this.adminEditFiefAncownTextBox.Text;
                        if (Globals_Game.pcMasterList.ContainsKey(ancOwnID))
                        {
                            ancOwn = Globals_Game.pcMasterList[ancOwnID];
                        }
                        if (ancOwn != null)
                        {
                            thisFief.ancestralOwner = ancOwn;
                        }

                        // bailiff
                        if (!String.IsNullOrWhiteSpace(this.adminEditFiefBailTextBox.Text))
                        {
                            string bailID = this.adminEditFiefBailTextBox.Text;
                            if (Globals_Game.pcMasterList.ContainsKey(bailID))
                            {
                                bail = Globals_Game.pcMasterList[bailID];
                            }
                            else if (Globals_Game.npcMasterList.ContainsKey(bailID))
                            {
                                bail = Globals_Game.npcMasterList[bailID];
                            }
                        }
                        if (bail != null)
                        {
                            thisFief.bailiff = bail;
                        }

                        // bailiff days
                        double bailDays = Convert.ToDouble(this.adminEditFiefBaildaysTextBox.Text);
                        thisFief.bailiffDaysInFief = bailDays;

                        // population
                        int pop = Convert.ToInt32(this.adminEditFiefBaildaysTextBox.Text);
                        thisFief.population = pop;

                        // fields
                        double fld = Convert.ToDouble(this.adminEditFiefFldTextBox.Text);
                        thisFief.fields = fld;

                        // industry
                        double ind = Convert.ToDouble(this.adminEditFiefIndTextBox.Text);
                        thisFief.industry = ind;

                        // taxrate
                        double tx = Convert.ToDouble(this.adminEditFiefTaxTextBox.Text);
                        thisFief.taxRate = tx;

                        // tax next
                        double txNxt = Convert.ToDouble(this.adminEditFiefTaxnxtTextBox.Text);
                        thisFief.taxRateNext = txNxt;

                        // officials spend next
                        uint offNxt = Convert.ToUInt32(this.adminEditFiefOffnxtTextBox.Text);
                        thisFief.officialsSpendNext = offNxt;

                        // garrison spend next
                        uint garrNxt = Convert.ToUInt32(this.adminEditFiefGarrnxtTextBox.Text);
                        thisFief.garrisonSpendNext = garrNxt;

                        // infrastructure spend next
                        uint infNxt = Convert.ToUInt32(this.adminEditFiefInfnxtTextBox.Text);
                        thisFief.infrastructureSpendNext = infNxt;

                        // keep spend next
                        uint kpNxt = Convert.ToUInt32(this.adminEditFiefKpnxtTextBox.Text);
                        thisFief.keepSpendNext = kpNxt;

                        // keep level
                        double kpLvl = Convert.ToDouble(this.adminEditFiefKplvlTextBox.Text);
                        thisFief.keepLevel = kpLvl;

                        // loyalty
                        double loy = Convert.ToDouble(this.adminEditFiefLoyTextBox.Text);
                        thisFief.loyalty = loy;

                        // status
                        char stat = Convert.ToChar(this.adminEditFiefStatTextBox.Text);
                        thisFief.status = stat;

                        // language
                        string langID = this.adminEditFiefLangTextBox.Text;
                        if (Globals_Game.languageMasterList.ContainsKey(langID))
                        {
                            thisLang = Globals_Game.languageMasterList[langID];
                        }
                        if (thisLang != null)
                        {
                            thisFief.language = thisLang;
                        }

                        // treasury
                        int treas = Convert.ToInt32(this.admineditFiefTreasTextBox.Text);
                        thisFief.treasury = treas;

                        // hasRecruited
                        bool hasRec = false;
                        if (this.adminEditFiefRecrCheckBox.Checked)
                        {
                            hasRec = true;
                        }
                        thisFief.hasRecruited = hasRec;

                        // isPillaged
                        bool isPill = false;
                        if (this.adminEditFiefPillCheckBox.Checked)
                        {
                            isPill = true;
                        }
                        thisFief.isPillaged = isPill;
                    }
                }

                else if (objectType.Equals("Province"))
                {
                    if (Globals_Game.provinceMasterList.ContainsKey(id))
                    {
                        thisProv = Globals_Game.provinceMasterList[id];
                    }

                    if (thisProv != null)
                    {
                        // get data from edit form and replace original data
                        // overlord taxrate
                        double oTx = Convert.ToDouble(this.adminEditProvTaxTextBox.Text);
                        thisProv.taxRate = oTx;

                        // kingdom
                        string kingID = this.adminEditProvKgdmTextBox.Text;
                        if (Globals_Game.kingdomMasterList.ContainsKey(kingID))
                        {
                            thisKing = Globals_Game.kingdomMasterList[kingID];
                        }
                        if (thisKing != null)
                        {
                            thisProv.kingdom = thisKing;
                        }
                    }
                }

                else if (objectType.Equals("Kingdom"))
                {
                    if (Globals_Game.kingdomMasterList.ContainsKey(id))
                    {
                        thisKing = Globals_Game.kingdomMasterList[id];
                    }

                    if (thisKing != null)
                    {
                        // get data from edit form and replace original data
                        // nationality
                        string natID = this.adminEditKingNatTextBox.Text;
                        if (Globals_Game.nationalityMasterList.ContainsKey(natID))
                        {
                            thisNat = Globals_Game.nationalityMasterList[natID];
                        }
                        if (thisNat != null)
                        {
                            thisKing.nationality = thisNat;
                        }
                    }
                }

                // get generic place data from edit form and replace original data
                if (objectType.Equals("Fief"))
                {
                    if (thisFief != null)
                    {
                        placeToSave = thisFief;
                    }
                }
                else if (objectType.Equals("Province"))
                {
                    if (thisProv != null)
                    {
                        placeToSave = thisProv;
                    }
                }
                else if (objectType.Equals("Kingdom"))
                {
                    if (thisKing != null)
                    {
                        placeToSave = thisKing;
                    }
                }

                if (placeToSave != null)
                {
                    // name
                    string nam = this.adminEditPlaceNameTextBox.Text;
                    placeToSave.name = nam;

                    // owner
                    string ownID = this.adminEditPlaceOwnTextBox.Text;
                    if (Globals_Game.pcMasterList.ContainsKey(ownID))
                    {
                        owner = Globals_Game.pcMasterList[ownID];
                    }
                    if (owner != null)
                    {
                        placeToSave.owner = owner;
                    }

                    // title holder
                    string tiHo = this.adminEditPlaceTiHoTextBox.Text;
                    placeToSave.titleHolder = tiHo;

                    // rank
                    byte rankID = Convert.ToByte(this.adminEditPlaceRankTextBox.Text);
                    if (Globals_Game.rankMasterList.ContainsKey(rankID))
                    {
                        thisRank = Globals_Game.rankMasterList[rankID];
                    }
                    if (thisRank != null)
                    {
                        placeToSave.rank = thisRank;
                    }
                }

                success = true;

            }
            catch (System.FormatException fe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(fe.Message + "\r\nPlease enter a valid value.");
                }
            }
            catch (System.OverflowException ofe)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(ofe.Message + "\r\nPlease enter a valid value.");
                }
            }

            return success;
        }
    }
}
