using System.Windows;

namespace MeowStore.Views
{
    /// <summary>
    /// Логика взаимодействия для AddCustomerWin.xaml
    /// </summary>
    public partial class AddCustomerWin : Window
    {
        // ViewModel для взаимодействия с данными пользователей
        private UserViewModel _viewModel;

        public AddCustomerWin()
        {
            InitializeComponent();

            // Создание нового экземпляра UserViewModel
            _viewModel = new UserViewModel();
        }

        // Обработчик события нажатия на кнопку перехода к следующей странице
        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FullNameTextBox.Text) || string.IsNullOrEmpty(ContactTextBox.Text))
            {
                MessageBox.Show("Заполните все поля");
            }
            else
            {
                // Получение данных из полей ввода
                var fullName = FullNameTextBox.Text;
                var contact = ContactTextBox.Text;

                // Добавление нового покупателя и создание его корзины
                long customerId = _viewModel.AddCustomerAndCreateBasket(fullName, contact);

                // Вывод сообщения об успешном добавлении покупателя
                MessageBox.Show("Покупатель добавлен");

                // Открытие окна OperatorWin с передачей идентификатора покупателя
                OperatorWin operatorWin = new OperatorWin(customerId);
                operatorWin.Show();

                // Закрытие текущего окна
                Close();
            }
        }
    }
}
