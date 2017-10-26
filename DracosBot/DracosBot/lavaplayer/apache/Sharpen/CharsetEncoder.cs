namespace Sharpen
{
	using System;
	using System.Text;

	internal class CharsetEncoder
	{
        Encoding enc;

        internal Encoder Encoder { get { return enc.GetEncoder(); } }

		public CharsetEncoder (Encoding enc)
		{
			this.enc = enc;
		}

		public ByteBuffer Encode (CharSequence str)
		{
			return Encode (str.ToString ());
		}

		public ByteBuffer Encode (string str)
		{
			return ByteBuffer.Wrap (enc.GetBytes (str));
		}
	}
}
