using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServiceApp
{
    internal static class LocalEventRepository
    {
        public static List<LocalEvent> Events { get; private set; }
        public static Dictionary<int, LocalEvent> EventLookup { get; private set; }
        public static SortedDictionary<DateTime, List<LocalEvent>> EventsByDate { get; private set; }
        public static HashSet<string> Categories { get; private set; }
        public static HashSet<DateTime> EventDates { get; private set; }
        public static Queue<LocalEvent> UpcomingEvents { get; private set; }
        public static Stack<LocalEvent> RecentlyViewed { get; private set; }
        public static EventPriorityQueue PriorityEvents { get; private set; }
        public static Dictionary<string, int> SearchFrequency { get; private set; }
        public static Queue<string> SearchHistory { get; private set; }

        static LocalEventRepository()
        {
            Events = new List<LocalEvent>();
            EventLookup = new Dictionary<int, LocalEvent>();
            EventsByDate = new SortedDictionary<DateTime, List<LocalEvent>>();
            Categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            EventDates = new HashSet<DateTime>();
            UpcomingEvents = new Queue<LocalEvent>();
            RecentlyViewed = new Stack<LocalEvent>();
            PriorityEvents = new EventPriorityQueue();
            SearchFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            SearchHistory = new Queue<string>();

            SeedEvents();
        }

        private static void SeedEvents()
        {
            DateTime baseDate = DateTime.Today;

            AddEvent(new LocalEvent
            {
                EventId = 1001,
                Title = "Community Clean-Up Day",
                Category = "Waste Management",
                EventDate = baseDate.AddDays(5),
                Location = "Central Community Hall",
                Description = "Residents can join the municipality in a neighbourhood clean-up and waste awareness activity.",
                Priority = 3,
                Type = "Event"
            });

            AddEvent(new LocalEvent
            {
                EventId = 1002,
                Title = "Water Services Public Meeting",
                Category = "Water",
                EventDate = baseDate.AddDays(8),
                Location = "Municipal Council Chamber",
                Description = "Public meeting covering planned water maintenance, interruptions and service delivery updates.",
                Priority = 1,
                Type = "Announcement"
            });

            AddEvent(new LocalEvent
            {
                EventId = 1003,
                Title = "Local Business Support Workshop",
                Category = "Other",
                EventDate = baseDate.AddDays(12),
                Location = "Economic Development Centre",
                Description = "A workshop for local entrepreneurs covering municipal support programmes and registration information.",
                Priority = 2,
                Type = "Event"
            });

            AddEvent(new LocalEvent
            {
                EventId = 1004,
                Title = "Electricity Maintenance Notice",
                Category = "Electricity",
                EventDate = baseDate.AddDays(15),
                Location = "Ward 7",
                Description = "Planned electrical maintenance will affect selected streets. Residents are encouraged to prepare in advance.",
                Priority = 2,
                Type = "Announcement"
            });

            AddEvent(new LocalEvent
            {
                EventId = 1005,
                Title = "Youth Development Day",
                Category = "Other",
                EventDate = baseDate.AddDays(20),
                Location = "Municipal Sports Centre",
                Description = "Information sessions and activities focused on youth opportunities, skills and community programmes.",
                Priority = 4,
                Type = "Event"
            });

            AddEvent(new LocalEvent
            {
                EventId = 1006,
                Title = "Road Safety Awareness Campaign",
                Category = "Roads",
                EventDate = baseDate.AddDays(24),
                Location = "Civic Centre Parking Area",
                Description = "A public road safety campaign with municipal officials and community partners.",
                Priority = 4,
                Type = "Event"
            });

            AddEvent(new LocalEvent
            {
                EventId = 1007,
                Title = "Recycling Information Session",
                Category = "Santation",
                EventDate = baseDate.AddDays(28),
                Location = "Green Park Library",
                Description = "Residents can learn about household recycling, waste separation and collection services.",
                Priority = 3,
                Type = "Event"
            });

            AddEvent(new LocalEvent
            {
                EventId = 1008,
                Title = "Ward Committee Community Forum",
                Category = "Street Lights",
                EventDate = baseDate.AddDays(32),
                Location = "Ward 3 Community Centre",
                Description = "Residents can raise local service delivery matters and hear updates from the ward committee.",
                Priority = 4,
                Type = "Event"
            });
        }

        private static void AddEvent(LocalEvent localEvent)
        {
            Events.Add(localEvent);
            EventLookup[localEvent.EventId] = localEvent;
            Categories.Add(localEvent.Category);
            EventDates.Add(localEvent.EventDate.Date);
            PriorityEvents.Enqueue(localEvent);

            if (!EventsByDate.ContainsKey(localEvent.EventDate.Date))
                EventsByDate[localEvent.EventDate.Date] = new List<LocalEvent>();

            EventsByDate[localEvent.EventDate.Date].Add(localEvent);
        }

        public static void PrepareUpcomingQueue()
        {
            UpcomingEvents.Clear();

            foreach (LocalEvent localEvent in Events
                .Where(e => e.EventDate.Date >= DateTime.Today)
                .OrderBy(e => e.EventDate)
                .ThenByDescending(e => e.Priority))
            {
                UpcomingEvents.Enqueue(localEvent);
            }
        }


        public static List<LocalEvent> GetUpcomingQueueSnapshot()
        {
            return UpcomingEvents.ToList();
        }

        public static List<LocalEvent> GetEventsFromDateIndex()
        {
            List<LocalEvent> result = new List<LocalEvent>();

            foreach (KeyValuePair<DateTime, List<LocalEvent>> entry in EventsByDate)
                result.AddRange(entry.Value);

            return result;
        }

        public static void RecordSearch(string searchTerm, string category)
        {
            string key = !string.IsNullOrWhiteSpace(category) ? category.Trim() : searchTerm.Trim();
            if (string.IsNullOrWhiteSpace(key))
                key = "All Events";

            if (!SearchFrequency.ContainsKey(key))
                SearchFrequency[key] = 0;

            SearchFrequency[key]++;
            SearchHistory.Enqueue(key);

            while (SearchHistory.Count > 5)
                SearchHistory.Dequeue();
        }
    }
}
