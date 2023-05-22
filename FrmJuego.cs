using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormPersonas
{
    public partial class FrmJuego : Form
    {
        bool juega = false;

        // conexion a SQL
        static string connectionString = @"Data Source=LAPTOP-HR0LNU7A\SQLEXPRESS;Initial Catalog=WinFormJuegoDB;Integrated Security=True";
        SqlConnection connection = new SqlConnection(connectionString);

        SqlCommand command;
        SqlDataReader reader;


        public FrmJuego()
        {
            InitializeComponent();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // insertar muniecos en la base de datos
        private void CargarMunieco(Munieco munieco)
        {
            try
            {
                connection.Open();

                string SqlInsert = "INSERT INTO Muniecos(nombre, energia) VALUES(@nombre, @energia)";
                command = new SqlCommand(SqlInsert, connection);

                command.Parameters.AddWithValue("@nombre", munieco.Nombre);
                command.Parameters.AddWithValue("@energia", munieco.Energia);

                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                // Manejo de excepciones o errores en la conexión o inserción
                MessageBox.Show("Error al guardar el muñeco en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // traer los muniecos desde la base de datos
        private void TraerMuniecos()
        {
            try
            {
                connection.Open();

                string selectQuery = "SELECT nombre, energia from Muniecos";
                command = new SqlCommand(selectQuery, connection);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string nombre = reader.GetString(0);
                    int energia = reader.GetInt32(1);

                    Munieco munieco = new Munieco(nombre, energia);
                    lstMuñecos.Items.Add(munieco);
                }
                reader.Close();
                connection.Close();

            }
            catch (Exception ex)
            {
                // Manejo de excepciones o errores en la conexión o consulta
                MessageBox.Show("Error al cargar los muñecos desde la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // eliminar muniecos de la base de datos
        private void EliminarMunieco(Munieco munieco)
        {
            try
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Muniecos WHERE nombre = @nombre";
                command = new SqlCommand(deleteQuery, connection);

                command.Parameters.AddWithValue("@nombre", munieco.Nombre);
                command.ExecuteNonQuery();

                connection.Close();

            }
            catch (Exception ex)
            {
                // Manejo de excepciones o errores en la conexión o consulta
                MessageBox.Show("Error al eliminar el muñeco de la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // actualizar muniecos en la base de datos
        private void ActualizarMunieco(Munieco munieco)
        {
            try
            {
                connection.Open();

                string updateQuery = "UPDATE Muniecos set energia=@energia WHERE nombre = @nombre";
                command = new SqlCommand(updateQuery, connection);

                command.Parameters.AddWithValue("@nombre", munieco.Nombre);
                command.Parameters.AddWithValue("@energia", munieco.Energia);
                command.ExecuteNonQuery();

                connection.Close();

            }
            catch (Exception ex)
            {
                // Manejo de excepciones o errores en la conexión o consulta
                MessageBox.Show("Error al eliminar el muñeco de la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnCrear_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text;

            if (String.IsNullOrEmpty(nombre) == false)
            {
                Munieco oMunieco = new Munieco(nombre);

                // cargar el muñeco en la bd
                CargarMunieco(oMunieco);

                lstMuñecos.Items.Add(oMunieco);
                txtNombre.Text = String.Empty; //Esto deja la caja en blanco nuevamente para una próx entrada
                txtNombre.Focus(); //Esto deja el curso sobre el componente

                
            }
            else
            {
                MessageBox.Show("Debe ingresar un nombre para el muñeco!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmJuego_Load(object sender, EventArgs e)
        {
            // lstMuñecos.Items.Clear(); esto es un obviedad porque cuando se carga a memoria ya esta vacío
            TraerMuniecos();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            if(lstMuñecos.SelectedItems.Count > 0)
            {
                Munieco munieco = (Munieco)lstMuñecos.SelectedItem;
                DialogResult result = MessageBox.Show("¿Seguro quiere borrar el muñeco?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if(result == DialogResult.Yes)
                {
                    
                    lstMuñecos.Items.Remove(munieco);
                    EliminarMunieco(munieco);
                }

            } else
            {
                MessageBox.Show("Debe Seleccionar un muñeco para borrar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void txtBoxSegs_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void btnJugar_Click(object sender, EventArgs e)
        {
            // falta la validacion de que solo se puedan ingresar numeros
            if (lstMuñecos.SelectedItems.Count > 0)
            {
                Munieco muniecoJuega = (Munieco)lstMuñecos.SelectedItem;
                int ind = lstMuñecos.SelectedIndex;
                if (juega == false)
                {
                    lstMuñecos.Visible = false;
                    btnBorrar.Visible = false;
                    btnComer.Visible = false;
                    btnCrear.Visible = false;
                    txtNombre.Visible = false;
                    label1.Visible = false;
                    label2.Text = "Vas a jugar con " + muniecoJuega.Nombre +
                        ". Ingresa los segundos a jugar: ";
                    label2.Visible = true;
                    txtBoxSegs.Visible = true;
                    btnVolverJugar.Visible = true;
                    juega = true;
                }
                else
                {
                    try
                    {
                        Convert.ToInt16(txtBoxSegs.Text);
                        int segs = Convert.ToInt16(txtBoxSegs.Text);
                        if(muniecoJuega.Energia < segs)
                        {
                            MessageBox.Show("El muñeco no tiene la energia suficiente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {

                            muniecoJuega.Jugar(segs);
                            // tengo problemas para que la se muestre disminuida 
                            // en el lstMuñecos
                            // estaria bien eliminar uno y agregar este nuevo?
                            lstMuñecos.Items.Remove(lstMuñecos.SelectedItem); // elimino el existente
                            lstMuñecos.Items.Insert(ind, muniecoJuega); // esto crea un objeto nuevo y da bien la energia
                            //lstMuñecos.Refresh();

                            // actualizamos la energia en la bd
                            ActualizarMunieco(muniecoJuega);

                            lstMuñecos.Visible = true;
                            btnBorrar.Visible = true;
                            btnComer.Visible = true;
                            btnCrear.Visible = true;
                            txtNombre.Visible = true;
                            label1.Visible = true;
                            label2.Text = "";
                            label2.Visible = false;
                            txtBoxSegs.Visible = false;
                            juega = false;
                            txtBoxSegs.Text = "";
                            btnVolverJugar.Visible = false;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Introducir solo numeros", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Debe Seleccionar un muñeco para jugar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnComer_Click(object sender, EventArgs e)
        {
            if (lstMuñecos.SelectedItems.Count > 0)
            {
                int ind = lstMuñecos.SelectedIndex;
                Munieco muniecoCome = (Munieco)lstMuñecos.SelectedItem;
                if(muniecoCome.Energia > 98)
                {
                    MessageBox.Show("El muñeco esta lleno", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    muniecoCome.Energia = 100;
                    lstMuñecos.Items.Remove(lstMuñecos.SelectedItem); // elimino el existente
                    lstMuñecos.Items.Insert(ind, muniecoCome); // esto crea un objeto nuevo y da bien la energia

                    // actualizamos la energia en la bd
                    ActualizarMunieco(muniecoCome);

                }
                else
                {
                    muniecoCome.Comer();
                    lstMuñecos.Items.Remove(lstMuñecos.SelectedItem); // elimino el existente
                    lstMuñecos.Items.Insert(ind, muniecoCome); // esto crea un objeto nuevo y da bien la energia

                    // actualizamos la energia en la bd
                    ActualizarMunieco(muniecoCome);
                }
            } else
            {
                MessageBox.Show("Debe Seleccionar un muñeco para comer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // aqui ocurre lo mismo que con el de Jugar()
        }

        private void btnVolverJugar_Click(object sender, EventArgs e)
        {
            //lstMuñecos.ResetText();
            lstMuñecos.Visible = true;
            btnBorrar.Visible = true;
            btnComer.Visible = true;
            btnCrear.Visible = true;
            txtNombre.Visible = true;
            label1.Visible = true;
            label2.Text = "";
            label2.Visible = false;
            txtBoxSegs.Visible = false;
            juega = false;
            txtBoxSegs.Text = "";
            btnVolverJugar.Visible = false;
        }
    }
}
