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

            //TODO: Open Edit Profile page/window
            MessageBox.Show("Edit Profile clicked! Ready for your form.");
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            CloseSidebar();
            MessageBox.Show("Settings coming soon!");
            //TODO: Open Settings page/window
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //Ask confirmation (professional!)
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

                this.Close();// close dashboard
            }
        }
    }
}
