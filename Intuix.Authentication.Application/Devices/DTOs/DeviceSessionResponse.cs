namespace Intuix.Authentication.Application.Devices.DTOs;

public class DeviceSessionResponse
{
    public Guid TokenId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUsedAt { get; set; }
    public bool IsCurrent { get; set; }
}
