using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace MeowStore.ViewModels
{
    public class ProductViewModel
    {
        // Хранит коллекцию продуктов, загруженных из базы данных
        private ObservableCollection<Product> _products;

        // Свойство для доступа к коллекции продуктов
        public ObservableCollection<Product> Products
        {
            get
            {
                // Если коллекция еще не создана, загружаем продукты из базы данных
                if (_products == null)
                {
                    _products = new ObservableCollection<Product>();
                    LoadProducts(); // Загрузка продуктов
                }
                return _products; // Возвращаем коллекцию продуктов
            }
        }

        // Метод для загрузки продуктов из базы данных
        private void LoadProducts()
        {
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT Products.*, Categories.NameCategory 
                FROM Products 
                INNER JOIN Categories ON Products.IdCategory = Categories.IdCategory";

                // Читаем данные и заполняем коллекцию _products
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            IdProduct = reader.GetInt32(0),
                            IdCategory = reader.GetInt32(1),
                            Category = new Category { NameCategory = reader.GetString(7) },
                            NameProduct = reader.GetString(2),
                            Description = reader.GetString(3),
                            Price = reader.GetDouble(4),
                            Count = reader.GetInt32(5),
                            PathImage = reader.GetString(6)
                        };

                        _products.Add(product); // Добавляем продукт в коллекцию
                    }
                }

                // Вычисляем сумму для каждого продукта в корзине
                foreach (var product in _products)
                {
                    command.Parameters.Clear();

                    // Запрос для получения количества продукта в корзине
                    command.CommandText = @"
                    SELECT Count FROM ProdBasket 
                    WHERE idProduct = @productId";
                    command.Parameters.AddWithValue("@productId", product.IdProduct);

                    // Выполняем запрос и вычисляем сумму
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Количество продукта в корзине
                            var count = reader.GetInt32(0);

                            // Вычисляем сумму
                            product.Sum = count * product.Price;
                        }
                    }
                }
            }
        }

        // Метод для поиска продуктов по подстроке в базе данных
        public static ObservableCollection<Product> FindProductsBySubstring(string substring)
        {
            // Создание коллекции для хранения найденных продуктов
            ObservableCollection<Product> foundProducts = new ObservableCollection<Product>();

            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для поиска продуктов по подстроке
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT Products.*, Categories.NameCategory 
                FROM Products 
                INNER JOIN Categories ON Products.IdCategory = Categories.IdCategory
                WHERE LOWER(NameProduct) LIKE @substring OR LOWER(Description) LIKE @substring OR LOWER(Categories.NameCategory) LIKE @substring OR Price LIKE @substring OR Count LIKE @substring";

                // Добавление параметра в команду SQL
                command.Parameters.AddWithValue("@substring", "%" + substring.ToLower() + "%");

                // Выполнение команды и обработка результатов
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создание объекта Product и заполнение его данными из базы данных
                        var product = new Product
                        {
                            IdProduct = reader.GetInt32(0),
                            IdCategory = reader.GetInt32(1),
                            Category = new Category { NameCategory = reader.GetString(7) },
                            NameProduct = reader.GetString(2),
                            Description = reader.GetString(3),
                            Price = reader.GetDouble(4),
                            Count = reader.GetInt32(5),
                            PathImage = reader.GetString(6)
                        };

                        // Добавление продукта в коллекцию найденных продуктов
                        foundProducts.Add(product);
                    }
                }
            }
            return foundProducts;
        }

        // Методы для обновления описания, цены и количества продукта в базе данных
        public void UpdateProductDescription(Product product, string newDescription)
        {
            UpdateProductField(product, "Description", newDescription);
            product.Description = newDescription; // Обновление описания в объекте Product
        }

        public void UpdateProductPrice(Product product, double newPrice)
        {
            UpdateProductField(product, "Price", newPrice.ToString());
            product.Price = newPrice; // Обновление цены в объекте Product
        }

        public void UpdateProductCount(Product product, int newCount)
        {
            UpdateProductField(product, "Count", newCount.ToString());
            product.Count = newCount; // Обновление количества в объекте Product
        }

        private void UpdateProductField(Product product, string fieldName, string newValue)
        {
            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для обновления поля продукта
                var command = connection.CreateCommand();
                command.CommandText = $@"
                UPDATE Products
                SET {fieldName} = @newValue
                WHERE IdProduct = @id";

                // Добавление параметров в команду SQL
                command.Parameters.AddWithValue("@newValue", newValue);
                command.Parameters.AddWithValue("@id", product.IdProduct);

                // Выполнение команды
                command.ExecuteNonQuery();
            }
        }

        // Метод для удаления продукта из базы данных
        public void DeleteProduct(Product product)
        {
            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для удаления продукта
                var command = connection.CreateCommand();
                command.CommandText = @"
                DELETE FROM Products
                WHERE IdProduct = @id";

                // Добавление параметра в команду SQL
                command.Parameters.AddWithValue("@id", product.IdProduct);

                // Выполнение команды
                command.ExecuteNonQuery();
            }

            // Удаление продукта из коллекции
            Products.Remove(product);
        }

        // Метод для добавления нового продукта
        public void AddProduct(string name, string description, double price, int count, string imagePath, Category category)
        {
            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для добавления нового продукта
                var command = connection.CreateCommand();
                command.CommandText = @"
                INSERT INTO Products (NameProduct, Description, Price, Count, PathImage, IdCategory)
                VALUES (@name, @description, @price, @count, @imagePath, @idCategory)";

                // Добавление параметров в команду SQL
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@count", count);
                command.Parameters.AddWithValue("@imagePath", imagePath);
                command.Parameters.AddWithValue("@idCategory", category.IdCategory);

                // Выполнение команды
                command.ExecuteNonQuery();
            }

            // Создание нового объекта Product и добавление его в коллекцию Products
            var product = new Product
            {
                NameProduct = name,
                Description = description,
                Price = price,
                Count = count,
                PathImage = imagePath,
                Category = category
            };
            Products.Add(product);
        }

        // Метод для загрузки категорий из базы данных
        public ObservableCollection<Category> LoadCategories()
        {
            var categories = new ObservableCollection<Category>();

            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для получения всех категорий
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Categories";

                // Выполнение команды и обработка результатов
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создание объекта Category и добавление его в коллекцию
                        var category = new Category
                        {
                            IdCategory = reader.GetInt32(0),
                            NameCategory = reader.GetString(1)
                        };

                        categories.Add(category);
                    }
                }
            }
            return categories;
        }

        // Метод для добавление товара в корзину покупателя
        public void AddProductToBasket(long productId, int count, double price, long customerId)
        {
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Проверка количества товара на складе
                var stockCheckCommand = connection.CreateCommand();
                stockCheckCommand.CommandText = @"
                SELECT Count FROM Products
                WHERE IdProduct = @productId";
                stockCheckCommand.Parameters.AddWithValue("@productId", productId);

                var stockCount = (long?)stockCheckCommand.ExecuteScalar();
                if (stockCount == null || stockCount < count)
                {
                    // Если товара нет на складе, прерываем выполнение метода
                    throw new InvalidOperationException("На складе недостаточно товара.");
                }

                // Получение idBasket для данного customerId
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT idBasket FROM Basket 
                WHERE idCustomer = @customerId";
                command.Parameters.AddWithValue("@customerId", customerId);
                var result = command.ExecuteScalar();

                // Здесь сохраняем полученный idBasket
                long basketId;

                if (result != null) // Если корзина уже существует
                {
                    basketId = (long)result;
                }
                else // Если корзины еще не существует
                {
                    // Создание новой корзины
                    command.CommandText = @"
                    INSERT INTO Basket (idCustomer)
                    VALUES (@customerId);
                    SELECT last_insert_rowid();";
                    basketId = (long)command.ExecuteScalar();
                }

                // Проверка, есть ли уже этот товар в корзине покупателя
                command.CommandText = @"
                SELECT Count FROM ProdBasket 
                WHERE idProduct = @productId AND idBasket = @basketId";
                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@basketId", basketId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read()) // Если товар уже в корзине
                    {
                        // Обновление количества и суммы товара в корзине
                        var oldCount = reader.GetInt32(0);
                        var newCount = oldCount + count;
                        var sum = newCount * price;
                        reader.Close(); // Закрытие reader перед выполнением новой команды
                        command.CommandText = @"
                        UPDATE ProdBasket SET Count = @newCount, Sum = @sum 
                        WHERE idProduct = @productId AND idBasket = @basketId";
                        command.Parameters.AddWithValue("@newCount", newCount);
                        command.Parameters.AddWithValue("@sum", sum);
                        command.ExecuteNonQuery();
                    }
                    else // Если товара еще нет в корзине
                    {
                        reader.Close(); // Закрытие reader перед выполнением новой команды
                        // Добавление нового товара в корзину
                        command.CommandText = @"
                        INSERT INTO ProdBasket (idProduct, Count, Sum, idBasket)
                        VALUES (@productId, @count, @sum, @basketId)";
                        command.Parameters.AddWithValue("@count", count);
                        command.Parameters.AddWithValue("@sum", price * count);
                        command.ExecuteNonQuery();
                    }
                }

                // Обновление количества товара на складе
                var updateProductCommand = connection.CreateCommand();
                updateProductCommand.CommandText = @"
                UPDATE Products
                SET Count = Count - @count
                WHERE IdProduct = @productId AND Count >= @count";

                updateProductCommand.Parameters.AddWithValue("@count", count);
                updateProductCommand.Parameters.AddWithValue("@productId", productId);

                // Если на складе недостаточно товара, не добавляем его в корзину
                int rowsAffected = updateProductCommand.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException("На складе недостаточно товара.");
                }
            }
        }

        // Метод для получения списка продуктов в корзине по ID корзины
        public List<Product> GetProductsInBasket(long basketId)
        {
            // Инициализация списка продуктов
            var productsInBasket = new List<Product>();

            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для выборки продуктов в корзине
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT Products.*, Categories.NameCategory, ProdBasket.Count, ProdBasket.Count * Products.Price AS Sum
                FROM ProdBasket
                INNER JOIN Products ON ProdBasket.idProduct = Products.idProduct
                INNER JOIN Categories ON Products.IdCategory = Categories.IdCategory
                WHERE ProdBasket.idBasket = @basketId";
                command.Parameters.AddWithValue("@basketId", basketId);

                // Выполнение команды и чтение результатов
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создание объекта Product и добавление его в список
                        var product = new Product
                        {
                            IdProduct = reader.GetInt32(0),
                            IdCategory = reader.GetInt32(1),
                            Category = new Category { NameCategory = reader.GetString(7) },
                            NameProduct = reader.GetString(2),
                            Description = reader.GetString(3),
                            Price = reader.GetDouble(4),
                            Count = reader.GetInt32(8), // Используем количество из ProdBasket
                            PathImage = reader.GetString(6),
                            Sum = reader.GetDouble(9)
                        };

                        productsInBasket.Add(product); // Добавление продукта в список
                    }
                }
            }
            return productsInBasket; // Возврат списка продуктов в корзине
        }

        // Метод для загрузки точек выдачи из базы данных
        public ObservableCollection<Point> GetPoints()
        {
            // Инициализация коллекции точек выдачи
            var points = new ObservableCollection<Point>();

            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для выборки всех точек выдачи
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Points";

                // Выполнение команды и чтение результатов
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создание объекта Point и заполнение его данными из базы данных
                        var point = new Point
                        {
                            IdPoint = reader.GetInt32(0),
                            AddressPoint = reader.GetString(1)
                        };

                        points.Add(point); // Добавление точки выдачи в коллекцию
                    }
                }
            }
            return points; // Возврат коллекции точек выдачи
        }

        // Метод для завершения формирования заказа
        public void CompleteOrder(long basketId, long pointId)
        {
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для обновления записи в таблице Basket
                var command = connection.CreateCommand();
                command.CommandText = @"
                UPDATE Basket
                SET idPoint = @pointId, idStatus = 1
                WHERE idBasket = @basketId";
                command.Parameters.AddWithValue("@pointId", pointId);
                command.Parameters.AddWithValue("@basketId", basketId);
                command.ExecuteNonQuery();
            }
        }

        // Метод для получения ID корзины по ID покупателя
        public long GetBasketId(long customerId)
        {
            long basketId; // Переменная для хранения ID корзины

            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для выборки ID корзины по ID покупателя
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT idBasket FROM Basket 
                WHERE idCustomer = @customerId";
                command.Parameters.AddWithValue("@customerId", customerId);
                basketId = (long)command.ExecuteScalar();
            }
            return basketId; // Возврат ID корзины
        }

        // Метод для удаления продукта из корзины или уменьшения его количества
        public void RemoveProductFromBasket(long basketId, long productId)
        {
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для получения количества продукта в корзине
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT Count FROM ProdBasket
                WHERE idBasket = @basketId AND idProduct = @productId";
                command.Parameters.AddWithValue("@basketId", basketId);
                command.Parameters.AddWithValue("@productId", productId);

                var result = command.ExecuteScalar();
                if (result != null)
                {
                    // Получение текущего количества продукта
                    var count = (long)result;
                    if (count > 1)
                    {
                        // Если количество больше 1, уменьшаем его на 1
                        command.CommandText = @"
                        UPDATE ProdBasket
                        SET Count = Count - 1
                        WHERE idBasket = @basketId AND idProduct = @productId";
                        command.ExecuteNonQuery();

                        // Увеличиваем количество товара на складе
                        UpdateProductCount(connection, productId, 1);
                    }
                    else
                    {
                        // Если количество равно 1, удаляем запись
                        command.CommandText = @"
                        DELETE FROM ProdBasket
                        WHERE idBasket = @basketId AND idProduct = @productId";
                        command.ExecuteNonQuery();

                        // Увеличиваем количество товара на складе
                        UpdateProductCount(connection, productId, 1);
                    }
                }
            }
        }

        // Метод для обновления количества продукта на складе
        private void UpdateProductCount(SqliteConnection connection, long productId, int count)
        {
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
            UPDATE Products
            SET Count = Count + @count
            WHERE IdProduct = @productId";

            updateCommand.Parameters.AddWithValue("@count", count);
            updateCommand.Parameters.AddWithValue("@productId", productId);

            updateCommand.ExecuteNonQuery();
        }

        // Метод для расчета общей стоимости товаров в корзине покупателя
        public double CalculateBasketSum(long customerId)
        {
            double sum = 0; // Инициализация переменной для хранения суммы

            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Получение всех продуктов в корзине для данного customerId
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT ProdBasket.Count, Products.Price 
                FROM ProdBasket 
                INNER JOIN Products ON ProdBasket.idProduct = Products.idProduct
                INNER JOIN Basket ON ProdBasket.idBasket = Basket.idBasket
                WHERE Basket.idCustomer = @customerId";
                command.Parameters.AddWithValue("@customerId", customerId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var count = reader.GetInt32(0);
                        var price = reader.GetDouble(1);

                        // Расчет и добавление стоимости продукта к общей сумме
                        sum += count * price;
                    }
                }
            }
            return sum; // Возврат рассчитанной суммы
        }

        // Метод для получения всех корзин из базы данных
        public ObservableCollection<Basket> GetAllBaskets()
        {
            // Инициализация коллекции для хранения корзин
            ObservableCollection<Basket> baskets = new ObservableCollection<Basket>();

            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для получения всех корзин
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT Basket.*, Customers.FullName, Points.AddressPoint, Status.NameStatus 
                FROM Basket
                INNER JOIN Customers ON Basket.IdCustomer = Customers.IdCustomer
                INNER JOIN Points ON Basket.IdPoint = Points.IdPoint
                INNER JOIN Status ON Basket.IdStatus = Status.IdStatus";

                // Выполнение команды и обработка результатов
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Создание объекта Basket и заполнение его данными из базы данных
                        var basket = new Basket
                        {
                            IdBasket = reader.GetInt32(0),
                            IdCustomer = reader.GetInt32(1),
                            IdStatus = reader.GetInt32(2),
                            IdPoint = reader.GetInt32(3),
                            Customer = new Customer { IdCustomer = reader.GetInt32(1), FullName = reader.GetString(4) },
                            Point = new Point { IdPoint = reader.GetInt32(3), AddressPoint = reader.GetString(5) },
                            Status = new Status { IdStatus = reader.GetInt32(2), NameStatus = reader.GetString(6) }
                        };

                        // Добавление корзины в коллекцию
                        baskets.Add(basket);
                    }
                }
            }
            return baskets; // Возврат коллекции корзин
        }

        // Метод для обновления статуса заказа в базе данных
        public void UpdateBasketStatus(Basket basket, string newStatusName)
        {
            long newStatusId; // Переменная для хранения ID нового статуса

            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для получения ID нового статуса
                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT idStatus 
                FROM Status 
                WHERE NameStatus = @name";

                // Добавление параметра в команду SQL
                command.Parameters.AddWithValue("@name", newStatusName);

                // Выполнение команды и получение результата
                newStatusId = (long)command.ExecuteScalar();
            }

            // Создание и открытие нового соединения с базой данных для обновления статуса корзины
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для обновления статуса корзины
                var command = connection.CreateCommand();
                command.CommandText = @"
                UPDATE Basket
                SET idStatus = @id
                WHERE idBasket = @idBasket";

                // Добавление параметров в команду SQL
                command.Parameters.AddWithValue("@id", newStatusId);
                command.Parameters.AddWithValue("@idBasket", basket.IdBasket);

                // Выполнение команды
                command.ExecuteNonQuery();
            }

            // Обновление статуса в объекте Basket
            basket.IdStatus = (int)newStatusId;
        }

        // Метод для генерации отчета о продажах
        public void GenerateSalesReport()
        {
            // Словарь для хранения количества проданных товаров
            Dictionary<string, int> productSales = new Dictionary<string, int>();

            // Создание и открытие соединения с базой данных
            using (var connection = new SqliteConnection("Data Source=meowstore.db"))
            {
                connection.Open();

                // Создание команды SQL для получения данных о продажах
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Products.NameProduct, SUM(ProdBasket.Count) as TotalSold 
                    FROM Products 
                    JOIN ProdBasket ON Products.idProduct = ProdBasket.idProduct 
                    GROUP BY Products.NameProduct";

                // Выполнение команды и обработка результатов
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Добавление информации о продажах в словарь
                        productSales.Add(reader.GetString(0), reader.GetInt32(1));
                    }
                }
            }

            // Путь к файлу отчета
            string reportPath = "sales_report.txt";

            // Запись отчета в текстовый файл
            using (StreamWriter file = new StreamWriter(reportPath))
            {
                foreach (var entry in productSales)
                {
                    file.WriteLine($"Товар: {entry.Key}, Продано: {entry.Value}");
                }
            }

            // Уведомление о том, что отчет сгенерирован
            MessageBox.Show($"Отчет о продажах сохранен в файл: {reportPath}", "Отчет сгенерирован", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
