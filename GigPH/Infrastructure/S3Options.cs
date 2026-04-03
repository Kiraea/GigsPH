namespace GigPH.Infrastructure;

public class S3Options
{
    public string BucketName { get; set; }
    public string ServiceUrl { get; set; } // localhost or whatever
    public string AccessKey { get; set; } // identifier think username
    public string Region { get; set;  } 
    public string SecretKey { get; set; } // think password
}