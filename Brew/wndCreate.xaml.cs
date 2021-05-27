using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace Brew
{
    /// <summary>
    /// Логика взаимодействия для wndCreate.xaml
    /// </summary>
    public partial class wndCreate : Window
    {
        // Создаем переменные коллекции и ряда в глобальной области видимости
        ObservableCollection<Brewery> breweries; // коллекция пивоварен
        Brewery row;

        // Конструктор для изменения ряда
        public wndCreate(string caption, ObservableCollection<Brewery> breweries, Brewery row)
        {
            InitializeComponent();
            // Устанавливаем заголовок окна
            Title = caption;
            // Заполняем поля по данным из переданного ряда
            NameTB.Text = row.Name;
            CountryTB.Text = row.Country;
            DateDP.SelectedDate = row.DateOfCreation;
            FounderTB.Text = row.FounderName;
            VarietyTB.Text = row.VarietyCount.ToString();
            AvgRatingTB.Text = string.Format(@"{0:F2}", row.AvgRating);
            // Создаем ComboBoxItem в котором будет содержаться список выбранных продуктов
            ComboBoxItem list = new ComboBoxItem();
            // Выставляем отступы как у остальных строк в ComboBox, потому что надо
            list.Margin = new Thickness(-5, -2, -5, -3); //толщина рамки 
            // выставляем ширину как у других элементов
            list.Width = 158;
            list.Content = row.Products;
            // Ставим высоту 0 чтобы элемент был скрыт
            list.Height = 0;
            // Ставим галочки
            foreach (CheckBox cb in ProductCB.Items)
            {
                foreach(string a in list.Content.ToString().Split(';'))
                {
                    if (cb.Content.ToString() == a.Trim()) cb.IsChecked = true;
                }
            }
            // Добавляем его в ComboBox
            ProductCB.Items.Add(list);
            
            // Делаем его как выбранный, чтобы он отображался в ComboBox
            ProductCB.SelectedItem = list;
            this.breweries = breweries;
            this.row = row;
            // Переименовываем кнопку
            AcceptBtn.Content = "Сохранить";
        }

        // Конструктор для Добавления
        public wndCreate(string caption)
        {
            InitializeComponent();
            Title = caption;
        }
        // Нажатие на CheckBox'ы
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Строка выбранных элементов
            string res = "";
            ComboBoxItem list = new ComboBoxItem();
            list.Margin = new Thickness(-5, -2, -5, -3);
            list.Width = 158;

            // Пытаемся удлаить последний элемент(ранее добавленный ComboBoxItem), чтобы не было ошибки в foreach
            try
            {
                object toDelete = ProductCB.Items[6];
                ProductCB.Items.Remove(toDelete);
            } catch(Exception)
            {
                //
            }
            foreach (CheckBox cb in ProductCB.Items)
            {
                // Формируем строку выбранных элементов из combobox и преобразуем их в строку
                res += (cb.IsChecked ?? true) ? (cb.Content + "; ") : "";
            }
            // Записываем строку в значение созданного ComboBoxItem, добавляем его и делаем выбранным
            list.Content = res;
            list.Height = 0;
            ProductCB.Items.Add(list);
            ProductCB.SelectedItem = list;
        }

        // Кнопка Добавить/Сохранить
        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            // Регулярные выражения для числа, страны, дробного числа, названия и имени
            Regex number = new Regex(@"^[1-9][0-9]*$"); // => Целое положительное число начинающееся не с нуля
            Regex country = new Regex(@"^[A-z][A-Za-z]+.*$"); // => В начале заглавная буква, потом только буквы(1 или более) потом любые символы
            Regex float_number = new Regex(@"^[0-9][,][0-9]{2}$"); // => число формата X,XX от 0.00 до 9.99
            Regex name = new Regex(@"^[A-Z][a-z]+.*$"); // => Начинается с большой буквы, потом маленькие(1 или более) потом любые символы
            Regex founder = new Regex(@"^[A-Z][.][\s][A-Z][a-z]+$"); // => Одна заглавная буква, точка, пробел, одно слово с заглавной буквы

            // Проверяем соответсвия, и в случае несовпадения выводим сообщение и выходим из функции
            if (!name.IsMatch(NameTB.Text))
            {
                MessageBox.Show("Неверный формат данных. (Название)\n\nПример: Beergood, Sinergoff");
                return;
            }
            else if (!country.IsMatch(CountryTB.Text))
            {
                MessageBox.Show("Неверный формат данных. (Страна)\n\nПример: Russia, Usa, Brazil");
                return;
            }
            else if (DateDP.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату основания.");
                return;
            }
            else if (!founder.IsMatch(FounderTB.Text))
            {
                MessageBox.Show("Неверный формат данных. (Основатель)\n\nПример: T. Hanks, R. Teres");
                return;
            }
            else if (!number.IsMatch(VarietyTB.Text))
            {
                MessageBox.Show("Количество продуктов должно быть целым положительным числом.");
                return;
            }
            else if (!float_number.IsMatch(AvgRatingTB.Text))
            {
                MessageBox.Show("Средний рейтинг должен быть в формате \"X,XX\"( От 0 до 9,99)");
                return;
            }
            else if(ProductCB.Text == "")
            {
                MessageBox.Show("Выберите продукцию.");
                return;
            }
            
            // Если мы в режиме Изменения
            if (Title == "Изменить")
            {
                // Вставляем на место текущего ряда новосозданный элемент
                breweries.Insert(breweries.IndexOf(row), Item);
                // Удаляем старый ряд
                breweries.Remove(row);
            }
            // Выходим из диалогового окна
            DialogResult = true;
        }

        // Получение элемента из введенных данных
        public Brewery Item
        {
            get
            {
                Brewery res = new Brewery
                {
                    Name = NameTB.Text.Trim(),
                    Country = CountryTB.Text.Trim(),
                    DateOfCreation = DateDP.SelectedDate.Value,
                    FounderName = FounderTB.Text.Trim(),
                    VarietyCount = int.Parse(VarietyTB.Text.Trim()),
                    AvgRating = double.Parse(AvgRatingTB.Text.Trim()),
                    Products = ProductCB.Text.Trim(),
                };
                return res;
            }
        }
    }
}
