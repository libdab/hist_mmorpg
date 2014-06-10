using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuickGraph;
using QuickGraph.Algorithms;

namespace hist_mmorpg
{

    /// <summary>
    /// Class defining HexMapGraph
    /// </summary>
    class HexMapGraph
    {

        /// <summary>
        /// Map object - AdjacencyGraph (i.e. directed)
        /// </summary>
        // public UndirectedGraph<Fief, TaggedUndirectedEdge<Fief, string>> myMap { get; set; }
        public AdjacencyGraph<Fief, TaggedEdge<Fief, string>> myMap { get; set; }
        /// <summary>
        /// Dictionary holding edge costs
        /// </summary>
        private Dictionary<TaggedEdge<Fief, string>, double> costs { get; set; }
        /// <summary>
        /// Constructor for HexMapGraph
        /// </summary>
        public HexMapGraph()
        {
            myMap = new AdjacencyGraph<Fief, TaggedEdge<Fief, string>>();
            costs = new Dictionary<TaggedEdge<Fief, string>, double>();
        }

        // TODO
        public bool addHexesAndRoute(Fief s, Fief t, string tag, double cost)
        {
            bool success = false;
            TaggedEdge<Fief, string> myEdge = this.createEdge(s, t, tag);
            success = this.myMap.AddVerticesAndEdge(myEdge);
            if (success)
            {
                this.addCost(myEdge, cost);
            }
            return success;
        }

        // TODO
        public bool addRoute(Fief s, Fief t, string tag, double cost)
        {
            bool success = false;
            TaggedEdge<Fief, string> myEdge = this.createEdge(s, t, tag);
            success = this.myMap.AddEdge(myEdge);
            if (success)
            {
                this.addCost(myEdge, cost);
            }
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
                        success = this.myMap.RemoveEdge(e);
                        if (success)
                        {
                            this.removeCost(e);
                        }
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
        public void addCost(TaggedEdge<Fief, string> e, double cost)
        {
            costs.Add(e, cost);
        }

        // TODO
        public void removeCost(TaggedEdge<Fief, string> e)
        {
            costs.Remove(e);
        }

        // TODO
        public TaggedEdge<Fief, string> createEdge(Fief s, Fief t, string tag)
        {
            TaggedEdge<Fief, string> myEdge = new TaggedEdge<Fief, string>(s, t, tag);
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
            for (int i = 0; i < ch.employees.Count; i++)
            {
                if (ch.employees[i].inEntourage)
                {
                    from.removeCharacter(ch.employees[i]);
                    to.addCharacter(ch.employees[i]);
                    ch.employees[i].location = to;
                    ch.employees[i].inKeep = false;
                }
            }

        }

        /// <summary>
        /// Selects random adjoining hex (also equal chance to select current hex)
        /// </summary>
        /// <param name="npc">NonPlayerCharacter to be moved (or not)</param>
        /// <param name="f">Current location of NPC</param>
        public void randomNPCmove(NonPlayerCharacter npc, Fief f)
        {
            List<TaggedEdge<Fief, string>> choices = new List<TaggedEdge<Fief, string>>();

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

        public string PrintShortestPath(Fief @from, Fief to)
        {
            string output = "";
            var edgeCost = AlgorithmExtensions.GetIndexer(costs);
            var tryGetPath = myMap.ShortestPathsDijkstra(edgeCost, @from);

            IEnumerable<TaggedEdge<Fief, string>> path;
            if (tryGetPath(to, out path))
            {
                output = PrintPath(@from, to, path);
               //  PrintPath(@from, to, path);
            }
            else
            {
                output = "No path found from " + @from.fiefID + " to " + to.fiefID;
                // Console.WriteLine("No path found from {0} to {1}.");
            }
            return output;
        }

        private static string PrintPath(Fief @from, Fief to, IEnumerable<TaggedEdge<Fief, string>> path)
        {
            string output = "";
            output += "Path found from " + @from.fiefID + " to " + to.fiefID + "is\r\n";
            foreach (var e in path)
                output += e.Tag + " to (" + e.Target.fiefID + ") ";
                // output += " > " + e.Target.fiefID;
            /* Console.Write("Path found from {0} to {1}: {0}", @from, to);
            foreach (var e in path)
                Console.Write(" > {0}", e.Target);
            Console.WriteLine(); */
            return output;
        }
    }

}
