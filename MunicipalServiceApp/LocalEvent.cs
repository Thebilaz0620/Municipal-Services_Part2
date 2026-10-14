using System;

namespace MunicipalServiceApp
{
    internal class LocalEvent
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("{0:dd MMM yyyy} - {1}", EventDate, Title);
        }
    }
}
