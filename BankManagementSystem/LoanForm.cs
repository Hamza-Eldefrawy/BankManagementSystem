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
    public partial class LoanForm : Form
    {
        public LoanForm()
        {
            InitializeComponent();
        }
        private void LoanForm_Load(object sender, EventArgs e)
        {
            cmbLoanType.Items.AddRange(new string[] { "Personal", "Mortgage", "Car" });

            cmbBranch.DataSource = DatabaseHelper.ExecuteQuery("SELECT Branch_ID, BranchName FROM Branch");
            cmbBranch.DisplayMember = "BranchName";
            cmbBranch.ValueMember = "Branch_ID";

            cmbCustomer.DataSource = DatabaseHelper.ExecuteQuery("SELECT SSN, Fname + ' ' + Lname AS FullName FROM Customer");
            cmbCustomer.DisplayMember = "FullName";
            cmbCustomer.ValueMember = "SSN";

            LoadLoans();
        }

        private void LoadLoans()
        {
            string query = @"SELECT L.ID, L.Amount, L.Type, L.interest_rate, L.startDate, B.BranchName, C.Fname + ' ' + C.Lname AS CustomerName 
                             FROM Loan L 
                             JOIN Branch B ON L.branch_id = B.Branch_ID 
                             JOIN Customer C ON L.customer_ID = C.SSN";
            dgvLoans.DataSource = DatabaseHelper.ExecuteQuery(query);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Loan (ID, Amount, Type, interest_rate, startDate, branch_id, customer_ID) VALUES (@id, @amt, @type, @rate, @date, @branch, @cust)";
            SqlParameter[] p = GetLoanParameters();

            if (DatabaseHelper.ExecuteNonQuery(query, p) > 0)
            {
                MessageBox.Show("Loan added successfully!");
                LoadLoans();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Loan SET Amount = @amt, Type = @type, interest_rate = @rate, startDate = @date, branch_id = @branch, customer_ID = @cust WHERE ID = @id";
            SqlParameter[] p = GetLoanParameters();

            if (DatabaseHelper.ExecuteNonQuery(query, p) > 0)
            {
                MessageBox.Show("Loan updated successfully!");
                LoadLoans();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this loan?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string query = "DELETE FROM Loan WHERE ID = @id";
                if (DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@id", int.Parse(txtLoanID.Text))) > 0)
                {
                    MessageBox.Show("Loan deleted.");
                    LoadLoans();
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadLoans();
        }
        private void dgvLoans_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLoans.Rows[e.RowIndex];
                txtLoanID.Text = row.Cells["ID"].Value.ToString();
                txtAmount.Text = row.Cells["Amount"].Value.ToString();
                cmbLoanType.Text = row.Cells["Type"].Value.ToString();
                txtInterestRate.Text = row.Cells["interest_rate"].Value.ToString();
                dtpStartDate.Value = Convert.ToDateTime(row.Cells["startDate"].Value);
            }
        }

        private SqlParameter[] GetLoanParameters()
        {
            return new SqlParameter[] {
                new SqlParameter("@id", int.Parse(txtLoanID.Text)),
                new SqlParameter("@amt", decimal.Parse(txtAmount.Text)),
                new SqlParameter("@type", cmbLoanType.SelectedItem.ToString()),
                new SqlParameter("@rate", decimal.Parse(txtInterestRate.Text)),
                new SqlParameter("@date", dtpStartDate.Value.ToString("yyyy-MM-dd")),
                new SqlParameter("@branch", (int)cmbBranch.SelectedValue),
                new SqlParameter("@cust", cmbCustomer.SelectedValue.ToString())
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
