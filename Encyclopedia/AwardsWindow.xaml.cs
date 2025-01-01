using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;

namespace FairyTaleEncyclopedia
{
    public partial class AwardsWindow : Window
    {
        private string connectionString = "server=localhost;user=Dante;database=FairyTaleEncyclopedia;password=Alighieri;";
        private ObservableCollection<Award> awards;

        public AwardsWindow()
        {
            InitializeComponent();
            LoadAwards();
        }

        private void LoadAwards()
        {
            awards = new ObservableCollection<Award>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                Awards.AwardID,
                Awards.AwardName,
                Awards.YearReceived,
                Writers.WriterID,
                CONCAT(Writers.FirstName, ' ', Writers.LastName) AS FullName,
                Works.WorkID,
                Works.Title,
                Awards.Description
            FROM 
                Awards
            LEFT JOIN 
                Writers ON Awards.WriterID = Writers.WriterID
            LEFT JOIN 
                Works ON Awards.WorkID = Works.WorkID";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    awards.Add(new Award
                    {
                        AwardID = reader.GetInt32("AwardID"),
                        AwardName = reader.GetString("AwardName"),
                        YearReceived = reader.IsDBNull(reader.GetOrdinal("YearReceived")) ? (int?)null : reader.GetInt32("YearReceived"),
                        FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString("FullName"),
                        Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString("Title"),
                        Description = reader.GetString("Description")
                    });
                }
            }

            AwardsDataGrid.ItemsSource = awards;
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditAwardWindow();

            // Открываем окно добавления награды
            if (addEditWindow.ShowDialog() == true)
            {
                // Получаем добавленную награду
                Award newAward = addEditWindow.CurrentAward;
                AddAward(newAward);  // Вызываем метод для добавления в базу данных
                LoadAwards(); // Обновляем список наград
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли награда для редактирования
            if (AwardsDataGrid.SelectedItem is Award selectedAward)
            {
                var addEditWindow = new AddEditAwardWindow(selectedAward);

                // Открываем окно редактирования награды
                if (addEditWindow.ShowDialog() == true)
                {
                    Award updatedAward = addEditWindow.CurrentAward;
                    UpdateAward(updatedAward); // Вызываем метод для обновления в базе данных
                    LoadAwards(); // Обновляем список наград
                }
            }
            else
            {
                MessageBox.Show("Выберите награду для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AwardsDataGrid.SelectedItem is Award selectedAward)
            {
                DeleteAward(selectedAward.AwardID);
                awards.Remove(selectedAward);
            }
        }

        private void AddAward(Award award)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Получаем WriterID по FullName
                string getWriterIdQuery = "SELECT WriterID FROM Writers WHERE CONCAT(FirstName, ' ', LastName) = @FullName";
                MySqlCommand getWriterCmd = new MySqlCommand(getWriterIdQuery, connection);
                getWriterCmd.Parameters.AddWithValue("@FullName", award.FullName);
                int writerId = Convert.ToInt32(getWriterCmd.ExecuteScalar());

                // Получаем WorkID по Title
                string getWorkIdQuery = "SELECT WorkID FROM Works WHERE Title = @Title";
                MySqlCommand getWorkCmd = new MySqlCommand(getWorkIdQuery, connection);
                getWorkCmd.Parameters.AddWithValue("@Title", award.Title);
                int workId = Convert.ToInt32(getWorkCmd.ExecuteScalar());

                // Вставляем новую запись награды
                string query = "INSERT INTO Awards (AwardName, YearReceived, WriterID, WorkID, Description) VALUES (@AwardName, @YearReceived, @WriterID, @WorkID, @Description)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AwardName", award.AwardName);
                cmd.Parameters.AddWithValue("@YearReceived", award.YearReceived);
                cmd.Parameters.AddWithValue("@WriterID", writerId);
                cmd.Parameters.AddWithValue("@WorkID", workId);
                cmd.Parameters.AddWithValue("@Description", award.Description);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateAward(Award award)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Получаем WriterID по FullName
                string getWriterIdQuery = "SELECT WriterID FROM Writers WHERE CONCAT(FirstName, ' ', LastName) = @FullName";
                MySqlCommand getWriterCmd = new MySqlCommand(getWriterIdQuery, connection);
                getWriterCmd.Parameters.AddWithValue("@FullName", award.FullName);
                int writerId = Convert.ToInt32(getWriterCmd.ExecuteScalar());

                // Получаем WorkID по Title
                string getWorkIdQuery = "SELECT WorkID FROM Works WHERE Title = @Title";
                MySqlCommand getWorkCmd = new MySqlCommand(getWorkIdQuery, connection);
                getWorkCmd.Parameters.AddWithValue("@Title", award.Title);
                int workId = Convert.ToInt32(getWorkCmd.ExecuteScalar());

                // Обновляем запись награды
                string query = "UPDATE Awards SET AwardName = @AwardName, YearReceived = @YearReceived, WriterID = @WriterID, WorkID = @WorkID, Description = @Description WHERE AwardID = @AwardID";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AwardID", award.AwardID);
                cmd.Parameters.AddWithValue("@AwardName", award.AwardName);
                cmd.Parameters.AddWithValue("@YearReceived", award.YearReceived);
                cmd.Parameters.AddWithValue("@WriterID", writerId);
                cmd.Parameters.AddWithValue("@WorkID", workId);
                cmd.Parameters.AddWithValue("@Description", award.Description);
                cmd.ExecuteNonQuery();
            }
        }


        private void DeleteAward(int awardID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Awards WHERE AwardID = @AwardID";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@AwardID", awardID);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class Award
    {
        public int AwardID { get; set; }
        public string AwardName { get; set; }
        public int? YearReceived { get; set; }
        public int? WriterID { get; set; }
        public int? WorkID { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }


}
