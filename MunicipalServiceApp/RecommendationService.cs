using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServiceApp
{
    internal static class RecommendationService
    {
        public static List<LocalEvent> GetRecommendations(string searchText, string category, DateTime? fromDate, DateTime? toDate)
        {
            string query = (searchText ?? string.Empty).Trim();
            string selectedCategory = (category ?? string.Empty).Trim();

            Dictionary<string, int> frequency = LocalEventRepository.SearchFrequency;
            int categoryFrequency = 0;
            if (!string.IsNullOrWhiteSpace(selectedCategory) && frequency.ContainsKey(selectedCategory))
                categoryFrequency = frequency[selectedCategory];

            HashSet<string> queryTerms = new HashSet<string>(
                query.Split(new[] { ' ', ',', '.', '-', '/' }, StringSplitOptions.RemoveEmptyEntries),
                StringComparer.OrdinalIgnoreCase);

            IEnumerable<LocalEvent> candidates = LocalEventRepository.PriorityEvents.ToSortedList()
                .Where(e => e.EventDate.Date >= DateTime.Today);

            if (fromDate.HasValue)
                candidates = candidates.Where(e => e.EventDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                candidates = candidates.Where(e => e.EventDate.Date <= toDate.Value.Date);

            List<RecommendationScore> scored = new List<RecommendationScore>();

            foreach (LocalEvent localEvent in candidates)
            {
                int score = 0;

                if (!string.IsNullOrWhiteSpace(selectedCategory) &&
                    localEvent.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase))
                {
                    score += 50 + (categoryFrequency * 10);
                }

                string searchable = (localEvent.Title + " " + localEvent.Category + " " +
                                     localEvent.Description + " " + localEvent.Location).ToLowerInvariant();

                foreach (string term in queryTerms)
                {
                    if (searchable.Contains(term.ToLowerInvariant()))
                        score += 15;
                }

                score += localEvent.Priority * 5;

                int daysAway = Math.Max(0, (localEvent.EventDate.Date - DateTime.Today).Days);
                score += Math.Max(0, 20 - Math.Min(daysAway, 20));

                scored.Add(new RecommendationScore { Event = localEvent, Score = score });
            }

            return scored
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Event.EventDate)
                .Take(3)
                .Select(x => x.Event)
                .ToList();
        }

        private class RecommendationScore
        {
            public LocalEvent Event { get; set; }
            public int Score { get; set; }
        }
    }
}
