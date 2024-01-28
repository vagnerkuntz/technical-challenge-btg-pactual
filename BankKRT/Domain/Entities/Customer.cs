namespace BankKRT.Domain.Entities
{
    public class Customer
    {
        public string Document { get; set; }
        public int NumberAgency { get; set; }
        public int NumberAccount { get; set; }
        public decimal LimitPix { get; set; }
    }
}
