using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class WordIndex
{
    public long WordId { get; set; }

    public string? Word { get; set; }

    public virtual ICollection<WordIndexTitle> WordIndexTitles { get; set; } = new List<WordIndexTitle>();
}
