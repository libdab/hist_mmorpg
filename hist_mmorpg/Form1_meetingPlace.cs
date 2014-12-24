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
    /// Partial class for Form1, containing functionality specific to the meeting place screen
    /// (court, tavern, outside keep)
    /// </summary>
    partial class Form1
    {

        // ------------------- MEETING PLACE

        /// <summary>
        /// Creates UI display for list of characters present in Court, Tavern, outside keep
        /// </summary>
        public void setUpMeetingPLaceCharsList()
        {
            // add necessary columns
            this.meetingPlaceCharsListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Sex", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Household", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Type", -2, HorizontalAlignment.Left);
            this.meetingPlaceCharsListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes UI Court, Tavern, outside keep display
        /// </summary>
        /// <param name="place">string specifying whether court, tavern, outside keep</param>
        public void refreshMeetingPlaceDisplay(string place)
        {
            // refresh general information
            this.meetingPlaceDisplayText();

            // remove any previously displayed characters
            this.meetingPlaceCharDisplayTextBox.ReadOnly = true;
            this.meetingPlaceCharDisplayTextBox.Text = "";

            // disable controls until character selected in list
            this.hireNPC_Btn.Enabled = false;
            this.hireNPC_TextBox.Text = "";
            this.hireNPC_TextBox.Enabled = false;
            this.meetingPlaceMoveToBtn.Enabled = false;
            this.meetingPlaceMoveToTextBox.Text = "";
            this.meetingPlaceMoveToTextBox.Enabled = false;
            this.meetingPlaceRouteBtn.Enabled = false;
            this.meetingPlaceRouteTextBox.Text = "";
            this.meetingPlaceRouteTextBox.Enabled = false;
            this.meetingPlaceEntourageBtn.Enabled = false;
            this.meetingPlaceProposeBtn.Enabled = false;
            this.meetingPlaceProposeTextBox.Text = "";
            this.meetingPlaceProposeTextBox.Enabled = false;

            // set label
            switch (place)
            {
                case "tavern":
                    this.meetingPlaceLabel.Text = "Ye Olde Tavern Of " + Globals_Client.fiefToView.name;
                    break;
                case "court":
                    this.meetingPlaceLabel.Text = "The Esteemed Court Of " + Globals_Client.fiefToView.name;
                    break;
                case "outsideKeep":
                    this.meetingPlaceLabel.Text = "Persons Present Outwith\r\nThe Keep Of " + Globals_Client.fiefToView.name;
                    break;
                default:
                    this.meetingPlaceLabel.Text = "A Generic Meeting Place";
                    break;
            }
            // refresh list of characters
            this.meetingPlaceDisplayList(place);
        }

        /// <summary>
        /// Refreshes general information displayed in Court, Tavern, outside keep
        /// </summary>
        public void meetingPlaceDisplayText()
        {
            string textToDisplay = "";
            // date/season and main character's days left
            textToDisplay += Globals_Game.clock.seasons[Globals_Game.clock.currentSeason] + ", " + Globals_Game.clock.currentYear + ".  Your days left: " + Globals_Client.myPlayerCharacter.days + "\r\n\r\n";
            // Fief name/ID and province name
            textToDisplay += "Fief: " + Globals_Client.myPlayerCharacter.location.name + " (" + Globals_Client.myPlayerCharacter.location.id + ")  in " + Globals_Client.myPlayerCharacter.location.province.name + ", " + Globals_Client.myPlayerCharacter.location.province.kingdom.name + "\r\n\r\n";
            // Fief owner
            textToDisplay += "Owner: " + Globals_Client.myPlayerCharacter.location.owner.firstName + " " + Globals_Client.myPlayerCharacter.location.owner.familyName + "\r\n";
            // Fief overlord
            textToDisplay += "Overlord: " + Globals_Client.myPlayerCharacter.location.getOverlord().firstName + " " + Globals_Client.myPlayerCharacter.location.getOverlord().familyName + "\r\n";

            this.meetingPlaceTextBox.ReadOnly = true;
            this.meetingPlaceTextBox.Text = textToDisplay;
        }

        /// <summary>
        /// Refreshes display of Character list in Court, Tavern, outside keep
        /// </summary>
        /// <param name="place">String specifying whether court, tavern, outside keep</param>
        public void meetingPlaceDisplayList(string place)
        {
            // clear existing items in list
            this.meetingPlaceCharsListView.Items.Clear();

            // select which characters to display - i.e. in the keep (court) or not (tavern)
            bool ifInKeep = false;
            if (place.Equals("court"))
            {
                ifInKeep = true;
            }

            // iterates through characters
            for (int i = 0; i < Globals_Client.myPlayerCharacter.location.charactersInFief.Count; i++)
            {
                ListViewItem charsInCourt = null;

                // only display characters in relevant location (in keep, or not)
                if (Globals_Client.myPlayerCharacter.location.charactersInFief[i].inKeep == ifInKeep)
                {
                    // don't show the player
                    if (Globals_Client.myPlayerCharacter.location.charactersInFief[i] != Globals_Client.myPlayerCharacter)
                    {

                        switch (place)
                        {
                            case "tavern":
                                // only show NPCs
                                if (Globals_Client.myPlayerCharacter.location.charactersInFief[i] is NonPlayerCharacter)
                                {
                                    // only show unemployed
                                    if ((Globals_Client.myPlayerCharacter.location.charactersInFief[i] as NonPlayerCharacter).salary == 0)
                                    {
                                        // Create an item and subitems for character
                                        charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myPlayerCharacter.location.charactersInFief[i]);
                                    }
                                }
                                break;
                            default:
                                // Create an item and subitems for character
                                charsInCourt = this.createMeetingPlaceListItem(Globals_Client.myPlayerCharacter.location.charactersInFief[i]);
                                break;
                        }

                    }
                }

                // add item to fiefsListView
                if (charsInCourt != null)
                {
                    this.meetingPlaceCharsListView.Items.Add(charsInCourt);
                }
            }
        }

        /// <summary>
        /// Creates item for Character list in Court, Tavern, outside keep
        /// </summary>
        /// <param name="ch">Character whose information is to be displayed</param>
        /// <returns>ListViewItem containing character details</returns>
        public ListViewItem createMeetingPlaceListItem(Character ch)
        {
            // name
            ListViewItem myItem = new ListViewItem(ch.firstName + " " + ch.familyName);

            // charID
            myItem.SubItems.Add(ch.charID);

            // sex
            if (ch.isMale)
            {
                myItem.SubItems.Add("Male");
            }
            else
            {
                myItem.SubItems.Add("Female");
            }

            // to store household and type data
            string myHousehold = "";
            string myType = "";
            bool isEmployee = false;
            bool isFamily = false;

            // household
            if (!String.IsNullOrWhiteSpace(ch.familyID))
            {
                myHousehold = ch.getHeadOfFamily().familyName + " (ID: " + ch.familyID + ")";

                if (ch.familyID.Equals(Globals_Client.myPlayerCharacter.charID))
                {
                    isFamily = true;
                }
            }
            else if (!String.IsNullOrWhiteSpace((ch as NonPlayerCharacter).employer))
            {
                myHousehold = (ch as NonPlayerCharacter).getEmployer().familyName + " (ID: " + (ch as NonPlayerCharacter).employer + ")";
            }

            myItem.SubItems.Add(myHousehold);

            // type (e.g. family, NPC, player)

            // check for players and PCs
            if (ch is PlayerCharacter)
            {
                if (!String.IsNullOrWhiteSpace((ch as PlayerCharacter).playerID))
                {
                    myType = "PC (player)";
                }
                else
                {
                    myType = "PC (inactive)";
                }
            }
            else
            {
                // check for employees
                if ((!String.IsNullOrWhiteSpace((ch as NonPlayerCharacter).employer)) && (ch as NonPlayerCharacter).employer.Equals(Globals_Client.myPlayerCharacter.charID))
                {
                    isEmployee = true;
                }

                // allocate NPC type
                if ((isFamily) || (isEmployee))
                {
                    myType = "My ";
                }
                if (isFamily)
                {
                    myType += "Family";
                }
                else if (isEmployee)
                {
                    myType += "Employee";
                }
                else
                {
                    myType = "NPC";
                }
            }

            myItem.SubItems.Add(myType);

            // show whether is in player's entourage
            bool isCompanion = false;

            if (ch is NonPlayerCharacter)
            {
                // iterate through employees checking for character
                for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                {
                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == ch)
                    {
                        if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                        {
                            isCompanion = true;
                        }
                    }
                }
            }

            if (isCompanion)
            {
                myItem.SubItems.Add("Yes");
            }

            return myItem;
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the meetingPlaceCharsListView object,
        /// invoking the displayCharacter method, passing a Character to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void meetingPlaceCharsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Character charToDisplay = null;

            // loop through the characters in the fief
            for (int i = 0; i < Globals_Client.fiefToView.charactersInFief.Count; i++)
            {
                if (meetingPlaceCharsListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (Globals_Client.fiefToView.charactersInFief[i].charID.Equals(this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Client.fiefToView.charactersInFief[i];

                        // check whether is this PC's employee or family
                        if (Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.fiefToView.charactersInFief[i]))
                        {
                            // see if is in entourage to set text of entourage button
                            if ((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).inEntourage)
                            {
                                this.meetingPlaceEntourageBtn.Text = "Remove From Entourage";
                            }
                            else
                            {
                                this.meetingPlaceEntourageBtn.Text = "Add To Entourage";
                            }

                            // enable 'move to' controls
                            this.meetingPlaceMoveToBtn.Enabled = true;
                            this.meetingPlaceMoveToTextBox.Enabled = true;
                            this.meetingPlaceRouteBtn.Enabled = true;
                            this.meetingPlaceRouteTextBox.Enabled = true;
                            this.meetingPlaceEntourageBtn.Enabled = true;

                            // disable marriage proposals
                            this.meetingPlaceProposeBtn.Enabled = false;
                            this.meetingPlaceProposeTextBox.Text = "";
                            this.meetingPlaceProposeTextBox.Enabled = false;

                            // if is employee
                            if ((!String.IsNullOrWhiteSpace((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).employer))
                                && ((Globals_Client.fiefToView.charactersInFief[i] as NonPlayerCharacter).employer.Equals(Globals_Client.myPlayerCharacter.charID)))
                            {
                                // set appropriate text for hire/fire button, and enable it
                                this.hireNPC_Btn.Text = "Fire NPC";
                                this.hireNPC_Btn.Enabled = true;
                                // disable 'salary offer' text box
                                this.hireNPC_TextBox.Visible = false;
                            }
                            else
                            {
                                this.hireNPC_Btn.Enabled = false;
                                this.hireNPC_TextBox.Enabled = false;
                            }
                        }

                        // if is not employee or family
                        else
                        {
                            // set appropriate text for hire/fire controls, and enable them
                            this.hireNPC_Btn.Text = "Hire NPC";
                            this.hireNPC_TextBox.Visible = true;

                            // can only employ men (non-PCs)
                            if (charToDisplay.checkCanHire(Globals_Client.myPlayerCharacter))
                            {
                                this.hireNPC_Btn.Enabled = true;
                                this.hireNPC_TextBox.Enabled = true;
                            }
                            else
                            {
                                this.hireNPC_Btn.Enabled = false;
                                this.hireNPC_TextBox.Enabled = false;
                            }

                            // disable 'move to' and entourage controls
                            this.meetingPlaceMoveToBtn.Enabled = false;
                            this.meetingPlaceMoveToTextBox.Enabled = false;
                            this.meetingPlaceRouteBtn.Enabled = false;
                            this.meetingPlaceRouteTextBox.Enabled = false;
                            this.meetingPlaceEntourageBtn.Enabled = false;

                            // checks for enabling marriage proposals
                            if (((!String.IsNullOrWhiteSpace(charToDisplay.spouse)) || (charToDisplay.isMale)) || (!String.IsNullOrWhiteSpace(charToDisplay.fiancee)))
                            {
                                // disable marriage proposals
                                this.meetingPlaceProposeBtn.Enabled = false;
                                this.meetingPlaceProposeTextBox.Text = "";
                                this.meetingPlaceProposeTextBox.Enabled = false;
                            }
                            else
                            {
                                // enable marriage proposals
                                this.meetingPlaceProposeBtn.Enabled = true;
                                this.meetingPlaceProposeTextBox.Text = Globals_Client.myPlayerCharacter.charID;
                                this.meetingPlaceProposeTextBox.Enabled = true;
                            }
                        }
                    }

                }

            }

            // retrieve and display character information
            if (charToDisplay != null)
            {
                Globals_Client.charToView = charToDisplay;
                string textToDisplay = "";
                textToDisplay += this.displayCharacter(charToDisplay);
                this.meetingPlaceCharDisplayTextBox.ReadOnly = true;
                this.meetingPlaceCharDisplayTextBox.Text = textToDisplay;
            }
        }

        /// <summary>
        /// Responds to the click event of any hireNPC button
        /// invoking either processEmployOffer or fireNPC
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void hireNPC_Btn_Click(object sender, EventArgs e)
        {
            bool amHiring = false;
            bool isHired = false;

            // get hireNPC_Btn tag (shows which meeting place are in)
            string place = Convert.ToString(((Button)sender).Tag);

            // if selected NPC is not a current employee
            if (!Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.charToView))
            {
                amHiring = true;

                try
                {
                    // get offer amount
                    UInt32 newOffer = Convert.ToUInt32(this.hireNPC_TextBox.Text);
                    // submit offer
                    isHired = Globals_Client.myPlayerCharacter.processEmployOffer((Globals_Client.charToView as NonPlayerCharacter), newOffer);

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

            // if selected NPC is already an employee
            else
            {
                // fire NPC
                Globals_Client.myPlayerCharacter.fireNPC(Globals_Client.charToView as NonPlayerCharacter);
            }

            // refresh appropriate screen
            // if firing an NPC
            if (!amHiring)
            {
                if (place.Equals("house"))
                {
                    this.refreshHouseholdDisplay();
                }
                else
                {
                    this.refreshMeetingPlaceDisplay(place);
                }
            }
            // if hiring an NPC
            else
            {
                // if in the tavern and NPC is hired, refresh whole screen (NPC removed from list)
                if (isHired)
                {
                    this.refreshMeetingPlaceDisplay(place);
                }
                else
                {
                    this.meetingPlaceCharDisplayTextBox.Text = this.displayCharacter(Globals_Client.charToView);
                }
            }

        }

        /// <summary>
        /// Responds to the click event of any entourage button
        /// invoking either addToEntourage or removeFromEntourage
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void entourageBtn_Click(object sender, EventArgs e)
        {
            // for messages
            string toDisplay = "";

            // get button
            Button button = sender as Button;

            // check button tag to see on which screen the command occurred
            string whichScreen = button.Tag.ToString();

            // check which action to perform
            // if is in entourage, remove
            if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
            {
                Globals_Client.myPlayerCharacter.removeFromEntourage((Globals_Client.charToView as NonPlayerCharacter));
            }

            // if is not in entourage, add
            else
            {
                // check to see if NPC is army leader
                // if not leader, proceed
                if (String.IsNullOrWhiteSpace(Globals_Client.charToView.armyID))
                {
                    // add to entourage
                    Globals_Client.myPlayerCharacter.addToEntourage((Globals_Client.charToView as NonPlayerCharacter));

                }

                // if is army leader, can't add to entourage
                else
                {
                    toDisplay += "Sorry, milord, this person is an army leader\r\n";
                    toDisplay += "and, therefore, cannot be added to your entourage.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }
            }

            // refresh appropriate screen
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                this.refreshMeetingPlaceDisplay(whichScreen); ;
            }
            else if (whichScreen.Equals("house"))
            {
                this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }
        }
    }
}
