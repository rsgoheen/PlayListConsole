using System;

namespace PlayList.Util
{
    public interface ICriteriaCounter<T>
    {
        Predicate<T> Criteria { get; }
        void Decrement();
        bool IsActive { get; }
    }
}
