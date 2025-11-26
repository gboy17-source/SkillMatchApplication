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

namespace SkillMatchApplication
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private bool isOpen = false;
        public Dashboard()
        {
            InitializeComponent();
            SetActiveButton(btnDashboard); //Set Dashboard as active on load
            new MessagesTestWindow().Show();//open test window (Can be removed later)
            SetupChatInput();
        }

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

        private void tbHamburger_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isOpen) CloseSidebar();
            else OpenSidebar();
        }

        private void Overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseSidebar();
        }

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
            var buttons = new[] { btnDashboard, btnMessages, btnSessions, btnNotifications };

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
        }

        private void btnMessages_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnMessages);
        }

        private void btnSessions_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnSessions);
        }

        private void btnNotifications_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(btnNotifications);
        }


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

        //Call this once when the window loads (in constructor or Loaded event)
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

        //Send message from dashboard input
        private void SendMessageFromDashboard()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text)) return;

            string text = txtMessage.Text;
            txtMessage.Clear();

            AddMessageToChat(text, true);//true = from me (right side)
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
    }
}
