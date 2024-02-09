using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace new_cook_book
{
    [Serializable]
    public class Recipe_Item
    {
        public string ID_image { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
    [Serializable]
    public class Recipe
    {
        public string IDRecipeItem { get; set; }
        public string ID { get; set; }//для связки с каритнкой
        public string NameResipe { get; set; }
        public string UrlRecipe { get; set; }
        public List<string> ingredients = new List<string>();
        public string Description { get; set; }
        public List<string> GetIngredients() { return ingredients; }
    }
}
