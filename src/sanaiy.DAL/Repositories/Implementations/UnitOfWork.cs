using Microsoft.EntityFrameworkCore.Storage;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Interfaces;
using sanaiy.DAL.Context;
using System;
using System.Threading.Tasks;

namespace sanaiy.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Repository instances
        private IGenericRepository<Category>? _categories;
        private IGenericRepository<Client>? _clients;
        private IGenericRepository<Craftsman>? _craftsmen;
        private IGenericRepository<Service>? _services;
        private IGenericRepository<Address>? _addresses;
        private IGenericRepository<Wallet>? _wallets;
        private IGenericRepository<Booking>? _bookings;
        private IGenericRepository<Quote>? _quotes;
        private IGenericRepository<Review>? _reviews;
        private IGenericRepository<PaymentTransaction>? _paymentTransactions;
        private IGenericRepository<Violation>? _violations;
        private IGenericRepository<Commission>? _commissions;
        private IGenericRepository<WalletTransaction>? _walletTransactions;
        private IGenericRepository<CraftsmanAvailability>? _craftsmanAvailability;


        public IGenericRepository<CraftsmanAvailability> CraftsmanAvailability
        {
            get { return _craftsmanAvailability ??= new GenericRepository<CraftsmanAvailability>(_context); }
        }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================
        // Repository Properties (Lazy Loading)
        // ============================================

        public IGenericRepository<Category> Categories
        {
            get { return _categories ??= new GenericRepository<Category>(_context); }
        }

        public IGenericRepository<Client> Clients
        {
            get { return _clients ??= new GenericRepository<Client>(_context); }
        }

        public IGenericRepository<Craftsman> Craftsmen
        {
            get { return _craftsmen ??= new GenericRepository<Craftsman>(_context); }
        }

        public IGenericRepository<Service> Services
        {
            get { return _services ??= new GenericRepository<Service>(_context); }
        }

        public IGenericRepository<Address> Addresses
        {
            get { return _addresses ??= new GenericRepository<Address>(_context); }
        }

        public IGenericRepository<Wallet> Wallets
        {
            get { return _wallets ??= new GenericRepository<Wallet>(_context); }
        }

        public IGenericRepository<Booking> Bookings
        {
            get { return _bookings ??= new GenericRepository<Booking>(_context); }
        }

        public IGenericRepository<Quote> Quotes
        {
            get { return _quotes ??= new GenericRepository<Quote>(_context); }
        }

        public IGenericRepository<Review> Reviews
        {
            get { return _reviews ??= new GenericRepository<Review>(_context); }
        }

        public IGenericRepository<PaymentTransaction> PaymentTransactions
        {
            get { return _paymentTransactions ??= new GenericRepository<PaymentTransaction>(_context); }
        }

        public IGenericRepository<Violation> Violations
        {
            get { return _violations ??= new GenericRepository<Violation>(_context); }
        }

        public IGenericRepository<Commission> Commissions
        {
            get { return _commissions ??= new GenericRepository<Commission>(_context); }
        }

        public IGenericRepository<WalletTransaction> WalletTransactions
        {
            get { return _walletTransactions ??= new GenericRepository<WalletTransaction>(_context); }
        }

        // ============================================
        // Transaction Methods
        // ============================================

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        // ============================================
        // Dispose Pattern
        // ============================================

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}