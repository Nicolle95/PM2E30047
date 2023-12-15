using Firebase.Database;
using Firebase.Database.Query;
using PM2E30047.Modelos;
using Firebase.Storage;
using Plugin.Maui.Audio;

namespace PM2E30047.Vistas;

public partial class Create : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://basegrupo2-default-rtdb.firebaseio.com/");
    private string Urlfoto { get; set; }
    readonly IAudioManager _audioManager;
    readonly IAudioRecorder _audioRecorder;
    private Empleado alumnosService;

    public Create(IAudioManager audioManager)
    {
        InitializeComponent();
        _audioManager = audioManager;
        _audioRecorder = audioManager.CreateRecorder();
        BindingContext = this;
        alumnosService = new Empleado();
        GenerarNumero();

    }


    private async void Guardar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtDesc.Text))
        {
            alertN();
        }
        else
        {
            DateTime selectedDate = fecha.Date;
            int currentCounter = await alumnosService.GetCounterAsync();
            int newId = currentCounter + 1;

            if (selectedDate < DateTime.Now)
        {
            await DisplayAlert("Error", "La fecha seleccionada no puede ser menor que la fecha actual", "OK");
            return; // Salir del método si la fecha es incorrecta
        }

            await client.Child("Empleados").Child(newId.ToString()).PutAsync(new Empleado
            {
                Id = newId.ToString(),
                descripcion = txtDesc.Text,
                Urlfoto = Urlfoto,
                fecha = selectedDate.ToString("yyyy-MM-dd")
            });
            await alumnosService.UpdateCounterAsync(newId);
            await Shell.Current.GoToAsync("..");
            alertP();


        }
    }

    private void GenerarNumero()
    {
        Random random = new Random();
        int numeroAleatorio = random.Next(1, 100); // Genera un número aleatorio entre 1 y 100

        // Asigna el número aleatorio al Entry
        entryNumero.Text = numeroAleatorio.ToString();
    }

    private async void Guardar_Audio(object sender, EventArgs e)
    {
        if (await Permissions.RequestAsync<Permissions.Microphone>() != PermissionStatus.Granted)
        {
            return;
        }
        if (!_audioRecorder.IsRecording)
        {
            await _audioRecorder.StartAsync();
        }
        else
        {
            var recorderdAudio = await _audioRecorder.StopAsync();
            var player = AudioManager.Current.CreatePlayer(recorderdAudio.GetAudioStream());
            player.Play();
        }
    }

    private async void alertP()
    {
        await DisplayAlert("Registro añadido Correctamente", "Correctamente", "OK");
    }

    private async void alertN()
    {
        await DisplayAlert("Registro no Realizado", "Rellene todo los campos", "OK");
    }

    private async void SeleccionarImagen_Clicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.PickPhotoAsync();
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