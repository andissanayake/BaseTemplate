﻿namespace BaseTemplate.Application.Common.Security;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuthorizeAttribute : Attribute
{
    public AuthorizeAttribute() { }
    public string Roles { get; set; } = string.Empty;
}
