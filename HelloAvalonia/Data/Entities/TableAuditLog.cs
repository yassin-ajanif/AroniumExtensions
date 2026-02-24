namespace AroniumFactures.Data.Entities;

public class TableAuditLog
{
    public int Id { get; set; }
    public string TableName { get; set; } = null!;
    public string Operation { get; set; } = null!;
    public string SqlStatement { get; set; } = null!;
    public string? CreatedAt { get; set; }
}
