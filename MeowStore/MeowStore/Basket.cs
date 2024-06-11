namespace MeowStore
{
    public class Basket
    {
        public int IdBasket { get; set; }
        public int IdCustomer { get; set; }
        public int IdStatus { get; set; }
        public int IdPoint { get; set; }
        public Status Status { get; set; }
        public Customer Customer { get; set; }
        public Point Point { get; set; }
    }
}
