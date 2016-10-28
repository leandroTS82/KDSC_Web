using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kdscweb.Shared.Entity
{
    public class TAB_KDSC_LOG
    {
        public int ID { get; set; }
        public string JOB { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        public string MENSAGEM_ERRO { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        public string OBSERVACAO { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string USUARIO_LOGADO { get; set; }
        public string TIPO { get; set; }
        public DateTime DATA_EXECUCAO { get; set; }
    }
}
