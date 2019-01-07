namespace Entities
{
    public class TransactionInput
    {
        public string TransactionOutputId;
        public TransactionOutput UnspentTxOutput;
        
        public TransactionInput(string transactionOutputId) {
            this.TransactionOutputId = transactionOutputId;
        }
    }
}