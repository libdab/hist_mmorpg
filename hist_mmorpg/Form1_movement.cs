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
    /// Partial class for Form1, containing functionality specific to movement and the travel screen
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Moves character to target fief
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="ch">Character to move</param>
        /// <param name="target">Target fief</param>
        /// <param name="cost">Travel cost (days)</param>
        /// <param name="siegeCheck">bool indicating whether to check whether the move would end a siege</param>
        public bool moveCharacter(Character ch, Fief target, double cost, bool siegeCheck = true)
        {
            bool success = false;
            bool proceedWithMove = true;

            // check to see if character is leading a besieging army
            if (siegeCheck)
            {
                Army myArmy = ch.getArmy();
                if (myArmy != null)
                {
                    string thisSiegeID = myArmy.checkIfBesieger();
                    if (!String.IsNullOrWhiteSpace(thisSiegeID))
                    {
                        // give player fair warning of consequences to siege
                        DialogResult dialogResult = MessageBox.Show("Your army is currently besieging this fief.  Moving will end the siege.\r\nClick 'OK' to proceed.", "Proceed with move?", MessageBoxButtons.OKCancel);

                        // if choose to cancel
                        if (dialogResult == DialogResult.Cancel)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Move cancelled.");
                            }
                            proceedWithMove = false;
                        }

                        // if choose to proceed
                        else
                        {
                            // end the siege
                            Siege thisSiege = Globals_Game.siegeMasterList[thisSiegeID];
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Siege (" + thisSiegeID + ") ended.");
                            }

                            // construct event description to be passed into siegeEnd
                            string siegeDescription = "On this day of Our Lord the forces of ";
                            siegeDescription += thisSiege.getBesiegingPlayer().firstName + " " + thisSiege.getBesiegingPlayer().familyName;
                            siegeDescription += " have chosen to abandon the siege of " + thisSiege.getFief().name;
                            siegeDescription += ". " + thisSiege.getDefendingPlayer().firstName + " " + thisSiege.getDefendingPlayer().familyName;
                            siegeDescription += " retains ownership of the fief.";

                            this.siegeEnd(thisSiege, false, siegeDescription);
                        }

                    }
                }
            }

            if (proceedWithMove)
            {
                // move character
                success = ch.moveCharacter(target, cost);
            }

            return success;
        }

        /// <summary>
        /// Moves an NPC without a boss one hex in a random direction
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="npc">NPC to move</param>
        public bool randomMoveNPC(NonPlayerCharacter npc)
        {
            bool success = false;

            // generate random int 0-6 to see if moves
            int randomInt = Globals_Game.myRand.Next(7);

            if (randomInt > 0)
            {
                // get a destination
                Fief target = Globals_Game.gameMap.chooseRandomHex(npc.location);

                // get travel cost
                double travelCost = this.getTravelCost(npc.location, target);

                // perform move
                success = this.moveCharacter(npc, target, travelCost);
            }

            return success;
        }

        /// <summary>
        /// Gets travel cost (in days) to move to a fief
        /// </summary>
        /// <returns>double containing travel cost</returns>
        /// <param name="source">Source fief</param>
        /// <param name="target">Target fief</param>
        private double getTravelCost(Fief source, Fief target, string armyID = null)
        {
            double cost = 0;
            // calculate base travel cost based on terrain for both fiefs
            cost = (source.terrain.travelCost + target.terrain.travelCost) / 2;

            // apply season modifier
            cost = cost * Globals_Game.clock.calcSeasonTravMod();

            // if necessary, apply army modifier
            if (!String.IsNullOrWhiteSpace(armyID))
            {
                cost = cost * Globals_Game.armyMasterList[armyID].calcMovementModifier();
            }

            return cost;
        }

        /// <summary>
        /// Refreshes travel display screen
        /// </summary>
        private void refreshTravelContainer()
        {
            // get current fief
            Fief thisFief = Globals_Client.myPlayerCharacter.location;

            // string[] to hold direction text
            string[] directions = new string[] { "NE", "E", "SE", "SW", "W", "NW" };
            // Button[] to hold corresponding travel buttons
            Button[] travelBtns = new Button[] { travel_NE_btn, travel_E_btn, travel_SE_btn, travel_SW_btn, travel_W_btn, travel_NW_btn };

            // get text for home button
            this.travel_Home_btn.Text = "CURRENT FIEF:\r\n\r\n" + thisFief.name + " (" + Globals_Client.myPlayerCharacter.location.id + ")" + "\r\n" + Globals_Client.myPlayerCharacter.location.province.name + ", " + Globals_Client.myPlayerCharacter.location.province.kingdom.name;

            for (int i = 0; i < directions.Length; i++)
            {
                // retrieve target fief for that direction
                Fief target = Globals_Game.gameMap.getFief(thisFief, directions[i]);
                // display fief details and travel cost
                if (target != null)
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\n";
                    travelBtns[i].Text += target.name + " (" + target.id + ")\r\n";
                    travelBtns[i].Text += target.province.name + ", " + target.province.kingdom.name + "\r\n\r\n";
                    travelBtns[i].Text += "Cost: " + this.getTravelCost(thisFief, target);
                }
                else
                {
                    travelBtns[i].Text = directions[i] + " FIEF:\r\n\r\nNo fief present";
                }
            }

            // set text for informational labels
            this.travelLocationLabel.Text = "You are here: " + thisFief.name + " (" + thisFief.id + ")";
            this.travelDaysLabel.Text = "Your remaining days: " + Globals_Client.myPlayerCharacter.days;

            // set text for 'enter/exit keep' button, depending on whether player in/out of keep
            if (Globals_Client.myPlayerCharacter.inKeep)
            {
                this.enterKeepBtn.Text = "Exit Keep";
            }
            else
            {
                this.enterKeepBtn.Text = "Enter Keep";
            }

            // enable all controls
            this.travel_E_btn.Enabled = true;
            this.travel_Home_btn.Enabled = true;
            this.travel_NE_btn.Enabled = true;
            this.travel_NW_btn.Enabled = true;
            this.travel_SE_btn.Enabled = true;
            this.travel_SW_btn.Enabled = true;
            this.travel_W_btn.Enabled = true;
            this.travelCampBtn.Enabled = true;
            this.travelCampDaysTextBox.Enabled = true;
            this.travelExamineArmiesBtn.Enabled = true;
            this.travelMoveToBtn.Enabled = true;
            this.travelMoveToTextBox.Enabled = true;
            this.travelRouteBtn.Enabled = true;
            this.travelRouteTextBox.Enabled = true;
            this.enterKeepBtn.Enabled = true;
            this.listOutsideKeepBtn.Enabled = true;
            this.visitCourtBtn1.Enabled = true;
            this.visitTavernBtn.Enabled = true;

            // clear existing data in TextBoxes
            this.travelMoveToTextBox.Text = "";
            this.travelCampDaysTextBox.Text = "";
            this.travelRouteTextBox.Text = "";

            // check to see if fief is besieged and, if so, disable various controls
            if (!String.IsNullOrWhiteSpace(thisFief.siege))
            {
                // check to see if are inside/outside keep
                if (Globals_Client.myPlayerCharacter.inKeep)
                {
                    // if inside keep, disable all controls except tavern and court
                    this.travel_E_btn.Enabled = false;
                    this.travel_Home_btn.Enabled = false;
                    this.travel_NE_btn.Enabled = false;
                    this.travel_NW_btn.Enabled = false;
                    this.travel_SE_btn.Enabled = false;
                    this.travel_SW_btn.Enabled = false;
                    this.travel_W_btn.Enabled = false;
                    this.travelCampBtn.Enabled = false;
                    this.travelCampDaysTextBox.Enabled = false;
                    this.travelExamineArmiesBtn.Enabled = false;
                    this.travelMoveToBtn.Enabled = false;
                    this.travelMoveToTextBox.Enabled = false;
                    this.travelRouteBtn.Enabled = false;
                    this.travelRouteTextBox.Enabled = false;
                    this.enterKeepBtn.Enabled = false;
                    this.listOutsideKeepBtn.Enabled = false;
                }

                else
                {
                    // if outside keep, disable tavern, court and 'enter keep' but leave all others enabled
                    this.enterKeepBtn.Enabled = false;
                    this.visitCourtBtn1.Enabled = false;
                    this.visitTavernBtn.Enabled = false;
                }
            }

        }

        /// <summary>
        /// Moves character to the target fief, through intervening fiefs (stored in goTo queue)
        /// </summary>
        /// <returns>bool indicating success</returns>
        /// <param name="ch">Character to be moved</param>
        private bool characterMultiMove(Character ch)
        {
            bool success = false;
            double travelCost = 0;
            int steps = ch.goTo.Count;

            for (int i = 0; i < steps; i++)
            {
                // get travel cost
                travelCost = this.getTravelCost(ch.location, ch.goTo.Peek(), ch.armyID);
                // attempt to move character
                success = this.moveCharacter(ch, ch.goTo.Peek(), travelCost);
                // if move successfull, remove fief from goTo queue
                if (success)
                {
                    ch.goTo.Dequeue();
                }
                // if not successfull, exit loop
                else
                {
                    break;
                }
            }

            if (ch == Globals_Client.myPlayerCharacter)
            {
                // if player has moved, indicate success
                if (ch.goTo.Count < steps)
                {
                    success = true;
                }
            }

            return success;

        }

        /// <summary>
        /// Moves a character to a specified fief using the shortest path
        /// </summary>
        /// <param name="whichScreen">String indicating on which screen the movement command occurred</param>
        public void moveTo(string whichScreen)
        {
            // get appropriate TextBox and remove from entourage, if necessary
            TextBox myTextBox = null;
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                myTextBox = this.meetingPlaceMoveToTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseMoveToTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelMoveToTextBox;
            }

            // check for existence of fief
            if (Globals_Game.fiefMasterList.ContainsKey(myTextBox.Text.ToUpper()))
            {
                // retrieves target fief
                Fief target = Globals_Game.fiefMasterList[myTextBox.Text.ToUpper()];

                // obtains goTo queue for shortest path to target
                Globals_Client.charToView.goTo = Globals_Game.gameMap.getShortestPath(Globals_Client.charToView.location, target);

                // if retrieve valid path
                if (Globals_Client.charToView.goTo.Count > 0)
                {
                    // if character is NPC, check entourage and remove if necessary
                    if (!whichScreen.Equals("travel"))
                    {
                        if (Globals_Client.myPlayerCharacter.myNPCs.Contains(Globals_Client.charToView))
                        {
                            (Globals_Client.charToView as NonPlayerCharacter).inEntourage = false;
                        }
                    }

                    // perform move
                    this.characterMultiMove(Globals_Client.charToView);
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
                else if (whichScreen.Equals("travel"))
                {
                    this.refreshTravelContainer();
                }

            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Target fief ID not found.  Please ensure fiefID is valid.");
                }
            }

        }

        /// <summary>
        /// Allows the character to remain in their current location for the specified
        /// number of days, incrementing bailiffDaysInFief if appropriate
        /// </summary>
        /// <param name="ch">The Character who wishes to camp</param>
        /// <param name="campDays">Number of days to camp</param>
        public void campWaitHere(Character ch, byte campDays)
        {
            bool proceed = true;
            // get army
            Army thisArmy = null;
            thisArmy = ch.getArmy();
            // get siege
            Siege thisSiege = null;
            if (thisArmy != null)
            {
                thisSiege = thisArmy.getSiege();
            }

            // check has enough days available
            if (ch.days < (Double)campDays)
            {
                campDays = Convert.ToByte(Math.Truncate(ch.days));
                DialogResult dialogResult = MessageBox.Show("You only have " + campDays + " available.  Click 'OK' to proceed.", "Proceed with camp?", MessageBoxButtons.OKCancel);

                // if choose to cancel
                if (dialogResult == DialogResult.Cancel)
                {
                    proceed = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You decide not to camp after all.");
                    }
                }
            }

            if (proceed)
            {
                // check if player's entourage needs to camp
                bool entourageCamp = false;

                // if character is player, camp entourage
                if (ch is PlayerCharacter)
                {
                    entourageCamp = true;
                }

                // if character NOT player
                else
                {
                    // if is in entourage, remove prior to camping
                    if ((ch as NonPlayerCharacter).inEntourage)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(ch.firstName + " " + ch.familyName + " has been removed from your entourage.");
                        }
                        Globals_Client.myPlayerCharacter.removeFromEntourage((ch as NonPlayerCharacter));
                    }
                }

                // check for siege
                // uses different method to adjust days of all objects involved and apply attrition)
                if (thisSiege != null)
                {
                    thisSiege.syncDays(ch.days - campDays);
                }

                // if no siege
                else
                {
                    // adjust character's days
                    if (ch is PlayerCharacter)
                    {
                        (ch as PlayerCharacter).adjustDays(campDays);
                    }
                    else
                    {
                        ch.adjustDays(campDays);
                    }

                    // inform player
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(ch.firstName + " " + ch.familyName + " remains in " + ch.location.name + " for " + campDays + " days.");
                    }

                    // check if character is army leader, if so check for army attrition
                    if (thisArmy != null)
                    {
                        // number of attrition checks
                        byte attritionChecks = 0;
                        attritionChecks = Convert.ToByte(campDays / 7);
                        // total attrition
                        uint totalAttrition = 0;

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            // calculate attrition
                            double attritionModifer = thisArmy.calcAttrition();
                            // apply attrition
                            if (attritionModifer > 0)
                            {
                                totalAttrition += thisArmy.applyTroopLosses(attritionModifer);
                            }
                        }

                        // inform player
                        if (totalAttrition > 0)
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Army (" + thisArmy.armyID + ") lost " + totalAttrition + " troops due to attrition.");
                            }
                        }
                    }
                }

                // keep track of bailiffDaysInFief before any possible increment
                Double bailiffDaysBefore = ch.location.bailiffDaysInFief;

                // keep track of identity of bailiff
                Character myBailiff = null;

                // check if character is bailiff of this fief
                if (ch.location.bailiff == ch)
                {
                    myBailiff = ch;
                }

                // if character not bailiff, if appropriate, check to see if anyone in entourage is
                else if (entourageCamp)
                {
                    // if player is bailiff
                    if (Globals_Client.myPlayerCharacter == ch.location.bailiff)
                    {
                        myBailiff = Globals_Client.myPlayerCharacter;
                    }
                    // if not, check for bailiff in entourage
                    else
                    {
                        for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                        {
                            if (Globals_Client.myPlayerCharacter.myNPCs[i].inEntourage)
                            {
                                if (Globals_Client.myPlayerCharacter.myNPCs[i] != ch)
                                {
                                    if (Globals_Client.myPlayerCharacter.myNPCs[i] == ch.location.bailiff)
                                    {
                                        myBailiff = Globals_Client.myPlayerCharacter.myNPCs[i];
                                    }
                                }
                            }
                        }
                    }

                }

                // if bailiff identified as someone who camped
                if (myBailiff != null)
                {
                    // increment bailiffDaysInFief
                    ch.location.bailiffDaysInFief += campDays;
                    // if necessary, display message to player
                    if (ch.location.bailiffDaysInFief >= 30)
                    {
                        // don't display this message if min bailiffDaysInFief was already achieved
                        if (!(bailiffDaysBefore >= 30))
                        {
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show(myBailiff.firstName + " " + myBailiff.familyName + " has fulfilled his bailiff duties in " + ch.location.name + ".");
                            }
                        }
                    }
                }
            }

            // refresh display
            if (proceed)
            {
                if (ch == Globals_Client.myPlayerCharacter)
                {
                    this.refreshTravelContainer();
                }
                else
                {
                    this.refreshHouseholdDisplay((Globals_Client.charToView as NonPlayerCharacter));
                }
            }

        }

        /// <summary>
        /// Allows a character to be moved along a specific route by using direction codes
        /// </summary>
        /// <param name="whichScreen">String indicating on which screen the movement command occurred</param>
        public void takeThisRoute(string whichScreen)
        {
            bool proceed;
            Fief source = null;
            Fief target = null;
            Queue<Fief> route = new Queue<Fief>();

            // get appropriate TextBox and remove from entourage, if necessary
            TextBox myTextBox = null;
            if ((whichScreen.Equals("tavern")) || (whichScreen.Equals("outsideKeep")) || (whichScreen.Equals("court")))
            {
                myTextBox = this.meetingPlaceRouteTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("house"))
            {
                myTextBox = this.houseRouteTextBox;
                if ((Globals_Client.charToView as NonPlayerCharacter).inEntourage)
                {
                    Globals_Client.myPlayerCharacter.removeFromEntourage(Globals_Client.charToView as NonPlayerCharacter);
                }
            }
            else if (whichScreen.Equals("travel"))
            {
                myTextBox = this.travelRouteTextBox;
            }

            // get list of directions
            string[] directions = myTextBox.Text.Split(',').ToArray<string>();

            // convert to Queue of fiefs
            for (int i = 0; i < directions.Length; i++)
            {
                // source for first move is character's current location
                if (i == 0)
                {
                    source = Globals_Client.charToView.location;
                }
                // source for all other moves is the previous target fief
                else
                {
                    source = target;
                }

                // get the target fief
                target = Globals_Game.gameMap.getFief(source, directions[i].ToUpper());

                // if target successfully acquired, add to queue
                if (target != null)
                {
                    route.Enqueue(target);
                }
                // if no target acquired, display message and break
                else
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Invalid direction code encountered.  Route halted at " + source.name + " (" + source.id + ")");
                    }
                    break;
                }

            }

            // if there are any fiefs in the queue, overwrite the character's goTo queue
            // then process by calling characterMultiMove
            if (route.Count > 0)
            {
                Globals_Client.charToView.goTo = route;
                proceed = this.characterMultiMove(Globals_Client.charToView);
            }

            // refresh appropriate screen
            this.refreshCurrentScreen();
        }
    }
}
