using PRN232.Lab2.CoffeeStore.Repositories.CategoryRepository;
using PRN232.Lab2.CoffeeStore.Repositories.CoffeeAddonRepository;
using PRN232.Lab2.CoffeeStore.Repositories.CoffeeVariantRepository;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Repositories.GenericRepository;
using PRN232.Lab2.CoffeeStore.Repositories.MenuRepository;
using PRN232.Lab2.CoffeeStore.Repositories.OrderDetailRepository;
using PRN232.Lab2.CoffeeStore.Repositories.OrderRepository;
using PRN232.Lab2.CoffeeStore.Repositories.PaymentRepository;
using PRN232.Lab2.CoffeeStore.Repositories.ProductRepository;
using PRN232.Lab2.CoffeeStore.Repositories.UserRepository;

namespace PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }

        ICoffeeAddonRepository CoffeeAddons { get; }

        ICoffeeVariantRepository CoffeeVariants { get; }

        IMenuRepository Menus { get; }

        ICategoryRepository Categories { get; }

        IUserRepository Users { get; }

        IOrderRepository Orders { get; }

        IOrderDetailRepository OrderDetails { get; }

        IPaymentRepository Payments { get; }

        IGenericRepository<Conversation> Conversations { get; }
        IGenericRepository<Participant> Participants { get; }
        IGenericRepository<Message> Messages { get; }


        Task BeginTransaction();

        Task CommitTransaction();

        Task RollbackTransaction();

        Task<ITransaction?> GetCurrentTransaction();

        Task<int> SaveChangesAsync();
    }
}
