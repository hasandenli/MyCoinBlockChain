using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Entities
{
    public class Block
    {
        public int BlockIndex { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PreviousBlockHash { get; set; }
        public string CurrentBlockHash { get; set; }
        public string TransactionAsString { get; set; }
        [JsonIgnore]
        public List<Transaction> TransactionData = new List<Transaction>();
        public int Nonce = 0;  

        public Block(DateTime timeStamp, string previousHash)
        {
            BlockIndex = 0;
            TimeStamp = timeStamp;
            PreviousBlockHash = previousHash;
            //CurrentBlockHash = CalculateHash();
        }

        public Block(DateTime timeStamp, string previousHash, List<Transaction> transactionData)
        {
            BlockIndex = 0;
            TimeStamp = timeStamp;
            PreviousBlockHash = previousHash;
            TransactionData = transactionData;
            //CurrentBlockHash = CalculateHash();
        }

        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousBlockHash ?? ""}-{TransactionAsString}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        public void MineBlock(int difficulty) 
        {
            this.TransactionAsString = GenerateTransactionString(this.TransactionData);

            CurrentBlockHash = CalculateHash();

		    String targetHash = new String(new char[difficulty]).Replace('\0', '0');
		    while(!CurrentBlockHash.Substring( 0, difficulty).Equals(targetHash)) {
                Nonce ++;
                CurrentBlockHash = CalculateHash();
		    }

            Console.WriteLine($"\nBlock Mined : {CurrentBlockHash}" );
	    }

        //Add transactions
        public bool AddTransaction(Transaction transaction) {
            //Try to process transaction if valid. If block is genesis block then ignore.
            if(transaction == null) 
                return false;		

            if(PreviousBlockHash != null) {
                if((transaction.ProcessTransaction() != true)) {
                    Console.WriteLine("!!! - Process failed. Discarded.");
                    return false;
                }
            }

            TransactionData.Add(transaction);
            Console.WriteLine("#Transaction Successfully added to Block");

            return true;
        }

        public static String GenerateTransactionString(List<Transaction> transactions) {
            int count = transactions.Count;

            List<string> previousTreeLayer = new List<string>();
            foreach(Transaction transaction in transactions) 
                previousTreeLayer.Add(transaction.TransactionID);

            List<string> treeLayer = previousTreeLayer;
            while(count > 1) {
                treeLayer = new List<string>();
                for(int i=1; i < previousTreeLayer.Count; i++) {
                    treeLayer.Add(SecurityUtil.ApplySha256(previousTreeLayer[i-1] + previousTreeLayer[i]));
                }
                count = treeLayer.Count;
                previousTreeLayer = treeLayer;
            }
            
            string transactionAsString = (treeLayer.Count == 1) ? treeLayer[0] : "";

            return transactionAsString;
        }
    }
}
