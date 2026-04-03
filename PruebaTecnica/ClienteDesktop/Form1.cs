using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClienteDesktop.Models;
using ClienteDesktop.Services;
using Newtonsoft.Json;

namespace ClienteDesktop
{
    public partial class Form1 : Form
    {
        private DataGridView dataGridView1 = new DataGridView();
        private BindingList<Client> clientList = new BindingList<Client>();
        private ProgressBar progressBar1 = new ProgressBar();
        private Button btnImport = new Button();

        public Form1()
        {
            InitializeComponent();

            // Configuración de la Rejilla
            dataGridView1.Dock = DockStyle.Top;
            dataGridView1.Height = 400;
            dataGridView1.DataSource = clientList;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(dataGridView1);

            // Configuración de la Barra de Progreso
            progressBar1.Location = new Point(10, 420);
            progressBar1.Size = new Size(760, 23);
            this.Controls.Add(progressBar1);

            // Configuración del Botón
            btnImport.Text = "Importar Datos";
            btnImport.Location = new Point(10, 460);
            btnImport.Size = new Size(200, 40);
            btnImport.Click += BtnImport_Click;
            this.Controls.Add(btnImport);

            LoadDataOnStartup();
        }

        private void LoadDataOnStartup()
        {
            string path = "clients_store.json";
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject<List<Client>>(json);
                if (data != null)
                {
                    foreach (var c in data) clientList.Add(c);
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de datos (*.csv;*.json)|*.csv;*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var service = new ImportService();
                    List<Client> imported = null;
                    string ext = Path.GetExtension(openFileDialog.FileName).ToLower();

                    try 
                    {
                        if (ext == ".csv")
                            imported = service.ImportFromCsv(openFileDialog.FileName, p => progressBar1.Value = p);
                        else
                            imported = service.ImportFromJson(openFileDialog.FileName);

                        if (imported != null && imported.Count > 0)
                        {
                            // Comprobar si algún DNI ya existe en la lista actual
                            bool existsAny = imported.Any(newCl => clientList.Any(curr => curr.DNI == newCl.DNI));

                            if (existsAny)
                            {
                                var result = MessageBox.Show(
                                    "Algunas filas ya existen. ¿Deseas sobreescribir los datos actuales? (Si eliges 'No', se añadirán duplicándolos)",
                                    "Aviso de Duplicados",
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Warning);

                                if (result == DialogResult.Yes)
                                {
                                    clientList.Clear(); // Sobreescribir: borra todo lo actual [cite: 14]
                                }
                                else if (result == DialogResult.Cancel)
                                {
                                    return; // No hace nada
                                }
                                // Si es 'No', simplemente sigue y los añade al final
                            }

                            foreach (var c in imported)
                            {
                                clientList.Add(c);
                            }
                            MessageBox.Show($"Procesados {imported.Count} registros.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                    finally { progressBar1.Value = 0; }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            File.WriteAllText("clients_store.json", JsonConvert.SerializeObject(clientList, Formatting.Indented));
        }
    }
}