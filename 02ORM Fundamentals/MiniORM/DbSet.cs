namespace MiniORM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class DbSet<TEntity> : ICollection<TEntity>
        where TEntity : class, new()
    {
        internal DbSet(IEnumerable<TEntity> entities)
        {
            Entities = entities.ToList();
            ChangeTracker = new ChangeTracker<TEntity>(entities);
        }

        internal ICollection<TEntity> Entities { get; set; }

        internal ChangeTracker<TEntity> ChangeTracker { get; set; }

        public int Count => Entities.Count;

        public bool IsReadOnly => Entities.IsReadOnly;

        public void Add(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null!");
            }

            Entities.Add(item);

            ChangeTracker.Add(item);
        }

        public void Clear()
        {
            while (Entities.Any())
            {
                var current = Entities.First();

                Remove(current);
            }
        }

        public bool Contains(TEntity item) => Entities.Contains(item);

        public void CopyTo(TEntity[] array, int arrayIndex) => Entities.CopyTo(array, arrayIndex);

        public bool Remove(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null!");
            }

            var removed = Entities.Remove(item);

            if (removed)
            {
                ChangeTracker.Remove(item);
            }

            return removed;
        }

        public IEnumerator<TEntity> GetEnumerator() => Entities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                Remove(entity);
            }
        }
    }
}