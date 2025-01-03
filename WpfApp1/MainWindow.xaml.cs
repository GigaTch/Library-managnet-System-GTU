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
        public ObservableCollection<Person> PeopleList { get; set; }
        public ObservableCollection<Book> BooksList { get; set; }
        //private ObservableCollection<Person> _people = new ObservableCollection<Person>();

        private int currentPage = 1;
        private int pageSize = 10; // Number of records per page
        private int totalRecords;

        public MainWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            PeopleList = new ObservableCollection<Person>();
            BooksList = new ObservableCollection<Book>();
            DataContext = this;

            ComboBoxStaff.ItemsSource = PeopleList;
            ComboBoxStaff.DisplayMemberPath = "Name";
            ComboBoxStaff.SelectedValuePath = "ID";


            // Load data when the window is initialized
            LoadPeople();
            LoadBooks();
        }


        private void LoadBooks()
        {
            var books = DatabaseHelper.GetAllBooks();
            BooksList.Clear();
            foreach (var book in books)
            {
                BooksList.Add(book);
            }
            DataGridBooks.ItemsSource = BooksList;
        }
        private void ButtonAddBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                string bookName = TextBoxBookName.Text;
                string author = TextBoxAuthor.Text;
                int staffId = (int)ComboBoxStaff.SelectedValue;
                DateTime signOutDate = DatePickerSignOutDate.SelectedDate.GetValueOrDefault();

                if (string.IsNullOrEmpty(bookName) || string.IsNullOrEmpty(author) || staffId == 0)
                {
                    MessageBox.Show("შეავსეთ ყველა ველი", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DatabaseHelper.AddBook(bookName, author, staffId, signOutDate);
                LoadBooks(); // Refresh the Books DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა! : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ButtonDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridBooks.SelectedItem is Book selectedBook)
            {
                DatabaseHelper.DeleteBook(selectedBook.ID);
                LoadBooks();
            }
            else
            {
                MessageBox.Show("წასაშლელათ მონიშნეთ მონაცემები.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ButtonUpdateBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //if (DataGridBooks.SelectedItem is Book selectedBook)
                //{

                //    selectedBook.BookName = TextBoxBookName.Text; // Replace with form input
                //    selectedBook.Author = TextBoxAuthor.Text;
                //    selectedBook.StaffName = ComboBoxStaff.Text;
                //    selectedBook.SignOutDate = DatePickerSignOutDate.DisplayDate;
                //    DatabaseHelper.UpdateBook(selectedBook);


                //    LoadPeople();

                //}
                if (DataGridBooks.SelectedItem is Book selectedBook)
                {
                    string bookName = TextBoxBookName.Text;
                    string author = TextBoxAuthor.Text;
                    int staffId = (int)ComboBoxStaff.SelectedValue;
                    DateTime signOutDate = DatePickerSignOutDate.SelectedDate.GetValueOrDefault();

                    if (string.IsNullOrEmpty(bookName) || string.IsNullOrEmpty(author) || staffId == 0)
                    {
                        MessageBox.Show("შეაცსეთ ყველა ველი!.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Update book in the database
                    DatabaseHelper.UpdateBook(selectedBook.ID, bookName, author, staffId, signOutDate);
                    LoadBooks(); // Refresh the Books DataGrid
                }
                else
                {
                    MessageBox.Show("მონიშნეთ მონაცემები.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPeople()
        {
            // Get total number of people records
            totalRecords = DatabaseHelper.GetPeopleCount();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Load the current page of people
            var people = DatabaseHelper.GetPeople(currentPage, pageSize);
            PeopleList.Clear();
            foreach (var person in people)
            {
                PeopleList.Add(person);
            }

            DataGridPeople.ItemsSource = PeopleList;
            TextBlockPageNumber.Text = $"Page {currentPage} of {totalPages}";
        }

        // Create a new person (open a dialog or form for user input)
        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate Age Input
                if (!int.TryParse(TextBoxAge.Text, out int age))
                {
                    MessageBox.Show("შეიყვანეთ ნორმალური პირადი ნომერი", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Validate Name and Branch
                string name = TextBoxName.Text.Trim();
                string branch = TextBoxBranch.Text.Trim();

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(branch))
                {
                    MessageBox.Show("გთხოვთ შეავსოთ სახელის და ფილიალის გრაფა!", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Add the Person to the Database
                var newPerson = new Person
                {
                    Name = name,
                    Age = age,
                    Branch = branch
                };
                DatabaseHelper.AddPerson(newPerson);

                // Reload the DataGrid
                LoadPeople();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა! : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Update the selected person
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {

            if (DataGridPeople.SelectedItem is Person selectedPerson && int.TryParse(TextBoxAge.Text, out int age))
            {
                string branch = TextBoxBranch.Text.Trim();
                selectedPerson.Name = TextBoxName.Text; // Replace with form input
                selectedPerson.Age = age;
                selectedPerson.Branch = branch;
                DatabaseHelper.UpdatePerson(selectedPerson);


                LoadPeople();

            }
        }

        // Delete the selected person
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPeople.SelectedItem is Person selectedPerson)
            {
                DatabaseHelper.DeletePerson(selectedPerson.ID);
                LoadPeople();
            }
            else
            {
                MessageBox.Show("წასაშლელათ მონიშნეთ მონაცემები", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Previous page button
        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPeople();
            }
        }

        // Next page button
        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadPeople();
            }
        }

        // Clear the input fields
        private void ClearFields()
        {
            TextBoxName.Clear();
            TextBoxAge.Clear();
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {   
                string searchQuery = SearchTextBox.Text.Trim();         
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var searchResults = DatabaseHelper.SearchPeopleByName(searchQuery);
                    PeopleList.Clear();
                    foreach (var person in searchResults)
                    {
                        PeopleList.Add(person);
                    }
                }
                else
                {
                    // If the search query is empty, reload all data
                    LoadPeople();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"შეცდომა! : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private void ButtonSearchBooks_Click(object sender, RoutedEventArgs e)
        {
            string searchName = TextBoxSearchBookName.Text.Trim();

            if (string.IsNullOrEmpty(searchName))
            {
                MessageBox.Show("შეიყვანეთ წიგნის სათაური.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var searchResults = DatabaseHelper.SearchBooksByName(searchName);

            if (searchResults.Any())
            {
                DataGridBooks.ItemsSource = searchResults;
            }
            else
            {
                MessageBox.Show("შესაბამისი წიგნი ვერ მოიძებნა", "Search Results", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



    }

}
