using Business;
using Business.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Shared;
using System.Linq;
using Shared.Entity;

namespace WEBUI.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.Instrucoes = true;
            return View();
        }



        [HttpPost]
        public ActionResult LoadFile(FormCollection itemFile)
        {
            ViewBag.Instrucoes = false;
            var fileItem = Request.Files[0];
            var uploadFILE1 = itemFile["uploadFILE1"];

            string retornoErro = "";
            // caminho provisório para teste, este deve receber o valor do input
            string caminho = itemFile["filePath"];

            if (ValidaPadraoDir(caminho))
            {
                List<DocumentoModel> itensPlanilha = new List<DocumentoModel>();
                itensPlanilha = ReadSpreadsheet.ObtemItems(fileItem, out retornoErro);
                @ViewBag.TotalPlanilha = itensPlanilha.Count;
                if (retornoErro == "")
                {
                    if (itensPlanilha.Count > 0)
                    {
                        List<ColecaoDocumentosModel> colecaoDocumentos = new List<ColecaoDocumentosModel>();
                        string dataDiretorio = "";
                        colecaoDocumentos = ManageFiles.LeArquivos(caminho, itensPlanilha, out retornoErro, out dataDiretorio);
                        new Log("Retorno de leitura de arquivos.", colecaoDocumentos);
                        if (retornoErro == "")
                        {
                            ServicoEmail.EnviaEmail(colecaoDocumentos, itensPlanilha, dataDiretorio, out retornoErro);
                            Sumarizacao(colecaoDocumentos);
                            if (retornoErro != "") @ViewBag.error = retornoErro;
                            return View("Index");
                        }
                        else
                        {
                            @ViewBag.error = retornoErro;
                            return View("Index");
                        }

                    }
                    else
                    {
                        @ViewBag.error = "Não foram retornadas informações a partir da planilha selecionada.";
                        return View("Index");
                    }
                }
                else
                {
                    @ViewBag.error = retornoErro;
                    return View("Index");
                }
            }
            return View("Index");
        }

        private void Sumarizacao(List<ColecaoDocumentosModel> colecaoDocumentos)
        {
            ViewBag.painelSumarizacao = true;
            List<FileInfo> arquivosNaoEncontrados = colecaoDocumentos.Find(x => x.tipoArquivo == "arquivosNaoEncontrados").fileDocumento;
            List<DocumentoModel> arquivosEncontrados = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEncontrados");
            List<DocumentoModel> arquivosErro = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosErro");
            List<DocumentoModel> arquivosErroDuplicado = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosErroDuplicado");
            List<DocumentoModel> arquivosEnviadoComSucesso = DefineColecaoDocumentos.ExtraiArquivosPorTipo(colecaoDocumentos, "arquivosEnviadoComSucesso");


            if (arquivosNaoEncontrados.Count > 0) ViewBag.arquivosNaoEncontrados = arquivosNaoEncontrados;
            if (arquivosErro.Count > 0) ViewBag.arquivosErro = arquivosErro;
            if (arquivosErroDuplicado.Count > 0) ViewBag.arquivosErroDuplicado = arquivosErroDuplicado;
            if (arquivosEnviadoComSucesso.Count > 0)
            {
                ViewBag.arquivosEnviadoComSucesso = arquivosEnviadoComSucesso;
                ViewBag.CountSuccess = arquivosEnviadoComSucesso.Count;
            }
            new Log("Retorno de envio de e-mails", colecaoDocumentos);
        }

        private bool ValidaPadraoDir(string caminho)
        {
            bool retorno = true;

            if (caminho == "")
            {
                ViewBag.error = "Por favor, insira o caminho de rede válido no campo \"Diretório dos Produtos Finais\".";
                return false;
            }
            // --
            // Valida o padrão do diretório
            string[] _t = caminho.Split('\\');
            string dataDiretorio = _t[_t.Length - 1];
            string[] _tmp = dataDiretorio.Split('.');

            if (_tmp.Length != 3)
            {
                ViewBag.error = "Este diretório não é válido.";
                return false;
            }

            try
            {
                DateTime _dtmp = new DateTime(
                               Convert.ToInt32(_tmp[0]),
                               Convert.ToInt32(_tmp[1]),
                               Convert.ToInt32(_tmp[2])
                            );
            }
            catch (Exception e)
            {
                ViewBag.error = "O nome do diretório não compõe uma data válida";
                return false;
            }

            if (!Directory.Exists(caminho))
            {
                ViewBag.error = "Diretório informado não existe";
                return false;
            }

            return retorno;
        }

    }
}