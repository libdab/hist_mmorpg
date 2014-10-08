using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace hist_mmorpg
{
    /// <summary>
    /// User interface component for selecting and passing data back to Form1
    /// </summary>
    public partial class SelectionForm : Form
    {
        /// <summary>
        /// Holds parent form, allowing access to its public variables
        /// </summary>
        public Form1 parent;
        /// <summary>
        /// Holds observer (if examining armies in fief)
        /// </summary>
        public Character observer { get; set; }

        /// <summary>
        /// Constructor for SelectionForm
        /// </summary>
        /// <param name="par">Parent Form1 object</param>
        /// <param name="myFunction">String indicating function to be performed</param>
        /// <param name="armyID">String indicating ID of army (if choosing leader)</param>
        /// <param name="observer">Observer (if examining armies in fief)</param>
        public SelectionForm(Form1 par, String myFunction, String armyID = null, Character obs = null)
        {
            // initialise form elements
            InitializeComponent();

            this.parent = par;
            this.observer = obs;

            // initialise NPC display
            this.initDisplay(myFunction, armyID);
        }

        /// <summary>
        /// Initialises NPC display screen
        /// </summary>
        /// <param name="myFunction">String indicating function to be performed</param>
        /// <param name="armyID">String indicating ID of army (if choosing leader)</param>
        private void initDisplay(String myFunction, String armyID = null)
        {
            if ((myFunction.Equals("bailiff")) || (myFunction.Equals("leader")))
            {
                // format list display
                this.setUpNpcList(myFunction, armyID);

                // refresh information 
                this.refreshNPCdisplay(myFunction, armyID);

                // show container
                this.npcContainer.BringToFront();
            }

            if (myFunction.Equals("lockout"))
            {
                // format list display
                this.setUpBarredList();

                // refresh information 
                this.refreshBarredDisplay();

                // show container
                this.lockOutContainer.BringToFront();
            }

            if (myFunction.Equals("transfer"))
            {
                // format list display
                this.setUpTransferList();

                // refresh information 
                this.refreshTransferDisplay(armyID);

                // show container
                this.transferContainer.BringToFront();
            }

            if (myFunction.Equals("armies"))
            {
                // format list display
                this.setUpArmiesList();

                // refresh information 
                this.refreshArmiesDisplay();

                // show container
                this.armiesContainer.BringToFront();
            }
        }

        /// <summary>
        /// Configures UI display for list of household NPCs
        /// </summary>
        /// <param name="myFunction">String indicating function to be performed</param>
        /// <param name="armyID">String indicating ID of army (if choosing leader)</param>
        public void setUpNpcList(String myFunction, String armyID = null)
        {
            // add necessary columns
            this.npcListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("Location", -2, HorizontalAlignment.Left);

            // set appropriate button text and tag
            if (myFunction.Equals("bailiff"))
            {
                this.chooseNpcBtn.Text = "Appoint This Person As Bailiff";
                this.chooseNpcBtn.Tag = myFunction;
            }
            else if (myFunction.Equals("leader"))
            {
                this.chooseNpcBtn.Text = "Appoint This Person As Leader";
                this.chooseNpcBtn.Tag = armyID;
            }

            // disable button (until NPC selected)
            this.chooseNpcBtn.Enabled = false;
        }

        /// <summary>
        /// Refreshes NPC list
        /// </summary>
        /// <param name="myFunction">String indicating function to be performed</param>
        /// <param name="armyID">String indicating ID of army (if choosing leader)</param>
        public void refreshNPCdisplay(String myFunction, String armyID = null)
        {
            // remove any previously displayed characters
            this.npcDetailsTextBox.ReadOnly = true;
            this.npcDetailsTextBox.Text = "";

            // clear existing items in list
            this.npcListView.Items.Clear();

            // if choosing army leader, get army
            Army myArmy = null;
            if (myFunction.Equals("leader"))
            {
                myArmy = Globals_Server.armyMasterList[armyID];
            }

            ListViewItem myNPC = null;

            // iterates through employees
            for (int i = 0; i < Globals_Client.myChar.myNPCs.Count; i++)
            {
                // Create an item and subitems for each character

                // name
                myNPC = new ListViewItem(Globals_Client.myChar.myNPCs[i].firstName + " " + Globals_Client.myChar.myNPCs[i].familyName);

                // charID
                myNPC.SubItems.Add(Globals_Client.myChar.myNPCs[i].charID);

                // location
                myNPC.SubItems.Add(Globals_Client.myChar.myNPCs[i].location.fiefID);

                // add item to fiefsListView
                if (myFunction.Equals("bailiff"))
                {
                    this.npcListView.Items.Add(myNPC);
                }
                // if appointing leader, only add item to fiefsListView if is in same fief as army
                else if (myFunction.Equals("leader"))
                {
                    if (Globals_Client.myChar.myNPCs[i].location.fiefID == myArmy.location)
                    {
                        this.npcListView.Items.Add(myNPC);
                    }
                }
            }
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the npcListView
        /// which then displays the selected NPC's details in the npcDetailsTextBox
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void npcListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            NonPlayerCharacter npcToDisplay = null;

            string textToDisplay = "";

            if (npcListView.SelectedItems.Count > 0)
            {
                // enable 'appoint this NPC' button
                this.chooseNpcBtn.Enabled = true;
                // set textbox to read only
                this.npcDetailsTextBox.ReadOnly = true;

                // get employee
                npcToDisplay = Globals_Server.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];

                // get details
                textToDisplay += this.displayNPC(npcToDisplay);

                // display details
                this.npcDetailsTextBox.Text = textToDisplay;
            }
        }

        /// <summary>
        /// Retrieves army details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="a">Army whose information is to be displayed</param>
        public string displayArmy(Army a)
        {
            bool isMyArmy = false;
            uint[] troopNumbers = new uint[6];
            uint totalTroops = 0;
            string armyText = "";

            // ID
            armyText += "ID: " + a.armyID + "\r\n\r\n";

            // owner
            PlayerCharacter thisOwner = a.getOwner();
            armyText += "Owner: " + thisOwner.firstName + " " + thisOwner.familyName + " (" + thisOwner.charID + ")\r\n\r\n";

            // check if is your army (will effect display of troop numbers)
            if (thisOwner == Globals_Client.myChar)
            {
                isMyArmy = true;
            }

            // leader
            Character armyLeader = a.getLeader();
            armyText += "Leader: ";

            if (armyLeader == null)
            {
                armyText += "THIS ARMY HAS NO LEADER!\r\n\r\n";
            }
            else
            {
                armyText += armyLeader.firstName + " " + armyLeader.familyName + " (" + armyLeader.charID + ")";
            }
            armyText += "\r\n\r\n";

            // get troop numbers
            if (isMyArmy)
            {
                // actual troop numbers if is player's army
                troopNumbers = a.troops;
            }
            else
            {
                // estimated troop numbers if is NOT player's army
                troopNumbers = a.getTroopsEstimate(observer);
            }

            armyText += "Troop numbers";
            if (!isMyArmy)
            {
                armyText += " (ESTIMATE)";
            }
            armyText += ":\r\n";

            // labels for troop types
            string[] troopTypeLabels = new string[] { " - Knights: ", " - Men-at-Arms: ", " - Light Cavalry: ", " - Yeomen: ", " - Foot: ", " - Rabble: " };

            // display numbers for each troop type
            for (int i = 0; i < troopNumbers.Length; i++ )
            {
                armyText += troopTypeLabels[i] + troopNumbers[i];
                totalTroops += troopNumbers[i];
                armyText += "\r\n";
            }

            // display total
            armyText += "   ==================\r\n";
            armyText += " - TOTAL: " + totalTroops + "\r\n\r\n";

            return armyText;
        }

        /// <summary>
        /// Retrieves NPC details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="npc">NonPlayerCharacter whose information is to be displayed</param>
        public string displayNPC(NonPlayerCharacter npc)
        {
            string charText = "";

            // ID
            charText += "ID: " + npc.charID + "\r\n";

            // name
            charText += "Name: " + npc.firstName + " " + npc.familyName + "\r\n";

            // age
            charText += "Age: " + npc.calcCharAge() + "\r\n";

            // nationality
            charText += "Nationality: " + npc.nationality + "\r\n";

            // health (& max. health)
            charText += "Health: ";
            if (!npc.isAlive)
            {
                charText += "Blimey, you're Dead!";
            }
            else
            {
                charText += npc.calculateHealth() + " (max. health: " + npc.maxHealth + ")";
            }
            charText += "\r\n";

            // any death modifiers (from skills)
            charText += "  (Death modifier from skills: " + npc.calcSkillEffect("death") + ")\r\n";

            // location
            charText += "Current location: " + npc.location.name + " (" + npc.location.province.name + ")\r\n";

            // if in process of auto-moving, display next hex
            if (npc.goTo.Count != 0)
            {
                charText += "Next Fief (if auto-moving): " + npc.goTo.Peek().fiefID + "\r\n";
            }

            // language
            charText += "Language: " + npc.language + "\r\n";

            // days left
            charText += "Days remaining: " + npc.days + "\r\n";

            // stature
            charText += "Stature: " + npc.calculateStature() + "\r\n";
            charText += "  (base stature: " + npc.calculateStature(false) + " | modifier: " + npc.statureModifier + ")\r\n";

            // management rating
            charText += "Management: " + npc.management + "\r\n";

            // combat rating
            charText += "Combat: " + npc.combat + "\r\n";

            // skills list
            charText += "Skills:\r\n";
            for (int i = 0; i < npc.skills.Length; i++)
            {
                charText += "  - " + npc.skills[i].Item1.name + " (level " + npc.skills[i].Item2 + ")\r\n";
            }

            // gather additional NPC-specific information
            charText += this.displayNonPlayerCharacter(npc);

            return charText;
        }

        /// <summary>
        /// Retrieves NonPlayerCharacter-specific details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="npc">NonPlayerCharacter whose information is to be displayed</param>
        public string displayNonPlayerCharacter(NonPlayerCharacter npc)
        {
            string npcText = "";

            // current salary
            npcText += "Current salary: " + npc.wage + "\r\n";

            return npcText;
        }

        /// <summary>
        /// Responds to the click event of the chooseNpcBtn button which appoints the selected NPC
        /// as either 1) the bailiff of the fief displayed in the main UI
        /// or 2) the player's heir.  Then closes the child (this) form.
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void chooseNpcBtn_Click(object sender, EventArgs e)
        {
            if (npcListView.SelectedItems.Count > 0)
            {
                // get selected NPC
                NonPlayerCharacter selectedNPC = Globals_Server.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];

                // get chooseNpcBtn tag (determines function)
                String myButtonTag = Convert.ToString(((Button)sender).Tag);

                // if appointing a bailiff
                if (myButtonTag.Equals("bailiff"))
                {
                    // set the selected NPC as bailiff
                    Globals_Client.fiefToView.bailiff = selectedNPC;
                    selectedNPC.myBoss = Globals_Client.myChar.charID;

                    // refresh the fief information (in the main form)
                    this.parent.refreshFiefContainer(Globals_Client.fiefToView);
                }

                // if appointing an army leader
                else
                {
                    // get army
                    Army thisArmy = Globals_Server.armyMasterList[myButtonTag];

                    thisArmy.assignNewLeader(selectedNPC);

                    // refresh the army information (in the main form)
                    this.parent.refreshArmyContainer(thisArmy);
                }
            }

            // close this form
            this.Close();
        }

        /// <summary>
        /// Configures UI display for list of barred characters
        /// </summary>
        public void setUpBarredList()
        {
            // add necessary columns
            this.barredListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.barredListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.barredListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes barred characters list
        /// </summary>
        public void refreshBarredDisplay()
        {
            // clear existing items in list
            this.barredListView.Items.Clear();

            ListViewItem[] barredChars = new ListViewItem[Globals_Client.fiefToView.barredCharacters.Count];

            // iterates through employees
            for (int i = 0; i < Globals_Client.fiefToView.barredCharacters.Count; i++)
            {
                // retrieve character
                //PlayerCharacter myBarredpc = null;
                //NonPlayerCharacter myBarrednpc = null;
                Character myBarredChar = null;

                if (Globals_Server.pcMasterList.ContainsKey(Globals_Client.fiefToView.barredCharacters[i]))
                {
                    myBarredChar = Globals_Server.pcMasterList[Globals_Client.fiefToView.barredCharacters[i]];
                }
                else if (Globals_Server.npcMasterList.ContainsKey(Globals_Client.fiefToView.barredCharacters[i]))
                {
                    myBarredChar = Globals_Server.npcMasterList[Globals_Client.fiefToView.barredCharacters[i]];
                }

                if (myBarredChar != null)
                {
                    // Create an item and subitems for each character

                    // name
                    barredChars[i] = new ListViewItem(myBarredChar.firstName + " " + myBarredChar.familyName);

                    // charID
                    barredChars[i].SubItems.Add(myBarredChar.charID);

                    // if is in player's nationality
                    barredChars[i].SubItems.Add(myBarredChar.nationality);

                    // add item to fiefsListView
                    this.barredListView.Items.Add(barredChars[i]);
                }

            }

            // disable 'UnBar Character' button until a list item is selected
            this.unbarCharBtn.Enabled = false;
            // ensure the nationality bar CheckBoxes are displaying correctly
            this.barEnglishCheckBox.Checked = Globals_Client.fiefToView.englishBarred;
            this.barFrenchCheckBox.Checked = Globals_Client.fiefToView.frenchBarred;
        }

        /// <summary>
        /// Configures UI display for list of troop detachments
        /// </summary>
        public void setUpTransferList()
        {
            // add necessary columns
            this.transferListView.Columns.Add("   ID", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Knights", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("M-A-A", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("LightCav", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Yeomen", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Foot", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Rabble", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Days", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);
            this.transferListView.Columns.Add("For", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Configures UI display for list of armies in feif
        /// </summary>
        public void setUpArmiesList()
        {
            // add necessary columns
            this.armiesListView.Columns.Add("   ID", -2, HorizontalAlignment.Left);
            this.armiesListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);

            // disable button
            this.armiesAttackBtn.Enabled = false;
        }

        /// <summary>
        /// Refreshes troop transfer list
        /// </summary>
        /// <param name="armyID">String indicating ID of army receiving troops</param>
        public void refreshTransferDisplay(string armyID)
        {
            ListViewItem thisDetachment = null;

            // get army
            Army thisArmy = Globals_Server.armyMasterList[armyID];

            // get fief
            Fief thisFief = Globals_Server.fiefMasterList[thisArmy.location];

            // add troop detachments to list
            foreach (KeyValuePair<string, string[]> troopDetachment in thisFief.troopTransfers)
            {
                if ((troopDetachment.Value[0] == thisArmy.owner) || (troopDetachment.Value[1] == thisArmy.owner))
                {
                    // Create an item and subitems for each detachment

                    // ID
                    thisDetachment = new ListViewItem(troopDetachment.Key);

                    // knights
                    thisDetachment.SubItems.Add(troopDetachment.Value[2]);
                    // menAtArms
                    thisDetachment.SubItems.Add(troopDetachment.Value[3]);
                    // lightCav
                    thisDetachment.SubItems.Add(troopDetachment.Value[4]);
                    // yeomen
                    thisDetachment.SubItems.Add(troopDetachment.Value[5]);
                    // foot
                    thisDetachment.SubItems.Add(troopDetachment.Value[6]);
                    // rabble
                    thisDetachment.SubItems.Add(troopDetachment.Value[7]);

                    // days
                    thisDetachment.SubItems.Add(troopDetachment.Value[8]);

                    // owner
                    thisDetachment.SubItems.Add(troopDetachment.Value[0]);

                    // for
                    thisDetachment.SubItems.Add(troopDetachment.Value[1]);

                    // add item to fiefsListView
                    this.transferListView.Items.Add(thisDetachment);

                }
            }
        }

        /// <summary>
        /// Refreshes list of armies in fief
        /// </summary>
        public void refreshArmiesDisplay()
        {
            ListViewItem armyEntry = null;

            // get fief
            Fief thisFief = this.observer.location;

            // Create an item and subitems for each army in the fief and add to list
            foreach (string armyID in thisFief.armies)
            {
                // get army
                Army thisArmy = Globals_Server.armyMasterList[armyID];

                // ID
                armyEntry = new ListViewItem(armyID);

                // owner
                armyEntry.SubItems.Add(thisArmy.owner);

                // add item to fiefsListView
                this.armiesListView.Items.Add(armyEntry);
            }
        }

        /// <summary>
        /// Responds to the click event of the barThisCharBtn button,
        /// adding the charID in the barThisCharTextBox to the barred characters list
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barThisCharBtn_Click(object sender, EventArgs e)
        {
            // if input ID is in pcMasterList
            if (Globals_Server.pcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                // add ID to barred characters
                Globals_Client.fiefToView.barredCharacters.Add(this.barThisCharTextBox.Text);
                // refresh display
                this.refreshBarredDisplay();
            }
            // if input ID is in npcMasterList
            else if (Globals_Server.npcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                // add ID to barred characters
                Globals_Client.fiefToView.barredCharacters.Add(this.barThisCharTextBox.Text);
                // refresh display
                this.refreshBarredDisplay();
            }
            // if input ID not found
            else
            {
                // ask player to check entry
                System.Windows.Forms.MessageBox.Show("Character could not be identified.  Please ensure charID is valid.");
            }
        }

        /// <summary>
        /// Responds to the click event of the unbarCharBtn button,
        /// removing the selected character in the ListView from the barred characters list
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void unbarCharBtn_Click(object sender, EventArgs e)
        {
            if (barredListView.SelectedItems.Count > 0)
            {
                // if selected character is in pcMasterList
                if (Globals_Server.pcMasterList.ContainsKey(this.barredListView.SelectedItems[0].SubItems[1].Text))
                {
                    // remove ID from barred characters
                    Globals_Client.fiefToView.barredCharacters.Remove(this.barredListView.SelectedItems[0].SubItems[1].Text);
                    // refresh display
                    this.refreshBarredDisplay();
                }
                // if selected character is in pcMasterList
                else if (Globals_Server.npcMasterList.ContainsKey(this.barredListView.SelectedItems[0].SubItems[1].Text))
                {
                    // remove ID from barred characters
                    Globals_Client.fiefToView.barredCharacters.Remove(this.barredListView.SelectedItems[0].SubItems[1].Text);
                    // refresh display
                    this.refreshBarredDisplay();
                }
                // if selected character not found
                else
                {
                    // display error message
                    System.Windows.Forms.MessageBox.Show("Selected character could not be identified.");
                }

            }
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the barredListView,
        /// enabling the 'unBar Character' button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barredListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (barredListView.SelectedItems.Count > 0)
            {
                // enable 'unBar Character' button
                this.unbarCharBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Responds to the CheckedChanged event of the barFrenchCheckBox,
        /// either barring or unbarring all French characters
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barFrenchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // bar/unbar French, depending on whether CheckBox is checked
            Globals_Client.fiefToView.frenchBarred = this.barFrenchCheckBox.Checked;
            // refresh the parent's fief container
            this.parent.refreshFiefContainer(Globals_Client.fiefToView);
        }

        /// <summary>
        /// Responds to the CheckedChanged event of the barEnglishCheckBox,
        /// either barring or unbarring all English characters
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void barEnglishCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // bar/unbar English, depending on whether CheckBox is checked
            Globals_Client.fiefToView.englishBarred = this.barEnglishCheckBox.Checked;
            // refresh the parent's fief container
            this.parent.refreshFiefContainer(Globals_Client.fiefToView);
        }

        /// <summary>
        /// Responds to the click event of the closeBtn button,
        /// closing the Lock Out Menu
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Responds to the click event of the transferPickupBtn button,
        /// allowing any selected detachments to be added to the currently displayed army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferPickupBtn_Click(object sender, EventArgs e)
        {
            bool proceed = true;
            bool adjustDays = true;
            double daysTaken = 0;
            double minDays = 0;
            bool displayNotAllMsg = false;
            uint[] totTroopsToAdd = new uint[] {0, 0, 0, 0, 0, 0};
            string toDisplay = "";

            // get army
            Army thisArmy = Globals_Client.armyToView;

            // set minDays to thisArmy.days (as default value)
            minDays = thisArmy.days;

            // get leader
            Character myLeader = thisArmy.getLeader();

            // get checked items in listview
            ListView.CheckedListViewItemCollection checkedItems = this.transferListView.CheckedItems;

            // if no detachment selected, do nothing except display message
            if (checkedItems.Count < 1)
            {
                System.Windows.Forms.MessageBox.Show("No detachments have been selected.");
            }

            // if any detachments selected, proceed
            else
            {
                // check have minimum days necessary for transfer
                if (thisArmy.days < 10)
                {
                    System.Windows.Forms.MessageBox.Show("You don't have enough days left for this transfer.  Transfer cancelled.");
                    proceed = false;
                    adjustDays = false;
                }
                else
                {
                    // calculate time taken for transfer
                    daysTaken = Globals_Server.myRand.Next(10, 31);

                    // check if have enough days for transfer in this instance
                    if (daysTaken > thisArmy.days)
                    {
                        daysTaken = thisArmy.days;
                        System.Windows.Forms.MessageBox.Show("Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.");
                        proceed = false;
                    }
                }

                if (proceed)
                {
                    // get fief
                    Fief thisFief = Globals_Server.fiefMasterList[thisArmy.location];

                    // check for minimum days
                    foreach (ListViewItem item in checkedItems)
                    {
                        double thisDays = Convert.ToDouble(item.SubItems[7].Text);

                        // check if detachment has enough days for transfer in this instance
                        // if not, flag display of message at end of process, but do nothing else
                        if (thisDays < daysTaken)
                        {
                            displayNotAllMsg = true;
                            toDisplay = "At least one detachment could not be added due to poor organisation.";
                        }
                        else
                        {
                            if (thisDays < minDays)
                            {
                                minDays = thisDays;
                            }
                        }
                    }

                    foreach (ListViewItem item in checkedItems)
                    {
                        double thisDays = Convert.ToDouble(item.SubItems[7].Text);

                        // get numbers of each type to add
                        uint[] thisTroops = new uint[] { 0, 0, 0, 0, 0, 0 };
                        uint thisTotal = 0;
                        for (int i = 0; i < thisTroops.Length; i++)
                        {
                            thisTroops[i] = Convert.ToUInt32(item.SubItems[i+1].Text);
                            thisTotal += thisTroops[i];
                        }

                        // if does have enough days, proceed
                        if (thisDays >= daysTaken)
                        {
                            // compare days to minDays, apply attrition if necessary
                            // then add to troopsToAdd
                            if (thisDays > minDays)
                            {
                                // check for attrition (to bring it down to minDays)
                                byte attritionChecks = 0;
                                attritionChecks = Convert.ToByte((thisDays - minDays) / 7);
                                double attritionModifier = 0;

                                for (int i = 0; i < attritionChecks; i++)
                                {
                                    attritionModifier = thisArmy.calcAttrition(thisTotal);
                                }

                                // apply attrition
                                for (int i = 0; i < totTroopsToAdd.Length; i++)
                                {
                                    totTroopsToAdd[i] += (thisTroops[i] - Convert.ToUInt32(thisTroops[i] * attritionModifier));
                                }
                            }
                            else
                            {
                                for (int i = 0; i < totTroopsToAdd.Length; i++)
                                {
                                    totTroopsToAdd[i] += thisTroops[i];
                                }
                            }

                            // remove detachment from fief
                            thisFief.troopTransfers.Remove(item.SubItems[0].Text);
                        }

                    }

                }

                if (adjustDays)
                {
                    if (thisArmy.days == minDays)
                    {
                        // add troops to army (this could be 0)
                        for (int i = 0; i < thisArmy.troops.Length; i++ )
                        {
                            thisArmy.troops[i] += totTroopsToAdd[i];
                        }

                        // adjust days
                        myLeader.adjustDays(daysTaken);

                        // calculate attrition for army
                        byte attritionChecks = Convert.ToByte(daysTaken / 7);
                        double attritionModifier = 0;

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            attritionModifier = thisArmy.calcAttrition();
                            thisArmy.applyTroopLosses(attritionModifier);
                        }
                    }
                    else
                    {
                        // any days army has had to 'wait' should go towards days taken
                        // for the transfer (daysTaken)
                        double differenceToMin = (thisArmy.days - minDays);
                        if (differenceToMin >= daysTaken)
                        {
                            daysTaken = 0;
                        }
                        else
                        {
                            daysTaken = daysTaken - differenceToMin;
                        }

                        // adjust days
                        myLeader.adjustDays(differenceToMin);

                        // calculate attrition for army (to bring it down to minDays)
                        byte attritionChecks = Convert.ToByte(differenceToMin / 7);
                        double attritionModifier = 0;

                        for (int i = 0; i < attritionChecks; i++)
                        {
                            attritionModifier = thisArmy.calcAttrition();
                            thisArmy.applyTroopLosses(attritionModifier);
                        }

                       // add troops to army
                        for (int i = 0; i < thisArmy.troops.Length; i++ )
                        {
                            thisArmy.troops[i] += totTroopsToAdd[i];
                        }

                        // check if are any remaining days taken for the transfer (daysTaken) 
                        if (daysTaken > 0)
                        {
                            // adjust days
                            myLeader.adjustDays(daysTaken);

                            // calculate attrition for army for days taken for transfer
                            attritionChecks = Convert.ToByte(daysTaken / 7);

                            for (int i = 0; i < attritionChecks; i++)
                            {
                                attritionModifier = thisArmy.calcAttrition();
                                thisArmy.applyTroopLosses(attritionModifier);
                            }
                        }
                    }

                    // if not all selected detachments could be picked up (not enough days), show message
                    if (displayNotAllMsg)
                    {
                        System.Windows.Forms.MessageBox.Show(toDisplay);
                    }
                }

                // refresh the army screen (in the main form) and close form
                this.parent.refreshArmyContainer(thisArmy);
                this.Close();
            }

        }

        /// <summary>
        /// Responds to the click event of the transferCancelBtn button,
        /// closing the selection form
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferCancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the armiesListView,
        /// displaying the details of the selected army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armiesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            string textToDisplay = "";

            if (armiesListView.SelectedItems.Count > 0)
            {
                // set textbox to read only
                this.armiesTextBox.ReadOnly = true;

                // get details
                Army otherArmy = Globals_Server.armyMasterList[this.armiesListView.SelectedItems[0].SubItems[0].Text];
                textToDisplay += this.displayArmy(otherArmy);

                // display details
                this.armiesTextBox.Text = textToDisplay;

                // get owner of selected army
                PlayerCharacter otherArmyOwner = otherArmy.getOwner();

                // if selected army is not owned by player && observer is an army leader, enable attack button
                if ((otherArmyOwner != Globals_Client.myChar) && (this.observer.armyID != null))
                {
                    this.armiesAttackBtn.Enabled = true;
                }
                else
                {
                    this.armiesAttackBtn.Enabled = false;
                }

            }
        }

        /// <summary>
        /// Responds to the click event of the armiesCloseBtn button,
        /// closing the armies list
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armiesCloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Responds to the click event of the armiesAttackBtn button,
        /// instigating an attack on the selected army
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void armiesAttackBtn_Click(object sender, EventArgs e)
        {
            // check has enough days to give battle (1)
            if (this.observer.days < 1)
            {
                System.Windows.Forms.MessageBox.Show("Your army doesn't have enough days left to give battle.");
            }
            else
            {
                // get armies
                Army attacker = this.observer.getArmy();
                Army defender = Globals_Server.armyMasterList[this.armiesListView.SelectedItems[0].SubItems[0].Text];

                // SIEGE INVOLVEMENT
                // check if defending army is the garrison in a siege
                string siegeID = defender.checkIfSiegeDefenderGarrison();
                if (siegeID != null)
                {
                    System.Windows.Forms.MessageBox.Show("The defending army is currently being besieged and\r\ncannot be attacked.  Attack cancelled.");
                }

                else
                {
                    // check if defending army is the additional defender in a siege
                    siegeID = defender.checkIfSiegeDefenderAdditional();
                    if (siegeID != null)
                    {
                        System.Windows.Forms.MessageBox.Show("The defending army is currently being besieged and\r\ncannot be attacked.  Attack cancelled.");
                    }

                    else
                    {
                        // check if attacking army is besieging a keep
                        siegeID = attacker.checkIfBesieger();
                        if (siegeID != null)
                        {
                            // display warning and get decision
                            DialogResult dialogResult = MessageBox.Show("Your army is besieging a keep and this action would end the siege.\r\nClick 'OK' to proceed.", "Proceed with attack?", MessageBoxButtons.OKCancel);

                            // if choose to cancel
                            if (dialogResult == DialogResult.Cancel)
                            {
                                System.Windows.Forms.MessageBox.Show("Attack cancelled.");
                            }

                            // if choose to proceed
                            else
                            {
                                Siege thisSiege = null;
                                thisSiege = Globals_Server.siegeMasterList[siegeID];

                                parent.siegeEnd(thisSiege);

                                // let slip the dogs of war
                                parent.giveBattle(attacker, defender);
                            }
                        }
                    }
                }
            }

            // close form
            this.Close();
        }

    }

}
