using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace FairyTaleEncyclopedia
{
    public partial class WorkDetailsWindow : Window
    {
        private int _workID;
        private string connectionString = "server=localhost;user=Dante;database=FairyTaleEncyclopedia;password=Alighieri;";

        public WorkDetailsWindow(int workID)
        {
            InitializeComponent();
            _workID = workID;
            LoadWorkDetails();
        }

        private void LoadWorkDetails()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Title, GenreName, YearOfPublication, Description FROM Works WHERE WorkID = @workID";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@workID", _workID);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Пример привязки данных к элементам интерфейса
                        TitleTextBlock.Text = reader["Title"].ToString();
                        GenreTextBlock.Text = reader["GenreName"].ToString();
                        YearTextBlock.Text = reader["YearOfPublication"].ToString();
                        DescriptionTextBlock.Text = reader["Description"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных о произведении: " + ex.Message);
                }
            }
        }
    }
}
