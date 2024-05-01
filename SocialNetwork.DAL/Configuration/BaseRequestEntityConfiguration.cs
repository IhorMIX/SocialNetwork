using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Configuration
{
    public class BaseRequestEntityConfiguration : IEntityTypeConfiguration<BaseRequestEntity>
    {
        public void Configure(EntityTypeBuilder<BaseRequestEntity> builder)
        {
            builder.HasOne(i => i.Sender)
               .WithMany(i => i.Requests)
               .HasForeignKey(i => i.SenderId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}