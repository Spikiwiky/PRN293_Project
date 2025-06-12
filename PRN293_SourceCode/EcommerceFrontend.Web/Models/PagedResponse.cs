namespace EcommerceFrontend.Web.Models
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
