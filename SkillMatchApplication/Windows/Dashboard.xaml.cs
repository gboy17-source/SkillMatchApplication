using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkillMatchApplication.Models;
using SkillMatchApplication.Services;
using SkillMatchApplication.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkillMatchApplication
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, INotifyPropertyChanged
    {
        private readonly ApiClient apiClient = new ApiClient();

        private Stack<Grid> history = new Stack<Grid>();
        private bool isOpen = false;
        private readonly ObservableCollection<Message> currentMessages = new ObservableCollection<Message>();
        private bool _isConversationSelected;
        public bool IsConversationSelected
        {
            get => _isConversationSelected;
            set
            {
                if (_isConversationSelected != value)
                {
                    _isConversationSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConversationSelected)));
                }
            }
        }
        //required for binding
        public event PropertyChangedEventHandler PropertyChanged;


        //fake data for all matches
        //private readonly List<MatchCard> allMatchesList = new List<MatchCard>
        //{
        //    new MatchCard { Name = "Jane Smith", Skill = "Python", Rating = 4.9 },
        //    new MatchCard { Name = "Mike Johnson", Skill = "Java", Rating = 4.7 },
        //    new MatchCard { Name = "John Doe", Skill = "React", Rating = 4.8 },
        //    new MatchCard { Name = "Sarah Kim", Skill = "UI/UX", Rating = 5.0 },
        //    new MatchCard { Name = "Alex Chen", Skill = "C#", Rating = 4.6 },
        //    new MatchCard { Name = "Tom Lee", Skill = "JavaScript", Rating = 4.9 }
        //};
        private List<MatchCard> allMatchesList = new List<MatchCard>();

        private class Conversation
        {
            public string Name { get; set; }
            public string LastMessage { get; set; }
            public string Avatar { get; set; }
        }

        private class Message
        {
            public string Text { get; set; }
            public bool IsSent { get; set; }
        }


        public Dashboard()
        {
            InitializeComponent();
            // Load data
            DataContext = this;
            messagesPanel.ItemsSource = currentMessages;
            //lbAllMatches.ItemsSource = allMatchesList;
            SetActiveButton(btnDashboard); //Set Dashboard as active on load
            Show(dashboardContent);//Show dashboard content on load 
            new MessagesTestWindow().Show();//open test window (Can be removed later)
            IsConversationSelected = false; //no conversation selected at start

            // Ensure ApiClient sends the saved JWT (if present)
            if (!string.IsNullOrEmpty(Services.Session.JwtToken))
                apiClient.SetJwt(Services.Session.JwtToken);


            _ = RefreshUserInfoAsync();

            ////FAKE DATA — SHOWS IMMEDIATELY IN RUNTIME
            //lbUpcomingSessions.ItemsSource = new List<SessionCard>
            //{
            //    new SessionCard { Day = "05", Month = "Dec", Skill = "React Basics", PartnerName = "Jane Smith", Time = "2:00 PM", IsTeaching = true },
            //    new SessionCard { Day = "08", Month = "Dec", Skill = "Python Fundamentals", PartnerName = "Mike Johnson", Time = "10:00 AM", IsTeaching = false },
            //    new SessionCard { Day = "15", Month = "Dec", Skill = "C# Advanced", PartnerName = "Alex Chen", Time = "4:00 PM", IsTeaching = true },
            //    new SessionCard { Day = "20", Month = "Dec", Skill = "UI/UX Design", PartnerName = "Sarah Kim", Time = "6:00 PM", IsTeaching = false }
            //};

            //// FAKE CONVERSATIONS — LOOKS REAL
            //lbConversations.ItemsSource = new List<Conversation>
            //{
            //    new Conversation { Name = "Jane Smith", LastMessage = "See you on December for t...", Avatar = "/Resources/Avatars/jane.jpg" },
            //    new Conversation { Name = "Mike Johnson", LastMessage = "Thanks for the React tutori...", Avatar = "/Resources/Avatars/mike.jpg" },
            //    new Conversation { Name = "Alex Chen", LastMessage = "Are we still on for Friday?", Avatar = "/Resources/Avatars/alex.jpg" },
            //    new Conversation { Name = "Sarah Kim", LastMessage = "Yes! 6 PM works", Avatar = "/Resources/Avatars/sarah.jpg" }
            //};

            //lbAllMatches.ItemsSource = allMatchesList;

            //lbPastSessions.ItemsSource = new List<SessionCard>
            //{
            //    new SessionCard { Day = "28", Month = "Nov", Skill = "Java OOP", PartnerName = "Anna Lee", Time = "3:00 PM", IsTeaching = true },
            //    new SessionCard { Day = "12", Month = "Nov", Skill = "Git Basics", PartnerName = "Tom Brown", Time = "11:00 AM", IsTeaching = false }
            //};

            ////FAKE DATA — DELETE LATER WHEN DATABASE IS READY
            //lbRecommendedMatches.ItemsSource = new List<MatchCard>
            //{
            //    new MatchCard { Name = "Jane Smith", Skill = "Python, CS, Fortnite, Siege, Minecraft,", Rating = 4.9 },
            //    new MatchCard { Name = "Mike Johnson", Skill = "Java", Rating = 4.7 },
            //    new MatchCard { Name = "John Doe", Skill = "React", Rating = 4.8 },
            //    new MatchCard { Name = "Anna Smith", Skill = "C#", Rating = 5.0 },
            //    new MatchCard { Name = "Tom Lee", Skill = "Design", Rating = 4.6 },
            //    new MatchCard { Name = "Sara Connor", Skill = "Machine Learning", Rating = 4.9 },
            //    new MatchCard { Name = "David Kim", Skill = "DevOps", Rating = 4.5 },
            //    new MatchCard { Name = "Linda Park", Skill = "UI/UX", Rating = 4.8 },
            //    new MatchCard { Name = "James Wilson", Skill = "Ruby on Rails", Rating = 4.7 },
            //    new MatchCard { Name = "Emily Davis", Skill = "Data Science", Rating = 4.9 },
            //    new MatchCard { Name = "Chris Brown", Skill = "Mobile Development", Rating = 4.6 },
            //    new MatchCard { Name = "Olivia Garcia", Skill = "Cybersecurity", Rating = 4.8 },
            //    new MatchCard { Name = "Daniel Martinez", Skill = "Cloud Computing", Rating = 4.7 },
            //    new MatchCard { Name = "Sophia Hernandez", Skill = "Project Management", Rating = 4.9 },
            //};
        }


        


        //Show the specified page and hide others
        public void Show(Grid page)
        {
            dashboardContent.Visibility = Visibility.Collapsed;
            MessengerGrid.Visibility = Visibility.Collapsed;
            SessionsGrid.Visibility = Visibility.Collapsed;
            MatchesGrid.Visibility = Visibility.Collapsed;

            page.Visibility = Visibility.Visible;

            if (history.Count == 0 || history.Peek() != page)
                history.Push(page);

            _ = RefreshUserInfoAsync();
        }

        // Static helper for dashboard navigation
        public static void NavigateTo(Grid page)
        {
            Application.Current.Windows.OfType<Dashboard>().FirstOrDefault()?.Show(page);
        }


        //Opens the sidebar 
        private void OpenSidebar()
        {
            //show sidebar 
            SidebarGrid.Visibility = Visibility.Visible;

            //Smooth fade-in for the dim overlay 
            Overlay.Visibility = Visibility.Visible;
            Overlay.Opacity = 0;

            var fade = new DoubleAnimation(0, 0.5, TimeSpan.FromMilliseconds(280));
            Overlay.BeginAnimation(OpacityProperty, fade);

            isOpen = true;
        }

        //Closes the sidebar
        private void CloseSidebar()
        {
            //hide sidebar
            SidebarGrid.Visibility = Visibility.Collapsed;

            //Smooth fade-out for overlay
            var fade = new DoubleAnimation(0.5, 0, TimeSpan.FromMilliseconds(220));
            fade.Completed += (s, e) => Overlay.Visibility = Visibility.Collapsed;
            Overlay.BeginAnimation(OpacityProperty, fade);

            isOpen = false;
        }

        //Toggle sidebar on hamburger click
        private void tbHamburger_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isOpen) CloseSidebar();
            else OpenSidebar();
        }

        //Close sidebar when clicking outside of it
        private void Overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseSidebar();
        }

        //Sidebar button handlers
        private void btnEditProfile_Click(object sender, RoutedEventArgs e)
        {
            //Close sidebar
            CloseSidebar();

            EditProfileWindow editProfile = new EditProfileWindow();
            editProfile.Show(); 
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            CloseSidebar();
            MessageBox.Show("Settings coming soon!");
            //TODO: Open Settings page/window
        }

        //Logout button handler
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //Ask confirmation
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CloseSidebar();

                // Clear session data and JWT
                try
                {
                    apiClient.ClearJwt();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error clearing JWT: " + ex.Message);
                }

                Session.JwtToken = null;
                Session.User = null;

                //Go back to Login window
                var login = new MainWindow(); 
                login.Show();

                this.Close();//close dashboard
            }
        }

        //Sets the active button and updates underline visibility
        private void SetActiveButton(Button activeButton)
        {
            var buttons = new[] { btnDashboard, btnMessages, btnSessions, btnMatches };

            foreach (var btn in buttons)
            {
                if (btn.Background is LinearGradientBrush brush)
                {
                    // Reset ALL underlines to invisible (Opacity 0)
                    brush.GradientStops[1].Color = Color.FromArgb(0, 143, 171, 212); // #00 8FABD4
                }
            }

            // Make ONLY the clicked one visible (Opacity 255)
            if (activeButton.Background is LinearGradientBrush activeBrush)
            {
                activeBrush.GradientStops[1].Color = Color.FromArgb(255, 143, 171, 212); // #FF 8FABD4
            }
        }

        // Button click handlers to set active button
        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnDashboard);
            Show(dashboardContent);
            MessengerGrid.Visibility = Visibility.Collapsed;
            SessionsGrid.Visibility = Visibility.Collapsed;
            MatchesGrid.Visibility = Visibility.Collapsed;
        }

        private void btnMessages_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnMessages);
            Show(MessengerGrid);
            dashboardContent.Visibility = Visibility.Collapsed;
            SessionsGrid.Visibility = Visibility.Collapsed;
            MatchesGrid.Visibility = Visibility.Collapsed;
        }

        private void btnSessions_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnSessions);
            Show(SessionsGrid);
            dashboardContent.Visibility = Visibility.Collapsed;
            MessengerGrid.Visibility = Visibility.Collapsed;
            MatchesGrid.Visibility = Visibility.Collapsed;
        }

        private void btnMatches_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnMatches);
            Show(MatchesGrid);
            dashboardContent.Visibility = Visibility.Collapsed;
            MessengerGrid.Visibility = Visibility.Collapsed;
            SessionsGrid.Visibility = Visibility.Collapsed;
        }

        // Handle selection of a recommended match

        private void lbRecommendedMatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbRecommendedMatches.SelectedItem is MatchCard selected)
            {
                var schedule = new ScheduleSessionWindow
                {
                    Owner = this,                                   // centers it
                    Title = $"Schedule Session with {selected.Name}"
                };

                // Pass the tutor id string directly (GUID or string id)
                if (!string.IsNullOrWhiteSpace(selected.MatchedUserId))
                    schedule.TutorId = selected.MatchedUserId;
                else if (!string.IsNullOrWhiteSpace(selected.CurrentUserId))
                    schedule.TutorId = selected.CurrentUserId;

                // Populate skill options from the parsed SkillsOffered list (preferred)
                if (selected.SkillsOffered != null && selected.SkillsOffered.Any())
                {
                    schedule.SkillOptions = selected.SkillsOffered;
                    if (selected.SkillIds != null && selected.SkillIds.Any())
                        schedule.SkillOptionIds = selected.SkillIds;
                }
                else if (!string.IsNullOrWhiteSpace(selected.Skill))
                {
                    // fallback to the legacy comma-separated Skill string
                    var skills = selected.Skill
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();

                    if (skills.Any())
                        schedule.SkillOptions = skills;
                }

                schedule.ShowDialog();                                   // modal window

                lbRecommendedMatches.SelectedItem = null;                // allow re-click
            }
        }

        //Store all matches for searching
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lbAllMatches == null || allMatchesList == null) return; 

            var text = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(text))
            {
                lbAllMatches.ItemsSource = allMatchesList;
                return;
            }

            lbAllMatches.ItemsSource = allMatchesList
                .Where(m => m.Name.ToLower().Contains(text) || m.Skill.ToLower().Contains(text))
                .ToList();
        }

        //Press Enter also triggers search
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SearchBox_TextChanged(sender, null);
        }

        private void lbAllMatches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbAllMatches.SelectedItem is MatchCard selected)
            {
                var schedule = new ScheduleSessionWindow
                {
                    Owner = this,
                    Title = $"Schedule Session with {selected.Name}"
                };

                if (!string.IsNullOrWhiteSpace(selected.MatchedUserId))
                    schedule.TutorId = selected.MatchedUserId;
                else if (!string.IsNullOrWhiteSpace(selected.CurrentUserId))
                    schedule.TutorId = selected.CurrentUserId;

                if (selected.SkillsOffered != null && selected.SkillsOffered.Any())
                {
                    schedule.SkillOptions = selected.SkillsOffered;
                    if (selected.SkillIds != null && selected.SkillIds.Any())
                        schedule.SkillOptionIds = selected.SkillIds;
                }
                else if (!string.IsNullOrWhiteSpace(selected.Skill))
                {
                    var skills = selected.Skill
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();

                    if (skills.Any())
                        schedule.SkillOptions = skills;
                }

                schedule.ShowDialog();
                lbAllMatches.SelectedItem = null; //allows clicking same person again
            }
        }

        //Messenger 
        public void AddMessage(string text, bool isSent)
        {
            currentMessages.Add(new Message { Text = text, IsSent = isSent });
            messageScroll.ScrollToEnd();
        }


        //Messenger
        private void lbConversations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbConversations.SelectedItem is Conversation conv)
            {
                tbChatWith.Text = conv.Name;
                currentMessages.Clear();
                // fake messages...
                currentMessages.Add(new Message { Text = "Hey!", IsSent = false });
                currentMessages.Add(new Message { Text = "Hi! Ready?", IsSent = true });

                IsConversationSelected = true;  // ← THIS SHOWS THE CHAT
                messageScroll.ScrollToEnd();
            }
            else
            {
                IsConversationSelected = false; // ← hides chat when nothing selected
            }
        }

        // Send button click handler Messnger
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        // Handle Enter key in txtMessage

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    // Shift + Enter = new line (do nothing)
                    return;
                }
                else
                {
                    // Just Enter = SEND
                    e.Handled = true;
                    SendMessage();
                }
            };
        }

        // Send the message

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;

            currentMessages.Add(new Message
            {
                Text = txtMessage.Text.Trim(),
                IsSent = true
            });

            txtMessage.Clear();
            messageScroll.ScrollToEnd();
        }

        // Fetch user info and update UI using Newtonsoft.Json
        private async Task RefreshUserInfoAsync()
        {
            try
            {
                // If we have a JWT, ensure ApiClient uses it
                if (!string.IsNullOrEmpty(Session.JwtToken))
                    apiClient.SetJwt(Session.JwtToken);

                // Fetch profile JSON from API endpoint
                var json = await apiClient.GetJson("/profile");
                System.Diagnostics.Debug.WriteLine("RAW JSON: " + json);

                // Handle empty response
                if (string.IsNullOrWhiteSpace(json)) return;

                // Parse JSON using JObject for flexible structure
                JObject root;
                try
                {
                    root = JObject.Parse(json);
                }
                catch (JsonReaderException)
                {
                    System.Diagnostics.Debug.WriteLine("RefreshUserInfoAsync: invalid JSON");
                    return;
                }

                // Locate candidate token: prefer user.dataValues -> user -> dataValues -> root
                JToken candidate = null;
                if (root["user"] != null)
                {
                    // root.user may be object; if it has dataValues, prefer that
                    var userToken = root["user"];
                    if (userToken["dataValues"] != null && userToken["dataValues"].HasValues)
                        candidate = userToken["dataValues"];
                    else if (userToken.HasValues)
                        candidate = userToken;
                }

                if (candidate == null)
                {
                    if (root["dataValues"] != null && root["dataValues"].HasValues)
                        candidate = root["dataValues"];
                    else
                        candidate = root;
                }

                if (candidate == null || !candidate.HasValues) return;

                var userData = new SimpleUser
                {
                    UserId = (string)(candidate["user_id"] ?? candidate["userId"] ?? candidate["id"]),
                    Email = (string)(candidate["email"] ?? candidate["Email"]),
                    Name = (string)(candidate["name"] ?? candidate["Name"]),
                    Role = (string)(candidate["role"] ?? candidate["Role"]),
                    Bio = (string)(candidate["bio"] ?? candidate["Bio"]),
                    ProfilePicture = (string)(candidate["profile_picture"] ?? candidate["profilePicture"]),
                    Rating = (string)(candidate["rating"] ?? candidate["Rating"])
                };

                // total_sessions might be numeric or string; parse defensively
                var totalToken = candidate["total_sessions"] ?? candidate["totalSessions"];
                if (totalToken != null && totalToken.Type != JTokenType.Null)
                {
                    int parsed;
                    if (int.TryParse(totalToken.ToString(), out parsed))
                        userData.TotalSessions = parsed;
                }

                Dispatcher.Invoke(() =>
                {
                    System.Diagnostics.Debug.WriteLine("Parsed user: " + JsonConvert.SerializeObject(userData));
                    tbWelcomeUser.Text = $"Welcome, {userData.Name ?? userData.Email ?? "User"}";
                    if (!string.IsNullOrEmpty(userData.Name)) tbSidebarName.Text = userData.Name;
                    if (!string.IsNullOrEmpty(userData.Email)) tbSidebarEmail.Text = userData.Email;

                    if (userData.TotalSessions.HasValue)
                        tbTotalSessions.Text = userData.TotalSessions.Value.ToString();

                    if (!string.IsNullOrEmpty(userData.Rating) && double.TryParse(userData.Rating, out double parsedRating))
                        tbRatings.Text = parsedRating.ToString("F1");
                });

                // Await the asynchronous GetRecommendedMatches result
                var matches = await GetRecommendedMatches();

                // Assign to ItemsSource on the UI thread
                Dispatcher.Invoke(() => lbRecommendedMatches.ItemsSource = matches);

                // Also refresh all matches list
                var allMatches = await GetRecommendedMatches();
                Dispatcher.Invoke(() => lbAllMatches.ItemsSource = allMatches);

                // Refresh session list
                var Sessions = await GetUpcomingSessions();
                Dispatcher.Invoke(() => lbUpcomingSessions.ItemsSource = Sessions);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RefreshUserInfoAsync failed: " + ex.Message);
            }
        }

        private async Task<List<SessionCard>> GetUpcomingSessions()
        {
            // Ensure ApiClient uses saved JWT if present
            if (!string.IsNullOrEmpty(Session.JwtToken))
                apiClient.SetJwt(Session.JwtToken);

            var json = await apiClient.GetJson("/api/sessions");
            System.Diagnostics.Debug.WriteLine("RAW SESSION JSON: " + json);

            if (string.IsNullOrWhiteSpace(json)) return new List<SessionCard>();

            try
            {
                var sessions = new List<SessionCard>();

                var rootToken = JToken.Parse(json);
                JArray arr = null;

                if (rootToken.Type == JTokenType.Array)
                {
                    arr = (JArray)rootToken;
                }
                else if (rootToken.Type == JTokenType.Object)
                {
                    var rootObj = (JObject)rootToken;
                    // Look for the sessions array explicitly (per provided example)
                    var mToken = rootObj["sessions"] ?? rootObj["data"] ?? rootObj["results"];

                    if (mToken != null && mToken.Type == JTokenType.Array)
                        arr = (JArray)mToken;
                    else if (mToken != null && mToken.Type == JTokenType.Object)
                        arr = new JArray(mToken); // single object -> treat as one-element array
                }

                if (arr == null)
                    return new List<SessionCard>();

                string myId = Session.User?.Id;

                foreach (var item in arr)
                {
                    // Fields expected per provided JSON example:
                    var tutorId = (string)(item["tutor_id"] ?? item["tutorId"]);
                    var learnerId = (string)(item["learner_id"] ?? item["learnerId"]);
                    var dateStr = (string)(item["date"]);
                    var timeStr = (string)(item["time"]); // expected "HH:mm:ss"
                    var skillIdToken = item["skill_id"] ?? item["skillId"];
                    int? skillId = null;
                    if (skillIdToken != null && skillIdToken.Type != JTokenType.Null)
                    {
                        int parsedSkill;
                        if (int.TryParse(skillIdToken.ToString(), out parsedSkill))
                            skillId = parsedSkill;
                    }

                    // Nested tutor/learner objects
                    var tutorObj = item["tutor"];
                    var learnerObj = item["learner"];

                    string tutorName = tutorObj?["name"]?.ToString();
                    string tutorEmail = tutorObj?["email"]?.ToString();
                    string tutorDisplay = (string)(item["tutor_display"] ?? item["tutorDisplay"]);

                    string learnerName = learnerObj?["name"]?.ToString();
                    string learnerEmail = learnerObj?["email"]?.ToString();
                    string learnerDisplay = (string)(item["learner_display"] ?? item["learnerDisplay"]);

                    // Skill may be an object with skill_name
                    string skill = null;
                    var skillToken = item["skill"];
                    if (skillToken != null)
                    {
                        if (skillToken.Type == JTokenType.Object)
                        {
                            skill = (string)(skillToken["skill_name"] ?? skillToken["skillName"] ?? skillToken["name"]);
                        }
                        else
                        {
                            skill = skillToken.ToString().Trim();
                        }
                    }

                    // Also check top-level fields
                    if (string.IsNullOrWhiteSpace(skill))
                        skill = (string)(item["skill_name"] ?? item["skillName"] ?? item["title"] ?? item["topic"]);

                    if (string.IsNullOrWhiteSpace(skill) && skillId.HasValue)
                        skill = $"Skill #{skillId.Value}";

                    if (string.IsNullOrWhiteSpace(skill))
                        skill = "Session";

                    // Parse date + time into DateTime
                    DateTime parsedDt;
                    bool parsed = false;
                    if (!string.IsNullOrWhiteSpace(dateStr) && !string.IsNullOrWhiteSpace(timeStr))
                    {
                        // date e.g. "2025-12-05", time e.g. "14:00:00"
                        parsed = DateTime.TryParse(dateStr + " " + timeStr, out parsedDt);
                    }
                    else if (!string.IsNullOrWhiteSpace(dateStr))
                    {
                        parsed = DateTime.TryParse(dateStr, out parsedDt);
                    }
                    else
                    {
                        parsed = DateTime.TryParse(item.ToString(), out parsedDt);
                    }

                    if (!parsed)
                        parsedDt = DateTime.Now;

                    // Decide whether current user is the tutor
                    bool isTeaching = false;
                    if (!string.IsNullOrWhiteSpace(myId) && !string.IsNullOrWhiteSpace(tutorId))
                        isTeaching = string.Equals(myId, tutorId, StringComparison.OrdinalIgnoreCase);

                    // Determine partner name (use tutor name/preferred values).
                    // User requested partnerName to be tutor's name (not the tutor id).
                    string partnerName;

                    if (isTeaching)
                    {
                        // I'm the tutor, so the partner is the learner
                        partnerName =
                            !string.IsNullOrWhiteSpace(learnerName) ? learnerName :
                            !string.IsNullOrWhiteSpace(learnerEmail) ? learnerEmail :
                            !string.IsNullOrWhiteSpace(learnerDisplay) ? learnerDisplay :
                            learnerId;
                    }
                    else
                    {
                        // I'm the learner, so the partner is the tutor
                        partnerName =
                            !string.IsNullOrWhiteSpace(tutorName) ? tutorName :
                            !string.IsNullOrWhiteSpace(tutorEmail) ? tutorEmail :
                            !string.IsNullOrWhiteSpace(tutorDisplay) ? tutorDisplay :
                            tutorId;
                    }

                    sessions.Add(new SessionCard
                    {
                        SessionId = (string)(item["session_id"] ?? item["sessionId"] ?? item["id"] ?? item["Id"]),
                        Day = parsedDt.ToString("dd"),
                        Month = parsedDt.ToString("MMM"),
                        Skill = skill,
                        PartnerName = partnerName,
                        Time = parsedDt.ToString("h:mm tt"),
                        IsTeaching = isTeaching
                    });
                }

                return sessions;
            }
            catch (JsonReaderException jex)
            {
                System.Diagnostics.Debug.WriteLine("GetUpcomingSessions JSON parse failed: " + jex.Message);
                return new List<SessionCard>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetUpcomingSessions failed: " + ex.Message);
                return new List<SessionCard>();
            }
        }

        private async Task<List<MatchCard>> GetRecommendedMatches()
        {
            // Ensure ApiClient uses saved JWT if present
            if (!string.IsNullOrEmpty(Session.JwtToken))
                apiClient.SetJwt(Session.JwtToken);

            var json = await apiClient.GetJson("/api/matches");
            System.Diagnostics.Debug.WriteLine("RAW MATCHES JSON: " + json);

            if (string.IsNullOrWhiteSpace(json))
                return new List<MatchCard>();

            try
            {
                var matches = new List<MatchCard>();

                // Use JToken.Parse so we can handle either an array or an object containing an array
                var rootToken = JToken.Parse(json);
                JArray arr = null;

                if (rootToken.Type == JTokenType.Array)
                {
                    arr = (JArray)rootToken;
                }
                else if (rootToken.Type == JTokenType.Object)
                {
                    var rootObj = (JObject)rootToken;
                    // Common property name in your logs is "matches"
                    var mToken = rootObj["matches"] ?? rootObj["data"] ?? rootObj["results"];

                    if (mToken != null && mToken.Type == JTokenType.Array)
                        arr = (JArray)mToken;
                    else if (mToken != null && mToken.Type == JTokenType.Object)
                        arr = new JArray(mToken); // single object -> treat as one-element array
                    else
                        return new List<MatchCard>(); // no array available
                }

                if (arr == null)
                    return new List<MatchCard>();

                foreach (var item in arr)
                {
                    // parse skillsOffered into list
                    var skillsOffered = new List<string>();
                    var skillsToken = item["skillsOffered"] ?? item["skills"];
                    if (skillsToken != null && skillsToken.Type == JTokenType.Array)
                    {
                        skillsOffered = skillsToken
                            .Children()
                            .Select(t => t.ToString().Trim())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList();
                    }
                    else if (skillsToken != null && skillsToken.Type != JTokenType.Null)
                    {
                        var s = skillsToken.ToString().Trim();
                        if (!string.IsNullOrEmpty(s)) skillsOffered.Add(s);
                    }

                    // parse overlapSkillIds (or other id-arrays)
                    var skillIds = new List<int>();
                    var idToken = item["overlapSkillIds"] ?? item["skillIds"] ?? item["skillsIds"];
                    if (idToken != null && idToken.Type == JTokenType.Array)
                    {
                        foreach (var t in idToken.Children())
                        {
                            int parsed;
                            if (int.TryParse(t.ToString(), out parsed))
                                skillIds.Add(parsed);
                        }
                    }

                    var skillDisplay = skillsOffered.Any() ? string.Join(", ", skillsOffered) : (string)(item["skill"] ?? item["Skill"] ?? string.Empty);

                    matches.Add(new MatchCard
                    {
                        CurrentUserId = (string)(item["id"] ?? item["Id"]),
                        MatchedUserId = (string)(item["userId"] ?? item["UserId"]),
                        Name = (string)(item["name"] ?? item["Name"]),
                        Skill = skillDisplay,
                        SkillsOffered = skillsOffered,
                        SkillIds = skillIds,
                        Rating = item["rating"] != null ? Convert.ToDouble(item["rating"]) : 0.0
                    });
                }

                // Keep local allMatchesList in sync (optional)
                allMatchesList = matches;

                return matches;
            }
            catch (JsonReaderException jex)
            {
                System.Diagnostics.Debug.WriteLine("GetRecommendedMatches JSON parse failed: " + jex.Message);
                return new List<MatchCard>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetRecommendedMatches failed: " + ex.Message);
                return new List<MatchCard>();
            }
        }

        // Minimal user DTO used locally
        private class SimpleUser
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Role { get; set; }
            public string Bio { get; set; }
            public string ProfilePicture { get; set; }
            public string Rating { get; set; }
            public int? TotalSessions { get; set; }
        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Ensure JWT is used
            if (!string.IsNullOrEmpty(Session.JwtToken))
                apiClient.SetJwt(Session.JwtToken);

            // Use the session object directly from the button DataContext
            var button = sender as Button;
            if (button == null)
            {
                MessageBox.Show("Unable to determine the session to cancel.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var session = button.DataContext as SessionCard;
            if (session == null)
            {
                MessageBox.Show("Session data unavailable for this item.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(session.SessionId))
            {
                MessageBox.Show("This session does not contain an ID. Cannot cancel.", "Missing ID", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to cancel session {session.SessionId}?", "Cancel Session", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var endpoint = $"/api/sessions/{session.SessionId}/cancel";
                // Call PUT /sessions/{id}/cancel (no body)
                await apiClient.PutJson(endpoint, null);

                MessageBox.Show("Session cancelled successfully.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh upcoming sessions list
                var sessions = await GetUpcomingSessions();
                Dispatcher.Invoke(() => lbUpcomingSessions.ItemsSource = sessions);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to cancel session: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReschedule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
