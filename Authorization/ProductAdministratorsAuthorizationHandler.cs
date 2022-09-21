using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using ProductCategories.Models;
using ProductCategories.Authorization;

namespace ProductCategories.Authorization
{
    public class ProductAdministratorsAuthorizationHandler
                  : AuthorizationHandler<OperationAuthorizationRequirement, ProductCategories.Models.Product>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       ProductCategories.Models.Product resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            // Administrators can do anything.
            if (context.User.IsInRole(Constants.AdministratorsRole))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
