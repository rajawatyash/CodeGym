namespace Gym.Dsa
{
    public class MyHashMap
    {
        // The array. Each slot is a list of (key, value) pairs, or null while it's empty.
        private List<(int Key, int Value)>?[] _buckets = new List<(int Key, int Value)>?[16];
        private int _count;   // how many pairs in total, needed for resizing

        // "key % size tells you which list" (& 0x7FFFFFFF keeps negative keys valid)
        private static int Index(int key, int size) => (key & 0x7FFFFFFF) % size;
        public int Count => _count;
        public int BucketCount => _buckets.Length;

        public int Get(int key)
        {
            var list = _buckets[Index(key, _buckets.Length)];
            if (list is null) return -1;

            foreach (var (k, v) in list)
                if (k == key)          // for a generic TKey this is EqualityComparer<TKey>.Default.Equals(k, key)
                    return v;

            return -1;
        }

        public void Put(int key, int value)
        {
            int i = Index(key, _buckets.Length);          // 1. which slot?          11 -> slot 11
            _buckets[i] ??= new List<(int Key, int Value)>(); // 2. empty slot? give it a list
            var list = _buckets[i];

            for (int j = 0; j < list?.Count; j++)          // 3. walk the pairs in that slot
            {
                if (list[j].Key == key)                   //    key already here?
                {
                    list[j] = (key, value);               //    yes: replace the value, done
                    return;
                }
            }

            list.Add((key, value));                       // 4. not found: add a new pair
            _count++;

            if (_count > _buckets.Length * 3 / 4)         // 5. too full? (16 * 3 / 4 = 12)
                Resize();
        }

        public void Remove(int key)
        {
            // 1. which slot? If it's null, return.
            int i = Index(key, _buckets.Length);
            var list = _buckets[i];
            if (list is null) return;            

            for (int j = 0; j < list.Count; j++)          // 3. walk the pairs in that slot
            {
                if (list[j].Key == key)                   //    key already here?
                {
                    list.RemoveAt(j);                   //    yes: remove
                    _count--;
                    return;
                }
            }
        }

        private void Resize()
        {
            var bigger = new List<(int Key, int Value)>?[_buckets.Length * 2];

            foreach(var bucket in _buckets)
            {
                if(bucket is null) continue;
                foreach(var pair in bucket)
                {
                    int i = Index(pair.Key, bigger.Length);
                    bigger[i] ??= new List<(int Key, int Value)>();

                    bigger[i]?.Add(pair);
                }
                
            }
            _buckets = bigger;
        }

    }
}