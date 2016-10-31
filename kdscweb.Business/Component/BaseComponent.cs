using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Entity;

namespace Business.Component
{
    public class BaseComponent
    {

        #region .: Properties :.
        private UnitOfWork _unitOfWork;
        #endregion

        #region .: Constructor :.
        public BaseComponent(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        public IQueryable<Engagement> GetEngagementAll()
        {
            return _unitOfWork.EngagementRepository.Get(noTracking: true);
        }

        public IQueryable<TAB_KDSC_LOG> GetKdsc_LogAll()
        {
            return _unitOfWork.Kdsc_LogRepository.Get(noTracking: true);
        }

        public void InsertLog(TAB_KDSC_LOG log)
        {            
            _unitOfWork.Kdsc_LogRepository.Insert(log);
        }

        public Engagement GetEngagementById(string id)
        {
            return _unitOfWork.EngagementRepository.GetByID(id);
        }

       

    }
}
