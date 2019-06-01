using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RogueLikeWPF2.Core.Tiles;

namespace RogueLikeWPF2.Core.Entities.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class Player
    {
        // If I implement multiplayer
        // Change control scheme if there are more than one player.
        public int id; // id of current player object (this).
        public Tile tile;

        // Player properties
        public string @class = "no_class"; // class of player.

        public int health = 100; // health of player
        public int maxHealth = 100;
        public int mana = 100; // mana of player
        public int maxMana = 100; // mana of player
        public int exp = 0; // experience of player
        public int level = 1; // level of player
        public string name = "no_name"; // name of player

        // Not implemented yet:
        //public Item[] inventory;


        /// <summary>
        /// Player object for spawing a player.
        /// </summary>
        /// <param name="X">Spawn point x for player</param>
        /// <param name="Y">Spawn point y for player</param>
        /// <param name="name">Name of the player</param>
        /// <param name="class"></param>
        /// <param name="color"></param>
        /// <param name="function"></param>
        /// <param name="id"></param>
        /// <param name="symbol"></param>
        public Player(string name, string @class, int id, int X, int Y, int symbol, int color = 1, int function = 0, bool isWalkable = false, int type = 0)
        {
            this.id = id; // ID for this player, usefull if I implement more than one player.
            this.name = name; // Name of this player.
            this.@class = @class;

            // Player Tile
            this.tile = new Tile() {
                X = X,
                Y = Y,
                tileSymbol = symbol,
                tileColor = color,
                tileFunction = function,
                isWalkable = isWalkable,
                tileType = type
            };
        }
        
        /// <summary>
        /// Level up effect for the player.
        /// </summary>
        public void LevelUp()
        {
            this.level++;
            this.health = this.maxHealth;
            this.maxHealth += 10;
            this.mana = this.maxMana;
            this.maxMana += 10;
            this.exp = 0;
        }

        /// <summary>
        /// Attack, open doors, stairs, etc.
        /// </summary>
        public void Action()
        {

        }
    }
}
