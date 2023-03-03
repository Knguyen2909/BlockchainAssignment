using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace BlockchainAssignment
{
    internal class Block
    {
        public bool threading = true;

        public string hash;
        public string prevHash;
        public string finalHash;
        public string finalHash0;
        public string finalHash1;

        public List<Transaction> transactionList;               
        public DateTime timeStamp;        
        
        public int index;                                 
        public int difficulty;                         
                                
                           
        public string merkleRoot;                       
        public string minerAddress;             
        public double reward = 1.0;

        public long nonce = 0;
        public int nonce0 = 0;
        public int nonce1 = 1;

        private bool th1Fin = false, th2Fin = false;

        // Constructor which is passed the previous block
        public Block(Block lastBlock)
        {
            this.timeStamp = DateTime.Now;
            nonce = 0;
            this.index = lastBlock.index + 1;
            this.prevHash = lastBlock.hash;
            if (threading == true)
            {
                ThreadedMine();
                hash = finalHash;
            }
            else hash = CreateHash();
            //    Create hash from index, prevhash and time
            transactionList = new List<Transaction>();
        }

        public Block(Block lastBlock, List<Transaction> TPool)
        {
            transactionList = new List<Transaction>();
            nonce = 0;
            timeStamp = DateTime.Now;
            index = lastBlock.index + 1;
            prevHash = lastBlock.hash;
            addTransactionPool(TPool, 2, "!");
            // Create hash from index, prevhash and time
        }

        // Constructor which is passed the index & hash of previous block
        public Block(int lastIndex, string lastHash)
        {
            nonce = 0;
            timeStamp = DateTime.Now;                     
            index = lastIndex + 1;                         
            prevHash = lastHash;                               
            hash = CreateHash();                              
        }

        // Constructor which is not passed anything
        public Block()
        {
            // This generates the Genesis Block 
            transactionList = new List<Transaction>();
            timeStamp = DateTime.Now;                                    
            index = 0;                                                 
            prevHash = string.Empty;                                   
            hash = CreateMine();                             
            difficulty = 4;
        }

        public Block(Block lastBlock, List<Transaction> TPool, string MinerAddress, int setting, string address)
        {
            transactionList = new List<Transaction>();
            nonce = 0;
            timeStamp = DateTime.Now;
            difficulty = lastBlock.difficulty;
            adjustDiff(lastBlock.timeStamp); 
            index = lastBlock.index + 1;
            prevHash = lastBlock.hash;
            minerAddress = MinerAddress;
            TPool.Add(createRewardTransaction(TPool)); // Create and append the reward transaction
            addTransactionPool(TPool, setting, address);
            //    Create hash from index, prevhash and time
            merkleRoot = MerkleRoot(transactionList); // Calculate the merkle root of the blocks transactions

            if (threading == true)
            {
                ThreadedMine();
                hash = finalHash;
            }
            else hash = CreateMine();//Create256Hash();
            Console.WriteLine("Here");
        }

        public override string ToString()
        {
            return ("\n\n\t\t[BLOCK START]"
                + "\nIndex: " + index
                + "\tTimestamp: " + timeStamp
                + "\nPrevious Hash: " + prevHash
                + "\n\t\t-- PoW --"
                + "\nDifficulty Level: " + difficulty
                + "\nNonce: " + nonce
                + "\nHash: " + hash + " " + finalHash
                + "\n\t\t-- Rewards --"
                + "\nReward: " + reward
                + "\nMiners Address: " + minerAddress
                + "\n\t\t-- " + transactionList.Count + " Transactions --"
                + "\nMerkle Root: " + merkleRoot
                + "\n" + string.Join("\n", transactionList)
                + "\n\t\t[BLOCK END]");
        }

        public string ReturnString()
        {
            return ("\n\n\t\t[BLOCK START]"
                + "\nIndex: " + index
                + "\tTimestamp: " + timeStamp
                + "\nPrevious Hash: " + prevHash
                + "\n\t\t-- PoW --"
                + "\nDifficulty Level: " + difficulty
                + "\nNonce: " + nonce
                + "\nHash: " + hash
                + "\n\t\t-- Rewards --"
                + "\nReward: " + reward
                + "\nMiners Address: " + minerAddress
                + "\n\t\t-- " + transactionList.Count + " Transactions --"
                + "\nMerkle Root: " + merkleRoot
                + "\n" + string.Join("\n", transactionList)
                + "\n\t\t[BLOCK END]");
        }

        public string readblock()
        {
            string s = "";
            s += ToString();
            foreach (Transaction T in transactionList)
            {
                s += "\n" + T.ToString();
            }
            return s;
        }

        public void addTransactionList(Transaction T)
        {
            transactionList.Add(T);
        }

        public void addTransactionPool(List<Transaction> TP, int mode, string address)
        {
            int LIMIT = 5;
            int idx = 0;
            while (transactionList.Count < LIMIT && TP.Count > 0)
            {
                if (mode == 0)
                {// greedy
                    //int idx = 0;
                    for (int i = 0; i < TP.Count; i++)
                    {
                        if (TP.ElementAt(i).fees > TP.ElementAt(idx).fees)
                        {
                            idx = i;
                        }
                    }
                    transactionList.Add(TP.ElementAt(idx));
                }
                else if (mode == 1)
                {// altruistic
                    for (int i = 0; (i < TP.Count) && (i < LIMIT); i++)
                    {
                        transactionList.Add(TP.ElementAt(i));
                    }
                }
                else if (mode == 2)
                {  //random      
                    Random random = new Random();
                    idx = random.Next(0, TP.Count);
                    transactionList.Add(TP.ElementAt(idx));
                }
                else if (mode == 3)
                {

                    //transactionList.Add(TP.ElementAt(0));
                    for (int i = 0; i < TP.Count && (transactionList.Count < LIMIT); i++)
                    {
                        if (TP.ElementAt(i).senderAddress == address)
                        {
                            transactionList.Add(TP.ElementAt(i));
                        }
                        else
                        {
                            if (TP.ElementAt(i).recipientAddress == address)
                            {
                                transactionList.Add(TP.ElementAt(i));
                            }
                            else
                            {
                                // ONLY TAKE FROM THIS ADDRESS: 
                                // If take address as priority and then add up to get to 5 --> add mode = 2 here
                            }
                        }
                        //TP = TP.Except(this.transactionList).ToList();

                    }
                    Console.WriteLine("Endless loop");
                }
                else
                { // No Valid input, choose default --> Altruistic
                    mode = 1;
                }
                TP = TP.Except(transactionList).ToList();

            }

        }

        public string CreateHash()
        {
            string hash = string.Empty;

            SHA256 hasher = SHA256.Create();
            string input = index.ToString() + timeStamp.ToString() + prevHash + nonce + merkleRoot + reward.ToString();
            
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));
            //Convert hash from byte array to string
            foreach (byte x in hashByte)
            {
                hash += String.Format("{0:x2}", x);
            }
            return hash;
        }

        private string CreateMine()
        {
            string hash = "";         
            string diffString = new string('0', difficulty);
            while (hash.StartsWith(diffString) == false)
            {
                hash = CreateHash();
                nonce++;
            }
            nonce--;
            if (hash is null)
            {
                throw new IndexOutOfRangeException("No hash generated");
            }
            return hash;
        }

        public static string MerkleRoot(List<Transaction> transactionList)
        {

            List<string> hashlist = transactionList.Select(t => t.hash).ToList(); // Get a list of transaction hashlist for "combining"

            if (hashlist.Count == 0) 
            {
                return string.Empty;
            }
            else if (hashlist.Count == 1) 
            {
                return HashCode.HashTools.combineHash(hashlist[0], hashlist[0]);
            }
            while (hashlist.Count != 1) 
            {
                List<string> merkleLeaves = new List<string>(); 

                for (int i = 0; i < hashlist.Count; i += 2) // Step over neighbouring pair combining each
                {
                    if (i == hashlist.Count - 1)
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashlist[i], hashlist[i])); // Handle an odd number of leaves
                    }
                    else
                    {
                        merkleLeaves.Add(HashCode.HashTools.combineHash(hashlist[i], hashlist[i + 1])); // Hash neighbours leaves
                    }
                }
                hashlist = merkleLeaves; // Update the working "layer"
            }
            return hashlist[0]; // Return the root node
        }

        // Create reward for the mining of block
        public Transaction createRewardTransaction(List<Transaction> transactions)
        {
            double fees = transactions.Aggregate(0.0, (acc, t) => acc + t.fees); // Sum all transaction fees
            return new Transaction("Mine Rewards", "", minerAddress, reward + fees, 0); // Issue reward as a transaction in the new block
        }

        /*FOR MULTI THREADING */
        public void ThreadedMine()
        {
            Thread th1 = new Thread(Mine0);
            Thread th2 = new Thread(Mine1);

            th1.Start();
            th2.Start();

            while (th1.IsAlive == true || th2.IsAlive == true) { Thread.Sleep(1); }


            if (finalHash1 is null)
            {
                nonce = nonce0;
                finalHash = finalHash0;
            }
            else
            {
                nonce = nonce1;
                finalHash = finalHash1;
            }
            if (finalHash is null)
            {
                Console.WriteLine(ReturnString());
                throw new Exception("NULL finalhash" +
                    " Nonce0: " + nonce0 +
                    " Nonce1: " + nonce1 +
                    " Nonce: " + nonce +
                    " finalhash0 " + finalHash0 +
                    " finalhash1: " + finalHash1 +
                    " NewHash: " + CreateHash());

            }

        }

        public void Mine0()
        {
            th1Fin = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool check = false;
            string newHash;
            string diffString = new string('0', difficulty);

            while (check == false)
            {
                newHash = Create256Hash(nonce0);
                if (newHash.StartsWith(diffString) == true)
                {
                    check = true;
                    finalHash0 = newHash;
                    Console.WriteLine("Block index: " + index);
                    Console.WriteLine("Thread 1 closed: Thread 1 found: " + finalHash0);
                    th1Fin = true;

                    Console.WriteLine(nonce0);
                    sw.Stop();
                    Console.WriteLine("Th1 mine:");
                    Console.WriteLine(sw.Elapsed);

                    return;
                }
                else if (th2Fin == true)
                {
                    Console.WriteLine("Thread 1 closed: Thread 2 found: " + finalHash1);
                    Thread.Sleep(1);
                    return;
                }
                else
                {
                    check = false;
                    nonce0 += 2;
                }
            }
            return;
        }

        public void Mine1()
        {
            th2Fin = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool check = false;
            string newHash;
            string diffString = new string('0', difficulty);
            while (check == false)
            {
                newHash = Create256Hash(nonce1);
                if (newHash.StartsWith(diffString) == true)
                {
                    check = true;
                    finalHash1 = newHash;
                    Console.WriteLine("Block index: " + index);
                    Console.WriteLine("Thread 2 closed: Thread 2 found: " + finalHash1);
                    th2Fin = true;

                    Console.WriteLine(nonce1);
                    sw.Stop();
                    Console.WriteLine("Th2 mine:");
                    Console.WriteLine(sw.Elapsed);

                    return;
                }
                else if (th1Fin == true)
                {
                    Console.WriteLine("Thread 2 closed: Thread 1 found: " + finalHash0);
                    Thread.Sleep(1);
                    return;
                }
                else
                {
                    check = false;
                    nonce1 += 2;
                }
            }
            return;
        }

        public string Create256Hash(int inNonce)
        {
            SHA256 hasher;
            hasher = SHA256.Create();
            string input = index.ToString() + timeStamp.ToString() + prevHash + inNonce + merkleRoot + reward.ToString();
            byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));
            string hash = string.Empty;

            foreach (byte x in hashByte)
            {
                hash += string.Format("{0:x2}", x);
            }
            return hash;
        }

        //Function to adjust the difficulty
        public void adjustDiff(DateTime lastTime)
        {
            //Gets the elapsed time between now and the last block mined
            DateTime startTime = DateTime.UtcNow;
            TimeSpan timeDiff = startTime - lastTime;

            //If the diff is less than 5 seconds, the diff is increased to attempt to increase the time
            if (timeDiff < TimeSpan.FromSeconds(5))
            {
                difficulty++;
                Console.WriteLine("Time since last mine");
                Console.WriteLine(timeDiff);
                Console.WriteLine("New Difficulty:");
                Console.WriteLine(difficulty);
            }
            //If the diff is more than 5 seconds, the diff is increased to attempt to decrease the time
            else if (timeDiff > TimeSpan.FromSeconds(5))
            {
                difficulty--;
                Console.WriteLine("Time since last mine");
                Console.WriteLine(timeDiff);
                Console.WriteLine("New Difficulty:");
                Console.WriteLine(difficulty);
            }

            //Difficulty can never be higher than 5 or lower than 0
            if (difficulty <= 0)
            {
                difficulty = 0;
                Console.WriteLine("Difficulty too low, new difficulty:");
                Console.WriteLine(difficulty);
            }
            else if (difficulty >= 6)
            {
                difficulty = 4;
                Console.WriteLine("Difficulty too high, new difficulty:");
                Console.WriteLine(difficulty);
            }
        }
    }
}