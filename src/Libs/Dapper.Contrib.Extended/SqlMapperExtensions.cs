﻿using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Dapper.Contrib.Extended;
/// <summary>
/// The Dapper.Contrib extensions for Dapper
/// </summary>
public static partial class SqlMapperExtensions
{
    /// <summary>
    /// Defined a proxy object with a possibly dirty state.
    /// </summary>
    public interface IProxy //must be kept public
    {
        /// <summary>
        /// Whether the object has been changed.
        /// </summary>
        bool IsDirty { get; set; }
    }

    /// <summary>
    /// Defines a table name mapper for getting table names from types.
    /// </summary>
    public interface ITableNameMapper
    {
        /// <summary>
        /// Gets a table name from a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get a name from.</param>
        /// <returns>The table name for the given <paramref name="type"/>.</returns>
        string GetTableName(Type type);
    }

    /// <summary>
    /// The function to get a database type from the given <see cref="IDbConnection"/>.
    /// </summary>
    /// <param name="connection">The connection to get a database type name from.</param>
    public delegate string GetDatabaseTypeDelegate(IDbConnection connection);
    /// <summary>
    /// The function to get a table name from a given <see cref="Type"/>
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get a table name for.</param>
    public delegate string TableNameMapperDelegate(Type type);

    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ExplicitKeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

    private static readonly ISqlAdapter DefaultAdapter = new SqlServerAdapter();
    private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary
        = new Dictionary<string, ISqlAdapter>(6)
        {
            ["sqlconnection"] = new SqlServerAdapter(),
            ["sqlceconnection"] = new SqlCeServerAdapter(),
            ["npgsqlconnection"] = new PostgresAdapter(),
            ["sqliteconnection"] = new SQLiteAdapter(),
            ["mysqlconnection"] = new MySqlAdapter(),
            ["fbconnection"] = new FbAdapter()
        };

    private static List<PropertyInfo> ComputedPropertiesCache(Type type)
    {
        if (ComputedProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
        {
            return pi.ToList();
        }

        var computedProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ComputedAttribute)).ToList();

        ComputedProperties[type.TypeHandle] = computedProperties;
        return computedProperties;
    }

    private static List<PropertyInfo> ExplicitKeyPropertiesCache(Type type)
    {
        if (ExplicitKeyProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
        {
            return pi.ToList();
        }

        var explicitKeyProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ExplicitKeyAttribute)).ToList();

        ExplicitKeyProperties[type.TypeHandle] = explicitKeyProperties;
        return explicitKeyProperties;
    }

    private static List<PropertyInfo> KeyPropertiesCache(Type type)
    {
        if (KeyProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
        {
            return pi.ToList();
        }

        var allProperties = TypePropertiesCache(type);
        var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

        if (keyProperties.Count == 0)
        {
            var idProp = allProperties.Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
            if (idProp != null && !idProp.GetCustomAttributes(true).Any(a => a is ExplicitKeyAttribute))
            {
                keyProperties.Add(idProp);
            }
        }

        KeyProperties[type.TypeHandle] = keyProperties;
        return keyProperties;
    }

    private static List<PropertyInfo> TypePropertiesCache(Type type)
    {
        if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
        {
            return pis.ToList();
        }

        var properties = type.GetProperties().Where(IsWriteable).ToArray();
        TypeProperties[type.TypeHandle] = properties;
        return properties.ToList();
    }

    private static bool IsWriteable(PropertyInfo pi)
    {
        var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false).AsList();
        if (attributes.Count != 1) return true;

        var writeAttribute = (WriteAttribute)attributes[0];
        return writeAttribute.Write;
    }

    private static PropertyInfo GetSingleKey<T>(string method)
    {
        var type = typeof(T);
        var keys = KeyPropertiesCache(type);
        var explicitKeys = ExplicitKeyPropertiesCache(type);
        var keyCount = keys.Count + explicitKeys.Count;
        if (keyCount > 1)
            throw new DataException($"{method}<T> only supports an entity with a single [Key] or [ExplicitKey] property. [Key] Count: {keys.Count}, [ExplicitKey] Count: {explicitKeys.Count}");
        if (keyCount == 0)
            throw new DataException($"{method}<T> only supports an entity with a [Key] or an [ExplicitKey] property");

        return keys.Count > 0 ? keys[0] : explicitKeys[0];
    }

    /// <summary>
    /// Returns a single entity by a single id from table "Ts".  
    /// Id must be marked with [Key] attribute.
    /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
    /// for optimal performance. 
    /// </summary>
    /// <typeparam name="T">Interface or type to create and populate</typeparam>
    /// <param name="connection">Open SqlConnection</param>
    /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
    /// <param name="transaction">The transaction to run under, null (the default) if none</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
    /// <returns>Entity of T</returns>
    public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
    {
        var type = typeof(T);

        if (!GetQueries.TryGetValue(type.TypeHandle, out string sql))
        {
            var key = GetSingleKey<T>(nameof(Get));
            var name = GetTableName(type);

            sql = $"select * from {name} where {key.Name} = @id";
            GetQueries[type.TypeHandle] = sql;
        }

        var dynParams = new DynamicParameters();
        dynParams.Add("@id", id);

        T obj;

        if (type.IsInterface)
        {
            if (!(connection.Query(sql, dynParams).FirstOrDefault() is IDictionary<string, object> res))
            {
                return null;
            }

            obj = ProxyGenerator.GetInterfaceProxy<T>();

            foreach (var property in TypePropertiesCache(type))
            {
                var val = res[property.Name];
                if (val == null) continue;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var genericType = Nullable.GetUnderlyingType(property.PropertyType);
                    if (genericType != null) property.SetValue(obj, Convert.ChangeType(val, genericType), null);
                }
                else
                {
                    property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
                }
            }

            ((IProxy)obj).IsDirty = false;   //reset change tracking and return
        }
        else
        {
            obj = connection.Query<T>(sql, dynParams, transaction, commandTimeout: commandTimeout).FirstOrDefault();
        }
        return obj;
    }

    /// <summary>
    /// Returns a list of entities from table "Ts".
    /// Id of T must be marked with [Key] attribute.
    /// Entities created from interfaces are tracked/intercepted for changes and used by the Update() extension
    /// for optimal performance.
    /// </summary>
    /// <typeparam name="T">Interface or type to create and populate</typeparam>
    /// <param name="connection">Open SqlConnection</param>
    /// <param name="transaction">The transaction to run under, null (the default) if none</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
    /// <returns>Entity of T</returns>
    public static IEnumerable<T> GetAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
    {
        var type = typeof(T);
        var cacheType = typeof(List<T>);

        if (!GetQueries.TryGetValue(cacheType.TypeHandle, out string sql))
        {
            GetSingleKey<T>(nameof(GetAll));
            var name = GetTableName(type);

            sql = "select * from " + name;
            GetQueries[cacheType.TypeHandle] = sql;
        }

        if (!type.IsInterface) return connection.Query<T>(sql, null, transaction, commandTimeout: commandTimeout);

        var result = connection.Query(sql);
        var list = new List<T>();
        foreach (IDictionary<string, object> res in result)
        {
            var obj = ProxyGenerator.GetInterfaceProxy<T>();
            foreach (var property in TypePropertiesCache(type))
            {
                var val = res[property.Name];
                if (val == null) continue;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var genericType = Nullable.GetUnderlyingType(property.PropertyType);
                    if (genericType != null) property.SetValue(obj, Convert.ChangeType(val, genericType), null);
                }
                else
                {
                    property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
                }
            }
            ((IProxy)obj).IsDirty = false;   //reset change tracking and return
            list.Add(obj);
        }
        return list;
    }

    /// <summary>
    /// Specify a custom table name mapper based on the POCO type name
    /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible - I agree with you, but we can't do that until we break the API
    public static TableNameMapperDelegate TableNameMapper;
#pragma warning restore CA2211 // Non-constant fields should not be visible

    private static string GetTableName(Type type)
    {
        if (TypeTableName.TryGetValue(type.TypeHandle, out string name)) return name;

        if (TableNameMapper != null)
        {
            name = TableNameMapper(type);
        }
        else
        {
            //NOTE: This as dynamic trick falls back to handle both our own Table-attribute as well as the one in EntityFramework 
            var tableAttrName =
                type.GetCustomAttribute<TableAttribute>(false)?.Name
                ?? (type.GetCustomAttributes(false).FirstOrDefault(attr => attr.GetType().Name == "TableAttribute") as dynamic)?.Name;

            if (tableAttrName != null)
            {
                name = tableAttrName;
            }
            else
            {
                name = type.Name + "s";
                if (type.IsInterface && name.StartsWith("I"))
                    name = name.Substring(1);
            }
        }

        TypeTableName[type.TypeHandle] = name;
        return name;
    }

    /// <summary>
    /// Inserts an entity into table "Ts" and returns identity id or number of inserted rows if inserting a list.
    /// </summary>
    /// <typeparam name="T">The type to insert.</typeparam>
    /// <param name="connection">Open SqlConnection</param>
    /// <param name="entityToInsert">Entity to insert, can be list of entities</param>
    /// <param name="transaction">The transaction to run under, null (the default) if none</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
    /// <returns>Identity of inserted entity, or number of inserted rows if inserting a list</returns>
    public static long Insert<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
    {
        var isList = false;

        var type = typeof(T);

        if (type.IsArray)
        {
            isList = true;
            type = type.GetElementType();
        }
        else if (type.IsGenericType)
        {
            var typeInfo = type.GetTypeInfo();
            bool implementsGenericIEnumerableOrIsGenericIEnumerable =
                typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

            if (implementsGenericIEnumerableOrIsGenericIEnumerable)
            {
                isList = true;
                type = type.GetGenericArguments()[0];
            }
        }

        var name = GetTableName(type);
        var sbColumnList = new StringBuilder(null);
        var allProperties = TypePropertiesCache(type);
        var keyProperties = KeyPropertiesCache(type);
        var computedProperties = ComputedPropertiesCache(type);
        var allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

        var adapter = GetFormatter(connection);

        for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++)
        {
            var property = allPropertiesExceptKeyAndComputed[i];
            adapter.AppendColumnName(sbColumnList, property.Name);  //fix for issue #336
            if (i < allPropertiesExceptKeyAndComputed.Count - 1)
                sbColumnList.Append(", ");
        }

        var sbParameterList = new StringBuilder(null);
        for (var i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++)
        {
            var property = allPropertiesExceptKeyAndComputed[i];
            sbParameterList.AppendFormat("@{0}", property.Name);
            if (i < allPropertiesExceptKeyAndComputed.Count - 1)
                sbParameterList.Append(", ");
        }

        int returnVal;
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) connection.Open();

        if (!isList)    //single entity
        {
            returnVal = adapter.Insert(connection, transaction, commandTimeout, name, sbColumnList.ToString(),
                sbParameterList.ToString(), keyProperties, entityToInsert);
        }
        else
        {
            //insert list of entities
            var cmd = $"insert into {name} ({sbColumnList}) values ({sbParameterList})";
            returnVal = connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
        }
        if (wasClosed) connection.Close();
        return returnVal;
    }

    /// <summary>
    /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
    /// </summary>
    /// <typeparam name="T">Type to be updated</typeparam>
    /// <param name="connection">Open SqlConnection</param>
    /// <param name="entityToUpdate">Entity to be updated</param>
    /// <param name="transaction">The transaction to run under, null (the default) if none</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
    /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
    public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
    {
        if (entityToUpdate is IProxy proxy && !proxy.IsDirty)
        {
            return false;
        }

        var type = typeof(T);

        if (type.IsArray)
        {
            type = type.GetElementType();
        }
        else if (type.IsGenericType)
        {
            var typeInfo = type.GetTypeInfo();
            bool implementsGenericIEnumerableOrIsGenericIEnumerable =
                typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

            if (implementsGenericIEnumerableOrIsGenericIEnumerable)
            {
                type = type.GetGenericArguments()[0];
            }
        }

        var keyProperties = KeyPropertiesCache(type).ToList();  //added ToList() due to issue #418, must work on a list copy
        var explicitKeyProperties = ExplicitKeyPropertiesCache(type);
        if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0)
            throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

        var name = GetTableName(type);

        var sb = new StringBuilder();
        sb.AppendFormat("update {0} set ", name);

        var allProperties = TypePropertiesCache(type);
        keyProperties.AddRange(explicitKeyProperties);
        var computedProperties = ComputedPropertiesCache(type);
        var nonIdProps = allProperties.Except(keyProperties.Union(computedProperties)).ToList();

        var adapter = GetFormatter(connection);

        for (var i = 0; i < nonIdProps.Count; i++)
        {
            var property = nonIdProps[i];
            adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
            if (i < nonIdProps.Count - 1)
                sb.Append(", ");
        }
        sb.Append(" where ");
        for (var i = 0; i < keyProperties.Count; i++)
        {
            var property = keyProperties[i];
            adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
            if (i < keyProperties.Count - 1)
                sb.Append(" and ");
        }
        var updated = connection.Execute(sb.ToString(), entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
        return updated > 0;
    }

    /// <summary>
    /// Delete entity in table "Ts".
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <param name="connection">Open SqlConnection</param>
    /// <param name="entityToDelete">Entity to delete</param>
    /// <param name="transaction">The transaction to run under, null (the default) if none</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
    /// <returns>true if deleted, false if not found</returns>
    public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
    {
        if (entityToDelete == null)
            throw new ArgumentException("Cannot Delete null Object", nameof(entityToDelete));

        var type = typeof(T);

        if (type.IsArray)
        {
            type = type.GetElementType();
        }
        else if (type.IsGenericType)
        {
            var typeInfo = type.GetTypeInfo();
            bool implementsGenericIEnumerableOrIsGenericIEnumerable =
                typeInfo.ImplementedInterfaces.Any(ti => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) ||
                typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>);

            if (implementsGenericIEnumerableOrIsGenericIEnumerable)
            {
                type = type.GetGenericArguments()[0];
            }
        }

        var keyProperties = KeyPropertiesCache(type).ToList();  //added ToList() due to issue #418, must work on a list copy
        var explicitKeyProperties = ExplicitKeyPropertiesCache(type);
        if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0)
            throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");

        var name = GetTableName(type);
        keyProperties.AddRange(explicitKeyProperties);

        var sb = new StringBuilder();
        sb.AppendFormat("delete from {0} where ", name);

        var adapter = GetFormatter(connection);

        for (var i = 0; i < keyProperties.Count; i++)
        {
            var property = keyProperties[i];
            adapter.AppendColumnNameEqualsValue(sb, property.Name);  //fix for issue #336
            if (i < keyProperties.Count - 1)
                sb.Append(" and ");
        }
        var deleted = connection.Execute(sb.ToString(), entityToDelete, transaction, commandTimeout);
        return deleted > 0;
    }

    /// <summary>
    /// Delete all entities in the table related to the type T.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    /// <param name="connection">Open SqlConnection</param>
    /// <param name="transaction">The transaction to run under, null (the default) if none</param>
    /// <param name="commandTimeout">Number of seconds before command execution timeout</param>
    /// <returns>true if deleted, false if none found</returns>
    public static bool DeleteAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
    {
        var type = typeof(T);
        var name = GetTableName(type);
        var statement = $"delete from {name}";
        var deleted = connection.Execute(statement, null, transaction, commandTimeout);
        return deleted > 0;
    }

    /// <summary>
    /// Specifies a custom callback that detects the database type instead of relying on the default strategy (the name of the connection type object).
    /// Please note that this callback is global and will be used by all the calls that require a database specific adapter.
    /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible - I agree with you, but we can't do that until we break the API
    public static GetDatabaseTypeDelegate GetDatabaseType;
#pragma warning restore CA2211 // Non-constant fields should not be visible

    private static ISqlAdapter GetFormatter(IDbConnection connection)
    {
        var name = GetDatabaseType?.Invoke(connection).ToLower()
                   ?? connection.GetType().Name.ToLower();

        return AdapterDictionary.TryGetValue(name, out var adapter)
            ? adapter
            : DefaultAdapter;
    }

    private static class ProxyGenerator
    {
        private static readonly Dictionary<Type, Type> TypeCache = new Dictionary<Type, Type>();

        private static AssemblyBuilder GetAsmBuilder(string name)
        {
#if !NET461
            return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = name }, AssemblyBuilderAccess.Run);
#else
                return Thread.GetDomain().DefineDynamicAssembly(new AssemblyName { Name = name }, AssemblyBuilderAccess.Run);
#endif
        }

        public static T GetInterfaceProxy<T>()
        {
            Type typeOfT = typeof(T);

            if (TypeCache.TryGetValue(typeOfT, out Type k))
            {
                return (T)Activator.CreateInstance(k);
            }
            var assemblyBuilder = GetAsmBuilder(typeOfT.Name);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule("SqlMapperExtensions." + typeOfT.Name); //NOTE: to save, add "asdasd.dll" parameter

            var interfaceType = typeof(IProxy);
            var typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "_" + Guid.NewGuid(),
                TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(typeOfT);
            typeBuilder.AddInterfaceImplementation(interfaceType);

            //create our _isDirty field, which implements IProxy
            var setIsDirtyMethod = CreateIsDirtyProperty(typeBuilder);

            // Generate a field for each property, which implements the T
            foreach (var property in typeof(T).GetProperties())
            {
                var isId = property.GetCustomAttributes(true).Any(a => a is KeyAttribute);
                CreateProperty<T>(typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
            }

#if NETSTANDARD2_0
                var generatedType = typeBuilder.CreateTypeInfo().AsType();
#else
            var generatedType = typeBuilder.CreateType();
#endif

            TypeCache.Add(typeOfT, generatedType);
            return (T)Activator.CreateInstance(generatedType);
        }

        private static MethodInfo CreateIsDirtyProperty(TypeBuilder typeBuilder)
        {
            var propType = typeof(bool);
            var field = typeBuilder.DefineField("_" + nameof(IProxy.IsDirty), propType, FieldAttributes.Private);
            var property = typeBuilder.DefineProperty(nameof(IProxy.IsDirty),
                                           System.Reflection.PropertyAttributes.None,
                                           propType,
                                           new[] { propType });

            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.SpecialName
                                              | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

            // Define the "get" and "set" accessor methods
            var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + nameof(IProxy.IsDirty),
                                         getSetAttr,
                                         propType,
                                         Type.EmptyTypes);
            var currGetIl = currGetPropMthdBldr.GetILGenerator();
            currGetIl.Emit(OpCodes.Ldarg_0);
            currGetIl.Emit(OpCodes.Ldfld, field);
            currGetIl.Emit(OpCodes.Ret);
            var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + nameof(IProxy.IsDirty),
                                         getSetAttr,
                                         null,
                                         new[] { propType });
            var currSetIl = currSetPropMthdBldr.GetILGenerator();
            currSetIl.Emit(OpCodes.Ldarg_0);
            currSetIl.Emit(OpCodes.Ldarg_1);
            currSetIl.Emit(OpCodes.Stfld, field);
            currSetIl.Emit(OpCodes.Ret);

            property.SetGetMethod(currGetPropMthdBldr);
            property.SetSetMethod(currSetPropMthdBldr);
            var getMethod = typeof(IProxy).GetMethod("get_" + nameof(IProxy.IsDirty));
            var setMethod = typeof(IProxy).GetMethod("set_" + nameof(IProxy.IsDirty));
            typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
            typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);

            return currSetPropMthdBldr;
        }

        private static void CreateProperty<T>(TypeBuilder typeBuilder, string propertyName, Type propType, MethodInfo setIsDirtyMethod, bool isIdentity)
        {
            //Define the field and the property 
            var field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
            var property = typeBuilder.DefineProperty(propertyName,
                                           System.Reflection.PropertyAttributes.None,
                                           propType,
                                           new[] { propType });

            const MethodAttributes getSetAttr = MethodAttributes.Public
                                                | MethodAttributes.Virtual
                                                | MethodAttributes.HideBySig;

            // Define the "get" and "set" accessor methods
            var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                                         getSetAttr,
                                         propType,
                                         Type.EmptyTypes);

            var currGetIl = currGetPropMthdBldr.GetILGenerator();
            currGetIl.Emit(OpCodes.Ldarg_0);
            currGetIl.Emit(OpCodes.Ldfld, field);
            currGetIl.Emit(OpCodes.Ret);

            var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                                         getSetAttr,
                                         null,
                                         new[] { propType });

            //store value in private field and set the isdirty flag
            var currSetIl = currSetPropMthdBldr.GetILGenerator();
            currSetIl.Emit(OpCodes.Ldarg_0);
            currSetIl.Emit(OpCodes.Ldarg_1);
            currSetIl.Emit(OpCodes.Stfld, field);
            currSetIl.Emit(OpCodes.Ldarg_0);
            currSetIl.Emit(OpCodes.Ldc_I4_1);
            currSetIl.Emit(OpCodes.Call, setIsDirtyMethod);
            currSetIl.Emit(OpCodes.Ret);

            //TODO: Should copy all attributes defined by the interface?
            if (isIdentity)
            {
                var keyAttribute = typeof(KeyAttribute);
                var myConstructorInfo = keyAttribute.GetConstructor(Type.EmptyTypes);
                var attributeBuilder = new CustomAttributeBuilder(myConstructorInfo, Array.Empty<object>());
                property.SetCustomAttribute(attributeBuilder);
            }

            property.SetGetMethod(currGetPropMthdBldr);
            property.SetSetMethod(currSetPropMthdBldr);
            var getMethod = typeof(T).GetMethod("get_" + propertyName);
            var setMethod = typeof(T).GetMethod("set_" + propertyName);
            typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
            typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
        }
    }
}

/// <summary>
/// Defines the name of a table to use in Dapper.Contrib commands.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    /// <summary>
    /// Creates a table mapping to a specific name for Dapper.Contrib commands
    /// </summary>
    /// <param name="tableName">The name of this table in the database.</param>
    public TableAttribute(string tableName)
    {
        Name = tableName;
    }

    /// <summary>
    /// The name of the table in the database
    /// </summary>
    public string Name { get; set; }
}

/// <summary>
/// Specifies that this field is a primary key in the database
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class KeyAttribute : Attribute
{
}

/// <summary>
/// Specifies that this field is an explicitly set primary key in the database
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExplicitKeyAttribute : Attribute
{
}

/// <summary>
/// Specifies whether a field is writable in the database.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class WriteAttribute : Attribute
{
    /// <summary>
    /// Specifies whether a field is writable in the database.
    /// </summary>
    /// <param name="write">Whether a field is writable in the database.</param>
    public WriteAttribute(bool write)
    {
        Write = write;
    }

    /// <summary>
    /// Whether a field is writable in the database.
    /// </summary>
    public bool Write { get; }
}

/// <summary>
/// Specifies that this is a computed column.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ComputedAttribute : Attribute
{
}
