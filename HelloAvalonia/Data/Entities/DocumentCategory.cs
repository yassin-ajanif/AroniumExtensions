using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class DocumentCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LanguageKey { get; set; }

    public virtual ICollection<DocumentType> DocumentTypes { get; set; } = new List<DocumentType>();
}
