using System.ComponentModel;
using ClienteDesktop.Models;
using ClienteDesktop.Services;
using ClienteDesktop.Services.Repository;
using ClienteDesktop.Helpers;

namespace ClienteDesktop
{
    public partial class Form1 : Form
    {
        #region Vars
        private DataGridView dataGridView1 = new DataGridView();
        private BindingList<Client> clientList = new BindingList<Client>();
        private ProgressBar progressBar1 = new ProgressBar();
        private Button btnImport = new Button();
        private Button btnDelete = new Button();
        
        // SOLID: Usamos una variable privada para el servicio y el repo
        private readonly IClientRepository _clientRepository;
        private readonly ImportService _importService;
        #endregion

        #region Constructors
        public Form1()
        {
            InitializeComponent();
            
            _clientRepository = new ClientRepository();
            _importService = new ImportService();

            ConfigureGrid();
            ConfigureControls();
            LoadData();
        }
        #endregion

        #region UI Configuration
        private void ConfigureGrid()
        {
            dataGridView1.Dock = DockStyle.Top;
            dataGridView1.Height = 400;
            dataGridView1.DataSource = clientList;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.AllowUserToAddRows = true;
            dataGridView1.AllowUserToDeleteRows = true;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.Controls.Add(dataGridView1);
        }

        private void ConfigureControls()
        {
            progressBar1.Location = new Point(10, 420);
            progressBar1.Size = new Size(760, 23);
            this.Controls.Add(progressBar1);

            btnImport.Text = "Importar Datos";
            btnImport.Location = new Point(10, 460);
            btnImport.Size = new Size(200, 40);
            btnImport.Click += BtnImport_Click;
            this.Controls.Add(btnImport);

            btnDelete.Text = "Eliminar Fila";
            btnDelete.Location = new Point(220, 460);
            btnDelete.Size = new Size(150, 40);
            btnDelete.Click += (s, e) => {
                if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
                {
                    if (MessageBox.Show("¿Seguro que deseas eliminar?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        clientList.RemoveAt(dataGridView1.CurrentRow.Index);
                    }
                }
            };
            this.Controls.Add(btnDelete);
        }
        #endregion

        #region Protected Methods
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _clientRepository.SaveAll(clientList);
        }
        #endregion

        #region Private Logic Methods
        private void LoadData() 
        {
            var datos = _clientRepository.GetAll();
            foreach(var c in datos) clientList.Add(c);
        }
        
        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de datos (*.csv;*.json)|*.csv;*.json";
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                try 
                {
                    List<Client> imported = _importService.Import(openFileDialog.FileName, p => progressBar1.Value = p);

                    if (imported == null || imported.Count == 0) return;

                    bool existsAny = imported.Any(newCl => clientList.Any(curr => curr.DNI == newCl.DNI));
                    if (existsAny)
                    {
                        var result = MessageBox.Show("¿Deseas sobreescribir los datos actuales?", "Duplicados", MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes) clientList.Clear();
                        else if (result == DialogResult.Cancel) return;
                    }

                    foreach (var c in imported) clientList.Add(c);
                    MessageBox.Show($"Importación finalizada con éxito.");
                }
                catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
                finally { progressBar1.Value = 0; }
            }
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!dataGridView1.IsCurrentCellDirty) return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].DataPropertyName;
            string value = e.FormattedValue?.ToString() ?? "";

            // DNI Validation
            if (columnName == "DNI" || columnName == "Dni")
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                
                if (clientList.Any(c => c.DNI == value && clientList.IndexOf(c) != e.RowIndex))
                {
                    MessageBox.Show("DNI ya existe.");
                    e.Cancel = true; 
                    return;
                }

                if (!Validator.IsValidDni(value))
                {
                    MessageBox.Show("DNI inválido.");
                    e.Cancel = true;
                }
            }
            
            // Email Validation
            else if (columnName == "Email")
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                if (!Validator.IsValidEmail(value))
                {
                    MessageBox.Show("El formato del email no es correcto.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }

            // Phone Validation
            else if (columnName == "Phone")
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                if (!Validator.IsValidPhone(value))
                {
                    MessageBox.Show("El teléfono debe ser numérico (9-15 dígitos).", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }
        #endregion
    }
}