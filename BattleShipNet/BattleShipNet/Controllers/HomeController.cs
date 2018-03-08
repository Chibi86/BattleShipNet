using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using GameEngine;
using BattleShipNet.Models;

namespace BattleShipNet.Controllers
{
    public class HomeController : Controller
    {
        private static GameBoards games;
        private static Dictionary<string, List<string>> messages;

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomeController()
        {
            if (games == null)
            {
                games = new GameBoards();
                messages = new Dictionary<string, List<string>>();
            }
        }

        /// <summary>
        /// Action for /Home/index.cshtml
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            InsertMessages();
            return View();
        }

        /// <summary>
        /// Action for /Home/newgame.cshtml
        /// </summary>
        /// <param name="name">Post name (string)</param>
        /// <param name="privateGame">Post private game (string)</param>
        /// <returns>View</returns>
        public ActionResult NewGame(string name = "", string privateGame = "")
        {
            InsertMessages();
            ViewBag.Name = name;
            ViewBag.Private = (privateGame == "true") ? true : false;
            return View(games);
        }

        /// <summary>
        /// Action for /Home/joingame.cshtml
        /// </summary>
        /// <param name="toDo">Join or new game (string)</param>
        /// <param name="name">Post name (string)</param>
        /// <param name="gameKey">Game key (string)</param>
        /// <returns>View</returns>
        public ActionResult JoinGame(string toDo = "join", string name = "", string gameKey = null)
        {
            InsertMessages();
            ViewBag.Name = name;
            ViewBag.ToDo = toDo;
            ViewBag.GameKey = gameKey;
            return View(games);
        }

        /// <summary>
        /// Action for take post to start a new gameboard
        /// </summary>
        /// <param name="name">Player name (string)</param>
        /// <param name="privateGame">If game are not allowed to join without game key (string)</param>
        /// <returns>Redirect</returns>
        public ActionResult StartNewGame(string name = "", string privateGame = "")
        {
            if ((name != null) && name.Length >= 2)
            {
                GameBoard gameBoard = new GameBoard();
                games.Add(gameBoard);
                gameBoard.Private = (privateGame == "true") ? true : false;
                gameBoard.Players[0].Name = name;
                Session["GameKey"] = gameBoard.GameKey;
                Session["PlayerId"] = 0;

                return RedirectToAction("Waiting", "Home");
            }

            AddMessage("danger", "Your name is too short and it should be mininum 2 characters.");
            return RedirectToAction("NewGame", "Home", new { name = name, privategame = privateGame });
        }

        /// <summary>
        /// Action for take post to join existing gameboard
        /// </summary>
        /// <param name="name">Player name (string)</param>
        /// <param name="gameKey">Game key to gameboard (string)</param>
        /// <returns>Redirect</returns>
        public ActionResult StartJoinGame(string name = "", string gameKey = "")
        {
            bool validate = true;

            if ((name == null) || name.Length < 2)
            {
                AddMessage("danger", "Your name is too short and it should be mininum 2 characters.");
                validate = false;
            }

            if((gameKey == null) || gameKey.Length != 6) 
            {
                AddMessage("danger", "Game key is invalid.");
                validate = false;
            }

            if (validate)
            {
                if (games.DoesItExist(gameKey))
                {
                    GameBoard game = games.Get(gameKey);

                    if (!game.Active)
                    {
                        game.Players[1].Name = name;
                        game.LastUpdate = DateTime.Now;
                        Session["GameKey"] = gameKey;
                        Session["PlayerId"] = 1;

                        return RedirectToAction("Game", "Home");
                    }
                    else
                    {
                        AddMessage("danger", "Game are already active.");
                    }
                }
                else if ((gameKey == null) || gameKey.Length != 6)
                {
                    AddMessage("danger", "Game with this game key doesn't exist.");
                }
            }
            
            return RedirectToAction("JoinGame", "Home", new { todo = "join", name = name, gamekey = gameKey });
        }

        /// <summary>
        /// Action for view /Home/waiting.cshtml
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Waiting()
        {
            // Get GameModel for session GameBoard
            GameModel gameModel = GetSessionGame();

            if(gameModel != null)
            {
                if (gameModel.Game.Active)
                {
                    return RedirectToAction("Game", "Home");
                }

                InsertMessages();

                return View(gameModel);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Action for EmailFriend
        /// </summary>
        /// <param name="email">Email to send mail to (string)</param>
        /// <returns>Redirect</returns>
        //public ActionResult EmailFriend(string email)
        //{
        //    // Get GameModel for session GameBoard
        //    GameModel gameModel = GetSessionGame();

        //    if (gameModel != null)
        //    {
        //        if (gameModel.Game.Active)
        //        {
        //            AddMessage("danger", "Email got never send, for a player had already join this game!");
        //            return RedirectToAction("Game", "Home");
        //        }

        //        string url = Url.Action("JoinGame", "Home", new { gamekey = Session["gamekey"] }, Request.Url.Scheme);

        //        try
        //        {
        //            MailModel.SendInviteEmail(email, gameModel.YourPlayer.Name, url);

        //            AddMessage("success", "A email with game url, was send to your friend!");
        //        }
        //        catch (Exception ex)
        //        {
        //            AddMessage("danger", ex.Message);
        //        }

        //        return RedirectToAction("Waiting", "Home");
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

        /// <summary>
        /// Action for view /Home/game.cshtml
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Game()
        {
            // Get GameModel for session GameBoard
            GameModel gameModel = GetSessionGame();

            if(gameModel != null)
            {
                if (!gameModel.Game.Active)
                {
                    AddMessage("danger", "Game has not started yet, still waiting on opponent player!");
                    return RedirectToAction("Waiting", "Home");
                }

                Player winner;
                if (gameModel.Game.IsGameEnd(out winner))
                {
                    return RedirectToAction("GameEnd", "Home");
                }

                InsertMessages();

                return View(gameModel);
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Action for view /Home/updategame.cshtml
        /// </summary>
        /// <returns>View</returns>
        public ActionResult UpdateGame(string lastupdate)
        {
            // Get GameModel for session GameBoard
            GameModel gameModel = GetSessionGame();

            if (gameModel != null)
            {
                DateTime lastUpdate;

                DateTime.TryParse(lastupdate, out lastUpdate);

                if ((lastUpdate == null) || gameModel.Game.LastUpdate > lastUpdate)
                {
                    Response.ContentType = "application/json";
                    return View(gameModel);
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.NotModified;
                    return null;
                }
            }

            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return null;
        }

        /// <summary>
        /// Action for view /Home/shoot.cshtml
        /// </summary>
        /// <param name="x">Position x to shoot on (int)</param>
        /// <param name="y">Position y to shoot on (int)</param>
        /// <returns>View/Redirect</returns>
        public ActionResult Shoot(string x, string y)
        {
            // If Request ask for json
            bool json = Request.Headers.Get("Accept").StartsWith("application/json");
            // Get GameModel for session GameBoard
            GameModel gameModel = GetSessionGame();

            if (gameModel != null)
            {
                try
                {
                    bool result = gameModel.Shoot(x, y);

                    ViewBag.Result = result;
                }
                catch (Exception ex)
                {
                    AddMessage("danger", ex.Message);

                    if (json)
                    {
                        InsertMessages();
                    }
                }

                if (json)
                {
                    Response.ContentType = "application/json";
                    return View(gameModel);
                }
                else {
                    return RedirectToAction("Game", "Home");
                }
            }

            if(json)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return null;
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Action for view /Home/gametestinfo.cshtml (Only to test game code)
        /// </summary>
        /// <returns>View</returns>
        //public ActionResult GameTestInfo()
        //{
        //    // Get GameModel for session GameBoard
        //    GameModel gameModel = GetSessionGame();

        //    if (gameModel != null)
        //    {
        //        return View(gameModel);
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

        /// <summary>
        /// Action for view /Home/gameend.cshtml
        /// </summary>
        /// <returns>View</returns>
        public ActionResult GameEnd()
        {
            // Get GameModel for session GameBoard
            GameModel gameModel = GetSessionGame();

            if (gameModel != null)
            {
                Player winner = new Player();
                if (gameModel.Game.IsGameEnd(out winner))
                {
                    ViewBag.Winner = (winner == gameModel.YourPlayer);
                    gameModel.YourPlayer.HaveSeenEndScreen = true;

                    RemoveSessionGame();
                    return View();
                }
                else
                {
                    return RedirectToAction("Game", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }
    
        /// <summary>
        /// Takes care to get GameBoard from session GameKey
        /// </summary>
        /// <returns>GameModel object with GameBoard</returns>
        private GameModel GetSessionGame()
        {
            if (Session["GameKey"] != null)
            {
                string gameKey = Session["GameKey"].ToString();

                if (games.DoesItExist(gameKey))
                {
                    return new GameModel(games.Get(gameKey));
                }
                else
                {
                    Session["GameKey"] = null;
                    Session["PlayerID"] = null;
                }
            }

            AddMessage("danger", "Fail to find a active game with your session.");

            return null;
        }

        /// <summary>
        /// Remove GameBoard with session GameKey
        /// </summary>
        private void RemoveSessionGame()
        {
            if (Session["GameKey"] != null)
            {
                string gameKey = Session["GameKey"].ToString();

                games.Remove(gameKey);
                Session["GameKey"] = null;
                Session["PlayerID"] = null;
            }
        }

        /// <summary>
        /// Method to add message to messages dictonary under right message type
        /// </summary>
        /// <param name="messageType">Message type (string)</param>
        /// <param name="message">Message (string)</param>
        private void AddMessage(string messageType, string message)
        {
            if (!messages.ContainsKey(messageType))
            {
                messages.Add(messageType, new List<string>());
            }

            messages[messageType].Add(message);
        }

        /// <summary>
        /// Method to help insert messages dictionary to Views
        /// </summary>
        private void InsertMessages()
        {
            ViewBag.Messages = messages;
            messages = new Dictionary<string, List<string>>();
        }
    }
}