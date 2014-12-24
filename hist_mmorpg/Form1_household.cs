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
    /// Partial class for Form1, containing functionality specific to household management
    /// </summary>
    partial class Form1
    {

        // ------------------- HOUSEHOLD MANAGEMENT

        /// <summary>
        /// Creates UI display for list of characters in the Household screen
        /// </summary>
        public void setUpHouseholdCharsList()
        {
            // add necessary columns
            this.houseCharListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Function", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Responsibilities", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
            this.houseCharListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes Household display
        /// </summary>
        /// <param name="npc">NonPlayerCharacter to display</param>
        public void refreshHouseholdDisplay(NonPlayerCharacter npc = null)
        {
            // set main character display as read only
            this.houseCharTextBox.ReadOnly = true;

            // disable controls until NPC selected in ListView
            this.houseCampBtn.Enabled = false;
            this.houseCampDaysTextBox.Enabled = false;
            this.familyNameChildButton.Enabled = false;
            this.familyNameChildTextBox.Enabled = false;
            this.familyNpcSpousePregBtn.Enabled = false;
            this.houseHeirBtn.Enabled = false;
            this.houseMoveToBtn.Enabled = false;
            this.houseMoveToTextBox.Enabled = false;
            this.houseRouteBtn.Enabled = false;
            this.houseEntourageBtn.Enabled = false;
            this.houseFireBtn.Enabled = false;
            this.houseExamineArmiesBtn.Enabled = false;

            // remove any previously displayed text
            this.houseCharTextBox.Text = "";
            this.houseCampDaysTextBox.Text = "";
            this.familyNameChildTextBox.Text = "";
            this.houseMoveToTextBox.Text = "";
            this.houseRouteTextBox.Text = "";
            this.houseProposeBrideTextBox.Text = "";

            // clear existing items in characters list
            this.houseCharListView.Items.Clear();

            // variables needed for name check (to see if NPC needs naming)
            string nameWarning = "The following offspring need to be named:\r\n\r\n";
            bool showNameWarning = false;

            // iterates through household characters adding information to ListView
            // and checking if naming is required
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
            {
                ListViewItem houseChar = null;

                // name
                houseChar = new ListViewItem(Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName);

                // charID
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].charID);

                // function (e.g. employee, son, wife, etc.)
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].getFunction(Globals_Client.myPlayerCharacter));

                // responsibilities (i.e. jobs)
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].getResponsibilities(Globals_Client.myPlayerCharacter));

                // location
                houseChar.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].location.id + " (" + Globals_Client.myPlayerCharacter.myNPCs[i].location.name + ")");

                // show whether is in player's entourage
                if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                {
                    houseChar.SubItems.Add("Yes");
                }

                // check if needs to be named
                if (!String.IsNullOrWhiteSpace(Globals_Client.myPlayerCharacter.myNPCs[i].familyID))
                {
                    bool nameRequired = Globals_Client.myPlayerCharacter.myNPCs[i].hasBabyName(0);

                    if (nameRequired)
                    {
                        showNameWarning = true;
                        nameWarning += " - " + Globals_Client.myPlayerCharacter.myNPCs[i].charID + " (" + Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName + ")\r\n";
                    }
                }

                if (houseChar != null)
                {
                    // if NPC passed in as parameter, show as selected
                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == npc)
                    {
                        houseChar.Selected = true;
                    }

                    // add item to fiefsListView
                    this.houseCharListView.Items.Add(houseChar);
                }

            }

            // always enable marriage proposal controls
            this.houseProposeBtn.Enabled = true;
            this.houseProposeBrideTextBox.Enabled = true;
            this.houseProposeGroomTextBox.Enabled = true;
            this.houseProposeGroomTextBox.Text = Globals_Client.myPlayerCharacter.charID;

            this.houseCharListView.HideSelection = false;
            this.houseCharListView.Focus();

            if (showNameWarning)
            {
                nameWarning += "\r\nAny children who are not named by the age of one will,\r\nwhere possible, be named after their royal highnesses the king and queen.";
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(nameWarning);
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the dealWithHouseholdAffairsToolStripMenuItem
        /// which causes the Household screen to display, listing the player's
        /// family and employed NPCs
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void dealWithHouseholdAffairsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.charToView = null;

            // refresh household affairs screen 
            this.refreshHouseholdDisplay();

            // display household affairs screen
            Globals_Client.containerToView = this.houseContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the houseCharListView object,
        /// invoking the displayCharacter method, passing a Character to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseCharListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Character charToDisplay = null;

            // loop through the characters in employees
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
            {
                if (this.houseCharListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.myPlayerCharacter.myNPCs[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.myPlayerCharacter.myNPCs[i];
                        break;
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                Globals_Client.charToView = charToDisplay;
                this.houseCharTextBox.Text = this.displayCharacter(charToDisplay);
                this.houseCharTextBox.ReadOnly = true;

                // see if is in entourage to set text of entourage button
                if ((charToDisplay as NonPlayerCharacter).inEntourage)
                {
                    this.houseEntourageBtn.Text = "Remove From Entourage";
                }
                else
                {
                    this.houseEntourageBtn.Text = "Add To Entourage";
                }

                // FAMILY MATTERS CONTROLS
                // if family selected, enable 'choose heir' button, disbale 'fire' button
                if ((!String.IsNullOrWhiteSpace(Globals_Client.charToView.familyID)) && (Globals_Client.charToView.familyID.Equals(Globals_Client.myPlayerCharacter.charID)))
                {
                    this.houseHeirBtn.Enabled = true;
                    this.houseFireBtn.Enabled = false;

                    // if is male and married, enable NPC 'get wife with child' control
                    if ((Globals_Client.charToView.isMale) && (!String.IsNullOrWhiteSpace(Globals_Client.charToView.spouse)))
                    {
                        this.familyNpcSpousePregBtn.Enabled = true;
                    }
                    else
                    {
                        this.familyNpcSpousePregBtn.Enabled = false;
                    }
                }
                else
                {
                    this.houseHeirBtn.Enabled = false;
                    this.houseFireBtn.Enabled = true;
                    this.familyNpcSpousePregBtn.Enabled = false;
                }

                // if character firstname = "Baby", enable 'name child' controls
                if ((charToDisplay as NonPlayerCharacter).firstName.Equals("Baby"))
                {
                    this.familyNameChildButton.Enabled = true;
                    this.familyNameChildTextBox.Enabled = true;
                }
                // if not, ensure are disabled
                else
                {
                    this.familyNameChildButton.Enabled = false;
                    this.familyNameChildTextBox.Enabled = false;
                }

                // 'get wife with child' button always enabled
                this.familyGetSpousePregBtn.Enabled = true;

                // SIEGE CHECKS
                // check to see if is inside besieged keep
                if ((Globals_Client.charToView.inKeep) && (!String.IsNullOrWhiteSpace(Globals_Client.charToView.location.siege)))
                {
                    // if is inside besieged keep, disable most of controls
                    this.houseCampBtn.Enabled = false;
                    this.houseCampDaysTextBox.Enabled = false;
                    this.houseMoveToBtn.Enabled = false;
                    this.houseMoveToTextBox.Enabled = false;
                    this.houseRouteBtn.Enabled = false;
                    this.houseEntourageBtn.Enabled = false;
                    this.houseFireBtn.Enabled = false;
                    this.houseExamineArmiesBtn.Enabled = false;
                }

                // is NOT inside besieged keep
                else
                {
                    // re-enable controls
                    this.houseCampBtn.Enabled = true;
                    this.houseCampDaysTextBox.Enabled = true;
                    this.houseMoveToBtn.Enabled = true;
                    this.houseMoveToTextBox.Enabled = true;
                    this.houseRouteBtn.Enabled = true;
                    this.houseRouteTextBox.Enabled = true;
                    this.houseEntourageBtn.Enabled = true;
                    this.houseExamineArmiesBtn.Enabled = true;

                }

            }
        }

        /// <summary>
        /// Responds to the click event of the houseCampBtn button
        /// invoking the campWaitHere method
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseCampBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get days to camp
                byte campDays = Convert.ToByte(this.houseCampDaysTextBox.Text);

                // camp
                this.campWaitHere(Globals_Client.charToView, campDays);
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
            finally
            {
                // refresh display
                this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }

        }

        /// <summary>
        /// Responds to the click event of the familyNameChildButton
        /// allowing the player to name the selected child
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void familyNameChildButton_Click(object sender, EventArgs e)
        {
            NonPlayerCharacter child = null;

            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get NPC to name
                for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                {
                    if (Globals_Client.myPlayerCharacter.myNPCs[i].charID.Equals(this.houseCharListView.SelectedItems[0].SubItems[1].Text))
                    {
                        child = Globals_Client.myPlayerCharacter.myNPCs[i];
                        break;
                    }
                }

                if (child != null)
                {
                    if (Regex.IsMatch(this.familyNameChildTextBox.Text.Trim(), @"^[a-zA-Z- ]+$"))
                    {
                        child.firstName = this.familyNameChildTextBox.Text;
                        this.refreshHouseholdDisplay(child);
                    }
                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("'" + this.familyNameChildTextBox.Text + "' is an unsuitable name, milord.");
                        }
                    }
                }
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Could not retrieve details of NonPlayerCharacter.");
                    }
                }
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Please select a character from the list.");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the houseHeirBtn button
        /// allowing the switch to another player (for testing)
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseHeirBtn_Click(object sender, EventArgs e)
        {
            if (this.houseCharListView.SelectedItems.Count > 0)
            {
                // get selected NPC
                NonPlayerCharacter selectedNPC = Globals_Game.npcMasterList[this.houseCharListView.SelectedItems[0].SubItems[1].Text];

                // check for an existing heir and remove
                foreach (NonPlayerCharacter npc in Globals_Client.myPlayerCharacter.myNPCs)
                {
                    if (npc.isHeir)
                    {
                        npc.isHeir = false;
                    }
                }

                // appoint NPC as heir
                selectedNPC.isHeir = true;

                // refresh the household screen (in the main form)
                this.refreshHouseholdDisplay(selectedNPC);
            }

        }

        /// <summary>
        /// Responds to the click event of the houseExamineArmiesBtn button
        /// displaying a list of all armies in the current NPC's fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseExamineArmiesBtn_Click(object sender, EventArgs e)
        {
            // NPC
            Character thisObserver = Globals_Client.charToView;

            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            // if no NPC selected
            if (this.houseCharListView.SelectedItems.Count < 1)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No NPC selected!");
                }
            }

            // if NPC selected
            else
            {
                // check if has minimum days
                if (Globals_Client.charToView.days < 1)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You don't have enough days for this operation.");
                    }
                }

                // has minimum days
                else
                {
                    // see how long reconnaissance takes
                    int reconDays = Globals_Game.myRand.Next(1, 4);

                    // check if runs out of time
                    if (Globals_Client.charToView.days < reconDays)
                    {
                        // set days to 0
                        Globals_Client.charToView.adjustDays(Globals_Client.armyToView.days);
                        this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Due to poor execution, you have run out of time for this operation.");
                        }
                    }

                    // doesn't run out of time
                    else
                    {
                        // adjust days
                        Globals_Client.charToView.adjustDays(reconDays);
                        this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));

                        // display armies list
                        SelectionForm examineArmies = new SelectionForm(this, "armies", obs: thisObserver);
                        examineArmies.Show();
                    }

                }

            }

        }
    }
}
