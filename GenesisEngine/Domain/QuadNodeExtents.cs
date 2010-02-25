using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class QuadNodeExtents
    {
        public double West { get; private set; }
        public double East { get; private set; }
        public double North { get; private set; }
        public double South { get; private set; }

        public QuadNodeExtents(double west, double east, double north, double south)
        {
            this.West = west;
            this.East = east;
            this.North = north;
            this.South = south;
        }

        public double Width
        {
            get { return this.East - this.West; }
        }

        public IEnumerable<QuadNodeExtents> Split()
        {
            return new List<QuadNodeExtents>()
            {
                new QuadNodeExtents(this.West, this.East - (this.Width / 2), this.North, this.South - (this.Width / 2)),
                new QuadNodeExtents(this.West + (this.Width / 2), this.East, this.North, this.South - (this.Width / 2)),
                new QuadNodeExtents(this.West, this.East - (this.Width / 2), this.North + (this.Width / 2), this.South),
                new QuadNodeExtents(this.West + (this.Width / 2), this.East, this.North + (this.Width / 2), this.South)
            };
        }
    }
}
