using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreoPOS.Core.EntityLayer
{
   public class Dish
    {
        [Key]
        public int DishID { get; set; }
        public string Name { get; set; }
    }
}
