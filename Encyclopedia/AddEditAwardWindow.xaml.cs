using FairyTaleEncyclopedia;
using System.Windows;

namespace YourNamespace
{
    public partial class AddEditAwardWindow : Window
    {
        public Award CurrentAward { get; private set; }

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
            // Загрузите список писателей из базы данных и установите в WriterComboBox
        }

        private void LoadWorks()
        {
            // Загрузите список произведений из базы данных и установите в WorkComboBox
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение данных
            CurrentAward.AwardName = AwardNameTextBox.Text;
            CurrentAward.YearReceived = int.TryParse(YearReceivedTextBox.Text, out int year) ? year : (int?)null;
            CurrentAward.Description = DescriptionTextBox.Text;

            // Здесь можно установить WriterID и WorkID на основе выбранных элементов в ComboBox
            // Например:
            CurrentAward.WriterID = ((Writer)WriterComboBox.SelectedItem).WriterID;
            CurrentAward.WorkID = ((Work)WorkComboBox.SelectedItem).WorkID;

            DialogResult = true;
        }
    }
}
