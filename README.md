# 🏦 Bank Management System

A desktop **Windows Forms** application built with **C#** and **Microsoft SQL Server** that simulates core banking operations.  
This project is **Phase 2** of a Database course project — implementing the relational schema designed in Phase 1, wiring it to a fully functional UI, and performing complete CRUD interactions against the database.

---

## 📋 Table of Contents

- [Project Theme](#-project-theme)
- [Phase Overview](#-phase-overview)
- [Database Architecture](#-database-architecture)
- [Forms & Functionality](#-forms--functionality)
- [Navigation Flow](#-navigation-flow)
- [Project Structure](#-project-structure)
- [How to Run / Setup](#-how-to-run--setup)
- [Technologies Used](#-technologies-used)

---

## 🏛 Project Theme

The **Bank Management System** models real-world banking workflows — customer registration, account creation, loan tracking, and transaction recording. Users authenticate through a login screen and navigate a central dashboard to manage every entity stored in the underlying SQL Server database.

---

## 📌 Phase Overview

| Phase | Focus |
|-------|-------|
| **Phase 1** | Initial database design & ER schema (conceptual / logical design only). |
| **Phase 2** *(current)* | Full implementation — database creation with sample data, a multi-form Windows Forms UI, and complete CRUD operations connecting the frontend to the database. |

### Phase 2 Deliverables

| Part | Requirement | Status |
|------|-------------|--------|
| **Part 1 – Database Architecture** | Relational schema with every table seeded with ≥ 3 rows of sample data. | ✅ |
| **Part 2 – Application Interface** | At least 5 distinct forms with seamless navigation. | ✅ (5 forms) |
| **Part 3 – Database Interactions** | Full CRUD — Select, Insert, Update, Delete through the UI. | ✅ |

---

## 🗄 Database Architecture

The SQL script (`Bank_system.sql`) creates a `Bank` database with the following **8 tables** (plus 1 junction table):

| # | Table | Purpose |
|---|-------|---------|
| 1 | `Branch` | Bank branches (ID, location, name, manager reference). |
| 2 | `Employee` | Staff members assigned to branches. |
| 3 | `Customer` | Bank customers (SSN, name, address, DOB). |
| 4 | `Account` | Customer bank accounts (Savings / Checking / Business). |
| 5 | `Loan` | Loans issued to customers (Personal / Mortgage / Car). |
| 6 | `Transaction` | Financial transactions linked to accounts. |
| 7 | `Login_Credentials` | Username & password pairs linked to customers. |
| 8 | `Customer_Phone` | Multi-valued phone numbers for customers (composite PK). |
| 9 | `Owns` | Junction table linking customers ↔ accounts (many-to-many). |

All tables are pre-seeded with sample data (branches, employees, customers, accounts, loans, transactions, credentials, and phone records) so the application is ready to use immediately after running the script.

---

## 🖥 Forms & Functionality

The application consists of **5 distinct Windows Forms**, each serving a dedicated purpose:

### 1. 🔐 Login Form (`LoginForm`)
- **Purpose:** Authenticates users before granting access to the system.
- **Features:**
  - Username and password input fields.
  - Validates credentials against the `Login_Credentials` table using parameterized queries.
  - Displays success / failure messages.
  - **Exit** button to close the application.

### 2. 📊 Dashboard Form (`DashboardForm`)
- **Purpose:** Central navigation hub — the main menu after login.
- **Features:**
  - Four navigation buttons: **Customers**, **Accounts**, **Loans**, **Transactions**.
  - Each button opens the corresponding management form as a dialog.
  - **Logout** button with a confirmation prompt that returns the user to the Login screen.

### 3. 👥 Customer Form (`CustomerForm`)
- **Purpose:** Full management of customer records and their phone numbers.
- **CRUD Operations:**
  - **Select** – Loads all customers (joined with phone numbers) into a DataGridView.
  - **Insert** – Adds a new customer to the `Customer` table and optionally inserts a phone number into `Customer_Phone`.
  - **Update** – Updates customer details; intelligently inserts or updates phone records.
  - **Delete** – Removes associated phone records first, then deletes the customer (handles FK constraints).
- **Extras:** Click any row in the grid to auto-populate the input fields; **Clear** and **Load** buttons.

### 4. 💳 Account Form (`AccountForm`)
- **Purpose:** Manage bank accounts and link them to customers.
- **CRUD Operations:**
  - **Select** – Displays accounts joined with branch names and owner names.
  - **Insert** – Creates an account and simultaneously inserts an ownership record into the `Owns` table.
  - **Update** – Modifies balance, type, or branch assignment.
  - **Delete** – Removes ownership records before deleting the account (handles FK constraints).
- **Dropdowns:** Account type (Savings / Checking / Business), Branch, and Customer are populated from the database.

### 5. 💰 Loan Form (`LoanForm`)
- **Purpose:** Track and manage loans issued to customers.
- **CRUD Operations:**
  - **Select** – Lists all loans with branch and customer names.
  - **Insert** – Records a new loan with amount, type, interest rate, start date, branch, and customer.
  - **Update** – Modifies any loan field.
  - **Delete** – Removes a loan record with confirmation.
- **Dropdowns:** Loan type (Personal / Mortgage / Car), Branch, and Customer are populated from the database.

### 6. 🔄 Transaction Form (`TransactionForm`)
- **Purpose:** Record and view financial transactions.
- **CRUD Operations:**
  - **Select** – Displays all transactions (ID, amount, type, account, date).
  - **Insert** – Records a new transaction and **automatically updates the account balance** (deposits add, withdrawals/transfers subtract).
  - **Delete** – Removes a transaction record with confirmation.
- **Dropdowns:** Transaction type (Deposit / Withdrawal / Transfer) and Account ID are populated from the database.

> **Note:** The application includes a shared `DatabaseHelper` utility class that centralizes all database connectivity — providing `ExecuteQuery`, `ExecuteNonQuery`, and `ExecuteScalar` methods used by every form.

---

## 🗺 Navigation Flow

```
┌─────────────────────┐
│     Login Form       │
│  (Authentication)    │
└─────────┬───────────┘
          │ Valid credentials
          ▼
┌─────────────────────┐
│   Dashboard Form     │
│   (Navigation Hub)   │
├─────────────────────┤
│  [Customers]  ──────────►  Customer Form   (CRUD)
│  [Accounts]   ──────────►  Account Form    (CRUD)
│  [Loans]      ──────────►  Loan Form       (CRUD)
│  [Transactions] ────────►  Transaction Form(CRUD)
│  [Logout]     ──────────►  Back to Login
└─────────────────────┘
```

1. The user starts at the **Login Form** and enters their credentials.
2. On successful login, the **Dashboard Form** opens.
3. From the Dashboard, the user clicks any of the four buttons to open the corresponding management form as a **modal dialog**.
4. Each management form provides full CRUD functionality and a **Close / Exit** button to return to the Dashboard.
5. The **Logout** button on the Dashboard returns the user to the Login Form.

---

## 📁 Project Structure

```
BankManagementSystem/
│
├── BankManagementSystem.slnx          # Solution file
├── Bank_system.sql                    # SQL script (CREATE TABLE + INSERT sample data)
├── .gitignore                         # Git ignore rules
├── README.md                          # Project documentation (this file)
│
└── BankManagementSystem/              # Main project folder
    ├── BankManagementSystem.csproj     # C# project file
    ├── Program.cs                     # Application entry point
    ├── DatabaseHelper.cs              # Centralized DB connection & query helper
    │
    ├── LoginForm.cs / .Designer.cs / .resx
    ├── DashboardForm.cs / .Designer.cs / .resx
    ├── CustomerForm.cs / .Designer.cs / .resx
    ├── AccountForm.cs / .Designer.cs / .resx
    ├── LoanForm.cs / .Designer.cs / .resx
    ├── TransactionForm.cs / .Designer.cs / .resx
    │
    ├── App.config                     # Application configuration
    └── Properties/                    # Assembly info & settings
```

---

## 🚀 How to Run / Setup

### Prerequisites

- **SQL Server** (any edition — Express, Developer, or LocalDB)
- **Visual Studio** 2019 or later (with the **.NET desktop development** workload installed)

### Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/<your-username>/BankManagementSystem.git
   ```

2. **Create the database**
   - Open **SQL Server Management Studio** (SSMS) or any SQL client connected to your local SQL Server instance.
   - Open and execute the `Bank_system.sql` script — this will:
     - Create the `Bank` database.
     - Create all required tables with constraints and foreign keys.
     - Seed every table with sample data (≥ 3 rows each).

3. **Configure the connection** *(if needed)*
   - The app connects using Windows Authentication to a local instance (`Server=.;Database=Bank;Integrated Security=True`).
   - If your SQL Server instance name differs, update the connection string in `BankManagementSystem/DatabaseHelper.cs`.

4. **Open & run the project**
   - Open `BankManagementSystem.slnx` in Visual Studio.
   - Build the solution (**Ctrl + Shift + B**).
   - Press **F5** to run.

5. **Login with sample credentials**
   | Username | Password |
   |----------|----------|
   | `mona_k` | `hashed_pass_001` |
   | `karim_s` | `hashed_pass_002` |

---

## 🛠 Technologies Used

| Technology | Purpose |
|------------|---------|
| **C# (.NET Framework)** | Application logic |
| **Windows Forms** | Desktop UI framework |
| **Microsoft SQL Server** | Relational database engine |
| **ADO.NET** (`System.Data.SqlClient`) | Database connectivity |
| **Visual Studio** | IDE & build toolchain |