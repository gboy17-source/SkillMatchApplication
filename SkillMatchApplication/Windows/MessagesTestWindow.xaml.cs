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

            txtMyMessage.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    SendAsMe();
                    e.Handled = true;
                }
            };
        }

        private void SendAsMe()
        {
            if (string.IsNullOrWhiteSpace(txtMyMessage.Text)) return;

            string text = txtMyMessage.Text;
            txtMyMessage.Clear();

            AddBubbleToTestWindow(text, true);

            _dashboard?.AddMessageToChat(text, true);        
            _dashboard?.ScrollChatToBottom();              
        }

        private void btnSendAsOther_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOtherMessage.Text)) return;

            string text = txtOtherMessage.Text;
            txtOtherMessage.Clear();

            AddBubbleToTestWindow(text, false);

            _dashboard?.AddMessageToChat(text, false);
            _dashboard?.ScrollChatToBottom();              
        }

        private void AddBubbleToTestWindow(string text, bool isMe) => AddBubble(messagesStackPanel, messageContainer, text, isMe);
        private void AddBubbleToDashboard(string text, bool isMe) => AddBubble(_dashboard.messagesStackPanel, _dashboard.messageContainer, text, isMe);

        private void AddBubble(Panel panel, ScrollViewer scrollViewer, string text, bool isMe)
        {
            var bubble = new Border
            {
                Background = isMe
                    ? new SolidColorBrush(Color.FromRgb(143, 171, 212))
                    : Brushes.LightGray,
                CornerRadius = new CornerRadius(20),
                Padding = new Thickness(16, 12, 16, 12),   // ← FIXED: 4 values
                MaxWidth = 600,
                HorizontalAlignment = isMe ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Margin = new Thickness(15, 8, 15, 8),      // ← FIXED: 4 values
                Child = new TextBlock
                {
                    Text = text,
                    Foreground = isMe ? Brushes.White : Brushes.Black,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 15
                }
            };

            panel.Children.Add(bubble);
            scrollViewer.ScrollToEnd();
        }
    }
}
