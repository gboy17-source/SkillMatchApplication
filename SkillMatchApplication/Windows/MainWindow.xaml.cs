using Newtonsoft.Json;
using SkillMatchApplication.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkillMatchApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Stack<Grid> history = new Stack<Grid>();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        //Show a specific page
        public void Show(Grid page)
        {
            //Hide all possible pages
            LoginContent.Visibility = Visibility.Collapsed;
            RegisterContent.Visibility = Visibility.Collapsed;
            ForgotEmailGrid.Visibility = Visibility.Collapsed;
            ForgotCodeGrid.Visibility = Visibility.Collapsed;
            ForgotNewPasswordGrid.Visibility = Visibility.Collapsed;
            SuccessGrid.Visibility = Visibility.Collapsed;

            //Show the one we want
            page.Visibility = Visibility.Visible;

            //Add to history (only if it's not already the current page)
            if (history.Count == 0 || history.Peek() != page)
                history.Push(page);
        }

        //Navigate to a page from anywhere in the app
        public static void NavigateTo(Grid page)
        {
            (Application.Current.MainWindow as MainWindow)?.Show(page);
        }

        //Login Button from LoginContent
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //Dashboard dashboard = new Dashboard();
            //dashboard.Show();
            //this.Close();

            string email = txtLoginEmail.Text;
            string password = txtLoginPassword.Password;

            try
            {
                // Call our api client
                var api = new ApiClient();

                // Request for login
                var loginData = new Models.AuthModels.LoginRequest()
                {
                    email = email,
                    password = password,
                };

                string json = JsonConvert.SerializeObject(loginData);

                string responseJson = await api.PostJson("/login", json);

                var response = JsonConvert.DeserializeObject<Models.AuthModels.LoginResponse>(responseJson);

                // Save the raw token
                Session.JwtToken = response.token;

                // Decode it for easy access
                Session.User = JwtService.Decode(response.token);

                api.SetJwt(Session.JwtToken);

                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Dashboard dashboard = new Dashboard();
                dashboard.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed: " + ex.Message, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Forgot password link from LoginContent
        /// backend not yet implemented
        //Forgot password link from LoginContent
        private void tbForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            Show(ForgotEmailGrid);
        }

        //Back to login button from ForgotEmailGrid
        private void btnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            Show(LoginContent);
            history.Clear();  //so Back button doesn't go back into forgot flow
        }
        //Submit email for password recovery from ForgotEmailGrid
        private void btnSubmitEmail_Click(object sender, RoutedEventArgs e)
        {
            Show(ForgotCodeGrid);
        }

        //Submit code from ForgotCodeGrid
        private void btnSubmitCode_Click(object sender, RoutedEventArgs e)
        {
            Show(ForgotNewPasswordGrid);
        }

        //Submit new password from ForgotNewPasswordGrid
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Show(ForgotEmailGrid);
        }
        //Enter new password button from ForgotNewPasswordGrid
        private void btnEnterNewPassword_Click(object sender, RoutedEventArgs e)
        {
            Show(SuccessGrid);
        }
        //Back to login button from SuccessGrid
        private void btnBackToLogin_Copy_Click(object sender, RoutedEventArgs e)
        {
            Show(LoginContent);
        }
        ///</summary>

        ///<summary>
        ///Contents of Register 
        ///Backend not yet implemented
       
        //Register link from LoginContent
        private void tbRegister_Click(object sender, MouseButtonEventArgs e)
        {
            Show(RegisterContent);
        }

        //Login link from RegisterContent
        private void tbLogin_Click(object sender, RoutedEventArgs e)
        {
            Show(LoginContent);
        }

        //Register button from RegisterContent
        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //Show(LoginContent); //Back to login content after successfully registering

            string email = txtRegisterEmail.Text;
            string password = txtRegisterPassword.Password;

            try
            {
                var api = new ApiClient();

                var registerData = new Models.AuthModels.RegisterRequest()
                {
                    email = email,
                    password = password,
                };

                string json = JsonConvert.SerializeObject(registerData);

                string responseJson = await api.PostJson("/register", json);

                var response = JsonConvert.DeserializeObject<Models.AuthModels.RegisterResponse>(responseJson);

                MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed: " + ex.Message, "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        ///</summary>
    }
}
