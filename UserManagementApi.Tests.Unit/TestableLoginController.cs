using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagementApi.Contracts.Models;
using UserManagementApi.Controllers;
using UserManagementApi.Data;
using UserManagementApi.DTO;
using UserManagementApi.DTO.Auth;

namespace UserManagementApi.Tests.Unit
{
    public sealed class TestableLoginController :UsersController
    {
        private readonly string _jwtToReturn;
        private readonly DateTime _expToReturn;
        private readonly UserPermissionsDto _permToReturn;

        public TestableLoginController(AppDbContext db,
                                       IOptions<JwtOptions> jwtopts)
            : base(db, jwtopts)
        {
            _jwtToReturn = "fake-jwt";
            _expToReturn = DateTime.UtcNow.AddHours(1);
            _permToReturn = new UserPermissionsDto(0, "abc", new List<CategoryDto>());

        }
        protected override Task<UserPermissionsDto> BuildPermissionsForUser(int userId)
            => Task.FromResult(_permToReturn);

        protected override string GenerateJwt(AppUser user, List<CategoryDto> Categories, out DateTime expiresAtUtc)
        {
            expiresAtUtc = _expToReturn;
            return _jwtToReturn;
        }
    }
}
