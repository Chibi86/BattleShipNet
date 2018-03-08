using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace GameEngine
{
    public class GameBoard
    {
        private DateTime lastUpdate;

        public Player[] Players { get; }
        public string GameKey { get; set; }
        public bool Private { get; set; }
        public int Turn { get; private set; }

        /// <summary>
        /// Properties for lastUpdate - get & set
        /// </summary>
        public DateTime LastUpdate
        {
            get {
                return lastUpdate;
            }
            set
            {
                if(value != null)
                {
                    // Remove miliseconds, for easier to compare to string date
                    lastUpdate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
                }
            }
        }

        /// <summary>
        /// Properties to check if game is active - get
        /// </summary>
        public bool Active
        {
            get
            {
                return (!string.IsNullOrEmpty(Players[1].Name));
            }
        }

        /// <summary>
        /// Properties for check if both player has seen end screen
        /// </summary>
        public bool BothPlayerHasSeenEndScreen
        {
            get
            {
                return (Players[0].HaveSeenEndScreen && Players[1].HaveSeenEndScreen);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameBoard()
        {
            Players = new Player[2] {
                new Player(),
                new Player()
            };

            Turn = 1;
            LastUpdate = DateTime.Now;
        }

        /// <summary>
        /// Shoot on a square in one a Player's board 
        /// </summary>
        /// <param name="playerId">Id for target player (int)</param>
        /// <param name="position">Position to hit (Position)</param>
        /// <returns>Het result (bool)</returns>
        public bool Shoot(int playerId, Position position)
        {
            int shooter = (playerId == 0) ? 1 : 0;

            Log.Information("Shoot on player " + playerId + " at position x " + position.X + " y " + position.Y + " in game " + GameKey);

            // Check so there really are our turn
            if (playerId != Turn)
            {
                Player player = Players[playerId];

                // Check so Position is not already hit
                if (!player.IsPositionAlreadyHit(position))
                {
                    LastUpdate = DateTime.Now;

                    // Check if a Boat was hit, if not change turn to enemy
                    if (!player.IsABoatHit(position))
                    {
                        Log.Information("Miss on player " + playerId + " at position x " + position.X + " y " + position.Y + " in game " + GameKey);
                        Turn = playerId;
                        return false;
                    }

                    Log.Information("Boat hit on player " + playerId + " at position x " + position.X + " y " + position.Y + " in game " + GameKey);

                    return true;
                }
                else
                {
                    Log.Warning("Position was already hit on player " + playerId + " at position x " + position.X + " y " + position.Y + " in game " + GameKey);
                    throw new Exception("Position is already hit!");
                }
            }
            else
            {
                Log.Warning("Player " + shooter + " try to shoot not their turn in game " + GameKey);
                throw new Exception("Not your turn!");
            }
        }

        /// <summary>
        /// Check if game is over, if it's return winner
        /// </summary>
        /// <param name="winner">Return winner's Player object</param>
        /// <returns>Validate result (bool)</returns>
        public bool IsGameEnd(out Player winner)
        {
            if (Players[0].HasPlayerLost)
            {
                winner = Players[1];
                return true;
            }

            if (Players[1].HasPlayerLost)
            {
                winner = Players[0];
                return true;
            }

            winner = null;
            return false;
        }
    }
}
