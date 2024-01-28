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
        public int IndexResipeList {  get; set; }
        public MainWindow()
        {
            InitializeComponent();
            if (System.IO.File.Exists("sections.xml"))
            {
                Deserial();
                ClearRecipelist();//перезаполнение 
            }
            else { AddSections(); }
              
        }
        private void AddSections()
        {
            for (int i = 0; i < RecipeList.Items.Count; i++)
            {
                string name = ((RecipeList.Items[i] as StackPanel).Children[1] as TextBlock).Text;
                string id = (RecipeList.Items[i] as StackPanel).Uid;
                Section section = new Section { Name = name, ID = id };
                sections.Add(section);
            }
        }
        private void Serial()
        {
            XmlSerializer bf = new XmlSerializer(typeof(List<Section>));
            using (Stream fstr = File.Create("sections.xml"))//добавляет 2 раза
            {
                bf.Serialize(fstr, sections);
            }
        }
        private void Deserial()
        {
            XmlSerializer bf = new XmlSerializer(typeof(List<Section>));
           
                using (Stream fsteread = File.OpenRead("sections.xml"))
                {
                    sections = (List<Section>)bf.Deserialize(fsteread);
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
            Button b = sender as Button;//надо получить индекс 
            Text1.Text = b.Uid;

            Recipe recipe = new Recipe { ID = b.Uid, NameResipe = "Soup", ingredients = { "hhdh", "hhddh" } };
            sections[Convert.ToInt32(b.Uid)].SetRecipe(recipe);
            Recipe recipe1 = new Recipe { ID = b.Uid, NameResipe = "Sdvfd", ingredients = { "hhdh", "hhddh" } };
            sections[Convert.ToInt32(b.Uid)].SetRecipe(recipe1);
            //Text1.Text = (sections[Convert.ToInt32(b.Uid)].GetRecipes()).Count().ToString();
            //Serial();

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
            panel.Children.Add(image);
            panel.Children.Add(text);
            panel.Children.Add(AddPlus((RecipeList.Items.Count).ToString()));
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
            RecipeList.Items.RemoveAt(IndexResipeList);//удаляем временную панель для редактирования
            RecipeList.Items.Insert(IndexResipeList, AddPanel(Path,nameSection, save_name));
            //foreach (var item in RecipeList.Items)//после удаления раздела нужно будет переустановить все уид плюсов
            //{            
            //    (item as StackPanel).Children[2].Uid = "new";
              
            //}
            //Text1.Text = ((RecipeList.Items[3] as StackPanel).Children[2] as Button).Uid;
        }
        private StackPanel RedPanel()
        {
            StackPanel panel = new StackPanel();
            Button button = new Button();
            button.Height = 50;
            button.Width = 50;
            button.Background = Brushes.WhiteSmoke;
            button.Content = "Иконка";
            button.Click += new RoutedEventHandler(AddImage_Click);//добавляем событие на кнопку        
            panel.Children.Add(button);
            TextBox box = new TextBox();
            box.Height = 34;
            box.Width = 160;
            box.FontSize = 18;
            box.Text = "Мой раздел";
            panel.Children.Add(box);
            return panel;
        }
        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
           
            RecipeList.Items.Add(RedPanel());
            IndexResipeList = RecipeList.Items.Count - 1;
        }

        private void MouseRight_RecipeList(object sender, MouseButtonEventArgs e)//вызов меню удалить редактировать разделы
        {
            Canvas.SetTop(RedMenu, e.GetPosition(this).Y-230);
            RedMenu.Visibility = Visibility.Visible;
            IndexResipeList = Convert.ToInt32((sender as StackPanel).Children[2].Uid);//иногда промахивается
            Text1.Text = IndexResipeList.ToString();
        }
        private void Red_Click(object sender, RoutedEventArgs e)
        {
            
            RecipeList.Items.RemoveAt(IndexResipeList);
            RecipeList.Items.Insert(IndexResipeList, RedPanel());
        }
    }
}
