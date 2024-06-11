namespace MeowStore
{
    public class Product
    {
        public int IdProduct { get; set; }
        public int IdCategory { get; set; }
        public string NameProduct { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public string PathImage { get; set; }

        public Category Category { get; set; }

        public double Sum { get; set; }
    }
}
