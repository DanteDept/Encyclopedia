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

        // Загрузка списка произведений из базы данных
        private void LoadWorks()
        {
            List<Work> works = new List<Work>();
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT WorkID, Title, GenreName, YearOfPublication FROM Works WHERE WriterID = @WriterID", connection);
                    command.Parameters.AddWithValue("@WriterID", _writerId);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            works.Add(new Work
                            {
                                WorkID = reader.GetInt32("WorkID"),
                                Title = reader.GetString("Title"),
                                GenreName = reader.GetString("GenreName"),
                                YearOfPublication = reader.IsDBNull("YearOfPublication") ? null : (int?)reader.GetInt32("YearOfPublication")
                            });
                        }
                    }

                    // Debug output to check if data is being loaded
                    if (works.Count > 0)
                    {
                        MessageBox.Show("Works loaded: " + works.Count);
                    }

                    WorksDataGrid.ItemsSource = works;  // Ensure works are assigned
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке произведений: " + ex.Message);
                }
            }
        }


        // Открытие окна для добавления нового произведения
        private void AddWorkButton_Click(object sender, RoutedEventArgs e)
        {
            AddWorkWindow addWorkWindow = new AddWorkWindow(_writerId);
            if (addWorkWindow.ShowDialog() == true)
            {
                LoadWorks(); // Обновление списка после добавления
            }
        }

        // Открытие окна для редактирования выбранного произведения
        private void EditWorkButton_Click(object sender, RoutedEventArgs e)
        {
            //if (WorksDataGrid.SelectedItem is Work selectedWork)
            //{
            //    EditWorkWindow editWorkWindow = new EditWorkWindow(selectedWork); // Передаем выбранное произведение
            //    if (editWorkWindow.ShowDialog() == true)
            //    {
            //        LoadWorks(); // Обновление списка после редактирования
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Пожалуйста, выберите произведение для редактирования.");
            //}
        }

        // Удаление выбранного произведения
        private void DeleteWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorksDataGrid.SelectedItem is Work selectedWork)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить это произведение?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DeleteWorkFromDatabase(selectedWork.WorkID);
                    LoadWorks(); // Обновление списка после удаления
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите произведение для удаления.");
            }
        }

        // Удаление произведения из базы данных
        private void DeleteWorkFromDatabase(int workId)
        {
            string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("DELETE FROM Works WHERE WorkID = @WorkID", connection);
                    command.Parameters.AddWithValue("@WorkID", workId);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении произведения: " + ex.Message);
                }
            }
        }
    }

    // Модель для работы с произведениями
    public class Work
    {
        public int WorkID { get; set; }
        public string Title { get; set; }
        public string GenreName { get; set; }
        public int? YearOfPublication { get; set; }
    }
}
