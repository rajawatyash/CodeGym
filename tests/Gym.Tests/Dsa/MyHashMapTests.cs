using Gym.Dsa;

namespace Gym.Tests.Dsa;

public class MyHashMapTests
{
    // ---- Q2's examples, one test each ----

    [Fact]
    public void Get_on_empty_map_returns_minus_one()
    {
        var map = new MyHashMap();

        Assert.Equal(-1, map.Get(5));
    }

    [Fact]
    public void Put_then_get_returns_the_value()
    {
        var map = new MyHashMap();

        map.Put(1, 10);

        Assert.Equal(10, map.Get(1));
        Assert.Equal(1, map.Count);
    }

    [Fact]
    public void Put_on_existing_key_replaces_the_value()
    {
        var map = new MyHashMap();

        map.Put(2, 20);
        map.Put(2, 99);

        Assert.Equal(99, map.Get(2));
        Assert.Equal(1, map.Count);          // replaced, not added twice
    }

    [Fact]
    public void Remove_then_get_returns_minus_one()
    {
        var map = new MyHashMap();
        map.Put(1, 10);

        map.Remove(1);

        Assert.Equal(-1, map.Get(1));
        Assert.Equal(0, map.Count);
    }

    [Fact]
    public void Remove_of_missing_key_does_nothing()
    {
        var map = new MyHashMap();
        map.Put(1, 10);

        map.Remove(99);

        Assert.Equal(10, map.Get(1));
        Assert.Equal(1, map.Count);
    }

    // ---- collisions: 1, 17 and 33 all land in slot 1 when there are 16 slots ----

    [Fact]
    public void Colliding_keys_are_kept_apart()
    {
        var map = new MyHashMap();

        map.Put(1, 10);
        map.Put(17, 170);
        map.Put(33, 330);

        Assert.Equal(10, map.Get(1));
        Assert.Equal(170, map.Get(17));
        Assert.Equal(330, map.Get(33));
    }

    [Fact]
    public void Removing_one_colliding_key_keeps_the_others()
    {
        var map = new MyHashMap();
        map.Put(1, 10);
        map.Put(17, 170);
        map.Put(33, 330);

        map.Remove(17);

        Assert.Equal(10, map.Get(1));
        Assert.Equal(-1, map.Get(17));
        Assert.Equal(330, map.Get(33));
        Assert.Equal(2, map.Count);
    }

    [Fact]
    public void Negative_keys_work()
    {
        var map = new MyHashMap();

        map.Put(-3, 30);

        Assert.Equal(30, map.Get(-3));
    }

    // ---- resizing ----

    [Fact]
    public void Does_not_resize_until_more_than_three_quarters_full()
    {
        var map = new MyHashMap();

        for (int k = 0; k < 12; k++) map.Put(k, k);
        Assert.Equal(16, map.BucketCount);   // 12 / 16 = 0.75, not above

        map.Put(12, 12);
        Assert.Equal(32, map.BucketCount);   // 13 / 16 > 0.75, so it doubled
    }

    [Fact]
    public void Keeps_every_value_after_many_resizes()
    {
        var map = new MyHashMap();

        for (int k = 0; k < 1000; k++) map.Put(k, k * 2);

        for (int k = 0; k < 1000; k++)
            Assert.Equal(k * 2, map.Get(k));
        Assert.Equal(-1, map.Get(1000));
        Assert.Equal(1000, map.Count);
        Assert.Equal(2048, map.BucketCount);
    }

    // ---- the senior trick: compare against the real Dictionary ----

    [Fact]
    public void Behaves_like_Dictionary_for_random_operations()
    {
        var mine = new MyHashMap();
        var real = new Dictionary<int, int>();
        var rng = new Random(42);            // fixed seed, so a failure is repeatable

        for (int step = 0; step < 20_000; step++)
        {
            int key = rng.Next(-500, 500);   // a small range forces lots of collisions and updates
            switch (rng.Next(3))
            {
                case 0:
                    int value = rng.Next();
                    mine.Put(key, value);
                    real[key] = value;
                    break;
                case 1:
                    mine.Remove(key);
                    real.Remove(key);
                    break;
                case 2:
                    int expected = real.TryGetValue(key, out int v) ? v : -1;
                    Assert.Equal(expected, mine.Get(key));
                    break;
            }
        }

        Assert.Equal(real.Count, mine.Count);
    }
}