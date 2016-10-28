using kdscweb.Business.Component;
using kdscweb.Shared;
using kdscweb.Shared.Entity;
using System;
using System.Collections.Generic;
using System.IO;

namespace kdscweb.Business
{
    public class Log
    {
        private string p;
        private List<ColecaoDocumentosModel> colecaoDocumentos;



        public Log(DocumentoModel documento)
        {
            TAB_KDSC_LOG logTeste = new TAB_KDSC_LOG();
            logTeste.DATA_EXECUCAO = DateTime.Now;
            logTeste.JOB = documento.job;
            if (documento.mensagemDeErro == null) logTeste.TIPO = "Success";
            else logTeste.TIPO = "Error";
            logTeste.MENSAGEM_ERRO = documento.mensagemDeErro;
            InseriLog(logTeste);
        }

        public Log(List<ColecaoDocumentosModel> colecaoDocumentos)
        {

            List<DocumentoModel> documentos = colecaoDocumentos.Find(x => x.tipoArquivo != "arquivosNaoEncontrados" && x.documento.Count > 0).documento;
            foreach (var item in documentos)
            {
                TAB_KDSC_LOG logTeste = new TAB_KDSC_LOG();
                logTeste.DATA_EXECUCAO = DateTime.Now;
                logTeste.JOB = item.job;
                if (item.mensagemDeErro == null) logTeste.TIPO = "Success";
                else logTeste.TIPO = "Error";
                logTeste.MENSAGEM_ERRO = item.mensagemDeErro;

                InseriLog(logTeste);
            }


        }

        public Log(string Observacao_log, DocumentoModel documento)
        {
            TAB_KDSC_LOG log = new TAB_KDSC_LOG();
            log.DATA_EXECUCAO = DateTime.Now;
            log.JOB = documento.job;
            if (documento.mensagemDeErro == null) log.TIPO = "Success";
            else log.TIPO = "Error";
            log.MENSAGEM_ERRO = documento.mensagemDeErro;
            log.OBSERVACAO = Observacao_log;
            InseriLog(log);
        }

        public Log(string Observacao_log)
        {
            TAB_KDSC_LOG log = new TAB_KDSC_LOG();
            log.DATA_EXECUCAO = DateTime.Now;
            log.OBSERVACAO = Observacao_log;
            InseriLog(log);
        }

        public Log(string MensagemAdicional_Log, List<ColecaoDocumentosModel> colecaoDocumentos)
        {
            List<DocumentoModel> documentos = colecaoDocumentos.Find(x => x.tipoArquivo != "arquivosNaoEncontrados" && x.documento.Count > 0).documento;
            foreach (var item in documentos)
            {
                TAB_KDSC_LOG logTeste = new TAB_KDSC_LOG();
                logTeste.DATA_EXECUCAO = DateTime.Now;
                logTeste.JOB = item.job;
                if (item.mensagemDeErro == null) logTeste.TIPO = "Success";
                else logTeste.TIPO = "Error";
                logTeste.MENSAGEM_ERRO = item.mensagemDeErro;
                logTeste.OBSERVACAO = MensagemAdicional_Log;

                InseriLog(logTeste);
            }
        }

        private void InseriLog(TAB_KDSC_LOG logTeste)
        {
            try
            {
                var _unitOfWork = new UnitOfWork();
                var _baseComponent = new BaseComponent(_unitOfWork);
                _baseComponent.InsertLog(logTeste);
            }
            catch (Exception ex)
            {
                throw;
            }

        }



    }
}
