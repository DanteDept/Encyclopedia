using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySql.Data.MySqlClient;

namespace FairyTaleEncyclopedia
{
    public partial class WorksWindow : Window
    {
        private string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";
        private int _writerId;
        MySqlConnection connection;

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

        private void OpenGenreWindow_Click(object sender, RoutedEventArgs e)
        {
                AddGenreWindow genreWindow = new AddGenreWindow();
                genreWindow.ShowDialog();

        }

        private void RemoveText(object sender, EventArgs e)
        {
            if (SearchBox.Text == "Поиск")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Поиск";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text;

            if (searchTerm == "Поиск" || string.IsNullOrWhiteSpace(searchTerm))
            {
                // Если поле пустое или равно "Поиск", выводим весь список
                searchTerm = ""; // Пустая строка для вывода всех записей
            }

            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Writers WHERE FirstName LIKE @search";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    WorksDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}");
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


        private void WorksDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Получаем информацию о том, что кликнуло
            var cellInfo = WorksDataGrid.CurrentCell;

            // Проверяем, что клик был по ячейке
            if (cellInfo != null && cellInfo.Column is DataGridTextColumn column)
            {
                // Если клик по колонке Жанра, открываем окно с описанием жанра
                if (column.Header.ToString() == "Жанр" && WorksDataGrid.SelectedItem is DataRowView selectedRow)
                {
                    string genreName = selectedRow["GenreName"].ToString();
                    OpenGenreDetails(genreName);  // Вызовем функцию открытия окна жанра
                    return;  // Прекращаем выполнение, если жанр был выбран
                }

                // Обычное поведение — открытие окна с деталями произведения
                if (WorksDataGrid.SelectedItem is DataRowView row)
                {
                    int workID = Convert.ToInt32(row["WorkID"]);
                    WorkDetailsWindow detailsWindow = new WorkDetailsWindow(workID);
                    detailsWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Не удалось открыть информацию о произведении. Пожалуйста, выберите корректное произведение.");
                }
            }
        }

        // Метод для открытия окна с описанием жанра
        private void OpenGenreDetails(string genreName)
        {
            // Здесь откроем окно с подробным описанием жанра
            GenreDetailsWindow genreWindow = new GenreDetailsWindow(genreName);
            genreWindow.ShowDialog();
        }



        // Открытие окна для редактирования выбранного произведения
        private void EditWorkButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorksDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int workID = Convert.ToInt32(selectedRow["WorkID"]);

                EditWorkWindow editWorkWindow = new EditWorkWindow(workID);
                if (editWorkWindow.ShowDialog() == true)
                {
                    LoadWorks();
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
            if (WorksDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int workID = Convert.ToInt32(selectedRow["WorkID"]);

                if (MessageBox.Show("Вы уверены, что хотите удалить это произведение?",
                                    "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            MySqlCommand command = new MySqlCommand("DELETE FROM Works WHERE WorkID = @WorkID", connection);
                            command.Parameters.AddWithValue("@WorkID", workID);
                            command.ExecuteNonQuery();
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
