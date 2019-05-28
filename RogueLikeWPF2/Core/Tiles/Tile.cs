using System.Drawing;

namespace RogueLikeWPF2.Core.Tiles
{
    public struct Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int tileSymbol { get; set; }
        public int tileColor { get; set; }
        public int tileFunction { get; set; }
        public bool isWalkable { get; set; }
    }
}