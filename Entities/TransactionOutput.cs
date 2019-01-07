using Org.BouncyCastle.Crypto;

namespace Entities
{
    public class TransactionOutput
    {
        public string Id;
        public AsymmetricKeyParameter Receiver;
        public float Amount;
        public string ParentTransactionId;
        
        public TransactionOutput(AsymmetricKeyParameter receiver, float amount, string parentTransactionId) {
            this.Receiver = receiver;
            this.Amount = amount;
            this.ParentTransactionId = parentTransactionId;
            this.Id = SecurityUtil.ApplySha256(SecurityUtil.GetPublicKeyBase64(receiver) + amount.ToString() + parentTransactionId);
        }
        
        public bool isMine(AsymmetricKeyParameter publicKey) {
            return (publicKey == Receiver);
        }
    }
}