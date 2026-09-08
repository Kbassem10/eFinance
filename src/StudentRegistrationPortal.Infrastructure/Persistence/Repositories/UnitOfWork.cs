using Microsoft.Extensions.Logging;
using MySqlConnector;
using StudentRegistrationPortal.Application.Common.Interfaces;
using StudentRegistrationPortal.Infrastructure.Persistence.DbContext;

namespace StudentRegistrationPortal.Infrastructure.Persistence.Repositories;

/// Implements Unit of Work pattern managing database connection and transaction lifecycle.
public class UnitOfWork : IUnitOfWork
{
    private readonly MySqlDataSource _dataSource;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILoggerFactory _loggerFactory;
    private MySqlConnection? _connection;
    private MySqlTransaction? _transaction;
    private IStudentRepository? _students;
    private IUserRepository? _users;
    private ICoursesRepository? _courses;
    private IAdminRepository? _admin;
    private ILookupRepository? _lookups;
    private IAuthRepository? _auth;
    private bool _disposed;

    public UnitOfWork(MySqlDataSource dataSource, ApplicationDbContext dbContext, ILoggerFactory loggerFactory)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public IStudentRepository Students =>
        _students ??= new StudentRepository(
            connectionProvider: async () =>
            {
                _connection ??= await _dataSource.OpenConnectionAsync();
                return _connection;
            },
            transactionProvider: () => _transaction,
            _loggerFactory.CreateLogger<StudentRepository>());

    public IUserRepository Users =>
        _users ??= new UserRepository(
            connectionProvider: async () =>
            {
                _connection ??= await _dataSource.OpenConnectionAsync();
                return _connection;
            },
            transactionProvider: () => _transaction,
            _loggerFactory.CreateLogger<UserRepository>());

    public ICoursesRepository Courses =>
        _courses ??= new CoursesRepository(
            connectionProvider: async () =>
            {
                _connection ??= await _dataSource.OpenConnectionAsync();
                return _connection;
            },
            transactionProvider: () => _transaction,
            _loggerFactory.CreateLogger<CoursesRepository>());

    public IAdminRepository Admin =>
        _admin ??= new AdminRepository(
            connectionProvider: async () =>
            {
                _connection ??= await _dataSource.OpenConnectionAsync();
                return _connection;
            },
            transactionProvider: () => _transaction,
            _loggerFactory.CreateLogger<AdminRepository>());

    public ILookupRepository Lookups =>
        _lookups ??= new LookupRepository(
            connectionProvider: async () =>
            {
                _connection ??= await _dataSource.OpenConnectionAsync();
                return _connection;
            },
            transactionProvider: () => _transaction,
            _dbContext,
            _loggerFactory.CreateLogger<LookupRepository>());

    public IAuthRepository Auth =>
        _auth ??= new AuthRepository(
            connectionProvider: async () =>
            {
                _connection ??= await _dataSource.OpenConnectionAsync();
                return _connection;
            },
            transactionProvider: () => _transaction,
            _loggerFactory.CreateLogger<AuthRepository>());

    public async Task BeginTransactionAsync()
    {
        _connection ??= await _dataSource.OpenConnectionAsync();
        _transaction = await _connection.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }
}
