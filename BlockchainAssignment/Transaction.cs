using System;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAssignment
{
    class Transaction
    {
        public String hash;
        String signature;
        public String senderAddress;
        public String recipientAddress;
        DateTime timestamp;
        public double amount;
        public double fees;
        private string v1;
        private string v2;
        private string minerAddress;
        private double v3;
        private int v4;
        private string text1;
        private string text2;
        private float v;
        private float v5;

        public static sbyte Count { get; internal set; }

        public Transaction(string text)
        {

        }
        public Transaction(String from, String to, double amount, double fee, String privateKey)
        {

            this.senderAddress = from;
            this.recipientAddress = to;
            this.amount = amount;
            this.fees = fees;

            this.timestamp = DateTime.Now;

            this.hash = CreateHash();
            this.signature = Wallet.Wallet.CreateSignature(from, privateKey, this.hash);

        }

        public Transaction(string v1, string v2, string minerAddress, double v3, int v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.minerAddress = minerAddress;
            this.v3 = v3;
            this.v4 = v4;
        }

        public Transaction(string text, string text1, string text2, float v, float v5) : this(text)
        {
            this.text1 = text1;
            this.text2 = text2;
            this.v = v;
            this.v5 = v5;
        }

        public String CreateHash()
        {
            String hash = String.Empty;

            SHA256 hasher = SHA256.Create();

            //hash all properties
            String input = senderAddress + recipientAddress + timestamp.ToString() + amount.ToString() + fees.ToString();
            
            Byte[] hashByte = hasher.ComputeHash(Encoding.UTF8.GetBytes((input)));

            //Convert Hash from byte array to string
            foreach (byte x in hashByte)
            {
                hash += string.Format("{0:x2}", x);
            }
            return hash;

        }


        public override string ToString()
        {
            return ("[TRANSACTION START]"
                + "\n\t  Transaction Hash: " + hash
                + "\n\t  Digital Signature: " + signature
                + "\n\t  Timestamp: " + timestamp
                + "\n\t  Transferred: " + amount + " AssignmentCoin"
                + "\t  Fee: " + fees
                + "\n\t  Sender: " + senderAddress
                + "\n\t  Reciever: " + recipientAddress
                + "\n  [TRANSACTION END]");

        }
        public string ReturnString()
        {
            return ("[TRANSACTION START]"
                + "\n\t  Transaction Hash: " + hash
                + "\n\t  Digital Signature: " + signature
                + "\n\t  Timestamp: " + timestamp
                + "\n\t  Transferred: " + amount + " AssignmentCoin"
                + "\t  Fee: " + fees
                + "\n\t  Sender: " + senderAddress
                + "\n\t  Reciever: " + recipientAddress
                + "\n  [TRANSACTION END]");


        }

    }
}
