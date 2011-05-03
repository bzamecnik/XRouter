using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using ObjectRemoter;

namespace ObjectRemoter.Test
{
    public class MarshallingTest
    {
        #region Happy-day test methods

        [Fact]
        public void MarshalAndUnmarshallString()
        {
            MarshalAndUnmarshall<string>("foo bar");
        }

        [Fact]
        public void MarshalAndUnmarshallPrimitiveInt32()
        {
            MarshalAndUnmarshall<Int32>(42);
        }

        [Fact]
        public void MarshalAndUnmarshallPrimitiveDouble()
        {
            MarshalAndUnmarshall<double>(3.1415926535);
        }

        [Fact]
        public void MarshalAndUnmarshallPrimitiveDoubleNegativeInfinity()
        {
            MarshalAndUnmarshall<double>(double.NegativeInfinity);
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfStrings()
        {
            MarshalAndUnmarshall(new[] { "foo", "bar", "baz" });
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfInt32()
        {
            MarshalAndUnmarshall(new[] { 654321354, 061506, 0, -546510 });
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfDouble()
        {
            MarshalAndUnmarshall(new[] {
                65432.1354, 0.123456, 1.1e5,
                double.NaN, double.NegativeInfinity });
        }

        [Fact]
        public void MarshalAndUnmarshallEmptyArrayOfString()
        {
            MarshalAndUnmarshall(new string[] { });
        }

        [Fact]
        public void MarshalAndUnmarshallAnonymousMethod()
        {
            System.Func<string> originalDelegate = () => string.Format("PI is {0}", 3.14);
            Type type = typeof(System.Func<string>);
            string marshalled = Marshalling.Marshal(originalDelegate, type);
            var unmarshalledDelegate = (System.Func<string>)Marshalling.Unmarshal(marshalled, type);
            string resultFromOriginal = originalDelegate();
            string resultFromUnmarshalled = unmarshalledDelegate();
            Assert.Equal(resultFromOriginal, resultFromUnmarshalled);
        }

        [Fact]
        public void MarshalAndUnmarshallSerializableWithColon()
        {
            System.Func<string> originalDelegate = () => string.Format("PI: {0}", 3.14);
            Type type = typeof(System.Func<string>);
            string marshalled = Marshalling.Marshal(originalDelegate, type);
            var unmarshalledDelegate = (System.Func<string>)Marshalling.Unmarshal(marshalled, type);
            string resultFromOriginal = originalDelegate();
            string resultFromUnmarshalled = unmarshalledDelegate();
            Assert.Equal(resultFromOriginal, resultFromUnmarshalled);
        }

        [Fact]
        public void MarshalNullObject()
        {
            Assert.DoesNotThrow(() =>
               Marshalling.Marshal(null, typeof(object))
            );
        }

        // TODO:
        // - object types:
        //   - structs, class instances, object, null
        //   - IRemotelyCloneable, IRemotelyReferable
        //   - delegates, System.Void, [Serializable]
        //   - arrays of various types
        //   - more primitives: bool, ...
        // - type:
        //   - object

        #endregion

        #region Bad-day test methods

        [Fact]
        public void MarshalWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Marshal("foo bar", null)
            );
        }

        [Fact]
        public void UnmarshalWithNullMarshalledString()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Unmarshal(null, typeof(string))
            );
        }

        [Fact]
        public void UnmarshalWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Unmarshal("foo bar", null)
            );
        }

        // TODO:
        // - marshal
        // - unmarshal
        //   - invalid strings, unsupported types

        #endregion

        #region Helper methods

        private static void MarshalAndUnmarshall<T>(T original)
        {
            Type type = typeof(T);
            string marshalled = Marshalling.Marshal(original, type);
            object result = Marshalling.Unmarshal(marshalled, type);
            Assert.Equal(original, result);
        }

        #endregion
    }
}
