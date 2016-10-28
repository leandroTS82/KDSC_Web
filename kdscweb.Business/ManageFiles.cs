using System.Collections.Generic;
using kdscweb.Shared;
using System.IO;
using System;
using System.Net.Mail;
using System.Net;
using System.Linq;

namespace kdscweb.Business
{
    public class ManageFiles
    {
        public static List<ColecaoDocumentosModel> LeArquivos(string caminho, List<DocumentoModel> planilhaCtrl, out string retornoErro, out string dataDiretorio)
        {
            List<ColecaoDocumentosModel> colecaoDocumentos = new List<ColecaoDocumentosModel>();
            retornoErro = "";
            dataDiretorio = "";
            try
            {
                // --
                // Lê todos os arquivos dentro do diretório
                // informado, assim como os subdiretórios
                List<FileInfo> arquivosNaoEncontrados = new List<FileInfo>();
                List<DocumentoModel> arquivosEncontrados = new List<DocumentoModel>();
                List<DocumentoModel> arquivosErro = new List<DocumentoModel>();
                List<DocumentoModel> arquivosEmailErro = new List<DocumentoModel>();
                List<DocumentoModel> arquivosErroDuplicado = new List<DocumentoModel>();
                List<DocumentoModel> arquivosEnviadoComSucesso = new List<DocumentoModel>();

                string[] _t = caminho.Split('\\');
                dataDiretorio = _t[_t.Length - 1];

                foreach (string nomeArquivo in Directory.GetFiles(caminho, "*.pdf", SearchOption.AllDirectories))
                {

                    FileInfo arquivo = new FileInfo(nomeArquivo);
                    // --
                    // Se o nome do último diretório for "Demandas não identificadas com responsável"
                    // eu ignoro os arquivos
                    string[] _tmpdirarq = arquivo.FullName.Split('\\');
                    if (_tmpdirarq[_tmpdirarq.Length - 2] == "Demandas não identificadas com responsável")
                        continue;

                    string jobTemp = arquivo.Name.Split('_')[0];
                    // --
                    // Verifica se o nome do arquivo começa com OK
                    // se o nome do arquivo começar com OK o documento já
                    // foi enviado por email para os profissionais
                    jobTemp = jobTemp == "OK" ? arquivo.Name.Split('_')[1] : jobTemp;
                    // --
                    // Procura na planilha de controle se o job está lá
                    DocumentoModel doc = planilhaCtrl.Find(x => x.job == jobTemp);
                    // --
                    // Se não for encontrado o job na planilha de controle
                    // guardamos o aqruivo na planilha de arquivos não encontrados
                    // para criar o log depois
                    // --
                    // Caso o nome do arquivo não esteja parametrizado
                    // também tratamos como arquivo não encontrado
                    if (doc == null || arquivo.Name.Split('_').Length == 0)
                    {
                        arquivosNaoEncontrados.Add(arquivo);
                        continue;
                    }

                    // --
                    // Se o arquivo for encontrado guardamos o restante das
                    // informações no objeto
                    doc.arquivo = arquivo;
                    doc.emailEnviado = arquivo.Name.Split('_')[0] == "OK"; 

                    // --
                    // Adiciona o documento na lista de arquivos a serem
                    // copiados
                    DocumentoModel _doc = new DocumentoModel();
                    copyDoc(ref _doc, doc);
                    arquivosEncontrados.Add(_doc);

                }
                // cria coleção de documentos
                colecaoDocumentos = DefineColecaoDocumentos.ColecaoDocumentos(arquivosNaoEncontrados, arquivosEnviadoComSucesso, arquivosEncontrados, arquivosErro, arquivosEmailErro, arquivosErroDuplicado, out retornoErro);
                //colecaoDocumentos = copiaArquivos(colecaoDocumentos, dataDiretorio, out retornoErro);
            }
            catch (Exception)
            {
                retornoErro = "Ocorreu um erro durante a leitura dos arquivos.";
            }
            return colecaoDocumentos;
        }
        public static List<ColecaoDocumentosModel> CopiaMoveArquivos(List<ColecaoDocumentosModel> colecaoDocumentos, string dataDiretorio, out string retornoErro)
        {
            List<ColecaoDocumentosModel> novaColecaoDocumentos = new List<ColecaoDocumentosModel>();
            retornoErro = "";
            List<FileInfo> arquivosNaoEncontrados = colecaoDocumentos.Find(x => x.tipoArquivo == "arquivosNaoEncontrados").fileDocumento;
            List<DocumentoModel> arquivosEncontrados = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEncontrados");
            List<DocumentoModel> arquivosErro = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosErro");
            List<DocumentoModel> arquivosEmailErro = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEmailErro");
            List<DocumentoModel> arquivosErroDuplicado = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosErroDuplicado");
            List<DocumentoModel> arquivosEnviadoComSucesso = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEnviadoComSucesso");
            retornoErro = "";
            try
            {
                List<DocumentoModel> jobs = arquivosEncontrados.Select(x => x.job).Distinct().Select(code => arquivosEncontrados.First(x => x.job == code)).ToList();
                foreach (DocumentoModel job in jobs)
                {
                    try
                    {
                        //verifica se há algum erro com o job.
                        if (job.erro) continue;
                        // --
                        List<DocumentoModel> docs = arquivosEncontrados.FindAll(delegate(DocumentoModel x)
                        {
                            return x.job == job.job && !x.emailEnviado && !x.erro;
                        });

                        foreach (DocumentoModel doc in docs)
                        {
                            try
                            {
                                // --
                                // Verifica se o diretório existe
                                if (!Directory.Exists(doc.diretorioDestino))
                                    throw new Exception("Diretório de destino não encontrado");
                                // --
                                // Verifica o último diretório para criar 
                                // no mesmo formato na árvore do profissional

                                doc.novoDiretorioDestino = Path.Combine(doc.diretorioDestino, dataDiretorio);

                                // --
                                // Cria o diretório caso não exista
                                if (!Directory.Exists(doc.novoDiretorioDestino))
                                    Directory.CreateDirectory(doc.novoDiretorioDestino);

                                // --
                                // Copia o arquivo para o diretório destino
                                File.Copy(doc.arquivo.FullName, Path.Combine(doc.novoDiretorioDestino, doc.arquivo.Name.Replace("OK_", "")), false);
                                // Renomeia o arquivo já enviado
                                File.Move(doc.arquivo.FullName, Path.Combine(doc.arquivo.DirectoryName, "OK_" + doc.arquivo.Name));
                                doc.emailEnviado = true;
                            }
                            catch (Exception ex)
                            {
                                doc.erro = true;
                                doc.mensagemDeErro = ex.Message;
                                if (doc.emailEnviado) arquivosErroDuplicado.Add(doc);
                                else arquivosErro.Add(doc);
                                new Log(doc);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        job.erro = true;
                        job.mensagemDeErro = ex.Message;
                        arquivosErro.Add(job);
                        new Log(string.Format("Ocorreu erro durante envio de e-mail. {0}", ex.Message), job);
                    }
                }
            }
            catch (Exception)
            {
                retornoErro = "Ocorreu um erro durante o tratamento dos arquivos.";
            }

            if (retornoErro == "")
            {
                novaColecaoDocumentos = DefineColecaoDocumentos.ColecaoDocumentos(arquivosNaoEncontrados,arquivosEnviadoComSucesso, arquivosEncontrados, arquivosErro, arquivosEmailErro, arquivosErroDuplicado, out retornoErro);
            }

            return novaColecaoDocumentos;
        }
        private static void copyDoc(ref DocumentoModel _doc, DocumentoModel doc)
        {
            _doc.arquivo = doc.arquivo;
            _doc.diretorioDestino = doc.diretorioDestino;
            _doc.emailEnviado = doc.emailEnviado;
            _doc.encarregado = doc.encarregado;
            _doc.erro = doc.erro;
            _doc.gerente = doc.gerente;
            _doc.job = doc.job;
            _doc.mensagemDeErro = doc.mensagemDeErro;
            _doc.nrDemanda = doc.nrDemanda;
            _doc.linha = doc.linha;
        }

    }
}