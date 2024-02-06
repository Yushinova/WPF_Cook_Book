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
        public ObservableCollection<Recipe_Item> recipe_Items = new ObservableCollection<Recipe_Item>();//с обычным листом так просто не получится сделать связку
        public ObservableCollection<Recipe> all_recipes = new ObservableCollection<Recipe>();//Список рецептов при выборке по разделу будет заполняться в соответствие с выборой
        public List<Recipe> recipes = new List<Recipe>();//обычный лист для хранения всех рецептов
        public Recipe recipe = new Recipe();
        public string path = "";
        public bool isChangeImage = false;
        public bool isAdd = false;
        public bool isRed = false;
        //public int IndexItem { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            if (System.IO.File.Exists("sections.xml"))
            {
                Deserial();
            }
            else
            {
                recipe_Items = new ObservableCollection<Recipe_Item>//заводские разделы меню
                {
                new Recipe_Item{ID_image="1", Name= "Первые блюда", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\1.png"))},
                new Recipe_Item{ID_image="2", Name= "Вторые блюда", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\2.png"))},
                new Recipe_Item{ID_image="3", Name= "Салаты", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\3.png"))},
                new Recipe_Item{ID_image="5", Name= "Завтрак", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\5.png"))},
                new Recipe_Item{ID_image="6", Name= "Десерты", myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\6.png"))},
                };
                recipes = new List<Recipe>
                {
                    new Recipe{ID="1", IDRecipeItem="1", NameResipe="Борщ", ImageRecipe=new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\1.png")), ingredients={"капуста","картофель","лук" }, Description="Положите одно, потом другое и третье" },
                    new Recipe{ID="2", IDRecipeItem="1", NameResipe="Суп", ImageRecipe=new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\2.png")), ingredients={"курица","картофель","лук" }, Description="Отварите курицу, пассируйте лук и морковь" },
                };
            }
            AllRecipe.ItemsSource = all_recipes;
            RecipeList.ItemsSource = recipe_Items;
        }
        private void Serial()
        {
            List<Recipe_Item> recipe_ = new List<Recipe_Item>();
            foreach (var item in recipe_Items)
            {
                Recipe_Item temp = new Recipe_Item { ID_image = item.ID_image, Name = item.Name };
                recipe_.Add(temp);
            }
            XmlSerializer bf = new XmlSerializer(typeof(List<Recipe_Item>));
            using (Stream fstr = File.Create("sections.xml"))
            {
                bf.Serialize(fstr, recipe_);
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
            Text1.Text = recipe_.Count.ToString();
            foreach (var item in recipe_)
            {
                ImageSource temp_im = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + item.ID_image + @".png"));
                recipe_Items.Add(new Recipe_Item { ID_image = item.ID_image, Name = item.Name, myImg = temp_im });
            }
        }
        //РАБОТА С РАЗДЕЛАМИ КУЛИНАРНОЙ КНИГИ
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
            //var item = (sender as FrameworkElement).DataContext;//получаем объект по которому жмякнули
            //IndexItem = RecipeList.Items.IndexOf(item);//запоминаем индекс по которому кликнули
            //Text1.Text = IndexItem.ToString();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)//удаление раздела с рецептами
        {
            if (MessageBox.Show("Вы хотите удалить " + (RecipeList.SelectedItem as Recipe_Item).Name + "?", "...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string str_del = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + (RecipeList.SelectedItem as Recipe_Item).ID_image + ".png";//запоминаем путь картинки которую нужно удалить
                Text1.Text = str_del;
               
                recipe_Items.Remove(RecipeList.SelectedItem as Recipe_Item);

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
            dialog.Filter = "Image (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png"; // Filter files by extension

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
        private Recipe_Item New_Resipe_Item()//Новый раздел рецептов
        {
            string save_name = Guid.NewGuid().ToString();//халабудим уникальный идентификатор название картинки
            BitmapImage save = new BitmapImage(new Uri(path));
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();//сохранение картинки в папке программы
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(save));
            using (FileStream fileStream = new FileStream(@"image\" + save_name + ".png", FileMode.Create))//bin debug
            jpegBitmapEncoder.Save(fileStream);

            Recipe_Item item = new Recipe_Item { ID_image = save_name, Name = RedText.Text, myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + save_name + ".png")) };
            return item;
        }
        private void Add_Item_Click(object sender, RoutedEventArgs e)//кнопка Ок при добавлении/редактировании нового раздела
        {
            if (isAdd)//если нажали на добаление
            {
                if (path == "")
                {
                    MessageBox.Show("Загрузите иконку", "Ошибка загрузки",
             MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    recipe_Items.Add(New_Resipe_Item());
                    RedPanel.Visibility = Visibility.Hidden;
                    isChangeImage = false;//сбрасываем ключ загрузки изображения
                    //Serial();
                }
            }
            if (isRed)//если нажали на редактирование
            {
                if (isChangeImage)//если мы решили сменить картинку
                {
                    recipe_Items[RecipeList.Items.IndexOf(RecipeList.SelectedItem)] = New_Resipe_Item();
                    //recipe_Items.RemoveAt(RecipeList.Items.IndexOf(RecipeList.SelectedItem));//удаляем неотредактированный раздел
                    //recipe_Items.Insert(RecipeList.Items.IndexOf(RecipeList.SelectedItem), New_Resipe_Item());//вставляем на нужную позицию отредактированный раздел
                    RedPanel.Visibility = Visibility.Hidden;
                    isChangeImage = false;
                }
                else//если картинку не меняли а поменяли название раздела
                {
                    string save_name = (RecipeList.SelectedItem as Recipe_Item).ID_image;//оставляем картинку и путь к каритнке тот же самый
                    Recipe_Item item = new Recipe_Item { ID_image = save_name, Name = RedText.Text, myImg = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + save_name + ".png")) };
                    //recipe_Items.RemoveAt(RecipeList.Items.IndexOf(RecipeList.SelectedItem));
                    //recipe_Items.Insert(RecipeList.Items.IndexOf(RecipeList.SelectedItem), item);
                    recipe_Items[RecipeList.Items.IndexOf(RecipeList.SelectedItem)] = item;
                    RedPanel.Visibility = Visibility.Hidden;
                    isChangeImage = false;
                }
            }
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
        private void Redact_Click(object sender, RoutedEventArgs e)//Нажатие Редактировать на контекстном меню попробовать биндинг
        {
            RedPanel.Visibility = Visibility.Visible;
            Image image = new Image();//на нашу кнопку для визуализации добавляем картинку
            image.Source = (RecipeList.SelectedItem as Recipe_Item).myImg;
            DownLoadImage.Content = image;
            RedText.Text = (RecipeList.SelectedItem as Recipe_Item).Name;
            isRed = true;//активируем триггер редактирования
            isAdd = false;//добавление сбросить
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
            all_recipes.Clear();
            foreach (var item in recipes)
            {
                all_recipes.Add(item);
            }
            AllRecipe.Visibility = Visibility.Visible;
            AddSection.Visibility = Visibility.Hidden;
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)//нажатие на плюс рядом с разделом (добавление нового раздела)
        {
            HidenGrid.Visibility = Visibility.Hidden;
            ViewRecipePanel.Visibility = Visibility.Hidden;
            RedRecipePanel.Visibility = Visibility.Visible;
            var item = (sender as FrameworkElement).DataContext;//получаем объект по которому жмякнули
            int ind = RecipeList.Items.IndexOf(item);//запоминаем индекс по которому кликнули
            recipe = new Recipe(); //инициализируем обнуляем наш буферный рецепт
            if (ind!=-1) { recipe.IDRecipeItem = recipe_Items[ind].ID_image; }//даем ему ID  выбранного раздела
            isAdd = true;
            isRed = false;
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
                path = f.FullName;
                // Path = dialog.FileName;
            }
            Image image = new Image();//на нашу кнопку для визуализации добавляем картинку
            image.Source = new BitmapImage(new Uri(path));
            DownloadImage_Recipe.Content = image;
            isChangeImage = true;
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
            string save_name = Guid.NewGuid().ToString();//халабудим уникальный идентификатор название картинки
            BitmapImage save = new BitmapImage(new Uri(path));
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();//сохранение картинки в папке программы
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(save));
            using (FileStream fileStream = new FileStream(@"image\" + save_name + ".png", FileMode.Create))//bin debug
            jpegBitmapEncoder.Save(fileStream);
            temp.ID = save_name;
            temp.NameResipe = RedNameRecipe.Text;
            temp.ImageRecipe = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"image\" + save_name + ".png"));
            foreach (var item in IngredientsBox.Items)//закидываем ингредиенты в лист ингредиентов
            {
                temp.ingredients.Add((item as ComboBoxItem).Content.ToString());
            }
            temp.Description = RedResipeText.Text;
           
            return temp;
        }
        private void OK_AddRecipe_Click(object sender, RoutedEventArgs e)//сохранение рецепта
        {
            int ind = AllRecipe.SelectedIndex;//индекс на вьюшке
            Recipe temp = new Recipe();
            if (isAdd)
            {
                if (path == "")
                {
                    MessageBox.Show("Загрузите картинку", "Ошибка загрузки",
             MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {                  
                    recipes.Add(New_Recipe());
                    isChangeImage = false;
                }
            }
            if(isRed)
            {
                if(isChangeImage)
                {
                    temp = New_Recipe();
                    
                    //recipes.Remove(recipe);
                    //recipes.Add(temp);
                }
                else
                {
                    temp = recipe;
                    temp.NameResipe = RedNameRecipe.Text;
                    temp.Description = RedResipeText.Text;
                    temp.ingredients.Clear();
                    foreach (var item in IngredientsBox.Items)//закидываем ингредиенты в лист ингредиентов
                    {
                        temp.ingredients.Add((item as ComboBoxItem).Content.ToString());
                    }

                }
                all_recipes[ind] = temp;
                recipes[recipes.IndexOf(recipe)] = temp;
                isChangeImage = false;
            }
          
        }
        private void No_AddRecipe_Click(object sender, RoutedEventArgs e)//нажимаем на отмену и сбрасываем все что содержится в полях редактировния
        {
            if(isAdd)
            {
                DownloadImage_Recipe.Content = "Загузить";
                path = "";
                RedNameRecipe.Text = "Название";
                TextIngredients.Text = "";
                IngredientsBox.Items.Clear();
                RedResipeText.Text = "Описание";
            }
            else
            {
                Red_Recipe_Click(sender, e);
            }

        }

        private void Recipe_MouseDown(object sender, MouseButtonEventArgs e)//по щелчку мыши отражаем рецепт на панели отображения
        {
            HidenGrid.Visibility = Visibility.Hidden;
            RedRecipePanel.Visibility = Visibility.Hidden;
            ViewRecipePanel.Visibility = Visibility.Visible;
            //recipe = new Recipe();
            //var item = (sender as FrameworkElement).DataContext;//получаем объект по которому жмякнули
            //int ind = AllRecipe.Items.IndexOf(item);//запоминаем индекс по которому кликнули
            //recipe = recipes[ind];
            //панель принимает именно наш рецепт    НЕ КОПИЮ!!
            ViewRecipePanel.DataContext = (sender as FrameworkElement).DataContext;//связать панель отображения с нашим рецептом
            ViewIngredientsBox.ItemsSource = ((sender as FrameworkElement).DataContext as Recipe).ingredients;//связать отображение ингредиентов с комбобокс ингридиентов
            //Text1.Text = recipe.NameResipe;
        }

        private void Del_Recipe_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы хотите удалить " + ((sender as FrameworkElement).DataContext as Recipe).NameResipe + "?", "...", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                recipes.Remove((sender as FrameworkElement).DataContext as Recipe);
                all_recipes.Remove((sender as FrameworkElement).DataContext as Recipe);
                ViewRecipePanel.Visibility = Visibility.Hidden;
                HidenGrid.Visibility = Visibility.Visible;
            }
        }

        private void Red_Recipe_Click(object sender, RoutedEventArgs e)//
        {
            IngredientsBox.Items.Clear();
            recipe = AllRecipe.SelectedItem as Recipe;//инициализируем буферный рецепт тем что мы хотим редактировать
           // Recipe temp = new Recipe();//
            Image im = new Image();
            im.Source = recipe.ImageRecipe;
            DownloadImage_Recipe.Content = im;
            RedNameRecipe.Text = recipe.NameResipe;
            foreach (var item in recipe.ingredients)
            {
                ComboBoxItem item_combo = new ComboBoxItem();
                item_combo.Content = item;
                IngredientsBox.Items.Add(item_combo);
            }
            RedResipeText.Text = recipe.Description;
            Text1.Text = IngredientsBox.Items.Count.ToString();
            ViewRecipePanel.Visibility = Visibility.Hidden;
            RedRecipePanel.Visibility = Visibility.Visible;
            isRed = true;
            isAdd = false;

        }
    }
}
