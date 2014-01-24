using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Controllers.Base;
using WebApp.Filters;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class TicTacToeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public Guid? MakeTurn(string gameId, int x, int y, int value)
        {
            //TODO: проверять кто сделал ход. И если пытаются два раза сходить, то агрится.
            //TODO: проверять не сделан ли ход в уже занятую клетку.
            if ((value != 0 && value != 1) || x < 0 || x > 2 || y < 0 || y > 2)
            {
                return null;
            }
            var turn = new TurnModel();
            turn.GameId = gameId == "" ? Guid.NewGuid() : new Guid(gameId);
            turn.X = x;
            turn.Y = y;
            turn.Value = value == 0 ? TicTacToeValue.X : TicTacToeValue.O;
            DataContext.Turns.Add(turn);
            DataContext.SaveChanges();
            return turn.GameId;
        }

        [AllowCrossSiteJson]
        public JsonResult GetState(string gameId)
        {
            if (gameId == "")
            {
                return null;
            }
            else
            {
                var turns = DataContext.Turns.Where(x => x.GameId == new Guid(gameId)).DefaultIfEmpty(null).ToList();
                if (turns == null)
                {
                    return null;
                }
                var state = new GameState { Turns = turns };
                if (turns.Count(x => x.X == x.Y && x.Value == TicTacToeValue.X) == 3 ||
                    turns.Count(x => x.X == 0 && x.Value == TicTacToeValue.X) == 3 ||
                    turns.Count(x => x.X == 1 && x.Value == TicTacToeValue.X) == 3 ||
                    turns.Count(x => x.X == 2 && x.Value == TicTacToeValue.X) == 3 ||
                    turns.Count(x => x.Y == 0 && x.Value == TicTacToeValue.X) == 3 ||
                    turns.Count(x => x.Y == 1 && x.Value == TicTacToeValue.X) == 3 ||
                    turns.Count(x => x.Y == 2 && x.Value == TicTacToeValue.X) == 3 ||
                    turns.Count(x => x.X == 2 - x.Y && x.Value == TicTacToeValue.X) == 3)
                {
                    state.State = "X";
                }
                else if (turns.Count(x => x.X == x.Y && x.Value == TicTacToeValue.O) == 3 ||
                    turns.Count(x => x.X == 0 && x.Value == TicTacToeValue.O) == 3 ||
                    turns.Count(x => x.X == 1 && x.Value == TicTacToeValue.O) == 3 ||
                    turns.Count(x => x.X == 2 && x.Value == TicTacToeValue.O) == 3 ||
                    turns.Count(x => x.Y == 0 && x.Value == TicTacToeValue.O) == 3 ||
                    turns.Count(x => x.Y == 1 && x.Value == TicTacToeValue.O) == 3 ||
                    turns.Count(x => x.Y == 2 && x.Value == TicTacToeValue.O) == 3 ||
                    turns.Count(x => x.X == 2 - x.Y && x.Value == TicTacToeValue.O) == 3)
                {
                    state.State = "O";
                }
                else if (turns.Count >= 9)
                {
                    state.State = "Draw";
                }
                else state.State = "Playing";
                return this.Json(state, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Nick()
        {
            return this.View();
        }


        #region Чатики
        public ActionResult Lee()
        {
            return View();
        }

        public ActionResult Sarychev()
        {
            return View();
        }

        public ActionResult Avramov()
        {
            return View();
        }

        public ActionResult Krakovetsky()
        {
            return View();
        }
        #endregion
    }

    public struct GameState
    {
        public string State;
        public List<TurnModel> Turns;
    }
}
