using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

// O namespace para ambas as classes será o do seu arquivo.
namespace RhSensoERP.Infrastructure.Auth
{
    // 1. O HANDLER COM A LÓGICA CORRIGIDA
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userPermissions = context.User.FindAll(c => c.Type == "perm").Select(c => c.Value).ToList();

            if (!userPermissions.Any())
            {
                return Task.CompletedTask;
            }

            var requiredPermissionParts = requirement.Permission.Split('.');
            if (requiredPermissionParts.Length < 2)
            {
                return Task.CompletedTask;
            }

            var requiredPrefix = string.Join(".", requiredPermissionParts.Take(requiredPermissionParts.Length - 1));
            var requiredOperation = requiredPermissionParts.Last();

            foreach (var userPermission in userPermissions)
            {
                var userPermissionParts = userPermission.Split('.');
                if (userPermissionParts.Length < 2)
                {
                    continue;
                }

                var userPrefix = string.Join(".", userPermissionParts.Take(userPermissionParts.Length - 1));
                var userAllowedOperations = userPermissionParts.Last();

                if (userPrefix.Equals(requiredPrefix, StringComparison.OrdinalIgnoreCase) &&
                    userAllowedOperations.Contains(requiredOperation, StringComparison.OrdinalIgnoreCase))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }

    // 2. A DEFINIÇÃO DA REQUIREMENT NO MESMO ARQUIVO
    //    Isso resolve o erro CS0246 "PermissionRequirement não pode ser encontrado".
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission;
    }
}

