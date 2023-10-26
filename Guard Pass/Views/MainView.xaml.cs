using Guard_Pass.Languages;
using Guard_Pass.Methods;
using Guard_Pass.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Guard_Pass.Views
{
    public partial class MainView : Window
    {
        Parameters? parameters = new Parameters();
        const string dataPath = "./Security/Data/";
        const string parametersPath = "./Config/Parameters.json";
        string currentPath = string.Empty;
        int tabIndex = 0;
        byte[] key;

        public MainView(byte[] key)
        {
            InitializeComponent();
            parameters = JsonSerializer.Deserialize<Parameters>(File.ReadAllText(parametersPath));
            if (parameters != null && parameters.language != null)
            {
                LanguagesManager.RemoveCurrentLanguage();
                LanguagesManager.AddLanguage(parameters.language);
                Image_Background.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/Backgrounds/{parameters.background}.jpg"));
                if (parameters.language == "us") Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/us.jpg"));
                if (parameters.language == "fr") Image_Flag.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Icons/fr.jpg"));
            }

            if (!Directory.Exists("./Security/Data/Application/")) Directory.CreateDirectory("./Security/Data/Application/");
            if (!Directory.Exists("./Security/Data/Game/")) Directory.CreateDirectory("./Security/Data/Game/");
            if (!Directory.Exists("./Security/Data/Other/")) Directory.CreateDirectory("./Security/Data/Other/");
            if (!Directory.Exists("./Security/Data/Private/")) Directory.CreateDirectory("./Security/Data/Private/");
            if (!Directory.Exists("./Security/Data/Social Network/")) Directory.CreateDirectory("./Security/Data/Social Network/");

            this.key = key;
            RB_Application.IsChecked = true;
        }


        // Button function :
        private void Btn_Language_Click(object sender, RoutedEventArgs e)
        {
            var content = File.ReadAllBytes("./Security/Data/Application/1.dat"); //TODO REMOVE
            (byte[] IV, byte[] data) = KeyManager.Separator(content);
            var json = Decrypter.Decrypt(data, key, IV);
            AccountData result = JsonSerializer.Deserialize<AccountData>(json);

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
        private void Btn_Add_Click(object sender, RoutedEventArgs e)
        {
            AddView addView = new AddView(currentPath + GetIndexFile() + ".dat", key, this, GetIndexFile());
            addView.Show();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var RB = (RadioButton)sender;
            if (RB.Name == "RB_Application")
            {
                currentPath = "./Security/Data/Application/";
                if (parameters != null && parameters.language == "us") Text_WindowName.Text = "Apps";
                if (parameters != null && parameters.language == "fr") Text_WindowName.Text = "Applications";
            }
            else if (RB.Name == "RB_SocialNetwork")
            {
                currentPath = "./Security/Data/Social Network/";
                if (parameters != null && parameters.language == "us") Text_WindowName.Text = "Social Network";
                if (parameters != null && parameters.language == "fr") Text_WindowName.Text = "Réseaux sociaux";
            }
            else if (RB.Name == "RB_Game")
            {
                currentPath = "./Security/Data/Game/";
                if (parameters != null && parameters.language == "us") Text_WindowName.Text = "Games";
                if (parameters != null && parameters.language == "fr") Text_WindowName.Text = "Jeux";
            }
            else if (RB.Name == "RB_Private")
            {
                currentPath = "./Security/Data/Private/";
                if (parameters != null && parameters.language == "us") Text_WindowName.Text = "Private";
                if (parameters != null && parameters.language == "fr") Text_WindowName.Text = "Privée";
            }
            else if (RB.Name == "RB_Other")
            {
                currentPath = "./Security/Data/Other/";
                if (parameters != null && parameters.language == "us") Text_WindowName.Text = "Others";
                if (parameters != null && parameters.language == "fr") Text_WindowName.Text = "Autres";
            }
            UpdateList();
        }
        private void Button_Trash_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (File.Exists(currentPath + button.Tag + ".dat")) File.Delete(currentPath + button.Tag + ".dat");
            UpdateList();
        }




        // Other function :
        public void UpdateList()
        {
            ListView.Items.Clear();
            var files = Directory.GetFiles(currentPath);
            if (files.Length == 0) return;

            foreach (string file in Directory.GetFiles(currentPath))
            {
                var content = File.ReadAllBytes(file);
                (byte[] IV, byte[] data) = KeyManager.Separator(content);
                var json = Decrypter.Decrypt(data, key, IV);
                AccountData? result = JsonSerializer.Deserialize<AccountData>(json);
                ListView.Items.Add(result);

            }
        }
        private int GetIndexFile()
        {
            int maxIndex = 0;
            var files = Directory.GetFiles(currentPath);
            if (files.Length == 0) return 1;

            foreach (string file in Directory.GetFiles(currentPath))
            {
                string fileName = System.IO.Path.GetFileName(file);
                if (fileName.EndsWith(".dat") && int.TryParse(System.IO.Path.GetFileNameWithoutExtension(fileName), out int fileNumber)) maxIndex = Math.Max(maxIndex, fileNumber);
            }
            return maxIndex + 1;
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
