using System;
using System.Threading.Tasks;
using sanaiy.BLL.Entities;

namespace sanaiy.BLL.Interfaces
{
    /// <summary>
    /// Unit of Work pattern - manages transactions and repositories
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Client> Clients { get; }
        IGenericRepository<Craftsman> Craftsmen { get; }
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<Wallet> Wallets { get; }
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Quote> Quotes { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<PaymentTransaction> PaymentTransactions { get; }
        IGenericRepository<Violation> Violations { get; }
        IGenericRepository<Commission> Commissions { get; }
        IGenericRepository<WalletTransaction> WalletTransactions { get; }
        IGenericRepository<CraftsmanAvailability> CraftsmanAvailability { get; }



        int Complete();
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
