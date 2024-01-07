﻿using Archivary.BACKEND.OBJECTS;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Archivary.BACKEND.USER_OPERATIONS.UserOperation;

namespace Archivary.BACKEND.BOOK_OPERATIONS
{
    public class BookOperation
    {

        public enum BookInfo
        {
            ISBN = 0,
            Title = 1,
            Category = 2,
            Genre = 3,
            Author = 4,
            Publisher = 5,
            Copyright = 6,
            Aisle = 7,
            Shelf = 8,
            ImagePath = 9,
        }

        #region Sorting methods
        public static void SortInfoBaseOnSequence(int[] sequence, string[] colValues)
        {
            //Insertion sort
            for (int i = 1; i < sequence.Length; ++i)
            {
                int tempt = sequence[i];
                string tempt2 = colValues[i];
                int j = i - 1;

                while (j >= 0 && sequence[j] > tempt)
                {
                    sequence[j + 1] = sequence[j];
                    colValues[j + 1] = colValues[j];
                    j--;
                }
                sequence[j + 1] = tempt;
                colValues[j + 1] = tempt2;
            }
        }

        #endregion

        public static List<Book> LoadBooksFromDatabase(string categoryFilter)
        {
            List<Book> booksList = new List<Book>();

            using (MySqlConnection connection = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                connection.Open();

                string query;
                if (categoryFilter.ToUpper() == "ALL")
                {
                    query = "SELECT * FROM books ORDER BY title ASC";
                }
                else
                {
                    query = "SELECT * FROM books WHERE category = @category ORDER BY title ASC";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (categoryFilter.ToUpper() != "All")
                    {
                        command.Parameters.AddWithValue("@category", categoryFilter);
                    }

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string title = reader.GetString("title");
                            string genre = reader.GetString("genre");
                            string author = reader.GetString("author");
                            string isbn = reader.GetString("isbn");
                            string category = reader.GetString("category");
                            string copyright = reader.GetString("copyright");
                            string publisher = reader.GetString("publisher");
                            string status = reader.GetString("status");
                            int aisle = reader.GetInt32("aisle");
                            int shelf = reader.GetInt32("shelf");
                            string imagePath = reader.GetString("book_img_path");

                            Book book = new Book(id, title, genre, author, isbn, category, copyright,
                                publisher, status, aisle, shelf, imagePath);

                            booksList.Add(book);
                        }
                    }
                }
            }
            return booksList;
        }

        public static List<Book> LoadAvailableBooksFromDatabase(string statusFilter)
        {
            List<Book> booksList = new List<Book>();

            using (MySqlConnection connection = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                connection.Open();
                string query = "SELECT * FROM books WHERE status = @status ORDER BY title ASC";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@status", statusFilter);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string title = reader.GetString("title");
                            string genre = reader.GetString("genre");
                            string author = reader.GetString("author");
                            string isbn = reader.GetString("isbn");
                            string category = reader.GetString("category");
                            string copyright = reader.GetString("copyright");
                            string publisher = reader.GetString("publisher");
                            string status = reader.GetString("status");
                            int aisle = reader.GetInt32("aisle");
                            int shelf = reader.GetInt32("shelf");
                            string imagePath = reader.GetString("book_img_path");

                            Book book = new Book(id, title, genre, author, isbn, category, copyright,
                                publisher, status, aisle, shelf, imagePath);

                            booksList.Add(book);
                        }
                    }
                }
            }
            return booksList;
        }

        public static List<Book> LoadReservedBooksOfUserFromDatabase(int borrowerId)
        {
            List<Book> bookList = new List<Book>();
            using (MySqlConnection connection = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                connection.Open();
                string query = "SELECT books.* FROM reserved_books JOIN books ON reserved_books.book_id = books.id WHERE reserved_books.borrower_id = @Id and reserved_books.is_borrowed = false";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", borrowerId);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string title = reader.GetString("title");
                            string genre = reader.GetString("genre");
                            string author = reader.GetString("author");
                            string isbn = reader.GetString("isbn");
                            string category = reader.GetString("category");
                            string copyright = reader.GetString("copyright");
                            string publisher = reader.GetString("publisher");
                            string status = reader.GetString("status");
                            int aisle = reader.GetInt32("aisle");
                            int shelf = reader.GetInt32("shelf");
                            string imagePath = reader.GetString("book_img_path");

                            Book book = new Book(id, title, genre, author, isbn, category, copyright,
                                publisher, status, aisle, shelf, imagePath);

                            bookList.Add(book);
                        }
                    }
                }
            }
            return bookList;
        }

        public static List<Book> SearchBooks(List<Book> books, string searchTerm)
        {
            // If the search term is empty, return all books
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Search Book")
            {
                return books;
            }

            // Convert the search term to lowercase for case-insensitive search
            string searchTermLower = searchTerm.ToLower();

            // Use LINQ to perform the search
            List<Book> searchResults = books
                .Where(book =>
                    book.BookTitle.ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookAuthor.ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookGenre.ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookISBN.ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookCategory.ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookPublisher.ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookStatus.ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookAisle.ToString().ToLower().Contains(searchTermLower.ToLower()) ||
                    book.BookShelf.ToString().ToLower().Contains(searchTermLower.ToLower())
                )
                .ToList();

            return searchResults;
        }

        public static void BorrowReserveBook(string table, List<Book> bookList, HashSet<string> ISBNs, int userId, int librarianID)
        {
            using (MySqlConnection conn = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                try
                {
                    foreach (Book book in bookList)
                    if (ISBNs.Contains(book.BookISBN))
                        using (MySqlCommand insertCommand = conn.CreateCommand())
                        {
                            insertCommand.Transaction = transaction;

                            string columnOne = (table == "borrowed_books") ? "borrowed_at" : "reserved_at";
                            string columnTwo = (table == "borrowed_books") ? "is_returned" : "is_borrowed";

                            insertCommand.CommandText = $"INSERT INTO {table} (book_id, borrower_id, {columnOne}, {columnTwo}, librarian_id) VALUES (@BookId, @UserId, @ColumnOneValue, false, @LibrarianId)";

                            insertCommand.Parameters.AddWithValue("@BookId", book.BookId);
                            insertCommand.Parameters.AddWithValue("@UserId", userId);
                            insertCommand.Parameters.AddWithValue("@ColumnOneValue", DateTime.Now);
                            insertCommand.Parameters.AddWithValue("@LibrarianId", librarianID);

                            insertCommand.ExecuteNonQuery();

                            string status = (table == "borrowed_books") ? "BORROWED" : "RESERVED";
                            UpdateBorrowReserveBook(transaction, status, book);
                        }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public static void UpdateBorrowReserveBook(MySqlTransaction transaction, string status, Book book)
        {
            using (MySqlCommand updateCommand = transaction.Connection.CreateCommand())
            {
                updateCommand.Transaction = transaction;
                updateCommand.CommandText = "UPDATE books SET status = @Status WHERE id = @BookId";

                updateCommand.Parameters.AddWithValue("@Status", status);
                updateCommand.Parameters.AddWithValue("@BookId", book.BookId);

                updateCommand.ExecuteNonQuery();
            }
        }

        public static int CheckCountOfExistingBorrowedReservedBooks(int userId, string type)
        {
            using (MySqlConnection connection = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                connection.Open();
                string reserved = type.Equals("borrowed_books") ? "" : "AND is_borrowed = false";
                string query = $"SELECT COUNT(*) FROM {type} WHERE borrower_id = @Id {reserved}";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", userId);

                    object result = command.ExecuteScalar();
                    return (result != null) ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public static void SetReservedBookToBorrowed(Book book, int borrowerId)
        {
            using (MySqlConnection connection = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                connection.Open();
                string query = "UPDATE reserved_books SET is_borrowed = true WHERE book_id = @id and borrower_id = @borrower_id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", book.BookId);
                    command.Parameters.AddWithValue("@borrower_id", borrowerId);
                    command.ExecuteScalar();
                }
            }
        }

        public static List<Book> ShowBorrowedBooks(int borrowerId)
        {
            List<Book> bookList = new List<Book>();
            using (MySqlConnection connection = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                connection.Open();
                string query = "SELECT books.* FROM borrowed_books JOIN books ON borrowed_books.book_id = books.id WHERE borrowed_books.borrower_id = @Id;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", borrowerId);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string title = reader.GetString("title");
                            string genre = reader.GetString("genre");
                            string author = reader.GetString("author");
                            string isbn = reader.GetString("isbn");
                            string category = reader.GetString("category");
                            string copyright = reader.GetString("copyright");
                            string publisher = reader.GetString("publisher");
                            string status = reader.GetString("status");
                            int aisle = reader.GetInt32("aisle");
                            int shelf = reader.GetInt32("shelf");
                            string imagePath = reader.GetString("book_img_path");

                            Book book = new Book(id, title, genre, author, isbn, category, copyright,
                                publisher, status, aisle, shelf, imagePath);

                            bookList.Add(book);
                        }
                    }
                }
            }
            return bookList;
        }


        public static void AddBook(string author, string genre, string isbn, string category, string title, string copyright,
            string publisher, int aisle, int shelf, string bookImage = "NO_IMAGE", string status = "AVAILABLE")
        {
            using (MySqlConnection connection = new MySqlConnection(Archivary.BACKEND.DATABASE.DatabaseConnection.ConnectionDetails()))
            {
                try
                {
                    //Open a connection to database
                    connection.Open();
                    Console.WriteLine("Connected to the database.");

                    //Prepare query 
                    string insertQuery = "INSERT INTO books(" +
                        "author, genre, isbn, category, title, copyright, publisher, status, aisle, shelf, book_img_path) " +
                        "VALUES(@author, @genre, @isbn, @category, @title, @copyright, @publisher, @status, @aisle, @shelf, @book_img_path)";



                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        //Add parameters to the command
                        command.Parameters.AddWithValue("@author", author);
                        command.Parameters.AddWithValue("@genre", genre);
                        command.Parameters.AddWithValue("@isbn", isbn);
                        command.Parameters.AddWithValue("@category", category);
                        command.Parameters.AddWithValue("@title", title);
                        command.Parameters.AddWithValue("@copyright", copyright);
                        command.Parameters.AddWithValue("@publisher", publisher);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@aisle", aisle);
                        command.Parameters.AddWithValue("@shelf", shelf);
                        command.Parameters.AddWithValue("@book_img_path", bookImage);

                        //Execute the SQL command
                        int rowsAffected = command.ExecuteNonQuery();

                        //Check if operation was successful
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"Operation successful. {rowsAffected} rows affected.");
                        }
                        else
                        {
                            Console.WriteLine("No rows affected. Something might be wrong in inserting employees.");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally { connection.Close(); }
            }
        }
        public static List<string> AddBookByExcel(string fileLocation, string workSheetLocation, int startRow)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; //Indicates to the dependency that this is for non-commercial use

            List<string> errors = new List<string>();

            using (var package = new ExcelPackage(new FileInfo(fileLocation)))
            {
                if (package != null)
                {
                    var worksheet = package.Workbook.Worksheets[workSheetLocation];

                    //Check if the worksheet exists
                    if (worksheet != null)
                    {
                        // Get the actual row count with data
                        int actualRowCount = 0;
                        for (int row = startRow - 1; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var rowHasData = worksheet.Cells[row, worksheet.Dimension.Start.Column, row, worksheet.Dimension.End.Column]
                                .Any(cell => cell.Value != null);

                            if (rowHasData)
                            {
                                actualRowCount++;
                            }
                        }

                        if (actualRowCount == 0 || actualRowCount == 1)
                        {
                            errors.Add("There are no information inside the excell file");
                            return errors;
                        }

                        int actualColumnCount = 0;
                        for (int col = worksheet.Dimension.Start.Column; col <= worksheet.Dimension.End.Column; col++)
                        {
                            var columnHasData = worksheet.Cells
                                .Any(cell => cell.Start.Column == col && cell.Value != null);

                            if (columnHasData)
                            {
                                actualColumnCount++;
                            }
                        }

                        // Check if the number of columns is as expected
                        if (actualColumnCount != 10)
                        {
                            errors.Add($"Expected {10} columns, but found {actualColumnCount} columns.");
                            return errors;
                        }

                        for (int row = 1; row <= (startRow - 2) + actualRowCount; row++)
                        {
                            if (row < startRow) continue; //skips the rows above the header
                            string[] colValues = new string[10]; //Store info here
                            //Iterate through columns
                            for (int col = worksheet.Dimension.Start.Column; col <= actualColumnCount; col++)
                            {
                                //Take cell info of value
                                var cellValue = worksheet.Cells[row, col].Value;

                                //Check if the cell value is int before inserting in the array
                                if (cellValue != null)
                                {
                                    // Check if the cell value is of type double
                                    if (cellValue is double doubleValue)
                                    {
                                        // Handle double value (convert to string or format as needed)
                                        colValues[col - 1] = doubleValue.ToString();
                                    }
                                    else
                                    {
                                        // Handle other types (string, date, etc.)
                                        colValues[col - 1] = cellValue.ToString().Trim();
                                    }
                                }
                                else
                                {
                                    // Handle null cell value if needed
                                    colValues[col - 1] = string.Empty;
                                }
                            }
                            //If start row is greater than 1, check the header to identify which info is in the column
                            //If start row is equal to 1 it will identify the information by default order
                            if (startRow > 1)
                            {
                                IdentificationResult result = IdentifyBookColumnInfoSequence(worksheet, startRow);
                                if (!result.Success)
                                {
                                    errors.Add(result.ErrorMessage);
                                    return errors;
                                }
                                //Check if header is appropriate and describes the required book info
                                if (result.Success)
                                {
                                    //Sort info
                                    SortInfoBaseOnSequence(result.Sequence, colValues);
                                }
                                //Check if all infos provided is valid
                            }
                            var validateResult = CheckBookInfoFromExcel(colValues);
                            if (validateResult.isValid)
                            {
                                if (string.IsNullOrEmpty(colValues[(int)BookInfo.ImagePath]) ||
                                    colValues[(int)BookInfo.ImagePath].ToUpper().Trim() == "NO_IMAGE" ||
                                    colValues[(int)BookInfo.ImagePath].ToUpper().Trim() == "NO IMAGE" ||
                                    colValues[(int)BookInfo.ImagePath].ToUpper().Trim() == "NO-IMAGE" ||
                                    colValues[(int)BookInfo.ImagePath].ToUpper().Trim() == "NOIMAGE" ||
                                    colValues[(int)BookInfo.ImagePath].ToUpper().Trim() == "N/A")
                                {
                                    AddBook(
                                        colValues[(int)BookInfo.Author],
                                        colValues[(int)BookInfo.Genre],
                                        colValues[(int)BookInfo.ISBN],
                                        colValues[(int)BookInfo.Category],
                                        colValues[(int)BookInfo.Title],
                                        colValues[(int)BookInfo.Copyright],
                                        colValues[(int)BookInfo.Publisher],
                                        int.Parse(colValues[(int)BookInfo.Aisle]),
                                        int.Parse(colValues[(int)BookInfo.Shelf])
                                        );
                                }
                                else
                                {
                                    AddBook(
                                        colValues[(int)BookInfo.Author],
                                        colValues[(int)BookInfo.Genre],
                                        colValues[(int)BookInfo.ISBN],
                                        colValues[(int)BookInfo.Category],
                                        colValues[(int)BookInfo.Title],
                                        colValues[(int)BookInfo.Copyright],
                                        colValues[(int)BookInfo.Publisher],
                                        int.Parse(colValues[(int)BookInfo.Aisle]),
                                        int.Parse(colValues[(int)BookInfo.Shelf]),
                                        colValues[(int)BookInfo.ImagePath]
                                        );
                                }
                            }
                            else
                            {
                                errors.Add($"Row Number: {row} {validateResult.errorMessage}");
                            }
                        }

                    }
                    else
                    {
                        errors.Add($"Worksheet {workSheetLocation} does not exist");
                    }
                }
                else
                {
                    errors.Add($"File: {fileLocation} does not exist");
                }
            }
            return errors;
        }

        private static IdentificationResult IdentifyBookColumnInfoSequence(ExcelWorksheet worksheet, int startRow)
        {
            //Check if the start row is greater than 1
            //if start row is equal to 1 it means that there are no header because worksheet is 1 indexed
            if (startRow < 2) return IdentificationResult.CreateError("Start Row is Less Than 2");

            int headerLocation = startRow - 1; //move one cell up assuming that the header is directly above the first row

            //Initialize array
            int[] sequence = new int[10];

            // Track encountered headers
            HashSet<string> encounteredHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //Loop to fill the array
            for (int row = headerLocation; row == headerLocation; row++)
            {
                for (int col = worksheet.Dimension.Start.Column; col <= sequence.Length; col++)
                {
                    //Take cell value
                    var cellValue = worksheet.Cells[headerLocation, col].Value;

                    //Check first if cell is null
                    if (cellValue != null)
                    {
                        // Check if the cells are all string
                        if (cellValue is double doubleValue || cellValue is int intValue || cellValue is float floatValue)
                        {
                            return IdentificationResult.CreateError("Cell Value is a type of number or decimal");
                        }

                        // Check if the header is unique
                        string headerName = cellValue.ToString().Trim().ToUpper();
                        if (!encounteredHeaders.Add(headerName))
                        {
                            return IdentificationResult.CreateError($"Duplicate header found '{headerName}' in column {col}");
                        }

                        //Fill the array with values based from cell header name
                        if (cellValue.ToString().Trim().ToUpper() == "ISBN")
                        {
                            sequence[col - 1] = (int)BookInfo.ISBN;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "TITLE")
                        {
                            sequence[col - 1] = (int)BookInfo.Title;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "CATEGORY")
                        {
                            sequence[col - 1] = (int)BookInfo.Category;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "GENRE")
                        {
                            sequence[col - 1] = (int)BookInfo.Genre;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "AUTHOR" ||
                            cellValue.ToString().Trim().ToUpper() == "AUTHOR NAME" ||
                            cellValue.ToString().Trim().ToUpper() == "AUTHOR_NAME" ||
                            cellValue.ToString().Trim().ToUpper() == "AUTHOR-NAME")
                        {
                            sequence[col - 1] = (int)BookInfo.Author;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "PUBLISHER")
                        {
                            sequence[col - 1] = (int)BookInfo.Publisher;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "COPYRIGHT")
                        {
                            sequence[col - 1] = (int)BookInfo.Copyright;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "AISLE")
                        {
                            sequence[col - 1] = (int)BookInfo.Aisle;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "SHELF")
                        {
                            sequence[col - 1] = (int)BookInfo.Shelf;
                        }
                        else if (cellValue.ToString().Trim().ToUpper() == "IMAGE PATH" ||
                                cellValue.ToString().Trim().ToUpper() == "IMAGE_PATH" ||
                                cellValue.ToString().Trim().ToUpper() == "IMAGE-PATH" ||
                                cellValue.ToString().Trim().ToUpper() == "IMAGEPATH" ||
                                cellValue.ToString().Trim().ToUpper() == "IMAGE")
                        {
                            sequence[col - 1] = (int)BookInfo.ImagePath;
                        }
                        else return IdentificationResult.CreateError($"Column {headerName} in your header is not in the right format or does not correctly identify the column");
                    }
                    else return IdentificationResult.CreateError($"Column {col} in your header is empty");
                }
            }
            return IdentificationResult.CreateSuccess(sequence);
        }
        public static (bool isValid, string errorMessage) CheckBookInfoFromExcel(string[] bookInfos)
        {
            //Checks isbn
            if (!IsValidIsbn(bookInfos[(int)BookInfo.ISBN]))
            {
                return (false, "Invalid ISBN");
            }
            //Checks aisle and shelf 
            if (!IsValidInteger(bookInfos[(int)BookInfo.Aisle]))
            {
                return (false, "Invalid aisle location");
            }
            if (!IsValidInteger(bookInfos[(int)BookInfo.Shelf]))
            {
                return (false, "Invalid shelf location");
            }
            if (!IsValidCategory(bookInfos[(int)(BookInfo.Category)]))
            {
                return (false, "Invalid Category");
            }
            if (string.IsNullOrEmpty(bookInfos[(int)BookInfo.ImagePath]) ||
                !(bookInfos[(int)BookInfo.ImagePath].ToUpper().Trim() == "NO_IMAGE" ||
                bookInfos[(int)BookInfo.ImagePath].ToUpper().Trim() == "NO IMAGE" ||
                bookInfos[(int)BookInfo.ImagePath].ToUpper().Trim() == "NO-IMAGE" ||
                bookInfos[(int)BookInfo.ImagePath].ToUpper().Trim() == "NOIMAGE" ||
                bookInfos[(int)BookInfo.ImagePath].ToUpper().Trim() == "N/A"))
            {
                if (!DoesFileExistAndIsImage(bookInfos[(int)BookInfo.ImagePath]))
                {
                    return (false, "Invalid image file or file does not exist");
                }
            }
            string[] headerNames = { "ISBN", "Title", "Category", "Genre",
                "Author", "Publisher", "Copyright", "Aisle", "Shelf", "IMAGE PATH" };

            for (int i = 0; i < bookInfos.Length; i++)
            {
                //skip iteration
                if (i == (int)BookInfo.ISBN || i == (int)BookInfo.Aisle || i == (int)BookInfo.Shelf || i == (int)BookInfo.Category) continue;
                //Checks other infos
                if (bookInfos[i] == null || string.IsNullOrEmpty(bookInfos[i]))
                {
                    return (false, $"Entered Book info is null in column {headerNames[i]}");
                }
            }
            return (true, null); // No errors
        }
        public static bool IsValidIsbn(string input)
        {
            return !string.IsNullOrEmpty(input) &&
                   input.All(char.IsDigit) &&
                   (input.Length == 10 || input.Length == 13);
        }
        public static bool IsValidInteger(string input)
        {
            // Try to parse the input as an integer, and check if it's greater than zero
            if (int.TryParse(input, out int parsedValue))
            {
                return parsedValue > 0;
            }

            return false;
        }
        public static bool IsValidStatus(string input)
        {
            if (!string.IsNullOrEmpty(input) &&
                (input.ToUpper().Trim() == "AVAILABLE" ||
                input.ToUpper().Trim() == "UNAVAILABLE" ||
                input.ToUpper().Trim() == "RESERVED"))
            {
                return true;
            }
            return false;
        }
        public static bool IsValidCategory(string input)
        {
            if (!string.IsNullOrEmpty(input) &&
                (input.ToUpper().Trim() == "FICTION" ||
                input.ToUpper().Trim() == "NON-FICTION" ||
                input.ToUpper().Trim() == "NON FICTION" ||
                input.ToUpper().Trim() == "NON_FICTION" ||
                input.ToUpper().Trim() == "NONFICTION" ||
                input.ToUpper().Trim() == "ACADEMIC"))
            {
                return true;
            }
            return false;
        }

    }
}

