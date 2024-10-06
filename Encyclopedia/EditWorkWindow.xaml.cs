using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace FairyTaleEncyclopedia
{
    public partial class EditWorkWindow : Window
    {
        private int _workId;
        public string Title { get; set; }
        public string GenreName { get; set; }
        public int? YearOfPublication { get; set; }
        public string Description { get; set; }

        public EditWorkWindow(int workId)
        {
            InitializeComponent();
            _workId = workId;
            LoadGenres();
            LoadWorkData();
        }

        // Загрузка списка жанров из базы данных
        private void LoadGenres()
        {
            List<string> genres = new List<string>();
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT GenreName FROM Genres", connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            genres.Add(reader.GetString("GenreName"));
                        }
                    }

                    GenreComboBox.ItemsSource = genres;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки жанров: " + ex.Message);
                }
            }
        }

        // Загрузка текущих данных о произведении
        private void LoadWorkData()
        {
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT Title, GenreName, YearOfPublication, Description FROM Works WHERE WorkID = @WorkID", connection);
                    command.Parameters.AddWithValue("@WorkID", _workId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TitleBox.Text = reader.GetString("Title");
                            GenreComboBox.SelectedItem = reader.GetString("GenreName");
                            YearBox.Text = reader.IsDBNull("YearOfPublication") ? string.Empty : reader.GetInt32("YearOfPublication").ToString();
                            DescriptionBox.Text = reader.GetString("Description");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных произведения: " + ex.Message);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Title = TitleBox.Text;
            GenreName = GenreComboBox.Text;
            if (int.TryParse(YearBox.Text, out int year))
            {
                YearOfPublication = year;
            }
            else
            {
                YearOfPublication = null;
            }
            Description = DescriptionBox.Text;

            if (!string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(GenreName))
            {
                if (UpdateWorkInDatabase())
                {
                    MessageBox.Show("Изменения сохранены успешно!");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить изменения.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
            }
        }

        private bool UpdateWorkInDatabase()
        {
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("UPDATE Works SET Title = @Title, GenreName = @GenreName, YearOfPublication = @YearOfPublication, Description = @Description WHERE WorkID = @WorkID", connection);
                    command.Parameters.AddWithValue("@Title", Title);
                    command.Parameters.AddWithValue("@GenreName", GenreName);
                    command.Parameters.AddWithValue("@YearOfPublication", YearOfPublication.HasValue ? (object)YearOfPublication.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Description", Description);
                    command.Parameters.AddWithValue("@WorkID", _workId);

                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
                    return false;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
