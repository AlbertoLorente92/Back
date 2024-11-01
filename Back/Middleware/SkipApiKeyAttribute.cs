namespace Back.Middleware
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipApiKeyAttribute : Attribute
    {
    }
}
