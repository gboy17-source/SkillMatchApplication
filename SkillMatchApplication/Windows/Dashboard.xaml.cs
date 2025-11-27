using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using SkillMatchApplication.Windows;
using SkillMatchApplication.Models;
using System.Windows.Media.Effects;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SkillMatchApplication
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, INotifyPropertyChanged
    {
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
        private readonly List<MatchCard> allMatchesList = new List<MatchCard>
        {
            new MatchCard { Name = "Jane Smith", Skill = "Python", Rating = 4.9 },
            new MatchCard { Name = "Mike Johnson", Skill = "Java", Rating = 4.7 },
            new MatchCard { Name = "John Doe", Skill = "React", Rating = 4.8 },
            new MatchCard { Name = "Sarah Kim", Skill = "UI/UX", Rating = 5.0 },
            new MatchCard { Name = "Alex Chen", Skill = "C#", Rating = 4.6 },
            new MatchCard { Name = "Tom Lee", Skill = "JavaScript", Rating = 4.9 }
        };

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
            lbAllMatches.ItemsSource = allMatchesList;
            SetActiveButton(btnDashboard); //Set Dashboard as active on load
            Show(dashboardContent);//Show dashboard content on load 
            new MessagesTestWindow().Show();//open test window (Can be removed later)
            IsConversationSelected = false; //no conversation selected at start
            //FAKE DATA — SHOWS IMMEDIATELY IN RUNTIME
            lbUpcomingSessions.ItemsSource = new List<SessionCard>
            {
                new SessionCard { Day = "05", Month = "Dec", Skill = "React Basics", PartnerName = "Jane Smith", Time = "2:00 PM", IsTeaching = true },
                new SessionCard { Day = "08", Month = "Dec", Skill = "Python Fundamentals", PartnerName = "Mike Johnson", Time = "10:00 AM", IsTeaching = false },
                new SessionCard { Day = "15", Month = "Dec", Skill = "C# Advanced", PartnerName = "Alex Chen", Time = "4:00 PM", IsTeaching = true },
                new SessionCard { Day = "20", Month = "Dec", Skill = "UI/UX Design", PartnerName = "Sarah Kim", Time = "6:00 PM", IsTeaching = false }
            };

            // FAKE CONVERSATIONS — LOOKS REAL
            lbConversations.ItemsSource = new List<Conversation>
            {
                new Conversation { Name = "Jane Smith", LastMessage = "See you on December for t...", Avatar = "/Resources/Avatars/jane.jpg" },
                new Conversation { Name = "Mike Johnson", LastMessage = "Thanks for the React tutori...", Avatar = "/Resources/Avatars/mike.jpg" },
                new Conversation { Name = "Alex Chen", LastMessage = "Are we still on for Friday?", Avatar = "/Resources/Avatars/alex.jpg" },
                new Conversation { Name = "Sarah Kim", LastMessage = "Yes! 6 PM works", Avatar = "/Resources/Avatars/sarah.jpg" }
            };

            lbAllMatches.ItemsSource = allMatchesList;

            lbPastSessions.ItemsSource = new List<SessionCard>
            {
                new SessionCard { Day = "28", Month = "Nov", Skill = "Java OOP", PartnerName = "Anna Lee", Time = "3:00 PM", IsTeaching = true },
                new SessionCard { Day = "12", Month = "Nov", Skill = "Git Basics", PartnerName = "Tom Brown", Time = "11:00 AM", IsTeaching = false }
            };

            //FAKE DATA — DELETE LATER WHEN DATABASE IS READY
            lbRecommendedMatches.ItemsSource = new List<MatchCard>
            {
                new MatchCard { Name = "Jane Smith", Skill = "Python, CS, Fortnite, Siege, Minecraft,", Rating = 4.9 },
                new MatchCard { Name = "Mike Johnson", Skill = "Java", Rating = 4.7 },
                new MatchCard { Name = "John Doe", Skill = "React", Rating = 4.8 },
                new MatchCard { Name = "Anna Smith", Skill = "C#", Rating = 5.0 },
                new MatchCard { Name = "Tom Lee", Skill = "Design", Rating = 4.6 },
                new MatchCard { Name = "Sara Connor", Skill = "Machine Learning", Rating = 4.9 },
                new MatchCard { Name = "David Kim", Skill = "DevOps", Rating = 4.5 },
                new MatchCard { Name = "Linda Park", Skill = "UI/UX", Rating = 4.8 },
                new MatchCard { Name = "James Wilson", Skill = "Ruby on Rails", Rating = 4.7 },
                new MatchCard { Name = "Emily Davis", Skill = "Data Science", Rating = 4.9 },
                new MatchCard { Name = "Chris Brown", Skill = "Mobile Development", Rating = 4.6 },
                new MatchCard { Name = "Olivia Garcia", Skill = "Cybersecurity", Rating = 4.8 },
                new MatchCard { Name = "Daniel Martinez", Skill = "Cloud Computing", Rating = 4.7 },
                new MatchCard { Name = "Sophia Hernandez", Skill = "Project Management", Rating = 4.9 },
            };
        }

        public void AddMessage(string text, bool isSent)
        {
            currentMessages.Add(new Message { Text = text, IsSent = isSent });
            messageScroll.ScrollToEnd();
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
                var schedule = new ScheduleSessionWindow();
                schedule.Owner = this;                                   // centers it
                schedule.Title = $"Schedule Session with {selected.Name}";
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
                var schedule = new ScheduleSessionWindow();
                schedule.Owner = this;
                schedule.Title = $"Schedule Session with {selected.Name}";
                schedule.ShowDialog();
                lbAllMatches.SelectedItem = null; //allows clicking same person again
            }
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

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

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


        /* No longer needed

          //Scroll the chat to the bottom
         public void ScrollChatToBottom()
         {
             if (messageContainer != null)
             {
                 messageContainer.Dispatcher.BeginInvoke(
                     System.Windows.Threading.DispatcherPriority.Background,
                     new Action(() => messageContainer.ScrollToEnd())
                 );
             }
         }

          private void SetupChatInput()
        {
            txtMessage.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        //Shift + Enter = allow new line (do nothing special)
                        //WPF will automatically insert a new line because AcceptsReturn="True"
                    }
                    else
                    {
                        //Just Enter = SEND
                        e.Handled = true;                   // ← Stops the newline from appearing
                        SendMessageFromDashboard();
                    }
                }
            };

            //Button also sends
            btnSendMessage.Click += (s, e) => SendMessageFromDashboard();
        }

        
        //Handle Enter key in txtMessage
        private void txtMessage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = true;
                btnSendMessage.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }


         //Add a message bubble to the chat
         public void AddMessageToChat(string text, bool isFromMe)
         {
             var bubble = new Border
             {
                 Background = isFromMe ? new SolidColorBrush(Color.FromRgb(143, 171, 212)) : Brushes.LightGray,
                 CornerRadius = new CornerRadius(20),
                 Padding = new Thickness(16, 12, 16, 12),
                 MaxWidth = 600,
                 HorizontalAlignment = isFromMe ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                 Margin = new Thickness(15, 8, 15, 8),
                 Child = new TextBlock
                 {
                     Text = text,
                     Foreground = isFromMe ? Brushes.White : Brushes.Black,
                     TextWrapping = TextWrapping.Wrap,
                     FontSize = 15
                 }
             };

             messagesStackPanel.Children.Add(bubble);
             ScrollChatToBottom();//always scrolls, no matter who sent it
         }
               private void SendMessageFromDashboard()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;

            string text = txtMessage.Text;
            txtMessage.Clear();

            AddMessageToChat(text, true);//true = from me (right side)
        }
        */
    }
}



















