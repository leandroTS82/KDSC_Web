using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Shared
{
    public static class DefineColecaoDocumentos
    {
        public static List<ColecaoDocumentosModel> ColecaoDocumentos(List<FileInfo> arquivosNaoEncontrados,List<DocumentoModel>arquivosEnviadoComSucesso, List<DocumentoModel> arquivosEncontrados, List<DocumentoModel> arquivosErro, List<DocumentoModel> arquivosEmailErro, List<DocumentoModel> arquivosErroDuplicado, out string retornoErro)
        {
            List<ColecaoDocumentosModel> col_Docs = new List<ColecaoDocumentosModel>();
            retornoErro = "";
            try
            {
                
                    ColecaoDocumentosModel col_arquivosEncontrados = new ColecaoDocumentosModel();
                    col_arquivosEncontrados.tipoArquivo = "arquivosEncontrados";
                    col_arquivosEncontrados.documento = arquivosEncontrados;
                    col_Docs.Add(col_arquivosEncontrados);                
               
                    ColecaoDocumentosModel col_arquivosErroDuplicado = new ColecaoDocumentosModel();
                    col_arquivosErroDuplicado.tipoArquivo = "arquivosErroDuplicado";
                    col_arquivosErroDuplicado.documento = arquivosErroDuplicado;
                    col_Docs.Add(col_arquivosErroDuplicado);              

                    ColecaoDocumentosModel col_arquivosErro = new ColecaoDocumentosModel();
                    col_arquivosErro.tipoArquivo = "arquivosErro";
                    col_arquivosErro.documento = arquivosErro;
                    col_Docs.Add(col_arquivosErro);                             

                    ColecaoDocumentosModel col_arquivosNaoEncontrados = new ColecaoDocumentosModel();
                    col_arquivosNaoEncontrados.tipoArquivo = "arquivosNaoEncontrados";
                    col_arquivosNaoEncontrados.fileDocumento = arquivosNaoEncontrados;
                    col_Docs.Add(col_arquivosNaoEncontrados);   
                
                    ColecaoDocumentosModel col_arquivosEmailErro = new ColecaoDocumentosModel();
                    col_arquivosEmailErro.tipoArquivo = "arquivosEmailErro";
                    col_arquivosEmailErro.documento = arquivosEmailErro;
                    col_Docs.Add(col_arquivosEmailErro);


                    ColecaoDocumentosModel col_arquivosEnviadoComSucesso = new ColecaoDocumentosModel();
                    col_arquivosEnviadoComSucesso.tipoArquivo = "arquivosEnviadoComSucesso";
                    col_arquivosEnviadoComSucesso.documento = arquivosEnviadoComSucesso;
                    col_Docs.Add(col_arquivosEnviadoComSucesso);
                
            }
            catch (Exception)
            {
                retornoErro = "Ocorreu um erro durante a Definição da coleção de documentos de retorno.";
            }

            return col_Docs;
        }

        public static List<DocumentoModel> ExtraiArquivosPorTipo(List<ColecaoDocumentosModel> ColecaoDocumentos, string tipoArquivo)
        {
            List<DocumentoModel> arquivos = new List<DocumentoModel>();
            arquivos = ColecaoDocumentos.Find(x => x.tipoArquivo == tipoArquivo).documento;
            return arquivos;
        }
    }
}