using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace MeowStore.Views
{
    public class UserViewModel
    {
        // Метод для загрузки ролей пользователей из базы данных
        public ObservableCollection<Role> LoadRoles()
        {
            // Создает коллекцию для хранения объектов ролей
            var roles = new ObservableCollection<Role>();

            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для получения всех ролей
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Roles";

                // Читаем данные и заполняем коллекцию roles
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создание объекта Role и добавление его в ComboBox
                        var role = new Role
                        {
                            IdRole = reader.GetInt32(0),
                            NameRole = reader.GetString(1)
                        };

                        roles.Add(role); // Добавляем роль в коллекцию
                    }
                }
            }
            return roles; // Возвращаем заполненную коллекцию ролей
        }

        // Метод для добавления нового пользователя
        public void AddUser(string username, string password, Role role)
        {
            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для добавления нового пользователя
                var command = connection.CreateCommand();
                command.CommandText = @"
                INSERT INTO Users (Username, Password, IdRole)
                VALUES (@username, @password, @idRole)";

                // Добавление параметров в команду SQL
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@idRole", role.IdRole);

                // Выполнение команды
                command.ExecuteNonQuery();
            }
        }

        // Метод для создания нового покупателя и его корзины
        public long AddCustomerAndCreateBasket(string fullName, string contact)
        {
            // Идентификаторы для нового покупателя и его корзины
            long customerId;
            long basketId;

            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Добавление нового покупателя
                var command = connection.CreateCommand();
                command.CommandText = @"
                INSERT INTO Customers (FullName, Contact)
                VALUES (@fullName, @contact);
                SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@fullName", fullName);
                command.Parameters.AddWithValue("@contact", contact);
                customerId = (long)command.ExecuteScalar();

                // Создание новой корзины для покупателя
                command.CommandText = @"
                INSERT INTO Basket (idCustomer)
                VALUES (@customerId);
                SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("@customerId", customerId);
                basketId = (long)command.ExecuteScalar();
            }
            return basketId; // Возвращаем ID созданной корзины
        }
    }
}
