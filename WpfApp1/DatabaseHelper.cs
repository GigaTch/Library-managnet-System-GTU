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

        public static List<Person> GetPeople()
        {
            var people = new List<Person>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM People";
                using (var command = new SQLiteCommand(selectQuery, connection))
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

        public static void AddPerson(string name, int age, string branch)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO People (Name, Age, Branch) VALUES (@Name, @Age, @Branch)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@Branch", branch);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdatePerson(int id, string name, int age, string branch)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE People SET Name = @Name, Age = @Age, Branch = @Branch WHERE ID = @ID";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Age", age);
                    command.Parameters.AddWithValue("@ID", id);
                    command.Parameters.AddWithValue("@Branch", branch);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeletePerson(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM People WHERE ID = @ID";
                using (var command = new SQLiteCommand(deleteQuery, connection))
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
        


        

    }
}
