using AuthService.Application.DTOs.Request;
using AuthService.Domain.Entities.Users;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Enums;
using JwtConfiguration;


namespace AuthService.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtBuilder _tokenGenerator;
        //private readonly PasswordHasher _passwordHasher;
        //private readonly IEmailService _emailService;

        public AuthenticationService(
            IUserRepository userRepository,
            IJwtBuilder tokenGenerator
            //PasswordHasher passwordHasher,
            //IEmailService emailService
            )
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            //_passwordHasher = passwordHasher;
            //_emailService = emailService;
        }

        public async Task<string> RegisterUserAsync(RegisterRequest registerRequest)
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(registerRequest.Username);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerRequest.Username,
                Email = registerRequest.Email,
                //PasswordHash = _passwordHasher.HashPassword(registerRequest.Password),
                PasswordHash = registerRequest.Password,
                //UserType = (UserType)registerRequest.UserType,
                UserType = UserType.RegularUser,
                Profile = new UserProfile
                {
                    FullName = registerRequest.FullName,
                    ProfilePictureUrl = registerRequest.AvatarUrl,
                    Email = registerRequest.Email,
                }

            };

            await _userRepository.CreateUserAsync(user);

            // Send welcome email
            //await _emailService.SendEmailAsync(
            //    user.Email,
            //    "Welcome to Social Media Platform",
            //    "Thank you for registering!"
            //);
             Console.WriteLine("Thank you for registering!");

            return _tokenGenerator.GetToken(user.Id);
        }

        public async Task<string> LoginAsync(LoginRequest login)
        {
            var user = await _userRepository.GetUserByEmailAsync(login.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Username does not exists");

            //if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            //    throw new UnauthorizedAccessException("Invalid username or password");

            if (login.Password != user.PasswordHash)
                throw new UnauthorizedAccessException("Invalid username or password");

            return _tokenGenerator.GetToken(user.Id);
        }

    }
}
