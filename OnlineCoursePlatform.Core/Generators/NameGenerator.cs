namespace OnlineCoursePlatform.Core.Generators
{
    public class NameGenerator
    {
        public static string GenerateUniqCode() => Guid.NewGuid().ToString().Replace("-", "");
    }
}
