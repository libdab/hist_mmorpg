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
        /// <summary>
        /// Retrieves information for Character display screen
        /// </summary>
        /// <returns>String containing information to display</returns>
        /// <param name="ch">Character whose information is to be displayed</param>
        public string DisplayCharacter(Character ch)
        {
            string charText = "";
            bool isMyNPC = false;

            // check if is in player's myNPCs
            if (Globals_Client.myPlayerCharacter.myNPCs.Contains((ch as NonPlayerCharacter)) || (ch == Globals_Client.myPlayerCharacter))
            {
                isMyNPC = true;
            }

            // check to see if is army leader
            if (!String.IsNullOrWhiteSpace(ch.armyID))
            {
                charText += "NOTE: This character is currently LEADING AN ARMY (" + ch.armyID + ")\r\n\r\n";
            }

            // check to see if is under siege
            if (!String.IsNullOrWhiteSpace(ch.location.siege))
            {
                if (ch.inKeep)
                {
                    charText += "NOTE: This character is located in a KEEP UNDER SIEGE\r\n\r\n";
                }
            }

            // character ID
            charText += "Character ID: " + ch.charID + "\r\n";

            // player ID
            if (ch is PlayerCharacter)
            {
                if (!String.IsNullOrWhiteSpace((ch as PlayerCharacter).playerID))
                {
                    charText += "Player ID: " + (ch as PlayerCharacter).playerID + "\r\n";
                }
            }

            // name
            charText += "Name: " + ch.firstName + " " + ch.familyName + "\r\n";

            // age
            charText += "Age: " + ch.CalcAge() + "\r\n";

            // sex
            charText += "Sex: ";
            if (ch.isMale)
            {
                charText += "Male";
            }
            else
            {
                charText += "Female";
            }
            charText += "\r\n";

            // nationality
            charText += "Nationality: " + ch.nationality.name + "\r\n";

            if (ch is PlayerCharacter)
            {
                // home fief
                Fief homeFief = (ch as PlayerCharacter).GetHomeFief();
                charText += "Home fief: " + homeFief.name + " (" + homeFief.id + ")\r\n";

                // ancestral home fief
                Fief ancHomeFief = (ch as PlayerCharacter).GetAncestralHome();
                charText += "Ancestral Home fief: " + ancHomeFief.name + " (" + ancHomeFief.id + ")\r\n";
            }

            if (isMyNPC)
            {
                // health (& max. health)
                charText += "Health: ";
                if (!ch.isAlive)
                {
                    charText += "You're Dead!";
                }
                else
                {
                    charText += ch.CalculateHealth() + " (max. health: " + ch.maxHealth + ")";
                }
                charText += "\r\n";

                // any death modifiers (from traits)
                charText += "  (Death modifier from traits: " + ch.CalcTraitEffect("death") + ")\r\n";

                // virility
                charText += "Virility: " + ch.virility + "\r\n";
            }

            // location
            charText += "Current location: " + ch.location.name + " (" + ch.location.province.name + ")\r\n";

            // language
            charText += "Language: " + ch.language.GetName() + "\r\n";

            if (isMyNPC)
            {
                // days left
                charText += "Days remaining: " + ch.days + "\r\n";
            }

            // stature
            charText += "Stature: " + ch.CalculateStature() + "\r\n";
            charText += "  (base stature: " + ch.CalculateStature(false) + " | modifier: " + ch.statureModifier + ")\r\n";

            // management rating
            charText += "Management: " + ch.management + "\r\n";

            // combat rating
            charText += "Combat: " + ch.combat + "\r\n";

            // traits list
            charText += "Trait:\r\n";
            for (int i = 0; i < ch.traits.Length; i++)
            {
                charText += "  - " + ch.traits[i].Item1.name + " (level " + ch.traits[i].Item2 + ")\r\n";
            }

            // whether inside/outside the keep
            charText += "You are ";
            if (ch.inKeep)
            {
                charText += "inside";
            }
            else
            {
                charText += "outside";
            }
            charText += " the keep\r\n";

            // marital status
            NonPlayerCharacter thisSpouse = null;
            charText += "You are ";
            if (!String.IsNullOrWhiteSpace(ch.spouse))
            {
                // get spouse
                if (Globals_Game.npcMasterList.ContainsKey(ch.spouse))
                {
                    thisSpouse = Globals_Game.npcMasterList[ch.spouse];
                }

                if (thisSpouse != null)
                {
                    charText += "happily married to " + thisSpouse.firstName + " " + thisSpouse.familyName;
                    charText += " (ID: " + ch.spouse + ").";
                }
                else
                {
                    charText += "apparently married (but your spouse cannot be identified).";
                }
            }
            else
            {
                charText += "not married.";
            }
            charText += "\r\n";

            // if pregnant
            if (!ch.isMale)
            {
                charText += "You are ";
                if (!ch.isPregnant)
                {
                    charText += "not ";
                }
                charText += "pregnant\r\n";
            }

            // if spouse pregnant
            else
            {
                if (thisSpouse != null)
                {
                    if (thisSpouse.isPregnant)
                    {
                        charText += "Your spouse is pregnant (congratulations!)\r\n";
                    }
                    else
                    {
                        charText += "Your spouse is not pregnant\r\n";
                    }
                }
            }

            // engaged
            charText += "You are ";
            if (!String.IsNullOrWhiteSpace(ch.fiancee))
            {
                charText += "engaged to be married to ID " + ch.fiancee;
            }
            else
            {
                charText += "not engaged to be married";
            }
            charText += "\r\n";

            // father
            charText += "Father's ID: ";
            if (!String.IsNullOrWhiteSpace(ch.father))
            {
                charText += ch.father;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";

            // mother
            charText += "Mother's ID: ";
            if (!String.IsNullOrWhiteSpace(ch.mother))
            {
                charText += ch.mother;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";

            // head of family
            charText += "Head of family's ID: ";
            if (!String.IsNullOrWhiteSpace(ch.familyID))
            {
                charText += ch.familyID;
            }
            else
            {
                charText += "N/A";
            }
            charText += "\r\n";

            // gather additional information for PC/NPC
            bool isPC = ch is PlayerCharacter;
            if (isPC)
            {
                if (isMyNPC)
                {
                    charText += this.DisplayPlayerCharacter((PlayerCharacter)ch);
                }
            }
            else
            {
                charText += this.DisplayNonPlayerCharacter((NonPlayerCharacter)ch);
            }


            // if TITLES are to be shown
            if (this.characterTitlesCheckBox.Checked)
            {
                charText += "\r\n\r\n------------------ TITLES ------------------\r\n\r\n";

                // kingdoms
                foreach (string titleEntry in ch.myTitles)
                {
                    // get kingdom
                    Place thisPlace = null;

                    if (Globals_Game.kingdomMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.kingdomMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.GetName(ch.language).ToUpper() + " (rank " + thisPlace.rank.id + ") of ";
                        // get kingdom details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";
                    }
                }
                charText += "\r\n";

                // provinces
                charText += "PROVINCES:\r\n";
                int provCount = 0;
                foreach (string titleEntry in ch.myTitles)
                {
                    // get province
                    Place thisPlace = null;

                    if (Globals_Game.provinceMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.provinceMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.GetName(ch.language) + " (rank " + thisPlace.rank.id + ") of ";

                        // get province details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";

                        provCount++;
                    }
                }
                if (provCount < 1)
                {
                    charText += "None\r\n";
                }
                charText += "\r\n";

                // fiefs
                // provinces
                charText += "FIEFS:\r\n";
                foreach (string titleEntry in ch.myTitles)
                {
                    // get fief
                    Place thisPlace = null;

                    if (Globals_Game.fiefMasterList.ContainsKey(titleEntry))
                    {
                        thisPlace = Globals_Game.fiefMasterList[titleEntry];
                    }

                    if (thisPlace != null)
                    {
                        // get correct title
                        charText += thisPlace.rank.GetName((thisPlace as Fief).language) + " (rank " + thisPlace.rank.id + ") of ";
                        // get fief details
                        charText += thisPlace.name + " (" + titleEntry + ")\r\n";
                    }
                }
            }

            return charText;
        }

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
            this.characterTextBox.Text = this.DisplayCharacter(ch);

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
