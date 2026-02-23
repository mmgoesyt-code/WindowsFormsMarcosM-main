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
    public partial class ItensVendas : Form
    {
        public ItensVendas()
        {
            InitializeComponent();
        }

        Connection con = new Connection();
        DataTable dtItens = new DataTable();
        int vendaId = 0;

        private void ItensVendas_Load(object sender, EventArgs e)
        {
            CarregarClientes();
            CarregarProdutos();
            InicializarGridItens();
            LimparCampos();
        }

        private void InicializarGridItens()
        {
            dtItens.Columns.Clear();
            dtItens.Columns.Add("ProdutoId", typeof(int));
            dtItens.Columns.Add("Produto", typeof(string));
            dtItens.Columns.Add("Quantidade", typeof(int));
            dtItens.Columns.Add("Custo", typeof(decimal));
            dtItens.Columns.Add("Total", typeof(decimal));

            dgItensVenda.DataSource = dtItens;
        }

        private void CarregarClientes()
        {
            using (SqlConnection conn = con.Connect())
            {
                string sql = "SELECT Id, Nome FROM Cliente ORDER BY Nome";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbCliente.DataSource = dt;
                cmbCliente.DisplayMember = "Nome";
                cmbCliente.ValueMember = "Id";
                cmbCliente.SelectedIndex = -1;
            }
        }

        private void CarregarProdutos()
        {
            using (SqlConnection conn = con.Connect())
            {
                string sql = "SELECT Id, Nome, Custo FROM Produto ORDER BY Nome";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbProduto.DataSource = dt;
                cmbProduto.DisplayMember = "Nome";
                cmbProduto.ValueMember = "Id";
                cmbProduto.SelectedIndex = -1;
            }
        }

        private void LimparCampos()
        {
            txtVendaId.Clear();
            cmbCliente.SelectedIndex = -1;
            cmbProduto.SelectedIndex = -1;
            txtQuantidade.Clear();
            txtTotal.Clear();
            dtpDataVenda.Value = DateTime.Now;
            dtItens.Clear();
            AtualizarTotalVenda();
            vendaId = 0;
        }

        private void CalcularTotal()
        {
            if (cmbProduto.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(txtQuantidade.Text))
            {
                int quantidade;
                if (int.TryParse(txtQuantidade.Text, out quantidade))
                {
                    DataRowView row = (DataRowView)cmbProduto.SelectedItem;
                    decimal custo = Convert.ToDecimal(row["Custo"]);
                    decimal total = custo * quantidade;
                    txtTotal.Text = total.ToString("F2");
                }
            }
        }

        private void AtualizarTotalVenda()
        {
            decimal totalVenda = 0;
            foreach (DataRow row in dtItens.Rows)
            {
                totalVenda += Convert.ToDecimal(row["Total"]);
            }
            lblTotalVenda.Text = "Total da Venda: R$ " + totalVenda.ToString("F2");
        }

        private void btnNovaVenda_Click(object sender, EventArgs e)
        {
            LimparCampos();
            cmbCliente.Focus();
            MessageBox.Show("Nova venda iniciada!");
        }

        private void btnAdicionarItem_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione um cliente!");
                cmbCliente.Focus();
                return;
            }

            if (cmbProduto.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione um produto!");
                cmbProduto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQuantidade.Text))
            {
                MessageBox.Show("Informe a quantidade!");
                txtQuantidade.Focus();
                return;
            }

            int quantidade;
            if (!int.TryParse(txtQuantidade.Text, out quantidade) || quantidade <= 0)
            {
                MessageBox.Show("Quantidade inválida!");
                txtQuantidade.Focus();
                return;
            }

            DataRowView produtoSelecionado = (DataRowView)cmbProduto.SelectedItem;
            int produtoId = Convert.ToInt32(produtoSelecionado["Id"]);
            string nomeProduto = produtoSelecionado["Nome"].ToString();
            decimal custo = Convert.ToDecimal(produtoSelecionado["Custo"]);
            decimal total = custo * quantidade;

            DataRow novoItem = dtItens.NewRow();
            novoItem["ProdutoId"] = produtoId;
            novoItem["Produto"] = nomeProduto;
            novoItem["Quantidade"] = quantidade;
            novoItem["Custo"] = custo;
            novoItem["Total"] = total;

            dtItens.Rows.Add(novoItem);

            cmbProduto.SelectedIndex = -1;
            txtQuantidade.Clear();
            txtTotal.Clear();
            AtualizarTotalVenda();

            MessageBox.Show("Item adicionado!");
        }

        private void btnRemoverItem_Click(object sender, EventArgs e)
        {
            if (dgItensVenda.SelectedRows.Count > 0)
            {
                DialogResult resultado = MessageBox.Show(
                    "Deseja remover este item?",
                    "Confirmação",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (resultado == DialogResult.Yes)
                {
                    dgItensVenda.Rows.RemoveAt(dgItensVenda.SelectedRows[0].Index);
                    AtualizarTotalVenda();
                    MessageBox.Show("Item removido!");
                }
            }
            else
            {
                MessageBox.Show("Selecione um item para remover!");
            }
        }

        private void btnFinalizarVenda_Click(object sender, EventArgs e)
        {
            if (cmbCliente.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione um cliente!");
                return;
            }

            if (dtItens.Rows.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um item à venda!");
                return;
            }

            try
            {
                using (SqlConnection conn = con.Connect())
                {
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Inserir Venda
                        string sqlVenda = @"INSERT INTO Venda (ClienteId, DataVenda) 
                                           VALUES (@ClienteId, @DataVenda);
                                           SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdVenda = new SqlCommand(sqlVenda, conn, transaction);
                        cmdVenda.Parameters.AddWithValue("@ClienteId", cmbCliente.SelectedValue);
                        cmdVenda.Parameters.AddWithValue("@DataVenda", dtpDataVenda.Value);

                        vendaId = Convert.ToInt32(cmdVenda.ExecuteScalar());

                        // Inserir Itens da Venda
                        string sqlItem = @"INSERT INTO ItemVenda (VendaId, ProdutoId, Quantidade, Desconto, Total)
                                          VALUES (@VendaId, @ProdutoId, @Quantidade, @Desconto, @Total)";

                        foreach (DataRow row in dtItens.Rows)
                        {
                            SqlCommand cmdItem = new SqlCommand(sqlItem, conn, transaction);
                            cmdItem.Parameters.AddWithValue("@VendaId", vendaId);
                            cmdItem.Parameters.AddWithValue("@ProdutoId", row["ProdutoId"]);
                            cmdItem.Parameters.AddWithValue("@Quantidade", row["Quantidade"]);
                            cmdItem.Parameters.AddWithValue("@Desconto", 0);
                            cmdItem.Parameters.AddWithValue("@Total", row["Total"]);

                            cmdItem.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show($"Venda #{vendaId} finalizada com sucesso!");
                        LimparCampos();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao finalizar venda: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "Deseja cancelar esta venda?",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                LimparCampos();
                MessageBox.Show("Venda cancelada!");
            }
        }

        private void cmbProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularTotal();
        }

        private void txtQuantidade_TextChanged(object sender, EventArgs e)
        {
            CalcularTotal();
        }

        private void txtVendaId_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}