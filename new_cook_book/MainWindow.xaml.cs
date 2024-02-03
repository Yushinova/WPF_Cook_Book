using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace new_cook_book
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Resipe_Item> resipe_Items = new ObservableCollection<Resipe_Item>();//с обычным листом так просто не получится сделать связку
        public string path = "";
        public bool isChangeImage = false;
        public bool isAdd = false;
        public bool isRed = false;
        public int IndexItem { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            if (System.IO.File.Exists("sections.xml"))
            {
                Deserial();
            }
            else
            {
                resipe_Items = new ObservableCollection<Resipe_Item>//заводские разделы меню
                {
                new Resipe_Item{ID_image="1", Name= "Первые блюда", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\1.png"))},
                new Resipe_Item{ID_image="2", Name= "Вторые блюда", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\2.png"))},
                new Resipe_Item{ID_image="3", Name= "Салаты", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\3.png"))},
                new Resipe_Item{ID_image="5", Name= "Завтрак", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\5.png"))},
                new Resipe_Item{ID_image="6", Name= "Десерты", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\6.png"))},
                };
            }
            RecipeList.ItemsSource = resipe_Items;
        }
        private void Serial()
        {
            List<Resipe_Item> resipe_ = new List<Resipe_Item>();
            foreach (var item in resipe_Items)
            {
                Resipe_Item temp = new Resipe_Item { ID_image = item.ID_image, Name = item.Name, recipes = item.recipes };
                resipe_.Add(temp);
            }
            XmlSerializer bf = new XmlSerializer(typeof(List<Resipe_Item>));
            using (Stream fstr = File.Create("sections.xml"))
            {
                bf.Serialize(fstr, resipe_);
            }
        }
        private void Deserial()
        {
            List<Resipe_Item> resipe_ = new List<Resipe_Item>();
            XmlSerializer bf = new XmlSerializer(typeof(List<Resipe_Item>));
            using (Stream fsteread = File.OpenRead("sections.xml"))
            {
                resipe_ = (List<Resipe_Item>)bf.Deserialize(fsteread);
            }
            Text1.Text = resipe_.Count.ToString();
            foreach (var item in resipe_)
            {
                ImageSource temp_im = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + item.ID_image + @".png"));
                resipe_Items.Add(new Resipe_Item { ID_image = item.ID_image, Name = item.Name, myImg = temp_im, recipes = item.recipes });
            }
        }
        private void AddSection_Click(object sender, RoutedEventArgs e)//нажатие по кнопке Добавить
        {
            DownLoadImage.Content = "Иконка";//если панель заполнена после редактирования или добавления чем то, возвращаем стандартные настройки
            path = "";//обнуляем путь катринки
            RedText.Text = "Ваш раздел";
            RedPanel.Visibility = Visibility.Visible;
            isAdd = true;//активируем ключ добавления
            isRed = false;//пр этом ключ редактирования сбрасываем
        }
        private void MouseLeaveMenuRed(object sender, MouseEventArgs e)//если выходим за пределы контекстного меню оно исчезает
        {
            RedMenu.Visibility = Visibility.Hidden;
        }
        private void Right_Click(object sender, MouseButtonEventArgs e)//вызов контекстного меню по правой кнопке мыши
        {
            Canvas.SetTop(RedMenu, e.GetPosition(this).Y - 230);//устанавливаем контекстное меню на координатах редактируемого раздела
            RedMenu.Visibility = Visibility.Visible;
            var item = (sender as FrameworkElement).DataContext;//получаем объект по которому жмякнули
            IndexItem = RecipeList.Items.IndexOf(item);//запоминаем индекс по которому кликнули
            //Text1.Text = IndexItem.ToString();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)//удаление раздела с рецептами
        {
            if (MessageBox.Show("Вы хотите удалить " + resipe_Items[IndexItem].Name + "?", "...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string str_del = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + resipe_Items[IndexItem].ID_image + ".png";//запоминаем путь картинки которую нужно удалить
                Text1.Text = str_del;
                resipe_Items[IndexItem].myImg = null;//пытаемся обнулить ресурсы удаляемого изображения(не работает)
                resipe_Items.RemoveAt(IndexItem);
                (new System.Threading.Thread(() =>//без этого невозможно удалить чертову картинку!!!!!!
                {
                    while (true)//в новом потоке пытаемся удалить картинку, получаается не всегда быстро
                    {
                        try
                        {
                            File.Delete(str_del);
                            MessageBox.Show("Picture Removed");
                            break;
                        }
                        catch { }
                    }
                })).Start();
            }
        }

        private void Add_Image(object sender, RoutedEventArgs e)//добавление картинки
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();//диалоговое окно
            dialog.FileName = "Image"; // Default file name
            dialog.DefaultExt = ".jpg"; // Default file extension
            dialog.Filter = "Image (.jpg)|*.jpg"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                FileInfo f = new FileInfo(dialog.FileName);
                path = f.FullName;
                // Path = dialog.FileName;
            }
            Image image = new Image();//на нашу кнопку для визуализации добавляем картинку
            image.Source = new BitmapImage(new Uri(path));
            DownLoadImage.Content = image;
            isChangeImage = true;//если произошла загрузка нового изображения
        }
        private Resipe_Item New_Resipe_Item()//Новый раздел рецептов
        {
            string save_name = Guid.NewGuid().ToString();//халабудим уникальный идентификатор название картинки
            BitmapImage save = new BitmapImage(new Uri(path));
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();//сохранение картинки в папке программы
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(save));
            using (FileStream fileStream = new FileStream(@"image\" + save_name + ".png", FileMode.Create))//bin debug
            jpegBitmapEncoder.Save(fileStream);

            Resipe_Item item = new Resipe_Item { ID_image = save_name, Name = RedText.Text, myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + save_name + ".png")) };
            return item;
        }
        private void Add_Item_Click(object sender, RoutedEventArgs e)//кнопка Ок при добавлении/редактировании нового раздела
        {
            if(isAdd)//если нажали на добаление
            {
                if (path == "")
                {
                    MessageBox.Show("Загрузите иконку", "Ошибка загрузки",
             MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    resipe_Items.Add(New_Resipe_Item());
                    RedPanel.Visibility = Visibility.Hidden;
                    isChangeImage = false;//сбрасываем ключ загрузки изображения
                    //Serial();
                }
            }
            if(isRed)//если нажали на редактирование
            {
                if (isChangeImage)//если мы решили сменить картинку
                {
                    resipe_Items.RemoveAt(IndexItem);//удаляем неотредактированный раздел
                    resipe_Items.Insert(IndexItem, New_Resipe_Item());//вставляем на нужную позицию отредактированный раздел
                    RedPanel.Visibility = Visibility.Hidden;
                    isChangeImage = false;
                }
                else//если картинку не меняли а поменяли название раздела
                {
                    string save_name = resipe_Items[IndexItem].ID_image;//оставляем картинку и путь к каритнке тот же самый
                    Resipe_Item item = new Resipe_Item { ID_image = save_name, Name = RedText.Text, myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + save_name + ".png")) };
                    resipe_Items.RemoveAt(IndexItem);
                    resipe_Items.Insert(IndexItem, item);
                    RedPanel.Visibility = Visibility.Hidden;
                    isChangeImage = false;
                }
            }          
        }
        private void Back_Click(object sender, RoutedEventArgs e)//возвращаемся на главную страницу
        {

        }
        private void Redact_Click(object sender, RoutedEventArgs e)//Нажатие Редактировать на контекстном меню
        {
            RedPanel.Visibility = Visibility.Visible;
            Image image = new Image();//на нашу кнопку для визуализации добавляем картинку
            image.Source = resipe_Items[IndexItem].myImg;
            DownLoadImage.Content = image;
            RedText.Text = resipe_Items[IndexItem].Name;
            isRed=true;//активируем триггер редактирования
            isAdd = false;//добавление сбросить
        }

        private void MouseDownHideRedPanel(object sender, MouseButtonEventArgs e)
        {
            RedPanel.Visibility = Visibility.Hidden;

        }
    }
}
