using PM2E30047.Modelos;

namespace PM2E30047.Vistas;

[QueryProperty(nameof(Detalle), "Detalle")]

public partial class PageView : ContentPage
{
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
    public PageView()
	{
		InitializeComponent();
        BindingContext = this;
    }
}