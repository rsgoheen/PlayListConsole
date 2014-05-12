using System;

namespace PlayList.Util
{
    public  class CriteriaCounter : ICriteriaCounter<Song>
    {
        private int _count;

        public CriteriaCounter(Predicate<Song> criteria, int count)
        {
            Criteria = criteria;
            _count = count;
        }
        
        public Predicate<Song> Criteria { get; private set; }

        public void Decrement()
        {
            _count--;
        }

       public void Deactivate()
       {
          _count = 0;
       }

       public bool IsActive { get { return _count > 0; } }
    }
}