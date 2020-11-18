Imports System
Imports System.Configuration
Imports System.Security.Cryptography
Imports System.IO
Imports System.Web

Namespace AWS.Modules.Warrants

    Public Class Encryptor
        Private ReadOnly TamperProofKey As String = GetKey("Warrants")

        Public Function QueryStringEncode(ByVal value As String) As String
            Return HttpUtility.UrlEncode(TamperProofStringEncode(value, TamperProofKey))
        End Function

        Public Function QueryStringEncode(ByVal value As String, key As String) As String
            Return HttpUtility.UrlEncode(TamperProofStringEncode(value, key))
        End Function

        Public Function StringEncode(ByVal value As String, key As String) As String
            Return TamperProofStringEncode(value, key)
        End Function

        Public Function QueryStringDecode(ByVal value As String) As String
            Return TamperProofStringDecode(value, TamperProofKey)
        End Function

        Public Function QueryStringDecode(ByVal value As String, key As String) As String
            Return TamperProofStringDecode(value, key)
        End Function

        Private Function TamperProofStringEncode(ByVal value As String,
                                 ByVal key As String) As String
            Dim mac3des As New System.Security.Cryptography.MACTripleDES()
            Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider()
            mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key))
            Return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value)) & "-"c & Convert.ToBase64String(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)))
        End Function

        Private Function TamperProofStringDecode(ByVal value As String,
                  ByVal key As String) As String
            Dim dataValue As String = ""
            Dim calcHash As String = ""
            Dim storedHash As String = ""

            Dim mac3des As New System.Security.Cryptography.MACTripleDES()
            Dim md5 As New System.Security.Cryptography.MD5CryptoServiceProvider()
            mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key))

            Try
                dataValue = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Split("-"c)(0)))
                storedHash = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value.Split("-"c)(1)))
                calcHash = System.Text.Encoding.UTF8.GetString(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dataValue)))

                If storedHash <> calcHash Then
                    'Data was corrupted

                    Throw New ArgumentException("Hash value does not match")
                    'This error is immediately caught below
                End If
            Catch ex As Exception
                Throw New ArgumentException("Invalid TamperProofString")
            End Try

            Return dataValue

        End Function

        Private Function GetKey(ByVal key As String) As String
            Return key.Remove(0, Int32.Parse(key.Length / 2))
        End Function

        Public Function EncryptStream(ByVal input As Byte()) As Byte()
            Dim rijn As New RijndaelManaged()
            Dim encrypted As Byte()
            Dim key As Byte() = New Byte() {&H22, &HC0, &H6D, &HCB, &H23, &HA6,
             &H3, &H1B, &H5A, &H1D, &HD3, &H9F,
             &H85, &HD, &HC1, &H72, &HED, &HF4,
             &H54, &HE6, &HBA, &H65, &HC, &H22,
             &H62, &HBE, &HF3, &HEC, &H14, &H81,
             &HA8, &HA}
            '32
            Dim IV As Byte() = New Byte() {&H43, &HB1, &H93, &HB, &H1A, &H87,
             &H52, &H62, &HFB, &H8, &HD, &HC0,
             &HCA, &H40, &HC2, &HDB}
            '16
            'Get an encryptor.
            Dim encryptor As ICryptoTransform = rijn.CreateEncryptor(key, IV)

            'Encrypt the data.
            Dim msEncrypt As New MemoryStream()
            Dim csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)


            'Write all data to the crypto stream and flush it.
            csEncrypt.Write(input, 0, input.Length)
            csEncrypt.FlushFinalBlock()

            'Get encrypted array of bytes.
            encrypted = msEncrypt.ToArray()

            Return encrypted

        End Function

        Public Function DecryptStream(ByVal input As Byte()) As Byte()
            Dim rijn As New RijndaelManaged()
            Dim decrypted As Byte()
            Dim key As Byte() = New Byte() {&H22, &HC0, &H6D, &HCB, &H23, &HA6,
             &H3, &H1B, &H5A, &H1D, &HD3, &H9F,
             &H85, &HD, &HC1, &H72, &HED, &HF4,
             &H54, &HE6, &HBA, &H65, &HC, &H22,
             &H62, &HBE, &HF3, &HEC, &H14, &H81,
             &HA8, &HA}
            '32
            Dim IV As Byte() = New Byte() {&H43, &HB1, &H93, &HB, &H1A, &H87,
             &H52, &H62, &HFB, &H8, &HD, &HC0,
             &HCA, &H40, &HC2, &HDB}
            '16 


            'Get a decryptor that uses the same key and IV as the encryptor.
            Dim decryptor As ICryptoTransform = rijn.CreateDecryptor(key, IV)

            'Now decrypt the previously encrypted message using the decryptor
            ' obtained in the above step.
            Dim msDecrypt As New MemoryStream(input)
            Dim csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)

            decrypted = New Byte(input.Length - 1) {}

            'Read the data out of the crypto stream.
            csDecrypt.Read(decrypted, 0, decrypted.Length)

            Return decrypted
        End Function
    End Class
End Namespace