using MeowStore.ViewModels;
using System.Windows;

namespace MeowStore.Views
{
    /// <summary>
    /// Логика взаимодействия для AddProductsWin.xaml
    /// </summary>
    public partial class AddProductsWin : Window
    {
        private ProductViewModel _viewModel;

        public AddProductsWin(ProductViewModel viewModel)
        {
            InitializeComponent();

            // Установка ViewModel для работы с данными
            _viewModel = viewModel;

            // Загрузка категорий из базы данных
            CategoryComboBox.ItemsSource = _viewModel.LoadCategories();
        }

        // Обработчик события нажатия на кнопку добавления продукта
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                // Получение данных из полей ввода
                var name = NameTextBox.Text;
                var description = DescriptionTextBox.Text;
                var price = double.Parse(PriceTextBox.Text);
                var count = int.Parse(CountTextBox.Text);
                var imagePath = PathTextBox.Text;
                var category = (Category)CategoryComboBox.SelectedItem;

                if (category != null) {
                    // Добавление нового продукта
                    _viewModel.AddProduct(name, description, price, count, imagePath, category);

                    // Вывод сообщения об успешном добавлении продукта и закрытие окна
                    MessageBox.Show("Товар добавлен");
                    Close();
                }
                else {
                    MessageBox.Show("Выберите категорию товара");
                }
            }
            catch {
                MessageBox.Show("Введите корректные данные для сохранения");
            }
        }
    }
}
