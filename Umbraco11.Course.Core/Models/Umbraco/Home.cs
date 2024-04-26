namespace Umbraco11.Course.Core.Models.Umbraco;

public partial class Home
{
    public string ShortHeroDescription
    {
        get
        {
            if (string.IsNullOrEmpty(this.HeroDescription))
            {
                return string.Empty;
            }
            return $"{this.HeroDescription.Substring(0,30)} ...";
        }
    }
}
