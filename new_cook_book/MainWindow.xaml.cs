using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace new_cook_book
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection <Resipe_Item> resipe_Items { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            resipe_Items = new ObservableCollection<Resipe_Item>
            {
                new Resipe_Item{ImageSourse="image/1.png", Name= "Первые блюда"},
                new Resipe_Item{ImageSourse="image/2.png", Name= "Вторые блюда"},
                new Resipe_Item{ImageSourse="image/3.png", Name= "Салаты"},
                new Resipe_Item{ImageSourse="image/5.png", Name= "Завтрак"},
                new Resipe_Item{ImageSourse="image/6.png", Name= "Десерты"},
            };
            RecipeList.ItemsSource = resipe_Items;
        }

        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            resipe_Items.Add(new Resipe_Item { ImageSourse = "image/2.png", Name = "hdhdhhdh" });
        }
    }
}
