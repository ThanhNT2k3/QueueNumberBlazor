namespace QMS.Web.Models;

/// <summary>
/// SAML2 configuration settings
/// </summary>
public class Saml2Settings
{
    /// <summary>
    /// Service Provider (SP) Entity ID - Your application's identifier
    /// </summary>
    public string ServiceProviderEntityId { get; set; } = string.Empty;

    /// <summary>
    /// Identity Provider (IdP) Entity ID
    /// </summary>
    public string IdentityProviderEntityId { get; set; } = string.Empty;

    /// <summary>
    /// Identity Provider Single Sign-On URL
    /// </summary>
    public string IdentityProviderSingleSignOnServiceUrl { get; set; } = string.Empty;

    /// <summary>
    /// Identity Provider Single Logout URL (optional)
    /// </summary>
    public string? IdentityProviderSingleLogoutServiceUrl { get; set; }

    /// <summary>
    /// Return URL after successful authentication
    /// </summary>
    public string ReturnUrl { get; set; } = "/";

    /// <summary>
    /// Whether to sign authentication requests
    /// </summary>
    public bool SignAuthenticationRequests { get; set; } = false;

    /// <summary>
    /// Attribute mapping for user claims
    /// </summary>
    public Saml2AttributeMapping AttributeMapping { get; set; } = new();

    /// <summary>
    /// Enable/disable SAML2 authentication
    /// </summary>
    public bool Enabled { get; set; } = false;
}

/// <summary>
/// Maps SAML attributes to application claims
/// </summary>
public class Saml2AttributeMapping
{
    /// <summary>
    /// SAML attribute name for user ID
    /// Default: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
    /// </summary>
    public string UserIdAttribute { get; set; } = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

    /// <summary>
    /// SAML attribute name for email
    /// Default: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress
    /// </summary>
    public string EmailAttribute { get; set; } = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

    /// <summary>
    /// SAML attribute name for display name
    /// Default: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name
    /// </summary>
    public string DisplayNameAttribute { get; set; } = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

    /// <summary>
    /// SAML attribute name for first name
    /// </summary>
    public string? FirstNameAttribute { get; set; } = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";

    /// <summary>
    /// SAML attribute name for last name
    /// </summary>
    public string? LastNameAttribute { get; set; } = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";

    /// <summary>
    /// SAML attribute name for roles/groups
    /// </summary>
    public string? RolesAttribute { get; set; } = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
}
