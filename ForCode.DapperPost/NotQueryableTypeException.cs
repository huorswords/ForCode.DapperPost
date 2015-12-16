namespace ForCode.DapperPost
{
    using System;

    [Serializable]
    public class NotQueryableTypeException : Exception
    {
        public NotQueryableTypeException() { }
        public NotQueryableTypeException(string message) : base(message) { }
        public NotQueryableTypeException(string message, Exception inner) : base(message, inner) { }
        protected NotQueryableTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
