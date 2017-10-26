using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Sharpen
{
	using System;
	using System.IO;

	internal class ObjectOutputStream : OutputStream
	{
        public void WriteObject (ISerializable ser)
        {
            var serializer = new BinaryFormatter();
            serializer.Serialize(bw.BaseStream, ser);
        }

		private BinaryWriter bw;

		public ObjectOutputStream (OutputStream os)
		{
			this.bw = new BinaryWriter (os.GetWrappedStream ());
		}

		public virtual void WriteInt (int i)
		{
			this.bw.Write (i);
		}
	}
}
