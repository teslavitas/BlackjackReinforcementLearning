using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine21;

namespace Strategies21
{
    public class RandomStratery : IStrategy21
    {
        private Random rand = new Random();

        public TurnOptions MakeTurn(GameState state)
        {
            if (rand.Next() % 2 == 0)
            {
                return TurnOptions.FinishGame;
            }
            else
            {
                return TurnOptions.TakeACard;
            }
        }
    }
}
