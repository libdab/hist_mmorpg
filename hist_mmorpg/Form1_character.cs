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
    /// Partial class for Form1, containing functionality specific to the character display
    /// </summary>
    partial class Form1
    {
        /*
        /// <summary>
        /// Retrieves PlayerCharacter-specific information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="pc">PlayerCharacter whose information is to be displayed</param>
        public string DisplayPlayerCharacter(PlayerCharacter pc)
        {
            string pcText = "";

            // whether outlawed
            pcText += "You are ";
            if (!pc.outlawed)
            {
                pcText += "not ";
            }
            pcText += "outlawed\r\n";

            // purse
            pcText += "Purse: " + pc.purse + "\r\n";

            // employees
            pcText += "Family and employees:\r\n";
            for (int i = 0; i < pc.myNPCs.Count; i++)
            {
                pcText += "  - " + pc.myNPCs[i].firstName + " " + pc.myNPCs[i].familyName;
                if (pc.myNPCs[i].inEntourage)
                {
                    pcText += " (travelling companion)";
                }
                pcText += "\r\n";
            }

            // owned fiefs
            pcText += "Fiefs owned:\r\n";
            for (int i = 0; i < pc.ownedFiefs.Count; i++)
            {
                pcText += "  - " + pc.ownedFiefs[i].name + "\r\n";
            }

            // owned provinces
            pcText += "Provinces owned:\r\n";
            for (int i = 0; i < pc.ownedProvinces.Count; i++)
            {
                pcText += "  - " + pc.ownedProvinces[i].name + "\r\n";
            }

            return pcText;
        }

        /// <summary>
        /// Retrieves NonPlayerCharacter-specific information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="npc">NonPlayerCharacter whose information is to be displayed</param>
        public string DisplayNonPlayerCharacter(NonPlayerCharacter npc)
        {
            string npcText = "";

            // boss
            if (!String.IsNullOrWhiteSpace(npc.employer))
            {
                npcText += "Hired by (ID): " + npc.employer + "\r\n";
            }

            // estimated salary level (if character is male)
            if (npc.isMale)
            {
                npcText += "Potential salary: " + npc.CalcSalary(Globals_Client.myPlayerCharacter) + "\r\n";

                // most recent salary offer from player (if any)
                npcText += "Last offer from this PC: ";
                if (npc.lastOffer.ContainsKey(Globals_Client.myPlayerCharacter.charID))
                {
                    npcText += npc.lastOffer[Globals_Client.myPlayerCharacter.charID];
                }
                else
                {
                    npcText += "N/A";
                }
                npcText += "\r\n";

                // current salary
                npcText += "Current salary: " + npc.salary + "\r\n";
            }

            return npcText;
        } */

        /// <summary>
        /// Retrieves information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="observed">Character whose information is to be displayed</param>
        /// <param name="observer">Character who is viewing this character's information</param>
        public string DisplayCharacter(Character observed, Character observer)
        {
            bool isMyNPC = false;

            // check if is in player's myNPCs
            if (observer is PlayerCharacter)
            {
                if ((observer as PlayerCharacter).myNPCs.Contains((observed as NonPlayerCharacter)) || (observed == Globals_Client.myPlayerCharacter))
                {
                    isMyNPC = true;
                }
            }

            string charText = observed.DisplayCharacter(isMyNPC, this.characterTitlesCheckBox.Checked, observer);

            return charText;
        }

        /// <summary>
        /// Refreshes main Character display screen
        /// </summary>
        /// <param name="ch">Character whose information is to be displayed</param>
        public void RefreshCharacterContainer(Character ch = null)
        {
            // if character not specified, default to player
            if (ch == null)
            {
                ch = Globals_Client.myPlayerCharacter;
            }

            // refresh Character display TextBox
            this.characterTextBox.ReadOnly = true;
            this.characterTextBox.Text = this.DisplayCharacter(ch, Globals_Client.myPlayerCharacter);

            // clear previous entries in Camp TextBox
            this.travelCampDaysTextBox.Text = "";

            // multimove button only enabled if is player or an employee
            if (ch != Globals_Client.myPlayerCharacter)
            {
                if (!Globals_Client.myPlayerCharacter.myNPCs.Contains(ch))
                {
                    this.travelMoveToBtn.Enabled = false;
                }
            }

            Globals_Client.containerToView = this.characterContainer;
            Globals_Client.containerToView.BringToFront();
        }
    }
}
