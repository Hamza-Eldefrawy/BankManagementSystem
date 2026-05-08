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
    public partial class AccountForm : Form
    {
        public AccountForm()
        {
            InitializeComponent();
        }
        private void AccountForm_Load(object sender, EventArgs e)
        {
            cmbType.Items.AddRange(new string[] { "Savings", "Checking", "Business" });

            DataTable branches = DatabaseHelper.ExecuteQuery("SELECT Branch_ID, BranchName FROM Branch");
            cmbBranch.DataSource = branches;
            cmbBranch.DisplayMember = "BranchName";
            cmbBranch.ValueMember = "Branch_ID";

            DataTable customers = DatabaseHelper.ExecuteQuery("SELECT SSN, Fname + ' ' + Lname AS FullName FROM Customer");
            cmbCustomer.DataSource = customers;
            cmbCustomer.DisplayMember = "FullName";
            cmbCustomer.ValueMember = "SSN";

            LoadAccounts();
        }

        private void LoadAccounts()
        {
            string query = @"SELECT A.AccountID, A.Balance, A.Type, B.BranchName, C.Fname + ' ' + C.Lname AS OwnerName 
                             FROM Account A 
                             JOIN Branch B ON A.Branch_ID = B.Branch_ID 
                             JOIN Owns O ON A.AccountID = O.AccountID 
                             JOIN Customer C ON O.customerID = C.SSN";
            dgvAccounts.DataSource = DatabaseHelper.ExecuteQuery(query);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int accountID = int.Parse(txtAccountID.Text);
            decimal balance = decimal.Parse(txtBalance.Text);
            string type = cmbType.SelectedItem.ToString();
            int branchID = (int)cmbBranch.SelectedValue;
            string customerID = cmbCustomer.SelectedValue.ToString();

            string query1 = "INSERT INTO Account (AccountID, Balance, Type, Branch_ID) VALUES (@id, @balance, @type, @branch)";
            SqlParameter[] p1 = {
                new SqlParameter("@id", accountID),
                new SqlParameter("@balance", balance),
                new SqlParameter("@type", type),
                new SqlParameter("@branch", branchID)
            };
            DatabaseHelper.ExecuteNonQuery(query1, p1);

            string query2 = "INSERT INTO Owns (customerID, AccountID) VALUES (@cust, @acc)";
            SqlParameter[] p2 = {
                new SqlParameter("@cust", customerID),
                new SqlParameter("@acc", accountID)
            };
            DatabaseHelper.ExecuteNonQuery(query2, p2);

            MessageBox.Show("Account created and linked to customer!");
            LoadAccounts();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Account SET Balance = @balance, Type = @type, Branch_ID = @branch WHERE AccountID = @id";
            SqlParameter[] p = {
                new SqlParameter("@id", int.Parse(txtAccountID.Text)),
                new SqlParameter("@balance", decimal.Parse(txtBalance.Text)),
                new SqlParameter("@type", cmbType.SelectedItem.ToString()),
                new SqlParameter("@branch", (int)cmbBranch.SelectedValue)
            };

            if (DatabaseHelper.ExecuteNonQuery(query, p) > 0)
            {
                MessageBox.Show("Account updated successfully!");
                LoadAccounts();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this account?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int accountId = int.Parse(txtAccountID.Text);

                // Delete child records first to prevent FK constraint errors
                DatabaseHelper.ExecuteNonQuery("DELETE FROM Owns WHERE AccountID = @id", new SqlParameter("@id", accountId));
                DatabaseHelper.ExecuteNonQuery("DELETE FROM Account WHERE AccountID = @id", new SqlParameter("@id", accountId));

                MessageBox.Show("Account deleted successfully!");
                LoadAccounts();
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadAccounts();
        }

        private void dgvAccounts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAccounts.Rows[e.RowIndex];
                txtAccountID.Text = row.Cells["AccountID"].Value.ToString();
                txtBalance.Text = row.Cells["Balance"].Value.ToString();
                cmbType.Text = row.Cells["Type"].Value.ToString();
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
