using System.Collections.Generic;

using RogueLikeWPF2.Core.Tiles;

namespace RogueLikeWPF2.Core.Levels
{
    public struct Level
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Tile[,] Tiles { get; set; }
    }
}