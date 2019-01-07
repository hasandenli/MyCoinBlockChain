using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Blockchain
    {
        public int Difficulty = 2;
        public static float MinimumTransaction = 0.1f;
        public static List<Block> BlockChain { set;  get; }
        public static Dictionary<string, TransactionOutput> UtxOutputList = new Dictionary<string, TransactionOutput>();

        public Blockchain()
        {
            //BlockChain Object is created.
            InitializeChain();

            // //First block is added.
            // AddGenesisBlock();
        }

        public void InitializeChain()
        {
            BlockChain = new List<Block>();
        }

        public void AddGenesisBlock(Transaction genesisTransaction)
        {
            BlockChain.Add(new Block(DateTime.Now, null));
            BlockChain[0].AddTransaction(genesisTransaction);
            BlockChain[0].MineBlock(this.Difficulty);
        }

        public void AddBlock(Block block)
        {
            Block lastBlock = GetLastBlock();
            block.BlockIndex = lastBlock.BlockIndex + 1;
            block.MineBlock(this.Difficulty);
            BlockChain.Add(block);
        }
        
        public Block GetLastBlock()
        {
            return BlockChain[BlockChain.Count - 1];
        }

        public bool IsChainValid()
        {
            Dictionary<string,TransactionOutput> tempUnspentTransactionOutputList = new Dictionary<string, TransactionOutput>(); //a temporary working list of unspent transactions at a given block state.
		    tempUnspentTransactionOutputList.Add(BlockChain[0].TransactionData[0].OutputList[0].Id, BlockChain[0].TransactionData[0].OutputList[0]);

            for (int i = 1; i < BlockChain.Count; i++)
            {
                Block currentBlock = BlockChain[i];
                Block previousBlock = BlockChain[i - 1];

                if (currentBlock.CurrentBlockHash != currentBlock.CalculateHash())
                {
                    Console.WriteLine("#Block Hash is not equal : Block(" + i + ")");
                    return false;
                }

                if (currentBlock.PreviousBlockHash != previousBlock.CurrentBlockHash)
                {
                    Console.WriteLine("#Block Previous Hash is not equal : Block(" + i + ")");
                    return false;
                }

                TransactionOutput tempOutput;
                for(int t=0; t <currentBlock.TransactionData.Count; t++) {
                    Transaction currentTransaction = currentBlock.TransactionData[t];
                    
                    if(!currentTransaction.VerifiySignature()) {
                        Console.WriteLine("!!! - Signature on Transaction(" + t + ") is Invalid");
                        return false; 
                    }

                    if(currentTransaction.GetInputsValue() != currentTransaction.GetOutputsValue()) {
                        Console.WriteLine("!!! - Inputs are note equal to outputs on Transaction(" + t + ")");
                        return false; 
                    }
                    
                    foreach(TransactionInput input in currentTransaction.InputList) {	
                        tempOutput = tempUnspentTransactionOutputList[input.TransactionOutputId];
                        
                        if(tempOutput == null) {
                            Console.WriteLine("!!! - Referenced input on Transaction(" + t + ") is Missing");
                            return false;
                        }
                        
                        if(input.UnspentTxOutput.Amount != tempOutput.Amount) {
                            Console.WriteLine("!!! - Referenced input Transaction(" + t + ") value is Invalid");
                            return false;
                        }
                        
                        tempUnspentTransactionOutputList.Remove(input.TransactionOutputId);
                    }
                    
                    foreach(TransactionOutput output in currentTransaction.OutputList) {
                        tempUnspentTransactionOutputList.Add(output.Id, output);
                    }
                    
                    if(currentTransaction.OutputList[0].Receiver != currentTransaction.Receiver) {
                        Console.WriteLine("!!! - Transaction(" + t + ") output receiver is not who it should be");
                        return false;
                    }

                    if(currentTransaction.OutputList[1].Receiver != currentTransaction.Sender) {
                        Console.WriteLine("!!! - Transaction(" + t + ") output 'change' is not sender.");
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
