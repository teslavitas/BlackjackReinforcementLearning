using Strategies21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine21;

namespace ConvnetSharpStrategy
{
    [Serializable]
    public class ConvnetStrategy : IStrategy21
    {
        private QAgent21 agent;
        public ConvnetStrategy(QAgent21 agent)
        {
            this.agent = agent;
        }

        public TurnOptions MakeTurn(GameState state)
        {
            return agent.Forward(state);
        }
    }
}
