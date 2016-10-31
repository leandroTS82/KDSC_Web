using ExcelInterop = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Web;
using Shared;
using System.Data;
using Excel;
using System.IO;

namespace Business
{
    public class ReadSpreadsheet
    {
        static public List<DocumentoModel> ObtemItems(HttpPostedFileBase fileItem, out string retorno)
        {
            List<DocumentoModel> planilhaCtrl = new List<DocumentoModel>();
            retorno = "";
            try
            {
                Stream stream = fileItem.InputStream;
                IExcelDataReader reader = null;
                if (fileItem.FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileItem.FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                DataSet result = reader.AsDataSet();
                reader.Close();
                var planCtrlRange = result.Tables[0];
                for (int rCnt = 0; rCnt < planCtrlRange.Rows.Count; rCnt++)
                {
                    // Instancia
                    DocumentoModel doc = new DocumentoModel();
                    // --
                    // Layout default do arquivo (antigo)
                    doc.linha = rCnt;
                    doc.nrDemanda = Convert.ToString(planCtrlRange.Rows[rCnt].ItemArray[0]);
                    doc.job = Convert.ToString((planCtrlRange.Rows[rCnt].ItemArray[12]));
                    doc.diretorioDestino = Convert.ToString(planCtrlRange.Rows[rCnt].ItemArray[13]);
                    doc.gerente = new ProfissionalModel()
                    {
                        nome = Convert.ToString(planCtrlRange.Rows[rCnt].ItemArray[15]),
                        email = Convert.ToString(planCtrlRange.Rows[rCnt].ItemArray[16]),
                    };
                    doc.encarregado = new ProfissionalModel()
                    {
                        nome = Convert.ToString(planCtrlRange.Rows[rCnt].ItemArray[17]),
                        email = Convert.ToString(planCtrlRange.Rows[rCnt].ItemArray[18]),
                    };
                    // --
                    // Adiciona na lista
                    planilhaCtrl.Add(doc);
                }
            }
            catch (Exception e)
            {
                retorno = string.Format("Ocorreu um erro durante a leitura da planilha de controle. error message: {0}", e.Message);
            }
            return planilhaCtrl;
        }

    }
}
