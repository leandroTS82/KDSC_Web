using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Entity;

namespace Data.Configuration
{
    public class EngagementMap : EntityTypeConfiguration<Engagement>
    {
        public EngagementMap()
        {
            // Primary Key
            this.HasKey(t => t.ENGAGEMENT_ID);
          
            // Table & Column Mappings
            this.ToTable("VW_ENGAGEMENT");
           
        }
    }
}
