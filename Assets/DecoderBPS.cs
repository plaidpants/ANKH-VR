using System.IO;
using System.Text;
using UnityEngine;

public class DecoderBSP : MonoBehaviour
{
	public const int MIN_PATCH_SIZE = 19;
	
	public enum PATCH_ACTION : byte
	{
		SOURCE_READ = 0,
		TARGET_READ = 1,
		SOURCE_COPY = 2,
		TARGET_COPY = 3
	};
	public static ulong decodePatch(ref FileStream patch)
	{
		// decode patch data
		ulong data = 0;
		ulong shift = 1;

		while (true)
		{
			int x = patch.ReadByte();

			data += (ulong)(x & 0x7f) * shift;
			if ((x & 0x80) != 0)
			{
				break;
			}

			shift <<= 7;
			data += shift;
		}

		return data;
	}

	public static void ApplyPatch(FileInfo sourceFile, FileInfo patchFile, FileInfo targetFile)
	{
		// Check patch size
		if (patchFile.Length < MIN_PATCH_SIZE)
		{
			Debug.Log("patch file size too small");
			return;
		}

		// Get streams for the source file and patch file
		FileStream sourceFileStream = sourceFile.OpenRead();
		FileStream patchFileStream = patchFile.OpenRead();

		// Check patch is a BPS version 1 patch file
		if ((patchFileStream.ReadByte() != 'B') ||
			(patchFileStream.ReadByte() != 'P') || 
			(patchFileStream.ReadByte() != 'S') ||
			(patchFileStream.ReadByte() != '1'))
		{
			Debug.Log("patch file header invalid");
			return;
		}

		if ((long)decodePatch(ref patchFileStream) != sourceFile.Length)
		{
			Debug.Log("source size mismatch");
			return;
		}

		// get the target file size
		uint targetSize = (uint)decodePatch(ref patchFileStream);

		// Create a memory buffer file stream for the target since
		// reading from the file as it is written to
		// can causes issues due to unflushed filesystem data
		byte [] targetData = new byte[targetSize];
		MemoryStream targetMemoryStream = new MemoryStream(targetData, true);

		// Read the manifest from patch file
		int metadataSize = (int)decodePatch(ref patchFileStream);
		byte[] metadata = new byte[metadataSize];
		patchFileStream.Read(metadata, 0, metadataSize);
		string manifest = Encoding.UTF8.GetString(metadata);

		// read the crc32 of the files
		long readUntil = patchFile.Length - 12;
		long patchPosistion = patchFileStream.Position;
		patchFileStream.Position = readUntil;
		// TODO add CRC checks
		uint sourceCRC = (uint)patchFileStream.ReadByte() + 
			((uint)patchFileStream.ReadByte() << 8) + 
			((uint)patchFileStream.ReadByte() << 16) + 
			((uint)patchFileStream.ReadByte() << 24);
		uint targetCRC = (uint)patchFileStream.ReadByte() + 
			((uint)patchFileStream.ReadByte() << 8) + 
			((uint)patchFileStream.ReadByte() << 16) + 
			((uint)patchFileStream.ReadByte() << 24);
		uint patchCRC = (uint)patchFileStream.ReadByte() + 
			((uint)patchFileStream.ReadByte() << 8) + 
			((uint)patchFileStream.ReadByte() << 16) + 
			((uint)patchFileStream.ReadByte() << 24);
		patchFileStream.Position = patchPosistion;

		long sourceOffset = 0;
		long targetOffset = 0;
		while (patchFileStream.Position < readUntil)
		{
			int data = (int)decodePatch(ref patchFileStream);
			PATCH_ACTION mode = (PATCH_ACTION)(data & 3);
			int length = (data >> 2) + 1;

			// Copy unchanged data from the source file
			if (mode == PATCH_ACTION.SOURCE_READ)
			{
				sourceFileStream.Position = targetMemoryStream.Position;
				while (length-- > 0)
				{
					int read = sourceFileStream.ReadByte();
					if (read == -1)
					{
						Debug.Log("hit end of source file unexpectedly");
						return;
					}
					targetMemoryStream.WriteByte((byte)read);
				}
			}
			// Copy from the patch file
			else if (mode == PATCH_ACTION.TARGET_READ)
			{
				patchFileStream.Read(targetData, (int)targetMemoryStream.Position, length);
				targetMemoryStream.Position += length;
			}
			else
			{
				data = (int)decodePatch(ref patchFileStream);
				int offset = ((data & 1) != 0) ? -(data >> 1) : (data >> 1);

				// Copy from another part of the source file
				if (mode == PATCH_ACTION.SOURCE_COPY)
				{
					sourceOffset += offset;
					sourceFileStream.Position = sourceOffset;
					sourceFileStream.Read(targetData, (int)targetMemoryStream.Position, length);
					targetMemoryStream.Position += length;
					sourceOffset += length;
				}
				// Copy from another part of the target file
				else
				{
					targetOffset += offset;
					// Need to copy a byte at a time as the source
					// reads may right behind the target writes
					for (int i = 0; i < length; i++)
					{
						targetMemoryStream.WriteByte(targetData[targetOffset+i]);
					}
					targetOffset += length;
				}
			}
		}

		try
		{
			FileStream targetFileStream = targetFile.OpenWrite();
			targetFileStream.Write(targetData, 0, targetData.Length);
			targetFileStream.Close();
		}
		catch
        {
			Debug.Log("DLL already in use");
        }
	}
}
