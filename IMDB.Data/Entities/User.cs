using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class User
{
  
    public Guid UserId { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserBookmark> UserBookmarks { get; set; } = new List<UserBookmark>();

    public virtual ICollection<UserNote> UserNotes { get; set; } = new List<UserNote>();
}
