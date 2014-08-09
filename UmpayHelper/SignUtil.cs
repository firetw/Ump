using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Security;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;

namespace Umpay.Hjdl
{


    /// <summary>
    /// SignUtil 的摘要说明
    /// </summary>
    /// <summary>
    /// haojinghua@umpay.com
    /// 2008-03-03
    /// </summary>
    public class SignUtil
    {
        protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string prikeyPath = AppDomain.CurrentDomain.BaseDirectory + "/Config/6882_HengJiuDongLi.key.der";//@"D:\公司文档\商户证书\testMer\testMer.key.der";
        private static string pubkeyPath = AppDomain.CurrentDomain.BaseDirectory + "/Config/cert_2d59.crt"; //@"D:\公司文档\商户证书\testUmpay\testUmpay.cert.crt";

        public SignUtil()
        {
        }
        //用私钥生成签名
        public static string sign(string unsign)
        {
            string prikey = DecodeDERKey(prikeyPath);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.FromXmlString(prikey);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return "";
            }
            // 加密对象 
            RSAPKCS1SignatureFormatter f = new RSAPKCS1SignatureFormatter(rsa);
            f.SetHashAlgorithm("SHA1");
            //把要签名的源串转化成字节数组
            byte[] source = System.Text.UnicodeEncoding.Default.GetBytes(unsign);
            SHA1Managed sha = new SHA1Managed();
            //对签名源串做哈昔算法,为sha1.
            byte[] result = sha.ComputeHash(source);
            string s = Convert.ToBase64String(result);
            Console.WriteLine(s);
            //对哈昔算法后的字符串进行签名.
            byte[] b = f.CreateSignature(result);
            //生长签名对象sign
            string sign = Convert.ToBase64String(b);
            return sign;

        }
        //用公钥进行签名的验证
        public static bool verify(string unsign, string sign)
        {
            string pubkey = pathToXMLKey(pubkeyPath);
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubkey);
            //实例化验签对象.
            RSAPKCS1SignatureDeformatter f2 = new RSAPKCS1SignatureDeformatter(rsa);
            //设置哈昔算法为sha1
            f2.SetHashAlgorithm("SHA1");
            //把签名串转化为字节数组
            byte[] key = Convert.FromBase64String(sign);

            SHA1Managed sha2 = new SHA1Managed();
            //计算源串的哈昔值,生成字节数组.
            byte[] name = sha2.ComputeHash(System.Text.UnicodeEncoding.Default.GetBytes(unsign));
            //通过字节数组生成哈昔值
            string s2 = Convert.ToBase64String(name);
            Console.WriteLine(s2);
            return f2.VerifySignature(name, key);
        }
        //  获取der格式的私钥,转化成xml格式的私钥.
        private static string DecodeDERKey(String filename)
        {
            string xmlprivatekey = null;
            RSACryptoServiceProvider rsa = null;
            byte[] keyblob = GetFileBytes(filename);
            if (keyblob == null)
                xmlprivatekey = "";
            rsa = DecodeRSAPrivateKey(keyblob);
            if (rsa != null)
            {
                xmlprivatekey = rsa.ToXmlString(true);
            }
            return xmlprivatekey;

        }
        //------- Parses binary ans.1 RSA private key; returns RSACryptoServiceProvider  ---
        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem, Encoding.ASCII);   //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;

            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)	//version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;

                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);
                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);
                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return null;
            }
            finally { binr.Close(); }
        }
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
                {
                    highbyte = binr.ReadByte();	// data size in next 2 bytes
                    lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;		// we already have the data size
                }

            while (binr.PeekChar() == 0x00)
            {	//remove high order zeros in data
                binr.ReadByte();
                count -= 1;
            }
            return count;
        }
        private static byte[] GetFileBytes(String filename)
        {
            if (!File.Exists(filename))
                return null;
            Stream stream = new FileStream(filename, FileMode.Open);
            int datalen = (int)stream.Length;
            byte[] filebytes = new byte[datalen];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(filebytes, 0, datalen);
            stream.Close();
            return filebytes;
        }
        //从x509格式的证书中,提取公钥,并转换成xml格式
        private static string pathToXMLKey(string pubkeyPath)
        {
            X509Certificate cert = null;
            string error = "";
            try
            {	 // Try loading certificate as binary DER into an X509Certificate object.
                cert = X509Certificate.CreateFromCertFile(pubkeyPath);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {	//not binary DER; try BASE64 format
                StreamReader sr = File.OpenText(pubkeyPath);
                String filestr = sr.ReadToEnd();
                sr.Close();
                StringBuilder sb = new StringBuilder(filestr);
                sb.Replace("-----BEGIN CERTIFICATE-----", "");
                sb.Replace("-----END CERTIFICATE-----", "");
                try
                {        //see if the file is a valid Base64 encoded cert
                    byte[] certBytes = Convert.FromBase64String(sb.ToString());
                    cert = new X509Certificate(certBytes);
                }
                catch (System.FormatException ex)
                {
                    error = "Not valid binary DER or Base64 X509 certificate format";
                    Console.WriteLine(error);
                    _log.Error(error + "\r\n\t" + ex);
                    return null;
                }
                catch (System.Security.Cryptography.CryptographicException ex)
                {
                    error = "Not valid binary DER or Base64 X509 certificate format";
                    Console.WriteLine(error);
                    _log.Error(error + "\r\n\t" + ex);
                    return null;
                }
            }  // end outer catch
            string xmlpublickey = null;
            byte[] modulus, exponent;
            byte[] rsakey = cert.GetPublicKey();
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(rsakey);
            BinaryReader binr = new BinaryReader(mem);
            ushort twobytes = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;
                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;
                if (twobytes == 0x8102)	//data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();	// read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte();	//advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian
                int modsize = BitConverter.ToInt32(modint, 0);
                int firstbyte = binr.PeekChar();
                if (firstbyte == 0x00)
                {	//if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();	//skip this null byte
                    modsize -= 1;	//reduce modulus buffer size by 1
                }
                modulus = binr.ReadBytes(modsize);	//read the modulus bytes
                if (binr.ReadByte() != 0x02)			//expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();		// should only need one byte for actual exponent data
                exponent = binr.ReadBytes(expbytes);
                if (binr.PeekChar() != -1)	// if there is unexpected more data, then this is not a valid asn.1 RSAPublicKey
                    return null;
                // ------- create RSACryptoServiceProvider instance and initialize with public key   -----
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                RSAParameters RSAKeyInfo = new RSAParameters();
                RSAKeyInfo.Modulus = modulus;
                RSAKeyInfo.Exponent = exponent;
                rsa.ImportParameters(RSAKeyInfo);
                xmlpublickey = rsa.ToXmlString(false);
                return xmlpublickey;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return null;
            }
            finally
            {
                binr.Close();
            }
        }
    }
}