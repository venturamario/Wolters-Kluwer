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
        private Button btnDelete = new Button();

        public Form1()
        {
            InitializeComponent();

        dataGridView1.Dock = DockStyle.Top;
        dataGridView1.Height = 400;
        dataGridView1.DataSource = clientList;
        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridView1.CellValidating += DataGridView1_CellValidating;

        dataGridView1.AllowUserToAddRows = true;    // Permite añadir una fila vacía al final
        dataGridView1.AllowUserToDeleteRows = true; // Permite borrar filas con la tecla 'Supr'
        dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2; // Edición in-line 

        this.Controls.Add(dataGridView1);

            // Configuración de la Barra de Progreso
            progressBar1.Location = new Point(10, 420);
            progressBar1.Size = new Size(760, 23);
            this.Controls.Add(progressBar1);

            // Configuración import button
            btnImport.Text = "Importar Datos";
            btnImport.Location = new Point(10, 460);
            btnImport.Size = new Size(200, 40);
            btnImport.Click += BtnImport_Click;
            this.Controls.Add(btnImport);

            // Configuración delete button
            btnDelete.Text = "Eliminar Fila";
            btnDelete.Location = new Point(220, 460); // Al lado del de importar
            btnDelete.Size = new Size(150, 40);
            btnDelete.Click += (s, e) => {
                if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
                {
                    var result = MessageBox.Show("¿Seguro que deseas eliminar este cliente?", "Confirmar", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        clientList.RemoveAt(dataGridView1.CurrentRow.Index);
                    }
                }
            };
            this.Controls.Add(btnDelete);

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
    
        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Solo validamos si la celda editada es la de "Dni"
            if (dataGridView1.Columns[e.ColumnIndex].DataPropertyName == "Dni")
            {
                string nuevoDni = e.FormattedValue.ToString();
                
                // Evitar validación si el campo está vacío (ya que es requisito que no lo esté)
                if (string.IsNullOrWhiteSpace(nuevoDni)) return;

                // Buscamos si ese DNI ya existe en OTRO registro que no sea el que estamos editando
                bool existe = clientList.Any(c => c.DNI == nuevoDni && clientList.IndexOf(c) != e.RowIndex);

                if (existe)
                {
                    var result = MessageBox.Show(
                        $"El DNI '{nuevoDni}' ya existe. ¿Deseas sobreescribir la lista completa con este nuevo registro? (Si eliges 'No', se permitirá el duplicado)",
                        "DNI Duplicado detectado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Sobreescribir según tu requerimiento: borrar todo y empezar de nuevo
                        // Usamos BeginInvoke para no interrumpir el ciclo de validación de la celda
                        this.BeginInvoke(new MethodInvoker(() => {
                            var clienteUnico = new Client { DNI = nuevoDni };
                            clientList.Clear();
                            clientList.Add(clienteUnico);
                        }));
                    }
                    // Si el usuario elige 'No', la validación pasa y se permite el duplicado.
                }
            }
        }
    }
}