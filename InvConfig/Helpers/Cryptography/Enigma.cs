using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers.Strings;

namespace Helpers.cryptography
{
    public static class Enigma
    {
        private const string CANNOT_PROCESS_INPUT = "Cannot process this plain/clipper text";
        private const int CMaxLen = 10;  	                //Maximum length of key and text
        private static int LCW;                	                //Length of CodeWord
        public static int LenEncrypt;                	                //Length of String to be Encrypted
        public static int LenArrMatrix;                    	            //Length of Array Matrix
        private static int MP;                     	            //Matrix Position
        private static string Matrix;                  	        //Starting Matrix
        private static string mov1;                   	            //First Part of Replacement String
        private static string mov2;                   	            //Second Part of Replacement String
        public static string CodeWord;               	            //CodeWord
        private static string CWL;                   	            //CodeWord Letter
        private static string EncryptedString;         	        //String to Return for Encrypt or String to UnEncrypt for UnEncrypt
        private static string EncryptedLetter;         	        //Storage Variable for Character just Encrypted
        public static string[] strCryptMatrix = new string[95];   //Matrix Array
        public static string keyString
        {
            get
            {
                return CodeWord;

            }
            set
            {
                if (value.Length >= CMaxLen)
                {
                    CodeWord = value.Left(CMaxLen);
                }
                else
                {
                    CodeWord = value.PadRight(CMaxLen);
                }
            }
        }

        static Enigma()
        {
            int w;                  //Loop Counter to set up Matrix
            int x;                  //Loop through Matrix

            for(int i = 0; i <= 94; i++)
            {

                strCryptMatrix[i] = string.Empty;
            }
            Matrix = "8x3p5BeabcdfghijklmnoqrstuvwyzACDEFGHIJKLMNOPQRSTUVWXYZ 1246790-.#/\\!@$<>&*()[]{}';:,?=+~`^|%_";

            //' Matrix = Matrix + Chr(13)  'Add Carriage Return to Matrix
            //' Matrix = Matrix + Chr(10)  'Add Line Feed to Matrix
            //' Matrix = Matrix + Chr(34)  'Add "
            //' Unique String used to make Matrix - 8x3p5Be
            //' Unique String can be any combination that has a character only ONCE.
            //' EACH Letter in the Matrix is Input ONLY once.
            w = 1;
            LenArrMatrix = Matrix.Length;
            strCryptMatrix[1] = Matrix;
            for (x = 2; x <= LenArrMatrix; x++)
            {
                //mov1 = Left(strCryptMatrix(W), 1)           'First Character of strCryptMatrix
                mov1 = StringExtensions.Left(strCryptMatrix[w],1);

                //mov2 = Right(strCryptMatrix(W), (LAM - 1))  'All but First Character of strCryptMatrix
                mov2 = StringExtensions.Right(strCryptMatrix[w],LenArrMatrix- 1);
                strCryptMatrix[x] = mov2 + mov1;
                w = w + 1;
            }
        }
        public static string Encrypt(string p_msgText)
        {
            int x;                      //Loop counter
            int y;                      //Loop counter
            int z;                      //Loop counter
            string C2E;                 //Charactor to Encrypt
            string Str2Encrypt;         //Text from TextBox

            try
            {
                if (p_msgText.Length >= CMaxLen)
                {
                    Str2Encrypt = StringExtensions.Left(p_msgText, CMaxLen);
                }
                else
                {
                    Str2Encrypt = p_msgText.PadRight(CMaxLen);
                }
                LenEncrypt = Str2Encrypt.Length;
                LCW = keyString.Length;
                EncryptedLetter = "";
                EncryptedString = "";

                y = 1;
                for (x = 0; x < LenEncrypt; x++)
                {
                    //C2E = Mid(Str2Encrypt, x, 1)
                    C2E = StringExtensions.Mid(Str2Encrypt, x, 1);
                    //MP = InStr(1, Matrix, C2E, 0)
                    //InStr(4, SearchString, SearchChar, CompareMethod.Text)
                    MP = StringExtensions.Instr(Matrix, C2E, 0);
                    //CWL = Mid(CodeWord, y, 1)
                    CWL = StringExtensions.Mid(keyString, x, 1);

                    for (z = 1; z <= LenArrMatrix; z++)
                    {
                        //If Mid(strCryptMatrix(Z), MP, 1) = CWL Then
                        if (StringExtensions.Mid(strCryptMatrix[z], MP, 1) == CWL)
                        {
                            //EncryptedLetter = Left(strCryptMatrix(Z), 1)
                            EncryptedLetter = StringExtensions.Left(strCryptMatrix[z], 1);
                            EncryptedString = EncryptedString + EncryptedLetter;
                            break;
                        }
                    }
                    y = y + 1;
                    //If y > LCW Then y = 1
                    if (y > LCW)
                    {
                        y = 1;
                    }

                }
            }
            catch
            {
                throw new ApplicationException(CANNOT_PROCESS_INPUT);
            }
            return EncryptedString;
        }
    }
}
