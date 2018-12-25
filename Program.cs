using Newtonsoft.Json;
using System;
using Entities;

namespace MyCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            //A instance is taken from BlockChain.
            Blockchain myCoin = new Blockchain();

            DateTime startTime = DateTime.Now;

            //New blocks are added to blockchain.
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Hasan,to:Emre,amount:10}"));
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Emre,to:Eray,amount:5}"));
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Emre,to:Eray,amount:5}"));

            DateTime endTime = DateTime.Now;

            TimeSpan duration = endTime - startTime;
            Console.WriteLine($"Duration: {duration}");

            //Last State of BlockChain
            Console.WriteLine(JsonConvert.SerializeObject(myCoin, Formatting.Indented));

            //Check blockChain is valid
            Console.WriteLine($"State of BlockChain Validation: { myCoin.IsChainValid() }");

            Console.ReadKey();
        }
    }
}
