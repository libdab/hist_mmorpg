﻿using System;
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
    /// Partial class for Form1, containing functionality specific to engagement and marriage
    /// </summary>
    partial class Form1
    {

        // ------------------- MARRIAGE

        /// <summary>
        /// Responds to the click event of the houseProposeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void houseProposeBtn_Click(object sender, EventArgs e)
        {
            bool proceed = true;

            Character bride = null;
            Character groom = null;
            string brideID = "";
            string groomID = "";

            if (String.IsNullOrWhiteSpace(this.houseProposeBrideTextBox.Text))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                }
            }
            else if (String.IsNullOrWhiteSpace(this.houseProposeGroomTextBox.Text))
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                }
            }
            else
            {
                // get bride and groom IDs
                brideID = this.houseProposeBrideTextBox.Text;
                groomID = this.houseProposeGroomTextBox.Text;

                // get bride
                if (Globals_Game.npcMasterList.ContainsKey(brideID))
                {
                    bride = Globals_Game.npcMasterList[brideID];
                }

                if (bride == null)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                    }
                }
                else
                {
                    // get groom
                    if (Globals_Game.npcMasterList.ContainsKey(groomID))
                    {
                        groom = Globals_Game.npcMasterList[groomID];
                    }
                    else if (Globals_Game.pcMasterList.ContainsKey(groomID))
                    {
                        groom = Globals_Game.pcMasterList[groomID];
                    }

                    if (groom == null)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                        }
                    }
                    else
                    {
                        // carry out conditional checks
                        proceed = this.checksBeforeProposal(bride, groom);

                        // if checks OK, process proposal
                        if (proceed)
                        {
                            this.proposeMarriage(bride, groom);

                            // refresh screen
                            this.refreshCurrentScreen();
                        }
                    }

                }
            }

        }

        /// <summary>
        /// Allows a character to propose marriage between himself, or a male family member,
        /// and a female family member of another player
        /// </summary>
        /// <returns>bool indicating whether proposal was processed successfully</returns>
        /// <param name="bride">The prospective bride</param>
        /// <param name="groom">The prospective groom</param>
        public bool proposeMarriage(Character bride, Character groom)
        {
            bool success = true;

            // get interested parties
            PlayerCharacter headOfFamilyGroom = Globals_Game.pcMasterList[groom.familyID];
            PlayerCharacter headOfFamilyBride = Globals_Game.pcMasterList[bride.familyID];

            // ID
            uint proposalID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string brideEntry = bride.charID + "|bride";
            string groomEntry = groom.charID + "|groom";
            string[] myProposalPersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, brideEntry, groomEntry };


            // description
            string description = "On this day of Our Lord a proposal has been made by ";
            description += headOfFamilyGroom.firstName + " " + headOfFamilyGroom.familyName + " to ";
            description += headOfFamilyBride.firstName + " " + headOfFamilyBride.familyName + " that ";
            if (headOfFamilyGroomEntry.Equals(groomEntry))
            {
                description += "he";
            }
            else
            {
                description += groom.firstName + " " + groom.familyName;
            }
            description += " be betrothed to " + bride.firstName + " " + bride.familyName;

            // create and send a proposal (journal entry)
            JournalEntry myProposal = new JournalEntry(proposalID, year, season, myProposalPersonae, "proposalMade", descr: description);
            success = Globals_Game.addPastEvent(myProposal);

            return success;
        }

        /// <summary>
        /// Allows a character to reply to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether reply was processed successfully</returns>
        /// <param name="jEntry">The proposal</param>
        /// <param name="proposalAccepted">bool indicating whether proposal accepted</param>
        public bool replyToProposal(JournalEntry jEntry, bool proposalAccepted)
        {
            bool success = true;

            // get interested parties
            PlayerCharacter headOfFamilyBride = null;
            PlayerCharacter headOfFamilyGroom = null;
            Character bride = null;
            Character groom = null;

            for (int i = 0; i < jEntry.personae.Length; i++)
            {
                string thisPersonae = jEntry.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                switch (thisPersonaeSplit[1])
                {
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "bride":
                        bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "groom":
                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        }
                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        }
                        break;
                    default:
                        break;
                }
            }

            // ID
            uint replyID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            List<string> tempPersonae = new List<string>();
            tempPersonae.Add(headOfFamilyBride.charID + "|headOfFamilyBride");
            tempPersonae.Add(headOfFamilyGroom.charID + "|headOfFamilyGroom");
            tempPersonae.Add(bride.charID + "|bride");
            tempPersonae.Add(groom.charID + "|groom");
            if (proposalAccepted)
            {
                tempPersonae.Add("all|all");
            }
            string[] myReplyPersonae = tempPersonae.ToArray();

            // type
            string type = "";
            if (proposalAccepted)
            {
                type = "proposalAccepted";
            }
            else
            {
                type = "proposalRejected";
            }

            // description
            string description = "On this day of Our Lord the proposed marriage between ";
            description += groom.firstName + " " + groom.familyName + " and ";
            description += bride.firstName + " " + bride.familyName + " has been ";
            if (proposalAccepted)
            {
                description += "ACCEPTED";
            }
            else
            {
                description += "REJECTED";
            }
            description += " by " + headOfFamilyBride.firstName + " " + headOfFamilyBride.familyName + ".";
            if (proposalAccepted)
            {
                description += " Let the bells ring out in celebration!";
            }

            // create and send a proposal reply (journal entry)
            JournalEntry myProposalReply = new JournalEntry(replyID, year, season, myReplyPersonae, type, descr: description);
            success = Globals_Game.addPastEvent(myProposalReply);

            if (success)
            {
                // mark proposal as replied
                jEntry.description += "\r\n\r\n** You ";
                if (proposalAccepted)
                {
                    jEntry.description += "ACCEPTED ";
                }
                else
                {
                    jEntry.description += "REJECTED ";
                }
                jEntry.description += "this proposal in " + Globals_Game.clock.seasons[season] + ", " + year;

                // if accepted, process engagement
                if (proposalAccepted)
                {
                    this.processEngagement(myProposalReply);
                }
            }

            // refresh screen
            this.refreshCurrentScreen();

            return success;
        }

        /// <summary>
        /// Processes the actions involved with an engagement
        /// </summary>
        /// <returns>bool indicating whether engagement was processed successfully</returns>
        /// <param name="jEntry">The marriage proposal acceptance</param>
        public bool processEngagement(JournalEntry jEntry)
        {
            bool success = false;

            // get interested parties
            PlayerCharacter headOfFamilyBride = null;
            PlayerCharacter headOfFamilyGroom = null;
            Character bride = null;
            Character groom = null;

            for (int i = 0; i < jEntry.personae.Length; i++)
            {
                string thisPersonae = jEntry.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                switch (thisPersonaeSplit[1])
                {
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "bride":
                        bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "groom":
                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        }
                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        }
                        break;
                    default:
                        break;
                }
            }

            // ID
            uint replyID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;
            if (season == 3)
            {
                season = 0;
                year++;
            }
            else
            {
                season++;
            }

            // personae
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string thisBrideEntry = bride.charID + "|bride";
            string thisGroomEntry = groom.charID + "|groom";
            string[] marriagePersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry };

            // type
            string type = "marriage";

            // create and add a marriage entry to the scheduledEvents journal
            JournalEntry marriageEntry = new JournalEntry(replyID, year, season, marriagePersonae, type);
            success = Globals_Game.addScheduledEvent(marriageEntry);

            // show bride and groom as engaged
            if (success)
            {
                bride.fiancee = groom.charID;
                groom.fiancee = bride.charID;
            }

            return success;
        }

        /// <summary>
        /// Processes the actions involved with a marriage
        /// </summary>
        /// <returns>bool indicating whether engagement was processed successfully</returns>
        /// <param name="jEntry">The marriage proposal acceptance</param>
        public bool processMarriage(JournalEntry jEntry)
        {
            bool success = false;

            // get interested parties
            PlayerCharacter headOfFamilyBride = null;
            PlayerCharacter headOfFamilyGroom = null;
            Character bride = null;
            Character groom = null;

            for (int i = 0; i < jEntry.personae.Length; i++)
            {
                string thisPersonae = jEntry.personae[i];
                string[] thisPersonaeSplit = thisPersonae.Split('|');

                switch (thisPersonaeSplit[1])
                {
                    case "headOfFamilyGroom":
                        headOfFamilyGroom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "headOfFamilyBride":
                        headOfFamilyBride = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "bride":
                        bride = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        break;
                    case "groom":
                        if (Globals_Game.pcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.pcMasterList[thisPersonaeSplit[0]];
                        }
                        else if (Globals_Game.npcMasterList.ContainsKey(thisPersonaeSplit[0]))
                        {
                            groom = Globals_Game.npcMasterList[thisPersonaeSplit[0]];
                        }
                        break;
                    default:
                        break;
                }
            }

            // ID
            uint marriageID = Globals_Game.getNextJournalEntryID();

            // date
            uint year = Globals_Game.clock.currentYear;
            byte season = Globals_Game.clock.currentSeason;

            // personae
            string headOfFamilyBrideEntry = headOfFamilyBride.charID + "|headOfFamilyBride";
            string headOfFamilyGroomEntry = headOfFamilyGroom.charID + "|headOfFamilyGroom";
            string thisBrideEntry = bride.charID + "|bride";
            string thisGroomEntry = groom.charID + "|groom";
            string allEntry = "all|all";
            string[] marriagePersonae = new string[] { headOfFamilyGroomEntry, headOfFamilyBrideEntry, thisBrideEntry, thisGroomEntry, allEntry };

            // type
            string type = "marriage";

            // description
            string description = "On this day of Our Lord there took place a marriage between ";
            description += groom.firstName + " " + groom.familyName + " and ";
            description += bride.firstName + " " + groom.familyName + " (nee " + bride.familyName + ").";
            description += " Let the bells ring out in celebration!";

            // create and add a marriage entry to the pastEvents journal
            JournalEntry marriageEntry = new JournalEntry(marriageID, year, season, marriagePersonae, type, descr: description);
            success = Globals_Game.addPastEvent(marriageEntry);

            if (success)
            {
                // remove fiancees
                bride.fiancee = null;
                groom.fiancee = null;

                // add spouses
                bride.spouse = groom.charID;
                groom.spouse = bride.charID;

                // change wife's family
                bride.familyID = groom.familyID;
                bride.familyName = groom.familyName;

                // switch myNPCs
                headOfFamilyBride.myNPCs.Remove(bride as NonPlayerCharacter);
                headOfFamilyGroom.myNPCs.Add(bride as NonPlayerCharacter);

                // move wife to groom's location
                bride.location = groom.location;

                // check to see if headOfFamilyBride should receive increase in stature
                // get highest rank for headOfFamilyBride and headOfFamilyGroom
                Rank brideHighestRank = headOfFamilyBride.getHighestRank();
                Rank groomHighestRank = headOfFamilyGroom.getHighestRank();

                // compare ranks
                if (groomHighestRank.id < brideHighestRank.id)
                {
                    headOfFamilyBride.adjustStatureModifier((brideHighestRank.id - groomHighestRank.id) * 0.4);
                }
            }

            return success;
        }

        /// <summary>
        /// Implements conditional checks prior to a marriage proposal
        /// </summary>
        /// <returns>bool indicating whether proposal can proceed</returns>
        /// <param name="bride">The prospective bride</param>
        /// <param name="groom">The prospective groom</param>
        public bool checksBeforeProposal(Character bride, Character groom)
        {
            bool proceed = true;
            string message = "";

            // ============= BRIDE
            // check is female
            if (bride.isMale)
            {
                message = "You cannot propose to a man!";
                proceed = false;
            }

            // check is of age
            else
            {
                if (bride.calcAge() < 14)
                {
                    message = "The prospective bride has yet to come of age.";
                    proceed = false;
                }

                else
                {
                    // check isn't engaged
                    if (!String.IsNullOrWhiteSpace(bride.fiancee))
                    {
                        message = "The prospective bride is already engaged.";
                        proceed = false;
                    }

                    else
                    {
                        // check isn't married
                        if (!String.IsNullOrWhiteSpace(bride.spouse))
                        {
                            message = "The prospective bride is already married.";
                            proceed = false;
                        }
                        else
                        {
                            // check is family member of player
                            if ((bride.getHeadOfFamily() == null) || (String.IsNullOrWhiteSpace(bride.getHeadOfFamily().playerID)))
                            {
                                message = "The prospective bride is not of a suitable family.";
                                proceed = false;
                            }
                            else
                            {
                                // ============= GROOM
                                // check is male
                                if (!groom.isMale)
                                {
                                    message = "The proposer must be a man.";
                                    proceed = false;
                                }
                                else
                                {
                                    // check is of age
                                    if (groom.calcAge() < 14)
                                    {
                                        message = "The prospective groom has yet to come of age.";
                                        proceed = false;
                                    }
                                    else
                                    {
                                        // check is unmarried
                                        if (!String.IsNullOrWhiteSpace(groom.spouse))
                                        {
                                            message = "The prospective groom is already married.";
                                            proceed = false;
                                        }
                                        else
                                        {
                                            // check isn't engaged
                                            if (!String.IsNullOrWhiteSpace(groom.fiancee))
                                            {
                                                message = "The prospective groom is already engaged.";
                                                proceed = false;
                                            }
                                            else
                                            {
                                                // check is family member of player OR is player themself
                                                if (String.IsNullOrWhiteSpace(groom.familyID))
                                                {
                                                    message = "The prospective groom is not of a suitable family.";
                                                    proceed = false;
                                                }
                                                else
                                                {
                                                    // check isn't in family same family as bride
                                                    if (groom.familyID.Equals(bride.familyID))
                                                    {
                                                        message = "The prospective bride and groom are in the same family!";
                                                        proceed = false;
                                                    }
                                                }

                                            }

                                        }

                                    }

                                }

                            }

                        }

                    }

                }
            }

            if (!proceed)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(message);
                }
            }

            return proceed;
        }

        /// <summary>
        /// Responds to the click event of the meetingPlaceProposeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void meetingPlaceProposeBtn_Click(object sender, EventArgs e)
        {
            // get entry
            if (this.meetingPlaceCharsListView.SelectedItems.Count > 0)
            {
                bool proceed = true;

                Character bride = null;
                Character groom = null;
                string brideID = "";
                string groomID = "";

                if (this.meetingPlaceProposeTextBox.Text.Trim() == "")
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                    }
                }
                else
                {
                    // get bride and groom IDs
                    brideID = this.meetingPlaceCharsListView.SelectedItems[0].SubItems[1].Text;
                    groomID = this.houseProposeGroomTextBox.Text;

                    // get bride
                    if (Globals_Game.npcMasterList.ContainsKey(brideID))
                    {
                        bride = Globals_Game.npcMasterList[brideID];
                    }

                    if (bride == null)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot identify the prospective bride.");
                        }
                    }
                    else
                    {
                        // get groom
                        if (Globals_Game.npcMasterList.ContainsKey(groomID))
                        {
                            groom = Globals_Game.npcMasterList[groomID];
                        }
                        else if (Globals_Game.pcMasterList.ContainsKey(groomID))
                        {
                            groom = Globals_Game.pcMasterList[groomID];
                        }

                        if (groom == null)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Cannot identify the prospective groom.");
                            }
                        }
                        else
                        {
                            // carry out conditional checks
                            proceed = this.checksBeforeProposal(bride, groom);

                            // if checks OK, process proposal
                            if (proceed)
                            {
                                this.proposeMarriage(bride, groom);
                            }
                        }

                    }
                }
            }

            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Please select a prospective bride.");
                }
            }

        }

        /// <summary>
        /// Responds to the click event of either of the proposal reply buttons,
        /// sending the appropriate reply
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void journalProposalReplyButton_Click(object sender, EventArgs e)
        {
            if (this.journalListView.SelectedItems.Count > 0)
            {
                bool proposalAccepted = false;

                // get tag from button
                Button button = sender as Button;
                string reply = button.Tag.ToString();

                // set appropriate response
                if (reply.Equals("accept"))
                {
                    proposalAccepted = true;
                }

                // get JournalEntry
                JournalEntry thisJentry = Globals_Client.eventSetToView.ElementAt(Globals_Client.jEntryToView).Value;

                // send reply
                this.replyToProposal(thisJentry, proposalAccepted);
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No journal entry selected.");
                }
            }
        }
    }
}