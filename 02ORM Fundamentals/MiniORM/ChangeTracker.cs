﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
	internal class ChangeTracker<T>
		where T: class, new()
	{
		private readonly List<T> allEntities;

		private readonly List<T> added;

		private readonly List<T> removed;

		public ChangeTracker(IEnumerable<T> entities)
        {
			added = new List<T>();
			removed = new List<T>();
			allEntities = CloneEntities(entities);
        }

		public IReadOnlyCollection<T> AllEntities => allEntities.AsReadOnly();

		public IReadOnlyCollection<T> Added => added.AsReadOnly();

		public IReadOnlyCollection<T> Removed => removed.AsReadOnly();

		public void Add(T item) => added.Add(item);

		public void Remove(T item) => removed.Add(item);

		public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
			var modifiedEntities = new List<T>();

			var primaryKeys = typeof(T)
				.GetProperties()
				.Where(pi => pi.HasAttribute<KeyAttribute>())
				.ToArray();

			foreach (var proxyEntity in AllEntities)
			{
				var primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity).ToArray();

				var entity = dbSet.Entities
					.Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

				var isModified = IsModified(proxyEntity, entity);

				if (isModified)
				{
					modifiedEntities.Add(entity);
				}

			}

			return modifiedEntities;
		}

        private static bool IsModified(T proxyEntity, T entity)
        {
			var monitoredProperties = typeof(T)
				.GetProperties()
				.Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType));

			var modifiedProperties = monitoredProperties
				.Where(pi => !Equals(pi.GetValue(entity), pi.GetValue(proxyEntity)))
				.ToArray();

			var isModified = modifiedProperties.Any();

			return isModified;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(PropertyInfo[] primaryKeys, T proxyEntity)
        {
			return primaryKeys.Select(pk => pk.GetValue(proxyEntity));
        }

        private static List<T> CloneEntities(IEnumerable<T> entities)
        {
			var clonedEntities = new List<T>();

			var propertiesToClone = typeof(T).GetProperties()
				.Where(pi => DbContext.AllowedSqlTypes.Contains(pi.PropertyType))
				.ToArray();

            foreach (var entity in entities)
            {
				var clonedEntitiy = Activator.CreateInstance<T>();

				foreach (var property in propertiesToClone)
				{
					var value = property.GetValue(entity);
					property.SetValue(clonedEntitiy, value);
				}

				clonedEntities.Add(clonedEntitiy);

			}

			return clonedEntities;
        }
	}
}