namespace Tariffs
{
    public class Option<T> where T : class
    {
        public Option()
        {
            HasValue = false;
            Value = null;
        }
    
        public Option(T value)
        {
            HasValue = true;
            Value = value;
        }
    
        public T Value { get; }
    
        public bool HasValue { get; }
    
        public override bool Equals(object obj) =>
            obj != null && obj is Option<T> other && other.Value == Value;
    
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
    
        public static bool operator ==(Option<T> left, Option<T> right) =>
            left?.Value == right?.Value;
    
        public static bool operator !=(Option<T> left, Option<T> right) =>
            left?.Value != right?.Value;
    
        public static readonly Option<T> None = new Option<T>();
    }
}
