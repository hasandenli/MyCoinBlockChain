using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Blockchain
    {
        public List<Block> BlockChain { set;  get; }

        public Blockchain()
        {
            //BlockChain Object is created.
            InitializeChain();

            //First block is added.
            AddGenesisBlock();
        }


        public void InitializeChain()
        {
            BlockChain = new List<Block>();
        }

        public void AddGenesisBlock()
        {
            BlockChain.Add(new Block(DateTime.Now, null, "{}"));
        }
        
        public Block GetLastBlock()
        {
            return BlockChain[BlockChain.Count - 1];
        }

        public void AddBlock(Block block)
        {
            Block lastBlock = GetLastBlock();
            block.BlockIndex = lastBlock.BlockIndex + 1;
            block.PreviousBlockHash = lastBlock.CurrentBlockHash;
            block.CurrentBlockHash = block.CalculateHash();
            BlockChain.Add(block);
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < BlockChain.Count; i++)
            {
                Block currentBlock = BlockChain[i];
                Block previousBlock = BlockChain[i - 1];

                if (currentBlock.CurrentBlockHash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousBlockHash != previousBlock.CurrentBlockHash)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
