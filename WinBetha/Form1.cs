using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using WinBetha.BethaV1RecepcionarLoteRps;

namespace WinBetha
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EnviaNotaFiscalServico(new NotaFiscalServico());
        }

        public void EnviaNotaFiscalServico(NotaFiscalServico nfse)
        {
            try
            {
                nfse.ValorServicos = 200;
                nfse.ValorDeducoes = 123;
                nfse.ValorPis = 33;
                nfse.ValorCofins = 44;
                nfse.ValorInss = 55;
                nfse.ValorCsll = 10;
                nfse.OutrasRetencoes = 2;


                var cert = new X509Certificate2();
                
                nfse.SetSequencialNfseSerie();

                #region loteRps

                var loteRps = new BethaV1RecepcionarLoteRps.EnviarLoteRpsEnvio();
                loteRps.LoteRps = new BethaV1RecepcionarLoteRps.tcLoteRps();
                loteRps.LoteRps.Id = nfse.GetLoteRpsId();
                loteRps.LoteRps.Cnpj = "947593475389475";
                loteRps.LoteRps.InscricaoMunicipal = null;
                loteRps.LoteRps.NumeroLote = 3940539485;
                loteRps.LoteRps.QuantidadeRps = 1;
                loteRps.LoteRps.ListaRps = new BethaV1RecepcionarLoteRps.tcRps[1];

                #region loteRps Item
                var itemRps = new BethaV1RecepcionarLoteRps.tcRps();
                itemRps.InfRps = GerarDeclaracaoPrestacaoServico(nfse);
                


                itemRps.Signature = Assinador.AssinarNfseBethaEnviar(itemRps, $"{itemRps.InfRps.Id}", cert);


                loteRps.LoteRps.ListaRps[0] = itemRps;
                #endregion

                #region loteRps Signature
                //loteRps.Signature = Assinador.AssinarNfseBethaEnviar(loteRps.LoteRps, $"{loteRps.LoteRps.Id}", cert);
                #endregion

                #endregion

                var soapClient = new BethaV1RecepcionarLoteRps.RecepcionarLoteRpsClient(BethaServiceBinding(), new EndpointAddress("https://e-gov.betha.com.br/e-nota-contribuinte-test-ws/recepcionarLoteRps"));
                var response = soapClient.EnviarLoteRpsEnvio(loteRps);

                #region GerarNfseResposta
                var resposta = response.EnviarLoteRpsResposta;
                nfse.RetornoNumeroLote = resposta.NumeroLote;
                nfse.RetornoDataRecebimento = resposta.DataRecebimento;
                nfse.RetornoProtocolo = resposta.Protocolo;

                var lista = resposta.ListaMensagemRetorno.ToList();
                foreach (var retornoNfse in lista)
                {
                    nfse.ErroCodigo = retornoNfse.Codigo;
                    nfse.ErroMensagem = retornoNfse.Mensagem;
                    nfse.ErroCorrecao = retornoNfse.Correcao;
                }

                if (!lista.Any())
                {
                    nfse.ErroCodigo = null;
                    nfse.ErroMensagem = null;
                    nfse.ErroCorrecao = null;
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private BethaV1RecepcionarLoteRps.tcInfRps GerarDeclaracaoPrestacaoServico(NotaFiscalServico nfse)
        {
            #region InfDeclaracaoPrestacaoServico

            var rps = new BethaV1RecepcionarLoteRps.tcInfRps();

            rps.Id = nfse.GetRpsId();
            rps.RegimeEspecialTributacao = 1;
            rps.OptanteSimplesNacional = 2;
            rps.DataEmissao = nfse.DataPrestacaoServico;
            rps.Status = 1;
            rps.NaturezaOperacao = nfse.GetNaturezaOperacao();
            rps.OutrasInformacoes = nfse.OutrasInformacoes;

            #region IdentificacaoRps

            rps.IdentificacaoRps = new BethaV1RecepcionarLoteRps.tcIdentificacaoRps();
            rps.IdentificacaoRps.Numero = nfse.Numero;
            rps.IdentificacaoRps.Serie = nfse.Serie;
            rps.IdentificacaoRps.Tipo = 1;

            #endregion
            
            #region Servico
            rps.Servico = new BethaV1RecepcionarLoteRps.tcDadosServico();
            rps.Servico.ItemListaServico = "1701";
            rps.Servico.CodigoTributacaoMunicipio = null;
            rps.Servico.Discriminacao = "Teste";
            rps.Servico.CodigoMunicipio = 3550308;
            //rps.Servico.CodigoCnae = cliente.GetPaisNfse(); //Código CNAE (7)

            rps.Servico.Valores = new BethaV1RecepcionarLoteRps.tcValores();
            rps.Servico.Valores.IssRetido = ((nfse.ValorIssRetido > 0) ? 1 : 2);
            rps.Servico.Valores.ValorServicos = (decimal)nfse.ValorServicos;
            rps.Servico.Valores.ValorDeducoes = (decimal)nfse.ValorDeducoes;
            rps.Servico.Valores.ValorPis = (decimal)nfse.ValorPis;
            rps.Servico.Valores.ValorCofins = (decimal)nfse.ValorCofins;
            rps.Servico.Valores.ValorInss = (decimal)nfse.ValorInss;
            rps.Servico.Valores.ValorIr = (decimal)nfse.ValorIr;
            rps.Servico.Valores.ValorCsll = (decimal)nfse.ValorCsll;
            rps.Servico.Valores.OutrasRetencoes = (decimal)nfse.OutrasRetencoes;
            rps.Servico.Valores.DescontoIncondicionado = (decimal)nfse.DescontoIncondicionado;
            rps.Servico.Valores.DescontoCondicionado = (decimal)nfse.DescontoCondicionado;
            rps.Servico.Valores.Aliquota = (decimal)nfse.AliquotaIss;
            rps.Servico.Valores.BaseCalculo = (decimal)nfse.ValorBaseCalculo;
            rps.Servico.Valores.ValorLiquidoNfse = (decimal)nfse.ValorLiquido;
            rps.Servico.Valores.ValorIssRetido = (decimal)nfse.ValorIssRetido;
            #endregion

            #region Prestador
            rps.Prestador = new BethaV1RecepcionarLoteRps.tcIdentificacaoPrestador();
            rps.Prestador.Cnpj = "4534534533453434";
            rps.Prestador.InscricaoMunicipal = null;
            #endregion

            #region Tomador

            rps.Tomador = new BethaV1RecepcionarLoteRps.tcDadosTomador();
            rps.Tomador.RazaoSocial = "ALIMENTOS";

            #region rps.Tomador.IdentificacaoTomador
            rps.Tomador.IdentificacaoTomador = new BethaV1RecepcionarLoteRps.tcIdentificacaoTomador();
            rps.Tomador.IdentificacaoTomador.CpfCnpj = new BethaV1RecepcionarLoteRps.tcCpfCnpj();
            rps.Tomador.IdentificacaoTomador.CpfCnpj.Cnpj = "4534534533453434";
            #endregion

            //rps.Tomador.Contato = new BethaV1RecepcionarLoteRps.tcContato() { Email = cliente.Email };

            rps.Tomador.Endereco = new BethaV1RecepcionarLoteRps.tcEndereco();
            rps.Tomador.Endereco.Endereco = "RUA AMARAL";
            rps.Tomador.Endereco.Bairro = "JARDIM";
            rps.Tomador.Endereco.Cep = "454564434";
            rps.Tomador.Endereco.CodigoMunicipio = 3550308;
            //rps.Tomador.Endereco.Numero = cliente.EnderecoNumero;
            rps.Tomador.Endereco.Uf = "SP";
            #endregion

            //rps.CondicaoPagamento;

            #endregion

            return rps;
        }

        BasicHttpBinding BethaServiceBinding()
        {
            return new BasicHttpBinding(BasicHttpSecurityMode.None)
            {
                Security = { Mode = BasicHttpSecurityMode.Transport },
                MaxReceivedMessageSize = int.MaxValue
                //Adicionar credenciais
            };
        }
        
    }
}
