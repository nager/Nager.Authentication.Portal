using Google.Authenticator;
using Microsoft.Extensions.Caching.Memory;
using Nager.AuthenticationService.Abstraction;
using Nager.AuthenticationService.Abstraction.Models;
using Nager.AuthenticationService.Abstraction.Services;
using Nager.AuthenticationService.WebApi.Helpers;
using Nager.AuthenticationService.WebApi.Models;

namespace Nager.AuthenticationService.WebApi.Services
{
    /// <summary>
    /// User Authentication Service
    /// </summary>
    /// <remarks>With Brute-Force Protection</remarks>
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly ILogger<UserAuthenticationService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheLiveTime = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _mfaTokenAcceptanceWindow = TimeSpan.FromMinutes(2);
        private readonly int _delayTimeMultiplier = 400; //ms
        private readonly int _maxInvalidLogins = 10;
        private readonly int _maxInvalidLoginsBeforeDelay = 3;
        private readonly int _timeoutDatabaseUpdate = 5; //s

        /// <summary>
        /// User Authentication Service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userRepository"></param>
        /// <param name="memoryCache"></param>
        public UserAuthenticationService(
            ILogger<UserAuthenticationService> logger,
            IUserRepository userRepository,
            IMemoryCache memoryCache)
        {
            this._logger = logger;
            this._userRepository = userRepository;
            this._memoryCache = memoryCache;
        }

        private string GetCacheKey(CacheKeyType cacheKeyType, string identifier)
        {
            return $"{cacheKeyType}.{identifier.Trim()}";
        }

        private void SetInvalidLogin(string identifier)
        {
            var cacheKey = this.GetCacheKey(CacheKeyType.AuthenticationInfo, identifier);
            if (!this._memoryCache.TryGetValue<AuthenticationInfo>(cacheKey, out var authenticationInfo))
            {
                authenticationInfo = new AuthenticationInfo();
            }

            if (authenticationInfo == null)
            {
                throw new NullReferenceException(nameof(authenticationInfo));
            }

            authenticationInfo.InvalidCount++;
            authenticationInfo.LastInvalid = DateTime.UtcNow;

            this._memoryCache.Set(cacheKey, authenticationInfo, this._cacheLiveTime);
        }

        private void SetValidLogin(string identifier)
        {
            var cacheKey = this.GetCacheKey(CacheKeyType.AuthenticationInfo, identifier);
            if (!this._memoryCache.TryGetValue<AuthenticationInfo>(cacheKey, out var authenticationInfo))
            {
                authenticationInfo = new AuthenticationInfo();
            }

            if (authenticationInfo == null)
            {
                throw new NullReferenceException(nameof(authenticationInfo));
            }

            authenticationInfo.LastValid = DateTime.UtcNow;
            authenticationInfo.InvalidCount = 0;

            this._memoryCache.Set(cacheKey, authenticationInfo, this._cacheLiveTime);
        }

        private async Task<bool> IsIdentifierBlockedAsync(string identifier)
        {
            var cacheKey = this.GetCacheKey(CacheKeyType.AuthenticationInfo, identifier);

            if (!this._memoryCache.TryGetValue<AuthenticationInfo>(cacheKey, out var authenticationInfo))
            {
                return false;
            }

            if (authenticationInfo == null)
            {
                throw new NullReferenceException(nameof(authenticationInfo));
            }

            if (authenticationInfo.InvalidCount < this._maxInvalidLoginsBeforeDelay)
            {
                return false;
            }

            await Task.Delay(authenticationInfo.InvalidCount * this._delayTimeMultiplier);

            if (authenticationInfo.InvalidCount > this._maxInvalidLogins && DateTime.UtcNow < authenticationInfo.LastInvalid.AddMinutes(2))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<AuthenticationResult> ValidateCredentialsAsync(
            AuthenticationRequest authenticationRequest,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(authenticationRequest);

            if (string.IsNullOrEmpty(authenticationRequest.IpAddress))
            {
                throw new NullReferenceException($"Missing {nameof(authenticationRequest.IpAddress)}");
            }

            if (await this.IsIdentifierBlockedAsync(authenticationRequest.IpAddress))
            {
                this._logger.LogWarning($"{nameof(ValidateCredentialsAsync)} - Block {authenticationRequest.IpAddress}");

                return new AuthenticationResult
                {
                    Status = AuthenticationStatus.TemporaryBlocked
                };
            }

            if (await this.IsIdentifierBlockedAsync(authenticationRequest.EmailAddress))
            {
                this._logger.LogWarning($"{nameof(ValidateCredentialsAsync)} - Block {authenticationRequest.EmailAddress}");

                return new AuthenticationResult
                {
                    Status = AuthenticationStatus.TemporaryBlocked
                };
            }

            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == authenticationRequest.EmailAddress, cancellationToken);
            if (userEntity == null)
            {
                this.SetInvalidLogin(authenticationRequest.IpAddress);
                this.SetInvalidLogin(authenticationRequest.EmailAddress);

                return new AuthenticationResult
                {
                    Status = AuthenticationStatus.Invalid
                };
            }

            if (userEntity.IsLocked)
            {
                this._logger.LogWarning($"{nameof(ValidateCredentialsAsync)} - User is locked {authenticationRequest.EmailAddress}");

                return new AuthenticationResult
                {
                    Status = AuthenticationStatus.Invalid
                };
            }

            if (userEntity.PasswordHash == null)
            {
                throw new NullReferenceException(nameof(userEntity.PasswordHash));
            }

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(this._timeoutDatabaseUpdate));

            var passwordHash = PasswordHelper.HashPasword(authenticationRequest.Password, userEntity.PasswordSalt);
            if (userEntity.PasswordHash.SequenceEqual(passwordHash))
            {
                if (userEntity.MfaActive)
                {
                    var mfaIdentifier = Guid.NewGuid().ToString();
                    var cacheKey = this.GetCacheKey(CacheKeyType.MfaIdentifier, mfaIdentifier);

                    this._memoryCache.Set(cacheKey, authenticationRequest.EmailAddress, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = this._mfaTokenAcceptanceWindow
                    });

                    return new AuthenticationResult
                    {
                        Status = AuthenticationStatus.MfaCodeRequired,
                        MfaIdentifier = mfaIdentifier
                    };
                }

                this.SetValidLogin(authenticationRequest.IpAddress);
                this.SetValidLogin(authenticationRequest.EmailAddress);

                await this._userRepository.SetLastSuccessfulValidationTimestampAsync(o => o.Id == userEntity.Id, cancellationTokenSource.Token)
                    .ContinueWith(task => { }); ;

                return new AuthenticationResult
                {
                    Status = AuthenticationStatus.Valid
                };
            }

            this.SetInvalidLogin(authenticationRequest.IpAddress);
            this.SetInvalidLogin(authenticationRequest.EmailAddress);

            await this._userRepository.SetLastValidationTimestampAsync(o => o.Id == userEntity.Id, cancellationTokenSource.Token)
                .ContinueWith(task => { }); ;

            return new AuthenticationResult
            {
                Status = AuthenticationStatus.Invalid
            };
        }

        /// <inheritdoc />
        public async Task<UserInfo?> GetUserInfoAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == emailAddress);
            if (userEntity == null)
            {
                return null;
            }

            return new UserInfo
            {
                Id = userEntity.Id,
                EmailAddress = userEntity.EmailAddress,
                Firstname = userEntity.Firstname,
                Lastname = userEntity.Lastname,
                Roles = RoleHelper.GetRoles(userEntity.RolesData)
            };
        }

        /// <inheritdoc />
        public async Task<ValidateTokenResult> ValidateTokenAsync(
            ValidateTokenRequest validateTokenRequest,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = this.GetCacheKey(CacheKeyType.MfaIdentifier, validateTokenRequest.MfaIdentifier);
            if (!this._memoryCache.TryGetValue<string>(cacheKey, out var emailAddress))
            {
                this._logger.LogError($"{nameof(ValidateTokenAsync)} - CacheKey {cacheKey} not found");

                return new ValidateTokenResult
                {
                    Success = false
                };
            }

            if (string.IsNullOrEmpty(emailAddress))
            {
                this._logger.LogError($"{nameof(ValidateTokenAsync)} - EmailAddress is empty");

                return new ValidateTokenResult
                {
                    Success = false
                };
            }

            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == emailAddress);
            if (userEntity == null)
            {
                this._logger.LogError($"{nameof(ValidateTokenAsync)} - No User available");

                return new ValidateTokenResult
                {
                    Success = false
                };
            }

            var timeTolerance = TimeSpan.FromSeconds(20);
            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            var isTokenValid = twoFactorAuthenticator.ValidateTwoFactorPIN(userEntity.MfaSecret, validateTokenRequest.Token, timeTolerance);

            if (isTokenValid)
            {
                this._memoryCache.Remove(cacheKey);

                if (!string.IsNullOrEmpty(validateTokenRequest.IpAddress))
                {
                    this.SetValidLogin(validateTokenRequest.IpAddress);
                }
                this.SetValidLogin(emailAddress);

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(this._timeoutDatabaseUpdate));
                await this._userRepository.SetLastSuccessfulValidationTimestampAsync(o => o.Id == userEntity.Id, cancellationTokenSource.Token)
                    .ContinueWith(task => { });
            }

            return new ValidateTokenResult
            {
                Success = isTokenValid,
                EmailAddress = emailAddress
            };
        }
    }
}
