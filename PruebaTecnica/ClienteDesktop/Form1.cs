using System.ComponentModel;
using ClienteDesktop.Models;
using Newtonsoft.Json;

public partial class Form1 : Form
{
    private DataGridView dataGridView1 = new DataGridView();
    private BindingList<Client> clientList = new BindingList<Client>();
    private ProgressBar progressBar1 = new ProgressBar();
    private Button btnImport = new Button();

    public Form1()
    {
        // Configuración básica de la ventana
        this.Text = "Importación de Clientes";
        this.Size = new Size(800, 500);

        // 1. DataGridView (La rejilla)
        dataGridView1.Dock = DockStyle.Top;
        dataGridView1.Height = 300;
        dataGridView1.DataSource = clientList; // Vinculación en memoria [cite: 14]
        this.Controls.Add(dataGridView1);

        progressBar1.Dock = DockStyle.Bottom;
        this.Controls.Add(progressBar1);

        // 3. Botón Importar
        btnImport.Text = "Importar CSV/JSON";
        btnImport.Location = new Point(10, 320);
        btnImport.Click += BtnImport_Click;
        this.Controls.Add(btnImport);
        LoadDataOnStartup();
    }

    private void LoadDataOnStartup() {
        string path = "clients_store.json";
        if (File.Exists(path)) {
            var data = JsonConvert.DeserializeObject<List<Client>>(File.ReadAllText(path));
            foreach (var c in data) clientList.Add(c);
        }
    }
}