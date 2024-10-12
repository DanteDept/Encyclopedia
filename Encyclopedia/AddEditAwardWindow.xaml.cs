using MySql.Data.MySqlClient;
using System.Windows;

namespace FairyTaleEncyclopedia
{
    public partial class AddEditAwardWindow : Window
    {
        public Award CurrentAward { get; private set; }
        private string connectionString = "server=localhost;user=root;database=FairyTaleEncyclopedia;password=;";

        public AddEditAwardWindow(Award award = null)
        {
            InitializeComponent();
            CurrentAward = award ?? new Award(); // Если null, создаем новый

            // Заполняем поля, если это редактирование
            AwardNameTextBox.Text = CurrentAward.AwardName;
            YearReceivedTextBox.Text = CurrentAward.YearReceived?.ToString();
            DescriptionTextBox.Text = CurrentAward.Description;

            // Здесь можно заполнить ComboBox списками писателей и произведений, например:
            LoadWriters();
            LoadWorks();
        }

        private void LoadWriters()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT WriterID, FirstName, LastName FROM Writers";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    List<Writer> writers = new List<Writer>();

                    while (reader.Read())
                    {
                        Writer writer = new Writer
                        {
                            WriterID = reader.GetInt32("WriterID"),
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName")
                        };
                        writers.Add(writer);
                    }

                    WriterComboBox.ItemsSource = writers;
                    WriterComboBox.DisplayMemberPath = "FullName";
                    WriterComboBox.SelectedValuePath = "WriterID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке писателей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void LoadWorks()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT WorkID, Title FROM Works";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    List<Work> works = new List<Work>();

                    while (reader.Read())
                    {
                        Work work = new Work
                        {
                            WorkID = reader.GetInt32("WorkID"),
                            Title = reader.GetString("Title")
                        };
                        works.Add(work);
                    }

                    WorkComboBox.ItemsSource = works;
                    WorkComboBox.DisplayMemberPath = "Title";
                    WorkComboBox.SelectedValuePath = "WorkID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке произведений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что введены необходимые данные
            if (string.IsNullOrWhiteSpace(AwardNameTextBox.Text) ||
                WriterComboBox.SelectedItem == null ||
                WorkComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Заполняем свойства награды данными из полей
            CurrentAward.AwardName = AwardNameTextBox.Text;
            CurrentAward.YearReceived = int.TryParse(YearReceivedTextBox.Text, out int year) ? year : (int?)null;
            CurrentAward.Description = DescriptionTextBox.Text;

            // Устанавливаем WriterID и WorkID на основе выбранных элементов ComboBox
            var selectedWriter = (Writer)WriterComboBox.SelectedItem;
            var selectedWork = (Work)WorkComboBox.SelectedItem;

            if (selectedWriter != null)
            {
                CurrentAward.WriterID = selectedWriter.WriterID;
                CurrentAward.FullName = $"{selectedWriter.FirstName} {selectedWriter.LastName}";
            }
            if (selectedWork != null)
            {
                CurrentAward.WorkID = selectedWork.WorkID;
                CurrentAward.Title = selectedWork.Title;
            }

            // Устанавливаем результат, чтобы окно закрылось, и данные передались в основное окно
            DialogResult = true;
        }

        public class Writer
        {
            public int WriterID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string FullName => $"{FirstName} {LastName}";
        }

    }
}
