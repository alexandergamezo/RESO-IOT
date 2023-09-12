using System.Runtime.Serialization;

namespace Business.Validation
{
    [Serializable]
    public class SensorException : Exception
    {
        public SensorException(string message) : base(message)
        { }

        protected SensorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
