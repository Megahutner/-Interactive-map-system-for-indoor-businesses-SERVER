namespace UWEServer.Responses
{
    public class ZoneDetailsResponse
    {
        public int Id { get; set; }
        public string? ZoneID { get; set; }
        public string? Name { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
