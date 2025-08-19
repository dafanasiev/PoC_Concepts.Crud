

// ReSharper disable once CheckNamespace
namespace Swashbuckle.AspNetCore.Annotations;

#pragma warning disable CS9113 // Parameter is unread.
public class SwaggerResponseAttribute(int statusCode, Type type, string description) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
