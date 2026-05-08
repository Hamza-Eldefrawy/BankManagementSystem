using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public partial class TransactionForm : Form
    {
        public TransactionForm()
        {
            InitializeComponent();
        }
        private void TransactionForm_Load(object sender, EventArgs e)
        {
            cmbType.Items.AddRange(new string[] { "Deposit", "Withdrawal", "Transfer" });

            cmbAccount.DataSource = DatabaseHelper.ExecuteQuery("SELECT AccountID FROM Account");
            cmbAccount.DisplayMember = "AccountID";
            cmbAccount.ValueMember = "AccountID";

            LoadTransactions();
        }

        private void LoadTransactions()
        {
            string query = "SELECT ID, Amount, Type, AccountID, TxnDate FROM [Transaction]";
            dgvTransactions.DataSource = DatabaseHelper.ExecuteQuery(query);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int txnID = int.Parse(txtTxnID.Text);
            decimal amount = decimal.Parse(txtAmount.Text);
            string type = cmbType.SelectedItem.ToString();
            int accountID = (int)cmbAccount.SelectedValue;

            string query = "INSERT INTO [Transaction] (ID, Amount, Type, AccountID) VALUES (@id, @amount, @type, @account)";
            SqlParameter[] parameters = {
                new SqlParameter("@id", txnID),
                new SqlParameter("@amount", amount),
                new SqlParameter("@type", type),
                new SqlParameter("@account", accountID)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

            if (result > 0)
            {
                string balanceQuery = type == "Deposit"
                    ? "UPDATE Account SET Balance = Balance + @amount WHERE AccountID = @acc"
                    : "UPDATE Account SET Balance = Balance - @amount WHERE AccountID = @acc";

                SqlParameter[] bp = {
                    new SqlParameter("@amount", amount),
                    new SqlParameter("@acc", accountID)
                };
                DatabaseHelper.ExecuteNonQuery(balanceQuery, bp);

                MessageBox.Show("Transaction recorded and balance updated!");
                LoadTransactions();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this transaction? (Note: This will not reverse account balances in this basic version)", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string query = "DELETE FROM [Transaction] WHERE ID = @id";
                if (DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@id", int.Parse(txtTxnID.Text))) > 0)
                {
                    MessageBox.Show("Transaction deleted.");
                    LoadTransactions();
                }
            }
        }

        private void dgvTransactions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTransactions.Rows[e.RowIndex];
                txtTxnID.Text = row.Cells["ID"].Value.ToString();
                txtAmount.Text = row.Cells["Amount"].Value.ToString();
                cmbType.Text = row.Cells["Type"].Value.ToString();
                cmbAccount.Text = row.Cells["AccountID"].Value.ToString();
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
