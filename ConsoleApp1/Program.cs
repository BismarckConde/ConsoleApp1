using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public class Raza
{
    public string Nombre { get; set; }

    public Raza(string nombre)
    {
        Nombre = nombre;
    }
}

public class Perro : Mascota
{
    public Raza Raza { get; set; }

    public Perro(string nombre, int edad, double costo, Raza raza)
        : base(nombre, edad, costo)
    {
        Raza = raza;
    }
}

public partial class MainForm : Form
{
    // Lista para almacenar mascotas (gatos y perros)
    private List<Mascota> listaMascotas = new List<Mascota>();
    // Lista para almacenar razas
    private List<Raza> listaRazas = new List<Raza>();

    public MainForm()
    {
        InitializeComponent();

        // Agrega algunas razas de ejemplo
        listaRazas.Add(new Raza("Labrador"));
        listaRazas.Add(new Raza("Dachshund"));
        listaRazas.Add(new Raza("Bulldog"));
    }

    private void btnIngresarPerro_Click(object sender, EventArgs e)
    {
        // Ventana de diálogo para ingresar datos de un perro
        IngresarPerroForm ingresarPerroForm = new IngresarPerroForm(listaRazas);
        if (ingresarPerroForm.ShowDialog() == DialogResult.OK)
        {
            // Agregar el perro a la lista de mascotas
            listaMascotas.Add(ingresarPerroForm.Perro);
            ActualizarListView();
        }
    }

    private void btnMostrarMascotas_Click(object sender, EventArgs e)
    {
        MostrarRazaConMasPerros();
        MostrarCantidadPerrosPorRaza();
    }

    private void ActualizarListView()
    {
        // Limpiar la lista
        lvMascotas.Items.Clear();

        // Agregar elementos a la lista con los datos de las mascotas
        foreach (var mascota in listaMascotas)
        {
            ListViewItem item;

            if (mascota is Gato gato)
            {
                item = new ListViewItem(new[] { "Gato", gato.Nombre, gato.Edad.ToString(), gato.Costo.ToString(), gato.ColorPelo, "-" });
            }
            else if (mascota is Perro perro)
            {
                item = new ListViewItem(new[] { "Perro", perro.Nombre, perro.Edad.ToString(), perro.Costo.ToString(), perro.Raza.Nombre, "-" });
            }
            else
            {
                // Manejo de otros tipos de mascotas, si es necesario
                continue;
            }

            lvMascotas.Items.Add(item);
        }
    }

    private void MostrarRazaConMasPerros()
    {
        var razaConMasPerros = listaMascotas
            .OfType<Perro>()
            .GroupBy(perro => new { Raza = perro.Raza })
            .OrderByDescending(grupo => grupo.Count())
            .Select(grupo => grupo.Key.Nombre)
            .FirstOrDefault();

        if (razaConMasPerros != null)
        {
            MessageBox.Show($"La raza con más perros es: {razaConMasPerros}", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show("No hay perros registrados", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void MostrarCantidadPerrosPorRaza()
    {
        var cantidadPerrosPorRaza = listaMascotas
            .OfType<Perro>()
            .GroupBy(perro => new { Raza = perro.Raza.Nombre })
            .Select(grupo => new
            {
                Raza = grupo.Key.Raza,
                Cantidad = grupo.Count()
            })
            .ToList();

        if (cantidadPerrosPorRaza.Any())
        {
            var resultado = cantidadPerrosPorRaza.Select(item => $"{item.Raza}: {item.Cantidad} perros").ToList();
            MessageBox.Show(string.Join("\n", resultado), "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show("No hay perros registrados", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

public class IngresarPerroForm : Form
{
    public Perro Perro { get; private set; }

    private TextBox txtNombre;
    private TextBox txtEdad;
    private TextBox txtCosto;
    private ComboBox cmbRaza;
    private Button btnAceptar;
    private Button btnCancelar;

    private List<Raza> listaRazas;

    public IngresarPerroForm(List<Raza> razas)
    {
        listaRazas = razas;
        InitializeComponents();
        CargarRazasEnComboBox();
        btnAceptar.Click += BtnAceptar_Click;
        btnCancelar.Click += BtnCancelar_Click;
    }

    private void InitializeComponents()
    {
        // Agrega los controles necesarios para ingresar datos de un perro
        // (puedes personalizar según tus necesidades)
    }

    private void CargarRazasEnComboBox()
    {
        cmbRaza.DataSource = listaRazas;
        cmbRaza.DisplayMember = "Nombre";
    }

    private void BtnAceptar_Click(object sender, EventArgs e)
    {
        // Validación y creación del objeto Perro
        if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtEdad.Text) ||
            string.IsNullOrWhiteSpace(txtCosto.Text) || cmbRaza.SelectedItem == null)
        {
            MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (!int.TryParse(txtEdad.Text, out int edad) || !double.TryParse(txtCosto.Text, out double costo))
        {
            MessageBox.Show("Ingrese valores válidos para Edad y Costo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var razaSeleccionada = (Raza)cmbRaza.SelectedItem;
        Perro = new Perro(txtNombre.Text, edad, costo, razaSeleccionada);
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnCancelar_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
