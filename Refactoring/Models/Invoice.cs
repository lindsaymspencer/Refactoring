namespace Refactoring.Models
{
    public class Invoice
    {
        public string Customer { get; set; }
        public Performance[] Performances { get; set; }
    }
}