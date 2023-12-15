using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Firebase.Database;
using Firebase.Storage;
using Plugin.Maui.Audio;
using PM2E30047.Modelos;
using Firebase.Database.Query;

namespace PM2E30047.Vistas
{

    public partial class UpdatePage : ContentPage
    {
        private FirebaseClient client = new FirebaseClient("https://basegrupo2-default-rtdb.firebaseio.com/");
        private string Urlfoto { get; set; }
        private Empleado empleadoToUpdate;

        private string token = string.Empty;
        private string rutastorage = "basegrupo2.appspot.com";
        private string lblaudios, lblfoto;

        private readonly IAudioRecorder _audioRecorder;
        private bool isRecording = false;
        public string pathaudio, filename;

        public UpdatePage(Empleado empleadoToUpdate)
        {
            InitializeComponent();
            _audioRecorder = new AudioManager().CreateRecorder();

            BindingContext = this;
            this.empleadoToUpdate = empleadoToUpdate;

            txtDesc.Text = empleadoToUpdate.descripcion;
            lblfoto = empleadoToUpdate.Urlfoto;
        }

        Empleado detalle;
        public Empleado Detalle
        {
            get => detalle;
            set
            {
                detalle = value;
                OnPropertyChanged();
            }

        }



        private async void Guardar_Clicked(object sender, EventArgs e)
        {
            DateTime selectedDate = fecha.Date;
            if (string.IsNullOrEmpty(txtDesc.Text))
            {
                alertN();
            }
            else
            {
                if (selectedDate < DateTime.Now)
                {
                    await DisplayAlert("Error", "La fecha seleccionada no puede ser menor que la fecha actual", "OK");
                    return; // Salir del método si la fecha es incorrecta
                }

                await client.Child("Empleados").Child(empleadoToUpdate.Id).PutAsync(new Empleado
                {
                    Id = empleadoToUpdate.Id,
                    descripcion = txtDesc.Text,
                    Urlfoto = lblfoto,
                    fecha = selectedDate.ToString("yyyy-MM-dd")
                });

                await Shell.Current.GoToAsync("..");
                alertP();
            }
        }

        private void GenerarNumero()
        {
            Random random = new Random();
            int numeroAleatorio = random.Next(1, 100);
            entryNumero.Text = numeroAleatorio.ToString();
        }


        private async void alertP()
        {
            await DisplayAlert("Registro actualizado correctamente", "Correctamente", "OK");
        }

        private async void alertN()
        {
            await DisplayAlert("Registro no actualizado", "Rellene todos los campos", "OK");
        }

        private async void SeleccionarImagen_Clicked(object sender, EventArgs e)
        {
            var foto = await Microsoft.Maui.Media.MediaPicker.PickPhotoAsync();
            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();
                Urlfoto = await new FirebaseStorage("basegrupo2.appspot.com")
                    .Child("Fotos")
                    .Child(DateTime.Now.ToString("ddMMyyyyhhmmss") + foto.FileName)
                    .PutAsync(stream);
                fotoImage.Source = Urlfoto;
            }
        }
    }
}
