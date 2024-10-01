using Encyclopedia;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Input;


namespace FairyTaleEncyclopedia
{
    public partial class MainWindow : Window
    {
        private string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";
        private MySqlConnection connection;

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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchBox.Text;

            if (!string.IsNullOrEmpty(searchTerm))
            {
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
                        string insertWriterQuery = "INSERT INTO Writers (FirstName, LastName, Patronymic, BirthDate, DeathDate, CountryName, Biography) " +
                                                   "VALUES (@FirstName, @LastName, @Patronymic, @BirthDate, @DeathDate, @CountryName, @Biography)";
                        MySqlCommand insertWriterCmd = new MySqlCommand(insertWriterQuery, connection);
                        insertWriterCmd.Parameters.AddWithValue("@FirstName", firstName);
                        insertWriterCmd.Parameters.AddWithValue("@LastName", lastName);
                        insertWriterCmd.Parameters.AddWithValue("@Patronymic", patronymic ?? (object)DBNull.Value);
                        insertWriterCmd.Parameters.AddWithValue("@BirthDate", birthDate.HasValue ? birthDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                        insertWriterCmd.Parameters.AddWithValue("@DeathDate", deathDate.HasValue ? deathDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                        insertWriterCmd.Parameters.AddWithValue("@CountryName", countryName);
                        insertWriterCmd.Parameters.AddWithValue("@Biography", biography ?? (object)DBNull.Value);
                        insertWriterCmd.ExecuteNonQuery();

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
                byte[] imageData = selectedRow["Photo"] != DBNull.Value ? (byte[])selectedRow["Photo"] : null;

                // Открываем новое окно с полной информацией
                WriterDetailsWindow detailsWindow = new WriterDetailsWindow();
                detailsWindow.SetWriterDetails(firstName, lastName, patronymic, birthDate, deathDate, countryName, biography, imageData);
                detailsWindow.ShowDialog();
            }
        }



        private void EditWriter_Click(object sender, RoutedEventArgs e)
        {
            if (WritersGrid.SelectedItem != null)
            {
                DataRowView selectedRow = (DataRowView)WritersGrid.SelectedItem;

                int writerID = Convert.ToInt32(selectedRow["WriterID"]);
                string currentFirstName = selectedRow["FirstName"].ToString();
                string currentLastName = selectedRow["LastName"].ToString();
                string currentPatronymic = selectedRow["Patronymic"] != DBNull.Value ? selectedRow["Patronymic"].ToString() : null;
                DateTime? currentBirthDate = selectedRow["BirthDate"] != DBNull.Value ? (DateTime?)selectedRow["BirthDate"] : null;
                DateTime? currentDeathDate = selectedRow["DeathDate"] != DBNull.Value ? (DateTime?)selectedRow["DeathDate"] : null;
                string currentCountryName = selectedRow["CountryName"] != DBNull.Value ? selectedRow["CountryName"].ToString() : null;
                string currentBiography = selectedRow["Biography"] != DBNull.Value ? selectedRow["Biography"].ToString() : null;

                // Открываем окно редактирования с текущими данными
                AddWriterWindow editWindow = new AddWriterWindow
                {
                    FirstNameBox = { Text = currentFirstName },
                    LastNameBox = { Text = currentLastName },
                    PatronymicBox = { Text = currentPatronymic },
                    BirthDatePicker = { SelectedDate = currentBirthDate },
                    DeathDatePicker = { SelectedDate = currentDeathDate },
                    CountryNameBox = { Text = currentCountryName },
                    BiographyBox = { Text = currentBiography }
                };

                if (editWindow.ShowDialog() == true)
                {
                    string newFirstName = editWindow.FirstName;
                    string newLastName = editWindow.LastName;
                    string newPatronymic = editWindow.Patronymic;
                    DateTime? newBirthDate = editWindow.BirthDate;
                    DateTime? newDeathDate = editWindow.DeathDate;
                    string newCountryName = editWindow.CountryName;
                    string newBiography = editWindow.Biography;

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
                            string query = "UPDATE Writers SET FirstName = @FirstName, LastName = @LastName, Patronymic = @Patronymic, " +
                                           "BirthDate = @BirthDate, DeathDate = @DeathDate, CountryName = @CountryName, Biography = @Biography " +
                                           "WHERE WriterID = @WriterID";
                            MySqlCommand cmd = new MySqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@FirstName", newFirstName);
                            cmd.Parameters.AddWithValue("@LastName", newLastName);
                            cmd.Parameters.AddWithValue("@Patronymic", newPatronymic ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@BirthDate", newBirthDate.HasValue ? newBirthDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@DeathDate", newDeathDate.HasValue ? newDeathDate.Value.ToString("yyyy-MM-dd") : (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@CountryName", newCountryName);
                            cmd.Parameters.AddWithValue("@Biography", newBiography ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@WriterID", writerID);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Данные писателя успешно обновлены.");
                            LoadWriters();  // Обновляем данные в таблице
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

    }
}
