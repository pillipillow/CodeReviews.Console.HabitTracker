using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitLogger
{
    internal class Program
    {
        static string connectionString = "Data Source=habit-Tracker.db";
        static List<HabitEntry> habitEntries = new List<HabitEntry>();
        static List<HabitLogEntry> habitLogEntries = new List<HabitLogEntry>();
        static CultureInfo enUS = new CultureInfo("en-US");
        static Random random = new Random();

        static void Main(string[] args)
        {
            CreateTable();
            Menu();
        }

        // CREATE TABLE IF NOT EXISTS = will only create the table once if it's in the directory
        private static void CreateTable()
        {
            using (var connection = new SqliteConnection(connectionString)) { 
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS habits (
                                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Name TEXT,
                                        Unit TEXT)
                                        ";

                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS habit_log (
                                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Date TEXT,
                                        Quantity INTEGER,
                                        HabitID INTEGER,
                                        FOREIGN KEY (HabitID) REFERENCES habits (ID))";

                command.ExecuteNonQuery();

                // Seed habits
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT COUNT(*) FROM habits";
                int rowCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (rowCount == 0)
                {
                    command.CommandText = @"INSERT INTO habits (Name, Unit)
                                        VALUES 
                                            ('Sleep','Hours'),
                                            ('Water','Glasses'),
                                            ('Gym','Hours'),
                                            ('Meds','Pills'),
                                            ('Walk','Hours')";
                    command.ExecuteNonQuery();
                }

                checkCommand.CommandText = "SELECT COUNT(*) FROM habit_log";
                rowCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (rowCount == 0)
                {
                    // using transaction to speed up the filling
                    using (var transaction = connection.BeginTransaction())
                    { 
                        command.Transaction = transaction;

                        for (int i = 0; i < 100; i++)
                        {
                            int id = random.Next(1, 5);
                            string date = DateTime.Now.AddDays(-i).ToString("dd-MM-yy");
                            int quantity = random.Next(1, 5);

                            command.CommandText = $"INSERT INTO habit_log(Date, Quantity, HabitID) VALUES ('{date}',{quantity},{id})";
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();

                    }
                }
            }
        }

        private static void Menu()
        { 
            bool closeApp = false;

            while (!closeApp) 
            {
                Console.Clear();
                Console.WriteLine("-----WELCOME TO THE HABIT LOG-----");
                Console.WriteLine("1 - View all the habit log entries");
                Console.WriteLine("2 - Insert a new habit");
                Console.WriteLine("3 - Insert a new habit log entry");
                Console.WriteLine("4 - Update a habit log entry");
                Console.WriteLine("5 - Delete a habit log entry");
                Console.WriteLine("0 - Exit");
                Console.Write("What would you like to do: ");

                string input = Console.ReadLine();

                if(input != null)
                    input = input.Trim();

                switch (input)
                {
                    case "0":
                        closeApp = true;
                        break;
                    case "1":
                        Console.Clear();
                        Console.WriteLine("------------HABIT LOG-------------");
                        GetHabitLog();
                        Console.Write("\nPress enter to go back to the main menu");
                        Console.ReadLine();
                        break;
                    case "2":
                        InsertNewHabit();
                        break;
                    case "3":
                        InsertHabitLogEntry();
                        break;
                    case "4":
                        UpdateHabitLogEntry();
                        break; 
                    case "5":
                        DeleteHabitLogEnrty();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Command. Please type a number from 0 to 5");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private static void GetHabitLog()
        {
            habitLogEntries.Clear();
            using (var connection = new SqliteConnection(connectionString)) {

                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = @"SELECT
                                            habit_log.ID,
                                            habit_log.Date,
                                            habits.Name,
                                            habit_log.Quantity,
                                            habits.Unit
                                        FROM habit_log JOIN habits ON habit_log.HabitID = habits.ID";

                SqliteDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        HabitLogEntry log = new HabitLogEntry();

                        log.id = Convert.ToInt32(reader["ID"]);
                        log.dateTime = DateTime.ParseExact(reader["Date"].ToString(), "dd-MM-yy", enUS);
                        log.habitName = reader["Name"].ToString();
                        log.quantity = Convert.ToInt32(reader["Quantity"]);
                        log.unit = reader["Unit"].ToString();

                        habitLogEntries.Add(log);
                    }
                }
            }

            Console.WriteLine("ID\tDate\t\tHabit Type\tQuantity\tMeasurement");
            if (habitLogEntries.Count <= 0)
            {
                Console.WriteLine("No log entries found");
            }
            else 
            {
                foreach (var log in habitLogEntries)
                {
                    Console.WriteLine($"{log.id}\t{log.dateTime.ToString("dd-MM-yyyy")}\t{log.habitName}\t\t{log.quantity}\t\t{log.unit}");
                }
            }
        }


        private static void InsertNewHabit()
        {
            Console.Clear();
            Console.WriteLine("--------INSERT A NEW HABIT--------");

            Console.WriteLine("Please insert your habit name (Press 0 to return to main menu): ");
            string name = Console.ReadLine();

            if (name == "0")
            {
                return;
            }

            Console.WriteLine("\nPlease insert your unit of measurement (ie. hours, glasses etc.)");
            string unit = Console.ReadLine();

            using (var connection = new SqliteConnection(connectionString)) { 
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = $"INSERT INTO habits (Name, Unit) VALUES ('{name}','{unit}')";
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"\nNew habit {name} has been recorded!");
            Console.ReadLine();
        }


        private static void InsertHabitLogEntry()
        {
            Console.Clear();
            Console.WriteLine("---INSERT A NEW HABIT LOG ENTRY---");

            GetHabits();
            if (habitEntries.Count <= 0)
            {
                Console.WriteLine("\nPlease insert at least one habit! You can do that at '2' in the main menu");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("\nID\tHabit Type\tMeasurement Unit");
                foreach (var entry in habitEntries)
                {
                    Console.WriteLine($"{entry.id}\t{entry.name}\t\t{entry.unit}");
                }

                Console.WriteLine("\nChoose which habit from the table above by insert their ID number (Press 0 to return to main menu): ");
                int id = CheckIfInputIsNumber();

                if (id == 0)
                {
                    return;
                }

                using (var connection = new SqliteConnection(connectionString)){
                    connection.Open();

                    var checkCommand = connection.CreateCommand();
                    checkCommand.CommandText = $"SELECT EXISTS(SELECT 1 FROM habits WHERE ID = {id})";
                    int checkQuery = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (checkQuery == 0)
                    {
                        Console.WriteLine($"Habit with ID {id} doesn't exist");
                        Console.ReadLine();
                        connection.Close();
                        InsertHabitLogEntry();
                    }
                    else
                    {
                        Console.WriteLine("\nPlease set the record date (dd-mm-yy) or type T for today's date: ");
                        string date = CheckIfDateInput();
                        Console.WriteLine("\nPlease insert the approximate quantity that you have done with your chosen habit");
                        int quantity = CheckIfInputIsNumber();

                        var command = connection.CreateCommand();
                        command.CommandText = $"INSERT INTO habit_log(Date, Quantity, HabitID) VALUES ('{date}',{quantity},{id})";
                        command.ExecuteNonQuery();

                        Console.WriteLine("\nHabit entry has been logged!");
                        Console.ReadLine();

                    }
                }
            }
        }

        private static void GetHabits()
        {
            habitEntries.Clear();
            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM habits";

                SqliteDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    { 
                        HabitEntry entry = new HabitEntry();

                        entry.id = Convert.ToInt32(reader["ID"]);
                        entry.name = reader["Name"].ToString();
                        entry.unit = reader["Unit"].ToString();

                       habitEntries.Add(entry);
                    }
                }
            }
        
        }

        private static void UpdateHabitLogEntry()
        {
            Console.Clear();
            Console.WriteLine("-----UPDATE A HABIT LOG ENTRY-----");

            GetHabitLog();

            if (habitLogEntries.Count <= 0)
            {
                Console.WriteLine("\nNo log entries to update");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nPlease insert the entry ID you want to update (Press 0 to return to the main menu)");
            int id = CheckIfInputIsNumber();

            if (id == 0)
            {
                return;
            }

            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();

                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = $"SELECT EXISTS(SELECT 1 FROM habit_log WHERE ID = {id})";
                int checkQuery = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.Write($"\nLog entry with ID {id} doesn't exists");
                    Console.ReadLine();
                    connection.Close();
                    UpdateHabitLogEntry();
                }
                else
                {
                    Console.WriteLine("\nPlease set the record date (dd-mm-yy) or type T for today's date: ");
                    string date = CheckIfDateInput();
                    Console.WriteLine("\nPlease insert the approximate quantity that you have done with your chosen habit");
                    int quantity = CheckIfInputIsNumber();

                    var command = connection.CreateCommand();
                    command.CommandText = $"UPDATE habit_log SET Date = '{date}', Quantity = {quantity} WHERE ID = {id}";

                    command.ExecuteNonQuery();

                    Console.WriteLine($"\nLog entry with ID {id} has been updated");
                    Console.ReadLine();
                }
            }

        }

        private static void DeleteHabitLogEnrty()
        {
            Console.Clear();
            Console.WriteLine("-----DELETE A HABIT LOG ENTRY-----");

            GetHabitLog();

            if (habitLogEntries.Count <= 0)
            {
                Console.WriteLine("\nNo log entries to delete");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nPlease insert the entry ID you want to delete (Press 0 to return to the main menu)");
            int id = CheckIfInputIsNumber();

            if (id == 0)
            {
                return;
            }

            using (var connection = new SqliteConnection(connectionString)) {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"DELETE from habit_log WHERE ID = {id}";
                int rowCount = command.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.Write($"\nLog entry with ID {id} doesn't exists");
                    Console.ReadLine();
                    connection.Close();
                    DeleteHabitLogEnrty();
                }
                else
                {
                    Console.WriteLine($"\nLog entry with ID {id} has been deleted");
                    Console.ReadLine();
                }
            }
        }

        private static int CheckIfInputIsNumber()
        {
            int number = 0;
            bool isValid = false;

            do 
            {
                var input = Console.ReadLine();
                input = input.Trim().ToLower();
                isValid = int.TryParse(input, out number);

                if (!isValid)
                    Console.WriteLine("\nPlease input a number!");


            } while(!isValid);

            return number;
        }

        private static string CheckIfDateInput()
        {
            string date = "";
            bool isValid = false;

            do
            {
                var input = Console.ReadLine();
                input = input.Trim().ToLower();

                if (input == "0")
                {
                    date = input;
                    isValid = true;
                }

                if (input == "t")
                { 
                    DateTime now = DateTime.Now;
                    input = now.Date.ToString("dd-MM-yy");
                }

                isValid = DateTime.TryParseExact(input, "dd-MM-yy", enUS, DateTimeStyles.None, out _);

                if (!isValid)
                    Console.WriteLine("Please input the right date format!");
                else
                    date = input;


            } while (!isValid);

            return date;
        }

    }

    public class HabitEntry
    {
        public int id { get; set; }
        public string name { get; set; }
        public string unit { get; set; }
    }

    public class HabitLogEntry
    {
        public int id { get; set; }
        public DateTime dateTime { get; set; }
        public string habitName { get; set; }
        public int quantity { get; set; }
        public string unit { get; set; }
    }
}
