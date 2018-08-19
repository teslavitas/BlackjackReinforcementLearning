using Engine21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategies21
{
    public interface IStrategy21
    {
        TurnOptions MakeTurn(GameState state);
    }
}
