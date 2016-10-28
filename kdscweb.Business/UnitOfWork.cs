using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kdscweb.Data;
using kdscweb.Data.Context;
using kdscweb.Shared.Entity;

namespace kdscweb.Business
{
    public class UnitOfWork : IDisposable
    {
        private kdscwebContext context = new kdscwebContext();

        #region .: Engagement :.
        private GenericRepository<Engagement> engagementRepository;

        public GenericRepository<Engagement> EngagementRepository
        {
            get
            {
                if (this.engagementRepository == null)
                {
                    this.engagementRepository = new GenericRepository<Engagement>(context);
                }
                return engagementRepository;
            }
        }
        #endregion
        #region .: Kdsc_Log :.
        private GenericRepository<TAB_KDSC_LOG> kdsc_LogRepository;

        public GenericRepository<TAB_KDSC_LOG> Kdsc_LogRepository
        {
            get
            {
                if (this.kdsc_LogRepository == null)
                {
                    this.kdsc_LogRepository = new GenericRepository<TAB_KDSC_LOG>(context);
                }
                return kdsc_LogRepository;
            }
        }
        #endregion
      
        

        #region .: Common :.
        public void Detach(object entity)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext.Detach(entity);
        }

        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region .: Dispose :.
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
