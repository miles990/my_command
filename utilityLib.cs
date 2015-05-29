// async (input) =>
// {
// 	return ".NET welcomes " + input.ToString();
// }
using System.Threading.Tasks;

using System;
using System.IO;
using System.Security.Cryptography;

class TestClass
{
	public static string Encode(string original)
	{
		//string original = "ABC 這是測試用字串 !!! Gevin 123";

		string password = "70OMG444999";//自己設定的密碼
		byte[] SALT = new byte[] { 0x25, 0x6c, 0x6f, 0x04, 0x6d, 0x6d, 0x5a, 0x6e, 0x67, 0x6e, 0x02, 0x6f, 0x3d, 0x07, 0x21, 0x3c };//打亂密碼的東西 byte array 內容可改 長度不可改變

		Rijndael myRijndael = Rijndael.Create();

		Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
		myRijndael.Key = pdb.GetBytes(32);
		myRijndael.IV = pdb.GetBytes(16);

		byte[] encrypted;
		// Create an Rijndael object
		// with the specified key and IV.
		using (Rijndael rijAlg = Rijndael.Create())
		{
				rijAlg.Key = myRijndael.Key;
				rijAlg.IV = myRijndael.IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

				// Create the streams used for encryption.
				using (MemoryStream msEncrypt = new MemoryStream())
				{
						using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						{
								using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
								{

										//Write all data to the stream.
										swEncrypt.Write(original);
								}
								encrypted = msEncrypt.ToArray();
						}
				}
		}

		//編成 Base64 字串
		string strBase64  = Convert.ToBase64String(encrypted);

		strBase64 = strBase64.Replace("+", "@");
		strBase64 = strBase64.Replace("/", "$");

		return strBase64;
	}

	public static string Decode(string strBase64)
  {
    string password = "70OMG444999";//自己設定的密碼
    byte[] SALT = new byte[] { 0x25, 0x6c, 0x6f, 0x04, 0x6d, 0x6d, 0x5a, 0x6e, 0x67, 0x6e, 0x02, 0x6f, 0x3d, 0x07, 0x21, 0x3c };//打亂密碼的東西 byte array 內容可改 長度不可改變

    Rijndael myRijndael = Rijndael.Create();

    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
    myRijndael.Key = pdb.GetBytes(32);
    myRijndael.IV = pdb.GetBytes(16);

    //////////解碼
    strBase64 = strBase64.Replace("@", "+");
    strBase64 = strBase64.Replace("$", "/");

    //base64 轉 byte
    byte[] cipherText = Convert.FromBase64String(strBase64);

    string decodeText = null;

    // Create an Rijndael object
    // with the specified key and IV.
    using (Rijndael rijAlg = Rijndael.Create())
    {
        rijAlg.Key = myRijndael.Key;
        rijAlg.IV = myRijndael.IV;

        // Create a decrytor to perform the stream transform.
        ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

        // Create the streams used for decryption.
        using (MemoryStream msDecrypt = new MemoryStream(cipherText))
        {
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {

                    // Read the decrypted bytes from the decrypting stream
                    // and place them in a string.
                    decodeText = srDecrypt.ReadToEnd();
                }
            }
        }

    }

    return decodeText;
  }


	public static void Main(string[] args)
	{
			// Display the number of command line arguments:
			if(args.Length > 0){
				string cmd = args[0];
				// System.Console.WriteLine(cmd);
				switch(cmd){
					case "encode":
					  if(args.Length == 2){
							string encodeMsg = Encode(args[1]);
							System.Console.Write(encodeMsg);
							// System.Console.WriteLine(encodeMsg);
						}
					break;

					case "decode":
						if(args.Length == 2){
							string decodeMsg = Decode(args[1]);
							System.Console.Write(decodeMsg);
							// System.Console.WriteLine(decodeMsg);
						}
					break;
				}
			}

			//
			// string encodeMsg = Encode("hello world");
			// System.Console.WriteLine(encodeMsg);
			// string decodeMsg = Decode(encodeMsg);
			// System.Console.WriteLine(decodeMsg);
			// System.Console.WriteLine("hello world");
			// return "asdf";
	}
}

//
// using System.Runtime.InteropServices;
//
// public class Win32 {
//     [DllImport("testCsharp.dll", EntryPoint="MessageBoxA")]
//     public static extern int MsgBox(int hWnd, String text, String caption,
//                                     uint type);
// }
