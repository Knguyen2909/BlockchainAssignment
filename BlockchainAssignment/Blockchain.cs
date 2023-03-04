using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockchainAssignment
{
    class Blockchain
    {
        public int maxBlock { get => Blocks.Count; }                                       // Maximum number of transactions per block
        public List<Block> Blocks = new List<Block>();                                      // List of block objects forming the blockchain
        public List<Transaction> TransactionPool = new List<Transaction>();                // List of pending transactions to be mined

        public Blockchain()
        {
            Blocks = new List<Block>()
            {
                new Block() // Create and append the Genesis Block
            };
        }
        public string BlockString(int index)
        {
            return (Blocks.ElementAt(index).ReturnString());
        }


        public void addTransactionPool(Transaction Trans)
        {
            TransactionPool.Add(Trans);
        }
        public void addToBlock(Block blck)
        {
            Blocks.Add(blck);
        }

        // Prints the block at the specified index to the UI
        public string GetBlockAsString(int index)
        {
            // Check if referenced block exists
            if (index >= 0 && index < Blocks.Count)
                return Blocks[index].ToString(); // Return block as a string
            else
                return "No such block exists";
        }
        public void purgeTPool(List<Transaction> chosenT)
        {
            TransactionPool = TransactionPool.Except(chosenT).ToList();
        }
        public Block GetLastBlock()
        {
            return Blocks[Blocks.Count - 1];
        }

        /* Function to return the transaction pool*/ 
        public List<Transaction> retTPool() {
            return TransactionPool;
        }

        /* Function to return the block chain */ 
        public override string ToString()
        {
            return string.Join("\n", Blocks);
        }

        // Check validity of a blocks hash by recomputing the hash and comparing with the mined value
        public static bool ValidateHash(Block b){

            string rehash = b.CreateHash();
            Console.WriteLine("Rehash: " + rehash + " --> Hash: " + b.hash);
            return rehash.Equals(b.hash);
        }

        public List<Transaction> getPendingTransactions()
        {
            int n = Math.Min(maxBlock, TransactionPool.Count);
            List<Transaction> pendingTransactions = TransactionPool.GetRange(0, n);

            TransactionPool.RemoveRange(0, n);

            return pendingTransactions;
        }


        // Check the balance associated with a wallet based on the public key
        public double GetBalance(string address)
        {
            // Accumulator value
            double balance = 10000;

            // Loop through all approved transactions in order to assess account balance
            foreach (Block b in Blocks)
            {
                foreach (Transaction t in b.transactionList)
                {
                    if (t.recipientAddress.Equals(address))
                    {
                        balance += t.amount; // Credit funds recieved
                    }
                    if (t.senderAddress.Equals(address))
                    {
                        balance -= (t.amount + t.fees); // Debit payments placed
                    }
                }
            }
            return balance;
        }

        // Check validity of the merkle root by recalculating the root and comparing with the mined value
        public static bool ValidateMerkleRoot(Block b){
            string reMerkle = Block.MerkleRoot(b.transactionList);
            return reMerkle.Equals(b.merkleRoot);
        }
    }
}
