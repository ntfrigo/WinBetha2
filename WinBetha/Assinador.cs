using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace WinBetha
{
    public static class Assinador
    {
        public static BethaV1RecepcionarLoteRps.Signature AssinarNfseBethaEnviar<T>(T objeto, string id, X509Certificate2 certificadoDigital) where T : class
        {
            try
            {
                var documento = new XmlDocument { PreserveWhitespace = true };
                documento.LoadXml(objeto.ToXml()); //<------------------ Problema ocorre

                var signedXml = new SignedXml(documento);
                signedXml.SigningKey = certificadoDigital.PrivateKey;

                Reference reference = new Reference();
                reference.Uri = $"#{id}";
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigC14NTransform());
                signedXml.AddReference(reference);

                // Configurar o tratamento das informações
                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(certificadoDigital));
                signedXml.KeyInfo = keyInfo;

                // Computando Assinatura
                signedXml.ComputeSignature();

                //// recuperando a representação do XML assinado
                var xmlDigitalSignature = signedXml.GetXml();

                //documento.DocumentElement.AppendChild(documento.ImportNode(xmlDigitalSignature, true));

                //return documento.InnerXml;

                //var assinatura = XmlNfse.XmlStringParaClasse<BethaV1RecepcionarLoteRps.Signature>(xmlDigitalSignature.OuterXml);
                return null;
            }
            catch (Exception ex)
            {
                //ex.Message
                return null;
            }
            finally
            {
                //certificadoDigital?.Reset();
            }
        }
    }
}
