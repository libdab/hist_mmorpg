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
    /// Partial class for Form1, containing functionality specific to fief management
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Responds to the Click event of the fiefsChallengeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsChallengeBtn_Click(object sender, EventArgs e)
        {
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                // get province
                Province targetProv = Globals_Game.provinceMasterList[this.fiefsListView.SelectedItems[0].SubItems[5].Text];

                // ensure aren't current owner
                if (Globals_Client.myPlayerCharacter == targetProv.owner)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You already own " + targetProv.name + "!");
                    }
                }

                // legitimate challenge
                else
                {
                    // create and send new OwnershipChallenge
                    OwnershipChallenge newChallenge = new OwnershipChallenge(Globals_Game.getNextOwnChallengeID(), Globals_Client.myPlayerCharacter.charID, "province", targetProv.id);
                    Globals_Game.addOwnershipChallenge(newChallenge);

                    // create and send journal entry
                    // get interested parties
                    PlayerCharacter currentOwner = targetProv.owner;

                    // ID
                    uint entryID = Globals_Game.getNextJournalEntryID();

                    // date
                    uint year = Globals_Game.clock.currentYear;
                    byte season = Globals_Game.clock.currentSeason;

                    // location
                    string entryLoc = targetProv.id;

                    // journal entry personae
                    string allEntry = "all|all";
                    string currentOwnerEntry = currentOwner.charID + "|owner";
                    string challengerEntry = Globals_Client.myPlayerCharacter.charID + "|challenger";
                    string[] entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                    // entry type
                    string entryType = "ownershipChallenge_new";

                    // journal entry description
                    string description = "On this day of Our Lord a challenge for the ownership of " + targetProv.name + " (" + targetProv.id + ")";
                    description += " has COMMENCED.  " + Globals_Client.myPlayerCharacter.firstName + " " + Globals_Client.myPlayerCharacter.familyName + " seeks to rest ownership from ";
                    description += "the current owner, " + currentOwner.firstName + " " + currentOwner.familyName + ".";

                    // create and send a proposal (journal entry)
                    JournalEntry myEntry = new JournalEntry(entryID, year, season, entryPersonae, entryType, descr: description, loc: entryLoc);
                    Globals_Game.addPastEvent(myEntry);
                }

                this.fiefsListView.Focus();
            }
        }

        /// <summary>
        /// Responds to the Click event of the fiefsViewBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsViewBtn_Click(object sender, EventArgs e)
        {
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                // get fief to view
                Globals_Client.fiefToView = Globals_Game.fiefMasterList[this.fiefsListView.SelectedItems[0].SubItems[1].Text];

                // go to fief display screen
                this.refreshFiefContainer(Globals_Client.fiefToView);
                Globals_Client.containerToView = this.fiefContainer;
                Globals_Client.containerToView.BringToFront();
            }
        }

        /// <summary>
        /// Creates UI display for PlayerCharacter's list of owned Fiefs
        /// </summary>
        public void setUpFiefsList()
        {
            // add necessary columns
            this.fiefsListView.Columns.Add("Fief Name", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Where am I?", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Home Fief?", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Province Name", -2, HorizontalAlignment.Left);
            this.fiefsListView.Columns.Add("Province ID", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes display of PlayerCharacter's list of owned Fiefs
        /// </summary>
        public void refreshMyFiefs()
        {
            // clear existing items in list
            this.fiefsListView.Items.Clear();

            // disable controls until fief selected
            this.disableControls(this.fiefsOwnedContainer.Panel1);

            ListViewItem[] fiefsOwned = new ListViewItem[Globals_Client.myPlayerCharacter.ownedFiefs.Count];
            // iterates through fiefsOwned
            for (int i = 0; i < Globals_Client.myPlayerCharacter.ownedFiefs.Count; i++)
            {
                // Create an item and subitem for each fief
                // name
                fiefsOwned[i] = new ListViewItem(Globals_Client.myPlayerCharacter.ownedFiefs[i].name);

                // ID
                fiefsOwned[i].SubItems.Add(Globals_Client.myPlayerCharacter.ownedFiefs[i].id);

                // current location
                if (Globals_Client.myPlayerCharacter.ownedFiefs[i] == Globals_Client.myPlayerCharacter.location)
                {
                    fiefsOwned[i].SubItems.Add("You are here");
                }
                else
                {
                    fiefsOwned[i].SubItems.Add("");
                }

                // home fief
                if (Globals_Client.myPlayerCharacter.ownedFiefs[i].id.Equals(Globals_Client.myPlayerCharacter.homeFief))
                {
                    fiefsOwned[i].SubItems.Add("Home");
                }
                else
                {
                    fiefsOwned[i].SubItems.Add("");
                }

                // province name
                fiefsOwned[i].SubItems.Add(Globals_Client.myPlayerCharacter.ownedFiefs[i].province.name);

                // province ID
                fiefsOwned[i].SubItems.Add(Globals_Client.myPlayerCharacter.ownedFiefs[i].province.id);

                // add item to fiefsListView
                this.fiefsListView.Items.Add(fiefsOwned[i]);
            }
        }

        /// <summary>
        /// Retrieves general information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        /// <param name="isOwner">bool indicating if fief owned by player</param>
        public string displayFiefGeneralData(Fief f, bool isOwner)
        {
            string fiefText = "";

            // ID
            fiefText += "ID: " + f.id + "\r\n";

            // name (& province name)
            fiefText += "Name: " + f.name + " (Province: " + f.province.name + ".  Kingdom: " + f.province.kingdom.name + ")\r\n";

            // rank
            fiefText += "Title (rank): ";
            fiefText += f.rank.getName(f.language) + " (" + f.rank.id + ")\r\n";

            // population
            fiefText += "Population: " + f.population + "\r\n";

            // fields
            fiefText += "Fields level: " + f.fields + "\r\n";

            // industry
            fiefText += "Industry level: " + f.industry + "\r\n";

            // owner's ID
            fiefText += "Owner (ID): " + f.owner.charID + "\r\n";

            // ancestral owner's ID
            fiefText += "Ancestral owner (ID): " + f.ancestralOwner.charID + "\r\n";

            // bailiff's ID
            fiefText += "Bailiff (ID): ";
            if (f.bailiff != null)
            {
                fiefText += f.bailiff.charID;
            }
            else
            {
                fiefText += "auto-bailiff";
            }
            fiefText += "\r\n";

            // no. of troops (only if owned)
            if (isOwner)
            {
                fiefText += "Garrison: " + Convert.ToInt32(f.keyStatsCurrent[4] / 1000) + " troops\r\n";
                fiefText += "Militia: Up to " + f.calcMaxTroops() + " troops are available for call up in this fief\r\n";
            }

            // fief status
            fiefText += "Status: ";
            // if under siege, replace status with siege
            if (!String.IsNullOrWhiteSpace(f.siege))
            {
                fiefText += "UNDER SIEGE!";
            }
            else
            {
                switch (f.status)
                {
                    case 'U':
                        fiefText += "Unrest";
                        break;
                    case 'R':
                        fiefText += "Rebellion!";
                        break;
                    default:
                        fiefText += "Calm";
                        break;
                }
            }

            fiefText += "\r\n";

            // language
            fiefText += "Language: " + f.language.getName() + "\r\n";

            // terrain type
            fiefText += "Terrain: " + f.terrain.description + "\r\n";

            // barred nationalities
            fiefText += "Barred nationalities: ";
            if (f.barredNationalities.Count > 0)
            {
                // get last entry
                string lastNatID = f.barredNationalities.Last();

                foreach (string natID in f.barredNationalities)
                {
                    // get nationality
                    Nationality thisNat = null;
                    if (Globals_Game.nationalityMasterList.ContainsKey(natID))
                    {
                        thisNat = Globals_Game.nationalityMasterList[natID];
                    }

                    if (thisNat != null)
                    {
                        fiefText += thisNat.name;

                        if (!natID.Equals(lastNatID))
                        {
                            fiefText += ", ";
                        }
                    }
                }
            }
            else
            {
                fiefText += "None";
            }
            fiefText += "\r\n";

            // barred characters
            fiefText += "Barred characters: ";
            if (f.barredCharacters.Count > 0)
            {
                // get last entry
                string lastCharID = f.barredCharacters.Last();

                foreach (string charID in f.barredCharacters)
                {
                    // get nationality
                    Character thisChar = null;
                    if (Globals_Game.npcMasterList.ContainsKey(charID))
                    {
                        thisChar = Globals_Game.npcMasterList[charID];
                    }
                    else if (Globals_Game.pcMasterList.ContainsKey(charID))
                    {
                        thisChar = Globals_Game.pcMasterList[charID];
                    }

                    if (thisChar != null)
                    {
                        fiefText += thisChar.firstName + " " + thisChar.familyName;

                        if (!charID.Equals(lastCharID))
                        {
                            fiefText += ", ";
                        }
                    }
                }
            }
            else
            {
                fiefText += "None";
            }
            fiefText += "\r\n";

            return fiefText;
        }

        /// <summary>
        /// Retrieves previous season's key information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        public string displayFiefKeyStatsPrev(Fief f)
        {
            bool displayData = true;

            string fiefText = "PREVIOUS SEASON\r\n=================\r\n\r\n";

            // if under siege, check to see if display data (based on siege start date)
            if (!String.IsNullOrWhiteSpace(f.siege))
            {
                Siege thisSiege = f.getSiege();
                displayData = this.checkToShowFinancialData(-1, thisSiege);
            }

            // if not OK to display data, show message
            if (!displayData)
            {
                fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
            }
            // if is OK, display as normal
            else
            {
                // loyalty
                fiefText += "Loyalty: " + f.keyStatsPrevious[0] + "\r\n\r\n";

                // GDP
                fiefText += "GDP: " + f.keyStatsPrevious[1] + "\r\n\r\n";

                // tax rate
                fiefText += "Tax rate: " + f.keyStatsPrevious[2] + "%\r\n\r\n";

                // officials spend
                fiefText += "Officials expenditure: " + f.keyStatsPrevious[3] + "\r\n\r\n";

                // garrison spend
                fiefText += "Garrison expenditure: " + f.keyStatsPrevious[4] + "\r\n\r\n";

                // infrastructure spend
                fiefText += "Infrastructure expenditure: " + f.keyStatsPrevious[5] + "\r\n\r\n";

                // keep spend
                fiefText += "Keep expenditure: " + f.keyStatsPrevious[6] + "\r\n";
                // keep level
                fiefText += "   (Keep level: " + f.keyStatsPrevious[7] + ")\r\n\r\n";

                // income
                fiefText += "Income: " + f.keyStatsPrevious[8] + "\r\n\r\n";

                // family expenses
                fiefText += "Family expenses: " + f.keyStatsPrevious[9] + "\r\n\r\n";

                // total expenses
                fiefText += "Total fief expenses: " + f.keyStatsPrevious[10] + "\r\n\r\n";

                // overlord taxes
                fiefText += "Overlord taxes: " + f.keyStatsPrevious[11] + "\r\n";
                // overlord tax rate
                fiefText += "   (tax rate: " + f.keyStatsPrevious[12] + "%)\r\n\r\n";

                // surplus
                fiefText += "Bottom line: " + f.keyStatsPrevious[13];
            }

            return fiefText;
        }

        /// <summary>
        /// Retrieves current season's key information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        public string displayFiefKeyStatsCurr(Fief f)
        {
            bool displayData = true;

            string fiefText = "CURRENT SEASON\r\n=================\r\n\r\n";

            // if under siege, check to see if display data (based on siege start date)
            if (!String.IsNullOrWhiteSpace(f.siege))
            {
                Siege thisSiege = f.getSiege();
                displayData = this.checkToShowFinancialData(0, thisSiege);
            }

            // if not OK to display data, show message
            if (!displayData)
            {
                fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
            }
            // if is OK, display as normal
            else
            {
                // loyalty
                fiefText += "Loyalty: " + f.keyStatsCurrent[0] + "\r\n\r\n";

                // GDP
                fiefText += "GDP: " + f.keyStatsCurrent[1] + "\r\n\r\n";

                // tax rate
                fiefText += "Tax rate: " + f.keyStatsCurrent[2] + "%\r\n\r\n";

                // officials spend
                fiefText += "Officials expenditure: " + f.keyStatsCurrent[3] + "\r\n\r\n";

                // garrison spend
                fiefText += "Garrison expenditure: " + f.keyStatsCurrent[4] + "\r\n\r\n";

                // infrastructure spend
                fiefText += "Infrastructure expenditure: " + f.keyStatsCurrent[5] + "\r\n\r\n";

                // keep spend
                fiefText += "Keep expenditure: " + f.keyStatsCurrent[6] + "\r\n";
                // keep level
                fiefText += "   (Keep level: " + f.keyStatsCurrent[7] + ")\r\n\r\n";

                // income
                fiefText += "Income: " + f.keyStatsCurrent[8] + "\r\n\r\n";

                // family expenses
                fiefText += "Family expenses: " + f.keyStatsCurrent[9] + "\r\n\r\n";

                // total expenses
                fiefText += "Total fief expenses: " + f.keyStatsCurrent[10] + "\r\n\r\n";

                // overlord taxes
                fiefText += "Overlord taxes: " + f.keyStatsCurrent[11] + "\r\n";
                // overlord tax rate
                fiefText += "   (tax rate: " + f.keyStatsCurrent[12] + "%)\r\n\r\n";

                // surplus
                fiefText += "Bottom line: " + f.keyStatsCurrent[13];
            }

            return fiefText;
        }

        /// <summary>
        /// Retrieves next season's key information for Fief display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="f">Fief for which information is to be displayed</param>
        public string displayFiefKeyStatsNext(Fief f)
        {
            string fiefText = "NEXT SEASON (ESTIMATE)\r\n========================\r\n\r\n";

            // if under siege, don't display data
            if (!String.IsNullOrWhiteSpace(f.siege))
            {
                fiefText += "CURRENTLY UNAVAILABLE - due to siege\r\n";
            }

            // if NOT under siege
            else
            {
                // loyalty
                fiefText += "Loyalty: " + f.calcNewLoyalty() + "\r\n";
                // various loyalty modifiers
                fiefText += "  (including Officials spend loyalty modifier: " + f.calcOffLoyMod() + ")\r\n";
                fiefText += "  (including Garrison spend loyalty modifier: " + f.calcGarrLoyMod() + ")\r\n";
                fiefText += "  (including Bailiff loyalty modifier: " + f.calcBlfLoyAdjusted(f.bailiffDaysInFief >= 30) + ")\r\n";
                fiefText += "    (which itself may include a Bailiff fiefLoy skills modifier: " + f.calcBailLoySkillMod(f.bailiffDaysInFief >= 30) + ")\r\n\r\n";

                // GDP
                fiefText += "GDP: " + f.calcNewGDP() + "\r\n\r\n";

                // tax rate
                fiefText += "Tax rate: " + f.taxRateNext + "%\r\n\r\n";

                // officials expenditure
                fiefText += "Officials expenditure: " + f.officialsSpendNext + "\r\n\r\n";

                // Garrison expenditure
                fiefText += "Garrison expenditure: " + f.garrisonSpendNext + "\r\n\r\n";

                // Infrastructure expenditure
                fiefText += "Infrastructure expenditure: " + f.infrastructureSpendNext + "\r\n\r\n";

                // keep expenditure
                fiefText += "Keep expenditure: " + f.keepSpendNext + "\r\n";
                // keep level
                fiefText += "   (keep level: " + f.calcNewKeepLevel() + ")\r\n\r\n";

                // income
                fiefText += "Income: " + f.calcNewIncome() + "\r\n";
                // various income modifiers
                fiefText += "  (including Bailiff income modifier: " + f.calcBlfIncMod(f.bailiffDaysInFief >= 30) + ")\r\n";
                fiefText += "  (including Officials spend income modifier: " + f.calcOffIncMod() + ")\r\n\r\n";

                // family expenses
                fiefText += "Family expenses: " + f.calcFamilyExpenses() + "\r\n";
                // famExpenses modifier for player/spouse
                if (!String.IsNullOrWhiteSpace(f.owner.spouse))
                {
                    if (f.owner.getSpouse().management > f.owner.management)
                    {
                        fiefText += "  (which may include a famExpense skills modifier: " + Globals_Game.npcMasterList[f.owner.spouse].calcSkillEffect("famExpense") + ")";
                    }
                }
                else
                {
                    fiefText += "  (which may include a famExpense skills modifier: " + f.owner.calcSkillEffect("famExpense") + ")";
                }
                fiefText += "\r\n\r\n";

                // total expenses (fief and family)
                fiefText += "Total fief expenses: " + (f.calcNewExpenses() + f.calcFamilyExpenses()) + "\r\n";
                // bailiff fief expenses modifier
                fiefText += "  (which may include a Bailiff fiefExpense skills modifier: " + f.calcBailExpModif(f.bailiffDaysInFief >= 30) + ")\r\n\r\n";

                // overlord taxes
                fiefText += "Overlord taxes: " + f.calcNewOlordTaxes() + "\r\n";
                // overlord tax rate
                fiefText += "   (tax rate: " + f.province.taxRate + "%)\r\n\r\n";

                // bottom line
                fiefText += "Bottom line: " + f.calcNewBottomLine();
            }

            return fiefText;
        }

        /// <summary>
        /// Refreshes main Fief display screen
        /// </summary>
        /// <param name="f">Fief whose information is to be displayed</param>
        public void refreshFiefContainer(Fief f = null)
        {
            // if fief not specified, default to player's current location
            if (f == null)
            {
                f = Globals_Client.myPlayerCharacter.location;
            }

            Globals_Client.fiefToView = f;

            bool isOwner = Globals_Client.myPlayerCharacter.ownedFiefs.Contains(Globals_Client.fiefToView);
            bool displayWarning = false;
            string toDisplay = "";

            // set name label text
            this.fiefLabel.Text = Globals_Client.fiefToView.name + " (" + Globals_Client.fiefToView.id + ")";
            // set siege label text
            if (!String.IsNullOrWhiteSpace(f.siege))
            {
                this.fiefSiegeLabel.Text = "Fief under siege";
            }
            else
            {
                this.fiefSiegeLabel.Text = "";
            }

            // refresh main fief TextBox with updated info
            this.fiefTextBox.Text = this.displayFiefGeneralData(Globals_Client.fiefToView, isOwner);

            // ensure textboxes aren't interactive
            this.fiefTextBox.ReadOnly = true;
            this.fiefPrevKeyStatsTextBox.ReadOnly = true;
            this.fiefCurrKeyStatsTextBox.ReadOnly = true;
            this.fiefNextKeyStatsTextBox.ReadOnly = true;
            this.fiefTransferAmountTextBox.Text = "";

            // if fief is NOT owned by player, disable fief management buttons and TextBoxes 
            if (!isOwner)
            {
                this.disableControls(this.fiefContainer.Panel1);
                this.disableControls(this.fiefFinanceContainer1.Panel1);
                this.disableControls(this.fiefFinanceContainer2.Panel1);
                this.disableControls(this.fiefFinanceContainer2.Panel2);
            }

            // if fief IS owned by player, enable fief management buttons and TextBoxes 
            else
            {
                // get home fief
                Fief home = Globals_Client.myPlayerCharacter.getHomeFief();

                // get home treasury
                int homeTreasury = 0;
                if (f == home)
                {
                    homeTreasury = home.getAvailableTreasury();
                }
                else
                {
                    homeTreasury = home.getAvailableTreasury(true);
                }

                // get this fief's treasury
                int fiefTreasury = f.getAvailableTreasury(); ;

                // if fief UNDER SIEGE, leave most controls disabled
                if (!String.IsNullOrWhiteSpace(f.siege))
                {
                    // allow view bailiff
                    this.viewBailiffBtn.Enabled = true;

                    // allow financial data TextBoxes to show appropriate data
                    this.fiefPrevKeyStatsTextBox.Text = this.displayFiefKeyStatsPrev(Globals_Client.fiefToView);
                    this.fiefCurrKeyStatsTextBox.Text = this.displayFiefKeyStatsCurr(Globals_Client.fiefToView);
                    this.fiefNextKeyStatsTextBox.Text = this.displayFiefKeyStatsNext(Globals_Client.fiefToView);
                }

                // if NOT under siege, enable usual controls
                else
                {
                    this.enableControls(this.fiefContainer.Panel1);
                    this.fiefPrevKeyStatsTextBox.Enabled = true;
                    this.fiefCurrKeyStatsTextBox.Enabled = true;
                    this.fiefNextKeyStatsTextBox.Enabled = true;

                    // don't enable 'appoint self' button if you're already the bailiff
                    if (f.bailiff == Globals_Client.myPlayerCharacter)
                    {
                        this.selfBailiffBtn.Enabled = false;
                    }
                    else
                    {
                        this.selfBailiffBtn.Enabled = true;
                    }

                    // don't enable treasury transfer controls if in Home Fief (can't transfer to self)
                    if (f == home)
                    {
                        this.fiefTransferToFiefBtn.Enabled = false;
                        this.fiefTransferToHomeBtn.Enabled = false;
                        this.fiefTransferAmountTextBox.Enabled = false;
                        this.FiefTreasTextBox.Enabled = false;
                        this.FiefTreasTextBox.Text = "";
                    }
                    else
                    {
                        this.fiefTransferToFiefBtn.Enabled = true;
                        this.fiefTransferToHomeBtn.Enabled = true;
                        this.fiefTransferAmountTextBox.Enabled = true;
                        this.fiefHomeTreasTextBox.Enabled = true;
                        this.FiefTreasTextBox.Enabled = true;
                        this.FiefTreasTextBox.Text = fiefTreasury.ToString();
                    }

                    // set TextBoxes to show appropriate data
                    this.adjGarrSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.garrisonSpendNext);
                    this.adjInfrSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.infrastructureSpendNext);
                    this.adjOffSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.officialsSpendNext);
                    this.adjustKeepSpendTextBox.Text = Convert.ToString(Globals_Client.fiefToView.keepSpendNext);
                    this.adjustTaxTextBox.Text = Convert.ToString(Globals_Client.fiefToView.taxRateNext);
                    this.fiefPrevKeyStatsTextBox.Text = this.displayFiefKeyStatsPrev(Globals_Client.fiefToView);
                    this.fiefCurrKeyStatsTextBox.Text = this.displayFiefKeyStatsCurr(Globals_Client.fiefToView);
                    this.fiefNextKeyStatsTextBox.Text = this.displayFiefKeyStatsNext(Globals_Client.fiefToView);

                    // display home treasury
                    this.fiefHomeTreasTextBox.Text = homeTreasury.ToString();

                    // check to see if proposed expenditure level doesn't exceed fief treasury
                    // get fief expenses (includes bailiff modifiers)
                    uint totalSpend = Convert.ToUInt32(Globals_Client.fiefToView.calcNewExpenses());

                    // make sure expenditure can be supported by the treasury
                    // if it can't, display a message and cancel the commit
                    if (!Globals_Client.fiefToView.checkExpenditureOK(totalSpend))
                    {
                        int difference = Convert.ToInt32(totalSpend - fiefTreasury);
                        toDisplay = "Your proposed expenditure exceeds the " + Globals_Client.fiefToView.name + " treasury by " + difference;
                        toDisplay += "\r\n\r\nYou must either transfer funds from your Home Treasury, or reduce your spending.";
                        toDisplay += "\r\n\r\nAny unsupportable expenditure levels will be automatically adjusted during the seasonal update.";
                        displayWarning = true;
                    }
                }

            }

            Globals_Client.containerToView = this.fiefContainer;
            Globals_Client.containerToView.BringToFront();

            if (displayWarning)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show(toDisplay, "WARNING: CANNOT SUPPORT PROPOSED EXPENDITURE");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the fiefManagementToolStripMenuItem
        /// which displays main Fief information screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.refreshFiefContainer(Globals_Client.myPlayerCharacter.location);
        }

        /// <summary>
        /// Responds to the click event of the adjustSpendBtn button
        /// which commits the expenditures and tax rate for the coming year
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void adjustSpendBtn_Click(object sender, EventArgs e)
        {
            // keep track of whether any spends ahve changed
            bool spendChanged = false;

            try
            {
                // get new amounts
                Double newTax = Convert.ToDouble(this.adjustTaxTextBox.Text);
                UInt32 newOff = Convert.ToUInt32(this.adjOffSpendTextBox.Text);
                UInt32 newGarr = Convert.ToUInt32(this.adjGarrSpendTextBox.Text);
                UInt32 newInfra = Convert.ToUInt32(this.adjInfrSpendTextBox.Text);
                UInt32 newKeep = Convert.ToUInt32(this.adjustKeepSpendTextBox.Text);

                // get total spend
                uint totalSpend = newOff + newGarr + newInfra + newKeep;

                // factor in bailiff skills modifier for fief expenses
                double bailiffModif = 0;

                // get bailiff modifier (passing in whether bailiffDaysInFief is sufficient)
                bailiffModif = Globals_Client.fiefToView.calcBailExpModif(Globals_Client.fiefToView.bailiffDaysInFief >= 30);

                if (bailiffModif != 0)
                {
                    totalSpend = totalSpend + Convert.ToUInt32(totalSpend * bailiffModif);
                }

                // check that expenditure can be supported by the treasury
                // if it can't, display a message and cancel the commit
                if (!Globals_Client.fiefToView.checkExpenditureOK(totalSpend))
                {
                    int difference = Convert.ToInt32(totalSpend - Globals_Client.fiefToView.getAvailableTreasury());
                    string toDisplay = "Your spending exceeds the " + Globals_Client.fiefToView.name + " treasury by " + difference;
                    toDisplay += "\r\n\r\nYou must either transfer funds from your Home Treasury, or reduce your spending.";
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay, "TRANSACTION CANCELLED");
                    }
                }
                // if treasury funds are sufficient to cover expenditure, do the commit
                else
                {
                    // tax rate
                    // check if amount/rate changed
                    if (newTax != Globals_Client.fiefToView.taxRateNext)
                    {
                        // adjust tax rate
                        Globals_Client.fiefToView.adjustTaxRate(newTax);
                        spendChanged = true;
                    }

                    // officials spend
                    // check if amount/rate changed
                    if (newOff != Globals_Client.fiefToView.officialsSpendNext)
                    {
                        // adjust officials spend
                        Globals_Client.fiefToView.adjustOfficialsSpend(Convert.ToUInt32(this.adjOffSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // garrison spend
                    // check if amount/rate changed
                    if (newGarr != Globals_Client.fiefToView.garrisonSpendNext)
                    {
                        // adjust garrison spend
                        Globals_Client.fiefToView.adjustGarrisonSpend(Convert.ToUInt32(this.adjGarrSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // infrastructure spend
                    // check if amount/rate changed
                    if (newInfra != Globals_Client.fiefToView.infrastructureSpendNext)
                    {
                        // adjust infrastructure spend
                        Globals_Client.fiefToView.adjustInfraSpend(Convert.ToUInt32(this.adjInfrSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // adjust keep spend
                    // check if amount/rate changed
                    if (newKeep != Globals_Client.fiefToView.keepSpendNext)
                    {
                        // adjust keep spend
                        Globals_Client.fiefToView.adjustKeepSpend(Convert.ToUInt32(this.adjustKeepSpendTextBox.Text));
                        spendChanged = true;
                    }

                    // display appropriate message
                    string toDisplay = "";
                    if (spendChanged)
                    {
                        toDisplay += "Expenditure adjusted";
                    }
                    else
                    {
                        toDisplay += "Expenditure unchanged";
                    }

                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
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
            finally
            {
                // refresh screen if expenditure changed
                if (spendChanged)
                {
                    // refresh display
                    this.refreshCurrentScreen();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of myFiefsToolStripMenuItem
        /// which refreshes and displays the owned fiefs screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void myFiefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals_Client.fiefToView = null;
            this.refreshMyFiefs();
            Globals_Client.containerToView = this.fiefsOwnedContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the click event of the viewBailiffBtn button
        /// which refreshes and displays the character screen, showing details of the
        /// bailiff for the selected fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewBailiffBtn_Click(object sender, EventArgs e)
        {
            if (Globals_Client.fiefToView.bailiff != null)
            {
                // if player is bailiff, show in personal characteristics screen
                if (Globals_Client.fiefToView.bailiff == Globals_Client.myPlayerCharacter)
                {
                    Globals_Client.charToView = Globals_Client.myPlayerCharacter;
                    this.refreshCharacterContainer(Globals_Client.charToView);
                }

                // if NPC is bailiff, show in household affairs screen
                else if (Globals_Client.fiefToView.bailiff is NonPlayerCharacter)
                {
                    Globals_Client.charToView = Globals_Client.fiefToView.bailiff;
                    // refresh household affairs screen 
                    this.refreshHouseholdDisplay(Globals_Client.charToView as NonPlayerCharacter);
                    // display household affairs screen
                    Globals_Client.containerToView = this.houseContainer;
                    Globals_Client.containerToView.BringToFront();
                }
            }

            // display message that is no bailiff
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("This fief currently has no bailiff.");
                }
            }
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of the fiefsListView object,
        /// invoking the displayFief method, passing a Fief to display
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // enable controls
            if (this.fiefsListView.SelectedItems.Count > 0)
            {
                this.fiefsChallengeBtn.Enabled = true;
                this.fiefsViewBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Responds to the click event of the setBailiffBtn button
        ///invoking and displaying the character selection screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void setBailiffBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm chooseBailiff = new SelectionForm(this, "bailiff");
            chooseBailiff.Show();
        }

        /// <summary>
        /// Responds to the click event of the lockoutBtn button
        /// invoking and displaying the lockout screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void lockoutBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm lockOutOptions = new SelectionForm(this, "lockout");
            lockOutOptions.Show();
        }

        /// <summary>
        /// Responds to the click event of the removeBaliffBtn button,
        /// relieving the current bailiff of his duties
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void removeBaliffBtn_Click(object sender, EventArgs e)
        {
            // if the fief has an existing bailiff
            if (Globals_Client.fiefToView.bailiff != null)
            {
                // relieve him of his duties
                Globals_Client.fiefToView.bailiff = null;

                // refresh fief display
                this.refreshFiefContainer(Globals_Client.fiefToView);
            }
        }

        /// <summary>
        /// Responds to the click event of the selfBailiffBtn button,
        /// appointing the player as bailiff of the displayed fief
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void selfBailiffBtn_Click(object sender, EventArgs e)
        {
            // give player fair warning of bailiff commitments
            DialogResult dialogResult = MessageBox.Show("Being a bailiff will restrict your movement.  Click 'OK' to proceed.", "Proceed with appointment?", MessageBoxButtons.OKCancel);

            // if choose to cancel
            if (dialogResult == DialogResult.Cancel)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Appointment cancelled.");
                }
            }

            // if choose to proceed
            else
            {
                // if the fief has an existing bailiff
                if (Globals_Client.fiefToView.bailiff != null)
                {
                    // relieve him of his duties
                    Globals_Client.fiefToView.bailiff = null;
                }

                // set player as bailiff
                Globals_Client.fiefToView.bailiff = Globals_Client.myPlayerCharacter;
            }

            // refresh fief display
            this.refreshFiefContainer(Globals_Client.fiefToView);
        }

        /// <summary>
        /// Responds to the click event of any of the 'transfer funds' buttons
        /// allowing players to transfer funds between treasuries
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferFundsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // get button
                Button button = sender as Button;
                // get transfer parameters from tag
                string transferType = button.Tag.ToString();

                Fief fiefFrom = null;
                Fief fiefTo = null;
                int amount = 0;

                switch (transferType)
                {
                    case "toFief":
                        fiefFrom = Globals_Client.myPlayerCharacter.getHomeFief();
                        fiefTo = Globals_Client.fiefToView;
                        amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);
                        break;
                    case "toHome":
                        fiefFrom = Globals_Client.fiefToView;
                        fiefTo = Globals_Client.myPlayerCharacter.getHomeFief();
                        amount = Convert.ToInt32(this.fiefTransferAmountTextBox.Text);
                        break;
                    default:
                        break;
                }

                if (((fiefFrom != null) && (fiefTo != null)) && (amount > 0))
                {
                    // make sure are enough funds to cover transfer
                    if (amount > fiefFrom.getAvailableTreasury(true))
                    {
                        // if not, inform player and adjust amount downwards
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Too few funds available for this transfer.");
                        }
                    }

                    else
                    {
                        // make the transfer
                        this.treasuryTransfer(fiefFrom, fiefTo, amount);
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
        /// Transfers funds between the home treasury and the fief treasury
        /// </summary>
        /// <param name="from">The Fief from which funds are to be transferred</param>
        /// <param name="to">The Fief to which funds are to be transferred</param>
        /// <param name="amount">How much to be transferred</param>
        public void treasuryTransfer(Fief from, Fief to, int amount)
        {
            // subtract from source treasury
            from.treasury = from.treasury - amount;

            // add to target treasury
            to.treasury = to.treasury + amount;

            // refresh fief display
            this.refreshCurrentScreen();
        }

        /// <summary>
        /// Checks to see if display of financial data for the specified financial period
        /// is permitted due to ongoing siege
        /// </summary>
        /// <returns>bool indicating whether display is permitted</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        /// <param name="s">The siege</param>
        public bool checkToShowFinancialData(int relativeSeason, Siege s)
        {
            bool displayData = true;

            uint financialPeriodYear = this.getFinancialYear(relativeSeason);
            if (financialPeriodYear > s.startYear)
            {
                displayData = false;
            }
            else if (financialPeriodYear == s.startYear)
            {
                byte financialPeriodSeason = this.getFinancialSeason(relativeSeason);
                if (financialPeriodSeason > s.startSeason)
                {
                    displayData = false;
                }
            }

            return displayData;
        }

        /// <summary>
        /// Gets the year for the specified financial period
        /// </summary>
        /// <returns>The year</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public uint getFinancialYear(int relativeSeason)
        {
            uint financialYear = 0;
            uint thisYear = Globals_Game.clock.currentYear;

            switch (relativeSeason)
            {
                case (-1):
                    if (Globals_Game.clock.currentSeason == 0)
                    {
                        financialYear = thisYear - 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                case (1):
                    if (Globals_Game.clock.currentSeason == 4)
                    {
                        financialYear = thisYear + 1;
                    }
                    else
                    {
                        financialYear = thisYear;
                    }
                    break;
                default:
                    financialYear = thisYear;
                    break;
            }

            return financialYear;
        }

        /// <summary>
        /// Gets the season for the specified financial period
        /// </summary>
        /// <returns>The season</returns>
        /// <param name="target">int indicating desired financial period relative to current season</param>
        public byte getFinancialSeason(int relativeSeason)
        {
            byte financialSeason = 0;
            byte thisSeason = Globals_Game.clock.currentSeason;

            switch (relativeSeason)
            {
                case (-1):
                    if (thisSeason == 0)
                    {
                        financialSeason = 4;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason--;
                    }
                    break;
                case (1):
                    if (thisSeason == 4)
                    {
                        financialSeason = 0;
                    }
                    else
                    {
                        financialSeason = thisSeason;
                        financialSeason++;
                    }
                    break;
                default:
                    financialSeason = thisSeason;
                    break;
            }

            return financialSeason;
        }

        /// <summary>
        /// Responds to the click event of the viewMyHomeFiefToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void viewMyHomeFiefToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get home fief
            Fief homeFief = Globals_Client.myPlayerCharacter.getHomeFief();

            if (homeFief != null)
            {
                // display home fief
                this.refreshFiefContainer(homeFief);
            }

            // if have no home fief
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("You have no home fief!");
                }
            }
        }

        /// <summary>
        /// Responds to the click event of any of the 'Max' buttons inn the fief management screen,
        /// filling in the maximum expenditure for the selected field
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void maxSpendButton_Click(object sender, EventArgs e)
        {
            uint maxSpend = 0;

            // get tag from button
            Button button = sender as Button;
            string expType = button.Tag.ToString();

            // get max spend of specified type
            maxSpend = Globals_Client.fiefToView.getMaxSpend(expType);

            if (maxSpend != 0)
            {
                switch (expType)
                {
                    case "garrison":
                        this.adjGarrSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "infrastructure":
                        this.adjInfrSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "keep":
                        this.adjustKeepSpendTextBox.Text = maxSpend.ToString();
                        break;
                    case "officials":
                        this.adjOffSpendTextBox.Text = maxSpend.ToString();
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Responds to the click event of the fiefGrantTitleBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefGrantTitleBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm chooseTitleHolder = new SelectionForm(this, "titleHolder");
            chooseTitleHolder.Show();
        }

        /// <summary>
        /// Responds to the click event of the fiefTransferFundsPlayerBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void fiefTransferFundsPlayerBtn_Click(object sender, EventArgs e)
        {
            // check for previously opened SelectionForm and close if necessary
            if (Application.OpenForms.OfType<SelectionForm>().Any())
            {
                Application.OpenForms.OfType<SelectionForm>().First().Close();
            }

            SelectionForm transferFunds = new SelectionForm(this, "transferFunds");
            transferFunds.Show();
        }
    }
}
