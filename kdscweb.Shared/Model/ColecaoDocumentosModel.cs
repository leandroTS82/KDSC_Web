﻿using System.Collections.Generic;
using System.IO;

namespace Shared
{
    public class ColecaoDocumentosModel
    {
        public string tipoArquivo { get; set; }
        public List<DocumentoModel> documento { get; set; }
        public List<FileInfo> fileDocumento { get; set; }
    }
}