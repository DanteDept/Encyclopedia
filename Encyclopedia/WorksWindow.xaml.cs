using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace FairyTaleEncyclopedia
{
    public partial class WorksWindow : Window
    {
        private string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    {
                        connection.Open();
                        string query = "SELECT * FROM Works";
                        MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        WorksDataGrid.ItemsSource = dt.DefaultView;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
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
            if (WorksDataGrid.SelectedItem is Work selectedWork)
            {
                EditWorkWindow editWorkWindow = new EditWorkWindow(selectedWork.WorkID);
                if (editWorkWindow.ShowDialog() == true)
                {
                    LoadWorks(); // Обновляем список после редактирования
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите произведение для редактирования.");
            }
        }


        // Удаление выбранного произведения
        private void DeleteWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorksDataGrid.SelectedItem is Work selectedWork)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить это произведение?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            MySqlCommand command = new MySqlCommand("DELETE FROM Works WHERE WorkID = @WorkID", connection);
                            command.Parameters.AddWithValue("@WorkID", selectedWork.WorkID);
                            command.ExecuteNonQuery();

                            // Обновление списка после успешного удаления
                            LoadWorks();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка при удалении произведения: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите произведение для удаления.");
            }
        }

    }

    public class Work
    {
        public int WorkID { get; set; }
        public string Title { get; set; }
        public string GenreName { get; set; }
        public int? YearOfPublication { get; set; }
    }
}
