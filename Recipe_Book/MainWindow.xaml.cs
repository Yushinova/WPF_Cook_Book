using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Recipe_Book
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Path;
        public List <Section> sections = new List <Section> ();
        public MainWindow()
        {
            InitializeComponent();
            Deserial();
        }
        private void Serial()//for нужен и установить ID
        {
            for (int i = 0; i < RecipeList.Items.Count; i++)
            {
                string name = ((RecipeList.Items[i] as StackPanel).Children[1] as TextBlock).Text;
                string id = (RecipeList.Items[i] as StackPanel).Uid;
                Section section = new Section { Name = name, ID = id };
                sections.Add(section);
            }
            Text1.Text = RecipeList.Items.Count.ToString();
            XmlSerializer bf = new XmlSerializer(typeof(List<Section>));
            using (Stream fstr = File.Create("sections.xml"))
            {
                bf.Serialize(fstr, sections);
            }
        }
        private void Deserial()
        {
            XmlSerializer bf = new XmlSerializer(typeof(List<Section>));
            if (System.IO.File.Exists("sections.xml"))
            {
                using (Stream fsteread = File.OpenRead("sections.xml"))
                {
                    sections = (List<Section>)bf.Deserialize(fsteread);
                }
                ClearRecipelist();
            }
        }
        private void ClearRecipelist()
        {
            string bin = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\";//адоес debug папки из текущего репозитория
            if (RecipeList.Items.Count > 0) RecipeList.Items.Clear();
            foreach (var item in sections)
            {
                string Path = bin + item.ID + ".png";
                RecipeList.Items.Add(AddPanel(Path, item.Name, item.ID));
            }
        }
        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            //FrameworkElement parent = (FrameworkElement)((Button)sender).Parent;
            Button b = sender as Button;
            //FrameworkElement parent2 = (FrameworkElement)(parent).Parent;
            Text1.Text = b.Uid;
        }
        private Button AddPlus(string text)
        {
            Button button = new Button();
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\4.png"));
            button.Content = img;
            button.Uid = text;
            button.Click += new RoutedEventHandler(Plus_Click);
            return button;
        }
        private StackPanel AddPanel(string path, string nameSection, string save_name)
        {
            Image image = new Image();//вынести в отдельную функцию для динамического добавления
            image.Width = 50;
            image.Height = 50;
            image.Source = new BitmapImage(new Uri(path));
            image.Stretch = Stretch.Fill;  
            StackPanel panel = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = nameSection;
            Text1.Text = text.Text;
            panel.Children.Add(image);
            panel.Children.Add(text);
            panel.Children.Add(AddPlus(save_name));
            panel.Uid = save_name;
            panel.MouseRightButtonDown += new MouseButtonEventHandler(MouseRight_RecipeList);
            //panel.Uid = Path.GetFileNameWithoutExtension(path);
            return panel;
        }
        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
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
                Path = f.FullName;
                Text1.Text = Path;
                // Path = dialog.FileName;
            }
            Button b = sender as Button;
            string nameSection = ((b.Parent as System.Windows.Controls.StackPanel).Children[1] as TextBox).Text;
            Text1.Text = b.Content.ToString();
            string save_name = Guid.NewGuid().ToString();//уникальный идентификатор
            BitmapImage save = new BitmapImage(new Uri(Path));
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();//сохранение картинки в папке программы
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(save));
            using (FileStream fileStream = new FileStream(@"image\" + save_name+ ".png", FileMode.Create))//bin debug
           jpegBitmapEncoder.Save(fileStream);
            RecipeList.Items.RemoveAt(RecipeList.Items.Count - 1);//удаляем временную панель для редактирования
            RecipeList.Items.Add(AddPanel(Path,nameSection, save_name));
        }
        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = new StackPanel();
            Button button = new Button();
            button.Height = 50;
            button.Width = 50;
            button.Background = Brushes.LightPink;
            button.Content = "Иконка";
            button.Click += new RoutedEventHandler(AddImage_Click);//добавляем событие на кнопку        
            panel.Children.Add(button);
            TextBox box = new TextBox();
            box.Height = 34;
            box.Width = 160;
            box.FontSize = 18;
            box.Text = "Мой раздел";
            panel.Children.Add(box);
            RecipeList.Items.Add(panel);
        }

        private void MouseRight_RecipeList(object sender, MouseButtonEventArgs e)//вызов меню удалить редактировать разделы
        {
            Point p = e.GetPosition(this);
            RedMenu.Visibility = Visibility.Visible;
        }
    }
}
