using System.ComponentModel;
using ClienteDesktop.Models;
using ClienteDesktop.Services;
using ClienteDesktop.Services.Repository;
using ClienteDesktop.Helpers;
using ClienteDesktop.Helpers.ImageHelper;

namespace ClienteDesktop
{
    public partial class Form1 : Form
    {
        #region Constants
        private const int GRID_HEIGHT = 400;
        private const bool ALLOW_ADD_DELETE_ROWS = true;
        #endregion

        #region Vars
        private DataGridView dataGridView1 = new DataGridView();
        private BindingList<Client> clientList = new BindingList<Client>();
        private ProgressBar progressBar1 = new ProgressBar();
        private Button btnImport = new Button();
        private Button btnDelete = new Button();
        Panel headerPanel = new Panel();
        Panel footerPanel = new Panel();
        
        
        private readonly IClientRepository _clientRepository;
        private readonly ImportService _importService;
        private readonly ImageHelper _imageHelper;
        #endregion

        #region Constructors
        public Form1()
        {
            InitializeComponent();
            
            _clientRepository = new ClientRepository();
            _importService = new ImportService();
            _imageHelper = new ImageHelper();

            ConfigureHeader();
            ConfigureControls();
            ConfigureGrid();

            headerPanel.SendToBack();
            footerPanel.SendToBack();
            dataGridView1.BringToFront();

            LoadData();
        }
        #endregion

        #region UI Configuration
        private void ConfigureHeader()
        {
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 150;
            headerPanel.BackColor = Color.White;

            
            PictureBox pbLogo = new PictureBox();
            pbLogo.Size = new Size(80, 80);
            pbLogo.Location = new Point(20, 20);
            pbLogo.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                Image woltersLogo = Image.FromFile("images/Icons/wolters.png");
                pbLogo.Image = _imageHelper.ResizeImage(woltersLogo,  pbLogo.Size);
            } catch
            {
                Console.WriteLine("Error con la imagen del header");
            }

            Label lblTitle = new Label();
            lblTitle.Text = "Prueba técnica Wolters Kluwer";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.Location = new Point(120, 25);
            lblTitle.AutoSize = true;

            // 3. TU NOMBRE
            Label lblAuthor = new Label();
            lblAuthor.Text = "Mario Ventura";
            lblAuthor.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblAuthor.ForeColor = Color.Gray;
            lblAuthor.Location = new Point(120, 55);
            lblAuthor.AutoSize = true;

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblAuthor);
            headerPanel.Controls.Add(pbLogo);

            this.Controls.Add(headerPanel);
        }

        private void ConfigureGrid()
        {
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Height = GRID_HEIGHT;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = clientList;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            dataGridView1.AllowUserToAddRows = ALLOW_ADD_DELETE_ROWS;
            dataGridView1.AllowUserToDeleteRows = ALLOW_ADD_DELETE_ROWS;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245); // Filas cebra
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Seleccionar fila completa
            dataGridView1.RowHeadersVisible = false;
            
            this.Controls.Add(dataGridView1);
        }

        private void ConfigureControls()
        {
            footerPanel = new Panel();
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 120; // Espacio suficiente para la barra y los botones
            // footerPanel.BackColor = Color.FromArgb(240, 240, 240); // Opcional: un gris clarito

            // 2. ProgressBar (Ahora su Location es relativa al PANEL, no al Form)
            progressBar1.Location = new Point(10, 10); 
            progressBar1.Size = new Size(this.ClientSize.Width - 40, 23);
            progressBar1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            footerPanel.Controls.Add(progressBar1);

            // 3. Icons (Carga normal)
            Size iconSize = new Size(24, 24);
            Image importImage = Image.FromFile("images/Icons/add.png");
            Image deleteImage = Image.FromFile("images/Icons/delete.png");

            // 4. Botón Importar
            btnImport.Text = " Importar Datos";
            btnImport.Location = new Point(10, 50); // Bajamos un poco respecto a la barra
            btnImport.Size = new Size(200, 40);
            btnImport.Image = _imageHelper.ResizeImage(importImage, iconSize);
            btnImport.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnImport.TextAlign = ContentAlignment.MiddleCenter;
            btnImport.ImageAlign = ContentAlignment.MiddleLeft;
            btnImport.Click += BtnImport_Click;
            footerPanel.Controls.Add(btnImport);

            // 5. Botón Eliminar
            btnDelete.Text = " Eliminar Fila";
            btnDelete.Location = new Point(220, 50);
            btnDelete.Size = new Size(150, 40);
            btnDelete.Image = _imageHelper.ResizeImage(deleteImage, iconSize);
            btnDelete.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnDelete.TextAlign = ContentAlignment.MiddleCenter;
            btnDelete.ImageAlign = ContentAlignment.MiddleLeft;
            btnDelete.Click += (s, e) => {
                if (dataGridView1.CurrentRow != null && !dataGridView1.CurrentRow.IsNewRow)
                {
                    if (dataGridView1.SelectedRows.Count > 0) {
                        if (MessageBox.Show("¿Seguro que deseas eliminar?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            clientList.RemoveAt(dataGridView1.CurrentRow.Index);
                        }
                    } else {
                        MessageBox.Show("Por favor, selecciona una fila completa para eliminar.");
                    }
                }
            };
            footerPanel.Controls.Add(btnDelete);

            // 6. AÑADIR EL PANEL AL FORMULARIO
            this.Controls.Add(footerPanel);
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
        
        
        private void BtnImport_Click(object? sender, EventArgs e)
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

        private void DataGridView1_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!dataGridView1.IsCurrentCellDirty)
            {
                return;
            }

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