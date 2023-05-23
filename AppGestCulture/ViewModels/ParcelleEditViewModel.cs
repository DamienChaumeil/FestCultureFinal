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
using Android.Accounts;
using System.Text.RegularExpressions;

namespace AppGestCulture.ViewModels
{
    public partial class ParcelleEditViewModel
    {
        public ObservableCollection<Espece> obEspece { get; private set; } = new ObservableCollection<Espece>();
        public Parcelle Parcelle { get; private set; }

        private static Database database = null;
        public ICommand btnUpdateParcelle { get; set; }
        public ICommand btnRemoveParcelle{ get; set; }

        private INavigation navigation;

        public ParcelleEditViewModel(Parcelle parcelle, INavigation _navigation)
        {
            navigation = _navigation;
            btnUpdateParcelle = new Command(async () => updateParcelle());
            btnRemoveParcelle = new Command(async () => removeParcelle());

            GetAllEspece();

            Parcelle = parcelle;
        }

        private async Task updateParcelle()
        {
            Regex regex = new Regex(@"^(?:(?:\+|00)33[\s.-]{0,3}(?:\(0\)[\s.-]{0,3})?|0)[1-9](?:(?:[\s.-]?\d{2}){4}|\d{2}(?:[\s.-]?\d{3}){2})$");
            Match match = regex.Match(Parcelle.Numero);

            if(match.Success) {

                MessagingCenter.Send(this, "UpdateParcelle", Parcelle);

                await customPopAsync(2);
            }else
            {
                App.Current.MainPage.DisplayAlert("Alert", "le numéro de téléphone n'est pas valide (ex:0606060606)", "OK");
            }

        }
        private async Task removeParcelle()
        {
            if (await App.Current.MainPage.DisplayAlert("Alert", $"Voulez vous supprimer {Parcelle.Code_parc}?", "Yes", "No"))
            {
                MessagingCenter.Send(this, "DeleteParcelle", Parcelle);
                await customPopAsync(2);
            }
        }
        public async Task GetAllEspece()
        {
            var especes = await GetConnection().GetAllEspece();

            foreach (var espece in especes)
                obEspece.Add(espece);

            OnPropertyChanged();
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
        private async Task customPopAsync(int counter)
        {
            for (var i = 1; i < counter; i++)
            {
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
            }
            await Navigation.PopAsync();
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
