# Информационная система “Система управления интернет-магазином”.
### Название приложения: Meow Store
### Описание приложения:
**Система «Meow Store» – это приложение для управления интернет-магазином, разработанное на WPF, которое позволяет вносить данные о товарах, клиентах, создавать заказы, проводить поиск товаров, отслеживать статус заказов и генерировать отчеты.**

# Функции, реализованные в программе:
1. **Просмотр товаров.** На главном экране представлен список всех товаров. Пользователь может увидеть категорию, наименование, описание товара, его цену и количество на складе.
2. **Добавление новых товаров.** Администратор может добавить в базу новый товар, перейдя в соответствующее окно посредством нажатия кнопки "Добавить товар".
3. **Поиск товаров.** В левом верхнем углу, введите информацию для поиска.
4. **Удаление товара.** Выберите товар из списка и в контекстном меню нажмите «Удалить товар».
5. **Установка статуса заказа.** Через контекстное меню можно установить один из 5 статусов (ожидание оплаты, оплачен, в обработке, отправлен, доставлен) для выбранного заказа.
6. **Редактирование товаров.** Двойным нажатием на колонку, можно изменить описание/цену/количество товара на складе, изменения сохраняются в базе данных и отображаются в листе.
7. **Добавление новых пользователей.** Нажатием кнопки «Добавить пользователя» открывается соответствующее окно, в котором нужно заполните поля данными и выбрать роль для нового пользователя.
8. **Генерация отчетов.** Менеджер может формировать отчет по продажам.
9. **Внос данных о клиентах.** Включает ФИО, контактную информацию, историю заказов.
10. **Создание заказов.** С выбором товаров, количеством и адресом доставки.

# Роли и функциональности:
- **Администратор**
  - Добавление новых пользователей (администраторов, менеджеров или операторов).
  - Добавление новых товаров.
  - Удаление товаров.
  - Поиск товаров.
  - Изменение товаров.
  - Просмотр отчётов.

- **Менеджер**
  - Просмотр всех заказов.
  - Изменение статуса заказа (статус: ожидание оплаты, оплачен, в обработке, отправлен, доставлен).
  - Генерация отчётов.

- **Оператор**
  - Добавление новых покупателей.
  - Обработка заказов.
  - Связь с клиентами.

# Технологии, которые были использованы для разработки приложения:
- **C#** - объектно-ориентированный язык программирования.
- **Visual Studio Community 2022** - интегрированная среда разработки (IDE), которая обеспечивает удобное создание, отладку и развертывание приложений.
- **Windows Presentation Foundation** - система для построения клиентских приложений Windows с визуально привлекательными возможностями взаимодействия с пользователем, графическая подсистема в составе .NET Framework, использующая язык XAML.
- **SQLite** - компактная встраиваемая СУБД.

# Описание базы данных:
#### Файл базы данных называется meowstore.db <br/>
Файл базы данных расположен локально в проекте по пути **MeowStore\MeowStore\bin\Debug** </br>
В базе данных находятся 9 таблиц: _Users_, _Roles_, _Points_, _Status_, _Customers_, _Categories_, _Products_, _Basket_, _ProdBasket_.

- **Таблица «Users»** _(idUser, Username, Password, idRole)_ содержит информацию о пользователях.
- **Таблица «Roles»** _(idRole, NameRole)_ содержит информацию о ролях пользователей.
- **Таблица «Points»** _(idPoint, AddressPoint)_ содержит информацию о пунктах выдачи.
- **Таблица «Status»** _(idStatus, NameStatus)_ содержит информацию о статусах заказов.
- **Таблица «Customers»** _(idCustomer, FullName, Contact)_ содержит информацию о покупателях.
- **Таблица «Categories»** _(idCategory, NameCategory)_ содержит информацию о категориях товаров.
- **Таблица «Products»** _(idProduct, idCategory, NameProduct, Description, Price, Count, PathImage)_ содержит информацию о продуктах.
- **Таблица «Basket»** _(idBasket, idCustomer, idPoint, idStatus)_ содержит информацию о корзинах покупок.
- **Таблица «ProdBasket»** _(idProduct, Count, Sum, idBasket)_ содержит информацию о товарах в корзине.

# Видеодемонстрация:
Видеодемонстрацию использования приложения доступна [по этой ссылке](https://drive.google.com/drive/folders/1nlCq9ILRut2anlsPN6z-F19V1PpdU_OY).

# Скриншоты приложения:

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/MainWindow.png">
</br>Окно авторизации
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/AdminWin.png">
</br>Окно администратора
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/ManagerWin.png">
</br>Окно менеджера
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/AddCustomerWin.png">
</br>Добавление нового покупателя
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/OperatorWin.png">
</br>Окно оператора
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/OrderWin.png">
</br>Оформление заказа
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/Search.png">
</br>Поиск
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/AddProductsWin.png">
</br>Добавление нового товара
</br> </br> </br>
</p>

<p align="center">
  <img <img src="https://github.com/vanyaokblog/MeowStore/blob/main/Screenshots/AddUserWin.png">
</br>Добавление нового пользователя
</br> </br> </br>
</p>
