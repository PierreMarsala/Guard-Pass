using Guard_Pass.Models;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using Guard_Pass.Languages;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Guard_Pass.Methods;
using System.Text;
using System.Reflection.Metadata;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.InteropServices.Marshalling;

namespace Guard_Pass.Views
{
    public partial class RegisterView : Window
    {
        Parameters? parameters = new Parameters();
        Color white = (Color)ColorConverter.ConvertFromString("#EDF5F4");
        Color red = (Color)ColorConverter.ConvertFromString("#E86056");
        const string securityPath = "./Security/Account/";
        const string accountPath = securityPath + "account.dat";
        const string parametersPath = "./Config/Parameters.json";
        string? messageBox = string.Empty;

        public RegisterView()
        {
            InitializeComponent();
            parameters = JsonSerializer.Deserialize<Parameters>(File.ReadAllText(parametersPath));
            if (parameters != null && parameters.language != null)
            {
                LanguagesManager.RemoveCurrentLanguage();
                LanguagesManager.AddLanguage(parameters.language);
                Image_Background.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/Backgrounds/{parameters.background}.jpg"));
                if (parameters.language == "us")
                {
                    Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/us.jpg"));
                    messageBox = "Overwrite your last account ?";
                }
                if (parameters.language == "fr")
                {
                    Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/fr.jpg"));
                    messageBox = "Écraser votre ancien compte ?";

                }
            }
        }

        // Button function :
        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if (Password_First.Password == string.Empty && Password_Second.Password == string.Empty) { BoxErrorManager(RegisterError.BothBox); return; }
            if (Password_First.Password == string.Empty) { BoxErrorManager(RegisterError.FirstBox); return; }
            if (Password_Second.Password == string.Empty) { BoxErrorManager(RegisterError.SecondBox); return; }
            if (Password_First.Password != Password_Second.Password) { BoxErrorManager(RegisterError.BothBox); return; }

            if (Password_First.Password == Password_Second.Password)
            {
                if (Directory.Exists(securityPath) && Directory.GetFiles(securityPath).Length > 0)
                {
                    MessageBoxResult result = MessageBox.Show(messageBox, "Create account", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) CreateAccount(); //TODO Remove all file stored
                    else return;
                }
                else CreateAccount();
            }
            ChangeToLoginView();
        }
        private void Btn_Language_Click(object sender, RoutedEventArgs e)
        {
            LanguagesManager.RemoveCurrentLanguage();
            if (parameters != null && parameters.language == "fr")
            {
                LanguagesManager.AddLanguage("us");
                parameters.language = "us";
                messageBox = "Overwrite your last account ?";
                File.WriteAllText(parametersPath, JsonSerializer.Serialize(parameters));
                Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/us.jpg"));
            }
            else if (parameters != null && parameters.language == "us")
            {
                LanguagesManager.AddLanguage("fr");
                parameters.language = "fr";
                messageBox = "Écraser votre ancien compte ?";
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
            ChangeToLoginView();
        }


        // others function :
        private void ChangeToLoginView()
        {
            LoginView loginView = new LoginView();
            loginView.Show();
            this.Close();
        }
        private void CreateAccount()
        {
            var IV = KeyManager.IVGenerator();
            var key = KeyManager.KeyGenerator(Password_First.Password);
            var encrypted = Encrypter.Encrypt(Password_First.Password, key, IV);

            if (!Directory.Exists("./Security/")) Directory.CreateDirectory("./Security/");
            if (!Directory.Exists("./Security/Account/")) Directory.CreateDirectory("./Security/Account/");
            if (!Directory.Exists("./Security/Data/")) Directory.CreateDirectory("./Security/Data/");
            KeyManager.DataToFile(accountPath, encrypted, IV);
        }
        private async void BoxErrorManager(RegisterError error)
        {
            ColorControllers(error, new SolidColorBrush(red));
            await Task.Delay(2000);
            ColorControllers(error, new SolidColorBrush(white));
        }
        private void ColorControllers(RegisterError error, SolidColorBrush color)
        {
            if (error == RegisterError.FirstBox)
            {
                Image_Lock1.Foreground = color;
                Password_First.Background = color;
            }
            else if (error == RegisterError.SecondBox)
            {
                Image_Lock2.Foreground = color;
                Password_Second.Background = color;
            }
            else if (error == RegisterError.BothBox)
            {
                Image_Lock1.Foreground = color;
                Image_Lock2.Foreground = color;
                Password_First.Background = color;
                Password_Second.Background = color;
                Password_First.Password = string.Empty;
                Password_Second.Password = string.Empty;
            }
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
