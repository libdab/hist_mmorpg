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
