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
            btnDelete.Location = new Point(220, 460);
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            File.WriteAllText("clients_store.json", JsonConvert.SerializeObject(clientList, Formatting.Indented));
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de datos (*.csv;*.json)|*.csv;*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var service = new ImportService();
                    List<Client> rawData = null;
                    string ext = Path.GetExtension(openFileDialog.FileName).ToLower();

                    try 
                    {
                        if (ext == ".csv")
                            rawData = service.ImportFromCsv(openFileDialog.FileName, p => progressBar1.Value = p);
                        else
                            rawData = service.ImportFromJson(openFileDialog.FileName);

                        if (rawData != null && rawData.Count > 0)
                        {
                            // --- CORRECCIÓN 1: Filtrar solo datos válidos para evitar errores al importar ---
                            var imported = rawData.Where(c => 
                                DataValidator.IsValidDni(c.DNI) && 
                                (string.IsNullOrEmpty(c.Email) || DataValidator.IsValidEmail(c.Email))
                            ).ToList();

                            if (imported.Count == 0)
                            {
                                MessageBox.Show("El archivo no contiene registros con formato de DNI válido.", "Importación cancelada");
                                return;
                            }

                            // Lógica de duplicados (se mantiene igual)
                            bool existsAny = imported.Any(newCl => clientList.Any(curr => curr.DNI == newCl.DNI));
                            if (existsAny)
                            {
                                var result = MessageBox.Show(
                                    "Algunas filas ya existen. ¿Deseas sobreescribir los datos actuales?",
                                    "Aviso de Duplicados",
                                    MessageBoxButtons.YesNoCancel,
                                    MessageBoxIcon.Warning);

                                if (result == DialogResult.Yes) clientList.Clear();
                                else if (result == DialogResult.Cancel) return;
                            }

                            foreach (var c in imported) clientList.Add(c);
                            MessageBox.Show($"Procesados {imported.Count} registros válidos (se omitieron {rawData.Count - imported.Count} inválidos).");
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                    finally { progressBar1.Value = 0; }
                }
            }
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!dataGridView1.IsCurrentCellDirty) return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
            string value = e.FormattedValue?.ToString() ?? "";

            // 1. VALIDACIÓN DE DNI
            if (columnName == "DNI" || columnName == "Dni")
            {
                // Si borra el DNI y lo deja vacío, no lanzamos duplicados (podemos dejar que el modelo gestione el nulo)
                if (string.IsNullOrWhiteSpace(value)) return;
                
                bool existeDuplicado = clientList.Any(c => c.DNI == value && clientList.IndexOf(c) != e.RowIndex);
                
                if (existeDuplicado)
                {
                    var result = MessageBox.Show(
                        $"El DNI '{value}' ya existe. ¿Deseas sobreescribir la lista completa con este nuevo registro?",
                        "DNI Duplicado detectado",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        this.BeginInvoke(new MethodInvoker(() => {
                            var clienteUnico = new Client { DNI = value };
                            clientList.Clear();
                            clientList.Add(clienteUnico);
                        }));
                        return;
                    }
                }

                if (!DataValidator.IsValidDni(value))
                {
                    MessageBox.Show("DNI inválido. Asegúrese de que la letra corresponde al número.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            
            // 2. VALIDACIÓN DE EMAIL
            else if (columnName == "Email")
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                if (!DataValidator.IsValidEmail(value))
                {
                    MessageBox.Show("El formato del email no es correcto.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }

            // 3. VALIDACIÓN DE TELÉFONO
            else if (columnName == "Phone")
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                if (!DataValidator.IsValidPhone(value))
                {
                    MessageBox.Show("El teléfono debe ser numérico (9-15 dígitos).", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }
    }
}