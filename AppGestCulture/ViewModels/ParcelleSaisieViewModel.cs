using Android.Util;
using AppGestCulture.Models;
using AppGestCulture.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Android.Net.Wifi.WifiEnterpriseConfig;
using static Android.Provider.CalendarContract;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using Android.App;

namespace AppGestCulture.ViewModels
{
    public partial class ParcelleSaisieViewModel
    {
        public ObservableCollection<Espece> obEspece { get; private set; } = new ObservableCollection<Espece>();
        public Parcelle Parcelle { get; private set; }
        public Exploitation Exploitation { get; private set; }


        private static Database database = null;
        public ICommand btnAddParcelle { get; set; }
        public int Id_espece { get; set; }

        private INavigation navigation;

        public ParcelleSaisieViewModel(Exploitation exploitation, INavigation _navigation)
        {
            navigation = _navigation;
            btnAddParcelle = new Command(async () => addParcelle());

            GetAllEspece();

            Exploitation = exploitation;

            var date = DateTime.Now.ToString("yyyy");

            DateTime dt1 = DateTime.Parse("01/07/"+date);
            if (dt1.Date > DateTime.Now)
            {
                var newDate = int.Parse(date) - 1;
                date = newDate.ToString();
            }

            Parcelle = new Parcelle
            {
                Id_exploi = exploitation.Id_exploi,
                Id_espece = 1,
                Code_parc = "",
                Surface = 0,
                Rendement_prev = 0,
                Rendement_reel = 0,
                Annee = date,
                Numero = "06060606",
            };
        }

        Espece _yourSelectedItem;
        public Espece SelectedEspece
        {
            get
            {
                return _yourSelectedItem;
            }
            set
            {
                _yourSelectedItem = value;
                Parcelle.Id_espece = value.Id_espece;
                OnPropertyChanged("YourSelectedItem");
            }
        }

        public async Task GetAllEspece()
        {
            var especes = await GetConnection().GetAllEspece();

            foreach (var espece in especes)
                obEspece.Add(espece);

            OnPropertyChanged();
        }
        private async Task addParcelle()
        {
            
            Regex regex = new Regex(@"^(?:(?:\+|00)33[\s.-]{0,3}(?:\(0\)[\s.-]{0,3})?|0)[1-9](?:(?:[\s.-]?\d{2}){4}|\d{2}(?:[\s.-]?\d{3}){2})$");
            Match match = regex.Match(Parcelle.Numero);

            if (match.Success)
            {
                await GetConnection().InsertParcelle(Parcelle);
                MessagingCenter.Send(this, "AddParcelle", Parcelle);
                await Navigation.PopAsync();
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Alert", "le numéro de téléphone n'est pas valide (ex:0606060606)", "OK");
            }

            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public INavigation Navigation
        {
            get { return navigation; }
            set { navigation = value; }
        }

        private static Database GetConnection()
        {
            if (database == null)
                database = new Database();
            return database;
        }
    }
}
