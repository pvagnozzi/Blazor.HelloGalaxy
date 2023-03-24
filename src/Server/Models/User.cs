using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blazor.HelloGalaxy.Server.Models;

public class User : Entity
{
    public User() : this(string.Empty, string.Empty)
    {

    }

    public User(string username, string password, string? firstName = null, string? lastName = null)
    {
        UserName = username;
        Password = password;
        FirstName = firstName ?? username;
        LastName = lastName ?? username;
    }

    [Required]
    [MaxLength(32)]
    [DisplayName("UserName")]
    public string UserName { get; protected internal set; }

    [Required]
    [MaxLength(32)]
    [DisplayName("Password")]
    public string Password { get; protected internal set; }

    [Required]
    [MaxLength(32)]
    [DisplayName("First Name")]
    public string FirstName { get; protected internal set; }

    [Required]
    [MaxLength(32)]
    [DisplayName("Last Name")]
    public string LastName { get; protected internal set; }
}

