using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace Brew
{
    // Класс Пивоварной компании со свойствами
    /* Название
     * Страна
     * Дата Основания
     * Имя основателя
     * Количество продукции
     * Средний рейтиинг
     * Что производится
     */
    public class Brewery
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public DateTime DateOfCreation { get; set; }
        public string FounderName { get; set; }
        public int VarietyCount { get; set; }
        public double AvgRating { get; set; }
        public string Products { get; set; }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Коллекция пивоварен
        ObservableCollection<Brewery> breweries;
        public MainWindow()
        {
            InitializeComponent();
            breweries = new ObservableCollection<Brewery>();

            // Если файл с таблицей существует
            if (new FileInfo("table.txt").Exists)
            {
                // Считываем строки и проходимся по ним
                foreach(string line in File.ReadAllLines("table.txt"))
                {
                    // Разделяем строку по символу-разделителю
                    // Вид строки в файле: Name|Country|dd,MM,yyyy|FounderName|00|0,00|a;b;c;
                    string[] data = line.Split('|');
                    // Разделяем дату по дню, месяцу и году
                    int[] date = data[2].Split(',').Select(int.Parse).ToArray();
                    // Заполняем пивоварню
                    Brewery newline = new Brewery
                    {
                        Name = data[0],
                        Country = data[1],
                        DateOfCreation = new DateTime(date[2], date[1], date[0]), // Дата в фромате YYYY, MM, DD
                        FounderName = data[3],
                        VarietyCount = int.Parse(data[4]),
                        AvgRating = float.Parse(data[5]),
                        Products = data[6]
                    };
                    // Добавляем ее в коллекцию
                    breweries.Add(newline);
                }
                // Очищаем файл
                File.WriteAllText("table.txt", "");
            }
            // Если файла нет, то создаем его
            else
            {
                FileStream fs = File.Create("table.txt");
                fs.Close();
            }
            // Устанавливаем источних данных для DataGrid
            BreweryGrid.ItemsSource = breweries;
        }

        // Нажатие на кнопку Добавить
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            // создается окно для добавления
            wndCreate add = new wndCreate("Добавить");

            // Инициализируем как диалогове окно
            add.ShowDialog();

            // Если мы нажали в окне добавление на "Добавить", то добавляем элемент в коллекцию
            if (add.DialogResult ?? true)
            {
                breweries.Add(add.Item);
            }
        }

        // Нажатие на кнопку Изменить
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            // Если не выбрана строка, выводим сообщение
            if(!(BreweryGrid.SelectedItem is Brewery row))
            {
                MessageBox.Show("Выберите строку для изменения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Создаем окно для изменения и передем в него коллекцию и выбраный ряд
                wndCreate Edit = new wndCreate("Изменить", breweries, row);
                // Инициализируем как диалогове окно
                Edit.ShowDialog();
            }
        }

        // Нажатие на кнопку Удалить
        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Если не выбрана строка, выводим сообщение 
            if (!(BreweryGrid.SelectedItem is Brewery row))
            {
                MessageBox.Show("Выберите строку для изменения.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBoxResult dlg = MessageBox.Show("Внатуре удалить?", "Опа!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(dlg == MessageBoxResult.Yes)
                    // Удаляем ряд
                    breweries.Remove(row);
            }
        }

        // При закрытии окна сохраняем таблицу в файл
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach(Brewery toAdd in breweries)
            {
                File.AppendAllText("table.txt", $"{toAdd.Name}|{toAdd.Country}|{toAdd.DateOfCreation:dd,MM,yyyy}|{toAdd.FounderName}|{toAdd.VarietyCount}|{toAdd.AvgRating:F2}|{toAdd.Products}\n");
            }
        }
    }
}
