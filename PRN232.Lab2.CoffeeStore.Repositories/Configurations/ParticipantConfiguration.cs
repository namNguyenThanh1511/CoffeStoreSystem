using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;

namespace PRN232.Lab2.CoffeeStore.Repositories.Configurations
{
    public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
    {
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
            builder.ToTable("Participants");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasOne(p => p.Conversation)
                   .WithMany(c => c.Participants)
                   .HasForeignKey(p => p.ConversationId);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Participants)
                   .HasForeignKey(p => p.UserId);
        }
    }
}
