using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Entities
{
    public class Block
    {
        public int BlockIndex { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PreviousBlockHash { get; set; }
        public string CurrentBlockHash { get; set; }
        public string TransactionData { get; set; }

        public Block(DateTime timeStamp, string previousHash, string transactionData)
        {
            BlockIndex = 0;
            TimeStamp = timeStamp;
            PreviousBlockHash = previousHash;
            TransactionData = transactionData;
            CurrentBlockHash = CalculateHash();
        }

        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousBlockHash ?? ""}-{TransactionData}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }
    }
}
