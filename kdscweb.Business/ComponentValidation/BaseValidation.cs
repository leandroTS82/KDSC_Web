using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using br.kpmg.Framework64.Web.KModels;

namespace kdscweb.Business.Validation
{
    public static class BaseValidation
    {
        public static KMessage KMessage;

        static BaseValidation()
        {
            KMessage = new KMessage();
            KMessage.Errors = new List<string>();
        }


        public static void Save()
        {              
                KMessage.Type = KMessage.Types.Success;
                KMessage.Message = "Registro gravado com sucesso.";
                KMessage.Autoclose = false;
                KMessage.Errors = new List<string>();
         
        }

        public static void Update()
        {
          
                KMessage.Type = KMessage.Types.Success;
                KMessage.Message = "Registro atualizado com sucesso.";
                KMessage.Autoclose = false;
                KMessage.Errors = new List<string>();
                KMessage.Autoclose = false;


        }


    }
}