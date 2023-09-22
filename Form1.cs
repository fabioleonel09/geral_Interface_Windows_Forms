using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.VisualBasic;

namespace geralInterfaceWinForm
{
    public partial class Form1 : Form
    {
        #region VARIAVEIS E CONSTANTES
        string apiUrl = ConfigurationManager.AppSettings["ApiUrl"];
        string URI = "";
        int codigoProduto = 1;
        #endregion
        public Form1()
        {
            InitializeComponent();
        }

        private void btnObterProdutos_Click(object sender, EventArgs e)
        {
            GetAllProdutos();
        }

        private void btnProdutosPorId_Click(object sender, EventArgs e)
        {
            InputBox();
            if (codigoProduto != -1)
            {
                GetProdutoById(codigoProduto);
            }
        }

        private void btnIncluirProduto_Click(object sender, EventArgs e)
        {
            AddProduto();
        }

        private void btnAtualizaProduto_Click(object sender, EventArgs e)
        {
            InputBox();
            if (codigoProduto != -1)
            {
                UpdateProduto(codigoProduto);
            }
        }

        private void btnDeletarProduto_Click(object sender, EventArgs e)
        {
            InputBox();
            if (codigoProduto != -1)
            {
                DeleteProduto(codigoProduto);
            }
        }

        private async void GetAllProdutos()
        {
            URI = apiUrl;
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(URI))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                        dgvDados.DataSource = JsonConvert.DeserializeObject<Produto[]>(ProdutoJsonString).ToList();
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível obter o produto : " + response.StatusCode);
                    }
                }
            }
        }

        private async void GetProdutoById(int codProduto)
        {
            using (var client = new HttpClient())
            {
                BindingSource bsDados = new BindingSource();
                URI = apiUrl + "/" + codProduto.ToString();
                HttpResponseMessage response = await client.GetAsync(URI);
                if (response.IsSuccessStatusCode)
                {
                    var ProdutoJsonString = await response.Content.ReadAsStringAsync();
                    bsDados.DataSource = JsonConvert.DeserializeObject<Produto>(ProdutoJsonString);
                    dgvDados.DataSource = bsDados;
                }
                else
                {
                    MessageBox.Show("Falha ao obter o produto : " + response.StatusCode);
                }
            }
        }

        private async void AddProduto()
        {
            URI = apiUrl;
            Produto prod = new Produto();
            prod.Nome = "NoteBook Lenovo";
            prod.Categoria = "Notebooks";
            prod.Preco = 1200.00M;
            using (var client = new HttpClient())
            {
                var serializedProduto = JsonConvert.SerializeObject(prod);
                var content = new StringContent(serializedProduto, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(URI, content);
            }
            GetAllProdutos();
        }

        private async void UpdateProduto(int codProduto)
        {
            URI = apiUrl;
            Produto prod = new Produto();
            prod.Id = codProduto;
            prod.Nome = "NoteBook Apple";
            prod.Categoria = "Notebooks";
            prod.Preco = 9900.00M; // atualizando o preço do produto

            using (var client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(URI + "/" + prod.Id, prod);
                if (responseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Produto atualizado");
                }
                else
                {
                    MessageBox.Show("Falha ao atualizar o produto : " + responseMessage.StatusCode);
                }
            }
            GetAllProdutos();
        }

        private async void DeleteProduto(int codProduto)
        {
            URI = apiUrl;
            int ProdutoID = codProduto;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URI);
                HttpResponseMessage responseMessage = await
client.DeleteAsync(String.Format("{0}/{1}", URI, ProdutoID));
                if (responseMessage.IsSuccessStatusCode)
                {
                    MessageBox.Show("Produto excluído com sucesso");
                }
                else
                {
                    MessageBox.Show("Falha ao excluir o produto  : " + responseMessage.StatusCode);
                }
            }
            GetAllProdutos();
        }

        private void InputBox()
        {
            /* usando a função VB.Net para exibir um prompt para o usuário informar a senha */
            string Prompt = "Informe o código do Produto.";
            string Titulo = "Sobre os Produtos:";
            string Resultado = Microsoft.VisualBasic.Interaction.InputBox(Prompt, Titulo, "9", 600, 350);
            /* verifica se o resultado é uma string vazia o que indica que foi cancelado. */
            if (Resultado != "")
            {
                codigoProduto = Convert.ToInt32(Resultado);
            }
            else
            {
                codigoProduto = -1;
            }
        }
    }
}
