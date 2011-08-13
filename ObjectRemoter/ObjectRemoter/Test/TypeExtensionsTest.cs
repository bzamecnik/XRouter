namespace ObjectRemoter.Test
{
    using System;
    using ObjectRemoter;
    using Xunit;

    public class TypeExtensionsTest
    {
        #region #region Happy-day tests

        [Fact(Skip="Need to find a suitable assembly.")]
        public void GetTypeThisClass()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // TODO: Provide an assembly which is not already loaded
            // and load a type from it. We must know its full name
            // before loading it.
            string assemblyFullName = typeof(TypeExtensionsTest).Assembly.FullName;
            Type loadedType = TypeExtensions.GetType(assemblies,
                string.Format("{0}!TypeExtensionsTest", assemblyFullName));

            Assert.NotNull(loadedType);
            Assert.Equal(typeof(TypeExtensionsTest), loadedType);
        }

        #endregion

        #region Bad-day tests

        [Fact]
        public void GetTypeTypeAndAssemblyFullNameNull()
        {
            var assemblies = GetGoodAssemblies();
            Assert.Throws<ArgumentNullException>(
                () => TypeExtensions.GetType(assemblies, null));
        }

        [Fact]
        public void GetTypeTypeAndAssemblyFullNameTooMuchParts()
        {
            var assemblies = GetGoodAssemblies();
            Assert.Throws<ArgumentException>(
                () => TypeExtensions.GetType(assemblies, "assembly!type!foo"));
        }

        [Fact]
        public void GetTypeTypeAndAssemblyFullNameTooLittleParts()
        {
            var assemblies = GetGoodAssemblies();
            Assert.Throws<ArgumentException>(
                () => TypeExtensions.GetType(assemblies, "assembly"));
        }

        [Fact]
        public void GetTypeLoadNonExistingAssembly()
        {
            var assemblies = GetGoodAssemblies();
            Assert.Throws<System.IO.FileNotFoundException>(
                () => TypeExtensions.GetType(assemblies, "Nonexistent.Assembly.dll!type"));
        }

        #endregion

        private static System.Reflection.Assembly[] GetGoodAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}