using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace new_cook_book
{
    public class Helper
    {
        public ObservableCollection<Recipe_Item> recipe_Items = new ObservableCollection<Recipe_Item>();//с обычным листом так просто не получится сделать связку
        public ObservableCollection<Recipe> all_recipes = new ObservableCollection<Recipe>();//Список рецептов при выборке по разделу будет заполняться в соответствие с выборой
        public List<Recipe> recipes = new List<Recipe>();//обычный лист для хранения всех рецептов
        public Recipe recipe = new Recipe();
        public string path = "";
        public bool isChangeImage = false;
        public bool isAdd = false;
        public bool isRed = false;
        public string domainPath = AppDomain.CurrentDomain.BaseDirectory.ToString();
        public string del_path = "";
        public string temp_ID = "";
    }
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value.ToString();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.UriSource = new Uri(path);
            bi.EndInit();
            return bi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
