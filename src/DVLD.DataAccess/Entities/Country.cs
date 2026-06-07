using System.Text;

namespace DVLD.DataAccess.Entities
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;

        public ICollection<Person> People { get; set; } = new List<Person>();
    }
}
