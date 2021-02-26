using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HtmlRaportGenerator.Models
{
    public class DaysCollectionValidationModel
    {
        [ValidateComplexType]
        public List<Day> Days { get; set; } = null!; //= new List<Day>();
    }
}
