using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace H2DT.Serializing
{
    public class Vector2SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector2 vector = (Vector2)obj;
            info.AddValue("x", vector.x);
            info.AddValue("y", vector.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector2 vector = (Vector2)obj;

            vector.x = (float)info.GetValue("x", typeof(float));
            vector.y = (float)info.GetValue("y", typeof(float));

            obj = vector;

            return obj;
        }
    }
}
