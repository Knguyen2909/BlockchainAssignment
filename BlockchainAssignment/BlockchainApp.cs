using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BlockchainAssignment
{

    public partial class BlockchainApp : Form
    {
        Blockchain blockchain;
        public BlockchainApp()
        {
            InitializeComponent();
            blockchain = new Blockchain();
            outputToRichTextBox1("New Blockchain Initialised");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
               // Produces error when attempting to print nothing
        private void printBtn_Click(object sender, EventArgs e)
        {

            if (Int32.TryParse(indexTBox.Text, out int index))
                outputToRichTextBox1(blockchain.GetBlockAsString(index));
            else
                outputToRichTextBox1("Invalid Block No.");

        }

        private void outputToRichTextBox1(string toBePrinted) { richTextBox1.Text = toBePrinted; }
        private void outputToTextBox(TextBox TBox, string toBePrinted) { TBox.Text = toBePrinted; }

        private void GenWalletBtn_Click(object sender, EventArgs e)
        {

            Wallet.Wallet myNewWallet = new Wallet.Wallet(out string privKey);
            String publicKey = myNewWallet.publicID;

            outputToTextBox(privKeyTBox, privKey);
            outputToTextBox(pubKeyTBox, publicKey) ;
        }

        private void ValKeysBtn_Click(object sender, EventArgs e)
        {
            Color color; 
           color =  Wallet.Wallet.ValidatePrivateKey(privKeyTBox.Text, pubKeyTBox.Text) ? Color.Green : Color.Red;
            valKeysBtn.BackColor = color;
        }

        private void CreateTransBtn_Click(object sender, EventArgs e)
        {

            bool testtrans = true;
            if (testtrans)
            {
                if ((Wallet.Wallet.ValidatePrivateKey(privKeyTBox.Text, pubKeyTBox.Text)) && (blockchain.GetBalance(pubKeyTBox.Text) > Convert.ToSingle(amountTBox.Text))){
                    Transaction transaction = new Transaction(pubKeyTBox.Text, privKeyTBox.Text, recieverKeyTBox.Text, Convert.ToSingle(amountTBox.Text), Convert.ToSingle(feeTBox.Text));
                    blockchain.addTransactionPool(transaction);
                    outputToRichTextBox1(transaction.ReturnString());
                }
                else
                {
                    outputToRichTextBox1("Transaction failed - Check keys match and sufficient funds are available");
                }
            }
            else
            {
                Transaction transaction = new Transaction(pubKeyTBox.Text, privKeyTBox.Text, recieverKeyTBox.Text, Convert.ToSingle(amountTBox.Text), Convert.ToSingle(feeTBox.Text));
                blockchain.addTransactionPool(transaction);
                outputToRichTextBox1(transaction.ReturnString());

            }   

        }

        private void BlockGenBtn_Click(object sender, EventArgs e)
        {
            List<Transaction> transactions = blockchain.getPendingTransactions();
            Block block = new Block(blockchain.GetLastBlock());
            blockchain.addToBlock(block);

            richTextBox1.Text = blockchain.ToString();
        }

        private void PrintAllBtn_Click(object sender, EventArgs e)
        {
            string printall = "";
            for (int i = 0; i < blockchain.maxBlock; i++)
            {
                printall += (blockchain.BlockString(Convert.ToInt32(i)) + "\n \n");
            }
            outputToRichTextBox1(printall);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Block block = new Block(blockchain.GetLastBlock(), blockchain.retTPool(), pubKeyTBox.Text, comboBox1.SelectedIndex, addressFind.Text);
            blockchain.purgeTPool(block.transactionList);
            blockchain.addToBlock(block);
            Console.WriteLine("added new block to chain - with " + block.transactionList.Count + " transactions");
        }

        private void ReadPendTrandBtn_Click(object sender, EventArgs e)
        { 
            UpdateText(String.Join("\n", blockchain.TransactionPool));
        }

        private void UpdateText(string v)
        {

        }

        private void PubKeyTBox_TextChanged(object sender, EventArgs e)
        {
            valKeysBtn.BackColor = Color.AntiqueWhite;
        }
        private void PrivKeyTBox_TextChanged(object sender, EventArgs e)
        {
            valKeysBtn.BackColor = Color.AntiqueWhite;
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            outputToRichTextBox1(blockchain.ToString());
        }

        private void BlockchainApp_Load(object sender, EventArgs e)
        {

        }

          private void IndexInput_TextChanged(object sender, EventArgs e)
        {

        }
    

        private void Validate_Click(object sender, EventArgs e)
        {
            // CASE: Genesis Block - Check only hash as no transactions are currently present
            if (blockchain.Blocks.Count == 1)
            {
                if (!Blockchain.ValidateHash(blockchain.Blocks[0])) // Recompute Hash to check validity
                    outputToRichTextBox1("Blockchain is invalid - Hash ");
                else
                    outputToRichTextBox1("Blockchain is valid");
                return;
            }

            Console.WriteLine(" NewBlock: " + (blockchain.Blocks.Count - 1)); 
            for (int i = 1; i < blockchain.Blocks.Count - 1; i++)
            {
                Console.WriteLine("Hash for block " + i);
                if (
                    blockchain.Blocks[i].prevHash != blockchain.Blocks[i - 1].hash || // Check hash "chain"
                    !Blockchain.ValidateHash(blockchain.Blocks[i]) ||  // Check each Block hash
                    !Blockchain.ValidateMerkleRoot(blockchain.Blocks[i]) // Check transaction integrity using Merkle Root
                )
                {
                    outputToRichTextBox1("Blockchain is invalid " + (blockchain.Blocks[i].prevHash != blockchain.Blocks[i - 1].hash).ToString() + "  " +
                    !Blockchain.ValidateHash(blockchain.Blocks[i]) + "  " + // Check each Block hash
                    !Blockchain.ValidateMerkleRoot(blockchain.Blocks[i]) + " " + blockchain.Blocks[i].nonce);// C);
                    return;
                }
            }
            outputToRichTextBox1("Blockchain is valid");
        }

        // Check the balance of current user
        private void CheckBalance_Click(object sender, EventArgs e)
        {
            outputToRichTextBox1(blockchain.GetBalance(pubKeyTBox.Text).ToString() + " Assignment Coin");
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void privKeyLbl_Click(object sender, EventArgs e)
        {

        }

        private void pubKeyLbl_Click(object sender, EventArgs e)
        {

        }
    }
}
