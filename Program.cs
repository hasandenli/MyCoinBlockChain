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

            //New blocks are added to blockchain.
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Hasan,to:Emre,amount:10}"));
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Emre,to:Eray,amount:5}"));
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Emre,to:Eray,amount:5}"));
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Eray,to:Hasan,amount:2}"));
            myCoin.AddBlock(new Block(DateTime.Now, null, "{from:Hasan,to:Eray,amount:1}"));

            //Last State of BlockChain
            Console.WriteLine(JsonConvert.SerializeObject(myCoin, Formatting.Indented));

            //Check blockChain is valid
            Console.WriteLine($"State of BlockChain Validation: { myCoin.IsChainValid() }");

            //Change manuel any block in blockChain
            Console.WriteLine($"Hack number of 1 block :)");
            myCoin.BlockChain[1].TransactionData = "{from:Emre,to:Hasan,amount:1000}";

            //Check blockChain is valid again
            Console.WriteLine($"State of BlockChain Validation: { myCoin.IsChainValid() }");

            Console.WriteLine($"Try to update hash of block :)");
            myCoin.BlockChain[1].CurrentBlockHash = myCoin.BlockChain[1].CalculateHash();

            Console.WriteLine($"State of BlockChain Validation: { myCoin.IsChainValid() }");

            Console.WriteLine($"Update the all blockchain :)");
            myCoin.BlockChain[2].PreviousBlockHash = myCoin.BlockChain[1].CurrentBlockHash;
            myCoin.BlockChain[2].CurrentBlockHash = myCoin.BlockChain[2].CalculateHash();
            myCoin.BlockChain[3].PreviousBlockHash = myCoin.BlockChain[2].CurrentBlockHash;
            myCoin.BlockChain[3].CurrentBlockHash = myCoin.BlockChain[3].CalculateHash();
            myCoin.BlockChain[4].PreviousBlockHash = myCoin.BlockChain[3].CurrentBlockHash;
            myCoin.BlockChain[4].CurrentBlockHash = myCoin.BlockChain[4].CalculateHash();
            myCoin.BlockChain[5].PreviousBlockHash = myCoin.BlockChain[4].CurrentBlockHash;
            myCoin.BlockChain[5].CurrentBlockHash = myCoin.BlockChain[5].CalculateHash();

            Console.WriteLine($"State of BlockChain Validation: { myCoin.IsChainValid() }");

            Console.ReadKey();
        }
    }
}
