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
        /// Constructor for SelectionForm
        /// </summary>
        /// <param name="par">Parent Form1 object</param>
        public SelectionForm(Form1 par)
        {
            // initialise form elements
            InitializeComponent();

            this.parent = par;

            // initialise NPC display
            this.initNPCdisplay();
        }

        /// <summary>
        /// Initialises NPC display screen
        /// </summary>
        private void initNPCdisplay()
        {
            // format list display
            this.setUpNpcList();

            // refresh information 
            this.refreshNPCdisplay();
        }

        /// <summary>
        /// Configures UI display for list of household NPCs
        /// </summary>
        public void setUpNpcList()
        {
            // add necessary columns
            this.npcListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.npcListView.Columns.Add("Companion", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes NPC list
        /// </summary>
        public void refreshNPCdisplay()
        {
            // remove any previously displayed characters
            this.npcDetailsTextBox.ReadOnly = true;
            this.npcDetailsTextBox.Text = "";

            // clear existing items in list
            this.npcListView.Items.Clear();

            ListViewItem[] myNPCs = new ListViewItem[this.parent.myChar.employees.Count];

            // iterates through employees
            for (int i = 0; i < this.parent.myChar.employees.Count; i++)
            {
                // Create an item and subitems for each character

                // name
                myNPCs[i] = new ListViewItem(this.parent.myChar.employees[i].name);

                // charID
                myNPCs[i].SubItems.Add(this.parent.myChar.employees[i].charID);

                // if is in player's entourage
                if (this.parent.myChar.employees[i].inEntourage)
                {
                    myNPCs[i].SubItems.Add("Yes");
                }

                // add item to fiefsListView
                this.npcListView.Items.Add(myNPCs[i]);
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

            // loop through employees
            for (int i = 0; i < this.parent.myChar.employees.Count; i++)
            {
                if (npcListView.SelectedItems.Count > 0)
                {
                    // find matching character
                    if (this.parent.myChar.employees[i].charID.Equals(this.npcListView.SelectedItems[0].SubItems[1].Text))
                    {
                        npcToDisplay = this.parent.myChar.employees[i];
                    }

                }

            }

            // retrieve and display character information
            if (npcToDisplay != null)
            {
                string textToDisplay = "";
                // get details
                textToDisplay += this.displayNPC(npcToDisplay);
                this.npcDetailsTextBox.ReadOnly = true;
                // display details
                this.npcDetailsTextBox.Text = textToDisplay;
            }
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
            charText += "Name: " + npc.name + "\r\n";

            // age
            charText += "Age: " + npc.age + "\r\n";

            // nationality
            charText += "Nationality: " + npc.nationality + "\r\n";

            // health (& max. health)
            charText += "Health: ";
            if (npc.health == 0)
            {
                charText += "Blimey, you're Dead!";
            }
            else
            {
                charText += npc.health + " (max. health: " + npc.maxHealth + ")";
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
            charText += "Stature: " + npc.stature + "\r\n";

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
        /// Responds to the click event of the chooseNpcBtn button
        /// which appoints the selected NPC as the bailiff of the fief displayed in the main UI
        /// and closes the child (this) form
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void chooseNpcBtn_Click(object sender, EventArgs e)
        {
            if (npcListView.SelectedItems.Count > 0)
            {
                // if the fief has an existing bailiff, relieve him of his duties
                if (this.parent.fiefToView.bailiff != null)
                {
                    this.parent.fiefToView.bailiff = null;
                }
                // set the selected NPC as bailiff
                this.parent.fiefToView.bailiff = this.parent.npcMasterList[this.npcListView.SelectedItems[0].SubItems[1].Text];
                // refresh the fief information (in the main form)
                this.parent.refreshFiefContainer(this.parent.fiefToView);
            }

            // close this form
            this.Close();
        }

    }

}
