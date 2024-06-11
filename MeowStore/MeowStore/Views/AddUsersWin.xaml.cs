using System.Windows;
using System.Windows.Controls;

namespace MeowStore.Views
{
    /// <summary>
    /// Логика взаимодействия для AddUsersWin.xaml
    /// </summary>
    public partial class AddUsersWin : Window
    {
        private UserViewModel _viewModel;

        public AddUsersWin()
        {
            InitializeComponent();

            // Создание нового экземпляра UserViewModel
            _viewModel = new UserViewModel();

            // Загрузка ролей из базы данных
            RoleComboBox.ItemsSource = _viewModel.LoadRoles();
        }

        // Обработчик события нажатия на кнопку добавления пользователя
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных из полей ввода
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;
            var role = (Role)RoleComboBox.SelectedItem;

            if (role != null)
            {
                // Добавление нового пользователя
                _viewModel.AddUser(username, password, role);

                // Вывод сообщения об успешном добавлении пользователя и закрытие окна
                MessageBox.Show("Пользователь добавлен");
                Close();
            }
            else
            {
                MessageBox.Show("Выберите роль пользователя");
            }
        }
    }
}
