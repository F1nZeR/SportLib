using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class TurnModel
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Guid GameId { get; set; }
        public TicTacToeValue Value { get; set; }
    }

    public enum TicTacToeValue
    {
        X,
        O
    }
}