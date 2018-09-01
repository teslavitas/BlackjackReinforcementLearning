using ConvnetSharp;
using DeepQLearning.DRLAgent;
using Engine21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvnetSharpStrategy
{
    [Serializable]
    public class QAgent21
    {

        public DeepQLearn Brain { get; private set; }

        public QAgent21(DeepQLearn brain)
        {
            this.Brain = brain;
        }

        public TurnOptions Forward(GameState state)
        {
            // in forward pass the agent simply behaves in the environment
            // create input to brain
            var input_array = new double[1];
            input_array[0] = state.Score / 21.0;//normalized for [0;1]

            Volume input = new Volume(1, 1, 1);
            input.w = input_array;

            // get action from brain
            var actionix = this.Brain.forward(input);
            var action = (TurnOptions)actionix;
            return action;
        }

        public void Backward(GameState state)
        {
            // in backward pass agent learns.
            // compute reward 

            double reward = 0.0;
            //don't like to lose
            if (state.IsFinished && state.Score == 0)
            {
                reward = -6.5;//-15/21.0;
            }
            //like to vin
            if (state.IsFinished && state.Score > 15)
            {
                reward = state.Score / 21.0;
            }

            // pass to brain for learning
            this.Brain.backward(reward);
        }
    }
}
