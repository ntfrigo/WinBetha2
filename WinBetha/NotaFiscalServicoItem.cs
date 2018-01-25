using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinBetha
{
    public class NotaFiscalServicoItem
    {
        public int NotaFiscalServicoItemId { get; set; }

        public int NotaFiscalServicoId { get; set; }
        public virtual NotaFiscalServico NotaFiscalServico { get; set; }

        public int? ProdutoId { get; set; }

        public int? TipoServicoIssId { get; set; }

        public string DescricaoProduto { get; set; }

        public double AliquotaPis { get; set; }
        public double ValorPis { get; set; }
        public double AliquotaCofins { get; set; }
        public double ValorCofins { get; set; }
        public double AliquotaInss { get; set; }
        public double ValorInss { get; set; }
        public double AliquotaIr { get; set; }
        public double ValorIr { get; set; }
        public double AliquotaCsll { get; set; }
        public double ValorCsll { get; set; }

        public double ValorServicos { get; set; }
        public double OutrasRetencoes { get; set; }
        public double AliquotaIss { get; set; }
        public double ValorIss { get; set; }
        public double ValorDeducoes { get; set; }
        public double ValorIssRetido { get; set; }
        public double DescontoIncondicionado { get; set; }
        public double DescontoCondicionado { get; set; }
        public double ValorBaseCalculo { get; set; }
        public double ValorLiquido { get; set; }

        public bool RecolheIss { get; set; }
        public bool Retem465 { get; set; }
        public bool MesmoMunicipio { get; set; }

        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public int UsuarioCadastro { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public int? UsuarioUltimaAlteracao { get; set; }

        public void Validate()
        {
        }
        
        #region Calculos
        
        private void CalculaPis()
        {
            ValorPis = (AliquotaPis > 0) ? ValorBaseCalculo * (AliquotaPis / 100) : 0;
        }

        private void CalculaCofins()
        {
            ValorCofins = (AliquotaCofins > 0) ? ValorBaseCalculo * (AliquotaCofins / 100) : 0;
        }

        private void CalculaInss()
        {
            ValorInss = (AliquotaInss > 0) ? ValorBaseCalculo * (AliquotaInss / 100) : 0;
        }

        private void CalculaIr()
        {
            ValorIr = (AliquotaIr > 0) ? ValorBaseCalculo * (AliquotaIr / 100) : 0;
            if (ValorIr < 10) ValorIr = 0;
        }

        private void CalculaCsll()
        {
            ValorCsll = (AliquotaCsll > 0) ? ValorBaseCalculo * (AliquotaCsll / 100) : 0;
        }

        private void CalculaRetencoes()
        {
            var maior10 = (ValorPis + ValorCofins + ValorInss + ValorCsll) > 10;

            if (Retem465 || maior10)
            {
                ValorLiquido -= ValorPis;
                ValorLiquido -= ValorCofins;
                ValorLiquido -= ValorInss;
                ValorLiquido -= ValorCsll;
            }

            if (ValorIr > 10) ValorLiquido -= ValorIr;
            if (RecolheIss && ValorIss > 0) ValorLiquido -= ValorIss;

            ValorLiquido -= ValorIssRetido;
        }
        
        #endregion


        public string GetDiscriminacaoBetha(int? pedidoVendaId)
        {
            return $"[[Descricao={DescricaoProduto}  PRODID:{ProdutoId} {DateTime.Now:dd/MM/yyyy} PVID:{pedidoVendaId}][Quantidade=1][ValorUnitario={(decimal)ValorServicos:N2}]]";
        }
    }
}
