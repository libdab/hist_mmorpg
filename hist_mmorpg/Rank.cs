using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hist_mmorpg
{
    /// <summary>
    /// Class storing data on rank and title
    /// </summary>
    public class Rank
    {
        /// <summary>
        /// Holds rank ID
        /// </summary>
        public byte id { get; set; }
        /// <summary>
        /// Holds title name in various languages
        /// </summary>
        public TitleName[] title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte stature { get; set; }

        /// <summary>
        /// Constructor for Rank
        /// </summary>
        /// <param name="id">byte holding rank ID</param>
        /// <param name="ti">TitleName[] holding title name in various languages</param>
        /// <param name="stat">byte holding base stature for rank</param>
        public Rank(byte id, TitleName[] ti, byte stat)
        {

            // TODO: validation

            // validate stature
            if (stat < 1)
            {
                stat = 1;
            }
            else if (stat > 6)
            {
                stat = 6;
            }

            this.id = id;
            this.title = ti;
            this.stature = stat;

        }

        /// <summary>
        /// Constructor for Rank using Position_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
        /// <param name="pr">Position_Riak object to use as source</param>
        public Rank(Position_Riak pr)
        {
            this.id = pr.id;
            this.title = pr.title;
            this.stature = pr.stature;
        }

        /// <summary>
        /// Constructor for Rank taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Rank()
        {
        }

    }

    /// <summary>
    /// Class storing data on positions of power
    /// </summary>
    public class Position : Rank
    {
        /// <summary>
        /// Holds ID of the office holder
        /// </summary>
        public string officeHolder { get; set; }
        /// <summary>
        /// Holds nationality associated with the position
        /// </summary>
        public Nationality nationality { get; set; }

        /// <summary>
        /// Constructor for Position
        /// </summary>
        /// <param name="holder">string holding ID of the office holder</param>
        /// <param name="nat">Nationality associated with the position</param>
        public Position(byte id, TitleName[] ti, byte stat, string holder, Nationality nat)
            : base(id, ti, stat)
        {
            this.officeHolder = holder;
            this.nationality = nat;
        }

        /// <summary>
        /// Constructor for Position using Position_Riak object
        /// For use when de-serialising from Riak
        /// </summary>
        /// <param name="pr">Position_Riak object to use as source</param>
        public Position(Position_Riak pr)
            : base(pr: pr)
        {
            this.officeHolder = pr.officeHolder;
            // nationality to be inserted later
            this.nationality = null;
        }

        /// <summary>
        /// Constructor for Position taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Position()
        {
        }

        /// <summary>
        /// Sets the Position's officeHolder attribute to the ID of the supplied PlayerCharacter
        /// </summary>
        /// <param name="pc">PlayerCharacter being assigned to the Position</param>
        public void bestowPosition(PlayerCharacter pc)
        {
            this.officeHolder = pc.charID;
        }
    }

    /// <summary>
    /// Class used to convert Position to/from format suitable for Riak (JSON)
    /// </summary>
    public class Position_Riak
    {
        /// <summary>
        /// Holds rank ID
        /// </summary>
        public byte id { get; set; }
        /// <summary>
        /// Holds title name in various languages
        /// </summary>
        public TitleName[] title { get; set; }
        /// <summary>
        /// Holds base stature for this rank
        /// </summary>
        public byte stature { get; set; }
        /// <summary>
        /// Holds ID of the office holder
        /// </summary>
        public string officeHolder { get; set; }
        /// <summary>
        /// Holds ID of Nationality associated with the position
        /// </summary>
        public string nationality { get; set; }

        /// <summary>
        /// Constructor for Position_Riak
        /// </summary>
        /// <param name="pos">Position object to use as source</param>
        public Position_Riak(Position pos)
        {
            this.id = pos.id;
            this.title = pos.title;
            this.stature = pos.stature;
            this.officeHolder = pos.officeHolder;
            this.nationality = pos.nationality.natID;
        }

        /// <summary>
        /// Constructor for Position taking no parameters.
        /// For use when de-serialising from Riak
        /// </summary>
        public Position_Riak()
        {
        }
    }

    /// <summary>
    /// Struct storing data on title name
    /// </summary>
    public struct TitleName
    {
        /// <summary>
        /// Holds Language ID
        /// </summary>
        public string langID;
        /// <summary>
        /// Holds title name associated with specific language
        /// </summary>
        public string name;

        public TitleName(string lang, string nam)
        {
            this.langID = lang;
            this.name = nam;
        }
    }
}
