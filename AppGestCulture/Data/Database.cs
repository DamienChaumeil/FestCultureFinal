using AppGestCulture.Models;
using SQLite;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.ObjectModel;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensionsAsync.Extensions;
using Java.Security;
using System.Security.Cryptography;
using Android.Util;

namespace AppGestCulture.Data
{

    public class Database
    {
        readonly SQLiteAsyncConnection connection;
        public Database()
        {
            connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            /*connection.DropTableAsync<Technicien>();
            connection.DropTableAsync<Espece>();
            connection.DropTableAsync<Exploitation>();
            connection.DropTableAsync<Parcelle>();*/
            connection.CreateTableAsync<Technicien>().Wait();
            connection.CreateTableAsync<Espece>().Wait();
            connection.CreateTableAsync<Exploitation>().Wait();
            connection.CreateTableAsync<Parcelle>().Wait();
        }
        public async Task<IEnumerable<Espece>> GetAllEspece()
        {
            var especes = await connection.Table<Espece>().ToListAsync();
            if (especes.Count() == 0)
            {
                await InsertEspece(new Espece() { Nom = "Blé" });
                await InsertEspece(new Espece() { Nom = "Orge" });
                await InsertEspece(new Espece() { Nom = "Betteraves" });
            }
            return await connection.Table<Espece>().ToListAsync();
        }
        public async Task<bool> CheckTechnicienByInfo(string username, string password)
        {
            password = HashString(password);
            var result = await connection.Table<Technicien>().Where(t => (t.Matricule == username && t.Mdp == password)).ToListAsync();
            return result.Count > 0;
        }
        public Task<List<Technicien>> GetAllTechnicien()
        {
            return connection.Table<Technicien>().ToListAsync();
        }
        public async Task<int> InsertTechnicien(Technicien technicien)
        {
            technicien.Mdp = HashString(technicien.Mdp);
            Log.Info("myapp", technicien.Mdp);
            return await connection.InsertAsync(technicien);
        }
        public async Task<int> InsertEspece(Espece espece)
        {
            return await connection.InsertAsync(espece);
        }
        public async Task<Espece> GetEspece(int id_espece)
        {
            return await connection.FindAsync<Espece>(id_espece);
        }

        public async Task<Technicien> GetTechnicien(int id_technicien)
        {
            return await connection.FindAsync<Technicien>(id_technicien);
        }


        public async Task<int> InsertParcelle(Parcelle parcelle)
        {
            return await connection.InsertAsync(parcelle);
        }
        public Task<int> UpdateParcelle(Parcelle parcelle)
        {
            return connection.UpdateAsync(parcelle);
        }
        public Task<int> DeleteParcelle(Parcelle parcelle)
        {
            return connection.DeleteAsync(connection.FindAsync<Parcelle>(parcelle.Id_parc));
        }
        public async Task<IEnumerable<Parcelle>> GetAllParcelleById(Exploitation exploitation)
        {
            return await connection.Table<Parcelle>().Where(e => e.Id_exploi == exploitation.Id_exploi).ToListAsync();
        }
        public async Task<Parcelle> GetParcelle(int id_parcelle)
        {
            return await connection.FindAsync<Parcelle>(id_parcelle);
        }


        public async Task<int> InsertExploitation(Exploitation exploitation)
        {
            return await connection.InsertAsync(exploitation);
        }
        public Task UpdateExploitation(Exploitation exploitation)
        {
            return connection.UpdateWithChildrenAsync(exploitation);
        }
        public Task<int> DeleteExploitation(Exploitation exploitation)
        {
            return connection.DeleteAsync(exploitation);
        }
        public async Task<IEnumerable<Exploitation>> GetAllExploitation()
        {
            await connection.CreateTableAsync<Exploitation>();
            return await connection.GetAllWithChildrenAsync<Exploitation>();
        }
        public async Task<Exploitation> GetExploitation(int id_exploitation)
        {
            return await connection.GetWithChildrenAsync<Exploitation>(id_exploitation);
        }

        static string HashString(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + Constants.Salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }
    }
}