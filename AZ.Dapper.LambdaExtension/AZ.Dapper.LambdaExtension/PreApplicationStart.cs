﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Text;
using System.Threading.Tasks;
//using System.Web;
using Dapper.LambdaExtension.LambdaSqlBuilder.Attributes;

using Dapper;

#if NETCOREAPP1_0
using System.Reflection.Metadata;
using Microsoft.Extensions.DependencyModel;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata.Ecma335;
#endif

//[assembly: PreApplicationStartMethod(typeof(Dapper.LambdaExtension.PreApplicationStart), "RegisterTypeMaps")]

namespace Dapper.LambdaExtension
{

    public static class PreApplicationStart
    {
        static Func<Type, string, PropertyInfo> _fu = (type, columnName) => type.GetProperties().FirstOrDefault(prop => GetColumnAttribute(prop) == columnName);
        private static bool _initialized = false;

        public static void RegisterTypeMaps()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            var aliasType = typeof(DBColumnAttribute);

            var mappedTypeList = new List<Type>();

#if NETCOREAPP1_0
            var libList = DependencyContext.Default.CompileLibraries.ToList();


            foreach (var library in libList)
            {
                foreach (var libraryAssembly in library.Assemblies)
                {
                    var assembly = Assembly.Load(new AssemblyName(libraryAssembly));
                    var types = assembly.GetExportedTypes().ToList();
                    var mappedtypes = types.Where(f =>
                 f.GetProperties().Any(
                     p =>
                     p.GetCustomAttributes(false).Any(
                         a => a.GetType().Name == aliasType.Name)));

                    mappedTypeList.AddRange(mappedtypes);
                }

            }

            var runLiblist = DependencyContext.Default.RuntimeLibraries.ToList();


            var tempTypes = Assembly.GetEntryAssembly().GetExportedTypes();
            var mappedtypesList = tempTypes.Where(f =>
          f.GetProperties().Any(
              p =>
              p.GetCustomAttributes(false).Any(
                  a => a.GetType().Name == aliasType.Name))).ToList();

            mappedTypeList.AddRange(mappedtypesList);



            foreach (var library in runLiblist)
            {
                foreach (var assembly in library.Assemblies)
                {
                    var assem = Assembly.Load(assembly.Name);
                    var types = assem.GetExportedTypes().ToList();
                    var mappedtypes = types.Where(f =>
            f.GetProperties().Any(
                p =>
                p.GetCustomAttributes(false).Any(
                    a => a.GetType().Name == aliasType.Name)));

                    mappedTypeList.AddRange(mappedtypes);
                }
            }
#else


            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

             AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        
            foreach (var assembly in assemblies)
            {
                var mappedTypes = assembly.GetTypes().Where(
                 f =>
                 f.GetProperties().Any(
                     p =>
                     p.GetCustomAttributes(false).Any(
                         a => a.GetType().Name == aliasType.Name)));

                mappedTypeList.AddRange(mappedTypes);
            }
#endif

            foreach (var mappedType in mappedTypeList)
            {
                SqlMapper.SetTypeMap(mappedType, new CustomPropertyTypeMap(mappedType, _fu));
            }
        }
#if NETCOREAPP1_0
#else
        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var aliasType = typeof(DBColumnAttribute);
            var mappedTypeList = new List<Type>();
            var assembly = args.LoadedAssembly;
             
                var mappedTypes = assembly.GetTypes().Where(
                    f =>
                        f.GetProperties().Any(
                            p =>
                                p.GetCustomAttributes(false).Any(
                                    a => a.GetType().Name == aliasType.Name)));

                mappedTypeList.AddRange(mappedTypes);
           
            foreach (var mappedType in mappedTypeList)
            {
                SqlMapper.SetTypeMap(mappedType, new CustomPropertyTypeMap(mappedType, _fu));
            }
        }
#endif
        static string GetColumnAttribute(MemberInfo member)
        {
            if (member == null) return null;

#if NETCOREAPP1_0
            var attrib = member.GetCustomAttribute<DBColumnAttribute>(false);
            //var attrib = (DBColumnAttribute)Attribute.GetCustomAttribute(member, typeof(DBColumnAttribute), false);
            return attrib == null ? member.Name : attrib.Name;//if not define DBcolumn attribute on an propertity/field then use it's own name.
#else
             var attrib = (DBColumnAttribute)Attribute.GetCustomAttribute(member, typeof(DBColumnAttribute), false);
            return attrib == null ? member.Name : attrib.Name;//if not define DBcolumn attribute on an propertity/field then use it's own name.
#endif
        }

    }
}
