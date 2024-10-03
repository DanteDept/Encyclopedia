using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace FairyTaleEncyclopedia
{
    public partial class WorksWindow : Window
    {
        private int _writerId;

        public WorksWindow(int writerId)
        {
            InitializeComponent();
            _writerId = writerId;
            LoadWorks();
        }

        // Метод для загрузки произведений из базы данных
        private void LoadWorks()
        {
            List<Work> works = new List<Work>();
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";  // Замените на вашу строку подключения

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT Title, GenreName, YearOfPublication FROM Works WHERE WriterID = @WriterID", connection);
                    command.Parameters.AddWithValue("@WriterID", _writerId);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            works.Add(new Work
                            {
                                Title = reader.GetString("Title"),
                                GenreName = reader.GetString("GenreName"),
                                YearOfPublication = reader.IsDBNull("YearOfPublication") ? null : (int?)reader.GetInt32("YearOfPublication")
                            });
                        }
                    }

                    WorksDataGrid.ItemsSource = works;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке произведений: " + ex.Message);
                }
            }
        }

        private void AddWorkButton_Click(object sender, RoutedEventArgs e)
        {
            AddWorkWindow addWorkWindow = new AddWorkWindow(_writerId);
            if (addWorkWindow.ShowDialog() == true)
            {
                LoadWorks(); // Обновляем список после добавления
            }
        }

        private void EditWorkButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования выбранного произведения
        }

        private void DeleteWorkButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления выбранного произведения
        }
    }

    public class Work
    {
        public string Title { get; set; }
        public string GenreName { get; set; }
        public int? YearOfPublication { get; set; }

    }
}
