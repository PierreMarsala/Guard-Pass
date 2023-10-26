using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Guard_Pass.Languages
{
    public static class LanguagesManager
    {
        public static void RemoveCurrentLanguage()
        {
            ResourceDictionary targetDictionary = new ResourceDictionary();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                if (dictionary.Source != null && dictionary.Source.OriginalString.Contains("languages"))
                {
                    targetDictionary = dictionary;
                    break;
                }
            }
            if (targetDictionary != null) Application.Current.Resources.MergedDictionaries.Remove(targetDictionary);
        }

        public static void AddLanguage(string language)
        {
            ResourceDictionary languageDictionary = new ResourceDictionary();
            languageDictionary.Source = new Uri($"/Languages/{language}.xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(languageDictionary);
        }
    }
}
