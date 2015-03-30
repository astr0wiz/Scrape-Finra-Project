
namespace ScrapeFinra.Models
{
    class Rating
    {
        public string ratingText { get; set; }
        public int ratingNumber { get; set; }

        public Rating()
        {
            ratingText = "";
            ratingNumber = 0;
        }
    }

}
