using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database.Query;
using Firebase.Database;
using Microsoft.Maui.Storage;

namespace PM2E30047.Modelos
{
    public class Empleado
    {

        FirebaseClient client = new FirebaseClient("https://basegrupo2-default-rtdb.firebaseio.com/");
        public async Task<int> GetCounterAsync()
        {
            var counterSnapshot = await client.Child("contador").OnceSingleAsync<int?>();
            return counterSnapshot ?? 0;
        }

        public async Task UpdateCounterAsync(int newCounterValue)
        {
            await client.Child("contador").PutAsync(newCounterValue);
        }

        public string Id { get; set; } // Ajustar el nombre de la propiedad según el nombre en Firebase
        public string descripcion { get; set; }
        public string fecha { get; set; }
        public string Urlfoto { get; set; }
    }
}
