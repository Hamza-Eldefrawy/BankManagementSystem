using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BankManagementSystem
{
    public partial class CustomerForm : Form
    {
        public CustomerForm()
        {
            InitializeComponent();
        }

        // ========================= LOAD CUSTOMERS =========================
        private void LoadCustomers()
        {
            string query =
                "SELECT c.SSN, c.Fname, c.Lname, c.Adress, c.DOB, p.Phone " +
                "FROM Customer c " +
                "LEFT JOIN Customer_Phone p ON c.SSN = p.SSN";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvCustomers.DataSource = dt;
        }

        // ========================= CLEAR FIELDS =========================
        private void ClearFields()
        {
            txtSSN.Clear();
            txtFname.Clear();
            txtLname.Clear();
            txtAddress.Clear();
            txtPhone.Clear();

            dtpDOB.Value = DateTime.Now;

            txtSSN.Focus();
        }

        // ========================= FORM LOAD =========================
        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        // ========================= ADD CUSTOMER =========================
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string ssn = txtSSN.Text.Trim();
            string fname = txtFname.Text.Trim();
            string lname = txtLname.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string dob = dtpDOB.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(ssn) ||
                string.IsNullOrEmpty(fname) ||
                string.IsNullOrEmpty(lname))
            {
                MessageBox.Show(
                    "SSN, First Name, and Last Name are required.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string query =
                "INSERT INTO Customer (SSN, Fname, Lname, Adress, DOB) " +
                "VALUES (@ssn, @fname, @lname, @address, @dob)";

            SqlParameter[] parameters =
            {
                new SqlParameter("@ssn", ssn),
                new SqlParameter("@fname", fname),
                new SqlParameter("@lname", lname),
                new SqlParameter("@address", address),
                new SqlParameter("@dob", dob)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

            if (result > 0)
            {
                if (!string.IsNullOrEmpty(phone))
                {
                    string phoneQuery =
                        "INSERT INTO Customer_Phone (SSN, Phone) " +
                        "VALUES (@ssn, @phone)";

                    SqlParameter[] phoneParameters =
                    {
                        new SqlParameter("@ssn", ssn),
                        new SqlParameter("@phone", phone)
                    };

                    DatabaseHelper.ExecuteNonQuery(
                        phoneQuery,
                        phoneParameters);
                }

                MessageBox.Show("Customer added successfully!");

                LoadCustomers();
                ClearFields();
            }
        }

        // ========================= UPDATE CUSTOMER =========================
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string ssn = txtSSN.Text.Trim();

            // Get existing data first
            string selectQuery = "SELECT Fname, Lname, Adress, DOB FROM Customer WHERE SSN = @ssn";
            SqlParameter[] selectParams =
            {
                new SqlParameter("@ssn", ssn)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(selectQuery, selectParams);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Customer not found!");
                return;
            }

            DataRow row = dt.Rows[0];

            string fname = string.IsNullOrWhiteSpace(txtFname.Text)
                ? row["Fname"].ToString()
                : txtFname.Text.Trim();

            string lname = string.IsNullOrWhiteSpace(txtLname.Text)
                ? row["Lname"].ToString()
                : txtLname.Text.Trim();

            string address = string.IsNullOrWhiteSpace(txtAddress.Text)
                ? row["Adress"].ToString()
                : txtAddress.Text.Trim();

            string phone = txtPhone.Text.Trim();

            DateTime dob = Convert.ToDateTime(row["DOB"]);

            string query =
                "UPDATE Customer SET " +
                "Fname = @fname, " +
                "Lname = @lname, " +
                "Adress = @address, " +
                "DOB = @dob " +
                "WHERE SSN = @ssn";

            SqlParameter[] parameters =
            {
                new SqlParameter("@ssn", ssn),
                new SqlParameter("@fname", fname),
                new SqlParameter("@lname", lname),
                new SqlParameter("@address", address),
                new SqlParameter("@dob", dob)
            };

            int result = DatabaseHelper.ExecuteNonQuery(query, parameters);

            if (result > 0)
            {
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    string checkPhoneQuery =
                        "SELECT COUNT(*) FROM Customer_Phone WHERE SSN = @ssn";

                    SqlParameter[] checkParams =
                    {
                        new SqlParameter("@ssn", ssn)
                    };

                    object countObj = DatabaseHelper.ExecuteScalar(checkPhoneQuery, checkParams);
                    int count = Convert.ToInt32(countObj);

                    if (count > 0)
                    {
                        string updatePhoneQuery =
                            "UPDATE Customer_Phone SET Phone = @phone WHERE SSN = @ssn";

                        SqlParameter[] phoneParams =
                        {
                            new SqlParameter("@phone", phone),
                            new SqlParameter("@ssn", ssn)
                        };

                        DatabaseHelper.ExecuteNonQuery(updatePhoneQuery, phoneParams);
                    }
                    else
                    {
                        string insertPhoneQuery =
                            "INSERT INTO Customer_Phone (SSN, Phone) VALUES (@ssn, @phone)";

                        SqlParameter[] phoneParams =
                        {
                            new SqlParameter("@ssn", ssn),
                            new SqlParameter("@phone", phone)
                        };

                        DatabaseHelper.ExecuteNonQuery(insertPhoneQuery, phoneParams);
                    }
                }

                MessageBox.Show("Customer updated successfully!");
                LoadCustomers();
            }
        }

        // ========================= DELETE CUSTOMER =========================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string ssn = txtSSN.Text.Trim();

            if (string.IsNullOrEmpty(ssn))
            {
                MessageBox.Show(
                    "Please select a customer to delete.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                string deletePhoneQuery =
                    "DELETE FROM Customer_Phone WHERE SSN = @ssn";

                SqlParameter[] phoneParams =
                {
                    new SqlParameter("@ssn", ssn)
                };

                DatabaseHelper.ExecuteNonQuery(
                    deletePhoneQuery,
                    phoneParams);

                string query =
                    "DELETE FROM Customer WHERE SSN = @ssn";

                SqlParameter[] parameters =
                {
                    new SqlParameter("@ssn", ssn)
                };

                int result =
                    DatabaseHelper.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    MessageBox.Show("Customer deleted successfully!");

                    LoadCustomers();
                    ClearFields();
                }
            }
        }

        // ========================= CLEAR BUTTON =========================
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // ========================= LOAD BUTTON =========================
        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        // ========================= GRID CELL CLICK =========================
        private void dgvCustomers_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row =
                    dgvCustomers.Rows[e.RowIndex];

                txtSSN.Text =
                    row.Cells["SSN"].Value.ToString();

                txtFname.Text =
                    row.Cells["Fname"].Value.ToString();

                txtLname.Text =
                    row.Cells["Lname"].Value.ToString();

                txtAddress.Text =
                    row.Cells["Adress"].Value?.ToString();

                txtPhone.Text =
                    row.Cells["Phone"].Value?.ToString();

                dtpDOB.Value =
                    Convert.ToDateTime(row.Cells["DOB"].Value);
            }
        }
    }
}