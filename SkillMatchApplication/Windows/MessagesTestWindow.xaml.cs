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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkillMatchApplication.Windows
{
    /// <summary>
    /// Interaction logic for MessagesTestWindow.xaml
    /// </summary>
    public partial class MessagesTestWindow : Window
    {
        private Dashboard _dashboard;

        public MessagesTestWindow()
        {
            InitializeComponent();

            _dashboard = Application.Current.Windows.OfType<Dashboard>().FirstOrDefault();

            // Send as "Me"
            txtMyMessage.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter && !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    SendAsMe();
                    e.Handled = true;
                }
            };

            // Optional: Send as "Other"
            btnSendAsOther.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtOtherMessage.Text))
                {
                    SendAsOther(txtOtherMessage.Text);
                    txtOtherMessage.Clear();
                }
            };
        }

        private void btnSendAsOther_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtOtherMessage.Text))
            {
                string text = txtOtherMessage.Text.Trim();
                txtOtherMessage.Clear();

                // Add to test window
                AddMessageToTest(text, false);

                // Add to real Dashboard
                _dashboard?.AddMessage(text, false);
            }
        }

        private void SendAsMe()
        {
            if (string.IsNullOrWhiteSpace(txtMyMessage.Text)) return;
            string text = txtMyMessage.Text.Trim();
            txtMyMessage.Clear();

            AddMessageToTest(text, true);
            _dashboard?.AddMessage(text, true);  // ← now works!
        }

        private void SendAsOther(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            AddMessageToTest(text, false);
            _dashboard?.AddMessage(text, false);
        }

        private void AddMessageToTest(string text, bool isSent)
        {
            var bubble = new Border
            {
                Background = isSent ? new SolidColorBrush(Color.FromRgb(30, 136, 229)) : new SolidColorBrush(Color.FromRgb(228, 230, 235)),
                CornerRadius = new CornerRadius(18),
                Padding = new Thickness(12, 10, 12, 10),
                MaxWidth = 500,
                HorizontalAlignment = isSent ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Margin = new Thickness(10, 5, 10, 1),
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = isSent ? Brushes.White : Brushes.Black,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 15
                }
            };

            messagesStackPanel.Children.Add(bubble);
            messageContainer.ScrollToEnd();
        }
    }
}
