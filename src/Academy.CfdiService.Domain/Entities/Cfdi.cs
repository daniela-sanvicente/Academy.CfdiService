namespace Academy.CfdiService.Domain.Entities;

public class Cfdi
{
    private Cfdi()
    {
    }

    public Cfdi(
        string issuerRfc,
        string receiverRfc,
        Guid uuid,
        DateTime issueDate,
        string status,
        DateTime lastUpdated)
    {
        IssuerRfc = issuerRfc;
        ReceiverRfc = receiverRfc;
        Uuid = uuid;
        IssueDate = issueDate;
        Status = status;
        LastUpdated = lastUpdated;
    }

    public int Id { get; private set; }

    public string IssuerRfc { get; private set; } = null!;

    public string ReceiverRfc { get; private set; } = null!;

    public Guid Uuid { get; private set; }

    public DateTime IssueDate { get; private set; }

    public string Status { get; private set; } = null!;

    public DateTime LastUpdated { get; private set; }
}
