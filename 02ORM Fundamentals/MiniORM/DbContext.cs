using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    public abstract class DbContext
    {
        private readonly DatabaseConnection connection;
        private readonly Dictionary<Type, PropertyInfo> dbSetProperties;

        internal static Type[] AllowedSqlTypes =
        {
            typeof(bool),
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(double),
            typeof(decimal),
            typeof(DateTime)
        };

        protected DbContext(string connectionString)
        {
            connection = new DatabaseConnection(connectionString);
            dbSetProperties = DiscoverDbSets();

            using (new ConnectionManager(connection))
            {
                InitializeDbSets();
            }

            MapAllRelations();
        }

        public void SaveChanges()
        {
            var dbSets = dbSetProperties
                .Select(pi => pi.Value.GetValue(this))
                .ToArray();

            foreach (IEnumerable<object> dbSet in dbSets)
            {
                var invalidEntities = dbSet
                    .Where(x => !IsObjectValid(x))
                    .ToArray();

                if (invalidEntities.Any())
                {
                    throw new InvalidOperationException($"{invalidEntities.Length} Invalid Entities found in {dbSet.GetType().Name}!");
                }

                using (new ConnectionManager(connection))
                {
                    using (var transaction = connection.StartTransaction())
                    {
                        foreach (var set in dbSets)
                        {
                            var dbSetType = set.GetType().GetGenericArguments().First();

                            var persistMethod = typeof(DbContext)
                                .GetMethod("Persist", BindingFlags.NonPublic | BindingFlags.Instance)
                                .MakeGenericMethod(dbSetType);

                            try
                            {
                                persistMethod.Invoke(this, new object[] { dbSet });
                            }
                            catch (TargetInvocationException tie)
                            {
                                throw tie.InnerException;
                            }
                            catch (InvalidOperationException)
                            {
                                transaction.Rollback();
                                throw;
                            }
                            catch (SqlException)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        private void Persist<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            var tableName = GetTableName(typeof(TEntity));
            var columns = this.connection.FetchColumnNames(tableName).ToArray();

            if (dbSet.ChangeTracker.Added.Any())
            {
                connection.InsertEntities(dbSet.ChangeTracker.Added, tableName, columns);
            }

            if (dbSet.ChangeTracker.Removed.Any())
            {
                connection.DeleteEntities(dbSet.ChangeTracker.Removed, tableName, columns);
            }

            var modifiedEntities = dbSet.ChangeTracker.GetModifiedEntities(dbSet).ToArray();

            if (modifiedEntities.Any())
            {
                connection.UpdateEntities(modifiedEntities, tableName, columns);
            }
        }

        private void InitializeDbSets()
        {
            foreach (var dbSetProperty in dbSetProperties)
            {
                var dbSetType = dbSetProperty.Key;
                var dbSetPropertyType = dbSetProperty.Value;

                var populateDbSetGeneric = typeof(DbContext)
                    .GetMethod("PopulateDbSet", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                populateDbSetGeneric.Invoke(this, new object[] { dbSetPropertyType });

            }
        }

        private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
            where TEntity : class, new()
        {
            var entities = LoadTableEntities<TEntity>();

            var dbSetInstance = new DbSet<TEntity>(entities);

            ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstance);
        }

        private void MapAllRelations()
        {
            foreach (var dbSetProperty in this.dbSetProperties)
            {
                var dbSetType = dbSetProperty.Key;

                var mapRelations = typeof(DbContext)
                    .GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(dbSetType);

                var dbSet = dbSetProperty.Value.GetValue(this);

                mapRelations.Invoke(this, new object[] { dbSet });
            }
        }

        private void MapRelations<TEntity>(DbSet<TEntity> dbSet)
            where TEntity : class, new()
        {
            var entityType = typeof(TEntity);

            MapNavigationProperties(dbSet);

            var collections = entityType
                .GetProperties()
                .Where(pi =>
                    pi.GetType().IsGenericType &&
                    pi.GetType().GetGenericTypeDefinition() == typeof(ICollection<>))
                .ToArray();

            foreach (var collection in collections)
            {
                var collectionType = collection.PropertyType.GenericTypeArguments.First();
                var mapCollectionMethod = typeof(DbContext)
                    .GetMethod("MapCollection", BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(entityType, collectionType);

                mapCollectionMethod.Invoke(this, new object[] { dbSet, collectionType });
            }
        }

        private void MapCollection<TDbSet, TCollection>(DbSet<TDbSet> dbSet, PropertyInfo collectionProperty)
            where TDbSet : class, new() where TCollection : class, new()
        {
            var entityType = typeof(TDbSet);

            var collectionType = typeof(TCollection);

            var primaryKeys = collectionType.GetProperties()
                .Where(pi => pi.HasAttribute<KeyAttribute>())
                .ToArray();

            var primaryKey = primaryKeys.First();

            var foreignKey = entityType.GetProperties()
                .First(pi => pi.HasAttribute<KeyAttribute>());

            bool isManyToMany = primaryKeys.Length > 1;

            if (isManyToMany)
            {
                primaryKey = collectionType.GetProperties()
                    .First(pi => collectionType
                                     .GetProperty(pi.GetCustomAttribute<ForeignKeyAttribute>().Name)
                                     .PropertyType == entityType);
            }

            var navigationDbSet = (DbSet<TCollection>)this.dbSetProperties[collectionType].GetValue(this);

            foreach (var entity in dbSet)
            {
                var primaryKeyValue = foreignKey.GetValue(entity);

                var navigationEntities = navigationDbSet
                    .Where(navigationEntity => primaryKey.GetValue(navigationEntity).Equals(primaryKeyValue))
                    .ToArray();

                ReflectionHelper.ReplaceBackingField(this, collectionProperty.Name, navigationEntities);
            }
        }

        private void MapNavigationProperties<TEntity>(DbSet<TEntity> dbSet) where TEntity : class, new()
        {
            var entityType = typeof(TEntity);

            var foreignKeys = entityType
                .GetProperties()
                .Where(pi => pi.HasAttribute<ForeignKeyAttribute>())
                .ToArray();

            foreach (var foreignKey in foreignKeys)
            {
                var navigationPropertyName = foreignKey
                    .GetCustomAttribute<ForeignKeyAttribute>().Name;

                var navigationProperty = entityType.GetProperty(navigationPropertyName);

                var navigationDbSet = this.dbSetProperties[navigationProperty.PropertyType].GetValue(this);

                var navigationPrimaryKey = navigationProperty.PropertyType.GetProperties()
                    .First(x => x.HasAttribute<KeyAttribute>());

                foreach (var entity in dbSet)
                {
                    var foreignKeyValue = foreignKey.GetValue(entity);

                    var navigationPropertyValue = ((IEnumerable<object>)navigationDbSet)
                        .First(navProperty => navigationPrimaryKey.GetValue(navProperty).Equals(foreignKeyValue));

                    navigationProperty.SetValue(entity, navigationPropertyValue);
                }
            }
        }

        private bool IsObjectValid(object x)
        {
            var validationContext = new ValidationContext(x);

            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(x, validationContext, validationResult, true);
        }

        private IEnumerable<TEntity> LoadTableEntities<TEntity>()
             where TEntity : class
        {
            var table = typeof(TEntity);

            var columns = GetColumnNames(table);

            var tableName = GetTableName(table);

            return connection.FetchResultSet<TEntity>(tableName, columns);
        }

        private string GetTableName(Type type)
        {
            var tableName = ((TableAttribute)Attribute.GetCustomAttribute(type, typeof(TableAttribute)))?.Name;

            if (tableName == null)
            {
                tableName = this.dbSetProperties[type].Name;
            }

            return tableName;
        }

        private Dictionary<Type, PropertyInfo> DiscoverDbSets()
        {
            return GetType()
                  .GetProperties()
                  .Where(pi => pi.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                  .ToDictionary(k => k.PropertyType.GetGenericArguments().First(), v => v);
        }

        private string[] GetColumnNames(Type table)
        {
            var tableName = GetTableName(table);

            var dbColumns = connection.FetchColumnNames(tableName);

            return table.GetProperties()
                        .Where(pi => dbColumns.Contains(pi.Name) &&
                              !pi.HasAttribute<NotMappedAttribute>() &&
                               AllowedSqlTypes.Contains(pi.PropertyType))
                        .Select(x => x.Name)
                        .ToArray();
        }
    }
}