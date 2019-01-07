using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Entities
{
    public class Wallet
    {
        public Guid WalletID;
        public AsymmetricKeyParameter PublicKey;
        public AsymmetricKeyParameter PrivateKey;
        public Dictionary<string, TransactionOutput> UnspentTransactionOutputList = new Dictionary<string, TransactionOutput>();

        public Wallet()
        {
            this.WalletID = Guid.NewGuid();

            GenerateKeys(2048);
        }

        private void GenerateKeys(int keySizeInBits)
        {
            try{
                var r = new RsaKeyPairGenerator();
                r.Init(new KeyGenerationParameters(new SecureRandom(), keySizeInBits));
                var keys = r.GenerateKeyPair();

                PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keys.Private);
                byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();

                SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keys.Public);
                byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();

                this.PrivateKey =  keys.Private;
                this.PublicKey =  keys.Public;
            }
            catch(Exception ex){
                throw new Exception(ex.Message);
            }
        }

        public float GetBalance() {
            float total = 0;	

            foreach (var item in Blockchain.UtxOutputList) 
            {
                TransactionOutput UTXO = item.Value;
                if(UTXO.isMine(PublicKey)) {
                    if(!UnspentTransactionOutputList.ContainsKey(UTXO.Id))
                    {
                        UnspentTransactionOutputList.Add(UTXO.Id, UTXO);
                    } 
                    
                    total += UTXO.Amount ; 
                }
            }  

            return total;
        }

        public Transaction SendCoin(AsymmetricKeyParameter _receiver, float amount ) {
            if(GetBalance() < amount) {
                Console.WriteLine("!!! - Not Enough coin to send transaction. Transaction Discarded.");
                return null;
            }
            
            List<TransactionInput> inputList = new List<TransactionInput>();
        
            float total = 0;
            foreach (var item in UnspentTransactionOutputList){
                TransactionOutput UTXO = item.Value;
                total += UTXO.Amount;
                inputList.Add(new TransactionInput(UTXO.Id));
                if(total > amount) break;
            }
            
            Transaction newTransaction = new Transaction(PublicKey, _receiver , amount, inputList);
            newTransaction.GenerateSignature(PrivateKey);
            
            foreach(TransactionInput input in inputList){
                UnspentTransactionOutputList.Remove(input.TransactionOutputId);
            }
            return newTransaction;
        }

    }
}
