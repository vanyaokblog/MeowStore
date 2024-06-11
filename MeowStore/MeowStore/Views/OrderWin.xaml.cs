using MeowStore.ViewModels;
using System;
using System.Windows;

namespace MeowStore.Views
{
    public partial class OrderWin : Window
    {
        private ProductViewModel _viewModel; // ViewModel для взаимодействия с данными продуктов
        private long _basketId; // ID корзины, с которой работает оператор
        private long _customerId; // ID покупателя

        public OrderWin(long basketId, long customerId)
        {
            InitializeComponent();

            _basketId = basketId;
            _customerId = customerId;
            _viewModel = new ProductViewModel();

            // Загрузка товаров в корзине и пунктов выдачи
            BasketDataGrid.ItemsSource = _viewModel.GetProductsInBasket(basketId);
            PointsComboBox.ItemsSource = _viewModel.GetPoints();

            UpdateBasketSum(); // Обновление суммы корзины
        }

        // Обработчик события нажатия на кнопку завершения заказа
        private void CompleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранного пункта выдачи
            var selectedPoint = PointsComboBox.SelectedItem as Point;

            if (selectedPoint != null)
            {
                // Проверка, есть ли товары в корзине
                var productsInBasket = _viewModel.GetProductsInBasket(_basketId);
                if (productsInBasket.Count > 0)
                {
                    // Завершение заказа
                    _viewModel.CompleteOrder(_basketId, selectedPoint.IdPoint);

                    // Вывод сообщения об успешном завершении заказа и закрытие окна
                    MessageBox.Show("Заказ успешно сформирован");
                    AddCustomerWin addCustomerWin = new AddCustomerWin();
                    addCustomerWin.Show();
                    Close();
                }
                else
                {
                    // Вывод сообщения о том, что корзина пуста
                    MessageBox.Show("Ваша корзина пуста. Добавьте товары в корзину перед оформлением заказа.");
                }
            }
            else
            {
                MessageBox.Show("Выберите пункт выдачи");
            }
        }

        // Метод для обновления общей суммы корзины
        private void UpdateBasketSum()
        {
            // Расчет общей суммы корзины для текущего покупателя
            var sum = _viewModel.CalculateBasketSum(_customerId);

            // Отображение общей суммы в интерфейсе
            BasketSumTextBlock.Text = $"Общая сумма: {sum} рублей";
        }

        // Обработчик события нажатия на кнопку удаления товара из корзины
        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранного товара из таблицы корзины
            var selectedProduct = (Product)BasketDataGrid.SelectedItem;
            if (selectedProduct != null)
            {
                // Удаление товара из корзины
                _viewModel.RemoveProductFromBasket(_basketId, selectedProduct.IdProduct);
                MessageBox.Show("Товар удален из корзины");

                // Обновляем список продуктов в корзине
                BasketDataGrid.ItemsSource = _viewModel.GetProductsInBasket(_basketId);
                BasketDataGrid.Items.Refresh();

                UpdateBasketSum();
            }
        }

        // Обработчик события нажатия на кнопку возврата к предыдущему окну
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Открытие окна оператора и закрытие текущего окна заказа
            var operatorWin = new OperatorWin(_customerId);
            operatorWin.Show();
            this.Close();
        }

    }
}
