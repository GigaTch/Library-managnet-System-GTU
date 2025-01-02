using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Person> _people = new ObservableCollection<Person>();
        private int _nextId = 1;
        public MainWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase(); // Ensure database is ready
            LoadPeople();
        }

        private void LoadPeople()
        {
            _people.Clear();
            foreach (var person in DatabaseHelper.GetPeople())
            {
                _people.Add(person);
            }
            DataGridItems.ItemsSource = _people;
        }
        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate Age Input
                if (!int.TryParse(TextBoxAge.Text, out int age))
                {
                    MessageBox.Show("Please enter a valid number for Age.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate Name and Branch
                string name = TextBoxName.Text.Trim();
                string branch = TextBoxBranch.Text.Trim();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(branch))
                {
                    MessageBox.Show("Name and Branch cannot be empty.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Add the Person to the Database
                DatabaseHelper.AddPerson(name, age, branch);

                // Reload the DataGrid
                LoadPeople();

                // Clear Input Fields
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonRead_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridItems.SelectedItem is Person selectedPerson)
            {
                TextBoxName.Text = selectedPerson.Name;
                TextBoxAge.Text = selectedPerson.Age.ToString();
                TextBoxBranch.Text = selectedPerson.Branch;
            }
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridItems.SelectedItem is Person selectedPerson && int.TryParse(TextBoxAge.Text, out int age))
            {
                string branch = TextBoxBranch.Text.Trim();
                DatabaseHelper.UpdatePerson(selectedPerson.ID, TextBoxName.Text, age, branch);
                LoadPeople();
                ClearFields();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridItems.SelectedItem is Person selectedPerson)
            {
                DatabaseHelper.DeletePerson(selectedPerson.ID);
                LoadPeople();
                ClearFields();
            }
        }

        private void ClearFields()
        {
            TextBoxName.Clear();
            TextBoxAge.Clear();
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = SearchTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var searchResults = DatabaseHelper.SearchPeopleByName(searchQuery);
                _people.Clear();
                foreach (var person in searchResults)
                {
                    _people.Add(person);
                }
            }
            else
            {
                // If the search query is empty, reload all data
                LoadPeople();
            }
        }
    }
}
