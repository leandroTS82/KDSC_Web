using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Shared;
using System.Configuration;


namespace Business
{
    public class ServicoEmail
    {
        static public List<ColecaoDocumentosModel> EnviaEmail(List<ColecaoDocumentosModel> colecaoDocumentos, List<DocumentoModel> planilhaCtrl, string dataDiretorio, out string retornoErro)
        {
            List<ColecaoDocumentosModel> novaColecaoDocumentos = new List<ColecaoDocumentosModel>();
            retornoErro = "";
            List<FileInfo> arquivosNaoEncontrados = colecaoDocumentos.Find(x => x.tipoArquivo == "arquivosNaoEncontrados").fileDocumento;
            List<DocumentoModel> arquivosEncontrados = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEncontrados");
            List<DocumentoModel> arquivosErro = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosErro");
            List<DocumentoModel> arquivosEmailErro = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEmailErro");
            List<DocumentoModel> arquivosErroDuplicado = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosErroDuplicado");
            List<DocumentoModel> arquivosEnviadoComSucesso = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEnviadoComSucesso");
            // --
            // Faz um filtro pra pegar jobs únicos
            // para envio de email
            List<DocumentoModel> jobs = arquivosEncontrados.Select(x => x.job).Distinct().Select(code => arquivosEncontrados.First(x => x.job == code)).ToList();
            foreach (DocumentoModel job in jobs)
            {
                try
                {
                    //verifica se há algum erro com o job.
                    if (job.erro) continue;
                    // --
                    // Procura todos os emails dos 
                    // profissionais que receberão o email
                    List<DocumentoModel> emails = planilhaCtrl.FindAll(x => x.job == job.job);
                    // --
                    // Procura todos os documentos relacionados
                    // ao documento
                    List<DocumentoModel> docs = arquivosEncontrados.FindAll(delegate(DocumentoModel x){ return x.job == job.job && !x.emailEnviado && !x.erro;});

                    //string docsEmail = MaganeFiles.MoveEAlteraArquivos(docs);
                    string docsEmail = "";
                    foreach (DocumentoModel doc in docs)
                    {
                        // Concatena os nomes dos documentos que serão enviados por email
                        docsEmail += docsEmail.Length > 0 ? "<BR>" + doc.arquivo.Name.Replace("OK_", "") : doc.arquivo.Name.Replace("OK_", "");
                    }
                    // --
                    // Não manda o email se não tiver documentos para enviar
                    if (docsEmail.Length == 0)
                    {
                        job.erro = true; 
                        var arr = job.arquivo.ToString().Split('\\');
                        string arquivo = arr[arr.Length - 1];
                        job.mensagemDeErro = string.Format("E-mail nâo enviado - A notificação de circularização referentes ao job {0} está marcado como enviado: \" {1}\"",job.job,arquivo);
                        arquivosErro.Add(job);                        
                        continue;
                    }
                    // --
                    // Instancia o email e configura o remetente
                    MailMessage mail = new MailMessage();
                    string endemail = ConfigurationSettings.AppSettings["MailFrom"].ToString();
                    string modoTeste = ConfigurationSettings.AppSettings["ModoTeste"].ToString();
                    string bodyMail = ConfigurationSettings.AppSettings["BodyMail"].ToString();
                    string smtpHost = ConfigurationSettings.AppSettings["SmtpHost"].ToString();
                    int smtpPort = Convert.ToInt32(ConfigurationSettings.AppSettings["SmtpPort"]);
                    string subject = ConfigurationSettings.AppSettings["Subject"].ToString();

                    
                   
                    string[] _tmpdata = dataDiretorio.Split('.');
                    string novaData = string.Format("{0}-{1}-{2}", _tmpdata[2], _tmpdata[1], _tmpdata[0]);
                    //#if DEBUG
                    //                    endemail = "ricardoosilva@kpmg.com.br";
                    //#else
                    //                        endemail = "br-fmkdsc@kpmg.com.br";
                    //#endif

                    mail.IsBodyHtml = true;
                    if (modoTeste == "true")
                    {
                        /* No modo de teste o corpo do e-mail será alterado, os e-mails serão enviados para um usuário apenas e os gerentes e encarregados serão exibidos no corpo do e-mail.*/
                        
                        var gerentesEncarregados = "<h1 style='background:#fbfbaf'>Atenção</h1><h3>Este é um e-mail de teste, o e-mail real deverá ser enviados para as pessoas abaixo:</h3>";
                        string gerentes ="<div>Gerente(s):</div>";
                        string encarregados = "<div>Encarregado(s):</div>";
                        foreach (DocumentoModel email in emails)
                        {
                            if (mail.To.Where(x => x.Address == email.gerente.email).ToList().Count == 0)
                            {
                                gerentes += string.Format("<ul><li>Nome: {0}</li> <li>E-mail: {1}</li></ul>",email.gerente.nome, email.gerente.email);
                            }

                            if (mail.To.Where(x => x.Address == email.encarregado.email).ToList().Count == 0)
                            {
                                encarregados += string.Format("<ul><li>Nome: {0}</li> <li>E-mail: {1}</li></ul>", email.encarregado.nome, email.encarregado.email);
                            }
                        }
                        bodyMail += gerentesEncarregados + gerentes + encarregados;
                        endemail = ConfigurationSettings.AppSettings["Teste_MailTo"].ToString();
                        string[] endemails = endemail.Split(';');
                        foreach (var to in endemails) mail.To.Add(to);                        
                    }
                    else
                    {                        
                        // --
                        // Inclui todos os líderes encontrados
                        foreach (DocumentoModel email in emails)
                        {
                            if (mail.To.Where(x => x.Address == email.gerente.email).ToList().Count == 0)
                            {
                                mail.To.Add(email.gerente.email);
                            }

                            if (mail.To.Where(x => x.Address == email.encarregado.email).ToList().Count == 0)
                            {
                                mail.To.Add(email.encarregado.email);
                            }
                        }
                    }
                    
                    mail.From = new MailAddress(endemail);
                    mail.CC.Add(mail.From);
                    mail.Subject = subject + novaData;
                    mail.Body = String.Format(bodyMail, job.job, docsEmail, job.novoDiretorioDestino);
                    mail.Priority = MailPriority.Normal;
                    SmtpClient SmtpServer = new SmtpClient() { Host = smtpHost, Port = smtpPort };
                    // --
                    // Envia o email
                    //SmtpServer.Send(mail);
                    arquivosEnviadoComSucesso.Add(job);
                }
                catch (Exception ex)
                {
                    job.erro = true;
                    job.mensagemDeErro = ex.Message;
                    arquivosErro.Add(job);
                    retornoErro = string.Format("Ocorreu erro durante envio de e-mail. {0}", ex.Message);
                }
            }
            if (arquivosErro.Count == 0)
            {
                novaColecaoDocumentos = ManageFiles.CopiaMoveArquivos(colecaoDocumentos, dataDiretorio, out retornoErro);
                return novaColecaoDocumentos;
            }
            else
            {
                return colecaoDocumentos;
            }            
            
        }
    }
}