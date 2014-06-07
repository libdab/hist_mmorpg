using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuickGraph;

namespace hist_mmorpg
{

    /// <summary>
    /// Class defining HexMapGraph
    /// </summary>
    class HexMapGraph
    {

        /// <summary>
        /// Map object (UndirectedGraph)
        /// </summary>
        public UndirectedGraph<Fief, TaggedUndirectedEdge<Fief, string>> myMap { get; set; }

        /// <summary>
        /// Constructor for HexMapGraph
        /// </summary>
        public HexMapGraph()
        {
            myMap = new UndirectedGraph<Fief, TaggedUndirectedEdge<Fief, string>>();
        }

        // TODO
        public bool addHexesAndRoute(Fief s, Fief t, string tag)
        {
            bool success = false;
            TaggedUndirectedEdge<Fief, string> myEdge = this.createEdge(s, t, tag);
            success = this.myMap.AddVerticesAndEdge(myEdge);
            return success;
        }

        // TODO
        public bool addRoute(Fief s, Fief t, string tag)
        {
            bool success = false;
            TaggedUndirectedEdge<Fief, string> myEdge = this.createEdge(s, t, tag);
            success = this.myMap.AddEdge(myEdge);
            return success;
        }

        // TODO
        public bool removeRoute(Fief s, string tag)
        {
            bool success = false;
            foreach (var e in this.myMap.Edges)
            {
                if (e.Source == s)
                {
                    if (e.Tag.Equals(tag))
                    {
                        this.myMap.RemoveEdge(e);
                        break;
                    }
                }
            }
            return success;
        }

        // TODO
        public bool addHex(Fief f)
        {
            bool success = false;
            success = this.myMap.AddVertex(f);
            return success;
        }

        // TODO
        public bool removeHex(Fief f)
        {
            bool success = false;
            success = this.myMap.RemoveVertex(f);
            return success;
        }

        // TODO
        public TaggedUndirectedEdge<Fief, string> createEdge(Fief s, Fief t, string tag)
        {
            TaggedUndirectedEdge<Fief, string> myEdge = new TaggedUndirectedEdge<Fief, string>(s, t, tag);
            return myEdge;
        }

        // TODO
        public Fief moveCharacter(Character ch, Fief f, string whereTo)
        {
            Fief newFief = null;

            f.removeCharacter(ch);
            foreach (var e in this.myMap.Edges)
            {
                if (e.Source == f)
                {
                    if (e.Tag.Equals(whereTo))
                    {
                        ch.location = e.Target;
                        newFief = e.Target;
                    }
                }
            }
            ch.location.addCharacter(ch);
            ch.inKeep = false;

            bool isPC = ch is PlayerCharacter;
            if (isPC)
            {
                this.moveEntourage((PlayerCharacter)ch, f);
            }

            return newFief;
        }

        // TODO
        public void moveEntourage(PlayerCharacter ch, Fief from)
        {
            Fief to = ch.location;
            for (int i = 0; i < ch.entourage.Count; i++)
            {
                from.removeCharacter(ch.entourage[i]);
                to.addCharacter(ch.entourage[i]);
                ch.entourage[i].location = to;
                ch.entourage[i].inKeep = false;
            }

        }

        /// <summary>
        /// Selects random adjoining hex (also equal chance to select current hex)
        /// </summary>
        /// <param name="npc">NonPlayerCharacter to be moved (or not)</param>
        /// <param name="f">Current location of NPC</param>
        public void randomNPCmove(NonPlayerCharacter npc, Fief f)
        {
            List<TaggedUndirectedEdge<Fief, string>> choices = new List<TaggedUndirectedEdge<Fief, string>>();

            // identify and store all target hexes from source hex
            foreach (var e in this.myMap.Edges)
            {
                if (e.Source == f)
                {
                    choices.Add(e);
                }
            }

            // generate random int between 0 and no. of targets
            Random rnd = new Random();
            int selection = rnd.Next(0, choices.Count);

            // get tag from selected edge
            string tag = choices[selection].Tag;

            if (selection != 0)
            {
                this.moveCharacter(npc, f, tag);
            }
        }

        // TODO
        public Fief getFief(Fief f, string direction)
        {
            Fief myFief = null;

            foreach (var e in this.myMap.Edges)
            {
                if (e.Source == f)
                {
                    if (e.Tag.Equals(direction))
                    {
                        myFief = e.Target;
                    }
                }
            }

            return myFief;

        }

    }

}
