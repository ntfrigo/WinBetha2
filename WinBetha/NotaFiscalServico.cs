using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinBetha
{
    public class NotaFiscalServico
    {
        public int NotaFiscalServicoId { get; set; }
        public string Numero { get; set; }
        public string Serie { get; set; }

        public int FilialId { get; set; } //Prestador

        public int? PedidoVendaId { get; set; }

        public int? TomadorId { get; set; }

        public int? NaturezaOperacaoServicoId { get; set; }

        public int FormaPagamentoId { get; set; }

        public string OutrasInformacoes { get; set; }
        public DateTime DataPrestacaoServico { get; set; }


        public DateTime? DataEnviadoPara { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public string MotivoCancelamento { get; set; }

        public DateTime? DataCancelamentoProvedor { get; set; }
        public string IdCancelamentoProvedor { get; set; }

        public virtual List<NotaFiscalServicoItem> NotaFiscalServicoItem { get; set; }

        public string XmlNfse { get; set; }

        public double ValorPis { get; set; }
        public double ValorCofins { get; set; }
        public double ValorInss { get; set; }
        public double ValorIr { get; set; }
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

        //Retorna da consulta do provedor(Betha/Publica)
        public double ValorCredito { get; set; }

        public string RetornoNumeroLote { get; set; }
        public DateTime RetornoDataRecebimento { get; set; }
        public string RetornoProtocolo { get; set; }

        //Retorna da consulta do provedor(Betha/Publica)
        public string CodigoVerificacao { get; set; }

        public string ErroCodigo { get; set; }
        public string ErroMensagem { get; set; }
        public string ErroCorrecao { get; set; }

        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public int UsuarioCadastro { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public int? UsuarioUltimaAlteracao { get; set; }

        public void Validate()
        {
        }
        
        

        public void CalcularValores()
        {
            ValorPis = NotaFiscalServicoItem.Sum(x => x.ValorPis);
            ValorCofins = NotaFiscalServicoItem.Sum(x => x.ValorCofins);
            ValorInss = NotaFiscalServicoItem.Sum(x => x.ValorInss);
            ValorIr = NotaFiscalServicoItem.Sum(x => x.ValorIr);
            ValorCsll = NotaFiscalServicoItem.Sum(x => x.ValorCsll);

            AliquotaIss = NotaFiscalServicoItem?.FirstOrDefault()?.AliquotaIss ?? 0;

            ValorServicos = NotaFiscalServicoItem.Sum(x => x.ValorServicos);
            OutrasRetencoes = NotaFiscalServicoItem.Sum(x => x.OutrasRetencoes);
            ValorIss = NotaFiscalServicoItem.Sum(x => x.ValorIss);
            ValorDeducoes = NotaFiscalServicoItem.Sum(x => x.ValorDeducoes);
            ValorIssRetido = NotaFiscalServicoItem.Sum(x => x.ValorIssRetido);
            DescontoIncondicionado = NotaFiscalServicoItem.Sum(x => x.DescontoIncondicionado);
            DescontoCondicionado = NotaFiscalServicoItem.Sum(x => x.DescontoCondicionado);
            ValorBaseCalculo = NotaFiscalServicoItem.Sum(x => x.ValorBaseCalculo);
            ValorLiquido = NotaFiscalServicoItem.Sum(x => x.ValorLiquido);
        }


        public void SetSequencialNfseSerie()
        {
            Numero = "4";
            Serie = "1";
        }

        /// <summary>
        /// Código de natureza da operação
        /// 1 – Tributação no município
        /// 2 – Tributação fora do município
        /// 3 – Isenção
        /// 4 – Imune
        /// 5 – Exigibilidade suspensa por decisão judicial
        /// 6 – Exigibilidade suspensa por procedimento administrativo
        /// 7 – Não Incidência
        /// 8 – Substituição Tributária
        /// </summary>
        /// <returns></returns>
        public int GetNaturezaOperacao()
        {
            return 1;
        }

            /*Chave	Descricao
101	ISS DEVIDO PARA O MUNICIPIO
111	ISS DEVIDO PARA OUTRO MUNICIPIO
121	ISS FIXO (Soc. Profissionais)
201	ISS RETIDO PELO TOMADOR OU INTERMEDIARIO
301	OPERACAO IMUNE, ISENTA OU NÃO TRIBUTADA*/


        public string GetRpsId()
        {
            return $"RPS4";
        }

        public string GetLoteRpsId()
        {
            return $"LOTE4";
        }
        
    }
}
