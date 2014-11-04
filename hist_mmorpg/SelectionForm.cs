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
        /// Holds current function for the form
        /// </summary>
        public string function { get; set; }
        /// <summary>
        /// Holds armyID (if appointing an army leader)
        /// </summary>
        public string armyID { get; set; }
        /// <summary>
        /// Holds observer (if examining armies in fief)
        /// </summary>
        public Character observer { get; set; }
        /// <summary>
        /// Holds place id (if granting place title)
        /// </summary>
        public string placeID { get; set; }

        /// <summary>
        /// Constructor for SelectionForm
        /// </summary>
        /// <param name="par">Parent Form1 object</param>
        /// <param name="funct">String indicating function to be performed</param>
        /// <param name="armID">String indicating ID of army (if choosing leader)</param>
        /// <param name="observer">Observer (if examining armies in fief)</param>
        public SelectionForm(Form1 par, String funct, String armID = null, Character obs = null, String place = null)
        {
            // initialise form elements
            InitializeComponent();

            this.parent = par;
            this.function = funct;
            this.armyID = armID;
            this.observer = obs;
            this.placeID = place;

            // initialise NPC display
            this.initDisplay();
        }

        /// <summary>
        /// Initialises display screen
        /// </summary>
        private void initDisplay()
        {
            if (((this.function.Equals("bailiff")) || (this.function.Equals("leader")))
                || (this.function.Equals("titleHolder")) || (this.function.Contains("royalGift")))
            {
                // format list display
                this.setUpNpcList();

                // refresh information 
                this.refreshNPCdisplay();

                // show container
                this.npcContainer.BringToFront();
            }

            if (this.function.Equals("lockout"))
            {
                // format list display
                this.setUpBarredList();

                // refresh information 
                this.refreshBarredDisplay();

                // show container
                this.lockOutContainer.BringToFront();
            }

            if (this.function.Equals("transferTroops"))
            {
                // format list display
                this.setUpTransferList();

                // refresh information 
                this.refreshTransferDisplay();

                // show container
                this.transferContainer.BringToFront();
            }

            if (this.function.Equals("transferFunds"))
            {
                // format list display
                this.setUpTransferFundsList();

                // refresh information 
                this.refreshTransferFundsDisplay();

                // show container
                this.transferFundsContainer.BringToFront();
            }

            if (this.function.Equals("armies"))
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
        public void setUpNpcList()
        {
            // add necessary columns
            this.npcListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("Character ID", -2, HorizontalAlignment.Left);

            switch (this.function)
            {
                case "bailiff":
                    // add necessary columns
                    this.npcListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Appoint This Person As Bailiff";
                    break;
                case "leader":
                    // add necessary columns
                    this.npcListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Appoint This Person As Leader";
                    break;
                case "titleHolder":
                    // add necessary columns
                    this.npcListView.Columns.Add("Location", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Grant Title To This Person";
                    break;
                case "royalGiftTitle":
                    // add necessary columns
                    this.npcListView.Columns.Add("Player ID", -2, HorizontalAlignment.Left);
                    this.npcListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Grant Title To This Person";
                    break;
                case "royalGiftFief":
                    // add necessary columns
                    this.npcListView.Columns.Add("Player ID", -2, HorizontalAlignment.Left);
                    this.npcListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);
                    // set appropriate button text and tag
                    this.chooseNpcBtn.Text = "Gift Fief To This Person";
                    break;
                default:
                    break;
            }

            // disable button (until NPC selected)
            this.chooseNpcBtn.Enabled = false;
        }

        /// <summary>
        /// Configures UI display for list of players
        /// </summary>
        public void setUpTransferFundsList()
        {
            // add necessary columns
            this.transferFundsListView.Columns.Add("PC Name", -2, HorizontalAlignment.Left);
            this.transferFundsListView.Columns.Add("PC ID", -2, HorizontalAlignment.Left);
            this.transferFundsListView.Columns.Add("Player ID", -2, HorizontalAlignment.Left);
            this.transferFundsListView.Columns.Add("Nationality", -2, HorizontalAlignment.Left);

            // disable controls (until PC selected)
            this.transferFundsBtn.Enabled = false;
            this.transferFundsTextBox.Enabled = false;
        }

        /// <summary>
        /// Refreshes NPC list
        /// </summary>
        public void refreshNPCdisplay()
        {
            bool addItem = true;

            // remove any previously displayed characters
            this.npcDetailsTextBox.ReadOnly = true;
            this.npcDetailsTextBox.Text = "";

            // clear existing items in list
            this.npcListView.Items.Clear();

            // if choosing army leader, get army
            Army myArmy = null;
            if (this.function.Equals("leader"))
            {
                if (this.armyID != null)
                {
                    myArmy = Globals_Server.armyMasterList[this.armyID];
                }
            }

            ListViewItem myCharItem = null;

            // for royal gifts, ITERATE THROUGH PLAYERS
            if (this.function.Contains("royalGift"))
            {
                foreach (KeyValuePair<string, PlayerCharacter> thisPlayer in Globals_Server.pcMasterList)
                {
                    // only show 'played' PCs
                    if (thisPlayer.Value.playerID != null)
                    {
                        // don't show this player
                        if (thisPlayer.Value != Globals_Client.myPlayerCharacter)
                        {
                            // Create an item and subitems for each character

                            // name
                            myCharItem = new ListViewItem(thisPlayer.Value.firstName + " " + thisPlayer.Value.familyName);

                            // PC ID
                            myCharItem.SubItems.Add(thisPlayer.Value.charID);

                            // player ID
                            myCharItem.SubItems.Add(thisPlayer.Value.playerID);

                            // nationality
                            myCharItem.SubItems.Add(thisPlayer.Value.nationality.name);
                        }
                    }
                }
            }

            // else ITERATE THROUGH EMPLOYEES/FAMILY
            else
            {
                for (int i = 0; i < Globals_Client.myPlayerCharacter.myNPCs.Count; i++)
                {
                    // can only appoint males
                    if (Globals_Client.myPlayerCharacter.myNPCs[i].isMale)
                    {
                        // Create an item and subitems for each character

                        // name
                        myCharItem = new ListViewItem(Globals_Client.myPlayerCharacter.myNPCs[i].firstName + " " + Globals_Client.myPlayerCharacter.myNPCs[i].familyName);

                        // charID
                        myCharItem.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].charID);

                        // location
                        myCharItem.SubItems.Add(Globals_Client.myPlayerCharacter.myNPCs[i].location.id);

                        // if appointing leader, only add item to fiefsListView if is in same fief as army
                        if (this.function.Equals("leader"))
                        {
                            if (myArmy != null)
                            {
                                if (Globals_Client.myPlayerCharacter.myNPCs[i].location.id != myArmy.location)
                                {
                                    addItem = false;
                                }
                            }
                        }
                    }
                }
            }

            // check for null items
            if (myCharItem == null)
            {
                addItem = false;
            }

            if (addItem)
            {
                this.npcListView.Items.Add(myCharItem);
            }
        }

        /// <summary>
        /// Refreshes transfer funds PC list
        /// </summary>
        public void refreshTransferFundsDisplay()
        {
            // remove any previously displayed characters
            this.transferFundsTextBox.Text = "";
            this.transferFundsLabel.Text = "";

            // clear existing items in list
            this.transferFundsListView.Items.Clear();

            ListViewItem myPlayerItem = null;

            // iterate through players
            foreach (KeyValuePair<string, PlayerCharacter> thisPlayer in Globals_Server.pcMasterList)
            {
                // only show 'played' PCs
                if (thisPlayer.Value.playerID != null)
                {
                    // don't show this player
                    if (thisPlayer.Value != Globals_Client.myPlayerCharacter)
                    {
                        // Create an item and subitems for each character

                        // name
                        myPlayerItem = new ListViewItem(thisPlayer.Value.firstName + " " + thisPlayer.Value.familyName);

                        // PC ID
                        myPlayerItem.SubItems.Add(thisPlayer.Value.charID);

                        // player ID
                        myPlayerItem.SubItems.Add(thisPlayer.Value.playerID);

                        // nationality
                        myPlayerItem.SubItems.Add(thisPlayer.Value.nationality.name);

                        // add item to list
                        this.transferFundsListView.Items.Add(myPlayerItem);
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
            Character charToDisplay = null;

            string textToDisplay = "";

            if (npcListView.SelectedItems.Count > 0)
            {
                // set textbox to read only
                this.npcDetailsTextBox.ReadOnly = true;

                // get character
                if (this.function.Contains("royalGift"))
                {
                    if (Globals_Server.pcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Server.pcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }
                else
                {
                    if (Globals_Server.npcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        charToDisplay = Globals_Server.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }

                if (charToDisplay != null)
                {
                    // enable 'appoint this NPC' button
                    this.chooseNpcBtn.Enabled = true;

                    // get details
                    textToDisplay += this.displayCharacter(charToDisplay);

                    // display details
                    this.npcDetailsTextBox.Text = textToDisplay;
                }
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

            // get owner
            PlayerCharacter thisOwner = a.getOwner();

            // ID
            armyText += "ID: " + a.armyID + "\r\n\r\n";

            // nationality
            armyText += "Nationality: " + thisOwner.nationality.name + "\r\n\r\n";

            // owner
            armyText += "Owner: " + thisOwner.firstName + " " + thisOwner.familyName + " (" + thisOwner.charID + ")\r\n\r\n";

            // check if is your army (will effect display of troop numbers)
            if (thisOwner == Globals_Client.myPlayerCharacter)
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
        /// Retrieves character details for display
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="thisChar">Character whose information is to be displayed</param>
        public string displayCharacter(Character thisChar)
        {
            string charText = "";

            // ID
            charText += "ID: " + thisChar.charID + "\r\n";

            // name
            charText += "Name: " + thisChar.firstName + " " + thisChar.familyName + "\r\n";

            // age
            charText += "Age: " + thisChar.calcCharAge() + "\r\n";

            // nationality
            charText += "Nationality: " + thisChar.nationality + "\r\n";

            // health (& max. health)
            charText += "Health: ";
            if (!thisChar.isAlive)
            {
                charText += "Blimey, you're Dead!";
            }
            else
            {
                charText += thisChar.calculateHealth() + " (max. health: " + thisChar.maxHealth + ")";
            }
            charText += "\r\n";

            // any death modifiers (from skills)
            charText += "  (Death modifier from skills: " + thisChar.calcSkillEffect("death") + ")\r\n";

            // location
            charText += "Current location: " + thisChar.location.name + " (" + thisChar.location.province.name + ")\r\n";

            // if in process of auto-moving, display next hex
            if (thisChar.goTo.Count != 0)
            {
                charText += "Next Fief (if auto-moving): " + thisChar.goTo.Peek().id + "\r\n";
            }

            // language
            charText += "Language: " + thisChar.language + "\r\n";

            // days left
            charText += "Days remaining: " + thisChar.days + "\r\n";

            // stature
            charText += "Stature: " + thisChar.calculateStature() + "\r\n";
            charText += "  (base stature: " + thisChar.calculateStature(false) + " | modifier: " + thisChar.statureModifier + ")\r\n";

            // management rating
            charText += "Management: " + thisChar.management + "\r\n";

            // combat rating
            charText += "Combat: " + thisChar.combat + "\r\n";

            // skills list
            charText += "Skills:\r\n";
            for (int i = 0; i < thisChar.skills.Length; i++)
            {
                charText += "  - " + thisChar.skills[i].Item1.name + " (level " + thisChar.skills[i].Item2 + ")\r\n";
            }

            // gather additional NPC-specific information
            if (thisChar is NonPlayerCharacter)
            {
                charText += this.displayNonPlayerCharacter(thisChar as NonPlayerCharacter);
            }

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
                Character selectedCharacter = null;
                if (this.function.Contains("royalGift"))
                {
                    if (Globals_Server.pcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        selectedCharacter = Globals_Server.pcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }
                else
                {
                    if (Globals_Server.npcMasterList.ContainsKey(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        selectedCharacter = Globals_Server.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                    }
                }

                // get place (if royal gift)
                Place thisPlace = null;
                if (this.function.Contains("royalGift"))
                {
                    // get place type and id
                    string[] placeDetails = this.placeID.Split('|');

                    // get place associated with title
                    if (placeDetails[0].Equals("province"))
                    {
                        if (Globals_Server.provinceMasterList.ContainsKey(placeDetails[1]))
                        {
                            thisPlace = Globals_Server.provinceMasterList[placeDetails[1]];
                        }
                    }
                    else if (placeDetails[0].Equals("fief"))
                    {
                        if (Globals_Server.fiefMasterList.ContainsKey(placeDetails[1]))
                        {
                            thisPlace = Globals_Server.fiefMasterList[placeDetails[1]];
                        }
                    }
                }

                // appoint NPC to position
                // if appointing a bailiff
                if (this.function.Equals("bailiff"))
                {
                    // set the selected NPC as bailiff
                    Globals_Client.fiefToView.bailiff = selectedCharacter;

                    // refresh the fief information (in the main form)
                    this.parent.refreshFiefContainer(Globals_Client.fiefToView);
                }

                // if making a royal gift (title)
                else if (this.function.Equals("royalGiftTitle"))
                {
                    if (thisPlace != null)
                    {
                        if (selectedCharacter != null)
                        {
                            // set the selected NPC as title holder
                            Globals_Client.myPlayerCharacter.grantTitle(selectedCharacter, thisPlace);

                            // refresh the fief information (in the main form)
                            this.parent.refreshCurrentScreen();
                        }
                    }
                }

                // if making a royal gift (title)
                else if (this.function.Equals("royalGiftFief"))
                {
                    if (thisPlace != null)
                    {
                        if (selectedCharacter != null)
                        {
                            // process the change of ownership
                            (thisPlace as Fief).changeOwnership((selectedCharacter as PlayerCharacter), "voluntary");

                            // refresh the fief information (in the main form)
                            this.parent.refreshCurrentScreen();
                        }
                    }
                }

                // if appointing fief title holder
                else if (this.function.Equals("titleHolder"))
                {
                    // set the selected NPC as title holder
                    Globals_Client.myPlayerCharacter.grantTitle(selectedCharacter, Globals_Client.fiefToView);

                    // refresh the fief information (in the main form)
                    this.parent.refreshCurrentScreen();
                }

                // if appointing an army leader
                else
                {
                    // get army
                    if (this.armyID != null)
                    {
                        Army thisArmy = Globals_Server.armyMasterList[this.armyID];

                        thisArmy.assignNewLeader(selectedCharacter);

                        // refresh the army information (in the main form)
                        this.parent.refreshArmyContainer(thisArmy);
                    }
                    else
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("No army specified.");
                        }
                    }
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
                    barredChars[i].SubItems.Add(myBarredChar.nationality.name);

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
       public void refreshTransferDisplay()
        {
            ListViewItem thisDetachment = null;

            // get army
            Army thisArmy = Globals_Server.armyMasterList[this.armyID];

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
            // get character
            Character thisCharacter = null;

            if (Globals_Server.pcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                thisCharacter = Globals_Server.pcMasterList[this.barThisCharTextBox.Text];
            }
            else if (Globals_Server.npcMasterList.ContainsKey(this.barThisCharTextBox.Text))
            {
                thisCharacter = Globals_Server.npcMasterList[this.barThisCharTextBox.Text];
            }
            else
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Character could not be identified.  Please ensure charID is valid.");
                }
            }

            if (thisCharacter != null)
            {
                // add ID to barred characters
                Globals_Client.fiefToView.barredCharacters.Add(thisCharacter.charID);

                // check if is currently in keep, and remove if necessary
                if (thisCharacter.inKeep)
                {
                    thisCharacter.inKeep = false;
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show(thisCharacter.firstName + " " + thisCharacter.familyName + " has been ejected from the keep.");
                    }

                }

                // refresh display
                this.refreshBarredDisplay();
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
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("Selected character could not be identified.");
                    }
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
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("No detachments have been selected.");
                }
            }

            // if any detachments selected, proceed
            else
            {
                // check have minimum days necessary for transfer
                if (thisArmy.days < 10)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You don't have enough days left for this transfer.  Transfer cancelled.");
                    }
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
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("Poor organisation means that you have run out of days for this transfer.\r\nTry again next season.");
                        }
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
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show(toDisplay);
                        }
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
                if ((otherArmyOwner != Globals_Client.myPlayerCharacter) && (this.observer.armyID != null))
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
            bool proceed = true;

            // get armies
            Army attacker = this.observer.getArmy();
            Army defender = Globals_Server.armyMasterList[this.armiesListView.SelectedItems[0].SubItems[0].Text];

            // check has enough days to give battle (1)
            if (this.observer.days < 1)
            {
                if (Globals_Client.showMessages)
                {
                    System.Windows.Forms.MessageBox.Show("Your army doesn't have enough days left to give battle.");
                    proceed = false;
                }
            }
            else
            {
                // SIEGE INVOLVEMENT
                // check if defending army is the garrison in a siege
                string siegeID = defender.checkIfSiegeDefenderGarrison();
                if (siegeID != null)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("The defending army is currently being besieged and\r\ncannot be attacked.  Attack cancelled.");
                        proceed = false;
                    }
                }

                else
                {
                    // check if defending army is the additional defender in a siege
                    siegeID = defender.checkIfSiegeDefenderAdditional();
                    if (siegeID != null)
                    {
                        if (Globals_Client.showMessages)
                        {
                            System.Windows.Forms.MessageBox.Show("The defending army is currently being besieged and\r\ncannot be attacked.  Attack cancelled.");
                            proceed = false;
                        }
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
                                if (Globals_Client.showMessages)
                                {
                                    System.Windows.Forms.MessageBox.Show("Attack cancelled.");
                                    proceed = false;
                                }
                            }

                            // if choose to proceed
                            else
                            {
                                Siege thisSiege = null;
                                thisSiege = Globals_Server.siegeMasterList[siegeID];

                                // construct event description to be passed into siegeEnd
                                string siegeDescription = "On this day of Our Lord the forces of ";
                                siegeDescription += thisSiege.getBesiegingPlayer().firstName + " " + thisSiege.getBesiegingPlayer().familyName;
                                siegeDescription += " have chosen to abandon the siege of " + thisSiege.getFief().name;
                                siegeDescription += ". " + thisSiege.getDefendingPlayer().firstName + " " + thisSiege.getDefendingPlayer().familyName;
                                siegeDescription += " retains ownership of the fief.";

                                parent.siegeEnd(thisSiege, siegeDescription);
                            }
                        }
                    }
                }
            }

            if (proceed)
            {
                // let slip the dogs of war
                parent.giveBattle(attacker, defender);
            }

            // close form
            this.Close();
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the transferFundsListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferFundsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlayerCharacter transferTo = null;

            if (this.transferFundsListView.SelectedItems.Count > 0)
            {
                // get player
                if (Globals_Server.pcMasterList.ContainsKey(this.transferFundsListView.SelectedItems[0].SubItems[1].Text))
                {
                    transferTo = Globals_Server.pcMasterList[this.transferFundsListView.SelectedItems[0].SubItems[1].Text];
                }

                if (transferTo != null)
                {
                    // enable controls
                    this.transferFundsBtn.Enabled = true;
                    this.transferFundsTextBox.Enabled = true;

                    // get home fief
                    Fief home = Globals_Client.myPlayerCharacter.getHomeFief();

                    // get home treasury
                    int homeTreasury = home.getAvailableTreasury(true);

                    // populate transferFundsLabel to show treasury available
                    this.transferFundsLabel.Text = homeTreasury.ToString();
                }
            }
        }

        /// <summary>
        /// Responds to the click event of the transferFundsBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void transferFundsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                PlayerCharacter playerTo = null;

                if (this.transferFundsListView.SelectedItems.Count > 0)
                {
                    // get player
                    if (Globals_Server.pcMasterList.ContainsKey(this.transferFundsListView.SelectedItems[0].SubItems[1].Text))
                    {
                        playerTo = Globals_Server.pcMasterList[this.transferFundsListView.SelectedItems[0].SubItems[1].Text];
                    }

                    if (playerTo != null)
                    {
                        // get home fief
                        Fief fiefFrom = Globals_Client.myPlayerCharacter.getHomeFief();

                        // get playerTo's home fief
                        Fief fiefTo = playerTo.getHomeFief();

                        // get amount to transfer
                        int amount = Convert.ToInt32(this.transferFundsTextBox.Text);

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
                                this.parent.treasuryTransfer(fiefFrom, fiefTo, amount);

                                // close form
                                this.Close();
                            }
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
    }

}
