using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine21
{
    public class GameEngine
    {
        private List<Tuple<string, int>> cards;

        public GameEngine()
        {
            cards = new List<Tuple<string, int>>();
            cards.Add(new Tuple<string, int>("2", 2));
            cards.Add(new Tuple<string, int>("3", 3));
            cards.Add(new Tuple<string, int>("4", 4));
            cards.Add(new Tuple<string, int>("5", 5));
            cards.Add(new Tuple<string, int>("6", 6));
            cards.Add(new Tuple<string, int>("7", 7));
            cards.Add(new Tuple<string, int>("8", 8));
            cards.Add(new Tuple<string, int>("9", 9));
            cards.Add(new Tuple<string, int>("10", 10));
            cards.Add(new Tuple<string, int>("J", 2));
            cards.Add(new Tuple<string, int>("Q", 3));
            cards.Add(new Tuple<string, int>("K", 4));
            cards.Add(new Tuple<string, int>("A", 11));

            this.Rand = new Random();
        }

        public Random Rand { get; set; }

        public GameState ApplyTurn(TurnOptions turn, GameState state)
        {
            if (state.IsFinished)
            {
                throw new ArgumentException("Try to make a turn in a finished game");
            }

            if (turn == TurnOptions.FinishGame)
            {
                GameState result = new GameState()
                {
                    Score = state.Score,
                    IsFinished = true
                };
                return result;
            }
            else
            {
                return TakeACard(state);
            }
        }

        private GameState TakeACard(GameState state)
        {
            int card = this.GenerateCard();
            int newScore = state.Score + card;

            GameState result;
            if (newScore > 21)
            {
                result = new GameState()
                {
                    Score = 0,
                    IsFinished = true
                };
            }
            else
            {
                result = new GameState() { Score = newScore };
            }

             
            return result;
        }

        private int GenerateCard()
        {
            int index = this.Rand.Next(cards.Count);
            return this.cards[index].Item2;
        }
    }
}
