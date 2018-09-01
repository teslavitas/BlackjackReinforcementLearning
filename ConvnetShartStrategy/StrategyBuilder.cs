using ConvnetSharp;
using DeepQLearning.DRLAgent;
using Engine21;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConvnetSharpStrategy
{
    public class StrategyBuilder
    {
        public static ConvnetStrategy BuildStrategy(GameEngine engine21)
        {
            var agent = TrainAgent(engine21);
            return new ConvnetStrategy(agent);
        }

        private static QAgent21 TrainAgent(GameEngine engine21)
        {
            var num_inputs = 1; // current score
            var num_actions = 2; // take a card or finish game
            var temporal_window = 0; // amount of temporal memory. 0 = agent lives in-the-moment :)
            var network_size = num_inputs * temporal_window + num_actions * temporal_window + num_inputs;

            var layer_defs = new List<LayerDefinition>();

            // the value function network computes a value of taking any of the possible actions
            // given an input state. Here we specify one explicitly the hard way
            // but user could also equivalently instead use opt.hidden_layer_sizes = [20,20]
            // to just insert simple relu hidden layers.
            layer_defs.Add(new LayerDefinition { type = "input", out_sx = 1, out_sy = 1, out_depth = network_size });
            //layer_defs.Add(new LayerDefinition { type = "fc", num_neurons = 21, activation = "relu" });
            //layer_defs.Add(new LayerDefinition { type = "fc", num_neurons = 96, activation = "relu" });
            //layer_defs.Add(new LayerDefinition { type = "fc", num_neurons = 96, activation = "relu" });
            layer_defs.Add(new LayerDefinition { type = "regression", num_neurons = num_actions });

            // options for the Temporal Difference learner that trains the above net
            // by backpropping the temporal difference learning rule.
            //var opt = new Options { method="sgd", learning_rate=0.01, l2_decay=0.001, momentum=0.9, batch_size=10, l1_decay=0.001 };
            var opt = new Options { method = "adadelta", l2_decay = 0.001, batch_size = 10 };

            var tdtrainer_options = new TrainingOptions();
            tdtrainer_options.temporal_window = temporal_window;
            tdtrainer_options.experience_size = 3000;// size of experience replay memory
            tdtrainer_options.start_learn_threshold = 1000;// number of examples in experience replay memory before we begin learning
            tdtrainer_options.gamma = 1.0;// gamma is a crucial parameter that controls how much plan-ahead the agent does. In [0,1]
            tdtrainer_options.learning_steps_total = 15000;// number of steps we will learn for
            tdtrainer_options.learning_steps_burnin = 1000;// how many steps of the above to perform only random actions (in the beginning)?
            tdtrainer_options.epsilon_min = 0.01;// what epsilon value do we bottom out on? 0.0 => purely deterministic policy at end
            tdtrainer_options.epsilon_test_time = 0.00;// what epsilon to use at test time? (i.e. when learning is disabled)
            tdtrainer_options.layer_defs = layer_defs;
            tdtrainer_options.options = opt;

            var brain = new DeepQLearn(num_inputs, num_actions, tdtrainer_options);
            var agent = new QAgent21(brain);

            int accumulatedScore = 0;
            int accumulatedGameLenght = 0;
            int gamesInAccumulatedScore = 0;
            int batchSize = 5000;
            int total = 0;
            Stream bestAgentSerialized = new MemoryStream();
            double bestBatchScore = double.MinValue;

            while (total < 50000)
            {
                GameState state = new GameState();

                while (!state.IsFinished)
                {
                    TurnOptions action = agent.Forward(state);
                    //if (action == TurnOptions.FinishGame)
                    //{
                    //    Console.WriteLine($"finish at {state.Score}");
                    //}
                    GameState newState = engine21.ApplyTurn(action, state);

                    agent.Backward(newState);
                    state = newState;

                    accumulatedGameLenght++;
                }

                accumulatedScore += state.Score;
                gamesInAccumulatedScore++;

                total++;
                if (gamesInAccumulatedScore == batchSize)
                {
                    double batchScore = accumulatedScore / (double)gamesInAccumulatedScore;
                    Console.WriteLine($"{total} iterations. Error: {brain.visSelf()}. Length: {accumulatedGameLenght/(double)gamesInAccumulatedScore} Average score: {batchScore}");
                    accumulatedScore = 0;
                    gamesInAccumulatedScore = 0;
                    accumulatedGameLenght = 0;

                    //if agent is good - save it
                    if (batchScore > bestBatchScore)
                    {
                        bestBatchScore = batchScore;
                        IFormatter formatter = new BinaryFormatter();
                        if (bestAgentSerialized != null)
                        {
                            bestAgentSerialized.Close();
                            bestAgentSerialized.Dispose();
                        }
                        bestAgentSerialized = new MemoryStream();
                        formatter.Serialize(bestAgentSerialized, agent);
                        
                    }
                }
            }
            Console.WriteLine($"Best score: {bestBatchScore}");
            Console.WriteLine("End");
            //File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "DNQ.trr", agent.AgentToJson());

            IFormatter readFormatter = new BinaryFormatter();
            bestAgentSerialized.Seek(0, SeekOrigin.Begin);
            var agentToReturn = (QAgent21)readFormatter.Deserialize(bestAgentSerialized);
            agentToReturn.Brain.learning = false;

            brain.learning = false;
            return agentToReturn;
        }
    }
}
