namespace UWEServer.Responses
{
    public class ZoneKioskResponse
    {
        public int Id { get; set; }
        public string ZoneId { get; set; }
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public List<ZoneObject> ObjectList { get; set; }
    }

    public class ZoneObject
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string ObjectId { get; set; }
        public string Name { get; set; }
        public double Front { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public virtual double Width { get; set; }
        public virtual double Height { get; set; }
        public virtual string Color { get; set; }
        public virtual string Category { get; set; }
    }
}