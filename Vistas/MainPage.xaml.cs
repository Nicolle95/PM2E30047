using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using PM2E30047.Modelos;
using System.Collections.ObjectModel;

namespace PM2E30047.Vistas
{
    public partial class MainPage : ContentPage
    {
        FirebaseClient client = new FirebaseClient("https://basegrupo2-default-rtdb.firebaseio.com/");
        public ObservableCollection<Empleado> Lista { get; set; } = new ObservableCollection<Empleado>();
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            CargarLista();
        }

        public void CargarLista()
        {
            client.Child("Empleados")
            .AsObservable<Empleado>()
            .Subscribe((empleado) =>
            {
                if (empleado.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                {
                    // Eliminar el elemento de la lista local
                    var empleadoToDelete = Lista.FirstOrDefault(e => e.Id == empleado.Object.Id);
                    if (empleadoToDelete != null)
                    {
                        Lista.Remove(empleadoToDelete);
                    }
                }
                else if (empleado.Object != null)
                {
                    // Agregar o actualizar el elemento en la lista local
                    var existingEmpleado = Lista.FirstOrDefault(e => e.Id == empleado.Object.Id);
                    if (existingEmpleado != null)
                    {
                        // Actualizar el elemento existente
                        Lista.Remove(existingEmpleado);
                    }

                    Lista.Add(empleado.Object);
                }
                OrdenarListaPorFecha();
            });
        }
        private async void nuevoReg_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Create));
        }
        private void filtroEntry_TextChanged(object sender, EventArgs e)
        {
            string filtro = filtroEntry.Text.ToLower();
            if (filtro.Length > 0)
            {
                listaCollection.ItemsSource = Lista.Where(x => x.descripcion.ToLower().Contains(filtro));
            }
            else
            {
                listaCollection.ItemsSource = Lista;
            }
            OrdenarListaPorFecha();
        }

        private async void listaCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Empleado empleado = e.CurrentSelection.FirstOrDefault() as Empleado;
            if (empleado != null)
            {
                DisplayAlert("Ha seleccionado la nota con el ID ", $"ID: {empleado.Id}", "OK");
            }
        }

        private void OrdenarListaPorFecha()
        {
            Lista = new ObservableCollection<Empleado>(Lista.OrderByDescending(e => DateTime.Parse(e.fecha)));
            listaCollection.ItemsSource = Lista;
        }
        private async void Eliminar_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (listaCollection.SelectedItem is Empleado empleado)
                {
                    Console.WriteLine($"Eliminando Empleado con ID: {empleado.Id}, Descripción: {empleado.descripcion}");

                    await client.Child("Empleados").Child(empleado.Id).DeleteAsync();

                    Lista.Remove(empleado);

                    listaCollection.ItemsSource = Lista.ToList();
                }
                else
                {
                    await DisplayAlert("Alerta", "Selecciona un empleado para eliminar", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el registro: {ex.Message}");
            }
        }


        private async void Actualizar_Clicked(object sender, EventArgs e)
        {
            Empleado empleado = listaCollection.SelectedItem as Empleado;

            if (empleado != null)
            {
                await Navigation.PushAsync(new UpdatePage(empleado));
            }
            else
            {
                await DisplayAlert("Alerta", "Selecciona un empleado para actualizar", "OK");
            }
        }

    }
}