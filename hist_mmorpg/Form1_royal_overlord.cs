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
    /// Partial class for Form1, containing functionality specific to royalty and province overlords
    /// </summary>
    partial class Form1
    {
        /// <summary>
        /// Responds to the Click event of the provinceChallengeBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceChallengeBtn_Click(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                // get kingdom
                Kingdom targetKingdom = Globals_Game.kingdomMasterList[this.provinceProvListView.SelectedItems[0].SubItems[4].Text];

                // ensure aren't current owner
                if (Globals_Client.myPlayerCharacter == targetKingdom.owner)
                {
                    if (Globals_Client.showMessages)
                    {
                        System.Windows.Forms.MessageBox.Show("You are already the King of " + targetKingdom.name + "!");
                    }
                }

                // legitimate challenge
                else
                {
                    // create and send new OwnershipChallenge
                    OwnershipChallenge newChallenge = new OwnershipChallenge(Globals_Game.getNextOwnChallengeID(), Globals_Client.myPlayerCharacter.charID, "kingdom", targetKingdom.id);
                    Globals_Game.addOwnershipChallenge(newChallenge);

                    // create and send journal entry
                    // get interested parties
                    PlayerCharacter currentOwner = targetKingdom.owner;

                    // ID
                    uint entryID = Globals_Game.getNextJournalEntryID();

                    // date
                    uint year = Globals_Game.clock.currentYear;
                    byte season = Globals_Game.clock.currentSeason;

                    // location
                    string entryLoc = targetKingdom.id;

                    // journal entry personae
                    string allEntry = "all|all";
                    string currentOwnerEntry = currentOwner.charID + "|king";
                    string challengerEntry = Globals_Client.myPlayerCharacter.charID + "|pretender";
                    string[] entryPersonae = new string[] { currentOwnerEntry, challengerEntry, allEntry };

                    // entry type
                    string entryType = "depose_new";

                    // journal entry description
                    string description = "On this day of Our Lord a challenge for the crown of " + targetKingdom.name + " (" + targetKingdom.id + ")";
                    description += " has COMMENCED.  " + Globals_Client.myPlayerCharacter.firstName + " " + Globals_Client.myPlayerCharacter.familyName + " seeks to press his claim ";
                    description += "and depose the current king, His Highness " + currentOwner.firstName + " " + currentOwner.familyName + ", King of " + targetKingdom.name + ".";

                    // create and send a proposal (journal entry)
                    JournalEntry myEntry = new JournalEntry(entryID, year, season, entryPersonae, entryType, descr: description, loc: entryLoc);
                    Globals_Game.addPastEvent(myEntry);
                }

                this.provinceProvListView.Focus();
            }
        }

        /// <summary>
        /// Creates UI display for king's lists of provinces and fiefs
        /// </summary>
        public void setUpRoyalGiftsLists()
        {
            // add necessary columns
            // provinces
            this.royalGiftsProvListView.Columns.Add("Province ID", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Title Holder", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Last Tax Rate", -2, HorizontalAlignment.Left);
            this.royalGiftsProvListView.Columns.Add("Last Tax Income", -2, HorizontalAlignment.Left);
            // fiefs
            this.royalGiftsFiefListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Province", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Title Holder", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Last GDP", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Last Tax Income", -2, HorizontalAlignment.Left);
            this.royalGiftsFiefListView.Columns.Add("Current Treasury", -2, HorizontalAlignment.Left);
            // positions
            this.royalGiftsPositionListView.Columns.Add("ID", -2, HorizontalAlignment.Left);
            this.royalGiftsPositionListView.Columns.Add("Position", -2, HorizontalAlignment.Left);
            this.royalGiftsPositionListView.Columns.Add("Stature", -2, HorizontalAlignment.Left);
            this.royalGiftsPositionListView.Columns.Add("Holder", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Creates UI display for overlord's lists of provinces (and associated fiefs)
        /// </summary>
        public void setUpProvinceLists()
        {
            // add necessary columns
            // provinces
            this.provinceProvListView.Columns.Add("Province ID", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Last season tax rate", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Kingdom ID", -2, HorizontalAlignment.Left);
            this.provinceProvListView.Columns.Add("Kingdom Name", -2, HorizontalAlignment.Left);
            // fiefs
            this.provinceFiefListView.Columns.Add("Fief ID", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Name", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Owner", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Current GDP", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("Last season tax income", -2, HorizontalAlignment.Left);
            this.provinceFiefListView.Columns.Add("", -2, HorizontalAlignment.Left);
        }

        /// <summary>
        /// Refreshes royal gifts display
        /// </summary>
        public void refreshRoyalGiftsContainer()
        {
            // get PlayerCharacter (to allow for herald viewing king's finances)
            PlayerCharacter thisKing = null;
            if (Globals_Client.myPlayerCharacter.checkIsKing())
            {
                thisKing = Globals_Client.myPlayerCharacter;
            }
            else if (Globals_Client.myPlayerCharacter.checkIsHerald())
            {
                thisKing = Globals_Client.myPlayerCharacter.getKing();
            }

            if (thisKing != null)
            {
                // to store financial data
                int totGDP = 0;
                int provTaxInc = 0;
                int totTaxInc = 0;
                int totTreas = 0;
                double taxRate = 0;

                // disable controls until place selected in ListView
                this.royalGiftsGiftFiefBtn.Enabled = false;
                this.royalGiftsGrantTitleBtn.Enabled = false;
                this.royalGiftsRevokeTitleBtn.Enabled = false;
                this.royalGiftsPositionBtn.Enabled = false;
                this.royalGiftsPositionRemoveBtn.Enabled = false;

                // remove any previously displayed text

                // clear existing items in places lists
                this.royalGiftsProvListView.Items.Clear();
                this.royalGiftsFiefListView.Items.Clear();
                this.royalGiftsPositionListView.Items.Clear();

                // iterates through owned provinces and fiefs, adding information to appropriate ListView
                // PROVINCES
                foreach (Province thisProvince in thisKing.ownedProvinces)
                {
                    ListViewItem provItem = null;

                    // id
                    provItem = new ListViewItem(thisProvince.id);

                    // name
                    provItem.SubItems.Add(thisProvince.name);

                    // title holder
                    // get character
                    PlayerCharacter thisHolder = null;
                    //System.Windows.Forms.MessageBox.Show("Got here!");
                    if (Globals_Game.pcMasterList.ContainsKey(thisProvince.titleHolder))
                    {
                        thisHolder = Globals_Game.pcMasterList[thisProvince.titleHolder];
                    }

                    // title holder name & id
                    if (thisHolder != null)
                    {
                        provItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                    }
                    else
                    {
                        provItem.SubItems.Add("");
                    }

                    // last season tax rate and total tax income
                    foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                    {
                        if (fiefEntry.Value.province == thisProvince)
                        {
                            taxRate = fiefEntry.Value.keyStatsCurrent[12];
                            // update tax income total for province
                            provTaxInc += Convert.ToInt32(fiefEntry.Value.keyStatsCurrent[11]);
                        }
                    }

                    // add tax rate subitem
                    provItem.SubItems.Add(taxRate.ToString());

                    // add tax income subitem
                    provItem.SubItems.Add("£" + provTaxInc);

                    // update total tax income for all provinces
                    totTaxInc += provTaxInc;

                    if (provItem != null)
                    {
                        // add item to fiefsListView
                        this.royalGiftsProvListView.Items.Add(provItem);
                    }

                }

                // add listviewitem with total tax income (all provinces)
                string[] provItemTotalSubs = new string[] { "", "", "", "", "£" + totTaxInc };
                ListViewItem provItemTotal = new ListViewItem(provItemTotalSubs);
                this.royalGiftsProvListView.Items.Add(provItemTotal);

                // FIEFS
                totTaxInc = 0;
                foreach (Fief thisFief in thisKing.ownedFiefs)
                {
                    ListViewItem fiefItem = null;

                    // id
                    fiefItem = new ListViewItem(thisFief.id);

                    // name
                    fiefItem.SubItems.Add(thisFief.name);

                    // province name
                    fiefItem.SubItems.Add(thisFief.province.name);

                    // title holder
                    // get character
                    Character thisHolder = null;
                    if (Globals_Game.pcMasterList.ContainsKey(thisFief.titleHolder))
                    {
                        thisHolder = Globals_Game.pcMasterList[thisFief.titleHolder];
                    }
                    else if (Globals_Game.npcMasterList.ContainsKey(thisFief.titleHolder))
                    {
                        thisHolder = Globals_Game.npcMasterList[thisFief.titleHolder];
                    }

                    // title holder name & id
                    if (thisHolder != null)
                    {
                        fiefItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                    }
                    else
                    {
                        fiefItem.SubItems.Add("");
                    }

                    // gdp
                    fiefItem.SubItems.Add("£" + thisFief.keyStatsCurrent[1]);
                    // update GDP total
                    totGDP += Convert.ToInt32(thisFief.keyStatsCurrent[1]);

                    // last tax income
                    fiefItem.SubItems.Add("£" + thisFief.keyStatsCurrent[11]);
                    // update tax income total
                    totTaxInc += Convert.ToInt32(thisFief.keyStatsCurrent[11]);

                    // treasury
                    fiefItem.SubItems.Add("£" + thisFief.treasury);
                    // update treasury total
                    totTreas += thisFief.treasury;

                    if (fiefItem != null)
                    {
                        // add item to fiefsListView
                        this.royalGiftsFiefListView.Items.Add(fiefItem);
                    }

                }

                // add listviewitem with total GDP and tax income (all fiefs)
                string[] fiefItemTotalSubs = new string[] { "", "", "", "", "£" + totGDP, "£" + totTaxInc, "£" + totTreas };
                ListViewItem fiefItemTotal = new ListViewItem(fiefItemTotalSubs);
                this.royalGiftsFiefListView.Items.Add(fiefItemTotal);

                // POSITIONS
                foreach (KeyValuePair<byte, Position> thisPos in Globals_Game.positionMasterList)
                {
                    // only list posistions for this nationality
                    if (thisPos.Value.nationality == thisKing.nationality)
                    {
                        ListViewItem posItem = null;

                        // id
                        posItem = new ListViewItem(thisPos.Value.id.ToString());

                        // name
                        posItem.SubItems.Add(thisPos.Value.getName(thisKing.language));

                        // stature
                        posItem.SubItems.Add(thisPos.Value.stature.ToString());

                        // holder
                        // get character
                        Character thisHolder = null;
                        if (!String.IsNullOrWhiteSpace(thisPos.Value.officeHolder))
                        {
                            if (Globals_Game.pcMasterList.ContainsKey(thisPos.Value.officeHolder))
                            {
                                thisHolder = Globals_Game.pcMasterList[thisPos.Value.officeHolder];
                            }
                        }

                        // title holder name & id
                        if (thisHolder != null)
                        {
                            posItem.SubItems.Add(thisHolder.firstName + " " + thisHolder.familyName + "(" + thisHolder.charID + ")");
                        }
                        else
                        {
                            posItem.SubItems.Add(" - ");
                        }

                        if (posItem != null)
                        {
                            // add item to royalGiftsPositionListView
                            this.royalGiftsPositionListView.Items.Add(posItem);
                        }
                    }
                }

                Globals_Client.containerToView = this.royalGiftsContainer;
                Globals_Client.containerToView.BringToFront();
            }

        }

        /// <summary>
        /// Refreshes overlord province management display
        /// </summary>
        /// <param name="province">Province to display</param>
        public void refreshProvinceContainer(Province province = null)
        {
            // disable controls until place selected in ListView
            this.disableControls(this.provinceContainer.Panel1);

            // clear existing items in places lists
            this.provinceProvListView.Items.Clear();
            this.provinceFiefListView.Items.Clear();

            // iterates through provinces where the character holds the title, adding information to ListView
            foreach (string placeID in Globals_Client.myPlayerCharacter.myTitles)
            {
                ListViewItem provItem = null;

                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(placeID))
                {
                    thisProvince = Globals_Game.provinceMasterList[placeID];
                }

                if (thisProvince != null)
                {
                    // id
                    provItem = new ListViewItem(thisProvince.id);

                    // name
                    provItem.SubItems.Add(thisProvince.name);

                    // owner
                    PlayerCharacter thisOwner = thisProvince.owner;
                    if (thisOwner != null)
                    {
                        provItem.SubItems.Add(thisOwner.firstName + " " + thisOwner.familyName + " (" + thisOwner.charID + ")");
                    }
                    else
                    {
                        provItem.SubItems.Add("");
                    }

                    // last season tax rate
                    // get a fief
                    Fief thisFief = null;
                    foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
                    {
                        if (fiefEntry.Value.province == thisProvince)
                        {
                            thisFief = fiefEntry.Value;
                            break;
                        }
                    }

                    // get tax rate from fief
                    if (thisFief != null)
                    {
                        provItem.SubItems.Add(thisFief.keyStatsCurrent[12].ToString());
                    }

                    // kingdom ID
                    provItem.SubItems.Add(thisProvince.getCurrentKingdom().id);

                    // kingdom name
                    provItem.SubItems.Add(thisProvince.getCurrentKingdom().name);

                    // see if province to view has been passed in
                    if (province != null)
                    {
                        if (province == thisProvince)
                        {
                            provItem.Selected = true;
                        }
                    }

                    if (provItem != null)
                    {
                        // add item to fiefsListView
                        this.provinceProvListView.Items.Add(provItem);
                    }
                }

            }

            Globals_Client.containerToView = this.provinceContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Refreshes information the fief list in the overlord's province management display
        /// </summary>
        public void refreshProvinceFiefList(Province p)
        {
            bool underOccupation = false;

            // clear existing items in list
            this.provinceFiefListView.Items.Clear();

            foreach (KeyValuePair<string, Fief> fiefEntry in Globals_Game.fiefMasterList)
            {
                ListViewItem fiefItem = null;

                if (fiefEntry.Value.province == p)
                {
                    // check for enemy occupation
                    underOccupation = fiefEntry.Value.checkEnemyOccupation();

                    // id
                    fiefItem = new ListViewItem(fiefEntry.Value.id);

                    // name
                    fiefItem.SubItems.Add(fiefEntry.Value.name);

                    // owner name & id
                    if (fiefEntry.Value.owner != null)
                    {
                        fiefItem.SubItems.Add(fiefEntry.Value.owner.firstName + " " + fiefEntry.Value.owner.familyName + " (" + fiefEntry.Value.owner.charID + ")");
                    }
                    else
                    {
                        fiefItem.SubItems.Add("");
                    }

                    // GDP
                    if (!underOccupation)
                    {
                        fiefItem.SubItems.Add("£" + fiefEntry.Value.keyStatsCurrent[1]);
                    }
                    else
                    {
                        fiefItem.SubItems.Add("-");
                    }

                    // last tax income
                    if (!underOccupation)
                    {
                        fiefItem.SubItems.Add("£" + fiefEntry.Value.keyStatsCurrent[11]);
                    }
                    else
                    {
                        fiefItem.SubItems.Add("-");
                    }

                    // check if underOccupation message needed
                    if (underOccupation)
                    {
                        fiefItem.SubItems.Add("Under enemy occupation!");
                        fiefItem.ForeColor = Color.Red;
                    }

                    if (fiefItem != null)
                    {
                        // add item to fiefsListView
                        this.provinceFiefListView.Items.Add(fiefItem);
                    }
                }

            }
        }

        /// <summary>
        /// Responds to the click event of the royalGiftsToolStripMenuItem
        /// which displays royal gifts screen
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // display royal gifts screen
            this.refreshRoyalGiftsContainer();
            Globals_Client.containerToView = this.royalGiftsContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the ItemSelectionChanged event of either of the royal gifts ListViews
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            Province thisProv = null;
            Fief thisFief = null;
            Position thisPos = null;

            // get ListView tag
            ListView listview = sender as ListView;
            string whichView = listview.Tag.ToString();

            // check for and correct 'loop backs' due to listview item deselection
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        whichView = "fief";
                    }
                    else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        whichView = "position";
                    }
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        whichView = "province";
                    }
                    else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        whichView = "position";
                    }
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsPositionListView.SelectedItems.Count < 1)
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        whichView = "province";
                    }
                    else if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        whichView = "fief";
                    }
                }
            }

            // get selected place or position
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    // get province
                    if (Globals_Game.provinceMasterList.ContainsKey(this.royalGiftsProvListView.SelectedItems[0].SubItems[0].Text))
                    {
                        thisProv = Globals_Game.provinceMasterList[this.royalGiftsProvListView.SelectedItems[0].SubItems[0].Text];
                    }
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    // get fief
                    if (Globals_Game.fiefMasterList.ContainsKey(this.royalGiftsFiefListView.SelectedItems[0].SubItems[0].Text))
                    {
                        thisFief = Globals_Game.fiefMasterList[this.royalGiftsFiefListView.SelectedItems[0].SubItems[0].Text];
                    }
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    // get position
                    if (Globals_Game.positionMasterList.ContainsKey(Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)))
                    {
                        thisPos = Globals_Game.positionMasterList[Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)];
                    }
                }
            }

            // deselect any selected items in other listView
            if (whichView.Equals("province"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsFiefListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsPositionListView.SelectedItems[0].Selected = false;
                }
            }
            else if (whichView.Equals("fief"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsProvListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsPositionListView.SelectedItems[0].Selected = false;
                }
            }
            else if (whichView.Equals("position"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsProvListView.SelectedItems[0].Selected = false;
                }
                else if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    this.royalGiftsFiefListView.SelectedItems[0].Selected = false;
                }
            }

            // set button text and tag
            if (whichView.Equals("province"))
            {
                this.royalGiftsGrantTitleBtn.Text = "Grant Province Title";
                this.royalGiftsRevokeTitleBtn.Text = "Revoke Province Title";
                if (thisProv != null)
                {
                    this.royalGiftsGrantTitleBtn.Tag = "province|" + thisProv.id;
                    this.royalGiftsRevokeTitleBtn.Tag = "province|" + thisProv.id;
                }
            }
            else if (whichView.Equals("fief"))
            {
                this.royalGiftsGrantTitleBtn.Text = "Grant Fief Title";
                this.royalGiftsRevokeTitleBtn.Text = "Revoke Fief Title";
                if (thisFief != null)
                {
                    this.royalGiftsGrantTitleBtn.Tag = "fief|" + thisFief.id;
                    this.royalGiftsRevokeTitleBtn.Tag = "fief|" + thisFief.id;
                    this.royalGiftsGiftFiefBtn.Tag = "fief|" + thisFief.id;
                }
            }
            else if (whichView.Equals("position"))
            {
                if (thisPos != null)
                {
                    this.royalGiftsPositionBtn.Tag = thisPos.id;
                }
            }

            // enable/disable controls as appropriate

            // check to see if viewer is king or herald
            if (!Globals_Client.myPlayerCharacter.checkIsHerald())
            {
                // provinces
                if (whichView.Equals("province"))
                {
                    if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                    {
                        if (thisProv != null)
                        {
                            // revoke title button
                            if (thisProv.titleHolder.Equals(Globals_Client.myPlayerCharacter.charID))
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = false;
                            }
                            else
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = true;
                            }
                        }
                    }

                    // 'grant title' button
                    this.royalGiftsGrantTitleBtn.Enabled = true;

                    // gift fief button
                    this.royalGiftsGiftFiefBtn.Enabled = false;

                    // position buttons
                    this.royalGiftsPositionBtn.Enabled = false;
                    this.royalGiftsPositionRemoveBtn.Enabled = false;
                }

                // fiefs
                else if (whichView.Equals("fief"))
                {
                    if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                    {
                        if (thisFief != null)
                        {
                            // revoke title button
                            if (thisFief.titleHolder.Equals(Globals_Client.myPlayerCharacter.charID))
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = false;
                            }
                            else
                            {
                                this.royalGiftsRevokeTitleBtn.Enabled = true;
                            }

                            // gift fief button
                            this.royalGiftsGiftFiefBtn.Enabled = true;

                            // 'grant title' button
                            this.royalGiftsGrantTitleBtn.Enabled = true;

                            // position buttons
                            this.royalGiftsPositionBtn.Enabled = false;
                            this.royalGiftsPositionRemoveBtn.Enabled = false;
                        }
                    }
                }

                // positions
                else if (whichView.Equals("position"))
                {
                    if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
                    {
                        if (thisPos != null)
                        {
                            // bestow position button
                            this.royalGiftsPositionBtn.Enabled = true;

                            // remove position button, enabled if there is a current holder
                            if (!String.IsNullOrWhiteSpace(thisPos.officeHolder))
                            {
                                this.royalGiftsPositionRemoveBtn.Enabled = true;
                            }
                            else
                            {
                                this.royalGiftsPositionRemoveBtn.Enabled = false;
                            }

                            // revoke title button
                            this.royalGiftsRevokeTitleBtn.Enabled = false;

                            // gift fief button
                            this.royalGiftsGiftFiefBtn.Enabled = false;

                            // always enable 'grant title' button
                            this.royalGiftsGrantTitleBtn.Enabled = false;
                        }
                    }
                }
            }

            // don't enable controls if herald
            else
            {
                this.royalGiftsGrantTitleBtn.Enabled = false;
                this.royalGiftsRevokeTitleBtn.Enabled = false;
                this.royalGiftsGiftFiefBtn.Enabled = false;
                this.royalGiftsPositionBtn.Enabled = false;
            }

            // give focus back to appropriate listview
            if (whichView.Equals("province"))
            {
                this.royalGiftsProvListView.Focus();
            }
            else if (whichView.Equals("fief"))
            {
                this.royalGiftsFiefListView.Focus();
            }
            else if (whichView.Equals("position"))
            {
                this.royalGiftsPositionListView.Focus();
            }
        }

        /// <summary>
        /// Responds to the click event of either the royalGiftsGrantTitleBtn button
        /// or the royalGiftsGiftFiefBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsBtn_Click(object sender, EventArgs e)
        {
            // get gift type and place id from button tag and name
            Button button = sender as Button;
            string giftID = button.Tag.ToString();
            string giftType = null;
            if (button.Name.ToString().Equals("royalGiftsGrantTitleBtn"))
            {
                giftType = "royalGiftTitle";
            }
            else if (button.Name.ToString().Equals("royalGiftsGiftFiefBtn"))
            {
                giftType = "royalGiftFief";
            }
            else if (button.Name.ToString().Equals("royalGiftsPositionBtn"))
            {
                giftType = "royalGiftPosition";
            }

            if (!String.IsNullOrWhiteSpace(giftType))
            {
                // check for previously opened SelectionForm and close if necessary
                if (Application.OpenForms.OfType<SelectionForm>().Any())
                {
                    Application.OpenForms.OfType<SelectionForm>().First().Close();
                }

                // open new SelectionForm
                SelectionForm royalGiftSelection = null;
                // if gifting place or place title
                if (!giftType.Equals("royalGiftPosition"))
                {
                    royalGiftSelection = new SelectionForm(this, giftType, place: giftID);
                }

                // if bestowing position
                else
                {
                    royalGiftSelection = new SelectionForm(this, giftType, posID: Convert.ToByte(giftID));
                }
                royalGiftSelection.Show();
            }

        }

        /// <summary>
        /// Responds to the Click event of the manageProvincesToolStripMenuItem
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void manageProvincesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // clear existing provinceToView
            Globals_Client.provinceToView = null;

            // display royal gifts screen
            this.refreshProvinceContainer();

            // display household affairs screen
            Globals_Client.containerToView = this.provinceContainer;
            Globals_Client.containerToView.BringToFront();
        }

        /// <summary>
        /// Responds to the SelectedIndexChanged event of the provinceProvListView
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceProvListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisProvince = Globals_Game.provinceMasterList[this.provinceProvListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisProvince != null)
                {
                    // refresh fief list
                    this.refreshProvinceFiefList(thisProvince);

                    // populate provinceTaxTextBox
                    this.provinceTaxTextBox.Text = thisProvince.taxRate.ToString();

                    // enable controls
                    this.provinceTaxBtn.Enabled = true;
                    this.provinceTaxTextBox.Enabled = true;
                    this.provinceChallengeBtn.Enabled = true;

                    // set provinceToView
                    Globals_Client.provinceToView = thisProvince;
                }
            }
        }

        /// <summary>
        /// Responds to the Click event of the provinceTaxBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void provinceTaxBtn_Click(object sender, EventArgs e)
        {
            if (this.provinceProvListView.SelectedItems.Count > 0)
            {
                bool rateChanged = false;

                // get province
                Province thisProvince = null;
                if (Globals_Game.provinceMasterList.ContainsKey(this.provinceProvListView.SelectedItems[0].SubItems[0].Text))
                {
                    thisProvince = Globals_Game.provinceMasterList[this.provinceProvListView.SelectedItems[0].SubItems[0].Text];
                }

                if (thisProvince != null)
                {
                    // keep track of whether tax has changed
                    double originalRate = thisProvince.taxRate;

                    try
                    {
                        // get new rate
                        Double newTax = Convert.ToDouble(this.provinceTaxTextBox.Text);

                        // if rate changed, commit new rate
                        if (newTax != originalRate)
                        {
                            // adjust tax rate
                            thisProvince.adjustTaxRate(newTax);
                            rateChanged = true;

                            // display confirmation message
                            if (Globals_Client.showMessages)
                            {
                                System.Windows.Forms.MessageBox.Show("Province tax rate changed.");
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
                        if (rateChanged)
                        {
                            // refresh display
                            this.refreshCurrentScreen();
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Responds to the Click event of the royalGiftsPositionRemoveBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsPositionRemoveBtn_Click(object sender, EventArgs e)
        {
            if (this.royalGiftsPositionListView.SelectedItems.Count > 0)
            {
                // get position
                Position thisPos = null;
                if (Globals_Game.positionMasterList.ContainsKey(Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)))
                {
                    thisPos = Globals_Game.positionMasterList[Convert.ToByte(this.royalGiftsPositionListView.SelectedItems[0].SubItems[0].Text)];
                }

                if (thisPos != null)
                {
                    // get current holder
                    PlayerCharacter currentHolder = thisPos.getOfficeHolder();

                    // remove from position
                    if (currentHolder != null)
                    {
                        thisPos.removeFromOffice(currentHolder);

                        // refresh screen
                        this.refreshCurrentScreen();
                    }
                }

            }
        }

        /// <summary>
        /// Responds to the Click event of the royalGiftsRevokeTitleBtn button
        /// </summary>
        /// <param name="sender">The control object that sent the event args</param>
        /// <param name="e">The event args</param>
        private void royalGiftsRevokeTitleBtn_Click(object sender, EventArgs e)
        {
            // get place type and id from button tag
            Button button = sender as Button;
            string[] placeDetails = button.Tag.ToString().Split('|');

            // fiefs
            if (placeDetails[0].Equals("fief"))
            {
                if (this.royalGiftsFiefListView.SelectedItems.Count > 0)
                {
                    // get fief
                    Fief thisFief = null;
                    if (Globals_Game.fiefMasterList.ContainsKey(placeDetails[1]))
                    {
                        thisFief = Globals_Game.fiefMasterList[placeDetails[1]];
                    }

                    // reassign title
                    if (thisFief != null)
                    {
                        Globals_Client.myPlayerCharacter.grantTitle(Globals_Client.myPlayerCharacter, thisFief);

                        // refresh screen
                        this.refreshCurrentScreen();
                    }
                }
            }

            // provinces
            else if (placeDetails[0].Equals("province"))
            {
                if (this.royalGiftsProvListView.SelectedItems.Count > 0)
                {
                    // get province
                    Province thisProv = null;
                    if (Globals_Game.provinceMasterList.ContainsKey(placeDetails[1]))
                    {
                        thisProv = Globals_Game.provinceMasterList[placeDetails[1]];
                    }

                    // reassign title
                    if (thisProv != null)
                    {
                        Globals_Client.myPlayerCharacter.grantTitle(Globals_Client.myPlayerCharacter, thisProv);

                        // refresh screen
                        this.refreshCurrentScreen();
                    }
                }
            }
        }
    }
}
