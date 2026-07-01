namespace StockManager.Core.Domain.Exceptions;

/// <summary>
/// Exception métier de base — mappée vers HTTP 400 Bad Request.
/// Portez un code machine (pour le frontend) et un message utilisateur.
/// </summary>
public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// Ressource introuvable — mappée vers HTTP 404 Not Found.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string code, string message) : base(code, message) { }

    /// <summary>Raccourci générique pour une entité non trouvée par ID.</summary>
    public static NotFoundException For(string entityName, object id)
        => new($"{entityName.ToUpper()}_NOT_FOUND", $"{entityName} avec l'identifiant {id} est introuvable.");
}

/// <summary>
/// Conflit de données — mappée vers HTTP 409 Conflict.
/// Ex: email déjà utilisé, doublon de numéro de série.
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string code, string message) : base(code, message) { }
}

/// <summary>
/// Accès interdit — mappée vers HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string code, string message) : base(code, message) { }
}
