namespace UWEServer.Model
{
    public class PageResponse<T>
    {
        public PageResponse()
        {
        }
        public PageResponse(List<T> data)
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }
        public List<T> Data { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
    }
}
