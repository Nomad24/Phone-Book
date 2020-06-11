using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Phone_Book
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                panel1.Enabled = true;
                App.PhoneBook.AddPhoneBookRow(App.PhoneBook.NewPhoneBookRow());
                phoneBookBindingSource.MoveLast();
                txtNumber.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            panel1.Enabled = true;
            txtNumber.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            phoneBookBindingSource.ResetBindings(false);
            panel1.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                phoneBookBindingSource.EndEdit();
                App.PhoneBook.AcceptChanges();
                App.PhoneBook.WriteXml(string.Format("{0}//data.dat", Application.StartupPath));
                panel1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        static AppData db;
        protected static AppData App
        {
            get
            {
                if (db == null)
                    db = new AppData();
                return db;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string fileName = string.Format("{0}//data.dat", Application.StartupPath);
            if (File.Exists(fileName))
                App.PhoneBook.ReadXml(fileName);
            phoneBookBindingSource.DataSource = App.PhoneBook;
            panel1.Enabled = false;
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Are you sure want to delete this record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    phoneBookBindingSource.RemoveCurrent();
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var query = from o in App.PhoneBook
                                where o.PhoneNumber == txtSearch.Text || o.Name.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) || o.Surname.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) || o.Email.ToLowerInvariant() == txtSearch.Text.ToLowerInvariant()
                                select o;
                    dataGridView.DataSource = query.ToList();
                }
                else
                    dataGridView.DataSource = phoneBookBindingSource;
            }
        }
    }
}
