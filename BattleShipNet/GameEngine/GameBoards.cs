using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GameEngine
{
    public class GameBoards
    {
        public List<GameBoard> Games { get; }

        /// <summary>
        /// Properties to return List of GameBoards which is open to join
        /// </summary>
        public List<GameBoard> OpenGames
        {
            get
            {
                return Games.FindAll(game => !game.Private && !game.Active);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameBoards()
        {
            Games = new List<GameBoard>();
        }

        /// <summary>
        /// Add new GameBoard
        /// </summary>
        /// <param name="newGameBoard">GameBoard object to add</param>
        /// <returns>Result (bool)</returns>
        public bool Add(GameBoard newGameBoard)
        {
            if(newGameBoard != null)
            {
                string gameKey;

                do
                {
                    gameKey = GenerateGameKey();
                } while (DoesItExist(gameKey));

                newGameBoard.GameKey = gameKey;
                Games.Add(newGameBoard);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Return existing GameBoard by GameKey
        /// </summary>
        /// <param name="GameKey">Game key (string)</param>
        /// <returns>GameBoard object</returns>
        public GameBoard Get(string gameKey)
        {
            return Games.Find(board => board.GameKey == gameKey);
        }

        /// <summary>
        /// Remove GameBoard with GameKey from Games
        /// </summary>
        /// <param name="GameKey">Game key (string)</param>
        public void Remove(string gameKey)
        {
            for (int i = 0; i < Games.Count; i++)
            {
                GameBoard game = Games.ElementAt(i);
                if (game.GameKey == gameKey && game.BothPlayerHasSeenEndScreen)
                {
                    Games[i] = null;
                    Games.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Check if GameBoard exist
        /// </summary>
        /// <param name="GameKey">Game key (string)</param>
        /// <returns>Validation result</returns>
        public bool DoesItExist(string gameKey)
        {
            return (Get(gameKey) != null);
        }

        /// <summary>
        /// Generate and returns a random game key
        /// </summary>
        /// <returns>Random game key (string)</returns>
        private string GenerateGameKey()
        {
            string gameKey = Guid.NewGuid().ToString();
            gameKey = Regex.Replace(gameKey, @"[^0-9a-zA-Z]+", "");
            gameKey = gameKey.Substring(0, 6);

            return gameKey;
        }
    }
}
