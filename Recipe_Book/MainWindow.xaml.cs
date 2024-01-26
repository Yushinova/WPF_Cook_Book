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
        }
        private void Serial()//for нужен и установить ID
        {
            string name = (((RecipeList.Items[0] as ListViewItem).Content as StackPanel).Children[1] as TextBlock).Text;
            string id = (RecipeList.Items[0] as ListViewItem).Uid;
            Section section = new Section { Name = name, ID = id };
            sections.Add(section);
            Text1.Text = sections[0].ID;


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
        private StackPanel AddPanel(string path, string nameSection)
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
            panel.Children.Add(AddPlus(text.Text));
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
            string Hash = nameSection.GetHashCode().ToString();
            Text1.Text = b.Content.ToString();
            string save_name = Guid.NewGuid().ToString();//уникальный идентификатор
            BitmapImage save = new BitmapImage(new Uri(Path));
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();//сохранение картинки в папке программы
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(save));
            using (FileStream fileStream = new FileStream(@"image\" + save_name+ ".png", FileMode.Create))//bin debug
                jpegBitmapEncoder.Save(fileStream);
            RecipeList.Items.RemoveAt(RecipeList.Items.Count - 1);
            RecipeList.Items.Add(AddPanel(Path,nameSection));
            Serial();

        }
        private void NewSection_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = new StackPanel();
            Button button = new Button();
            button.Height = 50;
            button.Width = 50;
            button.Content = "Иконка";
            button.Click += new RoutedEventHandler(AddImage_Click);//добавляем событие на кнопку
           
            panel.Children.Add(button);
            TextBox box = new TextBox();
            box.Height = 34;
            box.Width = 160;
            box.FontSize = 18;
            box.Text = "Добавить";
            panel.Children.Add(box);
            RecipeList.Items.Add(panel);
        }
    }
}
