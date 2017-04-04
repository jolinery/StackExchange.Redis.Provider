using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;

namespace StackExchange.Redis.Provider.Common.Serializers
{
    public class MessagePacker:ISerializers
    {
        
		private readonly Encoding _encoding;

		public MessagePacker(Action<SerializerRepository> customSerializerRegistrar = null, Encoding encoding = null)
		{
			if (customSerializerRegistrar != null)
			{
				customSerializerRegistrar(SerializationContext.Default.Serializers);
			}

			if (encoding == null)
			{
				this._encoding = Encoding.UTF8;
			}
		}

		public Task<object> DeserializeAsync(byte[] serializedObject)
		{
			return Task.Factory.StartNew(() => Deserialize(serializedObject));
		}

		public T Deserialize<T>(byte[] serializedObject)
		{
			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType(_encoding.GetString(serializedObject), typeof(T));
			}
			var serializer = MessagePackSerializer.Get<T>();

			using (var byteStream = new MemoryStream(serializedObject))
			{
				return serializer.Unpack(byteStream);
			}
		}

		public Task<T> DeserializeAsync<T>(byte[] serializedObject)
		{
			return Task.Factory.StartNew(() => Deserialize<T>(serializedObject));
		}

		public byte[] Serialize(object item)
		{
			if (item is string)
			{
				return _encoding.GetBytes(item.ToString());
			}

			var serializer = MessagePackSerializer.Get(item.GetType());

			using (var byteStream = new MemoryStream())
			{
				serializer.Pack(byteStream, item);

				return byteStream.ToArray();
			}
		}

		public Task<byte[]> SerializeAsync(object item)
		{
			return Task.Factory.StartNew(() => Serialize(item));
		}

		public object Deserialize(byte[] serializedObject)
		{
			return Deserialize<object>(serializedObject);
		}
    }
}
