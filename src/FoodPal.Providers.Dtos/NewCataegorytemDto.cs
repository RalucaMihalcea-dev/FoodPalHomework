using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPal.Providers.Dtos
{
    class NewCataegorytemDto
    {
        [MaxLength(150)]
        [Required]
        public string Name { get; set; }

        [RegularExpression(@"[0-9]|{}")]
        [Required]
        public decimal Price { get; set; }

        public int CatalogueId { get; set; }
        public int CategoryId { get; set; }

    }
}
