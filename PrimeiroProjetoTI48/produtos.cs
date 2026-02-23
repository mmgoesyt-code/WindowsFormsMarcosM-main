using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimeiroProjetoTI48
{
    public partial class produtos : Form
    {
        public produtos()
        {
            InitializeComponent();
        }

        Connection con = new Connection();

        private void produtos_Load(object sender, EventArgs e)
        {
            txtNome.Focus();
            AtualizarGrid();
        }

        private void LimparCampos()
        {
            txtId.Clear();
            txtNome.Clear();
            txtCusto.Clear();
            txtNome.Focus();
        }

        private void AtualizarGrid()
        {
            using (SqlConnection conn = con.Connect())
            {
                string sql = "SELECT Id, Nome, Custo FROM Produto";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                GridProduto.DataSource = dt;
            }
        }

        private void GridProduto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtId.Text = GridProduto.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                txtNome.Text = GridProduto.Rows[e.RowIndex].Cells["Nome"].Value.ToString();
                txtCusto.Text = GridProduto.Rows[e.RowIndex].Cells["Custo"].Value.ToString();
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

            if (string.IsNullOrWhiteSpace(txtCusto.Text))
            {
                MessageBox.Show("O campo Custo é obrigatório!");
                txtCusto.Focus();
                return;
            }

            decimal custo;
            if (!decimal.TryParse(txtCusto.Text, out custo))
            {
                MessageBox.Show("Custo inválido! Digite um valor numérico.");
                txtCusto.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = con.Connect())
                {
                    string sql = @"INSERT INTO Produto (Nome, Custo)
                                   VALUES (@Nome, @Custo)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@Custo", custo);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Produto cadastrado com sucesso!");
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
            if (txtId.Text == "")
            {
                MessageBox.Show("Selecione um produto para alterar!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("O campo Nome é obrigatório!");
                txtNome.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCusto.Text))
            {
                MessageBox.Show("O campo Custo é obrigatório!");
                txtCusto.Focus();
                return;
            }

            decimal custo;
            if (!decimal.TryParse(txtCusto.Text, out custo))
            {
                MessageBox.Show("Custo inválido! Digite um valor numérico.");
                txtCusto.Focus();
                return;
            }

            try
            {
                using (SqlConnection conn = con.Connect())
                {
                    string sql = @"UPDATE Produto 
                                   SET Nome=@Nome, Custo=@Custo
                                   WHERE Id=@Id";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Id", int.Parse(txtId.Text));
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@Custo", custo);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Produto alterado com sucesso!");
                AtualizarGrid();
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void btnAlterar_Click_1(object sender, EventArgs e)
        {
            btnAlterar_Click(sender, e);
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Selecione um produto para excluir!");
                return;
            }

            DialogResult resultado = MessageBox.Show(
                "Deseja realmente excluir este produto?",
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
                        string sql = "DELETE FROM Produto WHERE Id=@Id";

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@Id", int.Parse(txtId.Text));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Produto excluído com sucesso!");
                    AtualizarGrid();
                    LimparCampos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir: " + ex.Message);
                }
            }
        }

        private void btnExcluir_Click_1(object sender, EventArgs e)
        {
            btnExcluir_Click(sender, e);
        }

        private void btnMostrarDados_Click(object sender, EventArgs e)
        {
            AtualizarGrid();
        }

        private void btnMostrarDados_Click_1(object sender, EventArgs e)
        {
            btnMostrarDados_Click(sender, e);
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
                    string sql = "SELECT Id, Nome, Custo FROM Produto WHERE Nome LIKE @Nome";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Nome", "%" + txtNome.Text + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridProduto.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Nenhum produto encontrado!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na consulta: " + ex.Message);
            }
        }

        private void btnConsultar_Click_1(object sender, EventArgs e)
        {
            btnConsultar_Click(sender, e);
        }

        private void txtNome_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtId_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtCusto_TextChanged(object sender, EventArgs e)
        {
        }

        private void GridProduto_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}