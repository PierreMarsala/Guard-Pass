using Guard_Pass.Languages;
using Guard_Pass.Methods;
using Guard_Pass.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Guard_Pass.Views
{
    public partial class LoginView : Window
    {
        Parameters? parameters = new Parameters();
        Color white = (Color)ColorConverter.ConvertFromString("#EDF5F4");
        Color red = (Color)ColorConverter.ConvertFromString("#E86056");
        const string securityPath = "./Security/Account/";
        const string accountPath = securityPath + "account.dat";
        const string parametersPath = "./Config/Parameters.json";

        public LoginView()
        {
            InitializeComponent();
            parameters = JsonSerializer.Deserialize<Parameters>(File.ReadAllText(parametersPath));
            if (parameters != null && parameters.language != null)
            {
                LanguagesManager.RemoveCurrentLanguage();
                LanguagesManager.AddLanguage(parameters.language);
                Image_Background.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/Backgrounds/{parameters.background}.jpg"));
                if(parameters.language == "us") Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/us.jpg"));
                if(parameters.language == "fr") Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/fr.jpg"));
            }
        }


        // Button function :
        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if (Password_First.Password == string.Empty) { BoxErrorManager(); return; }
            var key = KeyManager.KeyGenerator(Password_First.Password);
            var content = File.ReadAllBytes(accountPath);
            (byte[] IV, byte[] data) = KeyManager.Separator(content);
            var password = Decrypter.Decrypt(data, key, IV);

            if (password == Password_First.Password) ChangeToMainView(key);
            else {BoxErrorManager(); Thread.Sleep(250); }
        }
        private void Btn_Language_Click(object sender, RoutedEventArgs e)
        {
            LanguagesManager.RemoveCurrentLanguage();
            if (parameters != null && parameters.language == "fr")
            {
                LanguagesManager.AddLanguage("us");
                parameters.language = "us";
                File.WriteAllText(parametersPath, JsonSerializer.Serialize(parameters));
                Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/us.jpg"));
            }
            else if (parameters != null && parameters.language == "us")
            {
                LanguagesManager.AddLanguage("fr");
                parameters.language = "fr";
                File.WriteAllText(parametersPath, JsonSerializer.Serialize(parameters));
                Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/fr.jpg"));
            }
        }
        private void Btn_Dice_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int number = random.Next(1, 5);
            Image_Background.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/Backgrounds/{number}.jpg"));
            if (parameters != null) parameters.background = number;
            File.WriteAllText(parametersPath, JsonSerializer.Serialize(parameters));
        }
        private void Btn_Login_Click(object sender, RoutedEventArgs e)
        {
            ChangeToRegisterView();
        }




        // others function :
        private void ChangeToMainView(byte[] key)
        {
            MainView mainView = new MainView(key);
            mainView.Show();
            this.Close();
        }
        private void ChangeToRegisterView()
        {
            RegisterView registerView = new RegisterView();
            registerView.Show();
            this.Close();
        }
        private async void BoxErrorManager()
        {
            ColorControllers(new SolidColorBrush(red));
            Password_First.Password = string.Empty;
            await Task.Delay(2000);
            ColorControllers(new SolidColorBrush(white));
        }
        private void ColorControllers(SolidColorBrush color)
        {
            Image_Lock1.Foreground = color;
            Password_First.Background = color;
        }



        // Interface interaction :
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
