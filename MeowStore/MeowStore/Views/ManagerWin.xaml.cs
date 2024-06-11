using MeowStore.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MeowStore.Views
{
    /// <summary>
    /// Логика взаимодействия для ManagerWin.xaml
    /// </summary>
    public partial class ManagerWin : Window
    {
        private ProductViewModel _viewModel;
        public ManagerWin()
        {
            InitializeComponent();
            _viewModel = new ProductViewModel();

            // Загрузка всех заказов
            OrdersDataGrid.ItemsSource = _viewModel.GetAllBaskets();
        }

        // Обработчик события нажатия на элемент управления для установки статуса заказа
        private void SetStatus_Click(object sender, RoutedEventArgs e)
        {
            // Получение выбранной корзины
            var selectedBasket = (Basket)OrdersDataGrid.SelectedItem;

            if (selectedBasket != null)
            {
                // Получение выбранного пункта меню
                var menuItem = e.OriginalSource as MenuItem;

                // Получение нового статуса
                var newStatusName = menuItem.Header.ToString();

                // Обновление статуса в базе данных
                _viewModel.UpdateBasketStatus(selectedBasket, newStatusName);

                // Перезагрузка данных для OrdersDataGrid
                OrdersDataGrid.ItemsSource = _viewModel.GetAllBaskets();
                OrdersDataGrid.Items.Refresh();
            }
        }

        // Обработчик события нажатия на кнопку генерации отчета о продажах
        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызов метода генерации отчета из ViewModel
            _viewModel.GenerateSalesReport();
        }

    }
}
