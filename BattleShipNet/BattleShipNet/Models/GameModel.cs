using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameEngine;

namespace BattleShipNet.Models
{
    public class GameModel
    {
        public GameBoard Game { get; }
        public List<string> Letters { get; }

        /// <summary>
        /// Properties returning session player id - get
        /// </summary>
        public int YourPlayerId
        {
            get
            {
                return (int)HttpContext.Current.Session["PlayerId"];
            }
        }

        /// <summary>
        /// Properties returning session's enemy's player id - get
        /// </summary>
        public int EnemyPlayerId
        {
            get
            {
                return (YourPlayerId == 1) ? 0 : 1;
            }
        }

        /// <summary>
        /// Properties returning session Player object - get
        /// </summary>
        public Player YourPlayer
        {
            get
            {
                return Game.Players[YourPlayerId];
            }
        }

        /// <summary>
        /// Properties returning enemy Player object - get
        /// </summary>
        public Player EnemyPlayer
        {
            get
            {
                return Game.Players[EnemyPlayerId];
            }
        }

        /// <summary>
        /// Constructor which takes GameBoard object
        /// </summary>
        /// <param name="newGameBoard">GameBoard object to make Model of</param>
        public GameModel(GameBoard newGameBoard)
        {
            Game = newGameBoard;

            Letters = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };
        }

        /// <summary>
        /// Return position information in form of css classes
        /// </summary>
        /// <param name="player">Player object to get position info from</param>
        /// <returns>String with valid css classes</returns>
        public string PositionsClasses(Player player, Position position, bool showBoats = false)
        {
            Square square = player.CheckPosition(position);

            string classes = "";

            if (square.HaveBoat)
            {
                if (showBoats)
                {
                    classes = " haveBoat";
                }

                if (square.HaveBeenHit)
                {
                    classes += " boatHit";
                }
            }
            else if (square.HaveBeenHit)
            {
                classes = " hit";
            }

            return classes;
        }

        /// <summary>
        /// Prepare Shoot before send it to GameEngine
        /// </summary>
        /// <param name="x">Position X (string)</param>
        /// <param name="y">Position Y (string)</param>
        /// <returns>Result</returns>
        public bool Shoot(string x, string y)
        {
            int positionX;
            int positionY;

            if (int.TryParse(x, out positionX) && int.TryParse(y, out positionY))
            {
                Position position;

                // Check so position is correct, then shoot. Catch trow error otherwish
                try
                {
                    position = new Position(positionX, positionY);
                }
                catch
                {
                    throw new Exception("You need to hit one existing position");
                }

                bool result = Game.Shoot(EnemyPlayerId, position);
                return result;
            }

            throw new Exception("You need to hit one existing position");
        }
    }
}