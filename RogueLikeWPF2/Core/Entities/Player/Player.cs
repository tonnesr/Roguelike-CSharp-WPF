using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeWPF2.Core.Entities.Player
{
    /// <summary>
    /// 
    /// </summary>
    public class Player
    {
        // If I implement multiplayer
        // Change control scheme if there are more than one player.
        public int playerID; // id of current player object (this).
        public int playerIcon; // icon of this player.

        // Player properties
        public string @class = "no_class"; // class of player.

        public int health = 100; // health of player
        public int maxHealth = 100;
        public int mana = 100; // mana of player
        public int maxMana = 100; // mana of player
        public int exp = 0; // experience of player
        public int level = 1; // level of player
        public string name = "no_name"; // name of player

        public int X; // X pos of player
        public int Y; // Y pos of player

        // Not implemented yet:
        //public Item[] inventory;


        /// <summary>
        /// Player object for spawing a player.
        /// </summary>
        /// <param name="X">Spawn point x for player</param>
        /// <param name="Y">Spawn point y for player</param>
        /// <param name="name">Name of the player</param>
        public Player(int X, int Y, string name, string @class, int playerID, int playerIcon)
        {
            this.playerID = playerID; // ID for this player.
            this.playerIcon = playerIcon; // Icon for this player.
            this.name = name; // Name of this player.
            this.X = X; // Pos of this player.
            this.Y = Y; // Pos of this player.
            this.@class = @class;
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
        /// Move the player using arrows or wasd.
        /// </summary>
        public void Move()
        {

        }

        /// <summary>
        /// Attack, open doors, stairs, etc.
        /// </summary>
        public void Action()
        {

        }
    }
}
