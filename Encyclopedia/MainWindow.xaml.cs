using Encyclopedia;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace FairyTaleEncyclopedia
{
    public partial class MainWindow : Window
    {
        private string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";
        private MySqlConnection connection;
        private int selectedWriterId;

        public MainWindow()
        {
            InitializeComponent();
            LoadWriters();
        }

        private void LoadWriters()
        {
            try
            {
                using (connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Writers";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    WritersGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
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
                    WritersGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}");
            }
        }

        private void WritersGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (WritersGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)WritersGrid.SelectedItem;

                string firstName = selectedRow["FirstName"].ToString();
                string lastName = selectedRow["LastName"].ToString();
                string patronymic = selectedRow["Patronymic"] != DBNull.Value ? selectedRow["Patronymic"].ToString() : null;
                DateTime? birthDate = selectedRow["BirthDate"] != DBNull.Value ? (DateTime?)selectedRow["BirthDate"] : null;
                DateTime? deathDate = selectedRow["DeathDate"] != DBNull.Value ? (DateTime?)selectedRow["DeathDate"] : null;
                string countryName = selectedRow["CountryName"] != DBNull.Value ? selectedRow["CountryName"].ToString() : null;
                string biography = selectedRow["Biography"] != DBNull.Value ? selectedRow["Biography"].ToString() : null;
                byte[] photoData = selectedRow["Photo"] != DBNull.Value ? (byte[])selectedRow["Photo"] : null;

                // Открываем новое окно с полной информацией
                WriterDetailsWindow detailsWindow = new WriterDetailsWindow();
                detailsWindow.SetWriterDetails(firstName, lastName, patronymic, birthDate, deathDate, countryName, biography, photoData);
                detailsWindow.ShowDialog();
            }
        }

        private void AddWriter_Click(object sender, RoutedEventArgs e)
        {
            AddWriterWindow addWindow = new AddWriterWindow();
            if (addWindow.ShowDialog() == true)
            {
                string firstName = addWindow.FirstName;
                string lastName = addWindow.LastName;
                string patronymic = addWindow.Patronymic;
                DateTime? birthDate = addWindow.BirthDate;
                DateTime? deathDate = addWindow.DeathDate;
                string countryName = addWindow.CountryName;
                string biography = addWindow.Biography;
                byte[] photoData = addWindow.PhotoData;

                try
                {
                    using (connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        // Проверка, существует ли страна
                        string checkCountryQuery = "SELECT CountryName FROM Countries WHERE CountryName = @CountryName";
                        MySqlCommand checkCountryCmd = new MySqlCommand(checkCountryQuery, connection);
                        checkCountryCmd.Parameters.AddWithValue("@CountryName", countryName);
                        var result = checkCountryCmd.ExecuteScalar();

                        // Если страна не существует, создаем новую запись в таблице Countries
                        if (result == null)
                        {
                            string insertCountryQuery = "INSERT INTO Countries (CountryName) VALUES (@CountryName)";
                            MySqlCommand insertCountryCmd = new MySqlCommand(insertCountryQuery, connection);
                            insertCountryCmd.Parameters.AddWithValue("@CountryName", countryName);
                            insertCountryCmd.ExecuteNonQuery();
                            MessageBox.Show($"Новая страна '{countryName}' была добавлена.");
                        }

                        // Вставляем нового писателя
                        // Проверка и выполнение SQL-запроса на добавление писателя в базу данных
                        using (MySqlConnection connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "INSERT INTO Writers (FirstName, LastName, Patronymic, BirthDate, DeathDate, CountryName, Biography, Photo) " +
                                           "VALUES (@FirstName, @LastName, @Patronymic, @BirthDate, @DeathDate, @CountryName, @Biography, @Photo)";

                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@FirstName", firstName);
                                command.Parameters.AddWithValue("@LastName", lastName);
                                command.Parameters.AddWithValue("@Patronymic", patronymic ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@BirthDate", birthDate ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@DeathDate", deathDate ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@CountryName", countryName ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Biography", biography ?? (object)DBNull.Value);

                                // Если фото загружено, передаем его в запрос. Если нет — передаем NULL.
                                if (photoData != null && photoData.Length > 0)
                                {
                                    command.Parameters.AddWithValue("@Photo", photoData);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@Photo", DBNull.Value);
                                }

                                command.ExecuteNonQuery();
                            }
                        }


                        MessageBox.Show("Писатель успешно добавлен.");
                        LoadWriters();  // Обновляем данные в таблице
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении писателя: {ex.Message}");
                }
            }
        }

        private void EditWriter_Click(object sender, RoutedEventArgs e)
        {
            if (WritersGrid.SelectedItem != null)
            {
                // Получаем данные выбранного автора
                DataRowView selectedRow = (DataRowView)WritersGrid.SelectedItem;

                int writerID = Convert.ToInt32(selectedRow["WriterID"]);
                string currentFirstName = selectedRow["FirstName"].ToString();
                string currentLastName = selectedRow["LastName"].ToString();
                string currentPatronymic = selectedRow["Patronymic"] != DBNull.Value ? selectedRow["Patronymic"].ToString() : null;
                DateTime? currentBirthDate = selectedRow["BirthDate"] != DBNull.Value ? (DateTime?)selectedRow["BirthDate"] : null;
                DateTime? currentDeathDate = selectedRow["DeathDate"] != DBNull.Value ? (DateTime?)selectedRow["DeathDate"] : null;
                string currentCountryName = selectedRow["CountryName"] != DBNull.Value ? selectedRow["CountryName"].ToString() : null;
                string currentBiography = selectedRow["Biography"] != DBNull.Value ? selectedRow["Biography"].ToString() : null;
                byte[] currentPhotoData = selectedRow["Photo"] != DBNull.Value ? (byte[])selectedRow["Photo"] : null;

                // Открываем окно редактирования и передаем данные выбранного автора
                EditWriterWindow editWindow = new EditWriterWindow(writerID, currentFirstName, currentLastName, currentPatronymic,
                                                                    currentBirthDate, currentDeathDate, currentCountryName, currentBiography, currentPhotoData);

                // Показываем окно для редактирования
                if (editWindow.ShowDialog() == true)
                {
                    // Получаем новые данные от пользователя из editWindow
                    string newFirstName = editWindow.FirstName;
                    string newLastName = editWindow.LastName;
                    string newPatronymic = editWindow.Patronymic;
                    DateTime? newBirthDate = editWindow.BirthDate;
                    DateTime? newDeathDate = editWindow.DeathDate;
                    string newCountryName = editWindow.CountryName;
                    string newBiography = editWindow.Biography;
                    byte[] newPhotoData = editWindow.PhotoData;

                    try
                    {
                        using (connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();

                            // Проверка существования страны
                            string checkCountryQuery = "SELECT CountryName FROM Countries WHERE CountryName = @CountryName";
                            MySqlCommand checkCountryCmd = new MySqlCommand(checkCountryQuery, connection);
                            checkCountryCmd.Parameters.AddWithValue("@CountryName", newCountryName);
                            var result = checkCountryCmd.ExecuteScalar();

                            if (result == null)
                            {
                                string insertCountryQuery = "INSERT INTO Countries (CountryName) VALUES (@CountryName)";
                                MySqlCommand insertCountryCmd = new MySqlCommand(insertCountryQuery, connection);
                                insertCountryCmd.Parameters.AddWithValue("@CountryName", newCountryName);
                                insertCountryCmd.ExecuteNonQuery();
                                MessageBox.Show($"Новая страна '{newCountryName}' была добавлена.");
                            }

                            // Обновление записи писателя
                            string updateQuery = "UPDATE Writers SET FirstName = @FirstName, LastName = @LastName, Patronymic = @Patronymic, " +
                                                 "BirthDate = @BirthDate, DeathDate = @DeathDate, CountryName = @CountryName, Biography = @Biography, Photo = @Photo " +
                                                 "WHERE WriterID = @WriterID";

                            using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@WriterID", writerID);
                                command.Parameters.AddWithValue("@FirstName", newFirstName);
                                command.Parameters.AddWithValue("@LastName", newLastName);
                                command.Parameters.AddWithValue("@Patronymic", string.IsNullOrEmpty(newPatronymic) ? (object)DBNull.Value : newPatronymic);
                                command.Parameters.AddWithValue("@BirthDate", newBirthDate.HasValue ? (object)newBirthDate.Value : DBNull.Value);
                                command.Parameters.AddWithValue("@DeathDate", newDeathDate.HasValue ? (object)newDeathDate.Value : DBNull.Value);
                                command.Parameters.AddWithValue("@CountryName", string.IsNullOrEmpty(newCountryName) ? (object)DBNull.Value : newCountryName);
                                command.Parameters.AddWithValue("@Biography", string.IsNullOrEmpty(newBiography) ? (object)DBNull.Value : newBiography);

                                if (newPhotoData != null && newPhotoData.Length > 0)
                                {
                                    command.Parameters.AddWithValue("@Photo", newPhotoData);
                                }
                                else
                                {
                                    command.Parameters.AddWithValue("@Photo", DBNull.Value);
                                }

                                command.ExecuteNonQuery();
                            }

                            MessageBox.Show("Данные писателя успешно обновлены.");
                            LoadWriters();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении писателя: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите писателя для редактирования.");
            }
        }


        private void DeleteWriter_Click(object sender, RoutedEventArgs e)
        {
            if (WritersGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)WritersGrid.SelectedItem;
                int writerID = Convert.ToInt32(selectedRow["WriteriD"]);

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этого автора?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (connection = new MySqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "DELETE FROM Writers WHERE WriteriD = @WriteriD";
                            MySqlCommand cmd = new MySqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@WriteriD", writerID);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Автор успешно удален.");
                            LoadWriters();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении автора: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите автора для удаления.");
            }
        }

        private void OpenWorksWindow_Click(object sender, RoutedEventArgs e )
        {
            if (selectedWriterId != null) 
            {
                WorksWindow worksWindow = new WorksWindow(selectedWriterId);
                worksWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите писателя.");
            }
        }

    }
}