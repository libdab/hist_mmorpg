using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on terrain
    /// </summary>
    public class Terrain
    {
        /// <summary>
        /// Holds terrain code
        /// </summary>
        public char terrainCode { get; set; }
        /// <summary>
        /// Holds terrain description
        /// </summary>
        public String description { get; set; }
        /// <summary>
        /// Holds terrain travel cost
        /// </summary>
        public double travelCost { get; set; }

        /// <summary>
        /// Constructor for Terrain
        /// </summary>
        /// <param name="c">char holding terrain code</param>
        /// <param name="desc">String holding terrain description</param>
        /// <param name="tc">double holding terrain travel cost</param>
        public Terrain(char c, string desc, double tc)
        {
            this.terrainCode = c;
            this.description = desc;
            this.travelCost = tc;
        }
    }
}
