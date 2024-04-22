using FluentValidation;
namespace UWEServer.Requests
{
    public class CategoryTransfer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
