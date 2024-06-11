using MeowStore.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace MeowStore.Views
{
    public partial class OperatorWin : Window
    {
        private ProductViewModel _viewModel; // ViewModel для взаимодействия с данными продуктов
        private long _customerId; // ID покупателя, для которого открыто окно
        private long _basketId; // ID корзины, связанной с покупателем

        public OperatorWin(long customerId)
        {
            InitializeComponent();

            _customerId = customerId;
            _viewModel = new ProductViewModel();

            // Получение idBasket для данного customerId
            _basketId = _viewModel.GetBasketId(_customerId);
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

        // Обработчик события нажатия на кнопку добавления в корзину
        private void AddToBasket_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранного продукта
            var selectedProduct = (Product)ProductsDataGrid.SelectedItem;

            if (selectedProduct != null)
            {
                try
                {
                    // Добавление продукта в корзину покупателя
                    _viewModel.AddProductToBasket(selectedProduct.IdProduct, 1, selectedProduct.Price, _customerId);

                    // Обновление списка продуктов после добавления в корзину
                    RefreshProductsList();
                }
                catch (InvalidOperationException ex)
                {
                    // Обработка исключений при добавлении в корзину
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Метод для обновления списка продуктов
        private void RefreshProductsList()
        {
            // Поиск продуктов с текущей подстрокой поиска
            ObservableCollection<Product> updatedProducts = ProductViewModel.FindProductsBySubstring(SearchTextBox.Text);

            // Установка обновлённых продуктов в качестве источника данных для DataGrid
            ProductsDataGrid.ItemsSource = updatedProducts;
            ProductsDataGrid.Items.Refresh();
        }

        // Обработчик события нажатия на кнопку перехода к следующей странице
        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна OrderWin с передачей идентификатора корзины
            var orderWin = new OrderWin(_basketId, _customerId);
            orderWin.Show();
            Close();
        }
    }
}