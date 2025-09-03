namespace LinkDev.Talabat.Core.Domain.Common
{
    public class BaseEntity <TKey>  where TKey : IEquatable<TKey>
    {
        public int Id { get; set; }
    }
}
