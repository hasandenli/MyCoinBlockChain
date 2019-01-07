using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;
using System.Text;

namespace Entities
{
    public class Transaction
    {
        public string TransactionID;
        public AsymmetricKeyParameter Sender; // senders address - public key.
        public AsymmetricKeyParameter Receiver; // Receiver address - public key.
        public float Amount;
	    public string Signature;
        public List<TransactionInput> InputList = new List<TransactionInput>();
	    public List<TransactionOutput> OutputList = new List<TransactionOutput>();

        private static int Sequence = 0;

        public Transaction(AsymmetricKeyParameter from, AsymmetricKeyParameter to, float amount,  List<TransactionInput> inputList) 
        {
            this.Sender = from;
            this.Receiver = to;
            this.Amount = amount;
            this.InputList = inputList;
	    }

        private string CalculateTransactionHash()
        {
            Sequence++;

            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(
                SecurityUtil.GetPublicKeyBase64(Sender) + 
                SecurityUtil.GetPublicKeyBase64(Receiver) +
                Amount.ToString() +
                Sequence);

            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        public void GenerateSignature(AsymmetricKeyParameter privateKey) {
            string data = SecurityUtil.GetPublicKeyBase64(Sender) + SecurityUtil.GetPublicKeyBase64(Receiver) + Amount.ToString()	;
            Signature = SecurityUtil.Sign(privateKey, data);		
        }
        
        public bool VerifiySignature() {
            string data = SecurityUtil.GetPublicKeyBase64(Sender) + SecurityUtil.GetPublicKeyBase64(Receiver) + Amount.ToString();
            return SecurityUtil.Verify(Sender, data, Signature);
        }

        public bool ProcessTransaction() {
		
            if(VerifiySignature() == false) {
                Console.WriteLine("!!! - Transaction Signature is not verified");
                return false;
            }
                    
            //collecting transaction inputs - !!!Make sure they are unspent
            foreach(TransactionInput i in InputList) {
                i.UnspentTxOutput = Blockchain.UtxOutputList[i.TransactionOutputId];
            }

            //check if transaction is valid for minimumtransaction
            if(GetInputsValue() < Blockchain.MinimumTransaction) {
                Console.WriteLine("!!! - Transaction Inputs to small: " + GetInputsValue());
                return false;
            }
            
            //generate transaction outputs
            float remainingAmount = GetInputsValue() - Amount;
            TransactionID = CalculateTransactionHash();
            OutputList.Add(new TransactionOutput( this.Receiver, Amount, TransactionID)); //send value to receiver
            OutputList.Add(new TransactionOutput( this.Sender, remainingAmount, TransactionID)); //send the remeaning amount back to sender		
                    
            //add outputs to Unspent list
            foreach(TransactionOutput o in OutputList) {
                Blockchain.UtxOutputList.Add(o.Id , o);
            }
            
            //remove transaction inputs from UtxOutputList as spent
            foreach(TransactionInput i in InputList) {
                //if Transaction can't be found skip it 
                if(i.UnspentTxOutput == null) 
                    continue; 

                Blockchain.UtxOutputList.Remove(i.UnspentTxOutput.Id);
            }
            
            return true;
        }
	
        //returns sum of UnspentTxOutput amounts
        public float GetInputsValue() {
            float total = 0;
            foreach(TransactionInput i in InputList) {
                //if Transaction can't be found skip it 
                if(i.UnspentTxOutput == null) 
                    continue; 

                total += i.UnspentTxOutput.Amount;
            }
            return total;
        }

        //returns sum of outputs
        public float GetOutputsValue() {
            float total = 0;
            foreach(TransactionOutput o in OutputList) {
                total += o.Amount;
            }
            return total;
        }
    }
}
