using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
namespace PersonalWealth.Api.Infrastructure;
public sealed class DevelopmentAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,ILoggerFactory logger,UrlEncoder encoder):AuthenticationHandler<AuthenticationSchemeOptions>(options,logger,encoder)
{
 protected override Task<AuthenticateResult> HandleAuthenticateAsync(){var tenant=Request.Headers["X-Tenant-Id"].FirstOrDefault();var subject=Request.Headers["X-Dev-User"].FirstOrDefault();if(!Guid.TryParse(tenant,out var id)||id==Guid.Empty||string.IsNullOrWhiteSpace(subject))return Task.FromResult(AuthenticateResult.Fail("Development authentication requires X-Tenant-Id and X-Dev-User."));var claims=new[]{new Claim(ClaimTypes.NameIdentifier,subject),new Claim("tenant_id",id.ToString()),new Claim(ClaimTypes.Role,"User")};var identity=new ClaimsIdentity(claims,Scheme.Name);return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity),Scheme.Name)));}
}
