using System;
using System.Collections;
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

namespace PrimeiroProjetoTI48
{
    public partial class frmCliente : Form
    {
        public frmCliente()
        {
            InitializeComponent();
        }

        Connection con = new Connection();

        bool EmailValido(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void frmCliente_Load(object sender, EventArgs e)
        {
            txtNome.Focus();
            txtDateTimePiker.Value = DateTime.Now;
            AtualizarGrid();
        }

        private void frmAgendda_Load(object sender, EventArgs e)
        {
            txtNome.Focus();
            txtDateTimePiker.Value = DateTime.Now;
            AtualizarGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void LimparCampos()
        {
            txtID.Clear();
            txtNome.Clear();
            txtEmail.Clear();
            txtTelefone.Clear();
            txtDateTimePiker.Value = DateTime.Now;
            txtNome.Focus();
        }

        private void AtualizarGrid()
        {
            using (SqlConnection conn = con.Connect())
            {
                string sql = "SELECT Id, Nome, Telefone, Email FROM Cliente";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dg.DataSource = dt;
            }
        }

        private void dg_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtID.Text = dg.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                txtNome.Text = dg.Rows[e.RowIndex].Cells["Nome"].Value.ToString();
                txtTelefone.Text = dg.Rows[e.RowIndex].Cells["Telefone"].Value.ToString();
                txtEmail.Text = dg.Rows[e.RowIndex].Cells["Email"].Value.ToString();
            }
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório!");
                txtNome.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("O campo Email é obrigatório!");
                txtEmail.Focus();
                return;
            }

            if (!EmailValido(txtEmail.Text))
            {
                MessageBox.Show("Email inválido!");
                txtEmail.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = con.Connect())
                {
                    string sql = @"INSERT INTO Cliente 
                           (Nome, Telefone, Email)
                           VALUES (@Nome, @Telefone, @Email)";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cliente cadastrado com sucesso!");
                AtualizarGrid();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "")
            {
                MessageBox.Show("Selecione um cliente para alterar!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório!");
                txtNome.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("O campo Email é obrigatório!");
                txtEmail.Focus();
                return;
            }

            if (!EmailValido(txtEmail.Text))
            {
                MessageBox.Show("Email inválido!");
                txtEmail.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = con.Connect())
                {
                    string sql = @"UPDATE Cliente 
                           SET Nome=@Nome, Telefone=@Telefone, Email=@Email
                           WHERE Id=@Id";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@Id", int.Parse(txtID.Text));
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@Telefone", txtTelefone.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cliente alterado com sucesso!");
                AtualizarGrid();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (txtID.Text == "")
            {
                MessageBox.Show("Selecione um cliente para excluir!");
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "Deseja realmente excluir este cliente?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = con.Connect())
                    {
                        string sql = "DELETE FROM Cliente WHERE Id=@Id";

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@Id", int.Parse(txtID.Text));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cliente excluído com sucesso!");
                    AtualizarGrid();
                    LimparCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir: " + ex.Message);
                }
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            AtualizarGrid();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                AtualizarGrid();
                return;
            }

            try
            {
                using (SqlConnection conn = con.Connect())
                {
                    string sql = "SELECT Id, Nome, Telefone, Email FROM Cliente WHERE Nome LIKE @Nome";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Nome", "%" + txtNome.Text + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dg.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Nenhum cliente encontrado!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na consulta: " + ex.Message);
            }
        }

        private void txtID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTelefone_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
    }
}