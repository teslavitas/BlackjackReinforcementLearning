using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine21;

namespace Strategies21
{
    public class StopAtNStratery : IStrategy21
    {
        private int n;
        public StopAtNStratery(int n)
        {
            this.n = n;
        }

        public TurnOptions MakeTurn(GameState state)
        {
            if (state.Score >= this.n)
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
