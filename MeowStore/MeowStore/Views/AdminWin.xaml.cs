using MeowStore.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MeowStore.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminWin.xaml
    /// </summary>
    public partial class AdminWin : Window
    {
        public AdminWin()
        {
            InitializeComponent();
        }

        // Обработчик события нажатия на кнопку поиска
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Получение подстроки из текстового поля
            string substring = SearchTextBox.Text;

            // Поиск продуктов по подстроке
            ObservableCollection<Product> foundProducts = ProductViewModel.FindProductsBySubstring(substring);

            if (foundProducts.Count == 0) // Если коллекция пуста
            {
                MessageBox.Show("По вашему запросу ничего не найдено."); // Отображение сообщения
            }
            else
            {
                // Установка найденных продуктов в качестве источника данных для ListView
                ProductsDataGrid.ItemsSource = foundProducts;
            }
        }

        // Обработчик события завершения редактирования ячейки в таблице данных
        private void ProductsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Получение редактируемого продукта
            var product = (Product)e.Row.Item;

            // Если редактируемая ячейка - это ячейка "Описание"
            if (e.Column.Header.ToString() == "Описание")
            {
                // Получение нового описания
                var newDescription = ((TextBox)e.EditingElement).Text;

                // Обновление описания в базе данных
                ((ProductViewModel)DataContext).UpdateProductDescription(product, newDescription);
            }
            // Если редактируемая ячейка - это ячейка "Цена"
            else if (e.Column.Header.ToString() == "Цена")
            {
                // Получение новой цены
                var newPrice = Double.Parse(((TextBox)e.EditingElement).Text);

                // Обновление цены в базе данных
                ((ProductViewModel)DataContext).UpdateProductPrice(product, newPrice);
            }
            // Если редактируемая ячейка - это ячейка "Количество товара на складе"
            else if (e.Column.Header.ToString() == "Количество товара на складе")
            {
                // Получение нового количества
                var newCount = Int32.Parse(((TextBox)e.EditingElement).Text);

                // Обновление количества в базе данных
                ((ProductViewModel)DataContext).UpdateProductCount(product, newCount);
            }
        }

        // Обработчик события нажатия на кнопку удаления продукта
        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение выбранного продукта
                var selectedProduct = (Product)ProductsDataGrid.SelectedItem;

                if (selectedProduct != null)
                {
                    // Удаление продукта из базы данных
                    ((ProductViewModel)DataContext).DeleteProduct(selectedProduct);
                }
            }
            catch
            {
                MessageBox.Show("Нельзя удалить товар, так как он находится в заказе у покупателя");
            }
        }

        // Обработчик события нажатия на кнопку добавления пользователя
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна для добавления новых пользователей
            AddUsersWin addUsersWin = new AddUsersWin();
            addUsersWin.Show();
        }

        // Обработчик события нажатия на кнопку добавления продукта
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Создание нового окна AddProductsWin и передача текущего ViewModel в его конструктор
            var addProductsWin = new AddProductsWin((ProductViewModel)DataContext);
            addProductsWin.Show();
        }
    }
}
