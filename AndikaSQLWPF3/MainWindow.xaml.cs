using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;
using System.Windows.Controls;
using System.Security.Cryptography;

namespace AndikaSQLWPF3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [AllowNull]
        SqlConnection sqlConnection;

        public MainWindow()
        {
            Init_db();

            InitializeComponent();

            Refresh();

        }

        private bool ValidateForm()
        {
            if (kode_box.Text == String.Empty) return false;
            if (mk_box.Text == String.Empty) return false;
            if (sks_box.Text == String.Empty) return false;
            if (semester_box.Text == String.Empty) return false;
            if (jurusan_box.Text == String.Empty) return false;

            return true;
        }

        private void AddData(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                MessageBox.Show("Invalid Value");
                return;
            }


            string queryString = "INSERT INTO study VALUES(@kode, @mk, @sks, @semester, @jurusan)";
            SqlCommand command = new(queryString, sqlConnection);

            command.Parameters.AddWithValue("@kode", kode_box.Text);
            command.Parameters.AddWithValue("@mk", mk_box.Text);
            command.Parameters.AddWithValue("@sks", sks_box.Text);
            command.Parameters.AddWithValue("@semester", semester_box.Text);
            command.Parameters.AddWithValue("@jurusan", jurusan_box.Text);

            try
            {
                sqlConnection.Open();
                command.ExecuteNonQuery();
                main_list.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
            }
            finally
            {
                sqlConnection.Close();
                ClearForm();
            }

            Refresh();
        }

        private void FetchAllData(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Init_db()
        {
            string connectionString = @"data source=LAPTOP-HSIL1GS3;Database=dummy2; Integrated Security=true";
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                MessageBox.Show("Connected to sql");

            }
            catch (Exception)
            {
                MessageBox.Show("Error");
                throw;
            }
        }

        private void Refresh()
        {

            string queryString = "SELECT * FROM study";
            SqlCommand command = new(queryString, sqlConnection);
            

            sqlConnection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                main_list.Items.Clear();
                while (reader.Read())
                {
                    var row = new { 
                        Kode = reader.GetInt32(0),
                        MK = reader.GetString(1),
                        Sks=reader.GetInt32(2),
                        Semester = reader.GetInt32(3),
                        Jurusan = reader.GetString(4)
                    };
                    main_list.Items.Add(row);
                }
            }

            sqlConnection.Close();
        }

        private void ClearForm()
        {
            kode_box.Text = String.Empty;
            mk_box.Text = String.Empty;
            sks_box.Text = String.Empty;
            semester_box.Text = String.Empty;
            jurusan_box.Text = String.Empty;
        }

        private void Search(string param)
        {
            string queryString = "SELECT * FROM study where kode LIKE @param OR mata_kuliah LIKE @param OR sks LIKE @param OR semester LIKE @param OR jurusan LIKE @param";
            SqlCommand command = new(queryString, sqlConnection);
            command.Parameters.AddWithValue("@param", "%" + param + "%");

            sqlConnection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                main_list.Items.Clear();
                while (reader.Read())
                {
                    var row = new
                    {
                        Kode = reader.GetInt32(0),
                        MK = reader.GetString(1),
                        Sks = reader.GetInt32(2),
                        Semester = reader.GetInt32(3),
                        Jurusan = reader.GetString(4)
                    };
                    main_list.Items.Add(row);
                }
            }

            sqlConnection.Close();
        }

        private void SearchKeydown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;

            string searchedValue = search_box.Text;

            Search(searchedValue);
        }
    }
}
