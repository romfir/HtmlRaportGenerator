using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HtmlRaportGenerator.Shared.Models
{
    public class DaysCollectionValidationModel
    {
        [ValidateComplexType]
        public List<Day> Days { get; set; } = null!; //= new List<Day>();
    }
}
