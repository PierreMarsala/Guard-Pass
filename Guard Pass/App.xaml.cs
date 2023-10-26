using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Guard_Pass.Views;

namespace Guard_Pass
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        const string accountPath = "./Security/Account/account.dat";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (File.Exists(accountPath))
            {
                LoginView loginView = new LoginView();
                loginView.Show();
            }
            else
            {
                RegisterView registerView = new RegisterView();
                registerView.Show();
            }
        }
    }
}
