using Guard_Pass.Methods;
using Guard_Pass.Models;
using System;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Guard_Pass.Views
{
    public partial class AddView : Window
    {
        public MainView mainView { get; set; }
        const string iconPath = "./Images/Templates/";
        string filePath = string.Empty;
        byte[] key;
        int ID = 0;
        public AddView(string path, byte[] key, MainView view, int ID)
        {
            InitializeComponent();
            filePath = path;
            this.key = key;
            this.mainView = view;
            this.ID = ID;
            PopulateComboBox();
        }

        // Button interaction :
        private void Btn_Valid_Click(object sender, RoutedEventArgs e)
        {

            AccountData data = new AccountData() { name = Textbox_Name.Text, icon = ComboBox_Icon.SelectedIndex , password = Textbox_Password.Text, user = Textbox_User.Text, ID = ID};
            data.imagePath = "pack://application:,,,/Images/Templates/" + (data.icon + 1).ToString() + ".png";
            var serialized = JsonSerializer.Serialize(data);
            var IV = KeyManager.IVGenerator();
            var encrypted = Encrypter.Encrypt(serialized, key, IV);
            KeyManager.DataToFile(filePath, encrypted, IV);
            mainView.UpdateList();
            this.Close();
        }
        private void ComboBox_Icon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Image_Icon.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/Templates/{ComboBox_Icon.SelectedIndex + 1}.png"));
        }


        // Other function :
        private void PopulateComboBox()
        {
            var enumIcons = Enum.GetNames(typeof(Icons));
            foreach (var name in enumIcons) ComboBox_Icon.Items.Add(name);
        }


        // Interface interaction :
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Textbox_Password_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox? TB = sender as TextBox;
            if (TB != null && TB.Text == "Password") TB.Text = string.Empty;
        }
        private void Textbox_User_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox? TB = sender as TextBox;
            if (TB != null && TB.Text == "User") TB.Text = string.Empty;
        }
        private void Textbox_Book_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox? TB = sender as TextBox;
            if (TB != null && TB.Text == "Name") TB.Text = string.Empty;
        }
    }
}
