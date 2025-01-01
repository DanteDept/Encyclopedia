using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace FairyTaleEncyclopedia
{
    public partial class GenreDetailsWindow : Window
    {
        private string connectionString = "server=localhost;user=Dante;database=FairyTaleEncyclopedia;password=Alighieri;";

        public GenreDetailsWindow(string genreName)
        {
            InitializeComponent();
            LoadGenreDetails(genreName);
        }

        private void LoadGenreDetails(string genreName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Параметризованный запрос для выбора описания жанра по имени
                    string query = "SELECT Description FROM Genres WHERE GenreName = @GenreName";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@GenreName", genreName);

                    // Выполняем запрос и получаем описание жанра
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        // Отображаем описание в GenreLabel
                        GenreBlock.Text = $"Описание жанра: {result.ToString()}";
                    }
                    else
                    {
                        // Если жанр не найден
                        GenreBlock.Text = "Описание жанра не найдено.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
                }
            }
        }
    }
}
