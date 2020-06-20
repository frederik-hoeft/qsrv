namespace qsrv
{
    public class Event
    {
        public readonly EventInfo EventInfo;
        public readonly string UserId;

        public Event(EventInfo eventInfo, string userid)
        {
            EventInfo = eventInfo;
            UserId = userid;
        }
    }
}