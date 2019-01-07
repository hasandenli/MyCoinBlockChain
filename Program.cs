using Newtonsoft.Json;
using System;
using Entities;
using System.Collections.Generic;

namespace MyCoin
{
    class Program
    {

        public static Transaction genesisTransaction;

        static void Main(string[] args)
        {
            
            // //1. Senaryo
            // Wallet wallet1 = new Wallet();
		    // Wallet wallet2 = new Wallet();

            // Console.WriteLine($"\nWallet1's Private and public keys:");
            // Console.WriteLine($"\nPrivateKey: {SecurityUtil.GetPrivateKeyBase64(wallet1.PrivateKey)}");
            // Console.WriteLine($"\nPublicKey: {SecurityUtil.GetPublicKeyBase64(wallet1.PublicKey)}");

            // Console.WriteLine($"\nWallet2's Private and public keys:");
            // Console.WriteLine($"\nPrivateKey: {SecurityUtil.GetPrivateKeyBase64(wallet2.PrivateKey)}");
            // Console.WriteLine($"\nPublicKey: {SecurityUtil.GetPublicKeyBase64(wallet2.PublicKey)}");

            // Transaction transaction = new Transaction(wallet1.PublicKey, wallet2.PublicKey, 5, null);
		    // transaction.GenerateSignature(wallet1.PrivateKey);

            // Console.WriteLine($"\nIs signature verified");
            // Console.WriteLine(transaction.VerifiySignature());

            // Console.ReadLine();

            ////2. Senaryo
            //A instance is taken from BlockChain.
            Blockchain myCoin = new Blockchain();

            List<Wallet> walletList = new List<Wallet>();

            //Create wallets:
            Wallet baseWallet = new Wallet();

            Wallet walletA = new Wallet(); 
            walletList.Add(walletA);
            Wallet walletB = new Wallet();
            walletList.Add(walletB);	
            Wallet walletC = new Wallet();
            walletList.Add(walletC);	

            //Create genesis transaction which sends 1000 MyCoin to walletA
            genesisTransaction = new Transaction(baseWallet.PublicKey, walletA.PublicKey, 1000f, null);
            //Signature the genesis transaction	as manually	
            genesisTransaction.GenerateSignature(baseWallet.PrivateKey);
            //Set the transactionid	as manually
            genesisTransaction.TransactionID = "0"; 
            //Add the Transactions Output as manually
            genesisTransaction.OutputList.Add(new TransactionOutput(genesisTransaction.Receiver, genesisTransaction.Amount, genesisTransaction.TransactionID)); 
            //Genesis Transaction Output must be in blockchain output list. It is important
            Blockchain.UtxOutputList.Add(genesisTransaction.OutputList[0].Id, genesisTransaction.OutputList[0]); 
		
            Console.WriteLine("Creating and Mining Genesis block...");
            myCoin.AddGenesisBlock(genesisTransaction);

            writeWalletsBalance(walletList);

            Block block1 = new Block(DateTime.Now, Blockchain.BlockChain[0].CurrentBlockHash);
		    Console.WriteLine("\n * - Wallet1 is generating a transaction to send coins 100 to Wallet2...");
            block1.AddTransaction(walletA.SendCoin(walletB.PublicKey, 100f));
            myCoin.AddBlock(block1);

            writeWalletsBalance(walletList);

            Block block2 = new Block(DateTime.Now, block1.CurrentBlockHash);
		    Console.WriteLine("\n * - Wallet2 is generating a transaction to send coins 50 to Wallet1...");
            block2.AddTransaction(walletB.SendCoin(walletA.PublicKey, 50f));
            myCoin.AddBlock(block2);

            writeWalletsBalance(walletList);

            Block block3 = new Block(DateTime.Now, block2.CurrentBlockHash);
		    Console.WriteLine("\n * - Wallet2 is is generating a transaction to send coins 30 to Wallet1...");
            block3.AddTransaction(walletB.SendCoin(walletA.PublicKey, 30f));
            myCoin.AddBlock(block3);

            writeWalletsBalance(walletList);

            Block block4 = new Block(DateTime.Now, block3.CurrentBlockHash);
		    Console.WriteLine("\n * - Wallet1 is is generating a transaction to send coins 1000 to Wallet2...");
            block4.AddTransaction(walletA.SendCoin(walletB.PublicKey, 1000f));
            myCoin.AddBlock(block4);

            writeWalletsBalance(walletList);

            Block block5 = new Block(DateTime.Now, block4.CurrentBlockHash);
		    Console.WriteLine("\n * - Wallet1 is generating a transaction to send coins 300 to Wallet2...");
            block5.AddTransaction(walletA.SendCoin(walletB.PublicKey, 300f));
            myCoin.AddBlock(block5);

            writeWalletsBalance(walletList);

            //Testing multiple transaction inside one block
            Block block6 = new Block(DateTime.Now, block5.CurrentBlockHash);
		    Console.WriteLine("\n * - Wallet1 is is generating a transaction to send coins 600 to Wallet3...");
            block6.AddTransaction(walletA.SendCoin(walletC.PublicKey, 600f));
		    Console.WriteLine("\n * - Wallet1 is is generating a transaction to send coins 80 to Wallet2...");
            block6.AddTransaction(walletA.SendCoin(walletB.PublicKey, 80f));
            myCoin.AddBlock(block6);

            writeWalletsBalance(walletList);

            //Check blockChain is valid
            Console.WriteLine($"\nState of BlockChain Validation: { myCoin.IsChainValid() }");


            //Last State of BlockChain
            Console.WriteLine(JsonConvert.SerializeObject(Blockchain.BlockChain, Formatting.Indented));

            Console.ReadKey();
        }

        public static void writeWalletsBalance(List<Wallet> walletList)
        {
            for(var i = 0; i < walletList.Count; i++){
                Console.WriteLine("\nWallet"+(i+1)+"'s balance is: " + walletList[i].GetBalance());
            }
        }
    }
}
