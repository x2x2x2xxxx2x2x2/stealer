using System;
using System.IO;

namespace Helper
{
	public static class ParseKeyBlob
	{
	  public static BlobParsedData Parse(byte[] blobData)
	  {
	    using (MemoryStream input = new MemoryStream(blobData))
	    {
	      using (BinaryReader binaryReader = new BinaryReader((Stream) input))
	      {
	        uint count = binaryReader.ReadUInt32();
	        binaryReader.ReadBytes((int) count);
	        int num1 = (int) binaryReader.ReadUInt32();
	        long position = input.Position;
	        byte[] numArray1 = (byte[]) null;
	        byte[] numArray2 = (byte[]) null;
	        byte[] numArray3 = (byte[]) null;
	        if (num1 == 32 /*0x20*/)
	        {
	          byte[] numArray4 = binaryReader.ReadBytes(32 /*0x20*/);
	          return new BlobParsedData()
	          {
	            Flag = 32 /*0x20*/,
	            Iv = numArray1,
	            Ciphertext = numArray2,
	            Tag = numArray3,
	            EncryptedAesKey = numArray4
	          };
	        }
	        byte num2 = binaryReader.ReadByte();
	        switch (num2)
	        {
	          case 1:
	          case 2:
	            byte[] numArray5 = binaryReader.ReadBytes(12);
	            byte[] numArray6 = binaryReader.ReadBytes(32 /*0x20*/);
	            byte[] numArray7 = binaryReader.ReadBytes(16 /*0x10*/);
	            return new BlobParsedData()
	            {
	              Flag = num2,
	              Iv = numArray5,
	              Ciphertext = numArray6,
	              Tag = numArray7,
	              EncryptedAesKey = (byte[]) null
	            };
	          case 3:
	          case 35:
	            byte[] numArray8 = binaryReader.ReadBytes(32 /*0x20*/);
	            byte[] numArray9 = binaryReader.ReadBytes(12);
	            byte[] numArray10 = binaryReader.ReadBytes(32 /*0x20*/);
	            byte[] numArray11 = binaryReader.ReadBytes(16 /*0x10*/);
	            return new BlobParsedData()
	            {
	              Flag = num2,
	              Iv = numArray9,
	              Ciphertext = numArray10,
	              Tag = numArray11,
	              EncryptedAesKey = numArray8
	            };
	          default:
	            throw new Exception();
	        }
	      }
	    }
	  }
	}
}
