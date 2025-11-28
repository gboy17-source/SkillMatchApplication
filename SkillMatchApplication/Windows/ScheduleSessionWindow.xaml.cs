using Newtonsoft.Json;
using SkillMatchApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SkillMatchApplication.Windows
{
    /// <summary>
    /// Interaction logic for ScheduleSessionWindow.xaml
    /// </summary>
    public partial class ScheduleSessionWindow : Window
    {
        private readonly ApiClient apiClient = new ApiClient();

        // Caller should set these before showing the window:
        public string TutorId { get; set; }
        
        public string LearnerId { get; set; }

        // Optional: back-end numeric skill id. If not provided we will send skill name instead.
        public int? SkillId { get; set; }

        // Optional list of skill names to populate the UI (e.g. ["Python","Java"])
        public List<string> SkillOptions { get; set; }

        // Optional parallel list of ids for SkillOptions (indexes match SkillOptions)
        public List<int> SkillOptionIds { get; set; }

        public ScheduleSessionWindow()
        {
            InitializeComponent();

            this.Loaded += ScheduleSessionWindow_Loaded;
        }

        private void ScheduleSessionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate skill combo if SkillOptions provided
            if (SkillOptions != null && SkillOptions.Any())
            {
                skillCombo.ItemsSource = SkillOptions;
                skillCombo.SelectedIndex = 0;
            }

            // If TutorId/SkillId are missing we still allow user to type/select values.
        }

        // Make the handler async so we can await ApiClient.PostJson
        private async void btnRequestSession_Click(object sender, RoutedEventArgs e)
        {
            // Ensure JWT is used
            if (!string.IsNullOrEmpty(Session.JwtToken))
                apiClient.SetJwt(Session.JwtToken);

            if (!datePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var date = datePicker.SelectedDate.Value.Date;

            // Get time text - use selected item or editable text
            string timeText = null;
            if (timeCombo.SelectedItem is ComboBoxItem cbi)
                timeText = cbi.Content?.ToString();
            else if (!string.IsNullOrWhiteSpace(timeCombo.Text))
                timeText = timeCombo.Text;

            if (string.IsNullOrWhiteSpace(timeText))
            {
                MessageBox.Show("Please select a time.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse time (e.g. "2:00 PM") into TimeSpan / 24-hour format
            if (!DateTime.TryParse(timeText, out DateTime parsedTime))
            {
                MessageBox.Show("Couldn't parse the selected time. Use format like '2:00 PM'.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dateString = date.ToString("yyyy-MM-dd");
            var timeString = new DateTime(date.Year, date.Month, date.Day, parsedTime.Hour, parsedTime.Minute, parsedTime.Second)
                                 .ToString("HH:mm:ss");

            if (string.IsNullOrWhiteSpace(TutorId))
            {
                MessageBox.Show("Tutor not specified. Please try again.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Determine selected skill name and/or id
            string selectedSkillName = null;
            int? selectedSkillId = SkillId; // may be prefilled by caller

            // If the UI was populated from SkillOptions/SkillOptionIds, pick the id by index
            if (SkillOptionIds != null && SkillOptionIds.Any() && skillCombo.SelectedIndex >= 0)
            {
                var idx = skillCombo.SelectedIndex;
                if (idx < SkillOptionIds.Count)
                {
                    selectedSkillId = SkillOptionIds[idx];
                }
            }

            if (skillCombo.SelectedItem is string s)
                selectedSkillName = s;
            else if (skillCombo.SelectedItem is ComboBoxItem selCbi)
                selectedSkillName = selCbi.Content?.ToString();
            else if (!string.IsNullOrWhiteSpace(skillCombo.Text))
                selectedSkillName = skillCombo.Text.Trim();

            // Build payload. Prefer sending skill_id if we have it, otherwise send skill name.
            var payload = new Dictionary<string, object>
            {
                ["tutor_id"] = TutorId,
                ["date"] = dateString,
                ["time"] = timeString,
                ["status"] = "pending"
            };

            if (selectedSkillId.HasValue && selectedSkillId.Value > 0)
            {
                payload["skill_id"] = selectedSkillId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(selectedSkillName))
            {
                // Some backends accept a skill name; include as fallback under "skill_name".
                // If your backend strictly requires numeric skill_id you should map skill names to ids before calling this window.
                payload["skill_name"] = selectedSkillName;
            }

            string json = JsonConvert.SerializeObject(payload);

            try
            {
                var response = await apiClient.PostJson("/api/sessions", json);

                MessageBox.Show("Session request sent.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Request failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
