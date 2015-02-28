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
    /// Partial class for Form1, containing functionality specific to army management
    /// </summary>
    partial class Form1
    {
        /*
        /// <summary>
        /// Retrieves information for Army display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="a">Army for which information is to be displayed</param>
        public string DisplayArmyData(Army a)
        {
            string armyText = "";
            uint[] troopNumbers = a.troops;
            Fief armyLocation = a.GetLocation();

            // check if is garrison in a siege
            string siegeID = a.CheckIfSiegeDefenderGarrison();
            if (String.IsNullOrWhiteSpace(siegeID))
            {
                // check if is additional defender in a siege
                siegeID = a.CheckIfSiegeDefenderAdditional();
            }

            // if is defender in a siege, indicate
            if (!String.IsNullOrWhiteSpace(siegeID))
            {
                armyText += "NOTE: This army is currently UNDER SIEGE\r\n\r\n";
            }

            else
            {
                // check if is besieger in a siege
                siegeID = a.CheckIfBesieger();

                // if is besieger in a siege, indicate
                if (!String.IsNullOrWhiteSpace(siegeID))
                {
                    armyText += "NOTE: This army is currently BESIEGING THIS FIEF\r\n\r\n";
                }

                // check if is siege in fief (but army not involved)
                else
                {
                    if (!String.IsNullOrWhiteSpace(armyLocation.siege))
                    {
                        armyText += "NOTE: This fief is currently UNDER SIEGE\r\n\r\n";
                    }
                }
            }

            // ID
            armyText += "ID: " + a.armyID + "\r\n\r\n";

            // nationality
            armyText += "Nationality: " + a.GetOwner().nationality.name + "\r\n\r\n";

            // days left
            armyText += "Days left: " + a.days + "\r\n\r\n";

            // location
            armyText += "Location: " + armyLocation.name + " (Province: " + armyLocation.province.name + ".  Kingdom: " + armyLocation.province.kingdom.name + ")\r\n\r\n";

            // leader
            Character armyLeader = a.GetLeader();

            armyText += "Leader: ";

            if (armyLeader == null)
            {
                armyText += "THIS ARMY HAS NO LEADER!  You should appoint one as soon as possible.";
            }
            else
            {
                armyText += armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")";
            }
            armyText += "\r\n\r\n";

            // labels for troop types
            string[] troopTypeLabels = new string[] { " - Knights: ", " - Men-at-Arms: ", " - Light Cavalry: ", " - Longbowmen: ", " - Crossbowmen: ", " - Foot: ", " - Rabble: " };

            // display numbers for each troop type
            for (int i = 0; i < troopNumbers.Length; i++)
            {
                armyText += troopTypeLabels[i] + troopNumbers[i];
                armyText += "\r\n";
            }
            armyText += "   ==================\r\n";
            armyText += " - TOTAL: " + a.CalcArmySize() + "\r\n\r\n";

            // whether is maintained (and at what cost)
            if (a.isMaintained)
            {
                uint armyCost = a.CalcArmySize() * 500;

                armyText += "This army is currently being maintained (at a cost of £" + armyCost + ")\r\n\r\n";
            }
            else
            {
                armyText += "This army is NOT currently being maintained\r\n\r\n";
            }

            // aggression level
            armyText += "Aggression level: " + a.aggression + "\r\n\r\n";

            // sally value
            armyText += "Sally value: " + a.combatOdds + "\r\n\r\n";

            return armyText;
        } */

        /// <summary>
        /// Refreshes main Army display screen
        /// </summary>
        /// <param name="a">Army whose information is to be displayed</param>
        public void RefreshArmyContainer(Army a = null)
        {
            // disable controls until army selected
            this.DisableControls(this.armyManagementPanel);
            this.DisableControls(this.armyCombatPanel);

            // always enable switch between management and combat panels
            this.armyDisplayCmbtBtn.Enabled = true;
            this.armyDisplayMgtBtn.Enabled = true;

            // ensure main textbox is empty and isn't interactive
            this.armyTextBox.Text = "";
            this.armyTextBox.ReadOnly = true;

            // clear existing items in armies list
            this.armyListView.Items.Clear();

            // iterates through player's armies adding information to ListView
            for (int i = 0; i < Globals_Client.myPlayerCharacter.myArmies.Count; i++)
            {
                ListViewItem thisArmy = null;

                // armyID
                thisArmy = new ListViewItem(Globals_Client.myPlayerCharacter.myArmies[i].armyID);

                // leader
                Character armyLeader = Globals_Client.myPlayerCharacter.myArmies[i].GetLeader();
                if (armyLeader != null)
                {
                    thisArmy.SubItems.Add(armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")");
                }
                else
                {
                    thisArmy.SubItems.Add("No leader");
                }

                // location
                Fief armyLocation = Globals_Client.myPlayerCharacter.myArmies[i].GetLocation();
                thisArmy.SubItems.Add(armyLocation.name + " (" + armyLocation.id + ")");

                // size
                thisArmy.SubItems.Add(Globals_Client.myPlayerCharacter.myArmies[i].CalcArmySize().ToString());

                if (thisArmy != null)
                {
                    // if army passed in as parameter, show as selected
                    if (Globals_Client.myPlayerCharacter.myArmies[i] == a)
                    {
                        thisArmy.Selected = true;
                    }

                    // add item to armyListView
                    this.armyListView.Items.Add(thisArmy);
                }

            }

            if (a == null)
            {
                // if player is not leading any armies, set button text to 'recruit new' and enable
                if (String.IsNullOrWhiteSpace(Globals_Client.myPlayerCharacter.armyID))
                {
                    this.armyRecruitBtn.Text = "Recruit a New Army In Current Fief";
                    this.armyRecruitBtn.Tag = "new";
                    this.armyRecruitBtn.Enabled = true;
                    this.armyRecruitTextBox.Enabled = true;
                }
            }

            Globals_Client.containerToView = this.armyContainer;
            Globals_Client.containerToView.BringToFront();

            // check which panel to display
            string armyPanelTag = this.armyContainer.Panel1.Tag.ToString();
            if (armyPanelTag.Equals("combat"))
            {
                this.armyCombatPanel.BringToFront();
            }
            else
            {
                this.armyManagementPanel.BringToFront();
            }

            this.armyListView.Focus();
        }

        /// <summary>
        /// Disbands the specified army
        /// </summary>
        /// <param name="a">Army to be disbanded</param>
        public void DisbandArmy(Army a)
        {
            // carry out functions associated with disband
            a.DisbandArmy();

            // set army to null
            a = null;
        }

        /// <summary>
        /// Retrieves details of all armies in observer's current fief
        /// </summary>
        /// <param name="observer">The observer</param>
        private void ExamineArmiesInFief(Character observer)
        {
            bool proceed = observer.ChecksBefore_ExamineArmies();

            // refresh display to show amended observer days
            if (Globals_Client.containerToView == this.armyContainer)
            {
                this.RefreshArmyContainer(Globals_Client.armyToView);
            }
            else if (Globals_Client.containerToView == this.houseContainer)
            {
                this.RefreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
            }
            else if (Globals_Client.containerToView == this.travelContainer)
            {
                this.RefreshTravelContainer();
            }

            if (proceed)
            {
                // check for previously opened SelectionForm and close if necessary
                if (Application.OpenForms.OfType<SelectionForm>().Any())
                {
                    Application.OpenForms.OfType<SelectionForm>().First().Close();
                }

                // display armies list
                SelectionForm examineArmies = new SelectionForm(this, "armies", obs: observer);
                examineArmies.Show();
            }


        }
    }
}