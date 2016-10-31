using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Shared
{
    public class DocumentoModel
    {
        public int linha { get; set; }
        public string nrDemanda { get; set; }
        public string job { get; set; }
        public ProfissionalModel gerente { get; set; }
        public ProfissionalModel encarregado { get; set; }
        public string diretorioDestino { get; set; }
        public string novoDiretorioDestino { get; set; }
        public FileInfo arquivo { get; set; }
        public bool emailEnviado { get; set; }
        public bool erro { get; set; }
        public string mensagemDeErro { get; set; }
        public string mensagemDeSucesso { get; set; }
    }
}