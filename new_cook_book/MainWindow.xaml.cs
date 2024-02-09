using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
        Helper helper = new Helper();
        public MainWindow()
        {
            InitializeComponent();
            if (System.IO.File.Exists("sections.xml"))
            {
                Deserial();
            }
            else
            {
                helper.recipe_Items = new ObservableCollection<Recipe_Item>//заводские разделы меню
                {
                new Recipe_Item{ID_image="1", Name= "Первые блюда", Url = helper.domainPath + @"image\1.png"},
                new Recipe_Item{ID_image="2", Name= "Вторые блюда", Url = helper.domainPath + @"image\2.png"},
                new Recipe_Item{ID_image="3", Name= "Салаты", Url = helper.domainPath + @"image\3.png"},
                new Recipe_Item{ID_image="5", Name= "Завтрак", Url = helper.domainPath + @"image\5.png"},
                new Recipe_Item{ID_image="6", Name= "Десерты", Url = helper.domainPath + @"image\6.png"},
                };
                helper.recipes = new List<Recipe>
                {
                    new Recipe{ID="1", IDRecipeItem="1", NameResipe="Борщ", UrlRecipe = helper.domainPath + @"image\1.png", ingredients={"капуста","картофель","лук" }, Description="Положите одно, потом другое и третье" },
                    new Recipe{ID="2", IDRecipeItem="1", NameResipe="Суп", UrlRecipe = helper.domainPath + @"image\2.png", ingredients={"курица","картофель","лук" }, Description="Отварите курицу, пассируйте лук и морковь" },
                };
            }   
            AllRecipe.ItemsSource = helper.all_recipes;
            RecipeList.ItemsSource = helper.recipe_Items;
        }   
        private void Serial()
        {
            List<Recipe_Item> recipe_ = new List<Recipe_Item>();
            foreach (var item in helper.recipe_Items)
            {
                recipe_.Add(item);
            }
            XmlSerializer bf = new XmlSerializer(typeof(List<Recipe_Item>));
            using (Stream fstr = File.Create("sections.xml"))
            {
                bf.Serialize(fstr, recipe_);
            }
            bf = new XmlSerializer(typeof(List<Recipe>));
            using (Stream fstr = File.Create("recipes.xml"))
            {
                bf.Serialize(fstr, helper.recipes);
            }
        }
        private void Deserial()
        {
            List<Recipe_Item> recipe_ = new List<Recipe_Item>();
            XmlSerializer bf = new XmlSerializer(typeof(List<Recipe_Item>));
            using (Stream fsteread = File.OpenRead("sections.xml"))
            {
                recipe_ = (List<Recipe_Item>)bf.Deserialize(fsteread);
            }
            foreach (var item in recipe_)
            {
                helper.recipe_Items.Add(new Recipe_Item { ID_image = item.ID_image, Name = item.Name, Url = helper.domainPath + @"image\" + item.ID_image + @".png" });
            }
            bf = new XmlSerializer(typeof(List<Recipe>));
            using (Stream fsteread = File.OpenRead("recipes.xml"))
            {
                helper.recipes = (List<Recipe>)bf.Deserialize(fsteread);
            }
        }
        //РАБОТА С РАЗДЕЛАМИ КУЛИНАРНОЙ КНИГИ
        private void AddSection_Click(object sender, RoutedEventArgs e)//нажатие по кнопке Добавить
        {
            DownLoadImage.Content = "Иконка";//если панель заполнена после редактирования или добавления чем то, возвращаем стандартные настройки
            helper.path = "";//обнуляем путь катринки
            RedText.Text = "Ваш раздел";
            RedPanel.Visibility = Visibility.Visible;
            helper.isAdd = true;//активируем ключ добавления
            helper.isRed = false;//пр этом ключ редактирования сбрасываем
        }
        private void MouseLeaveMenuRed(object sender, MouseEventArgs e)//если выходим за пределы контекстного меню оно исчезает
        {
            RedMenu.Visibility = Visibility.Hidden;
        }
        private void Right_Click(object sender, MouseButtonEventArgs e)//вызов контекстного меню по правой кнопке мыши
        {
            Canvas.SetTop(RedMenu, e.GetPosition(this).Y - 230);//устанавливаем контекстное меню на координатах редактируемого раздела
            RedMenu.Visibility = Visibility.Visible;
        }
        private void Delete_Click(object sender, RoutedEventArgs e)//удаление раздела с рецептами
        {
            if (MessageBox.Show("Вы хотите удалить " + (RecipeList.SelectedItem as Recipe_Item).Name + "?", "...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                helper.temp_ID = (RecipeList.SelectedItem as Recipe_Item).ID_image;
                helper.del_path = helper.domainPath + @"image\" + (RecipeList.SelectedItem as Recipe_Item).ID_image + ".png";//запоминаем путь картинки которую нужно удалить

                helper.recipe_Items.Remove(RecipeList.SelectedItem as Recipe_Item);

                for (int i = 0; i < helper.recipes.Count; i++)//удаляем все рецепты из раздела, который был удален
                {
                    if (helper.recipes[i].IDRecipeItem == helper.temp_ID)
                    {
                        string temp_del_path = helper.recipes[i].UrlRecipe;
                        helper.recipes.Remove(helper.recipes[i]);
                        if (System.IO.File.Exists(temp_del_path)) { File.Delete(temp_del_path); temp_del_path = ""; }
                        i--;
                    }
                        
                }
                File.Delete(helper.del_path);
                Serial();             
            }
        }
        public BitmapImage SetBitmap(string path)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.UriSource = new Uri(path);
            bi.EndInit();
            return bi;
        }
        private void Add_Image(object sender, RoutedEventArgs e)//добавление картинки
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();//диалоговое окно
            dialog.FileName = "Image"; // Default file name
            dialog.DefaultExt = ".jpg"; // Default file extension
            dialog.Filter = "Image (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                FileInfo f = new FileInfo(dialog.FileName);
                helper.path = f.FullName;
                // Path = dialog.FileName;
                Image image = new Image();//на нашу кнопку для визуализации добавляем картинку
                image.Source = SetBitmap(helper.path);
                DownLoadImage.Content = image;
                helper.isChangeImage = true;//если произошла загрузка нового изображения
            }
        }
        private Recipe_Item New_Resipe_Item()//Новый раздел рецептов
        {

            string save_name = Guid.NewGuid().ToString();//халабудим уникальный идентификатор название картинки
            BitmapImage save = new BitmapImage(new Uri(helper.path));
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();//сохранение картинки в папке программы
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(save));
            using (FileStream fileStream = new FileStream(@"image\" + save_name + ".png", FileMode.Create))//bin debug
            jpegBitmapEncoder.Save(fileStream);
         
            Recipe_Item item = new Recipe_Item { ID_image = save_name, Name = RedText.Text, Url = helper.domainPath + @"image\" + save_name + ".png" };
            return item;
        }
        private void Add_Item_Click(object sender, RoutedEventArgs e)//кнопка Ок при добавлении/редактировании нового раздела
        {
            if (helper.isAdd)//режим добавления
            {
                if (helper.path == "")//если картинка не выбрана
                {
                    MessageBox.Show("Загрузите иконку", "Ошибка загрузки",
             MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    helper.recipe_Items.Add(New_Resipe_Item());
                    RedPanel.Visibility = Visibility.Hidden;
                    helper.isChangeImage = false;//сбрасываем ключ загрузки изображения
                    MessageBox.Show("Раздел добавлен", "",
          MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            if (helper.isRed)//режим редактирования
            {
                if (helper.isChangeImage)//если мы решили сменить картинку
                {
                    helper.del_path = (RecipeList.SelectedItem as Recipe_Item).Url;
                    helper.recipe_Items[RecipeList.Items.IndexOf(RecipeList.SelectedItem)] = New_Resipe_Item();
                    if(System.IO.File.Exists(helper.del_path)) { File.Delete(helper.del_path); helper.del_path = ""; }
                    RedPanel.Visibility = Visibility.Hidden;
                    helper.isChangeImage = false;
                }
                else//если картинку не меняли а поменяли название раздела
                {
                    string save_name = (RecipeList.SelectedItem as Recipe_Item).ID_image;//оставляем картинку и путь к каритнке тот же самый
                    Recipe_Item item = new Recipe_Item { ID_image = save_name, Name = RedText.Text, Url = helper.domainPath + @"image\" + save_name + ".png" };
                    helper.recipe_Items[RecipeList.Items.IndexOf(RecipeList.SelectedItem)] = item;
                    RedPanel.Visibility = Visibility.Hidden;
                    helper.isChangeImage = false;
                }
            }
            Serial();
        }
        private void Back_Click(object sender, RoutedEventArgs e)//возвращаемся на главную страницу
        {
            RecipeList.Visibility = Visibility.Visible;

            AllRecipe.Visibility = Visibility.Hidden;
           RedRecipePanel.Visibility = Visibility.Hidden;
            ViewRecipePanel.Visibility = Visibility.Hidden;
            HidenGrid.Visibility = Visibility.Visible;
            AddSection.Visibility = Visibility.Visible;

        }
        private void Redact_Click(object sender, RoutedEventArgs e)//Нажатие Редактировать на контекстном меню
        {
            RedPanel.Visibility = Visibility.Visible;
            Image image = new Image();//на нашу кнопку для визуализации добавляем картинку
            image.Source = SetBitmap((RecipeList.SelectedItem as Recipe_Item).Url);
            DownLoadImage.Content = image;
            RedText.Text = (RecipeList.SelectedItem as Recipe_Item).Name;
            helper.isRed = true;//активируем триггер редактирования
            helper.isAdd = false;//добавление сбросить
        }

        private void MouseDownHideRedPanel(object sender, MouseButtonEventArgs e)//любое нажатие прячет панель редактирования разделов рецептов
        {
            RedPanel.Visibility = Visibility.Hidden;
            RedMenu.Visibility = Visibility.Hidden;

        }
        //РАБОТА С РЕЦЕПТАМИ
        private void AllRecipe_Click(object sender, RoutedEventArgs e)//рецепты все которые есть
        {
            RecipeList.Visibility = Visibility.Hidden;
            helper.all_recipes.Clear();
            foreach (var item in helper.recipes)
            {
                helper.all_recipes.Add(item);
            }
            AllRecipe.Visibility = Visibility.Visible;
            AddSection.Visibility = Visibility.Hidden;
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)//нажатие на плюс рядом с разделом (добавление нового раздела)
        {
            DownloadImage_Recipe.Content = "Загрузить";
            RedNameRecipe.Text = "Название";
            IngredientsBox.Items.Clear();
            RedResipeText.Text = "Описание рецепта";
            HidenGrid.Visibility = Visibility.Hidden;
            ViewRecipePanel.Visibility = Visibility.Hidden;
            RedRecipePanel.Visibility = Visibility.Visible;
            var item = (sender as FrameworkElement).DataContext;//получаем объект по которому жмякнули
            int ind = RecipeList.Items.IndexOf(item);//запоминаем индекс по которому кликнули
            helper.recipe = new Recipe(); //инициализируем обнуляем наш буферный рецепт
            helper.temp_ID = helper.recipe_Items[ind].ID_image;//даем ему ID  выбранного раздела
            helper.isAdd = true;
            helper.isRed = false;
        }

        private void AddImage_Recipe(object sender, RoutedEventArgs e)//добавляем картинку на наш рецепт
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();//диалоговое окно
            dialog.FileName = "Image"; // Default file name
            dialog.DefaultExt = ".jpg"; // Default file extension
            dialog.Filter = "Image (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                FileInfo f = new FileInfo(dialog.FileName);
                helper.path = f.FullName;
               
                Image image = new Image();//на нашу кнопку для визуализации добавляем картинку
                image.Source = SetBitmap(helper.path);
                DownloadImage_Recipe.Content = image;
                helper.isChangeImage = true;
            }
         
        }

        private void AddIngredientsPlus_Click(object sender, RoutedEventArgs e)//добавление ингридиентов
        {
            if (TextIngredients.Text != "")
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = TextIngredients.Text;
                IngredientsBox.Items.Add(item);
                TextIngredients.Text = "";

            }
        }
        private Recipe New_Recipe()
        {
            Recipe temp = new Recipe();

            if (helper.isRed)
            {
                helper.del_path = helper.recipe.UrlRecipe;//путь старой катринки, которую нужно удалить будет после редактирования
            }
            if(helper.isChangeImage)//картинка менялась
            {
                string save_name = Guid.NewGuid().ToString();//халабудим уникальный идентификатор название картинки
                BitmapImage save = new BitmapImage(new Uri(helper.path));
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();//сохранение картинки в папке программы
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(save));
                using (FileStream fileStream = new FileStream(@"image\" + save_name + ".png", FileMode.Create))//bin debug сохранение картинки в папке
                jpegBitmapEncoder.Save(fileStream);
                temp.ID = save_name;
                temp.UrlRecipe = helper.domainPath + @"image\" + save_name + ".png";//новый УРЛ
            }
            if(!helper.isChangeImage)//если картинка не менялась, берем из редактируемого рецепта, остальное берем из буферного
            {
                temp.ID = helper.recipe.ID;
                temp.UrlRecipe = helper.recipe.UrlRecipe;
            }
           
            temp.IDRecipeItem = helper.temp_ID;
            temp.NameResipe = RedNameRecipe.Text;
            foreach (var item in IngredientsBox.Items)//закидываем ингредиенты в лист ингредиентов
            {
                temp.ingredients.Add((item as ComboBoxItem).Content.ToString());
            }
            temp.Description = RedResipeText.Text;
           
            return temp;
        }
        private void OK_AddRecipe_Click(object sender, RoutedEventArgs e)//сохранение рецепта
        {
            if (helper.isAdd)//если находимся в режиме добавления нового рецепта
            {
                if (helper.path == "")
                {
                    MessageBox.Show("Загрузите картинку", "Ошибка загрузки",
             MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {                  
                    helper.recipes.Add(New_Recipe());
                    helper.isChangeImage = false;
                    MessageBox.Show("Рецепт добавлен", "",
           MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            if(helper.isRed)//редактирование существующего рецепта
            {
                Recipe temp = new Recipe();
                temp = New_Recipe();
                if (helper.isChangeImage)//была смена катринки
                {
                    if (System.IO.File.Exists(helper.del_path)) { File.Delete(helper.del_path); helper.del_path = ""; }//удаляем старую картинку
                }
                helper.all_recipes[helper.all_recipes.IndexOf(helper.recipe)] = temp;//находим нужный рецепт по буферному и заменяем рецепт в обсервал
                helper.recipes[helper.recipes.IndexOf(helper.recipe)] = temp;//заменяем рецепт в общем листе
            }
            helper.isChangeImage = false;
            RedRecipePanel.Visibility = Visibility.Hidden;
            ViewRecipePanel.Visibility = Visibility.Hidden;
            HidenGrid.Visibility = Visibility.Visible;
            helper.path = "";//обнуляем путь катринки
            helper.recipe = new Recipe();//обнуляем буферный рецепт
            Serial();

        }
        private void No_AddRecipe_Click(object sender, RoutedEventArgs e)//нажимаем на отмену и сбрасываем все что содержится в полях редактировния
        {
            if(helper.isAdd)//режим добавления
            {
                DownloadImage_Recipe.Content = "Загрузить";
                helper.path = "";
                RedNameRecipe.Text = "Название";
                TextIngredients.Text = "";
                IngredientsBox.Items.Clear();
                RedResipeText.Text = "Описание рецепта:";
            }
            else//режим редактирования
            {
                Red_Recipe_Click(sender, e);
            }

        }

        private void Recipe_MouseDown(object sender, MouseButtonEventArgs e)//по щелчку мыши отражаем рецепт на панели отображения
        {
            HidenGrid.Visibility = Visibility.Hidden;
            RedRecipePanel.Visibility = Visibility.Hidden;
            ViewRecipePanel.Visibility = Visibility.Visible;
            //панель принимает именно наш рецепт    НЕ КОПИЮ!!
            ViewRecipePanel.DataContext = (sender as FrameworkElement).DataContext;//связать панель отображения с нашим рецептом
            ViewIngredientsBox.ItemsSource = ((sender as FrameworkElement).DataContext as Recipe).ingredients;//связать отображение ингредиентов с комбобокс ингридиентов
        }

        private void Del_Recipe_Click(object sender, RoutedEventArgs e)//удаление рецепта
        {
            helper.del_path = ((sender as FrameworkElement).DataContext as Recipe).UrlRecipe;
            if (MessageBox.Show("Вы хотите удалить " + ((sender as FrameworkElement).DataContext as Recipe).NameResipe + "?", "...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                helper.recipes.Remove((sender as FrameworkElement).DataContext as Recipe);
                helper.all_recipes.Remove((sender as FrameworkElement).DataContext as Recipe);
                ViewRecipePanel.Visibility = Visibility.Hidden;
                HidenGrid.Visibility = Visibility.Visible;
                if (System.IO.File.Exists(helper.del_path)) { File.Delete(helper.del_path); helper.del_path = ""; }
            }
            Serial();
        }

        private void Red_Recipe_Click(object sender, RoutedEventArgs e)//редакторование рецепта
        {
            IngredientsBox.Items.Clear();//очищаем ингридиенты во вьюшке
            helper.recipe = AllRecipe.SelectedItem as Recipe;//инициализируем буферный рецепт тем что мы хотим редактировать
            Image im = new Image();
            im.Source = SetBitmap(helper.recipe.UrlRecipe);
            DownloadImage_Recipe.Content = im;
            RedNameRecipe.Text = helper.recipe.NameResipe;
            foreach (var item in helper.recipe.ingredients)
            {
                ComboBoxItem item_combo = new ComboBoxItem();
                item_combo.Content = item;
                IngredientsBox.Items.Add(item_combo);
            }
            RedResipeText.Text = helper.recipe.Description;
            ViewRecipePanel.Visibility = Visibility.Hidden;
            RedRecipePanel.Visibility = Visibility.Visible;
            helper.isRed = true;
            helper.isAdd = false;

        }
        private void MouseDoubleClick_Item(object sender, MouseButtonEventArgs e)//сортировка по разделам
        {
            helper.all_recipes.Clear();
            string var = (RecipeList.SelectedItem as Recipe_Item).ID_image;
            foreach (var item in helper.recipes)
            {
                if (item.IDRecipeItem == var) helper.all_recipes.Add(item);
            }
            RecipeList.Visibility = Visibility.Hidden;
            AllRecipe.Visibility = Visibility.Visible;
            AddSection.Visibility = Visibility.Hidden;
        }
    }
}
