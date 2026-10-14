using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MunicipalServiceApp
{
    public partial class LocalEventsForm : Form
    {
        private List<LocalEvent> currentResults = new List<LocalEvent>();

        public LocalEventsForm()
        {
            InitializeComponent();
            WireEvents();
            InitialiseFilters();
            LocalEventRepository.PrepareUpcomingQueue();
            DisplayEvents(LocalEventRepository.GetUpcomingQueueSnapshot());
            DisplayRecommendations();
            UpdateSearchHistory();
        }

        private void WireEvents()
        {
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnBack.Click += btnBack_Click;
            dgvEvents.SelectionChanged += dgvEvents_SelectionChanged;
        }

        private void InitialiseFilters()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");

            foreach (string category in LocalEventRepository.Categories.OrderBy(c => c))
                cmbCategory.Items.Add(category);

            cmbCategory.SelectedIndex = 0;
            dtpFrom.Checked = false;
            dtpTo.Checked = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchEvents(true);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbCategory.SelectedIndex = 0;
            dtpFrom.Checked = false;
            dtpTo.Checked = false;
            SearchEvents(false);
        }

        private void SearchEvents(bool recordSearch)
        {
            DateTime? fromDate = dtpFrom.Checked ? dtpFrom.Value.Date : (DateTime?)null;
            DateTime? toDate = dtpTo.Checked ? dtpTo.Value.Date : (DateTime?)null;

            if (fromDate.HasValue && toDate.HasValue && fromDate.Value.Date > toDate.Value.Date)
            {
                MessageBox.Show("The From date cannot be after the To date.",
                    "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string category = cmbCategory.SelectedIndex > 0 ? cmbCategory.SelectedItem.ToString() : string.Empty;
            string searchText = txtSearch.Text.Trim();

            IEnumerable<LocalEvent> results = LocalEventRepository.GetEventsFromDateIndex()
                .Where(e => e.EventDate.Date >= DateTime.Today);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                results = results.Where(e =>
                    e.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Category.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Description.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Location.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(category))
                results = results.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            if (fromDate.HasValue)
                results = results.Where(e => e.EventDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                results = results.Where(e => e.EventDate.Date <= toDate.Value.Date);

            currentResults = results
                .OrderBy(e => e.EventDate)
                .ThenByDescending(e => e.Priority)
                .ToList();

            if (recordSearch)
            {
                LocalEventRepository.RecordSearch(searchText, category);
                DisplayRecommendations();
                UpdateSearchHistory();
            }

            DisplayEvents(currentResults);
        }

        private void DisplayEvents(IEnumerable<LocalEvent> events)
        {
            dgvEvents.Rows.Clear();

            foreach (LocalEvent localEvent in events)
            {
                dgvEvents.Rows.Add(
                    localEvent.EventDate.ToString("dd MMM yyyy"),
                    localEvent.Type,
                    localEvent.Title,
                    localEvent.Category,
                    localEvent.Location,
                    localEvent.Priority);

                dgvEvents.Rows[dgvEvents.Rows.Count - 1].Tag = localEvent;
            }

            lblStatus.Text = "Events displayed: " + dgvEvents.Rows.Count +
                             " | Categories available: " + LocalEventRepository.Categories.Count +
                             " | Dates indexed: " + LocalEventRepository.EventDates.Count;
        }

        private void DisplayRecommendations()
        {
            string category = cmbCategory.SelectedIndex > 0 ? cmbCategory.SelectedItem.ToString() : string.Empty;
            DateTime? fromDate = dtpFrom.Checked ? dtpFrom.Value.Date : (DateTime?)null;
            DateTime? toDate = dtpTo.Checked ? dtpTo.Value.Date : (DateTime?)null;

            List<LocalEvent> recommendations = RecommendationService.GetRecommendations(
                txtSearch.Text.Trim(), category, fromDate, toDate);

            lstRecommendations.Items.Clear();

            if (recommendations.Count == 0)
            {
                lstRecommendations.Items.Add("No recommendations found. Try a broader search.");
                return;
            }

            foreach (LocalEvent localEvent in recommendations)
            {
                lstRecommendations.Items.Add(string.Format(
                    "{0:dd MMM} - {1} ({2})",
                    localEvent.EventDate,
                    localEvent.Title,
                    localEvent.Category));
            }
        }

        private void UpdateSearchHistory()
        {
            lstSearchHistory.Items.Clear();

            foreach (string search in LocalEventRepository.SearchHistory.Reverse())
                lstSearchHistory.Items.Add(search);
        }

        private void dgvEvents_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEvents.SelectedRows.Count == 0)
                return;

            LocalEvent selectedEvent = dgvEvents.SelectedRows[0].Tag as LocalEvent;
            if (selectedEvent == null)
                return;

            // Stack records the most recently viewed events (LIFO).
            LocalEventRepository.RecentlyViewed.Push(selectedEvent);
            lblRecentViewed.Text = selectedEvent.Title + Environment.NewLine +
                                   selectedEvent.EventDate.ToString("dd MMM yyyy") + " | " +
                                   selectedEvent.Location + Environment.NewLine +
                                   "Priority: " + selectedEvent.Priority;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
