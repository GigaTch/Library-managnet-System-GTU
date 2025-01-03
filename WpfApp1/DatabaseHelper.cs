using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace WpfApp1
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString = "Data Source=Database.sqlite;Version=3;";

        public static void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS People (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Age INTEGER NOT NULL,
                    Branch TEXT
                );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
                string addBranchColumnQuery = "ALTER TABLE People ADD COLUMN Branch TEXT";
                try
                {
                    using (var command = new SQLiteCommand(addBranchColumnQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (SQLiteException ex)
                {
                    // Ignore error if the column already exists
                    if (!ex.Message.Contains("duplicate column name"))
                    {
                        throw;
                    }
                }
            }
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Books (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    BookName TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    StaffID INTEGER NOT NULL, -- Staff who signed the book out (Foreign Key)
                    SignOutDate TEXT NOT NULL, -- Date of the signing out
                    FOREIGN KEY (StaffID) REFERENCES People(ID) -- Foreign Key linking to the People table
                );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void AddBook(string bookName, string author, int staffId, DateTime signOutDate)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Books (BookName, Author, StaffID, SignOutDate) VALUES (@BookName, @Author, @StaffID, @SignOutDate)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@BookName", bookName);
                    command.Parameters.AddWithValue("@Author", author);
                    command.Parameters.AddWithValue("@StaffID", staffId);
                    command.Parameters.AddWithValue("@SignOutDate", signOutDate.ToString("yyyy-MM-dd"));
                    command.ExecuteNonQuery();
                }
            }
        }
        public static List<Book> GetAllBooks()
        {
            var books = new List<Book>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT b.ID, b.BookName, b.Author, b.SignOutDate, p.Name AS StaffName " +
                                     "FROM Books b " +
                                     "JOIN People p ON b.StaffID = p.ID";
                using (var command = new SQLiteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        books.Add(new Book
                        {
                            ID = reader.GetInt32(0),
                            BookName = reader.GetString(1),
                            Author = reader.GetString(2),
                            SignOutDate = DateTime.Parse(reader.GetString(3)),
                            StaffName = reader.GetString(4)
                        });
                    }
                }
            }
            return books;
        }

        public static List<Person> GetPeople(int page, int pageSize)
        {
            var people = new List<Person>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM People LIMIT {pageSize} OFFSET {(page - 1) * pageSize}";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        people.Add(new Person
                        {
                            ID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Age = reader.GetInt32(2),
                            Branch = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                        });
                    }
                }
            }
            return people;
        }

        public static int GetPeopleCount()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM People";
                using (var command = new SQLiteCommand(query, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static void AddPerson(Person person)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO People (Name, Age, Branch) VALUES (@Name, @Age, @Branch)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@Age", person.Age);
                    command.Parameters.AddWithValue("@Branch", person.Branch);
                    //command.Parameters.AddWithValue("@Branch", person.Branch ?? string.Empty);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdatePerson(Person person)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "UPDATE People SET Name = @Name, Age = @Age, Branch=@Branch WHERE ID = @ID";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", person.ID);
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@Age", person.Age);
                    command.Parameters.AddWithValue("@Branch", person.Branch);
                    command.ExecuteNonQuery();
                }
            }

        }
        public static void UpdateBook(int bookId, string bookName, string author, int staffId, DateTime signOutDate)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string updateQuery = @"
                UPDATE Books 
                SET BookName = @BookName, Author = @Author, StaffID = @StaffID, SignOutDate = @SignOutDate 
                WHERE ID = @ID"; 
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@ID", bookId);
                    command.Parameters.AddWithValue("@BookName", bookName);
                    command.Parameters.AddWithValue("@Author", author);
                    command.Parameters.AddWithValue("@StaffID", staffId);
                    command.Parameters.AddWithValue("@SignOutDate", signOutDate);
                    command.ExecuteNonQuery();
                }
            }

        }

        public static void DeletePerson(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM People WHERE ID = @ID";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void DeleteBook(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "DELETE FROM Books WHERE ID = @ID";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static List<Person> SearchPeopleByName(string name)
        {
            var people = new List<Person>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string searchQuery = "SELECT * FROM People WHERE Name LIKE @Name";
                using (var command = new SQLiteCommand(searchQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", $"%{name}%"); // Use LIKE for partial match
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            people.Add(new Person
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Age = reader.GetInt32(2),
                                Branch = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            return people;
        }
        public static List<Book> SearchBooksByName(string bookName)
        {
            var books = new List<Book>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string searchQuery = @"
            SELECT b.ID, b.BookName, b.Author, b.SignOutDate, p.Name AS StaffName
            FROM Books b
            JOIN People p ON b.StaffID = p.ID
            WHERE b.BookName LIKE @BookName";
                using (var command = new SQLiteCommand(searchQuery, connection))
                {
                    command.Parameters.AddWithValue("@BookName", $"%{bookName}%"); // Partial match
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book
                            {
                                ID = reader.GetInt32(0),
                                BookName = reader.GetString(1),
                                Author = reader.GetString(2),
                                SignOutDate = DateTime.Parse(reader.GetString(3)),
                                StaffName = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return books;
        }





    }
}
