using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace hist_mmorpg
{

    /// <summary>
    /// Class storing data on province
    /// </summary>
    public class Province
    {

        /// <summary>
        /// Holds province ID
        /// </summary>
        public String provinceID { get; set; }
        /// <summary>
        /// Holds province name
        /// </summary>
        public String name { get; set; }
        /// <summary>
        /// Holds province overlord (ID)
        /// </summary>
        public uint overlord { get; set; }
        /// <summary>
        /// Holds province overlord tax rate
        /// </summary>
        public Double overlordTaxRate { get; set; }
        /// <summary>
        /// Holds province language
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// Constructor for Province
        /// </summary>
        /// <param name="id">String holding province ID</param>
        /// <param name="nam">String holding province name</param>
        /// <param name="olord">uint holding province overlord ID</param>
        /// <param name="otax">Double holding province overlord tax rate</param>
        /// <param name="lang">String holding province language code</param>
        public Province(String id, String nam, uint olord, Double otax, string lang)
        {

            // TODO: validate id = string E/AR,BK,CG,CH,CU,CW,DR,DT,DU,DV,EX,GL,HE,HM,KE,LA,LC,NF,NH,NO,NU,NW,OX,PM,SM,SR,ST,SU,SW,
            // SX,SY,WK,YS/00

            // validate nam length = 1-40
            if ((nam.Length < 1) || (nam.Length > 40))
            {
                throw new InvalidDataException("Province name must be between 1 and 40 characters in length");
            }

            // validate olord ID = 1-10000
            if ((olord < 1) || (olord > 10000))
            {
                throw new InvalidDataException("Province overlord ID must be an integer between 1 and 10000");
            }

            // validate otax = 0-100.00
            if ((otax < 0) || (otax > 100))
            {
                throw new InvalidDataException("Province overlord tax rate must be a double between 0 and 100");
            }

            // TODO: validate lang = string B,C,D,E,F,G,H,I,L/1-3

            this.provinceID = id;
            this.name = nam;
            this.overlord = olord;
            this.overlordTaxRate = otax;
            this.language = lang;

        }
    }
}
