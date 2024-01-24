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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            //FrameworkElement parent = (FrameworkElement)((Button)sender).Parent;
            Button b = sender as Button;
            //FrameworkElement parent2 = (FrameworkElement)(parent).Parent;
            Text1.Text = b.Uid;
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
            Image image = new Image();//вынести в отдельную функцию для динамического добавления
            image.Width = 50;
            image.Height = 50;
            image.Source = new BitmapImage(new Uri(Path));
            image.Stretch = Stretch.Fill;
            Button b = sender as Button;
            Text1.Text = b.Content.ToString();
            StackPanel panel = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = ((b.Parent as System.Windows.Controls.StackPanel).Children[1] as TextBlock).Text;
            Text1.Text = text.Text;
            panel.Children.Add(image);
            panel.Children.Add(text);
            Button button = new Button();
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\4.png"));
            button.Content = img;
            button.Uid = text.Text;
            button.Click+= new RoutedEventHandler(Plus_Click);
            panel.Children.Add(button);
            RecipeList.Items.RemoveAt(RecipeList.Items.Count - 1);
            RecipeList.Items.Add(panel);
        }
        private void NewSection_Click(object sender, RoutedEventArgs e)
        {
            string s = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\1.png";//для примера берем картинку из папки
            StackPanel panel = new StackPanel();
            Button button = new Button();
            button.Height = 50;
            button.Width = 50;
            button.Content = "Иконка";
            button.Click += new RoutedEventHandler(AddImage_Click);//добавляем событие на кнопку
           
            panel.Children.Add(button);
            TextBlock block = new TextBlock();
            
            //block.Height = 50;
            //block.Width = 80;
            block.Text = "Мой раздел";
            panel.Children.Add(block);
            RecipeList.Items.Add(panel);
        }
    }
}
