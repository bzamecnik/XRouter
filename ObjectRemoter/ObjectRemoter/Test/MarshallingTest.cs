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
        public void MarshalAndUnmarshallBool()
        {
            MarshalAndUnmarshall<bool>(true);
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfStrings()
        {
            MarshalAndUnmarshallArray(new[] { "foo", "bar", "baz" });
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfInt32()
        {
            MarshalAndUnmarshallArray(new[] { 654321354, 061506, 0, -546510 });
        }

        [Fact]
        public void MarshalAndUnmarshallArrayOfDouble()
        {
            MarshalAndUnmarshallArray(new[] {
                65432.1354, 0.123456, 1.1e5,
                double.NaN, double.NegativeInfinity });
        }

        [Fact]
        public void MarshalAndUnmarshallEmptyArrayOfString()
        {
            MarshalAndUnmarshallArray(new string[] { });
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
        public void MarshalAndUnmarshallObject()
        {
            Type type = typeof(object);
            object original = new object();
            string marshalled = null;
            Assert.DoesNotThrow(() => marshalled = Marshalling.Marshal(original, type));
            object result = null;
            Assert.DoesNotThrow(() => result = Marshalling.Unmarshal(marshalled, type));
            Assert.NotNull(result);

            // NOTE: The unmarshalled object might have different hash code,
            // even when both objects are equal in fact. An object with a
            // proper implementation of Equals() should be used.
        }

        [Fact]
        public void MarshalAndUnmarshallNullObject()
        {
            MarshalAndUnmarshall<object>(null);
        }

        [Fact]
        public void MarshalAndUnmarshallNullString()
        {
            MarshalAndUnmarshall<string>(null);
        }

        [Fact]
        public void MarshalNullIRemotelyReferable()
        {
            MarshalAndUnmarshall<IRemotelyReferable>(null);
        }

        [Fact]
        public void MarshalNullIRemotelyCloneable()
        {
            MarshalAndUnmarshall<IRemotelyCloneable>(null);
        }

        [Fact]
        public void MarshalNullDelegate()
        {
            MarshalAndUnmarshall<Delegate>(null);
        }

        [Fact]
        public void MarshalAndUnmarshallSerializable()
        {
            MarshalAndUnmarshall(new SampleSerializable("foo", 42,
                new SampleSerializable("bar", 13, null)));
        }

        [Fact]
        public void MarshalAndUnmarshallRemotelyReferable()
        {
            var original = new SampleRemotelyReferable("foo",
                new SampleRemotelyReferable("bar", null));
            var comparer = new SampleRemotelyReferableEquivalenceComparer();
            MarshalAndUnmarshall<ISampleRemotelyReferable>(original, comparer);
        }

        // TODO:
        // - object types:
        //   - structs, class instances
        //   - IRemotelyCloneable
        //   - delegates, System.Void
        //   - arrays of various types
        //   - more primitives
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
        public void UnmarshalWithNullType()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Unmarshal("foo bar", null)
            );
        }

        [Fact]
        public void UnmarshalNullMarshalledString()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Marshalling.Unmarshal(null, typeof(string))
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
        private static void MarshalAndUnmarshall<T>(T original, IEqualityComparer<T> comparer)
        {
            Type type = typeof(T);
            string marshalled = Marshalling.Marshal(original, type);
            object result = Marshalling.Unmarshal(marshalled, type);
            Assert.True(type.IsAssignableFrom(result.GetType()));
            Assert.Equal<T>((T)original, (T)result, comparer);
        }

        private static void MarshalAndUnmarshallArray<T>(T[] original)
        {
            Type type = typeof(T[]);
            string marshalled = Marshalling.Marshal(original, type);
            object result = Marshalling.Unmarshal(marshalled, type);
            Assert.True(result.GetType().IsArray);
            T[] convertedResult = Array.ConvertAll((object[])result, item => (T)item);
            Assert.Equal(original, convertedResult);
        }

        #endregion

        public interface ISampleRemotelyReferable : IRemotelyReferable
        {
            string Name { get; set; }
            ISampleRemotelyReferable Next { get; set; }
        }

        public class SampleRemotelyReferable : ISampleRemotelyReferable
        {
            public string Name { get; set; }
            public ISampleRemotelyReferable Next { get; set; }

            public SampleRemotelyReferable(string Name, ISampleRemotelyReferable Next)
            {
                this.Name = Name;
                this.Next = Next;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                //       
                // See the full list of guidelines at
                //   http://go.microsoft.com/fwlink/?LinkID=85237  
                // and also the guidance for operator== at
                //   http://go.microsoft.com/fwlink/?LinkId=85238
                //

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                ISampleRemotelyReferable other = (ISampleRemotelyReferable)obj;
                return object.Equals(this.Name, other.Name) &&
                    object.Equals(this.Next, other.Next);
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                int hashCode = 0;
                if (Name != null)
                {
                    hashCode ^= 7 * Name.GetHashCode();
                }
                if (Next != null)
                {
                    hashCode ^= 17 * Next.GetHashCode();
                }
                return hashCode;
            }
        }

        class SampleRemotelyReferableEquivalenceComparer : IEqualityComparer<ISampleRemotelyReferable>
        {
            public bool Equals(ISampleRemotelyReferable x, ISampleRemotelyReferable y)
            {
                if ((x == null) && (y == null))
                {
                    return true;
                }
                bool nameEquals = object.Equals(x.Name, y.Name);
                bool nextEquals = Equals(x.Next, y.Next);
                return nameEquals && nextEquals;
            }

            public int GetHashCode(ISampleRemotelyReferable obj)
            {
                throw new NotImplementedException();
            }
        }


        //internal class SampleRemotelyCloneable : IRemotelyCloneable
        //{
        //}

        [Serializable]
        private class SampleSerializable
        {
            public string text;
            public int number;
            public SampleSerializable obj;

            public SampleSerializable(string text, int number, SampleSerializable obj)
            {
                this.text = text;
                this.number = number;
                this.obj = obj;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                //       
                // See the full list of guidelines at
                //   http://go.microsoft.com/fwlink/?LinkID=85237  
                // and also the guidance for operator== at
                //   http://go.microsoft.com/fwlink/?LinkId=85238
                //

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                SampleSerializable other = (SampleSerializable)obj;
                return object.Equals(this.text, other.text) &&
                    (this.number == other.number) && object.Equals(this.obj, other.obj);
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                int hashCode = 13 * number;
                if (text != null)
                {
                    hashCode ^= 7 * text.GetHashCode();
                }
                if (obj != null)
                {
                    hashCode ^= 17 * obj.GetHashCode();
                }
                return hashCode;
            }
        }
    }
}
