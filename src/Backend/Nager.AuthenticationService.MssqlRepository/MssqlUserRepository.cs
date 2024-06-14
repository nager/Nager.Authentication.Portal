using Microsoft.EntityFrameworkCore;
using Nager.Authentication.Abstraction.Entities;
using Nager.Authentication.Abstraction.Validators;
using System.Linq.Expressions;

namespace Nager.AuthenticationService.MssqlRepository
{
    public class MssqlUserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseContext;

        public MssqlUserRepository(DatabaseContext databaseContext)
        {
            this._databaseContext = databaseContext;
        }

        public async Task<UserEntity[]> QueryAsync(
            int take,
            int skip,
            Expression<Func<UserEntity, bool>>? predicate = default,
            CancellationToken cancellationToken = default)
        {
            var query = this._databaseContext.Users.AsNoTracking().AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query
                .OrderBy(o => o.Id)
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<UserEntity?> GetAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var query = this._databaseContext.Users.AsNoTracking().AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> AddAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default)
        {
            this._databaseContext.Users.Add(entity);
            await this._databaseContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UpdateAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default)
        {
            var existingItem = await this._databaseContext.Users.SingleOrDefaultAsync(o => o.Id == entity.Id, cancellationToken);
            if (existingItem == null)
            {
                return false;
            }

            existingItem.Firstname = entity.Firstname;
            existingItem.Lastname = entity.Lastname;
            existingItem.EmailAddress = entity.EmailAddress;
            existingItem.RolesData = entity.RolesData;
            existingItem.PasswordHash = entity.PasswordHash;
            existingItem.IsLocked = entity.IsLocked;
            existingItem.MfaSecret = entity.MfaSecret;
            existingItem.MfaActive = entity.MfaActive;

            await this._databaseContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var totalRowsDeleted = await this._databaseContext.Users
                .Where(predicate)
                .ExecuteDeleteAsync(cancellationToken);

            return totalRowsDeleted > 0;
        }

        public async Task<bool> SetLastValidationTimestampAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var updatedRows = await this._databaseContext.Users
                .Where(predicate)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.LastFailedValidationTimestamp, DateTime.UtcNow), cancellationToken);

            return updatedRows > 0;
        }

        public async Task<bool> SetLastSuccessfulValidationTimestampAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var updatedRows = await this._databaseContext.Users
                .Where(predicate)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.LastSuccessfulValidationTimestamp, DateTime.UtcNow), cancellationToken);

            return updatedRows > 0;
        }
    }
}
