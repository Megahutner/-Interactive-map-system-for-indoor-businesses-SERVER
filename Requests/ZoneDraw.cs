namespace UWEServer.Requests
{
    public class ZoneDraw
    {
        public int Id { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public List<ZoneObject> ObjectList { get; set; }
    }

    public class ZoneObject
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string? ObjectId { get; set; }
        public string Name { get; set; }
        public int Front { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public virtual double Width { get; set; }
        public virtual double Height { get; set; }
        public virtual string? Color { get; set; }
        public virtual int CategoryId { get; set; }
        public virtual int ZoneLinkId { get; set; }
    }
}
