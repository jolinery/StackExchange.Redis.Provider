using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack;
using MsgPack.Serialization;

namespace StackExchange.Redis.Provider.Common.Serializers
{
    public class InterfaceMessagePacker <T>: MessagePackSerializer<T>
    {
        private readonly Dictionary<string, IMessagePackSerializer> _serializers;

        public InterfaceMessagePacker()
            : this(SerializationContext.Default)
        {
        }

        public InterfaceMessagePacker(SerializationContext context)
            : base(context)
        {
            _serializers = new Dictionary<string, IMessagePackSerializer>();

            // Get all types that implement T interface
            var implementingTypes = System.Reflection.Assembly
                .GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.ImplementedInterfaces.Contains(typeof(T)));

            // Create serializer for each type and store it in dictionary
            foreach (var type in implementingTypes)
            {
                var key = type.Name;
                var value = MessagePackSerializer.Get(type, context);
                _serializers.Add(key, value);
            }
        }

        protected override void PackToCore(Packer packer, T objectTree)
        {
            IMessagePackSerializer serializer;
            string typeName = objectTree.GetType().Name;

            // Find matching serializer
            if (!_serializers.TryGetValue(typeName, out serializer))
            {
                throw SerializationExceptions.NewTypeCannotSerialize(typeof(T));
            }

            packer.PackArrayHeader(2);             // Two-element array:
            packer.PackString(typeName);           //  0: Type name
            serializer.PackTo(packer, objectTree); //  1: Packed object
        }

        protected override T UnpackFromCore(Unpacker unpacker)
        {
            IMessagePackSerializer serializer;
            string typeName;

            // Read type name and packed object
            if (!(unpacker.ReadString(out typeName) && unpacker.Read()))
            {
                throw SerializationExceptions.NewUnexpectedEndOfStream();
            }

            // Find matching serializer
            if (!_serializers.TryGetValue(typeName, out serializer))
            {
                throw SerializationExceptions.NewTypeCannotDeserialize(typeof(T));
            }

            // Unpack and return
            return (T)serializer.UnpackFrom(unpacker);
        }
    }
}
