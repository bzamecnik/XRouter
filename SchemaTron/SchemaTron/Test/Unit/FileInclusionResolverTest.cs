namespace SchemaTron.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SchemaTron;
    using Xunit;

    public class FileInclusionResolverTest
    {
        // Note: FileInclusionResolver is internal and access to it
        // must have been allowed explicitly.

        [Fact]
        public void ResolveNullHref()
        {
            FileInclusionResolver resolver = new FileInclusionResolver();
            Assert.Throws<ArgumentNullException>(() => resolver.Resolve(null));
        }
    }
}
