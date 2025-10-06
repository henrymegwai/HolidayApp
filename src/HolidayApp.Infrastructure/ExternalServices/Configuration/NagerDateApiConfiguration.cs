using System.ComponentModel.DataAnnotations;

namespace HolidayApp.Infrastructure.ExternalServices.Configuration;

public class NagerDateApiConfiguration
{
    [Required(ErrorMessage = "ServiceUrl is required")]
    [Url(ErrorMessage = "ServiceUrl must be a valid URL")]
    public string ServiceUrl { get; set; } = null!;

    [Required(ErrorMessage = "ConnectionTimeoutInSeconds is required")]
    [Range(1, 300, ErrorMessage = "ConnectionTimeoutInSeconds must be between 1 and 300")]
    public string ConnectionTimeoutInSeconds { get; set; } = null!;
}