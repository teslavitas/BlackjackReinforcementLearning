using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine21;
using Strategies21;
using ConvnetShartStrategy;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace _21
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new GameEngine();

            var strategy = StrategyBuilder.BuildStrategy(engine); //new StopAtNStratery(14);

            if (strategy.GetType().IsSerializable)
            {
                IFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream("strategySerialized.bin", FileMode.Create))
                {
                    formatter.Serialize(fs, strategy);
                    fs.Close();
                }
            }


            //IFormatter formatter = new BinaryFormatter();
            //ConvnetStrategy strategy;
            //using (FileStream fs = new FileStream("strategySerialized_15.34.bin", FileMode.Open))
            //{
            //    strategy = formatter.Deserialize(fs) as ConvnetStrategy;
            //    fs.Close();
            //}

            int gamesCount = 500000;
            List<int> gameResults = new List<int>();

            for (int i = 0; i < gamesCount; ++i)
            {
                var state = new GameState();

                while (!state.IsFinished)
                {
                    var turn = strategy.MakeTurn(state);
                    state = engine.ApplyTurn(turn, state);
                }

                gameResults.Add(state.Score);
                if (i % 1000 == 0)
                {
                    Console.WriteLine(i);
                }
            }

            Console.WriteLine(gameResults.Average());
        }

    }
}
