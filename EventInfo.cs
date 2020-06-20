namespace qsrv
{
    public class EventInfo
    {
        public readonly string EventId;
        public readonly string Title;
        public readonly int ExpirationDate;
        public readonly string Date;
        public readonly string Time;
        public readonly string Location;
        public readonly string Url;
        public readonly string Image;
        public readonly string Description;

        public EventInfo(string eventId, string title, int expirationDate, string date, string time, string location, string url, string image, string description)
        {
            EventId = eventId;
            Title = title;
            ExpirationDate = expirationDate;
            Date = date;
            Time = time;
            Location = location;
            Url = url;
            Image = image;
            Description = description;
        }
    }
}